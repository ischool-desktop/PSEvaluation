using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.Editor;
using Framework.Legacy;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon
{
    public partial class AddCourse : FISCA.Presentation.Controls.BaseForm
    {
        public AddCourse()
        {
            InitializeComponent();
            cboSchoolYear.Text = GlobalOld.SystemConfig.DefaultSchoolYear.ToString();
            cboSemester.Items.Add("1");
            cboSemester.Items.Add("2");
            cboSemester.Text = GlobalOld.SystemConfig.DefaultSemester.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
                return;
            bool chkHasCourseName = false;
            int SchoolYear, Semester;
                int.TryParse(cboSchoolYear.Text, out SchoolYear);
                int.TryParse(cboSemester.Text, out Semester);


            foreach (CourseRecord cr in Course.Instance.Items)
            {
                if (cr.SchoolYear == SchoolYear && cr.Semester == Semester && cr.Name == txtName.Text)
                {
                    chkHasCourseName = true;
                    MessageBox.Show("課程名稱重複");
                    break;
                }
            }
            if (chkHasCourseName == false)
            {
                CourseRecordEditor cre = Course.Instance.AddCourse();
                cre.SchoolYear = SchoolYear;
                cre.Semester = Semester;
                cre.Name = txtName.Text;
                cre.Save();
                Course.Instance.SyncDataBackground(cre.ID);
                if (chkInputData.Checked == true)
                {
                    foreach (CourseRecord cr in Course.Instance.Items)
                    {
                        if (cr.SchoolYear == SchoolYear && cr.Semester == Semester && cr.Name == txtName.Text)
                        {
                            Course.Instance.PopupDetailPane(cr.ID);
                            Course.Instance.SyncDataBackground(cr.ID);
                        }
                    }
                }                
                this.Close();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close ();
        }
    }
}
