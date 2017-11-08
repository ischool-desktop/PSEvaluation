using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using System.Threading;

namespace JHEvaluation.ExamScoreCopy
{
    public partial class MainForm : BaseForm
    {
        public static void Run()
        {
            new MainForm().ShowDialog();
        }

        /// <summary>
        /// 讀取課程、修課記錄
        /// </summary>
        private BackgroundWorker CourseWorker { get; set; }
        /// <summary>
        /// 取得在校學生
        /// </summary>
        private BackgroundWorker PreLoader { get; set; }
        /// <summary>
        /// 儲存評量成績
        /// </summary>
        private BackgroundWorker ScoreSaver { get; set; }

        private ManualResetEvent Event { get; set; }
        private Reporter Reporter { get; set; }
        private UserConfig Config { get; set; }

        private List<JHCourseRecord> Courses { get; set; }
        private List<JHStudentRecord> Students { get; set; }

        private int RunningSchoolYear { get; set; }
        private int RunningSemester { get; set; }

        /// <summary>
        /// 選擇的試別名稱
        /// </summary>
        private JHExamRecord Exam
        {
            get
            {
                if (cboExam.SelectedItem != null)
                    return cboExam.SelectedItem as JHExamRecord;
                return null;
            }
        }
        /// <summary>
        /// 選擇的來源科目
        /// </summary>
        private string SourceSubject { get { return cboSource.Text; } }
        /// <summary>
        /// 選擇的目的科目
        /// </summary>
        private List<string> TargetSubjects
        {
            get
            {
                List<string> targets = new List<string>();
                foreach (DataGridViewRow row in dgvTargets.Rows)
                {
                    if (row.IsNewRow) continue;

                    string target = "" + row.Cells[chTargetSubjects.Index].Value;
                    if (string.IsNullOrEmpty(target)) continue;
                    if (target == SourceSubject) continue;
                    if (!targets.Contains(target)) targets.Add(target);
                }
                return targets;
            }
        }

        private bool ControlEnabled
        {
            set
            {
                gpCopy.Enabled = value;
                btnStart.Enabled = value;
            }
        }

        public MainForm()
        {
            InitializeComponent();

            Reporter = new Reporter();
            //FISCA.Presentation.MotherForm.SetStatusBarMessage("初始化...");

            InitializeSubjectRows();
            InitializeBackgroundWorker();
            InitializeSemester();

            PreLoader.RunWorkerAsync();
        }

        private void InitializeSubjectRows()
        {
            for (int i = 0; i < 5; i++)
                dgvTargets.Rows.Add();
        }

        private void InitializeSemester()
        {
            try
            {
                int schoolYear = int.Parse(K12.Data.School.DefaultSchoolYear);

                for (int i = -3; i <= 3; i++)
                    cboSchoolYear.Items.Add(schoolYear + i);
                cboSemester.Items.Add(1);
                cboSemester.Items.Add(2);
            }
            catch (Exception ex)
            {
                MsgBox.Show("初始化學年度學期錯誤");
            }
        }

        private void InitializeBackgroundWorker()
        {
            Event = new ManualResetEvent(false);

            CourseWorker = new BackgroundWorker();
            CourseWorker.DoWork += new DoWorkEventHandler(CourseWorker_DoWork);
            CourseWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CourseWorker_RunWorkerCompleted);
            CourseWorker.ProgressChanged += new ProgressChangedEventHandler(CourseWorker_ProgressChanged);
            CourseWorker.WorkerReportsProgress = true;

            PreLoader = new BackgroundWorker();
            PreLoader.DoWork += new DoWorkEventHandler(PreLoader_DoWork);
            PreLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PreLoader_RunWorkerCompleted);

