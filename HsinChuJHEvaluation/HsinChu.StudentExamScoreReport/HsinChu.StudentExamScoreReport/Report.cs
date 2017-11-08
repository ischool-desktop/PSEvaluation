using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using System.IO;
using System.ComponentModel;
using JHSchool.Data;
using HsinChu.JHEvaluation.Data;
using System.Xml;
using HsinChu.StudentExamScoreReport.Processor;
using FISCA.Presentation;
using FISCA.Presentation.Controls;
using System.Windows.Forms;
using JHSchool.Evaluation.Calculation;
using Campus.Report;
using System.Linq;

namespace HsinChu.StudentExamScoreReport
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
        

        public Report(Config config)
        {
            _config = config;
            _rc = new ReportConfiguration(Global.ReportName);
        

            _doc = new Document();
            _doc.Sections.Clear();
            if (_rc.Template == null)
                _rc.Template = new ReportTemplate(Properties.Resources.評量成績通知單, TemplateType.Word);
            _template = _rc.Template.ToDocument();
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
            MotherForm.SetStatusBarMessage(Global.ReportName + "產生完成");
            ReportSaver.SaveDocument(_doc, Global.ReportName);
            

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
            foreach (JHSCAttendRecord record in JHSCAttend.SelectByStudentIDs(student_ids))
            {
                if (!course_ids.Contains(record.RefCourseID))
                    course_ids.Add(record.RefCourseID);
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
            Dictionary<string, List<HC.JHSCETakeRecord>> sceScoreCache = new Dictionary<string, List<HC.JHSCETakeRecord>>();
            foreach (JHSCETakeRecord record in JHSCETake.SelectByStudentAndCourse(student_ids, course_ids))
            {
                if (record.RefExamID != _config.Exam.ID) continue;

                if (!sceScoreCache.ContainsKey(record.RefStudentID))
                    sceScoreCache.Add(record.RefStudentID, new List<HC.JHSCETakeRecord>());
                sceScoreCache[record.RefStudentID].Add(new HC.JHSCETakeRecord(record));
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

            #region 建立節次對照及假別列表
            Dictionary<string, string> periodMapping = new Dictionary<string, string>();
            foreach (JHPeriodMappingInfo info in JHPeriodMapping.SelectAll())
            {
                if (!periodMapping.ContainsKey(info.Name))
                    periodMapping.Add(info.Name, info.Type);
            }

            List<string> absenceList = new List<string>();
            foreach (JHAbsenceMappingInfo info in JHAbsenceMapping.SelectAll())
                absenceList.Add(info.Name);
            #endregion

            #region 判斷要列印的領域科目
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

            string domainSubjectSetup = _rc.GetString("領域科目設定", "Domain");
            _config.DomainSubjectSetup = domainSubjectSetup;

            if (domainSubjectSetup == "Domain")
            {
                foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                    domains.Add(domain, true);
                //domains.Add("彈性課程", false);

                if (domains.ContainsKey("語文")) domains["語文"] = false;
                if (!domains.ContainsKey("")) domains.Add("", false);
            }
            else if (domainSubjectSetup == "Subject")
            {
                foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                    domains.Add(domain, false);
                //domains.Add("彈性課程", false);
                if (!domains.ContainsKey("")) domains.Add("", false);
            }
            else
                throw new Exception("請重新儲存一次列印設定");

            foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                subjects.Add(domain, new List<string>());
            //subjects.Add("彈性課程", new List<string>());
            subjects.Add("", new List<string>());

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
            _config.SetPrintSubjects(subjects);
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
                Cell first = templateBuilder.CurrentParagraph.ParentNode as Cell;
                double width = first.CellFormat.Width;
                Table table = first.ParentRow.ParentTable;
                foreach (Row row in table.Rows)
                {
                    if (row.ChildNodes.Count == 1) break;
                    row.RemoveChild(row.LastCell);
                    int lastIndex = row.ChildNodes.Count - 1;
                    row.Cells[lastIndex--].CellFormat.Width += (width / 3f);
                    row.Cells[lastIndex--].CellFormat.Width += (width / 3f);
                    row.Cells[lastIndex].CellFormat.Width += (width / 3f);
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

            #region 如果使用者不印定期評量，從畫面上將欄位拿掉
            templateBuilder.MoveToMergeField("定期評量");

            if (_rc.GetBoolean("列印定期評量", false) == false)
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
                        eachRow.RemoveChild(eachRow.Cells[eachRow.Count - 3]);
                        lastIndex = eachRow.ChildNodes.Count - 3;
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
                templateBuilder.Write("定期評量");
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

            #region 如果使用者不印定期學習評量總成績，從畫面上將欄位拿掉
            templateBuilder.MoveToMergeField("定期學習評量總成績");

            if (_rc.GetBoolean("列印定期學習評量總成績", false) == false)
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
                templateBuilder.Write("定期學習評量總成績");
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
                        
            // 取得評量比例
            Utility.ScorePercentageHSDict.Clear();
            Utility.ScorePercentageHSDict = Utility.GetScorePercentageHS();
            

            #region 產生
            foreach (JHStudentRecord student in _config.Students)
            {
                count++;
                if (_overStudentList.Contains(student.ID)) continue;
                Document each = (Document)_template.Clone(true);
                DocumentBuilder builder = new DocumentBuilder(each);

                #region 學生基本資料
                StudentBasicInfo basicInfo = new StudentBasicInfo(builder);
                basicInfo.SetStudent(student);
                #endregion

                #region 班導師
                builder.MoveToMergeField("班導師");
                if (historyCache.ContainsKey(student.ID))
                    builder.Write(historyCache[student.ID].Teacher);
                else
                    builder.Write(string.Empty);
                #endregion

                #region 成績
                List<HC.JHSCETakeRecord> sceScoreList = null;
                if (sceScoreCache.ContainsKey(student.ID))
                    sceScoreList = sceScoreCache[student.ID];
                else
                    sceScoreList = new List<HC.JHSCETakeRecord>();
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
                examScore.PrintPeriod = _rc.GetBoolean("列印節數", false);
                examScore.PrintCredit = _rc.GetBoolean("列印權數", false);
                examScore.PrintText = _rc.GetBoolean("列印文字評語", true);
                examScore.PrintScore = _rc.GetBoolean("列印定期評量", true);
                examScore.PrintAssScore = _rc.GetBoolean("列印平時評量", true);
                examScore.PrintTotalScore = _rc.GetBoolean("列印定期學習評量總成績", true);

                examScore.SetScoreCalculator(studentCalculator);
                if (scCache.ContainsKey(student.ID)) examScore.SetSubjects(scCache[student.ID]);
                examScore.SetData(sceScoreList);

                #endregion

                if (_config.DomainSubjectSetup == "Subject")
                {
                    // 科目定期評量加權平均
                    if (builder.MoveToMergeField("加權平均") || builder.MoveToMergeField("定期評量加權平均"))
                    {
                        if(examScore.SubjAvgScore>0)
                            builder.Write(examScore.SubjAvgScore.ToString());
                        else 
                            builder.Write("");
                    }

                    // 科目平時評量加權平均
                    if (builder.MoveToMergeField("平時評量加權平均"))
                    {
                        if (examScore.SubjAvgAssignmentScore > 0)
                            builder.Write(examScore.SubjAvgAssignmentScore.ToString());
                        else
                            builder.Write("");
                    }

                    // 科目學習總分加權平均       
                    if (builder.MoveToMergeField("定期學習評量總成績加權平均"))
                    { 
                        if(examScore.SubjAvgFinalScore>0)
                            builder.Write(examScore.SubjAvgFinalScore.ToString());
                        else
                            builder.Write("");
                    }                   

                }
                else
                {
                    // 領域定期評量加權平均
                    if (builder.MoveToMergeField("加權平均") || builder.MoveToMergeField("定期評量加權平均"))
                    {
                        if (examScore.DomainAvgScore > 0)
                            builder.Write(examScore.DomainAvgScore.ToString());
                        else
                            builder.Write("");
                    }

                     // 領域平時評量加權平均
                    if (builder.MoveToMergeField("平時評量加權平均"))
                    {
                        if (examScore.DomainAvgAssignmentScore > 0)
                            builder.Write(examScore.DomainAvgAssignmentScore.ToString());
                        else
                            builder.Write("");
                    }
                    
                    // 領域學習總分加權平均
                    if (builder.MoveToMergeField("定期學習評量總成績加權平均"))
                    {
                        if (examScore.DomainAvgFinalScore > 0)
                            builder.Write(examScore.DomainAvgFinalScore.ToString());
                        else
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
