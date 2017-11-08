using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Framework;
using JHSchool.Evaluation.Calculation;
using FISCA.Presentation;
using JHSchool.Data;

namespace JHSchool.Evaluation.ClassExtendControls.Ribbon.CreateCoursesRelated
{
    public partial class CreateClassCourseForm : FISCA.Presentation.Controls.BaseForm
    {
        private EnhancedErrorProvider _error;
        private BackgroundWorker _worker;
        private List<JHClassRecord> _classes;

        public CreateClassCourseForm(List<JHClassRecord> classes)
        {
            InitializeComponent();

            _classes = classes;

            InitializeErrorProvider();
            InitializeSemester();
            InitializeDomainAndSubject();
            InitializeWorker();
        }

        /// <summary>
        /// 初始化 BackgroundWorker
        /// </summary>
        private void InitializeWorker()
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);

            _worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {
                MotherForm.SetStatusBarMessage("" + e.UserState, e.ProgressPercentage);
            };

            _worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Result is List<string>)
                {
                    ErrorViewer viewer = new ErrorViewer();
                    viewer.SetHeader("班級");
                    foreach (string msg in e.Result as List<string>)
                    {
                        string className = msg.Split(new string[] { "：" }, StringSplitOptions.None)[0];
                        string errMsg = msg.Split(new string[] { "：" }, StringSplitOptions.None)[1];
                        viewer.SetMessage(className, new List<string>(new string[] { errMsg }));
                    }
                    viewer.Show();
                }
                else
                {
                    MotherForm.SetStatusBarMessage("班級開課完成");
                }
            };
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            #region DoWork
            object[] objs = (object[])e.Argument;
            int schoolyear = Framework.Int.Parse(objs[0] as string);
            int semester = Framework.Int.Parse(objs[1] as string);
            string domain = objs[2] as string;
            string subject = objs[3] as string;
            string periodcredit = objs[4] as string;

            PeriodCredit pc = new PeriodCredit();
            pc.Parse(periodcredit);

            double total = _classes.Count;
            double counter = 0;
            if (total == 0)
                total = 1;
            else
                total *= 2;
            _worker.ReportProgress(1, "正在檢查班級課程…");

            _classes.Sort(SortClassesByClassName);

            #region 檢查重複開課
            List<string> errors = new List<string>();

            List<string> classIDs = new List<string>();
            foreach (JHClassRecord cla in _classes)
                classIDs.Add(cla.ID);

            Dictionary<string, List<Data.JHCourseRecord>> classExistCourses = new Dictionary<string, List<JHSchool.Data.JHCourseRecord>>();
            List<Data.JHCourseRecord> orphanCourse = new List<JHSchool.Data.JHCourseRecord>();

            foreach (Data.JHCourseRecord course in Data.JHCourse.SelectBySchoolYearAndSemester(schoolyear, semester))
            {
                if (!classIDs.Contains(course.RefClassID))
                {
                    orphanCourse.Add(course);
                    continue;
                }

                if (!classExistCourses.ContainsKey(course.RefClassID))
                    classExistCourses.Add(course.RefClassID, new List<JHSchool.Data.JHCourseRecord>());
                classExistCourses[course.RefClassID].Add(course);
            }

            foreach (JHClassRecord cla in _classes)
            {
                if (!classExistCourses.ContainsKey(cla.ID)) continue;

                foreach (Data.JHCourseRecord course in classExistCourses[cla.ID])
                {
                    if (course.Subject == subject)
                        errors.Add(cla.Name + "：已有相同科目(" + subject + ")的課程。");
                }

                foreach (Data.JHCourseRecord course in orphanCourse)
                {
                    if (course.Name == cla.Name + " " + subject)
                        errors.Add(cla.Name + "：已有相同課程名稱(" + course.Name + ")的課程。");
                }
            }

            if (errors.Count > 0)
            {
                e.Result = errors;
                return;
            }
            #endregion

            #region 開課
            Dictionary<string, string> classNewCourse = new Dictionary<string, string>();

            foreach (JHClassRecord cla in _classes)
            {
                Data.JHCourseRecord newCourse = new JHSchool.Data.JHCourseRecord();
                newCourse.CalculationFlag = "1";
                newCourse.Period = pc.Period;
                newCourse.Credit = pc.Credit;
                newCourse.Domain = domain;
                newCourse.Subject = subject;
                newCourse.Name = cla.Name + " " + subject;
                newCourse.SchoolYear = schoolyear;
                newCourse.Semester = semester;
                newCourse.RefClassID = cla.ID;

                classNewCourse.Add(cla.ID, Data.JHCourse.Insert(newCourse));
                counter++;
                _worker.ReportProgress((int)(counter * 100d / total), "正在進行開課…");
            }
            #endregion

            #region 加入學生修課

            foreach (JHClassRecord cla in _classes)
            {
                List<Data.JHSCAttendRecord> scattends = new List<JHSchool.Data.JHSCAttendRecord>();
                foreach (Data.JHStudentRecord stu in cla.Students)
                {
                    Data.JHSCAttendRecord scattend = new JHSchool.Data.JHSCAttendRecord();
                    scattend.RefCourseID = classNewCourse[cla.ID];
                    scattend.RefStudentID = stu.ID;

                    scattends.Add(scattend);
                }

                if (scattends.Count > 0)
                    Data.JHSCAttend.Insert(scattends);

                counter++;
                _worker.ReportProgress((int)(counter * 100d / total), "正在加入學生修課…");
            }
            #endregion

            e.Result = string.Empty;

            #endregion
        }

        private int SortClassesByClassName(JHClassRecord x, JHClassRecord y)
        {
            string xx = x.DisplayOrder.PadLeft(3, '0') + ":" + x.Name;
            string yy = y.DisplayOrder.PadLeft(3, '0') + ":" + y.Name;
            return xx.CompareTo(yy);
        }

        /// <summary>
        /// 初始化 ErrorProvider
        /// </summary>
        private void InitializeErrorProvider()
        {
            _error = new EnhancedErrorProvider();
            _error.BlinkRate = 0;
        }

        /// <summary>
        /// 初始化學年度學期
        /// </summary>
        private void InitializeSemester()
        {
            try
            {
                int schoolyear = Framework.Int.Parse(School.DefaultSchoolYear);
                int semester = Framework.Int.Parse(School.DefaultSemester);

                cboSchoolYear.Items.Clear();
                cboSemester.Items.Clear();

                for (int i = -2; i <= 2; i++)
                {
                    cboSchoolYear.Items.Add(schoolyear + i);
                }

                cboSemester.Items.Add(1);
                cboSemester.Items.Add(2);

                cboSchoolYear.SelectedItem = schoolyear;
                cboSemester.SelectedItem = semester;
            }
            catch (Exception ex)
            {
                cboSchoolYear.Items.Clear();
                cboSemester.Items.Clear();
            }
        }

        /// <summary>
        /// 初始化領域
        /// </summary>
        private void InitializeDomainAndSubject()
        {
            try
            {
                cboDomain.Items.Clear();

                foreach (string domain in Subject.Domains)
                    cboDomain.Items.Add(domain);

                cboDomain.SelectedIndex = 0;

                cboSubject.Items.Clear();

                foreach (string subject in Subject.Subjects)
                    cboSubject.Items.Add(subject);
            }
            catch (Exception ex)
            {
                cboDomain.Items.Clear();
                cboSubject.Items.Clear();
            }
        }

        private void SchoolYearAndSemester_TextChanged(object sender, EventArgs e)
        {
            ComboBox cbo = sender as ComboBox;
            int i;
            if (!int.TryParse(cbo.Text, out i))
                _error.SetError(cbo, "必須為整數");
            else
                _error.SetError(cbo, "");
        }

        private void txtPeriodCredit_TextChanged(object sender, EventArgs e)
        {
            PeriodCredit pc = new PeriodCredit();
            if (!pc.Parse(txtPeriodCredit.Text))
                _error.SetError(txtPeriodCredit, pc.Error);
            else
                _error.SetError(txtPeriodCredit, "");
        }

        private void cboSubject_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboSubject.Text))
                _error.SetError(cboSubject, "不可為空白");
            else
                _error.SetError(cboSubject, "");
        }

        private bool IsValid()
        {
            bool valid = true;

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is TextBox)
                {
                    if (string.IsNullOrEmpty((ctrl as TextBox).Text))
                        _error.SetError(ctrl, "不可為空白");
                    else
                        _error.SetError(ctrl, "");
                }
                else if (ctrl is ComboBox)
                {
                    if (ctrl.Name == "cboDomain") continue;

                    if (string.IsNullOrEmpty((ctrl as ComboBox).Text))
                        _error.SetError(ctrl, "不可為空白");
                    else
                        _error.SetError(ctrl, "");
                }
            }

            valid = !_error.HasError;

            return valid;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!IsValid()) return;

            if (!_worker.IsBusy)
            {
                _worker.RunWorkerAsync(new object[] {
                    cboSchoolYear.Text,
                    cboSemester.Text,
                    cboDomain.Text,
                    cboSubject.Text.Trim(),
                    txtPeriodCredit.Text.Trim() });
            }
            this.DialogResult = DialogResult.OK;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
