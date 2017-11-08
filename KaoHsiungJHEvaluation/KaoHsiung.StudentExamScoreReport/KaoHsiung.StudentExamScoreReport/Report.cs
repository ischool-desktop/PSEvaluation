using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Aspose.Words;
using Campus.Report;
using FISCA.Presentation;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using JHSchool.Evaluation.Calculation;
using KaoHsiung.JHEvaluation.Data;
using KaoHsiung.StudentExamScoreReport.Processor;
using System.Linq;
using Aspose.Words.Tables;
using System.IO;
using System.Windows.Forms;

namespace KaoHsiung.StudentExamScoreReport
{
    internal class Report
    {
        private Document _doc;
        private Document _template;
        private int _rowCount = 0;

        private BackgroundWorker _worker;
        private List<string> _overStudentList = new List<string>();

        private Config _config;
        private ReportConfiguration _rc;

        private EffortMapper _effortMapper;

        public Report(Config config)
        {
            _config = config;
            _rc = new ReportConfiguration(Global.ReportName);
            // 設定列印文字評語不出現
            _rc.SetBoolean("列印文字評語", false);
            _rc.Save();
            _effortMapper = new EffortMapper();

            _doc = new Document();
            _doc.Sections.Clear();
            if (_rc.Template == null)
                _rc.Template = new ReportTemplate(Properties.Resources.評量成績通知單, TemplateType.Word);
            //_template = _rc.Template.ToDocument();
            _template = new Document(new MemoryStream(_rc.Template.ToBinary()));

            _rowCount = GetRowCount(_template);

            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.WorkerReportsProgress = true;
        }

        /// <summary>
        /// 計算成績資料列個數
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        private int GetRowCount(Document template)
        {
            try
            {
                DocumentBuilder tempBuilder = new DocumentBuilder(template);
                tempBuilder.MoveToMergeField("row_start");
                Row startRow = (tempBuilder.CurrentParagraph.ParentNode as Cell).ParentRow;
                tempBuilder.MoveToMergeField("row_end");
                Row endRow = (tempBuilder.CurrentParagraph.ParentNode as Cell).ParentRow;
                Table table = startRow.ParentTable;
                return table.IndexOf(endRow) - table.IndexOf(startRow) + 1;
            }
            catch (Exception)
            {
                return 19;
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MotherForm.SetStatusBarMessage(Global.ReportName + "產生中", e.ProgressPercentage);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            K12.Presentation.NLDPanels.Student.AddToTemp(_overStudentList);

            if (e.Error != null)
            {
                MsgBox.Show("產生報表時發生錯誤。" + e.Error.Message);
                return;
            }

            if ("" + e.Result == "Cancel")
            {
                MotherForm.SetStatusBarMessage("");
                return;
            }

            bool pdf = _rc.GetBoolean("輸出成PDF格式", false);

            //PDF及WORD儲存路徑
            string path1 = System.Windows.Forms.Application.StartupPath + "\\Reports\\" + Global.ReportName + ".pdf";
            string path2 = System.Windows.Forms.Application.StartupPath + "\\Reports\\" + Global.ReportName + ".doc";

            //PDF重覆檔名判斷
            int i = 1;
            while (File.Exists(path1))
            {
                path1 = System.Windows.Forms.Application.StartupPath + "\\Reports\\" + Global.ReportName + i + ".pdf";
                i++;
            }

            //WORD重覆檔名判斷
            i = 1;
            while (File.Exists(path2))
            {
                path2 = System.Windows.Forms.Application.StartupPath + "\\Reports\\" + Global.ReportName + i + ".doc";
                i++;
            }

            if (pdf)
            {
                bool done = false;
                MotherForm.SetStatusBarMessage("轉換成 PDF 格式中...");
                //ReportSaver.SaveDocument(_doc, Global.ReportName, ReportSaver.OutputType.PDF);
                MotherForm.SetStatusBarMessage(Global.ReportName + "產生完成");

                try
                {
                    _doc.Save(path1, Aspose.Words.SaveFormat.Pdf);
                    done = true;
                }
                catch
                {
                    MsgBox.Show("檔案儲存失敗");
                }

                if (done)
                {
                    if (DialogResult.OK == MessageBox.Show(path1 + "產生完成，是否立刻開啟？", "ischool", MessageBoxButtons.OKCancel))
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(path1);
                        }
                        catch
                        {
                            MsgBox.Show(path1 + " 檔案開啟失敗");
                        }
                    }
                }
            }
            else
            {
                bool done = false;
                MotherForm.SetStatusBarMessage(Global.ReportName + "產生完成");
                //ReportSaver.SaveDocument(_doc, Global.ReportName);

                try
                {
                    _doc.Save(path2, Aspose.Words.SaveFormat.Doc);
                    done = true;
                }
                catch
                {
                    MsgBox.Show("檔案儲存失敗");
                }

                if (done)
                {
                    if (DialogResult.OK == MessageBox.Show(path2 + "產生完成，是否立刻開啟？", "ischool", MessageBoxButtons.OKCancel))
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(path2);
                        }
                        catch
                        {
                            MsgBox.Show(path2 + " 檔案開啟失敗");
                        }
                    }
                }
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            double total = _config.Students.Count;
            double count = 0;

