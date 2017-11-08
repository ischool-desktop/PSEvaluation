using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CW = KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizard;
using JHSchool.Evaluation.StudentExtendControls.Ribbon.CalculationWizard;
using JHSchool.Evaluation.Calculation;
using JHSchool.Evaluation.Editor;
using JHSchool.Editor;
using Framework;
using System.Xml;
using KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizard;
using JHSchool;
using JHSchool.Data;
using JHSchool.Evaluation;

namespace KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon
{
    public partial class CalculationWizardStudent : FISCA.Presentation.Controls.BaseForm, KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizard.IProgressUI
    {
        private BackgroundWorker _current;
        private BackgroundWorker _calc_worker, _upload_worker, _history_worker;
        private CourseDataLoader _raw_data;
        private InnerScoreCalculator _inner_calculator;
        private ErrorViewer _viewer;
        private string _type;
        private Dictionary<string, int> _gradeYears;
        private List<StudentRecord> _students;
        private Dictionary<string, JHSchool.Data.JHSemesterHistoryRecord> _studentHistoryDict;
        private Dictionary<int, int> _schoolDayMapping;

        public CalculationWizardStudent(string type)
        {
            InitializeComponent();
            InitializeSemester();
            InitializeGradeYear();

            _schoolDayMapping = new Dictionary<int, int>();
            ConfigData cd = School.Configuration["SCHOOL_HOLIDAY_CONFIG_STRING"];
            if (!string.IsNullOrEmpty(cd["CONFIG_STRING"]))
            {
                XmlElement xml = XmlHelper.LoadXml(cd["CONFIG_STRING"]);
                XmlElement g;
                g = (XmlElement)xml.SelectSingleNode("SchoolDayCountG1");
                if (g != null) _schoolDayMapping.Add(1, int.Parse(g.InnerText));
                g = (XmlElement)xml.SelectSingleNode("SchoolDayCountG2");
                if (g != null) _schoolDayMapping.Add(2, int.Parse(g.InnerText));
                g = (XmlElement)xml.SelectSingleNode("SchoolDayCountG3");
                if (g != null) _schoolDayMapping.Add(3, int.Parse(g.InnerText));
            }

            _type = type;
            if (type.Equals("Student"))
            {
                panelStudent.Visible = true;
                wizardPage1.PageTitle = "選擇學年度學期";
                wizardPage2.NextButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.False;
                wizardPage2.NextButtonVisible = DevComponents.DotNetBar.eWizardButtonState.False;
            }
            else if (type.Equals("EduAdmin"))
            {
                panelEduAdmin.Visible = true;
                wizardPage1.PageTitle = "選擇年級";
            }
            InitializeBackgroundWorker();
            InitializeCalculator();
            InitializeErrorViewer();
        }

        /// <summary>
        /// 初始化年級
        /// </summary>
        private void InitializeGradeYear()
        {
            intGradeYear.Value = 1;
        }

        /// <summary>
        /// 初始化學年度學期
        /// </summary>
        private void InitializeSemester()
        {
            try
            {
                intSchoolYear.Value = int.Parse(School.DefaultSchoolYear);
                intSemester.Value = int.Parse(School.DefaultSemester);
            }
            catch (Exception ex)
            {
                intSchoolYear.Value = intSchoolYear.MinValue;
                intSemester.Value = intSemester.MinValue;
            }
        }

        /// <summary>
        /// 初始化 BackgroundWorker
        /// </summary>
        private void InitializeBackgroundWorker()
        {
            _history_worker = new BackgroundWorker();
            _history_worker.DoWork += new DoWorkEventHandler(History_worker_DoWork);
            _history_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(History_worker_RunWorkerCompleted);
            _history_worker.ProgressChanged += new ProgressChangedEventHandler(History_worker_ProgressChanged);
            _history_worker.WorkerReportsProgress = true;
            _history_worker.WorkerSupportsCancellation = true;

            _calc_worker = new BackgroundWorker();
            _calc_worker.DoWork += new DoWorkEventHandler(CalcWorker_DoWork);
            _calc_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CalcWorker_RunWorkerCompleted);
            _calc_worker.ProgressChanged += new ProgressChangedEventHandler(CalcWorker_ProgressChanged);
            _calc_worker.WorkerReportsProgress = true;
            _calc_worker.WorkerSupportsCancellation = true;

