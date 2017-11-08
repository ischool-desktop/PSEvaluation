using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using System.IO;
using System.ComponentModel;
using JHSchool.Data;
using KaoHsiung.JHEvaluation.Data;
using System.Xml;
using FISCA.Presentation;
using FISCA.Presentation.Controls;
using System.Windows.Forms;
using KaoHsiung.MidTermTransferReport.Processor;
using JHSchool.Evaluation.Calculation;

namespace KaoHsiung.MidTermTransferReport
{
    /// <summary>
    /// 報表主要流程
    /// </summary>
    internal class Report
    {
        private Document _doc;
        private Document _template;

        private BackgroundWorker _worker;

        private Config _config;

        private List<JHStudentRecord> Students { get { return _config.Students; } }

        private EffortMapper _effortMapper;

        /// <summary>
        /// 初始化報表使用者設定
        /// </summary>
        /// <param name="config"></param>
        public Report(Config config)
        {
            _config = config;
            _effortMapper = new EffortMapper();

            _doc = new Document();
            _doc.Sections.Clear();
            _template = new Document(new MemoryStream(Properties.Resources.期中轉學成績證明書));

            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.WorkerReportsProgress = true;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MotherForm.SetStatusBarMessage(Global.ReportName + "產生中", e.ProgressPercentage);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MsgBox.Show("產生報表時發生錯誤。" + e.Error.Message);
                return;
            }

            #region 儲存
            if (_doc.Sections.Count > 0)
            {
                string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists) dir.Create();

                path = Path.Combine(path, Global.ReportName + ".doc");

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
                    _doc.Save(path);
                    FISCA.LogAgent.ApplicationLog.Log("成績系統.報表", "列印" + Global.ReportName, string.Format("產生{0}份{1}", Students.Count, Global.ReportName));

                    MotherForm.SetStatusBarMessage(Global.ReportName + "產生完成");

                    if (MsgBox.Show(Global.ReportName + "產生完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(path);
                    }
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存失敗" + ex.Message);
                }
            }
            #endregion
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            double total = Students.Count;
            double count = 0;

            _worker.ReportProgress(0);

            List<string> studentIDs = Students.Select(x => x.ID).ToList();

            #region 快取資料

            //獎勵
            // 1.依學生編號、開始日期、結束日期，取得學生獎勵紀錄
            // 2.依學生編號進行分群
            Dictionary<string, List<JHMeritRecord>> meritCache = new Dictionary<string, List<JHMeritRecord>>();
            foreach (JHMeritRecord record in JHMerit.Select(studentIDs, _config.StartDate, _config.EndDate, null, null, null, null))
            {
                if (!meritCache.ContainsKey(record.RefStudentID))
                    meritCache.Add(record.RefStudentID, new List<JHMeritRecord>());
                meritCache[record.RefStudentID].Add(record);
            }

            //懲戒
            // 1.依學生編號、開始日期、結束日期，取得學生懲戒紀錄
            // 2.依學生編號進行分群
            Dictionary<string, List<JHDemeritRecord>> demeritCache = new Dictionary<string, List<JHDemeritRecord>>();
            foreach (JHDemeritRecord record in JHDemerit.Select(studentIDs, _config.StartDate, _config.EndDate, null, null, null, null))
            {
                if (!demeritCache.ContainsKey(record.RefStudentID))
                    demeritCache.Add(record.RefStudentID, new List<JHDemeritRecord>());
                demeritCache[record.RefStudentID].Add(record);
            }

            //缺曠
            // 1.依學生編號、開始日期、結束日期，取得學生缺曠紀錄
            // 2.依學生編號進行分群
            Dictionary<string, List<JHAttendanceRecord>> attendanceCache = new Dictionary<string, List<JHAttendanceRecord>>();
            foreach (JHAttendanceRecord record in JHAttendance.Select(studentIDs, _config.StartDate, _config.EndDate, null, null, null))
            {
                if (!attendanceCache.ContainsKey(record.RefStudentID))
                    attendanceCache.Add(record.RefStudentID, new List<JHAttendanceRecord>());
                attendanceCache[record.RefStudentID].Add(record);
            }

            // 建立平時評量與修課成績Idx Key studentID
            Dictionary<string, List<JHSCAttendRecord>> _studAttendRecDict = new Dictionary<string, List<JHSCAttendRecord>>();


