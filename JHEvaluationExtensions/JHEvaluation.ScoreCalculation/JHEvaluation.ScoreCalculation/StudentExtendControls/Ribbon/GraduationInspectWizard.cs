using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aspose.Cells;
using Framework;
using JHSchool.Data;
using JHSchool.Evaluation.Calculation;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon
{
    /// <summary>
    /// 畢業審查畫面
    /// </summary>
    public partial class GraduationInspectWizard : FISCA.Presentation.Controls.BaseForm
    {
        private List<StudentRecord> _errorList;
        private Dictionary<string, bool> _passList;
        private EvaluationResult _result;
        private List<StudentRecord> _students;
        private bool _gradeYearSelected = false;

        private BackgroundWorker _historyWorker, _inspectWorker, _writeWorker;

        public GraduationInspectWizard(string type)
        {
            InitializeComponent();
            
            pic_loading.Visible = false;

            #region 判斷是在學生身上選還是在教務作業上選
            if (type.Equals("Student"))
            {
                lblMessage.Visible = true;
                _gradeYearSelected = true;
                _students = Student.Instance.SelectedList.GetInSchoolStudents();
                InitializeMessage(_students.Count);
            }
            else if (type.Equals("EduAdmin"))
            {
                lblGradeYear.Visible = true;
                intGradeYear.Visible = true;
                _students = new List<StudentRecord>();
            }
            else
            {
                MsgBox.Show("初始化畫面發生錯誤。");
            }
            #endregion

            _errorList = new List<StudentRecord>();
            _passList = new Dictionary<string, bool>();
            _result = new EvaluationResult();

            InitializeWorkers();

            //先清除Instance再做Refresh,否則會被預警報表所影響
            Graduation.Instance.Reset();
            Graduation.Instance.Refresh();
        }

        private void InitializeMessage(int count)
        {
            lblMessage.Text = "系統即將依您設定之畢業條件進行畢業資格審查，目前系統內準畢業生(九年級學生)人數為 " + count + " 人，請您選按「下一步」進行審查作業。";
        }

        /// <summary>
        /// 初始化BackgroundWorkder
        /// </summary>
        private void InitializeWorkers()
        {
            _historyWorker = new BackgroundWorker();
            _historyWorker.DoWork += new DoWorkEventHandler(_historyWorker_DoWork);
            _historyWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_historyWorker_RunWorkerCompleted);
            _historyWorker.ProgressChanged += new ProgressChangedEventHandler(_historyWorker_ProgressChanged);

            _inspectWorker = new BackgroundWorker();
            _inspectWorker.DoWork += new DoWorkEventHandler(_inspectWorker_DoWork);
            _inspectWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_inspectWorker_RunWorkerCompleted);
            _inspectWorker.ProgressChanged += new ProgressChangedEventHandler(_inspectWorker_ProgressChanged);

            _writeWorker = new BackgroundWorker();
            _writeWorker.DoWork += new DoWorkEventHandler(_writeWorker_DoWork);
            _writeWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_writeWorker_RunWorkerCompleted);
            _writeWorker.ProgressChanged += new ProgressChangedEventHandler(_writeWorker_ProgressChanged);
        }

        #region WriteWorker Event

        class TagConfigRecordComparer : IEqualityComparer<JHSchool.Data.JHTagConfigRecord>
        {
            #region IEqualityComparer<TagConfigRecord> 成員

            public bool Equals(JHSchool.Data.JHTagConfigRecord x, JHSchool.Data.JHTagConfigRecord y)
            {
                return x.FullName.Equals(y.FullName);
            }

            public int GetHashCode(JHSchool.Data.JHTagConfigRecord obj)
            {
                return obj.GetHashCode();
            }

            #endregion
        }

        class StudentTagRecordComparer : IEqualityComparer<JHSchool.Data.JHStudentTagRecord>
        {
            #region IEqualityComparer<StudentTagRecord> 成員

            public bool Equals(JHSchool.Data.JHStudentTagRecord x, JHSchool.Data.JHStudentTagRecord y)
            {
                return (x.RefEntityID == y.RefEntityID && x.RefTagID == y.RefTagID);
            }

            public int GetHashCode(JHSchool.Data.JHStudentTagRecord obj)
            {
                return obj.GetHashCode();
            }

            #endregion
        }

        private void _writeWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void _writeWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblProgress.Text = "審查完成";

            btnExit.Enabled = true;
            btnExport.Visible = true;

            int passed = 0;
            int failed = 0;
            foreach (string id in _passList.Keys)
            {
                if (_passList[id]) passed++;
                else failed++;
            }
            lblMessage.Text = "準畢業生人數：" + _passList.Count + "\n取得畢業資格人數：" + passed + "\n未達畢業標準人數：" + failed;

            lblProgress.Visible = false;
            progressBar.Visible = false;
            lblMessage.Visible = true;
            pic_loading.Visible = false;
        }

        private void _writeWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 寫入離校資訊
            try
            {                
                List<JHLeaveInfoRecord> leaveInfoRecordList = new List<JHLeaveInfoRecord>();
                foreach (JHLeaveInfoRecord record in JHLeaveIfno.SelectByStudentIDs(_students.AsKeyList()))
                {
                    if (!_passList.ContainsKey(record.RefStudentID)) continue;

                    leaveInfoRecordList.Add(record);                                       

                    if (_passList[record.RefStudentID] == true)
                    {
                        record.Reason = "畢業";
                        record.Memo = string.Empty;
                    }
                    else
                    {
                        record.Reason = "修業";

                        StringBuilder builder = new StringBuilder();
                        _result[record.RefStudentID].Sort();
                        foreach (ResultDetail rd in _result[record.RefStudentID])
                        {
                            string semester = rd.GradeYear + (rd.Semester.Equals("1") ? "上" : "下") + ":";
                            string details = string.Empty;
                            foreach (string detail in rd.Details)
                                details += detail + ",";
                            if (details.EndsWith(",")) details = details.Substring(0, details.Length - 1);
                            builder.AppendLine(semester + details);
                        }
                        record.Memo = builder.ToString();
                    }
                    
                    // 系統目前預設學年度
                    int SchoolYear;
                    int.TryParse(JHSchool.School.DefaultSchoolYear, out SchoolYear);
                    record.SchoolYear = SchoolYear;
                                    }                
                JHLeaveIfno.Update(leaveInfoRecordList);
            }
            catch (Exception ex)
            {
                BugReporter.ReportException(ex, false);
            }

            #endregion

            #region 寫入類別(Tags)
            string FailurePrefix = "未達畢業標準";
            try
            {
                List<JHTagConfigRecord> tagList = JHTagConfig.SelectByCategory(K12.Data.TagCategory.Student);
                TagConfigRecordComparer tagComparer = new TagConfigRecordComparer();
                StudentTagRecordComparer studentTagComparer = new StudentTagRecordComparer();

                #region 製作各種未達畢業標準的標籤
                List<JHTagConfigRecord> tagConfigRecordList = new List<JHSchool.Data.JHTagConfigRecord>();
                foreach (StudentRecord student in _students)
                {
                    //如果學生通過畢業審查
                    if (_passList[student.ID]) continue;

                    //如果審查結果沒有該學生編號則跳到下一筆 (這應該不會發生才對)
                    if (!_result.ContainsKey(student.ID)) continue;

                    List<ResultDetail> rdList = _result[student.ID];
                    foreach (ResultDetail rd in rdList)
                    {
                        foreach (string msg in rd.Messages)
                        {
                            JHTagConfigRecord tagConfigRecord = new JHTagConfigRecord();
                            tagConfigRecord.Prefix = FailurePrefix;
                            tagConfigRecord.Name = msg;
                            tagConfigRecord.Category = "Student";
                            if (!tagList.Contains(tagConfigRecord, tagComparer))
                            {
                                tagConfigRecordList.Add(tagConfigRecord);
                                tagList.Add(tagConfigRecord);
                                //foreach (string tag_id in JHSchool.Data.TagConfig.Insert(tagConfigRecord))
                                //{
                                //    if (!tagIDs.ContainsKey(tagConfigRecord.FullName))
                                //        tagIDs.Add(tagConfigRecord.FullName, tag_id);
                                //}
                            }
                        }
                    }
                }

                JHTagConfig.Insert(tagConfigRecordList);
                tagList = JHTagConfig.SelectByCategory(K12.Data.TagCategory.Student);
                #endregion

                #region 貼上標籤
                List<JHStudentTagRecord> studentTagsList = new List<JHStudentTagRecord>();
                List<JHStudentTagRecord> deleteList = new List<JHStudentTagRecord>();
                foreach (StudentRecord student in _students)
                {
                    //如果學生通過畢業審查，應該把標籤拿下來
                    if (_passList[student.ID])
                    {
                        foreach (JHStudentTagRecord student_tag in JHStudentTag.SelectByStudentID(student.ID))
                        {
                            if (student_tag.Prefix == FailurePrefix)
                                deleteList.Add(student_tag);
                        }
                        continue;
                    }

                    if (!_result.ContainsKey(student.ID)) continue;

                    Dictionary<string, JHStudentTagRecord> studentTags = new Dictionary<string, JHStudentTagRecord>();
                    foreach (JHStudentTagRecord student_tag in JHStudentTag.SelectByStudentID(student.ID))
                    {
                        if (!studentTags.ContainsKey(student_tag.RefTagID))
                            studentTags.Add(student_tag.RefTagID, student_tag);
                    }

                    List<ResultDetail> rdList = _result[student.ID];
                    foreach (ResultDetail rd in rdList)
                    {
                        foreach (string msg in rd.Messages)
                        {
                            string fullname = FailurePrefix + ":" + msg;

                            foreach (JHTagConfigRecord record in tagList)
                            {
                                if (record.FullName == fullname && !studentTags.ContainsKey(record.ID))
                                {
                                    JHStudentTagRecord r = new JHStudentTagRecord();
                                    r.RefTagID = record.ID;
                                    r.RefEntityID = student.ID;
                                    if (!studentTagsList.Contains(r, studentTagComparer))
                                        studentTagsList.Add(r);
                                }
                            }
                        }
                    }
                }

                JHStudentTag.Insert(studentTagsList);
                JHStudentTag.Delete(deleteList);
                #endregion
            }
            catch (Exception ex)
            {
                BugReporter.ReportException(ex, false);
            }

            #endregion
        }
        #endregion

        #region InspectWorker Event

        private void _inspectWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void _inspectWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Value = 100;

            if (e.Error != null)
            {
                btnExit.Enabled = true;
                MsgBox.Show("審查時發生錯誤。" + e.Error.Message);
                pic_loading.Visible = false;
                return;
            }

            //lblProgress.Text = "審查完成";

            if (_passList.Count > 0)
            {
                //lblProgress.Text = "寫入類別資訊…";
                _writeWorker.RunWorkerAsync();
            }

            //btnExit.Enabled = true;
            //btnExport.Visible = true;

            //int passed = 0;
            //int failed = 0;
            //foreach (string id in _passList.Keys)
            //{
            //    if (_passList[id]) passed++;
            //    else failed++;
            //}
            //lblMessage.Text = "準畢業生人數：" + _passList.Count + "\n取得畢業資格人數：" + passed + "\n未達畢業標準人數：" + failed;

            //lblProgress.Visible = false;
            //progressBar.Visible = false;
            //lblMessage.Visible = true;
        }

        private void _inspectWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //object[] objs = e.Argument as object[];
                //Dictionary<string, bool> passList = objs[0] as Dictionary<string, bool>;
                //EvaluationResult result = objs[1] as EvaluationResult;

                //Dictionary<string, bool> list = Graduation.Instance.Evaluate(e.Items);
                //foreach (string id in list.Keys)
                //{
                //    if (!_passList.ContainsKey(id))
                //        _passList.Add(id, list[id]);
                //}

                _passList = Graduation.Instance.Evaluate(_students);

                if (Graduation.Instance.Result.Count > 0) MergeResult(_passList, Graduation.Instance.Result, _result);
            }
            catch (Exception ex)
            {
                MsgBox.Show("審查發生錯誤。" + ex.Message);
            }
        }

        #endregion

        #region HistoryWorker Event

        private void _historyWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void _historyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Value = 100;

            if (e.Error != null)
            {
                MsgBox.Show("檢查學期歷程時發生錯誤。" + e.Error.Message);
                return;
            }

            if (_errorList.Count > 0)
            {
                btnExit.Enabled = true;

                JHSchool.Evaluation.Calculation_for_JHEvaluation.ScoreCalculation.ErrorViewer viewer = new JHSchool.Evaluation.Calculation_for_JHEvaluation.ScoreCalculation.ErrorViewer();
                viewer.SetHeader("學生");
                foreach (StudentRecord student in _errorList)
                    viewer.SetMessage(student, new List<string>(new string[] { "學期歷程不完整" }));
                viewer.ShowDialog();
                pic_loading.Visible = false;
                return;
            }
            else
            {                
                // 加入這段主要在處理當學期還沒有產生學期歷程，資料可以判斷
                UIConfig._StudentSHistoryRecDict.Clear();
                Dictionary<string, int> studGradeYearDict = new Dictionary<string, int>();

                // 取得學生ID
                List<string> studIDs = (from data in _students select data.ID).Distinct().ToList();

                foreach (JHStudentRecord stud in JHStudent.SelectByIDs(studIDs))
                {
                    if (stud.Class != null)
                    {
                        if (stud.Class.GradeYear.HasValue)
                            if (!studGradeYearDict.ContainsKey(stud.ID))
                                studGradeYearDict.Add(stud.ID, stud.Class.GradeYear.Value);
                    }
                }
                bool checkInsShi = false;
                // 取得學生學期歷程，並加入學生學習歷程Cache
                foreach (JHSemesterHistoryRecord rec in JHSemesterHistory.SelectByStudentIDs(studIDs))
                {
                    checkInsShi = true;
                    K12.Data.SemesterHistoryItem shi = new K12.Data.SemesterHistoryItem();
                    shi.SchoolYear = UIConfig._UserSetSHSchoolYear;
                    shi.Semester = UIConfig._UserSetSHSemester;
                    if (studGradeYearDict.ContainsKey(rec.RefStudentID))
                        shi.GradeYear = studGradeYearDict[rec.RefStudentID];

                    foreach (K12.Data.SemesterHistoryItem shiItem in rec.SemesterHistoryItems)
                        if (shiItem.SchoolYear == shi.SchoolYear && shiItem.Semester == shi.Semester)
                            checkInsShi = false;
                    if (checkInsShi)
                        rec.SemesterHistoryItems.Add(shi);

                    if (!UIConfig._StudentSHistoryRecDict.ContainsKey(rec.RefStudentID))
                        UIConfig._StudentSHistoryRecDict.Add(rec.RefStudentID, rec);
                }


                lblProgress.Text = "畢業資格審查中…";
                FISCA.LogAgent.ApplicationLog.Log("成績系統.畢業資格審查", "畢業資格審查", "進行畢業資格審查");
                _inspectWorker.RunWorkerAsync();
            }
        }

        private void _historyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _errorList.AddRange(Graduation.Instance.CheckSemesterHistories(_students));
            }
            catch (Exception ex)
            {
                MsgBox.Show("檢查學期歷程時發生錯誤。" + ex.Message);
                BugReporter.ReportException(ex, true);
            }
        }

        #endregion

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // 取得使用者選擇學年度學期
            UIConfig._UserSetSHSchoolYear = iptSchoolYear.Value;
            UIConfig._UserSetSHSemester = iptSemester.Value;

            if (UIConfig._UserSetSHSchoolYear == 0 || UIConfig._UserSetSHSemester == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("日前學年度或學期輸入錯誤.");
                return;
            }

            if (!_gradeYearSelected)
            {
                lblGradeYear.Visible = false;
                intGradeYear.Visible = false;

                foreach (StudentRecord each in Student.Instance.Items)
                {
                    if (each.Status != "一般") continue;
                    if (each.Class == null) continue;
                    if (each.Class.GradeYear == "" + intGradeYear.Value)
                        _students.Add(each);
                }

                InitializeMessage(_students.Count);
                lblMessage.Visible = true;

                _gradeYearSelected = true;
            }
            else
            {
                lblMessage.Visible = false;
                lblProgress.Visible = true;
                progressBar.Visible = true;
                lblProgress.Text = "檢查學期歷程…";
                btnNext.Enabled = false;
                btnNext.Visible = false;
                btnExit.Enabled = false;
                pic_loading.Visible = true;
                _historyWorker.RunWorkerAsync();
            }
        }

        private void MergeResult(Dictionary<string, bool> passList, EvaluationResult sourceResult, EvaluationResult targetResult)
        {
            foreach (string student_id in sourceResult.Keys)
            {
                if (passList.ContainsKey(student_id) && passList[student_id]) continue;
                targetResult.MergeResults(student_id, sourceResult[student_id]);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (_result.Count <= 0) return;

            SaveFileDialog sd = new SaveFileDialog();
            sd.FileName = "未達畢業標準學生名冊";
            sd.Filter = "Excel檔案(*.xls)|*.xls";
            if (sd.ShowDialog() != DialogResult.OK) return;
            
            Workbook template = new Workbook();
            template.Open(new MemoryStream(JHEvaluation.ScoreCalculation.Properties.Resources.未達畢業標準學生名冊template));
            Worksheet tempsheet = template.Worksheets[0];
            Worksheet tempsheet2 = template.Worksheets[1];
            
            Workbook book = new Workbook();
            book.Open(new MemoryStream(JHEvaluation.ScoreCalculation.Properties.Resources.未達畢業標準學生名冊template));
            //Worksheet sheet = book.Worksheets[0];
            //sheet.Name = "未達畢業標準學生";
            Range temprow = tempsheet.Cells.CreateRange(3, 1, false);
            Range temprow2 = tempsheet2.Cells.CreateRange(3, 1, false);

            book.Worksheets[0].Cells[0, 0].PutValue(School.DefaultSchoolYear + "學年度 " + School.ChineseName);
            book.Worksheets[1].Cells[0, 0].PutValue(School.DefaultSchoolYear + "學年度 " + School.ChineseName);

            int rowIndex = 3;
            int sheet2_Index = 3;

            List<StudentRecord> sorted = new List<StudentRecord>();
            foreach (string id in _result.Keys)
                sorted.Add(Student.Instance.Items[id]);
            sorted.Sort();

            
            foreach (StudentRecord stu in sorted)
            {
                if (!_result.ContainsKey(stu.ID)) continue;

                //正常年級的清單
                List<ResultDetail> regularGrades = new List<ResultDetail>();
                //年級為0的清單
                List<ResultDetail> zeroGrades = new List<ResultDetail>();

                foreach (ResultDetail rd in _result[stu.ID])
                {
                    int gradeYear = int.Parse(rd.GradeYear);

                    //分類result
                    if (gradeYear == 0)
                    {
                        zeroGrades.Add(rd);
                    }
                    else
                    {
                        regularGrades.Add(rd);
                    }
                }

                if (regularGrades.Count > 0)
                {
                    Worksheet sheet = book.Worksheets[0];
                    sheet.Cells.CreateRange(rowIndex, 1, false).Copy(temprow);
                    sheet.Cells.CreateRange(rowIndex, 1, false).CopyStyle(temprow);

                    //學生資訊
                    if (stu.Class != null) sheet.Cells[rowIndex, 0].PutValue(stu.Class.Name);
                    sheet.Cells[rowIndex, 1].PutValue(stu.SeatNo);
                    sheet.Cells[rowIndex, 2].PutValue(stu.StudentNumber);
                    sheet.Cells[rowIndex, 3].PutValue(stu.Name);

                    foreach (ResultDetail rd in regularGrades)
                    {
                        int gradeYear = int.Parse(rd.GradeYear);

                        if (gradeYear > 6) gradeYear -= 6;

                        int index = (gradeYear - 1) * 2 + int.Parse(rd.Semester);

                        string details = string.Empty;
                        details += string.Join(",", rd.Details);
                        //foreach (string detail in rd.Details)
                            //details += detail + ",";
                        //if (details.EndsWith(",")) details = details.Substring(0, details.Length - 1);
                        sheet.Cells[rowIndex, index + 3].PutValue(details);
                    }

                    rowIndex++;
                }

                if(zeroGrades.Count > 0)
                {
                    //處理年級為0的result
                    foreach (ResultDetail rd in zeroGrades)
                    {
                        //學期也為0代表是所有學期
                        if (rd.Semester == "0")
                        {
                            Worksheet sheet = book.Worksheets[1];
                            sheet.Cells.CreateRange(sheet2_Index, 1, false).Copy(temprow2);
                            sheet.Cells.CreateRange(sheet2_Index, 1, false).CopyStyle(temprow2);

                            if (stu.Class != null) sheet.Cells[sheet2_Index, 0].PutValue(stu.Class.Name);
                            sheet.Cells[sheet2_Index, 1].PutValue(stu.SeatNo);
                            sheet.Cells[sheet2_Index, 2].PutValue(stu.StudentNumber);
                            sheet.Cells[sheet2_Index, 3].PutValue(stu.Name);

                            string val = "";
                            val += string.Join(",", rd.Details);
                            //foreach (string str in rd.Details)
                            //{
                            //    val += str + ",";
                            //}
                            //if (val.EndsWith(",")) val = val.Substring(0, val.Length - 1);
                            sheet.Cells[sheet2_Index, 4].PutValue(val);
                        }
                    }

                    sheet2_Index++;
                }
                

                //foreach (ResultDetail rd in _result[stu.ID])
                //{
                //    int gradeYear = int.Parse(rd.GradeYear);

                //    //年級為0的後續再處理
                //    if (gradeYear == 0)
                //    {
                //        zeroGrades.Add(rd);
                //        continue;
                //    }
                        
                //    if (gradeYear > 6) gradeYear -= 6;

                //    int index = (gradeYear - 1) * 2 + int.Parse(rd.Semester);

                //    string details = string.Empty;
                //    foreach (string detail in rd.Details)
                //        details += detail + ",";
                //    if (details.EndsWith(",")) details = details.Substring(0, details.Length - 1);
                //    sheet.Cells[rowIndex, index + 3].PutValue(details);
                //}

                
            }

            //sheet.AutoFitColumns();

            try
            {
                book.Save(sd.FileName, FileFormatType.Excel2003);

                if (MsgBox.Show("匯出完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(sd.FileName);
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show("匯出失敗。" + ex.Message);
            }
        }

        private void GraduationInspectWizard_Load(object sender, EventArgs e)
        {

            this.MaximumSize = this.MinimumSize = this.Size;

            int schoolYear = int.Parse(School.DefaultSchoolYear);
            int semester = int.Parse(School.DefaultSemester);
            iptSchoolYear.Value = schoolYear;
            iptSemester.Value = semester;

        }
    }
}