            _worker.ReportProgress(0);

            List<string> student_ids = new List<string>();
            foreach (JHStudentRecord item in _config.Students)
                student_ids.Add(item.ID);

            #region 快取資料

            //// 取得父母資料
            //Dictionary<string, JHParentRecord> parentCache = JHParent.SelectByStudentIDs(student_ids).ToDictionary(x => x.RefStudentID);

            //// 取得電話
            //Dictionary<string, JHPhoneRecord> phoneCache = JHPhone.SelectByStudentIDs(student_ids).ToDictionary(x => x.RefStudentID);

            //// 取得地址
            //Dictionary<string, JHAddressRecord> addressCache = JHAddress.SelectByStudentIDs(student_ids).ToDictionary(x => x.RefStudentID);

            //// 取得照片
            //Dictionary<string, string> base64Cache = K12.Data.Photo.SelectGraduatePhoto(student_ids);

            //獎勵
            Dictionary<string, List<JHMeritRecord>> meritCache = new Dictionary<string, List<JHMeritRecord>>();
            foreach (JHMeritRecord record in JHMerit.SelectByStudentIDs(student_ids))
            {
                if (record.OccurDate < _config.StartDate) continue;
                if (record.OccurDate > _config.EndDate) continue;

                if (!meritCache.ContainsKey(record.RefStudentID))
                    meritCache.Add(record.RefStudentID, new List<JHMeritRecord>());
                meritCache[record.RefStudentID].Add(record);
            }

            //懲戒
            Dictionary<string, List<JHDemeritRecord>> demeritCache = new Dictionary<string, List<JHDemeritRecord>>();
            foreach (JHDemeritRecord record in JHDemerit.SelectByStudentIDs(student_ids))
            {
                if (record.OccurDate < _config.StartDate) continue;
                if (record.OccurDate > _config.EndDate) continue;

                if (!demeritCache.ContainsKey(record.RefStudentID))
                    demeritCache.Add(record.RefStudentID, new List<JHDemeritRecord>());
                demeritCache[record.RefStudentID].Add(record);
            }

            //缺曠
            Dictionary<string, List<JHAttendanceRecord>> attendanceCache = new Dictionary<string, List<JHAttendanceRecord>>();
            foreach (JHAttendanceRecord record in JHAttendance.SelectByStudentIDs(student_ids))
            {
                if (record.OccurDate < _config.StartDate) continue;
                if (record.OccurDate > _config.EndDate) continue;

                if (!attendanceCache.ContainsKey(record.RefStudentID))
                    attendanceCache.Add(record.RefStudentID, new List<JHAttendanceRecord>());
                attendanceCache[record.RefStudentID].Add(record);
            }