            //修課紀錄
            // 1.依學生編號、學年度、學期，取得修課紀錄
            // 2.
            List<string> courseIDs = new List<string>();
            List<string> attendIDs = new List<string>();
            Dictionary<string, List<string>> scCache = new Dictionary<string, List<string>>();
            foreach (var attend in JHSCAttend.Select(studentIDs, null, null, "" + _config.SchoolYear, "" + _config.Semester))
            {
                
                attendIDs.Add(attend.ID);
                if (!scCache.ContainsKey(attend.RefStudentID))
                    scCache.Add(attend.RefStudentID, new List<string>());
                scCache[attend.RefStudentID].Add(attend.RefCourseID);
                if (!courseIDs.Contains(attend.RefCourseID))
                    courseIDs.Add(attend.RefCourseID);
                
                // 建立評量與修課成績
                if (_studAttendRecDict.ContainsKey(attend.RefStudentID))
                {
                    _studAttendRecDict[attend.RefStudentID].Add(attend);
                }
                else
                {
                    List<JHSCAttendRecord> atten = new List<JHSCAttendRecord>();
                    atten.Add(attend);
                    _studAttendRecDict.Add(attend.RefStudentID, atten);
                }

            }

            //課程
            // 1.依課程編號取得課程紀錄
            // 2.略過不列入成績計算的課程
            JHCourse.RemoveByIDs(courseIDs);
            Dictionary<string, JHCourseRecord> courseCache = JHCourse.SelectByIDs(courseIDs).Where(x => x.CalculationFlag != "2").ToDictionary(x => x.ID);

            //試別資訊: 取得所有試別
            Dictionary<string, JHExamRecord> examCache = JHExam.SelectAll().ToDictionary(x => x.ID);

            // 取得社團ExamID
            List<string> NExamIDList=new List<string> ();
            foreach (JHExamRecord rec in JHExam.SelectAll())
            {
                if (rec.Name.IndexOf("社團") > -1)
                    NExamIDList.Add(rec.ID);            
            }

            //評量成績
            // 1.依修課記錄及所有試別取得評量成績
            // 2.依第1點的評量成績取得試別編號 (實際學生評量成績中的試別編號)
            // 3.依學生編號進行分群
            Dictionary<string, List<KH.JHSCETakeRecord>> sceScoreCache = new Dictionary<string, List<KH.JHSCETakeRecord>>();
            List<string> validExamIDs = new List<string>();

            // 檢查有修課才讀取成績
            if (attendIDs.Count > 0 && examCache.Count > 0)
            {
                foreach (JHSCETakeRecord record in JHSCETake.Select(null, null, examCache.Keys, null, attendIDs))
                {
                    if (!NExamIDList.Contains(record.RefExamID))
                        if (!validExamIDs.Contains(record.RefExamID))
                            validExamIDs.Add(record.RefExamID);

                    if (!sceScoreCache.ContainsKey(record.RefStudentID))
                        sceScoreCache.Add(record.RefStudentID, new List<KH.JHSCETakeRecord>());
                    sceScoreCache[record.RefStudentID].Add(new KH.JHSCETakeRecord(record));
                }
            }

            //將『所有試別編號』與『實際學生評量成績中的試別編號』做交集，以取得使用管理的試別次序
            //假設『所有試別編號』為1,4,3,5
            //假設『實際學生評量成績中的試別編號』為3,4,1
            //交集後的結果為1,4,3
            validExamIDs = examCache.Keys.Intersect(validExamIDs).ToList();

            validExamIDs.Add("平時評量");
            //validExamIDs.Add("課程總成績");

            // 取得學生成績計算規則
            // 如果學生沒有計算規則一律用預設，預設進位方式取到小數點第2位
            ScoreCalculator defaultScoreCalculator = new ScoreCalculator(null);
            //key: ScoreCalcRuleID
            Dictionary<string, ScoreCalculator> calcCache = new Dictionary<string, ScoreCalculator>();
            //key: StudentID, val: ScoreCalcRuleID
            Dictionary<string, string> calcIDCache = new Dictionary<string, string>();

            List<string> scoreCalcRuleIDList = new List<string>();

