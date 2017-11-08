using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using System.IO;
using System.ComponentModel;
using JHSchool.Data;
using HsinChu.JHEvaluation.Data;
using System.Xml;
using FISCA.Presentation;
using FISCA.Presentation.Controls;
using System.Windows.Forms;
using HsinChu.MidTermTransferReport.Processor;
using JHSchool.Evaluation.Calculation;
using System.Linq;

namespace HsinChu.MidTermTransferReport
{
    /// <summary>
    /// 報表主要流程
    /// </summary>
    internal class Report
    {
        private Document _doc;
        private Document _template;

        private BackgroundWorker _worker;

        public string _SchoolYear = "";
        public string _Semester = "";

        private Config _config;
//        private string ReportName { get { return _config.ReportName; } }
        private List<JHStudentRecord> Students { get { return _config.Students; } }

        /// <summary>
        /// 初始化報表使用者設定
        /// </summary>
        /// <param name="config"></param>
        public Report(Config config)
        {
            _config = config;

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

            //List<string> student_ids = new List<string>();
            //foreach (JHStudentRecord item in Students)
            //    student_ids.Add(item.ID);

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
            //Dictionary<string, List<JHMeritRecord>> meritCache = new Dictionary<string, List<JHMeritRecord>>();
            //foreach (JHMeritRecord record in JHMerit.SelectByStudentIDs(student_ids))
            //{
            //    if (record.OccurDate < _config.StartDate) continue;
            //    if (record.OccurDate > _config.EndDate) continue;

            //    if (!meritCache.ContainsKey(record.RefStudentID))
            //        meritCache.Add(record.RefStudentID, new List<JHMeritRecord>());
            //    meritCache[record.RefStudentID].Add(record);
            //}

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

            //Dictionary<string, List<JHDemeritRecord>> demeritCache = new Dictionary<string, List<JHDemeritRecord>>();
            //foreach (JHDemeritRecord record in JHDemerit.SelectByStudentIDs(student_ids))
            //{
            //    if (record.OccurDate < _config.StartDate) continue;
            //    if (record.OccurDate > _config.EndDate) continue;

            //    if (!demeritCache.ContainsKey(record.RefStudentID))
            //        demeritCache.Add(record.RefStudentID, new List<JHDemeritRecord>());
            //    demeritCache[record.RefStudentID].Add(record);
            //}


            ////缺曠
            //// 1.依學生編號、開始日期、結束日期，取得學生缺曠紀錄
            //// 2.依學生編號進行分群
            //Dictionary<string, List<JHAttendanceRecord>> attendanceCache = new Dictionary<string, List<JHAttendanceRecord>>();
            //foreach (JHAttendanceRecord record in JHAttendance.Select(studentIDs, _config.StartDate, _config.EndDate, null, null, null))
            //{
            //    if (!attendanceCache.ContainsKey(record.RefStudentID))
            //        attendanceCache.Add(record.RefStudentID, new List<JHAttendanceRecord>());
            //    attendanceCache[record.RefStudentID].Add(record);
            //}
            //Dictionary<string, List<JHAttendanceRecord>> attendanceCache = new Dictionary<string, List<JHAttendanceRecord>>();
            //foreach (JHAttendanceRecord record in JHAttendance.SelectByStudentIDs(student_ids))
            //{
            //    if (record.OccurDate < _config.StartDate) continue;
            //    if (record.OccurDate > _config.EndDate) continue;

            //    if (!attendanceCache.ContainsKey(record.RefStudentID))
            //        attendanceCache.Add(record.RefStudentID, new List<JHAttendanceRecord>());
            //    attendanceCache[record.RefStudentID].Add(record);
            //}

            //List<string> studentIDs = new List<string>();
            //foreach (var stu in _config.Students)
            //    studentIDs.Add(stu.ID);

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
            }

            //課程
            // 1.依課程編號取得課程紀錄
            // 2.略過不列入成績計算的課程
            JHCourse.RemoveByIDs(courseIDs);
            Dictionary<string, JHCourseRecord> courseCache = JHCourse.SelectByIDs(courseIDs).Where(x => x.CalculationFlag != "2").ToDictionary(x => x.ID);

            //試別資訊: 取得所有試別
            Dictionary<string, JHExamRecord> examCache = JHExam.SelectAll().ToDictionary(x => x.ID);

            // 取得社團ExamID
            List<string> NExamIDList = new List<string>();
            foreach (JHExamRecord rec in JHExam.SelectAll())
            {
                if (rec.Name.IndexOf("社團") > -1)
                    NExamIDList.Add(rec.ID);
            }