            List<string> course_ids = new List<string>();

            //平時評量快取
            Dictionary<string, List<JHSCAttendRecord>> assignmentScoreCache = new Dictionary<string, List<JHSCAttendRecord>>();

            foreach (JHSCAttendRecord record in JHSCAttend.SelectByStudentIDs(student_ids))
            {
                if (!course_ids.Contains(record.RefCourseID))
                    course_ids.Add(record.RefCourseID);

                if (!assignmentScoreCache.ContainsKey(record.RefStudentID))
                    assignmentScoreCache.Add(record.RefStudentID, new List<JHSCAttendRecord>());
                assignmentScoreCache[record.RefStudentID].Add(record);
            }

            //課程
            JHCourse.RemoveAll();
            int schoolYear = _config.SchoolYear;
            int semester = _config.Semester;
            Dictionary<string, JHCourseRecord> courseCache = new Dictionary<string, JHCourseRecord>();
            foreach (JHCourseRecord record in JHCourse.SelectByIDs(course_ids))
            {
                if ("" + record.SchoolYear != "" + schoolYear) continue;
                if ("" + record.Semester != "" + semester) continue;
                // 濾過社團
                if (string.IsNullOrEmpty(record.Domain) && string.IsNullOrEmpty(record.Subject))
                    continue;
                // 過濾使用者所選課程才放入
                if (Global._selectCourseIDList.Contains(record.ID))
                    if (!courseCache.ContainsKey(record.ID))
                        courseCache.Add(record.ID, record);
            }

            //修課記錄
            Dictionary<string, List<string>> scCache = new Dictionary<string, List<string>>();
            //foreach (JHSCAttendRecord sc in JHSCAttend.Select(student_ids, course_ids, new string[] { }, "" + _config.SchoolYear, "" + _config.Semester))
            foreach (JHSCAttendRecord sc in JHSCAttend.SelectByStudentIDAndCourseID(student_ids, course_ids))
            {
                if (!courseCache.ContainsKey(sc.RefCourseID)) continue;

                if (!scCache.ContainsKey(sc.RefStudentID))
                    scCache.Add(sc.RefStudentID, new List<string>());

                // 過濾使用者不選
                if (Global._selectCourseIDList.Contains(sc.RefCourseID))
                    scCache[sc.RefStudentID].Add(sc.RefCourseID);
            }

            //評量成績
            Dictionary<string, List<KH.JHSCETakeRecord>> sceScoreCache = new Dictionary<string, List<KH.JHSCETakeRecord>>();

            foreach (JHSCETakeRecord record in JHSCETake.SelectByStudentAndCourse(student_ids, course_ids))
            {
                if (record.RefExamID == _config.Exam.ID)
                {
                    if (!sceScoreCache.ContainsKey(record.RefStudentID))
                        sceScoreCache.Add(record.RefStudentID, new List<KH.JHSCETakeRecord>());
                    sceScoreCache[record.RefStudentID].Add(new KH.JHSCETakeRecord(record));
                }
                //else if (record.RefExamID == _rc.GetString("平時評量試別", string.Empty))
                //{
                //    if (!assignmentScoreCache.ContainsKey(record.RefStudentID))
                //        assignmentScoreCache.Add(record.RefStudentID, new List<KH.JHSCETakeRecord>());
                //    assignmentScoreCache[record.RefStudentID].Add(new KH.JHSCETakeRecord(record));
                //}
            }

            //學期歷程
            Dictionary<string, K12.Data.SemesterHistoryItem> historyCache = new Dictionary<string, K12.Data.SemesterHistoryItem>();
            foreach (JHSemesterHistoryRecord record in JHSemesterHistory.SelectByStudentIDs(student_ids))
            {
                foreach (K12.Data.SemesterHistoryItem item in record.SemesterHistoryItems)
                {
                    if ("" + item.SchoolYear != K12.Data.School.DefaultSchoolYear) continue;
                    if ("" + item.Semester != K12.Data.School.DefaultSemester) continue;

                    if (!historyCache.ContainsKey(record.RefStudentID))
                        historyCache.Add(record.RefStudentID, item);
                }
            }