            _upload_worker = new BackgroundWorker();
            _upload_worker.DoWork += new DoWorkEventHandler(UploadWorker_DoWork);
            _upload_worker.ProgressChanged += new ProgressChangedEventHandler(UploadWorker_ProgressChanged);
            _upload_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UploadWorker_RunWorkerCompleted);
            _upload_worker.WorkerReportsProgress = true;
        }

        /// <summary>
        /// 初始化計算機
        /// </summary>
        private void InitializeCalculator()
        {
            _inner_calculator = new InnerScoreCalculator();
        }

        /// <summary>
        /// 初始化訊息瀏覽器
        /// </summary>
        private void InitializeErrorViewer()
        {
            _viewer = new ErrorViewer();
        }

        #region HistoryWorker Event
        private void History_worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!Cancellation)
            {
                labelX4.Text = "" + e.UserState;
                progressBarX1.Value = e.ProgressPercentage;
            }
        }

        private void History_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                wizardPage2.NextButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.True;
                wizardPage2.BackButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.True;

                if (e.Result is bool)
                {
                    lnErrorViewHistory.Visible = true;
                    if (_type.Equals("Student"))
                    {
                        labelX4.Text = "學期歷程有誤";
                        _viewer.ShowDialog();
                    }
                    else if (_type.Equals("EduAdmin"))
                    {
                        labelX5.Visible = true;
                    }
                }
                else
                {
                    wizard1.SelectedPage = wizardPage3;
                }
            }
            else
            {

            }
        }

        private void History_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool error = false;

            List<StudentRecord> students = _students;
            List<string> student_ids = new List<string>();
            foreach (StudentRecord each in students)
                student_ids.Add(each.ID);
            int total = students.Count;
            int count = 0;

            #region 檢查學期歷程
            _viewer.Clear();
            _viewer.SetHeader("學生");

            Dictionary<string, JHSchool.Data.JHSemesterHistoryRecord> studentHistories = new Dictionary<string, JHSchool.Data.JHSemesterHistoryRecord>();
            foreach (JHSchool.Data.JHSemesterHistoryRecord record in JHSchool.Data.JHSemesterHistory.SelectByStudentIDs(student_ids))
            {
                if (!studentHistories.ContainsKey(record.RefStudentID))
                    studentHistories.Add(record.RefStudentID, record);
            }

            _studentHistoryDict = new Dictionary<string, JHSchool.Data.JHSemesterHistoryRecord>();
            //SemesterHistory.Instance.SyncDataBackground(student_ids);
            //_studentHistoryEditors = new Dictionary<string, SemesterHistoryRecordEditor>();

            _gradeYears = new Dictionary<string, int>();

            foreach (StudentRecord each in students)
            {
                count++;

                bool hasSemesterHistory = false;
                foreach (K12.Data.SemesterHistoryItem item in studentHistories[each.ID].SemesterHistoryItems)
                {
                    if (item.SchoolYear == intSchoolYear.Value && item.Semester == intSemester.Value)
                    {
                        if (!_gradeYears.ContainsKey(each.ID))
                            _gradeYears.Add(each.ID, item.GradeYear);

                        if (_type.Equals("EduAdmin"))
                        {
                            item.ClassName = (each.Class != null) ? each.Class.Name : "";
                            int i;
                            item.SeatNo = int.TryParse(each.SeatNo, out i) ? (int?)i : null;
                            item.Teacher = (each.Class != null) ? ((each.Class.Teacher != null) ? each.Class.Teacher.Name : "") : "";
                            item.SchoolDayCount = (_schoolDayMapping.ContainsKey(item.GradeYear)) ? (int?)_schoolDayMapping[item.GradeYear] : null;
                            if (!_studentHistoryDict.ContainsKey(each.ID))
                                _studentHistoryDict.Add(each.ID, studentHistories[each.ID]);
                        }

                        hasSemesterHistory = true;
                        break;
                    }
                }

                if (!hasSemesterHistory)
                {
                    if (_type.Equals("EduAdmin"))
                    {
                        K12.Data.SemesterHistoryItem item = new K12.Data.SemesterHistoryItem();
                        item.SchoolYear = intSchoolYear.Value;
                        item.Semester = intSemester.Value;
                        item.GradeYear = intGradeYear.Value;
                        item.ClassName = (each.Class != null) ? each.Class.Name : "";
                        int i;
                        item.SeatNo = int.TryParse(each.SeatNo, out i) ? (int?)i : null;
                        item.Teacher = (each.Class != null) ? ((each.Class.Teacher != null) ? each.Class.Teacher.Name : "") : "";
                        item.SchoolDayCount = (_schoolDayMapping.ContainsKey(item.GradeYear)) ? (int?)_schoolDayMapping[item.GradeYear] : null;
                        studentHistories[each.ID].SemesterHistoryItems.Add(item);
                        if (!_studentHistoryDict.ContainsKey(each.ID))
                            _studentHistoryDict.Add(each.ID, studentHistories[each.ID]);
                    }
                    error = true;
                    _viewer.SetMessage(each, new List<string>(new string[] { "缺少學期歷程資訊" }));
                }

                _history_worker.ReportProgress((int)((double)count * 100 / (double)total), "檢查學期歷程中…");
            }

            if (error)
            {
                e.Result = error;
                return;
            }

            #endregion

            e.Result = "OK";
        }
        #endregion

        #region CalcWorker Event
        private void CalcWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool error = false;

            List<StudentRecord> students = _students;
            List<string> student_ids = new List<string>();
            foreach (StudentRecord each in students)
                student_ids.Add(each.ID);
            double total = students.Count;
            double count = 0;

            #region 寫入學期歷程
            if (_studentHistoryDict != null && _studentHistoryDict.Count > 0)
            {
                try
                {
                    List<JHSchool.Data.JHSemesterHistoryRecord> list = new List<JHSchool.Data.JHSemesterHistoryRecord>();
                    int size = 50;
                    double count2 = 0;
                    double total2 = _studentHistoryDict.Count;
                    foreach (JHSchool.Data.JHSemesterHistoryRecord record in _studentHistoryDict.Values)
                    {
                        if (_calc_worker.CancellationPending) return;

                        list.Add(record);
                        count2++;
                        if (list.Count == size)
                        {
                            JHSchool.Data.JHSemesterHistory.Update(list);
                            list.Clear();
                            _calc_worker.ReportProgress((int)(count2 * 100 / total2), "寫入學期歷程中…");
                        }
                    }
                    if (list.Count > 0)
                    {
                        JHSchool.Data.JHSemesterHistory.Update(list);
                        list.Clear();
                        _calc_worker.ReportProgress(100, "學期歷程寫入完成");
                    }
                }
                catch (Exception ex)
                {
                    MsgBox.Show("寫入學期歷程失敗。" + ex.Message);
                    e.Result = null;
                    return;
                }
            }
            #endregion

            #region 計算課程成績

            _viewer.Clear();
            _viewer.SetHeader("課程");

            _calc_worker.ReportProgress(0, "計算課程成績…");

            if (_calc_worker.CancellationPending) return;
            _raw_data = new CourseDataLoader();
            _raw_data.LoadCalculationData(this, _students, intSchoolYear.Value, intSemester.Value);

            foreach (CW.Course course in _raw_data.Courses.Values)
            {
                if (string.IsNullOrEmpty(course.ExamTemplateId) && course.ExamRequired)
                {
                    _viewer.SetMessage(course.CourseName, new List<string>(new string[] { "缺少評量設定" }));
                    error = true;
                }
            }

            if (error)
            {
                e.Result = error;
                return;
            }

            _viewer.Clear();
            if (_calc_worker.CancellationPending) return;
            CourseScoreCalculate calculate = new CourseScoreCalculate(_raw_data.Courses);
            calculate.Calculate();

            foreach (CW.Course course in _raw_data.Courses.Values)
            {
                Dictionary<string, string> examLacks = new Dictionary<string, string>();
                Dictionary<string, int> examScoreLacks = new Dictionary<string, int>();
                Dictionary<string, int> examEffortLacks = new Dictionary<string, int>();

                List<CW.SCAttend> scattends = new List<CW.SCAttend>();
                List<CW.SCAttend> no_exam_scattends = new List<CW.SCAttend>();
                foreach (CW.SCAttend scattend in course.SCAttends.Values)
                {
                    if (student_ids.Contains(scattend.StudentIdentity))
                    {
                        if (scattend.ContainsLack)
                            scattends.Add(scattend);
                        if (scattend.NoExam)
                            no_exam_scattends.Add(scattend);
                    }
                }

                foreach (CW.SCAttend scattend in scattends)
                {
                    foreach (string exam in scattend.ScoreLack)
                    {
                        if (!examLacks.ContainsKey(exam))
                            examLacks.Add(exam, "");

                        if (!examScoreLacks.ContainsKey(exam))
                            examScoreLacks.Add(exam, 0);
                        examScoreLacks[exam]++;
                    }

                    foreach (string exam in scattend.EffortLack)
                    {
                        if (!examLacks.ContainsKey(exam))
                            examLacks.Add(exam, "");

                        if (!examEffortLacks.ContainsKey(exam))
                            examEffortLacks.Add(exam, 0);
                        examEffortLacks[exam]++;
                    }
                }

                if (scattends.Count > 0)
                {
                    List<string> msgs = new List<string>();
                    foreach (string exam in new List<string>(examLacks.Keys))
                    {
                        if (examScoreLacks.ContainsKey(exam))
                            examLacks[exam] += "有" + examScoreLacks[exam] + "位學生缺少分數評量,";
                        if (examEffortLacks.ContainsKey(exam))
                            examLacks[exam] += "有" + examEffortLacks[exam] + "位學生缺少努力程度,";
                        if (!string.IsNullOrEmpty(examLacks[exam]))
                            examLacks[exam] = examLacks[exam].Substring(0, examLacks[exam].Length - 1);

                        msgs.Add(exam + ":" + examLacks[exam]);
                    }
                    _viewer.SetMessage(course.CourseName, msgs);
                    error = true;
                }

                if (no_exam_scattends.Count > 0)
                {
                    _viewer.SetMessage(course.CourseName, new List<string>(new string[] { "沒有設定各次評量" }));
                    error = true;
                }
            }

            if (error)
            {
                e.Result = true;
                return;
            }

            if (_calc_worker.CancellationPending) return;
            CourseScoreUpdater updater = new CourseScoreUpdater(_raw_data.Courses, this, false);
            updater.UpdateToServer();
            #endregion

            #region 計算學期成績

            //List<SemesterScoreRecordEditor> editors = new List<SemesterScoreRecordEditor>();
            List<JHSemesterScoreRecord> semesterScoreRecordList = new List<JHSchool.Data.JHSemesterScoreRecord>();

            JHSchool.Evaluation.SCAttend.Instance.SyncAllBackground();

            Dictionary<string, List<JHSemesterScoreRecord>> studentSemesterScoreCache = new Dictionary<string, List<JHSchool.Data.JHSemesterScoreRecord>>();
            foreach (JHSemesterScoreRecord record in JHSemesterScore.SelectByStudentIDs(students.AsKeyList()))
            {
                if (!studentSemesterScoreCache.ContainsKey(record.RefStudentID))
                    studentSemesterScoreCache.Add(record.RefStudentID, new List<JHSchool.Data.JHSemesterScoreRecord>());
                studentSemesterScoreCache[record.RefStudentID].Add(record);
            }

            count = 0;
            foreach (StudentRecord each in students)
            {
                count++;

                ScoreCalcRuleRecord old = GetScoreCalcRuleRecord(each);
                JHScoreCalcRuleRecord dalrecord = null;
                if (old != null)
                {
                    List<JHScoreCalcRuleRecord> list = JHScoreCalcRule.SelectByIDs(new string[] { old.ID });
                    if (list.Count > 0) dalrecord = list[0];
                }
                ScoreCalculator calculator = new ScoreCalculator(dalrecord);

                List<SCAttendRecord> scattends = new List<SCAttendRecord>();
                foreach (SCAttendRecord scattend in JHSchool.Evaluation.SCAttend.Instance.GetStudentAttend(each.ID))
                {
                    CourseRecord course = scattend.Course;
                    if (course.SchoolYear == intSchoolYear.Value &&
                        course.Semester == intSemester.Value &&
                        !string.IsNullOrEmpty(course.RefAssessmentSetupID) &&
                        course.CalculationFlag == "1"
                        )
                        scattends.Add(scattend);
                }

                if (scattends.Count > 0)
                {
                    List<K12.Data.SubjectScore> subjectScores = _inner_calculator.CalculateSubjectScore(scattends);
                    foreach (K12.Data.SubjectScore subject in subjectScores)
                        subject.Score = calculator.ParseSubjectScore((decimal)subject.Score);
                    List<K12.Data.DomainScore> domainScores = _inner_calculator.CalculateDomainScore(subjectScores);
                    foreach (K12.Data.DomainScore domain in domainScores)
                        domain.Score = calculator.ParseDomainScore((decimal)domain.Score);

                    List<K12.Data.DomainScore> domainListWithoutElastic = new List<K12.Data.DomainScore>();
                    bool hasElasticCourse = false;
                    foreach (K12.Data.DomainScore domain in domainScores)
                    {
                        if (domain.Domain == "彈性課程")
                            hasElasticCourse = true;
                        else
                            domainListWithoutElastic.Add(domain);
                    }

                    decimal? learnDomainScore = calculator.ParseLearnDomainScore(_inner_calculator.CalculateTotalDomainScore(domainListWithoutElastic));
                    decimal? courseLearnScore = null;
                    if (hasElasticCourse)
                        courseLearnScore = calculator.ParseLearnDomainScore(_inner_calculator.CalculateTotalDomainScore(domainScores));

                    JHSemesterScoreRecord current = null;
                    if (studentSemesterScoreCache.ContainsKey(each.ID))
                    {
                        foreach (JHSemesterScoreRecord record in studentSemesterScoreCache[each.ID])
                        {
                            if (record.SchoolYear == intSchoolYear.Value && record.Semester == intSemester.Value)
                                current = record;
                        }
                    }

                    if (current != null)
                    {
                        //editor = current.GetEditor();
                        current.Subjects = new Dictionary<string, K12.Data.SubjectScore>();
                        current.Domains = new Dictionary<string, K12.Data.DomainScore>();
                    }
                    else
                    {
                        //current = new SemesterScoreRecordEditor(each, intSchoolYear.Value, intSemester.Value, _gradeYears[each.ID]);
                        current = new JHSchool.Data.JHSemesterScoreRecord();
                        current.RefStudentID = each.ID;
                        current.SchoolYear = intSchoolYear.Value;
                        current.Semester = intSemester.Value;
                    }

                    foreach (K12.Data.SubjectScore subject in subjectScores)
                        current.Subjects.Add(subject.Subject, subject);
                    foreach (K12.Data.DomainScore domain in domainScores)
                        current.Domains.Add(domain.Domain, domain);
                    current.LearnDomainScore = learnDomainScore;
                    current.CourseLearnScore = courseLearnScore;

                    //editors.Add(editor);
                    semesterScoreRecordList.Add(current);
                }

                _calc_worker.ReportProgress((int)((double)count * 100 / (double)total), "計算學期成績…");
            }

            e.Result = semesterScoreRecordList;
            #endregion
        }

        private void CalcWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                if (e.Result is bool)
                {
                    lblMsgCalc.Text = "停止";
                    prgsBarCalc.Value = 0;
                    lnErrorView.Visible = true;
                    _viewer.ShowDialog();
                }
                else
                {
                    List<JHSemesterScoreRecord> list = e.Result as List<JHSemesterScoreRecord>;
                    wizardPage4.FinishButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.False;
                    wizard1.SelectedPage = wizardPage4;
                    _current = _upload_worker;
                    _upload_worker.RunWorkerAsync(list);
                }
            }
            else
            {

            }
        }

        private void CalcWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!Cancellation)
            {
                lblMsgCalc.Text = "" + e.UserState;
                prgsBarCalc.Value = e.ProgressPercentage;
            }
        }
        #endregion

        #region UploadWorker Event
        private void UploadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<JHSemesterScoreRecord> list = e.Argument as List<JHSemesterScoreRecord>;
            int total = list.Count;
            if (total == 0) return;
            int count = 0;
            foreach (JHSemesterScoreRecord record in list)
            {
                count++;
                //editor.Save();
                if (string.IsNullOrEmpty(record.ID))
                    JHSemesterScore.Insert(record);
                else
                    JHSemesterScore.Update(record);

                _upload_worker.ReportProgress((int)((double)count * 100 / (double)total), "上傳學期成績…");
            }
        }

        private void UploadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                wizardPage4.FinishButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.True;
                lblMsgUpload.Text = "上傳完成";
            }
        }

        private void UploadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!Cancellation)
            {
                lblMsgUpload.Text = "" + e.UserState;
                prgsBarUpload.Value = e.ProgressPercentage;
            }
        }
        #endregion

        #region IProgressUI 成員

        public void ReportProgress(string message, int progress)
        {
            _current.ReportProgress(progress, message);
        }

        public void Cancel()
        {

        }

        public bool Cancellation
        {
            get { return _current.CancellationPending; }
        }

        #endregion

        public ScoreCalcRuleRecord GetScoreCalcRuleRecord(StudentRecord studentRec)
        {
            string id = "";
            if (studentRec != null)
                if (string.IsNullOrEmpty(studentRec.OverrideScoreCalcRuleID))
                {
                    if (studentRec.Class != null)
                        return studentRec.Class.GetScoreCalcRuleRecord();
                }
                else
                    id = studentRec.OverrideScoreCalcRuleID;

            return ScoreCalcRule.Instance.Items[id];
        }

        private void wizardPage2_AfterPageDisplayed(object sender, DevComponents.DotNetBar.WizardPageChangeEventArgs e)
        {
            if (_type.Equals("Student"))
            {
                _students = JHSchool.Student.Instance.SelectedList.GetInSchoolStudents();
            }
            else if (_type.Equals("EduAdmin"))
            {
                _students = new List<StudentRecord>();

                foreach (StudentRecord each in JHSchool.Student.Instance.Items)
                {
                    if (each.Class == null) continue;
                    if (each.Class.GradeYear == "" + intGradeYear.Value)
                        _students.Add(each);
                }
            }
            labelX4.Text = "檢查學期歷程中…";
            wizardPage2.NextButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.False;
            wizardPage2.BackButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.False;
            _current = _history_worker;
            _history_worker.RunWorkerAsync();
        }

        private void wizardPage3_AfterPageDisplayed(object sender, DevComponents.DotNetBar.WizardPageChangeEventArgs e)
        {
            lblMsgCalc.Text = "";
            _current = _calc_worker;
            _calc_worker.RunWorkerAsync();
        }

        private void FormClose(object sender, CancelEventArgs e)
        {
            if (_current != null && _current.IsBusy)
                _current.CancelAsync();
            this.Close();
        }

        private void lnErrorView_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _viewer.ShowDialog();
        }

        private void wizardPage3_BackButtonClick(object sender, CancelEventArgs e)
        {
            wizard1.SelectedPage = wizardPage1;
            labelX5.Visible = false;
        }
    }
}