            //評量成績
            // 1.依修課記錄及所有試別取得評量成績
            // 2.依第1點的評量成績取得試別編號 (實際學生評量成績中的試別編號)
            // 3.依學生編號進行分群
            Dictionary<string, List<HC.JHSCETakeRecord>> sceScoreCache = new Dictionary<string, List<HC.JHSCETakeRecord>>();
            List<string> validExamIDs = new List<string>();

            //  檢查當有修課紀錄才取成績資料
            if (attendIDs.Count > 0 && examCache.Count > 0)
            {
                foreach (JHSCETakeRecord record in JHSCETake.Select(null, null, examCache.Keys, null, attendIDs))
                {
                    if (!NExamIDList.Contains(record.RefExamID))
                        if (!validExamIDs.Contains(record.RefExamID))
                            validExamIDs.Add(record.RefExamID);

                    if (!sceScoreCache.ContainsKey(record.RefStudentID))
                        sceScoreCache.Add(record.RefStudentID, new List<HC.JHSCETakeRecord>());
                    sceScoreCache[record.RefStudentID].Add(new HC.JHSCETakeRecord(record));
                }
            }

            //將『所有試別編號』與『實際學生評量成績中的試別編號』做交集，以取得使用管理的試別次序
            //假設『所有試別編號』為1,4,3,5
            //假設『實際學生評量成績中的試別編號』為3,4,1
            //交集後的結果為1,4,3
            validExamIDs = examCache.Keys.Intersect(validExamIDs).ToList();



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


            ////修課紀錄
            //// 1.依學生編號、學年度、學期，取得修課紀錄
            //// 2.
            //List<string> courseIDs = new List<string>();
            //foreach (var attend in JHSCAttend.SelectByStudentIDs(studentIDs))
            //{
            //    if (!courseIDs.Contains(attend.RefCourseID))
            //        courseIDs.Add(attend.RefCourseID);
            //}

            //int schoolYear = _config.SchoolYear;
            //int semester = _config.Semester;
            //List<JHCourseRecord> courses = JHCourse.SelectByIDs(courseIDs);

            ////課程
            //Dictionary<string, JHCourseRecord> courseCache = new Dictionary<string, JHCourseRecord>();
            //foreach (JHCourseRecord record in JHCourse.SelectByIDs(courseIDs))
            //{
            //    if ("" + record.SchoolYear != "" + schoolYear) continue;
            //    if ("" + record.Semester != "" + semester) continue;
            //    if (record.CalculationFlag == "2") continue;
            //    //if (string.IsNullOrEmpty(record.Domain)) continue; //沒有填領域

            //    if (!courseCache.ContainsKey(record.ID))
            //        courseCache.Add(record.ID, record);
            //}

            ////試別資訊
            //Dictionary<string, JHExamRecord> examCache = new Dictionary<string, JHExamRecord>();
            //foreach (JHExamRecord exam in JHExam.SelectAll())
            //{
            //    if (!examCache.ContainsKey(exam.ID))
            //        examCache.Add(exam.ID, exam);
            //}
            //List<string> validExamIDs = new List<string>();

            ////評量成績
            //Dictionary<string, List<HC.JHSCETakeRecord>> sceScoreCache = new Dictionary<string, List<HC.JHSCETakeRecord>>();
            //foreach (JHSCETakeRecord record in JHSCETake.SelectByStudentIDs(student_ids))
            //{
            //    if (examCache.ContainsKey(record.RefExamID))
            //    {
            //        if (!validExamIDs.Contains(record.RefExamID))
            //            validExamIDs.Add(record.RefExamID);
            //    }
            //    else
            //        continue;

            //    if (!sceScoreCache.ContainsKey(record.RefStudentID))
            //        sceScoreCache.Add(record.RefStudentID, new List<HC.JHSCETakeRecord>());
            //    sceScoreCache[record.RefStudentID].Add(new HC.JHSCETakeRecord(record));
            //}
            //// TODO: 這邊的排序有可能再改
            //validExamIDs.Sort(delegate(string x, string y)
            //{
            //    int ix, iy;
            //    if (!int.TryParse(x, out ix))
            //        ix = int.MaxValue;
            //    if (!int.TryParse(y, out iy))
            //        iy = int.MaxValue;
            //    return ix.CompareTo(iy);
            //});

            //學期歷程
            //Dictionary<string, K12.Data.SemesterHistoryItem> historyCache = new Dictionary<string, K12.Data.SemesterHistoryItem>();
            //foreach (JHSemesterHistoryRecord record in JHSemesterHistory.SelectByStudentIDs(student_ids))
            //{
            //    foreach (K12.Data.SemesterHistoryItem item in record.SemesterHistoryItems)
            //    {
            //        if ("" + item.SchoolYear != K12.Data.School.DefaultSchoolYear) continue;
            //        if ("" + item.Semester != K12.Data.School.DefaultSemester) continue;