            //取得所有教師，為了Cache下來
            JHTeacher.SelectAll();
            #endregion

            #region 建立節次對照
            Dictionary<string, string> periodMapping = new Dictionary<string, string>();
            foreach (JHPeriodMappingInfo info in JHPeriodMapping.SelectAll())
            {
                if (!periodMapping.ContainsKey(info.Name))
                    periodMapping.Add(info.Name, info.Type);
            }
            #endregion

            #region 假別列表
            List<string> absenceList = new List<string>();
            foreach (JHAbsenceMappingInfo info in JHAbsenceMapping.SelectAll())
                absenceList.Add(info.Name);
            #endregion

            #region 判斷要列印的領域科目
            Dictionary<string, bool> domains = new Dictionary<string, bool>();
            //Dictionary<string, List<string>> subjects = new Dictionary<string, List<string>>();

            List<JHCourseRecord> courseList = new List<JHCourseRecord>(courseCache.Values);
            courseList.Sort(delegate(JHCourseRecord x, JHCourseRecord y)
            {
                return JHSchool.Evaluation.Subject.CompareSubjectOrdinal(x.Subject, y.Subject);
            });

            string domainSubjectSetup = _rc.GetString("領域科目設定", "Domain");
            _config.DomainSubjectSetup = domainSubjectSetup;
            if (domainSubjectSetup == "Domain")
            {
                foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                    domains.Add(domain, true);

                //if (domains.ContainsKey("語文")) domains["語文"] = false;
                if (!domains.ContainsKey("")) domains.Add("", false);
            }
            else if (domainSubjectSetup == "Subject")
            {
                foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                    domains.Add(domain, false);
                if (!domains.ContainsKey("")) domains.Add("", false);
            }
            else
                throw new Exception("請重新儲存一次列印設定");

            //foreach (var domain in JHSchool.Evaluation.Subject.Domains)
            //    subjects.Add(domain, new List<string>());
            //subjects.Add("", new List<string>());

            //foreach (JHCourseRecord course in courseList)
            //{
            //    if (!domains.ContainsKey(course.Domain)) continue;

            //    if (!subjects.ContainsKey(course.Domain))
            //        subjects.Add(course.Domain, new List<string>());

            //    //很怪
            //    if (domains[course.Domain] == true) continue;

            //    if (!subjects[course.Domain].Contains(course.Subject))
            //        subjects[course.Domain].Add(course.Subject);
            //}

            _config.SetPrintDomains(domains);
            //_config.SetPrintSubjects(subjects);
            #endregion

            DocumentBuilder templateBuilder = new DocumentBuilder(_template);

            #region 判斷是否列印文字評語

            templateBuilder.MoveToMergeField("文字評語");
            if (_rc.GetBoolean("列印文字評語", true))
            {
                templateBuilder.Write("文字評語");
            }
            else
            {
                // 預設文字評語不出現
                Cell first = templateBuilder.CurrentParagraph.ParentNode as Cell;
                double width1 = first.CellFormat.Width;
                Table table = first.ParentRow.ParentTable;
                foreach (Row row in table.Rows)
                {
                    if (row.ChildNodes.Count == 1) break;
                    row.RemoveChild(row.LastCell);
                    int lastIndex = row.ChildNodes.Count - 1;
                    row.Cells[lastIndex--].CellFormat.Width += (width1 / 3f);
                    row.Cells[lastIndex--].CellFormat.Width += (width1 / 3f);
                    row.Cells[lastIndex].CellFormat.Width += (width1 / 3f);
                }
            }
            #endregion