            ScoreSaver = new BackgroundWorker();
            ScoreSaver.DoWork += new DoWorkEventHandler(ScoreSaver_DoWork);
            ScoreSaver.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ScoreSaver_RunWorkerCompleted);
            ScoreSaver.ProgressChanged += new ProgressChangedEventHandler(ScoreSaver_ProgressChanged);
            ScoreSaver.WorkerReportsProgress = true;
        }

        private void ScoreSaver_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Reporter.Feedback("" + e.UserState, e.ProgressPercentage);
        }

        private void ScoreSaver_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);
                MsgBox.Show("儲存時發生錯誤。" + e.Error.Message);
                return;
            }

            FISCA.Presentation.MotherForm.SetStatusBarMessage("儲存完成");
            MsgBox.Show("複製完成");
        }

        private void ScoreSaver_DoWork(object sender, DoWorkEventArgs e)
        {
            List<JHSCETakeRecord> sceList = new List<JHSCETakeRecord>();

            List<StudentData> sdList = AttendManager.Instance.GetStudentDataList();
            int count = 0;
            foreach (StudentData sd in sdList)
            {
                count++;
                sceList.AddRange(sd.GetCopiedScores(Config.SourceSubject, Config.TargetSubjects));
                ScoreSaver.ReportProgress(Util.CalculatePercentage(sdList.Count, count), "複製評量成績中...");
            }

            List<JHSCETakeRecord> insertList = new List<JHSCETakeRecord>();
            List<JHSCETakeRecord> updateList = new List<JHSCETakeRecord>();
            foreach (JHSCETakeRecord sce in sceList)
            {
                if (string.IsNullOrEmpty(sce.ID)) insertList.Add(sce);
                else updateList.Add(sce);
            }

            if (insertList.Count > 0)
            {
                FunctionSpliter<JHSCETakeRecord, string> spliterInsert = new FunctionSpliter<JHSCETakeRecord, string>(100, Util.MaxThread);
                spliterInsert.Function = delegate(List<JHSCETakeRecord> part)
                {
                    JHSCETake.Insert(part);
                    return null;
                };
                spliterInsert.ProgressChange = delegate(int progress)
                {
                    ScoreSaver.ReportProgress(Util.CalculatePercentage(insertList.Count, progress), "儲存評量成績中...");
                };
                spliterInsert.Execute(insertList);
            }
            if (updateList.Count > 0)
            {
                FunctionSpliter<JHSCETakeRecord, string> spliterUpdate = new FunctionSpliter<JHSCETakeRecord, string>(100, Util.MaxThread);
                spliterUpdate.Function = delegate(List<JHSCETakeRecord> part)
                {
                    JHSCETake.Update(part);
                    return null;
                };
                spliterUpdate.ProgressChange = delegate(int progress)
                {
                    ScoreSaver.ReportProgress(Util.CalculatePercentage(updateList.Count, progress), "儲存評量成績中...");
                };
                spliterUpdate.Execute(updateList);
            }
        }

        private void PreLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cboExam.Items.Clear();
            foreach (JHExamRecord exam in (e.Result as List<JHExamRecord>))
                cboExam.Items.Add(exam);
            FISCA.Presentation.MotherForm.SetStatusBarMessage("");

            Event.Set();
        }

        private void PreLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            Students = new List<JHStudentRecord>();
            foreach (JHClassRecord cla in JHClass.SelectByIDs(K12.Presentation.NLDPanels.Class.SelectedSource))
                Students.AddRange(cla.Students);
            Students = Students.GetInSchoolStudents();

            e.Result = JHExam.SelectAll();
        }

        private void CourseWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Reporter.Feedback("" + e.UserState, e.ProgressPercentage);
        }

        private void CourseWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MsgBox.Show(e.Error.Message);
                return;
            }
            else if (!errorProvider.HasError &&
                    ("" + RunningSchoolYear != cboSchoolYear.Text || "" + RunningSemester != cboSemester.Text))
            {
                RunningSchoolYear = int.Parse(cboSchoolYear.Text);
                RunningSemester = int.Parse(cboSemester.Text);

                CourseWorker.RunWorkerAsync();
            }
            else
            {
                List<string> subjects = Courses.GetSubjects();

                cboSource.Items.Clear();
                cboSource.Items.AddRange(subjects.ToArray());
                chTargetSubjects.Items.Clear();
                chTargetSubjects.Items.AddRange(subjects.ToArray());

                ControlEnabled = true;

                cboExam.Focus();
                dgvTargets.ClearSelection();

                FISCA.Presentation.MotherForm.SetStatusBarMessage("");
            }
        }

        private void CourseWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            CourseWorker.ReportProgress(0, "讀取學生資料...");
            Event.WaitOne();
            CourseWorker.ReportProgress(100, "讀取學生資料...");

            List<string> studentIDs = Students.AsKeyList();
            FunctionSpliter<string, JHSCAttendRecord> spliter = new FunctionSpliter<string, JHSCAttendRecord>(100, Util.MaxThread);
            spliter.Function = delegate(List<string> part)
            {
                return JHSCAttend.Select(part, null, null, "" + RunningSchoolYear, "" + RunningSemester);
            };
            spliter.ProgressChange = delegate(int progress)
            {
                CourseWorker.ReportProgress(Util.CalculatePercentage(studentIDs.Count, progress), "讀取修課記錄...");
            };
            List<JHSCAttendRecord> AllAttends = spliter.Execute(studentIDs);

            AttendManager.Instance.Set(Students, AllAttends);

            List<string> validCourseIDs = new List<string>();
            foreach (JHSCAttendRecord sca in AllAttends)
            {
                if (!validCourseIDs.Contains(sca.RefCourseID))
                    validCourseIDs.Add(sca.RefCourseID);
            }

            //CourseWorker.ReportProgress(0, "讀取課程...");

            JHCourse.RemoveAll();

            FunctionSpliter<string, JHCourseRecord> spliter2 = new FunctionSpliter<string, JHCourseRecord>(100, Util.MaxThread);
            spliter2.Function = delegate(List<string> part)
            {
                return JHCourse.SelectByIDs(part);
            };
            spliter2.ProgressChange = delegate(int progress)
            {
                CourseWorker.ReportProgress(Util.CalculatePercentage(validCourseIDs.Count, progress), "讀取課程...");
            };
            Courses = spliter2.Execute(validCourseIDs);
        }

        private void ValidateSemester()
        {
            int i;

            if (!int.TryParse(cboSchoolYear.Text, out i))
                errorProvider.SetError(cboSchoolYear, "學年度必須為數字");
            else
                errorProvider.SetError(cboSchoolYear, string.Empty);

            if (!int.TryParse(cboSemester.Text, out i))
                errorProvider.SetError(cboSemester, "學期必須為數字");
            else
                errorProvider.SetError(cboSemester, string.Empty);
        }

        private void ValidateUserDefined()
        {
            errorProvider.SetError(cboExam, (Exam == null) ? "試別不能空白" : "");
            errorProvider.SetError(cboSource, string.IsNullOrEmpty(SourceSubject) ? "來源科目不能空白" : "");
            errorProvider.SetError(dgvTargets, TargetSubjects.Count == 0 ? "目的科目不能空白" : "");
        }

        private void cboSchoolYearSemester_TextChanged(object sender, EventArgs e)
        {
            ValidateSemester();

            if (!errorProvider.HasError)
            {
                if (!CourseWorker.IsBusy)
                {
                    ControlEnabled = false;
                    RunningSchoolYear = int.Parse(cboSchoolYear.Text);
                    RunningSemester = int.Parse(cboSemester.Text);
                    CourseWorker.RunWorkerAsync();
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            ValidateUserDefined();
            if (errorProvider.HasError) return;

            Config = new UserConfig();
            Config.Exam = Exam;
            Config.SchoolYear = RunningSchoolYear;
            Config.Semester = RunningSemester;
            Config.SourceSubject = SourceSubject;
            Config.TargetSubjects.AddRange(TargetSubjects);
            AttendManager.Instance.Config = Config;

            if (Validator.ValidateCourses(Courses, Config) == false) return;
            if (Validator.ValidateStudentAttends(Config) == false) return;

            AttendManager.Instance.ReadScores(Reporter);
            FISCA.Presentation.MotherForm.SetStatusBarMessage("");

            if (Validator.ValidateStudentScores(Config))
            {
                this.Hide();
                ScoreSaver.RunWorkerAsync(); //儲存
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                AttendManager.Instance.ClearData();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("");
        }
    }
}