            //        if (!historyCache.ContainsKey(record.RefStudentID))
            //            historyCache.Add(record.RefStudentID, item);
            //    }
            //}
            #endregion

            #region 判斷領域是否需要展開
            //判斷領域是否展開對照表
            //Key:領域名稱
            //Value:false=展開,true=不展開
            //展開: 詳列該領域下所有科目成績
            //不展開: 只列該領域成績
            Dictionary<string, bool> domains = new Dictionary<string, bool>();
            Dictionary<string, List<string>> subjects = new Dictionary<string, List<string>>();

            List<JHCourseRecord> courseList = new List<JHCourseRecord>(courseCache.Values);
            courseList.Sort(delegate(JHCourseRecord x, JHCourseRecord y)
            {
                return JHSchool.Evaluation.Subject.CompareSubjectOrdinal(x.Subject, y.Subject);
                //List<string> list = new List<string>(new string[] { "國語文", "國文", "英文", "英語", "數學", "歷史", "地理", "公民", "理化", "生物" });
                //int ix = list.IndexOf(x.Subject);
                //int iy = list.IndexOf(y.Subject);

                //if (ix >= 0 && iy >= 0)
                //    return ix.CompareTo(iy);
                //else if (ix >= 0)
                //    return -1;
                //else if (iy >= 0)
                //    return 1;
                //else
                //    return x.Subject.CompareTo(y.Subject);
            });

            DomainSubjectSetup domainSubjectSetup = _config.DomainSubjectSetup;
            //使用者設定"只列印領域"
            if (domainSubjectSetup == DomainSubjectSetup.Domain)
            {
                //預設從領域資料管理來的領域名稱皆不展開
                foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                    domains.Add(domain, DomainSubjectExpand.不展開);
                if (domains.ContainsKey("語文")) domains["語文"] = DomainSubjectExpand.展開;

                //彈性課程一定展開
                if (!domains.ContainsKey("")) domains.Add("", DomainSubjectExpand.展開);
                //if (!domains.ContainsKey("彈性課程")) domains.Add("彈性課程", false);
            }
            //使用者設定"只列印科目"
            else if (domainSubjectSetup == DomainSubjectSetup.Subject)
            {
                //預設從領域資料管理來的領域名稱皆展開
                foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                    domains.Add(domain, DomainSubjectExpand.展開);
                //彈性課程一定展開
                if (!domains.ContainsKey("")) domains.Add("", DomainSubjectExpand.展開);
                //if (!domains.ContainsKey("彈性課程")) domains.Add("彈性課程", false);
            }
            else
                throw new Exception("請重新儲存一次列印設定");

            //foreach (var domain in JHSchool.Evaluation.Subject.Domains)
            //    subjects.Add(domain, new List<string>());

            //if (!subjects.ContainsKey("")) subjects.Add("", new List<string>());
            //if (!subjects.ContainsKey("彈性課程")) subjects.Add("彈性課程", new List<string>());

            //foreach (var course in courseList)
            //{
            //    if (!subjects.ContainsKey(course.Domain))
            //        subjects.Add(course.Domain, new List<string>());
            //    if (!subjects[course.Domain].Contains(course.Subject))
            //        subjects[course.Domain].Add(course.Subject);
            //}

            //_config.SetPrintDomains(domains);
            //_config.SetPrintSubjects(subjects);
            _config.PrintDomains = domains;
            #endregion

            #region 建立節次對照
            Dictionary<string, string> periodMapping = JHPeriodMapping.SelectAll().ToDictionary(x => x.Name, y => y.Type);
            //Dictionary<string, string> periodMapping = new Dictionary<string, string>();
            //foreach (JHPeriodMappingInfo info in JHPeriodMapping.SelectAll())
            //{
            //    if (!periodMapping.ContainsKey(info.Name))
            //        periodMapping.Add(info.Name, info.Type);
            //}
            #endregion

            #region 假別列表
            List<string> absenceList = JHAbsenceMapping.SelectAll().Select(x => x.Name).ToList();
            //List<string> absenceList = new List<string>();
            //foreach (JHAbsenceMappingInfo info in JHAbsenceMapping.SelectAll())
            //    absenceList.Add(info.Name);
            #endregion

            #region 依評量試別重新劃分範本
            int rowCount = 0;
            DocumentBuilder templateBuilder = new DocumentBuilder(_template);
            templateBuilder.MoveToMergeField("各次評量");
            Font font = templateBuilder.Font;
            Cell examsCell = templateBuilder.CurrentParagraph.ParentNode as Cell;
            Table table = examsCell.ParentRow.ParentTable;
            double width = examsCell.CellFormat.Width;
            double examWidth = width / (double)validExamIDs.Count;
            double scoreWidth = width / (double)validExamIDs.Count / 3.0;

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
                WordHelper.Write(topHeaderCell, font, examCache[examID].Name);
                table.Rows[0].Cells.Add(topHeaderCell);