            #region 依節權數設定，在畫面上顯示
            string pcDisplay = string.Empty;
            bool p = _rc.GetBoolean("列印節數", false);
            bool c = _rc.GetBoolean("列印權數", false);
            if (p && c)
                pcDisplay = "節/權數";
            else if (p)
                pcDisplay = "節數";
            else if (c)
                pcDisplay = "權數";

            while (templateBuilder.MoveToMergeField("節權數"))
                templateBuilder.Write(pcDisplay);
            #endregion

            #region 如果使用者不印分數評量，從畫面上將欄位拿掉
            templateBuilder.MoveToMergeField("分數評量");

            if (_rc.GetBoolean("列印分數評量", false) == false)
            {
                Cell assignmentCell = templateBuilder.CurrentParagraph.ParentNode as Cell;
                double width = assignmentCell.CellFormat.Width;
                bool hasText = false;
                if (assignmentCell.NextSibling != null)
                {
                    hasText = true;
                }
                Table scoreTable = assignmentCell.ParentRow.ParentTable;
                foreach (Row eachRow in scoreTable.Rows)
                {
                    if (eachRow.Cells.Count == 1) break;

                    int lastIndex = 0;
                    if (hasText)
                    {
                        eachRow.RemoveChild(eachRow.Cells[eachRow.Count - 2]);
                        lastIndex = eachRow.ChildNodes.Count - 2;
                    }
                    else
                    {
                        eachRow.RemoveChild(eachRow.LastCell);
                        lastIndex = eachRow.ChildNodes.Count - 1;
                    }
                    eachRow.Cells[lastIndex--].CellFormat.Width += (width / 2f);
                    eachRow.Cells[lastIndex].CellFormat.Width += (width / 2f);
                }
            }
            else
                templateBuilder.Write("分數評量");
            #endregion


            #region 如果使用者不印努力程度，從畫面上將欄位拿掉
            templateBuilder.MoveToMergeField("努力程度");

            if (_rc.GetBoolean("列印努力程度", false) == false)
            {
                Cell assignmentCell = templateBuilder.CurrentParagraph.ParentNode as Cell;
                double width = assignmentCell.CellFormat.Width;
                bool hasText = false;
                if (assignmentCell.NextSibling != null)
                {
                    hasText = true;
                }
                Table scoreTable = assignmentCell.ParentRow.ParentTable;
                foreach (Row eachRow in scoreTable.Rows)
                {
                    if (eachRow.Cells.Count == 1) break;

                    int lastIndex = 0;
                    if (hasText)
                    {
                        eachRow.RemoveChild(eachRow.Cells[eachRow.Count - 2]);
                        lastIndex = eachRow.ChildNodes.Count - 2;
                    }
                    else
                    {
                        eachRow.RemoveChild(eachRow.LastCell);
                        lastIndex = eachRow.ChildNodes.Count - 1;
                    }
                    eachRow.Cells[lastIndex--].CellFormat.Width += (width / 2f);
                    eachRow.Cells[lastIndex].CellFormat.Width += (width / 2f);
                }
            }
            else
                templateBuilder.Write("努力程度");
            #endregion


            #region 如果使用者不印平時評量，從畫面上將欄位拿掉
            templateBuilder.MoveToMergeField("平時評量");

            if (_rc.GetBoolean("列印平時評量", false) == false)
            {
                Cell assignmentCell = templateBuilder.CurrentParagraph.ParentNode as Cell;
                double width = assignmentCell.CellFormat.Width;
                bool hasText = false;
                if (assignmentCell.NextSibling != null)
                {
                    hasText = true;
                }
                Table scoreTable = assignmentCell.ParentRow.ParentTable;
                foreach (Row eachRow in scoreTable.Rows)
                {
                    if (eachRow.Cells.Count == 1) break;

                    int lastIndex = 0;
                    if (hasText)
                    {
                        eachRow.RemoveChild(eachRow.Cells[eachRow.Count - 2]);
                        lastIndex = eachRow.ChildNodes.Count - 2;
                    }
                    else
                    {
                        eachRow.RemoveChild(eachRow.LastCell);
                        lastIndex = eachRow.ChildNodes.Count - 1;
                    }
                    eachRow.Cells[lastIndex--].CellFormat.Width += (width / 2f);
                    eachRow.Cells[lastIndex].CellFormat.Width += (width / 2f);
                }
            }
            else
                templateBuilder.Write("平時評量");
            #endregion

