using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;
using System.ComponentModel;
using JHSchool.Evaluation.Ranking;
using Aspose.Cells;
using System.IO;
using K12.Data;
using JHSchool.Evaluation;
using KaoHsiung.ClassExamScoreAvgComparison.Model;
using FISCA.Presentation.Controls;
using FISCA.Presentation;
using System.Windows.Forms;

namespace KaoHsiung.ClassExamScoreAvgComparison
{
    internal class Report
    {
        //private List<ComputeMethod> _methods;
        private List<ClassExamScoreData> _data;
        private Dictionary<string, JHCourseRecord> _courseDict;
        private JHExamRecord _exam;
        private List<string> _domains;
        private JHSchool.Evaluation.Calculation.ScoreCalculator _calc;
        /// <summary>
        /// 使用者選擇數量
        /// </summary>
        public static int _UserSelectCount = 0;

        private BackgroundWorker _worker;

        public event EventHandler GenerateCompleted;
        public event EventHandler GenerateError;

        private string SchoolName;
        private string Semester;

        //public Report(List<ClassExamScoreData> data, Dictionary<string, JHCourseRecord> courseDict, JHExamRecord exam, List<ComputeMethod> methods)
        //{
        public Report(List<ClassExamScoreData> data, Dictionary<string, JHCourseRecord> courseDict, JHExamRecord exam, List<string> domains)
        {
            _data = data;
            _courseDict = courseDict;
            _exam = exam;
            _domains = domains;
            _calc = new JHSchool.Evaluation.Calculation.ScoreCalculator(null);
            //_methods = methods;

            InitializeWorker();

            SchoolName = School.ChineseName;
            //Semester = string.Format("{0}學年度 第{1}學期", School.DefaultSchoolYear, School.DefaultSemester);
            Semester = string.Format("{0}學年度 第{1}學期", Global.UserSelectSchoolYear, Global.UserSelectSemester);
        }

        /// <summary>
        /// 初始化 BackgroundWorker
        /// </summary>
        private void InitializeWorker()
        {
            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.WorkerReportsProgress = true;
        }

        #region BackgroundWorker Events
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MsgBox.Show("產生報表時發生錯誤" + e.Error.Message);
                if (GenerateError != null)
                    GenerateError.Invoke(this, new EventArgs());
                return;
            }

            #region 儲存
            Workbook book = e.Result as Workbook;