            // 取得學生計算規則的ID，建立對照表
            // 如果學生身上有指定成績計算規則，就以學生身上成績計算規則為主
            // 如果學生身上沒有指定成績計算規則，就以學生所屬班級成績計算規則為主
            foreach (JHStudentRecord student in _config.Students)
            {                
                string calcID = string.Empty;                
                if (!string.IsNullOrEmpty(student.OverrideScoreCalcRuleID))
                    calcID = student.OverrideScoreCalcRuleID;
                else if (student.Class != null && !string.IsNullOrEmpty(student.Class.RefScoreCalcRuleID))
                    calcID = student.Class.RefScoreCalcRuleID;

                if (!string.IsNullOrEmpty(calcID))
                    calcIDCache.Add(student.ID, calcID);
            }
            // 取得計算規則建立成績進位器
            foreach (JHScoreCalcRuleRecord record in JHScoreCalcRule.SelectByIDs(calcIDCache.Values))
            {
                if (!calcCache.ContainsKey(record.ID))
                    calcCache.Add(record.ID, new ScoreCalculator(record));
            }

            #endregion

            
            #region 判斷領域是否需要展開
            //判斷領域是否展開對照表
            //Key:領域名稱
            //Value:false=展開,true=不展開
            //展開: 詳列該領域下所有科目成績
            //不展開: 只列該領域成績
            Dictionary<string, bool> domains = new Dictionary<string, bool>();
            
            DomainSubjectSetup domainSubjectSetup = _config.DomainSubjectSetup;
            //使用者設定"只列印領域"
            if (domainSubjectSetup == DomainSubjectSetup.Domain)
            {
                //預設從領域資料管理來的領域名稱皆不展開
                foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                    domains.Add(domain, DomainSubjectExpand.不展開);
                //彈性課程一定展開
                if (!domains.ContainsKey("")) domains.Add("", DomainSubjectExpand.展開);

            }
            //使用者設定"只列印科目"
            else if (domainSubjectSetup == DomainSubjectSetup.Subject)
            {
                //預設從領域資料管理來的領域名稱皆展開
                foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                    domains.Add(domain, DomainSubjectExpand.展開);
                //彈性課程一定展開
                if (!domains.ContainsKey("")) domains.Add("", DomainSubjectExpand.展開);
            }
            else
                throw new Exception("請重新儲存一次列印設定");

            _config.PrintDomains = domains;
            #endregion

            #region 建立節次對照
            Dictionary<string, string> periodMapping = JHPeriodMapping.SelectAll().ToDictionary(x => x.Name, y=>y.Type);
            #endregion

            #region 假別列表
            List<string> absenceList = JHAbsenceMapping.SelectAll().Select(x => x.Name).ToList() ;
            #endregion

            #region 依評量試別重新劃分範本
            //如有不懂自求多福
            int rowCount = 0;
            DocumentBuilder templateBuilder = new DocumentBuilder(_template);
            templateBuilder.MoveToMergeField("各次評量");
            Font font = templateBuilder.Font;
            Cell examsCell = templateBuilder.CurrentParagraph.ParentNode as Cell;
            Table table = examsCell.ParentRow.ParentTable;
            double width = examsCell.CellFormat.Width;
            double examWidth = width / (double)validExamIDs.Count;
            double scoreWidth = width / (double)validExamIDs.Count / 2.0;

            //計算有幾個 Score Row
            foreach (Row row in table.Rows)
                if (row.Cells.Count > 3) rowCount++;

            #region Header Cell
            //建立評量欄位對照表
            Dictionary<string, int> columnMapping = new Dictionary<string, int>();
            int columnShift = 3;
            int columnIndex = 0;

            table.Rows[0].LastCell.Remove();
            table.Rows[1].LastCell.Remove();
           
            foreach (string examID in validExamIDs)
            {

                columnMapping.Add(examID, columnIndex + columnShift);

                Cell topHeaderCell = new Cell(_template);
                if (examID == "平時評量" || examID == "課程總成績")
                    WordHelper.Write(topHeaderCell, font, examID);
                else
                    WordHelper.Write(topHeaderCell, font, examCache[examID].Name);
                table.Rows[0].Cells.Add(topHeaderCell);

                Cell subHeaderCell1 = new Cell(_template);
                WordHelper.Write(subHeaderCell1, font, "分數", "評量");
                table.Rows[1].Cells.Add(subHeaderCell1);
                columnIndex++;

                Cell subHeaderCell2 = new Cell(_template);
                WordHelper.Write(subHeaderCell2, font, "努力", "程度");
                table.Rows[1].Cells.Add(subHeaderCell2);
                columnIndex++;

                topHeaderCell.CellFormat.Width = examWidth;
                subHeaderCell1.CellFormat.Width = subHeaderCell2.CellFormat.Width = scoreWidth;

                topHeaderCell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                subHeaderCell1.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                subHeaderCell2.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
            }