            #region 取得學生成績計算規則

            ScoreCalculator defaultScoreCalculator = new ScoreCalculator(null);

            //key: ScoreCalcRuleID
            Dictionary<string, ScoreCalculator> calcCache = new Dictionary<string, ScoreCalculator>();
            //key: StudentID, val: ScoreCalcRuleID
            Dictionary<string, string> calcIDCache = new Dictionary<string, string>();
            List<string> scoreCalcRuleIDList = new List<string>();
            foreach (JHStudentRecord student in _config.Students)
            {
                //calcCache.Add(student.ID, new ScoreCalculator(student.ScoreCalcRule));
                string calcID = string.Empty;
                if (!string.IsNullOrEmpty(student.OverrideScoreCalcRuleID))
                    calcID = student.OverrideScoreCalcRuleID;
                else if (student.Class != null && !string.IsNullOrEmpty(student.Class.RefScoreCalcRuleID))
                    calcID = student.Class.RefScoreCalcRuleID;

                if (!string.IsNullOrEmpty(calcID))
                    calcIDCache.Add(student.ID, calcID);
            }
            foreach (JHScoreCalcRuleRecord record in JHScoreCalcRule.SelectByIDs(calcIDCache.Values))
            {
                if (!calcCache.ContainsKey(record.ID))
                    calcCache.Add(record.ID, new ScoreCalculator(record));
            }
            //MsgBox.Show("" + (Environment.TickCount - t));
            #endregion

            #region 檢查學生成績是否超出可列印行數

            foreach (JHStudentRecord student in _config.Students)
            {
                if (scCache.ContainsKey(student.ID))
                {
                    int checkCount = 0;
                    if (_config.DomainSubjectSetup == "Subject")
                        checkCount = scCache[student.ID].Count;
                    else
                    {
                        List<string> checkDomains = new List<string>();
                        foreach (string courseID in scCache[student.ID])
                        {
                            JHCourseRecord course = courseCache[courseID];
                            if (string.IsNullOrEmpty(course.Domain))
                                checkCount++;
                            else if (!checkDomains.Contains(course.Domain))
                                checkDomains.Add(course.Domain);
                        }
                        checkCount += checkDomains.Count;
                    }

                    if (checkCount > _rowCount)
                        _overStudentList.Add(student.ID);
                }
            }

            //有學生資料超出範圍
            if (_overStudentList.Count > 0)
            {
                //K12.Presentation.NLDPanels.Student.AddToTemp(overStudentList);
                System.Windows.Forms.DialogResult result = MsgBox.Show("有 " + _overStudentList.Count + " 位學生評量成績資料超出範本可列印範圍，已將學生加入待處理。\n是否繼續列印？", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.No)
                    e.Result = "Cancel";
            }
            #endregion

            // 取得學生課程ID對照
            Dictionary<string, List<string>> studCourseID = new Dictionary<string, List<string>>();
            List<string> sid = (from stud in _config.Students select stud.ID).ToList();
            foreach (JHSCAttendRecord attend in JHSCAttend.SelectByStudentIDs(sid))
            {
                if (attend.Course.SchoolYear.HasValue && attend.Course.Semester.HasValue)
                    if (attend.Course.SchoolYear == _config.SchoolYear && attend.Course.Semester == _config.Semester)
                        if (studCourseID.ContainsKey(attend.RefStudentID))
                        {
                            studCourseID[attend.RefStudentID].Add(attend.RefCourseID);
                        }
                        else
                        {
                            List<string> coid = new List<string>();
                            coid.Add(attend.RefCourseID);
                            studCourseID.Add(attend.RefStudentID, coid);
                        }
            }