                Cell subHeaderCell1 = new Cell(_template);
                WordHelper.Write(subHeaderCell1, font, "定期", "評量");
                table.Rows[1].Cells.Add(subHeaderCell1);
                columnIndex++;

                Cell subHeaderCell2 = new Cell(_template);
                WordHelper.Write(subHeaderCell2, font, "平時", "評量");
                table.Rows[1].Cells.Add(subHeaderCell2);
                columnIndex++;

                Cell subHeaderCell3 = new Cell(_template);
                WordHelper.Write(subHeaderCell3, font, "總成績");
                table.Rows[1].Cells.Add(subHeaderCell3);
                columnIndex++;

                topHeaderCell.CellFormat.Width = examWidth;
                subHeaderCell1.CellFormat.Width = scoreWidth;
                subHeaderCell2.CellFormat.Width = scoreWidth;
                subHeaderCell3.CellFormat.Width = scoreWidth;

                topHeaderCell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                subHeaderCell1.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                subHeaderCell2.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                subHeaderCell3.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
            }
            WordHelper.MergeVerticalCell(table.Rows[0].Cells[1], 2);
            #endregion

            #region Content Cell
            int shift = 2; //Header has 2 rows
            for (int i = 0; i < rowCount; i++)
            {
                table.Rows[i + shift].LastCell.Remove();

                for (int j = 0; j < validExamIDs.Count * 3; j++)
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

            //#region 取得學生成績計算規則

            //ScoreCalculator defaultScoreCalculator = new ScoreCalculator(null);

            ////key: ScoreCalcRuleID
            //Dictionary<string, ScoreCalculator> calcCache = new Dictionary<string, ScoreCalculator>();
            ////key: StudentID, val: ScoreCalcRuleID
            //Dictionary<string, string> calcIDCache = new Dictionary<string, string>();

            //List<string> scoreCalcRuleIDList = new List<string>();

            //foreach (JHStudentRecord student in _config.Students)
            //{
            //    string calcID = string.Empty;
            //    if (!string.IsNullOrEmpty(student.OverrideScoreCalcRuleID))
            //        calcID = student.OverrideScoreCalcRuleID;
            //    else if (student.Class != null && !string.IsNullOrEmpty(student.Class.RefScoreCalcRuleID))
            //        calcID = student.Class.RefScoreCalcRuleID;

            //    if (!string.IsNullOrEmpty(calcID))
            //        calcIDCache.Add(student.ID, calcID);
            //}
            //foreach (JHScoreCalcRuleRecord record in JHScoreCalcRule.SelectByIDs(calcIDCache.Values))
            //{
            //    if (!calcCache.ContainsKey(record.ID))
            //        calcCache.Add(record.ID, new ScoreCalculator(record));
            //}
            //#endregion

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
            List<string> sidList = (from data in Students select data.ID).ToList();
            Config._SRDict = Utility.GetServiceLearningDetail(sidList, _config.SchoolYear, _config.Semester);

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

                //#region 班導師
                //builder.MoveToMergeField("班導師");
                //if (historyCache.ContainsKey(student.ID))
                //    builder.Write(historyCache[student.ID].Teacher);
                //else
                //    builder.Write(string.Empty);
                //#endregion

                #region 各評量成績
                List<HC.JHSCETakeRecord> sceScoreList = null;
                if (sceScoreCache.ContainsKey(student.ID))
                    sceScoreList = sceScoreCache[student.ID];
                else
                    sceScoreList = new List<HC.JHSCETakeRecord>();

                ScoreCalculator studentCalculator = defaultScoreCalculator;
                if (calcIDCache.ContainsKey(student.ID) && calcCache.ContainsKey(calcIDCache[student.ID]))
                    studentCalculator = calcCache[calcIDCache[student.ID]];

                StudentExamScore examScore = new StudentExamScore(builder, _config, courseCache);
                if (scCache.ContainsKey(student.ID)) examScore.SetSubjects(scCache[student.ID]);
                examScore.SetColumnMap(columnMapping);
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
            //globalFieldValue.Add(K12.Data.School.DefaultSchoolYear);
            globalFieldValue.Add(_config.SchoolYear.ToString());

            globalFieldName.Add("學期");
            //globalFieldValue.Add(K12.Data.School.DefaultSemester);
            globalFieldValue.Add(_config.Semester.ToString());

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
