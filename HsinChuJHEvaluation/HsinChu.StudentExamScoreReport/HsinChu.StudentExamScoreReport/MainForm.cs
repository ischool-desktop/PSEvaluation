using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using Framework;
using HsinChu.StudentExamScoreReport.ConfigForm;
using K12.Presentation;
using K12.Data;
using Campus.Report;
using System.Linq;

namespace HsinChu.StudentExamScoreReport
{
    public partial class MainForm : BaseForm
    {
        private BackgroundWorker _worker;

        private Config _config;
        private List<JHExamRecord> _exams;
        private EnhancedErrorProvider _error;
        private EnterType _enterType;

        internal static void Run(EnterType enterType)
        {
            new MainForm(enterType).ShowDialog();
        }

        internal List<JHStudentRecord> GetStudents()
        {
            if (_enterType == EnterType.Student)
                return JHStudent.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource);
            else
            {
                List<JHStudentRecord> list = new List<JHStudentRecord>();
                // 取得班級學生(一般和輟學)
                foreach (JHClassRecord cla in JHClass.SelectByIDs(K12.Presentation.NLDPanels.Class.SelectedSource))
                {
                    foreach (JHStudentRecord stud in cla.Students)
                    {
                        if (stud.Status == StudentRecord.StudentStatus.一般 || stud.Status == StudentRecord.StudentStatus.輟學)
                            list.Add(stud);
                    }                 
                }
                return list;
            }
        }

        public MainForm(EnterType enterType)
        {
            InitializeComponent();
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.Text = Global.ReportName;
            _enterType = enterType;

            _config = new Config(Global.ReportName);
            _error = new EnhancedErrorProvider();
            _error.BlinkRate = 0;

            InitializeSemester();
            InitializeDate();

            _worker = new BackgroundWorker();
            _worker.DoWork += delegate
            {
                #region 取得試別
                _exams = JHExam.SelectAll();
                //List<string> examIDs = new List<string>();
                //foreach (JHExamRecord exam in _exams)
                //{
                //    examIDs.Add(exam.ID);
                //    _ecMapping.Add(exam.ID, new List<string>());
                //}
                #endregion
            };
            _worker.RunWorkerCompleted += delegate
            {
                //Campus.Configuration.Config.App.Sync("新竹個人評量成績單試別");
                //Campus.Configuration.ConfigData cd = Campus.Configuration.Config.App["新竹個人評量成績單試別"];
                //string str = cd["新竹個人評量成績單試別"];

                K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration["新竹個人評量成績單試別"];
                string str = cd["新竹個人評量成績單試別"];

                cbExam.DisplayMember = "Name";
                foreach (var exam in _exams)
                    cbExam.Items.Add(exam);
                int idx = cbExam.FindString(str);
                if (idx < 0)
                    cbExam.SelectedIndex = 0;
                else
                    cbExam.SelectedIndex = idx;

                cbExam.Enabled = true;
                btnNext.Enabled = true;
                LoadSubjectName();
            };
            _worker.RunWorkerAsync();
        }

        private void InitializeDate()
        {
            ReportConfiguration rc = new ReportConfiguration(Global.ReportName);
            txtSDate.Text = rc.GetString("缺曠獎懲統計開始日期", DateTime.Today.ToString("yyyy/MM/dd"));
            txtEDate.Text = rc.GetString("缺曠獎懲統計結束日期", DateTime.Today.ToString("yyyy/MM/dd"));
            txtCorrect.Text = rc.GetString("成績校正日期", DateTime.Today.ToString("yyyy/MM/dd"));
        }

        private void InitializeSemester()
        {
            try
            {
                int schoolYear = int.Parse(School.DefaultSchoolYear);
                int semester = int.Parse(School.DefaultSemester);
                for (int i = -3; i <= 2; i++)
                    cboSchoolYear.Items.Add(schoolYear + i);
                cboSemester.Items.Add(1);
                cboSemester.Items.Add(2);

                //cboSchoolYear.SelectedIndex = 2;
                //cboSemester.SelectedIndex = semester - 1;
                cboSchoolYear.Text = schoolYear.ToString();
                cboSemester.Text = semester.ToString();
            }
            catch (Exception ex)
            {

            }
        }

        private void lnType_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SelectTypeForm form = new SelectTypeForm(Global.ReportName);
            form.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (!IsValid())
            {
                return;
            }

            // 取得畫面賞用者所選的課程ID
            Global._selectCourseIDList.Clear();
            foreach (ListViewItem lv in lvSubject.CheckedItems)
            {
                foreach (KeyValuePair<string, string> str in Global._CourseIDSubjectDict)
                    if (str.Value == lv.Text)
                        Global._selectCourseIDList.Add(str.Key);
            }