            string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists) dir.Create();

            path = Path.Combine(path, Global.ReportName + ".xls");

            if (File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            try
            {
                book.Save(path, FileFormatType.Excel2003);
                FISCA.LogAgent.ApplicationLog.Log("成績系統.報表", "列印" + Global.ReportName, string.Format("產生{0}份" + Global.ReportName, _data.Count));

                MotherForm.SetStatusBarMessage(Global.ReportName + "產生完成");
                if (GenerateCompleted != null)
                    GenerateCompleted.Invoke(this, new EventArgs());

                if (MsgBox.Show(Global.ReportName + "產生完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(path);
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show("儲存失敗");
            }
            #endregion

            #region 大掃除
            foreach (var ced in _data)
                ced.Clear();
            #endregion
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            // 取得班級人數值
            Dictionary<string,int> ClassStudCount = new Dictionary<string,int> ();
            foreach (ClassExamScoreData ces in _data )
                if(!ClassStudCount.ContainsKey(ces.Class.ID))
                    ClassStudCount.Add(ces.Class.ID,ces.Students.Count );
                                       

            Workbook book = new Workbook();
            Workbook template = new Workbook();
            Worksheet ws;
            int rowIndex = 0;

            // 當班級數大於30使用另一張樣板
            if (_data.Count > 30 || _UserSelectCount >14)
            {
                book.Open(new MemoryStream(Resource1.班級評量成績平均比較表60));
                ws = book.Worksheets[0];                
                template.Open(new MemoryStream(Resource1.班級評量成績平均比較表60));
            }
            else
            {
                book.Open(new MemoryStream(Resource1.班級評量成績平均比較表));
                ws = book.Worksheets[0];                
                template.Open(new MemoryStream(Resource1.班級評量成績平均比較表));
            }

            Range all = template.Worksheets.GetRangeByName("All");

            //Range printDate = template.Worksheets.GetRangeByName("PrintDate");
            //printDate[0, 0].PutValue(DateTime.Today.ToString("yyyy/MM/dd"));

            Range title = template.Worksheets.GetRangeByName("Title");
            //Range feedback = template.Worksheets.GetRangeByName("Feedback");
            Range rowHeaders = template.Worksheets.GetRangeByName("RowHeaders");
            Range columnHeaders = template.Worksheets.GetRangeByName("ColumnHeaders");
            //Range rankColumnHeader = template.Worksheets.GetRangeByName("RankColumnHeader");

            int RowNumber = all.RowCount;
            int DataRowNumber = rowHeaders.RowCount;

            //_data.Sort(delegate(ClassExamScoreData x, ClassExamScoreData y)
            //{
            //    return x.Class.Name.CompareTo(y.Class.Name);
            //});

            int dataRowIndex = rowHeaders.FirstRow;

            List<string> headers = new List<string>();
            Dictionary<string, int> colMapping = new Dictionary<string, int>();
            foreach (ClassExamScoreData ced in _data)
            {
                //ws.Cells.CreateRange(rowIndex, RowNumber, false).Copy(all);

                //int classColIndex = 0;

                foreach (string courseID in ced.ValidCourseIDs)
                {
                    JHCourseRecord course = _courseDict[courseID];
                    if (!headers.Contains(course.Subject))
                        headers.Add(course.Subject);

                    #region comment
                    //if (!colMapping.ContainsKey(GetTaggedDomain(course.Domain)) && _domains.Contains(course.Domain))
                    //{
                    //    colMapping.Add(GetTaggedDomain(course.Domain), classColIndex);
                    //    ws.Cells[columnHeaders.FirstRow + rowIndex, columnHeaders.FirstColumn + classColIndex].PutValue(course.Domain);
                    //    ws.Cells[columnHeaders.FirstRow + rowIndex, columnHeaders.FirstColumn + classColIndex].Style.Font.IsBold = true;

                    //    classColIndex++;
                    //    ws.Cells[columnHeaders.FirstRow + rowIndex, columnHeaders.FirstColumn + classColIndex].PutValue("排序");
                    //    classColIndex++;
                    //}
                    //if (!colMapping.ContainsKey(course.Subject))
                    //{
                    //    colMapping.Add(course.Subject, classColIndex);
                    //    ws.Cells[columnHeaders.FirstRow + rowIndex, columnHeaders.FirstColumn + classColIndex].PutValue(course.Subject);

                    //    classColIndex++;
                    //    ws.Cells[columnHeaders.FirstRow + rowIndex, columnHeaders.FirstColumn + classColIndex].PutValue("排序");
                    //    classColIndex++;
                    //}
                    #endregion
                }

                foreach (StudentRow row in ced.Rows.Values)
                {
                    foreach (var sce in row.RawScoreList)
                    {
                        if (sce.RefExamID != _exam.ID) continue;
                        JHCourseRecord course = _courseDict[sce.RefCourseID];

                        if (!headers.Contains(GetTaggedDomain(course.Domain)))
                            headers.Add(GetTaggedDomain(course.Domain));
                    }
                }
            }

            #region 產生 Headers
            headers.Sort(Sort);
            int classColIndex = 0;
            foreach (string each in headers)
            {
                if (IsDomain(each))
                {
                    if (_domains.Contains(GetOriginalDomain(each)))
                    {
                        colMapping.Add(each, classColIndex);
                        ws.Cells[columnHeaders.FirstRow + rowIndex, columnHeaders.FirstColumn + classColIndex].PutValue(GetOriginalDomain(each));
                        ws.Cells[columnHeaders.FirstRow + rowIndex, columnHeaders.FirstColumn + classColIndex].Style.Font.IsBold = true;
                        classColIndex++;
                        ws.Cells[columnHeaders.FirstRow + rowIndex, columnHeaders.FirstColumn + classColIndex].PutValue("排序");
                        classColIndex++;
                    }
                }
                else
                {
                    colMapping.Add(each, classColIndex);
                    ws.Cells[columnHeaders.FirstRow + rowIndex, columnHeaders.FirstColumn + classColIndex].PutValue(each);
                    classColIndex++;
                    ws.Cells[columnHeaders.FirstRow + rowIndex, columnHeaders.FirstColumn + classColIndex].PutValue("排序");
                    classColIndex++;
                }
            }
            #endregion

            foreach (ClassExamScoreData ced in _data)
            {
                Dictionary<string, decimal?> subjectScores = new Dictionary<string, decimal?>();
                Dictionary<string, int> subjectCount = new Dictionary<string, int>();

                foreach (StudentRow row in ced.Rows.Values)
                {
                    foreach (var sce in row.RawScoreList)
                    {
                        if (sce.RefExamID != _exam.ID) continue;
                        JHCourseRecord course = _courseDict[sce.RefCourseID];

                        string ds = GetDomainSubjectKey(course.Domain, course.Subject);
                        if (!subjectScores.ContainsKey(ds))
                        {
                            subjectScores.Add(ds, 0);
                            subjectCount.Add(ds, 0);
                        }
                        if (sce.Score.HasValue)
                        {
                            subjectCount[ds]++;
                            decimal value = decimal.Zero;
                            if (row.StudentScore.CalculationRule != null)
                                value = row.StudentScore.CalculationRule.ParseSubjectScore(sce.Score.Value);
                            else
                                value = _calc.ParseSubjectScore(sce.Score.Value);
                            subjectScores[ds] += value;
                        }
                    }
                }

                foreach (string ds in new List<string>(subjectScores.Keys))
                {
                    if (subjectScores[ds].HasValue && subjectCount[ds] > 0)
                        subjectScores[ds] = subjectScores[ds].Value / (decimal)subjectCount[ds];
                }

                Dictionary<string, decimal?> domainScores = new Dictionary<string, decimal?>();
                Dictionary<string, int> domainCount = new Dictionary<string, int>();

                foreach (string ds in subjectScores.Keys)
                {
                    string domain = GetOnlyDomain(ds);
                    if (!domainScores.ContainsKey(domain))
                    {
                        domainScores.Add(domain, 0);
                        domainCount.Add(domain, 0);
                    }
                    domainCount[domain]++;
                    domainScores[domain] += subjectScores[ds];
                }

                foreach (string domain in new List<string>(domainScores.Keys))
                {
                    if (domainScores[domain].HasValue && domainCount[domain] > 0)
                        domainScores[domain] = domainScores[domain].Value / (decimal)domainCount[domain];
                }

                #region 填入班級平均
                //int rankIndex = rankColumnHeader.FirstColumn;

                // 班級平均
                Model.ClassCourseAvg cca = new KaoHsiung.ClassExamScoreAvgComparison.Model.ClassCourseAvg();

                foreach (var stud in ced.Students)
                {
                    if (ced.Rows.ContainsKey(stud.ID))
                    {
                        // 當學生沒有班級關聯不列入
                        if (stud.Class == null)
                            continue;

                        //阿寶...這也許是地雷....
                        cca.ClassID = stud.Class.ID;
                        cca.ClassName = stud.Class.Name;

                        StudentRow srow = ced.Rows[stud.ID];
                        foreach (CourseScore cs in srow.CourseScoreList)
                        {
                            if (cs.Score.HasValue)
                            {
                                //MsgBox.Show("學生：「" + srow.StudentScore.Name + "」，分數：「" + srow.StudentScore.CalculationRule.ParseSubjectScore(cs.Score.Value) + "」");
                                cca.ClassID = stud.Class.ID;
                                cca.ClassName = stud.Class.Name;
                                decimal value = decimal.Zero;
                                if (srow.StudentScore.CalculationRule != null)
                                    value = srow.StudentScore.CalculationRule.ParseSubjectScore(cs.Score.Value);
                                else
                                    value = _calc.ParseSubjectScore(cs.Score.Value);
                                cca.AddSubjectScore(_courseDict[cs.CourseID].Subject, value);
                            }
                            //decimal? s = GetRoundScore(cs.Score);
                            //if (s.HasValue)
                            //{
                            //    cca.ClassID = stud.Class.ID;
                            //    cca.ClassName = stud.Class.Name;
                            //    cca.AddCourseScore(cs.CourseID, s.Value);
                            //}
                        }
                    }
                }

                // 放班級
                ws.Cells[dataRowIndex, 0].PutValue(cca.ClassName);

                // 放班級人數
                int peoColIdx = 32;
                // 找到人數放位置
                for (int i = 32; i <= 60; i++)
                    if (ws.Cells[2, i].StringValue.Trim() == "人數")
                    {
                        peoColIdx = i;
                        break;
                    }
                if(cca.ClassID !=null && cca.ClassName !="" )
                if (ClassStudCount.ContainsKey(cca.ClassID))
                    ws.Cells[dataRowIndex, peoColIdx].PutValue(ClassStudCount[cca.ClassID].ToString());

                //decimal? value = cca.GetCourseStudScoreAvg();
                //if (value.HasValue)
                //{
                //    decimal roundValue = Math.Round(value.Value, 2, MidpointRounding.AwayFromZero);
                //    ws.Cells[dataRowIndex, colMapping[_courseDict[val.Key].Subject] + 1].PutValue(roundValue);
                //}
                foreach (KeyValuePair<string, decimal?> val in cca.GetSubjectStudScoreAvg())
                {
                    if (val.Value.HasValue)
                    {
                        decimal value = Math.Round((decimal)val.Value, 2, MidpointRounding.AwayFromZero);
                        ws.Cells[dataRowIndex, colMapping[val.Key] + 1].PutValue(value);
                    }
                }
                //填入領域
                bool domainHasData = false;
                foreach (string domain in _domains)
                {
                    if (!domainScores.ContainsKey(domain)) continue;
                    if (!domainScores[domain].HasValue) continue;

                    string taggedDomain = GetTaggedDomain(domain);
                    if (colMapping.ContainsKey(taggedDomain))
                    {
                        decimal value = Math.Round((decimal)domainScores[domain].Value, 2, MidpointRounding.AwayFromZero);
                        ws.Cells[dataRowIndex, colMapping[taggedDomain] + 1].PutValue(value);
                        domainHasData = true;
                    }
                }
                if (cca.GetSubjectStudScoreAvg().Count > 0 || domainHasData)
                    dataRowIndex++;

                #endregion

                #region 填入標題及回條
                //Ex. 新竹市立光華國民中學 97 學年度第 1 學期    第1次平時評量成績單
                ws.Cells[title.FirstRow + rowIndex, title.FirstColumn].PutValue(string.Format("{0}  {1}  {2} 各班各科平均成績比較表", SchoolName, Semester, _exam.Name));
                //Ex. 101 第1次平時評量回條 (家長意見欄)
                //ws.Cells[feedback.FirstRow + rowIndex, feedback.FirstColumn].PutValue(string.Format("{0}  {1}回條  (家長意見欄)", ced.Class.Name, _exam.Name));
                #endregion

                //rowIndex += RowNumber;
                //ws.HPageBreaks.Add(rowIndex, 0);
            }

            e.Result = book;
        }

        private string GetDomainSubjectKey(string p, string p_2)
        {
            return p + "_" + p_2;
        }

        private string GetOnlyDomain(string p)
        {
            if (p.Contains("_")) return p.Split(new string[] { "_" }, StringSplitOptions.None)[0];
            else return p;
        }

        private string GetTaggedDomain(string p)
        {
            return "領域" + p;
        }

        private string GetOriginalDomain(string p)
        {
            if (p.StartsWith("領域")) return p.Replace("領域", "");
            else return p;
        }

        private bool IsDomain(string p)
        {
            if (p.StartsWith("領域")) return true;
            else return false;
        }

        private int Sort(string x, string y)
        {
            List<string> list = new List<string>(new string[] { "國語文", "國文", "英文", "英語", "領域語文", "數學", "領域數學", "歷史", "公民", "地理", "領域社會", "領域藝術與人文", "理化", "生物", "領域自然與生活科技", "領域健康與體育", "領域綜合活動" });
            int ix = list.IndexOf(x);
            int iy = list.IndexOf(y);

            if (ix >= 0 && iy >= 0)
                return ix.CompareTo(iy);
            else if (ix >= 0)
                return -1;
            else if (iy >= 0)
                return 1;
            else
                return x.CompareTo(y);
        }
        #endregion

        /// <summary>
        /// 產生報表
        /// </summary>
        internal void Generate()
        {
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }

        //private decimal? GetRoundScore(decimal? score)
        //{
        //    if (!score.HasValue) return null;

        //    decimal seed = Convert.ToDecimal(Math.Pow(0.1, Convert.ToDouble(2)));

        //    decimal s = score.Value / seed;
        //    s = decimal.Floor(s);
        //    s *= seed;

        //    return s;

        //}
    }
}