            WordHelper.MergeVerticalCell(table.Rows[0].Cells[1], 2);
            #endregion

            #region Content Cell
            int shift = 2; //Header has 2 rows
            for (int i = 0; i < rowCount; i++)
            {
                table.Rows[i + shift].LastCell.Remove();

                for (int j = 0; j < validExamIDs.Count * 2; j++)
                {
                    Cell contentCell = new Cell(_template);
                    contentCell.CellFormat.Width = scoreWidth;
                    contentCell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;

                    table.Rows[i + shift].Cells.Add(contentCell);
                }
            }
            #endregion

            #endregion

            #region 依節權數設定，在畫面上顯示
            string pcDisplay = string.Empty;
            if (_config.PrintPeriod && _config.PrintCredit)
                pcDisplay = "節/權數";
            else if (_config.PrintPeriod)
                pcDisplay = "節數";
            else if (_config.PrintCredit)
                pcDisplay = "權數";

            templateBuilder.MoveToMergeField("節權數");
            templateBuilder.Write(pcDisplay);
            #endregion
            
           // 取得學期歷程
            Config._StudSemesterHistoryItemDict.Clear();

            List<JHSemesterHistoryRecord> semHisRec = JHSemesterHistory.SelectByStudents(Students);
            // 當畫面學期歷程內的學年度學期與畫面上設定相同，加入
            foreach (JHSemesterHistoryRecord rec in semHisRec)
                foreach (K12.Data.SemesterHistoryItem shi in rec.SemesterHistoryItems)
                    if (shi.SchoolYear == _config.SchoolYear && shi.Semester == _config.Semester)
                        if (!Config._StudSemesterHistoryItemDict.ContainsKey(shi.RefStudentID))
                            Config._StudSemesterHistoryItemDict.Add(shi.RefStudentID, shi);

            // 取得學生服務學習時數
            Config._SRDict.Clear();
            List<string> sidList=(from data in Students select data.ID).ToList();            
            Config._SRDict = Utility.GetServiceLearningDetail(sidList,_config.SchoolYear,_config.Semester);