            // 儲存試別
            //Campus.Configuration.ConfigData cd = Campus.Configuration.Config.App["新竹個人評量成績單試別"];
            //cd["新竹個人評量成績單試別"] = cbExam.Text;
            //cd.Save();
            K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration["新竹個人評量成績單試別"];
            cd["新竹個人評量成績單試別"] = cbExam.Text;
            cd.Save();

            ReportConfiguration rc = new ReportConfiguration(Global.ReportName);
            rc.SetString("缺曠獎懲統計開始日期", txtSDate.Text);
            rc.SetString("缺曠獎懲統計結束日期", txtEDate.Text);
            rc.SetString("成績校正日期", txtCorrect.Text);
            rc.Save();

            JHExamRecord exam = cbExam.SelectedItem as JHExamRecord;
            DateTime from = DateTime.Parse(txtSDate.Text);
            DateTime to = DateTime.Parse(txtEDate.Text);
            List<JHStudentRecord> students = GetStudents();
            students.Sort(delegate(JHStudentRecord x, JHStudentRecord y)
            {
                JHClassRecord c1 = x.Class;
                JHClassRecord c2 = y.Class;
                if (x.Class == null) return -1;
                if (y.Class == null) return 1;

                if (c1.ID == c2.ID)
                {
                    int seatNo1 = x.SeatNo.HasValue ? x.SeatNo.Value : int.MinValue;
                    int seatNo2 = y.SeatNo.HasValue ? y.SeatNo.Value : int.MinValue;

                    if (seatNo1 == seatNo2)
                        return x.StudentNumber.CompareTo(y.StudentNumber);
                    else
                        return seatNo1.CompareTo(seatNo2);
                }
                else
                {
                    return c1.Name.CompareTo(c2.Name);
                }
            });
            _config.SetExam(exam);
            _config.SetDateRange(from, to);
            _config.SetStudents(students);
            _config.SchoolYear = (int)cboSchoolYear.SelectedItem;
            _config.Semester = (int)cboSemester.SelectedItem;



            _config.Load();
            Report report = new Report(_config);
            report.Generate();

            this.DialogResult = DialogResult.OK;
        }

        private bool IsValid()
        {
            DateTime? s = null, e = null;
            if (string.IsNullOrEmpty(txtSDate.Text))
            {
                _error.SetIconPadding(txtSDate, -10);
                _error.SetError(txtSDate, "日期格式錯誤");
            }
            else
                s = DateTime.Parse(txtSDate.Text);

            if (string.IsNullOrEmpty(txtEDate.Text))
            {
                _error.SetIconPadding(txtEDate, -10);
                _error.SetError(txtEDate, "日期格式錯誤");
            }
            else
                e = DateTime.Parse(txtEDate.Text);

            if (s.HasValue && e.HasValue)
            {
                if(e.Value < s.Value)
                {
                    _error.SetIconPadding(txtEDate, -10);
                    _error.SetError(txtEDate, "統計期間錯誤");
                }
            }

            return !_error.HasError;
        }

        private void lnPC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PrintConfigForm form = new PrintConfigForm();
            form.ShowDialog();
            lvSubject.Enabled = CheckSelectItemEnable();
            LoadSubjectName();
        }

        /// <summary>
        /// 檢查科目選單是否開啟
        /// </summary>
        /// <returns></returns>
        private bool CheckSelectItemEnable()
        {
            bool retVal = false;
            ReportConfiguration _config = new ReportConfiguration(Global.ReportName);
            string print=_config.GetString("領域科目設定", string.Empty);
            if(print == "Subject")
                retVal=true;
            return retVal;
        }

        private void txtDate_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            string text = tb.Text;

            _error.SetError(tb, string.Empty);

            DateTime dt;
            if (!DateTime.TryParse(text, out dt))
            {
                _error.SetIconPadding(tb, -10);
                _error.SetError(tb, "日期格式錯誤");
            }
        }

        public void LoadSubjectName()
        {
            lvSubject.Items.Clear();

            List<string> studIDList = new List<string>();
            if (_enterType == EnterType.Student)
            {
                studIDList = K12.Presentation.NLDPanels.Student.SelectedSource;
            }
            else
            {
                studIDList = Utility.GetClassStudentIDList18ByClassIDList(K12.Presentation.NLDPanels.Class.SelectedSource);
            }

            Global._CourseIDSubjectDict = Utility.Get_SCAttend_CourseID_Subject(studIDList, cbExam.Text, cboSchoolYear.Text, cboSemester.Text);
            List<string> subjNameList = Global._CourseIDSubjectDict.Values.Distinct().ToList();

            foreach (string str in subjNameList)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = str;
                lvi.Checked = true;
                lvSubject.Items.Add(lvi);
            }

        }

        private void cboSchoolYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSubjectName();
        }

        private void cboSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSubjectName();
        }

        private void cbExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSubjectName();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lvSubject.Enabled = CheckSelectItemEnable();

        }
    }
    public enum EnterType
    {
        Student, Class
    }
}
