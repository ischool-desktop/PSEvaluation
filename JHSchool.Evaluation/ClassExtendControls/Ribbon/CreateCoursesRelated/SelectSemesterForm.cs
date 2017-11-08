using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Framework;
using Framework.Legacy;

namespace JHSchool.Evaluation.ClassExtendControls.Ribbon.CreateCoursesRelated
{
    public partial class SelectSemesterForm : FISCA.Presentation.Controls.BaseForm
    {
        public SelectSemesterForm()
        {
            InitializeComponent();

            Text = "選擇開課學年度學期";

            try
            {
                for (int i = -2; i <= 2; i++)
                {
                    cboSchoolYear.Items.Add(GlobalOld.SystemConfig.DefaultSchoolYear + i);
                }
                cboSchoolYear.Text = GlobalOld.SystemConfig.DefaultSchoolYear.ToString();
                cboSemester.Text = GlobalOld.SystemConfig.DefaultSemester.ToString();

                for (int i = -2; i < 2; i++)
                {
                    cboPreviousSchoolYear.Items.Add(GlobalOld.SystemConfig.DefaultSchoolYear + i);
                }
                cboPreviousSchoolYear.Text = (GlobalOld.SystemConfig.DefaultSchoolYear - 1).ToString();
                cboPreviousSemester.Text = GlobalOld.SystemConfig.DefaultSemester.ToString();

            }
            catch (Exception ex)
            {
                //CurrentUser.ReportError(ex);
            }

            chkOption.Checked = false;
        }

        private EnhancedErrorProvider ErrorProvider
        {
            get { return errorProvider1; }
        }

        public int SchoolYear
        {
            get
            {
                int a;
                if (int.TryParse(cboSchoolYear.Text, out a))
                    return a;
                return GlobalOld.SystemConfig.DefaultSchoolYear;
            }
        }

        public int Semester
        {
            get
            {
                int a;
                if (int.TryParse(cboSemester.Text, out a))
                    return a;
                return GlobalOld.SystemConfig.DefaultSemester;
            }
        }

        public int CopySchoolYear
        {
            get
            {
                int a;
                if (int.TryParse(cboPreviousSchoolYear.Text, out a))
                    return a;
                return GlobalOld.SystemConfig.DefaultSchoolYear;
            }
        }

        public int CopySemester
        {
            get
            {
                int a;
                if (int.TryParse(cboPreviousSemester.Text, out a))
                    return a;
                return GlobalOld.SystemConfig.DefaultSemester;
            }
        }

        public bool CopyTeacher
        {
            get { return chkCopyTeacher.Checked; }
        }

        public bool CopyAssessmentSetup
        {
            get { return chkCopyAssessmentSetup.Checked; }
        }

        public bool CopyOptionEnabled
        {
            get { return chkOption.Checked; }
        }

        private new bool Validate()
        {
            ErrorProvider.Clear();
            int i;
            if (!int.TryParse(cboSchoolYear.Text, out i))
                ErrorProvider.SetError(cboSchoolYear, "學年度必須為數字");
            if (int.TryParse(cboSchoolYear.Text, out i) && i <= 0)
                ErrorProvider.SetError(cboSchoolYear, "學年度必須為正整數");
            if (!int.TryParse(cboSemester.Text, out i))
                ErrorProvider.SetError(cboSemester, "學期必須為 1 或 2");

            if (chkOption.Checked)
            {
                if (chkCopyTeacher.Checked || chkCopyAssessmentSetup.Checked)
                {
                    if (!int.TryParse(cboPreviousSchoolYear.Text, out i))
                        ErrorProvider.SetError(cboPreviousSchoolYear, "學年度必須為數字");
                    if (int.TryParse(cboPreviousSchoolYear.Text, out i) && i <= 0)
                        ErrorProvider.SetError(cboPreviousSchoolYear, "學年度必須為正整數");
                    if (!int.TryParse(cboPreviousSemester.Text, out i))
                        ErrorProvider.SetError(cboPreviousSemester, "學期必須為 1 或 2");
                }
            }

            return !ErrorProvider.HasError;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!Validate())
                return;

            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chkOP_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOption.Checked == true)
            {
                gpOptions.Visible = true;
                this.Size = new Size(313, 241);
                this.MaximumSize = new Size(313, 241);
                this.MinimumSize = new Size(313, 241);
            }
            else
            {
                chkCopyAssessmentSetup.Checked = false;
                chkCopyTeacher.Checked = false;
                gpOptions.Visible = false;
                this.Size = new Size(313, 241);
                this.MaximumSize = new Size(313, 116);
                this.MinimumSize = new Size(313, 116);
            }
        }

        private void chkCopyAssessmentSetup_CheckedChanged(object sender, EventArgs e)
        {
            cboPreviousSchoolYear.Enabled = cboPreviousSemester.Enabled = (chkCopyAssessmentSetup.Checked || chkCopyTeacher.Checked);
        }

        private void chkCopyTeacher_CheckedChanged(object sender, EventArgs e)
        {
            cboPreviousSchoolYear.Enabled = cboPreviousSemester.Enabled = (chkCopyAssessmentSetup.Checked || chkCopyTeacher.Checked);
        }
    }
}