            // 取得學期歷程
            Config._StudSemesterHistoryItemDict.Clear();

            List<JHSemesterHistoryRecord> semHisRec = JHSemesterHistory.SelectByStudents(_config.Students);
            // 當畫面學期歷程內的學年度學期與畫面上設定相同，加入
            foreach (JHSemesterHistoryRecord rec in semHisRec)
                foreach (K12.Data.SemesterHistoryItem shi in rec.SemesterHistoryItems)
                    if (shi.SchoolYear == _config.SchoolYear && shi.Semester == _config.Semester)
                        if (!Config._StudSemesterHistoryItemDict.ContainsKey(shi.RefStudentID))
                            Config._StudSemesterHistoryItemDict.Add(shi.RefStudentID, shi);

            #region 產生
            foreach (JHStudentRecord student in _config.Students)
            {
                count++;

                if (_overStudentList.Contains(student.ID)) continue;

                Document each = (Document)_template.Clone(true);
                DocumentBuilder builder = new DocumentBuilder(each);



                //#region 班導師
                //builder.MoveToMergeField("班導師");
                //if (historyCache.ContainsKey(student.ID))
                //    builder.Write(historyCache[student.ID].Teacher);
                //else
                //    builder.Write(string.Empty);
                //#endregion

                #region 成績
                List<KH.JHSCETakeRecord> sceScoreList = null;
                if (sceScoreCache.ContainsKey(student.ID))
                    sceScoreList = sceScoreCache[student.ID];
                else
                    sceScoreList = new List<KH.JHSCETakeRecord>();

                List<JHSCAttendRecord> assignmentScoreList = new List<JHSCAttendRecord>();
                if (assignmentScoreCache.ContainsKey(student.ID))
                {
                    foreach (JHSCAttendRecord scattend in assignmentScoreCache[student.ID])
                        if (courseCache.ContainsKey(scattend.RefCourseID))
                            assignmentScoreList.Add(scattend);
                }

                ScoreCalculator studentCalculator = defaultScoreCalculator;
                if (calcIDCache.ContainsKey(student.ID) && calcCache.ContainsKey(calcIDCache[student.ID]))
                    studentCalculator = calcCache[calcIDCache[student.ID]];

                // 過濾 courseCache studid
                Dictionary<string, JHCourseRecord> courseCache1 = new Dictionary<string, JHCourseRecord>();

                foreach (KeyValuePair<string, JHCourseRecord> val in courseCache)
                {
                    // 從學生修課來對應到課程
                    if (studCourseID.ContainsKey(student.ID))
                        if (studCourseID[student.ID].Contains(val.Value.ID))
                            courseCache1.Add(val.Key, val.Value);
                }

                StudentExamScore examScore = new StudentExamScore(builder, _config, courseCache1);
                examScore.SetEffortMap(_effortMapper);
                examScore.PrintPeriod = _rc.GetBoolean("列印節數", false);
                examScore.PrintCredit = _rc.GetBoolean("列印權數", false);
                examScore.PrintText = _rc.GetBoolean("列印文字評語", true);
                examScore.PrintAssignment = _rc.GetBoolean("列印平時評量", true);
                examScore.PrintScore = _rc.GetBoolean("列印分數評量", true);
                examScore.PrintEffort = _rc.GetBoolean("列印努力程度", true);

                examScore.SetScoreCalculator(studentCalculator);
                if (scCache.ContainsKey(student.ID)) examScore.SetSubjects(scCache[student.ID]);
                examScore.SetData(sceScoreList);
                examScore.SetAssignmentData(assignmentScoreList);
                examScore.FillScore();
                #endregion

                if (_config.DomainSubjectSetup == "Subject")
                {
                    // 科目加權平均
                    if (examScore.SubjAvgScore > 0)
                    {
                        if (builder.MoveToMergeField("加權平均") == true)
                            builder.Write(examScore.SubjAvgScore.ToString());
                    }
                    else
                    {
                        if (builder.MoveToMergeField("加權平均") == true)
                            builder.Write("");
                    }
                }
                else
                {
                    // 領域加權平均
                    if (examScore.DomainAvgScore > 0)
                    {
                        if (builder.MoveToMergeField("加權平均") == true)
                            builder.Write(examScore.DomainAvgScore.ToString());
                    }
                    else
                    {
                        if (builder.MoveToMergeField("加權平均") == true)
                            builder.Write("");
                    }
                }
                #region 日常表現
                List<JHMeritRecord> meritList = null;
                List<JHDemeritRecord> demeritList = null;
                List<JHAttendanceRecord> attendanceList = null;

                meritList = (meritCache.ContainsKey(student.ID)) ? meritCache[student.ID] : new List<JHMeritRecord>();
                demeritList = (demeritCache.ContainsKey(student.ID)) ? demeritCache[student.ID] : new List<JHDemeritRecord>();
                attendanceList = (attendanceCache.ContainsKey(student.ID)) ? attendanceCache[student.ID] : new List<JHAttendanceRecord>();

                StudentMoralScore moral = new StudentMoralScore(builder, _config, periodMapping, absenceList);
                moral.SetData(meritList, demeritList, attendanceList);
                #endregion

                #region 學生基本資料
                StudentBasicInfo basicInfo = new StudentBasicInfo(builder);
                basicInfo.SetStudent(student);
                #endregion

                foreach (Section sec in each.Sections)
                    _doc.Sections.Add(_doc.ImportNode(sec, true));

                //回報進度
                _worker.ReportProgress((int)(count * 100.0 / total));
            }