            #region 產生
            foreach (JHStudentRecord student in Students)
            {
                count++;
                    
                Document each = (Document)_template.Clone(true);
                DocumentBuilder builder = new DocumentBuilder(each);

                #region 學生基本資料
                StudentBasicInfo basicInfo = new StudentBasicInfo(builder);
                basicInfo.SetStudent(student);
                #endregion


                #region 各評量成績
                List<KH.JHSCETakeRecord> sceScoreList = null;
                if (sceScoreCache.ContainsKey(student.ID))
                    sceScoreList = sceScoreCache[student.ID];
                else
                    sceScoreList = new List<KH.JHSCETakeRecord>();

                ScoreCalculator studentCalculator = defaultScoreCalculator;
                if (calcIDCache.ContainsKey(student.ID) && calcCache.ContainsKey(calcIDCache[student.ID]))
                    studentCalculator = calcCache[calcIDCache[student.ID]];

                // 課程成績
                Dictionary<string, JHSCAttendRecord> attendRecDict = new Dictionary<string, JHSCAttendRecord>();
                // 領域成績(平時)
                Dictionary<string, decimal?> domainScDict = new Dictionary<string, decimal?>();
                //// 領域成績(總)
                //Dictionary<string, decimal> domainScDictTT = new Dictionary<string, decimal>();

                if (_studAttendRecDict.ContainsKey(student.ID))
                {
                    foreach (JHSCAttendRecord rec in _studAttendRecDict[student.ID])
                    {
                        
                        if (!attendRecDict.ContainsKey(rec.Course.Subject))
                            attendRecDict.Add(rec.Course.Subject, rec);
                    }


                    List<string> tName = (from xx in _studAttendRecDict[student.ID] select xx.Course.Domain).Distinct().ToList();

                    foreach (string Name in tName)
                    {
                        decimal? sc = 0, co = 0;
                        foreach (JHSCAttendRecord rec in _studAttendRecDict[student.ID])
                        {
                            if (rec.Course.Domain == Name)
                            {
                                if (rec.OrdinarilyScore.HasValue && rec.Course.Credit.HasValue)
                                {
                                    sc += (rec.OrdinarilyScore.Value * rec.Course.Credit.Value);
                                    // 有成績才算入
                                    co+=rec.Course.Credit.Value;                               
                                }  
                            }
                        }

                        if (co.HasValue && sc.HasValue)
                        { 
                            if(co.Value>0)
                            if(!domainScDict.ContainsKey(Name ))
                                domainScDict.Add(Name,(sc.Value/co.Value));
                        }
                    }

                }

                //StudentExamScore examScore = new StudentExamScore(builder, _config, courseCache, attendRecDict,domainScDict,domainScDictTT);
                StudentExamScore examScore = new StudentExamScore(builder, _config, courseCache, attendRecDict, domainScDict);
                if (scCache.ContainsKey(student.ID)) examScore.SetSubjects(scCache[student.ID]);
                examScore.SetColumnMap(columnMapping);
                examScore.SetEffortMapper(_effortMapper);
                examScore.SetCalculator(studentCalculator);
                examScore.SetData(sceScoreList);
                #endregion

                #region 缺曠獎懲
                List<JHMeritRecord> meritList = null;
                List<JHDemeritRecord> demeritList = null;
                List<JHAttendanceRecord> attendanceList = null;

                meritList = (meritCache.ContainsKey(student.ID)) ? meritCache[student.ID] : new List<JHMeritRecord>();
                demeritList = (demeritCache.ContainsKey(student.ID)) ? demeritCache[student.ID] : new List<JHDemeritRecord>();
                attendanceList = (attendanceCache.ContainsKey(student.ID)) ? attendanceCache[student.ID] : new List<JHAttendanceRecord>();

                StudentMoralScore moral = new StudentMoralScore(builder, _config, periodMapping, absenceList);
                moral.SetData(meritList, demeritList, attendanceList);
                #endregion

                foreach (Section sec in each.Sections)
                    _doc.Sections.Add(_doc.ImportNode(sec, true));

                //回報進度
                _worker.ReportProgress((int)(count * 100.0 / total));
            }

            #region 全域 MergeField
            List<string> globalFieldName = new List<string>();
            List<object> globalFieldValue = new List<object>();

            globalFieldName.Add("學校名稱");
            globalFieldValue.Add(K12.Data.School.ChineseName);

            globalFieldName.Add("學年度");
            globalFieldValue.Add(_config.SchoolYear.ToString());
            //globalFieldValue.Add(K12.Data.School.DefaultSchoolYear);

            globalFieldName.Add("學期");
            globalFieldValue.Add(_config.Semester.ToString());
            //globalFieldValue.Add(K12.Data.School.DefaultSemester);

            globalFieldName.Add("統計期間");
            globalFieldValue.Add(_config.StartDate.ToShortDateString() + " ~ " + _config.EndDate.ToShortDateString());

            globalFieldName.Add("列印日期");
            globalFieldValue.Add(DateConvert.CDate(DateTime.Now.ToString("yyyy/MM/dd")) + " " + DateTime.Now.ToString("HH:mm:ss"));

            string chancellor, eduDirector, stuDirector;
            chancellor = eduDirector = stuDirector = string.Empty;

            XmlElement schoolInfo = K12.Data.School.Configuration["學校資訊"].PreviousData;
            XmlElement chancellorElement = (XmlElement)schoolInfo.SelectSingleNode("ChancellorChineseName");
            XmlElement eduDirectorElement = (XmlElement)schoolInfo.SelectSingleNode("EduDirectorName");
            XmlElement stuDirectorElement = (XmlElement)schoolInfo.SelectSingleNode("StuDirectorName");

            if (chancellorElement != null) chancellor = chancellorElement.InnerText;
            if (eduDirectorElement != null) eduDirector = eduDirectorElement.InnerText;
            if (stuDirectorElement != null) stuDirector = stuDirectorElement.InnerText;

            globalFieldName.Add("校長");
            globalFieldValue.Add(chancellor);

            globalFieldName.Add("教務主任");
            globalFieldValue.Add(eduDirector);

            globalFieldName.Add("學務主任");
            globalFieldValue.Add(stuDirector);

            _doc.MailMerge.Execute(globalFieldName.ToArray(), globalFieldValue.ToArray());
            #endregion

            #endregion
        }

        internal void Generate()
        {
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }
    }
}
