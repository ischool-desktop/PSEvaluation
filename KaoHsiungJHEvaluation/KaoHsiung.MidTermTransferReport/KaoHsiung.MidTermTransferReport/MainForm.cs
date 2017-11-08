using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using Campus.Report;
using Framework;
using K12.Data;

namespace KaoHsiung.MidTermTransferReport
{
    /// <summary>
    /// 報表主畫面，可以設定：列印設定、假別設定。
    /// </summary>
    public partial class MainForm : BaseForm
    {
        private Config _config;
        private EnhancedErrorProvider _error;

        internal static void Run()
        {
            new MainForm().ShowDialog();
        }

        public MainForm()
        {
            InitializeComponent();

            _config = new Config();
            _error = new EnhancedErrorProvider();
            _error.BlinkRate = 0;

            InitializeSemester();
            InitializeDate();

            this.Text = Global.ReportName;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;

        }

        private void InitializeDate()
        {
            ReportConfiguration rc = new ReportConfiguration(Global.ReportName);
            txtSDate.Text = rc.GetString("缺曠獎懲統計開始日期", DateTime.Today.ToString("yyyy/MM/dd"));
            txtEDate.Text = rc.GetString("缺曠獎懲統計結束日期", DateTime.Today.ToString("yyyy/MM/dd"));
        }

        private void InitializeSemester()
        {
            try
            {
                int schoolYear = int.Parse(School.DefaultSchoolYear);
                int semester = int.Parse(School.DefaultSemester);

                iptSchoolYear.Value = schoolYear;
                iptSemester.Value = semester;

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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (!IsValid())
            {
                return;
            }

            ReportConfiguration rc = new ReportConfiguration(Global.ReportName);
            rc.SetString("缺曠獎懲統計開始日期", txtSDate.Text);
            rc.SetString("缺曠獎懲統計結束日期", txtEDate.Text);
            rc.Save();
            _config.StartDate = DateTime.Parse(txtSDate.Text);
            _config.EndDate = DateTime.Parse(txtEDate.Text);
            


            List<JHStudentRecord> students = JHStudent.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource);
            //排序學生
            students.Sort(SortStudentByClassSeatNo);
            _config.Students =students;

            _config.SchoolYear = iptSchoolYear.Value;
            _config.Semester = iptSemester.Value;
            _config.Load();
            Report report = new Report(_config);
            report.Generate();

            this.DialogResult = DialogResult.OK;
        }

        private int SortStudentByClassSeatNo(JHStudentRecord x, JHStudentRecord y)
        {
            JHClassRecord c1 = x.Class;
            JHClassRecord c2 = y.Class;
            if (x.RefClassID == y.RefClassID)
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
                if (c1 == null)
                    return -1;
                else if (c2 == null)
                    return 1;
                return c1.Name.CompareTo(c2.Name);
            }
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
                if (e.Value < s.Value)
                {
                    _error.SetIconPadding(txtEDate, -10);
                    _error.SetError(txtEDate, "統計期間錯誤");
                }
            }

            return !_error.HasError;
        }

        private void lnPrint_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PrintConfigForm form = new PrintConfigForm(Global.ReportName);
            form.ShowDialog();
        }

        private void txtSDate_TextChanged(object sender, EventArgs e)
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

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