            List<string> globalFieldName = new List<string>();
            List<object> globalFieldValue = new List<object>();

            globalFieldName.Add("學校名稱");
            globalFieldValue.Add(K12.Data.School.ChineseName);

            globalFieldName.Add("學年度");
            globalFieldValue.Add(_config.SchoolYear);

            globalFieldName.Add("學期");
            globalFieldValue.Add(_config.Semester);

            globalFieldName.Add("評量名稱");
            globalFieldValue.Add(_config.Exam.Name);

            globalFieldName.Add("統計期間");
            globalFieldValue.Add(_config.StartDate.ToShortDateString() + " ~ " + _config.EndDate.ToShortDateString());

            globalFieldName.Add("列印日期");
            globalFieldValue.Add(DateConvert.CDate(DateTime.Now.ToString("yyyy/MM/dd")) + " " + DateTime.Now.ToString("HH:mm:ss"));

            string chancellor, eduDirector, stuDirector;
            chancellor = eduDirector = stuDirector = string.Empty;

            XmlElement schoolInfo = K12.Data.School.Configuration["學校資訊"].PreviousData;
            XmlElement chancellorElement = (XmlElement)schoolInfo.SelectSingleNode("ChancellorChineseName");
            XmlElement eduDirectorElement = (XmlElement)schoolInfo.SelectSingleNode("EduDirectorName");

            if (chancellorElement != null) chancellor = chancellorElement.InnerText;
            if (eduDirectorElement != null) eduDirector = eduDirectorElement.InnerText;

            globalFieldName.Add("教務主任");
            globalFieldValue.Add(eduDirector);



            globalFieldName.Add("校長");
            globalFieldValue.Add(chancellor);
            globalFieldName.Add("成績校正日期");
            globalFieldValue.Add(_rc.GetString("成績校正日期", string.Empty));

            if (_config.Students.Count > _overStudentList.Count)
                _doc.MailMerge.Execute(globalFieldName.ToArray(), globalFieldValue.ToArray());

            #endregion
        }

        internal void Generate()
        {
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }
    }
}
