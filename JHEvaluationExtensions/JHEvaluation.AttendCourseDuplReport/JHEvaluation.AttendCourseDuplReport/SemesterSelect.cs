using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace JHEvaluation.AttendCourseDuplReport
{
    public partial class SemesterSelect : BaseForm
    {
        /// <summary>
        /// 取得選擇的學年度
        /// </summary>
        public int SchoolYear
        {
            get
            {
                if (cboSchoolYear.SelectedItem != null)
                    return int.Parse("" + cboSchoolYear.SelectedItem);
                else
                    return 0;
            }
        }

        /// <summary>
        /// 取得選擇的學期
        /// </summary>
        public int Semester
        {
            get
            {
                if (cboSemester.SelectedItem != null)
                    return int.Parse("" + cboSemester.SelectedItem);
                else
                    return 0;
            }
        }

        public SemesterSelect()
        {
            InitializeComponent();
            InitializeSemester();
        }

        private void InitializeSemester()
        {
            try
            {
                int schoolYear = int.Parse(K12.Data.School.DefaultSchoolYear);
                int semester = int.Parse(K12.Data.School.DefaultSemester);

                cboSchoolYear.Items.Clear();
                for (int i = -2; i < 2; i++)
                    cboSchoolYear.Items.Add(schoolYear + i);
                cboSemester.Items.Add(1);
                cboSemester.Items.Add(2);

                cboSchoolYear.SelectedIndex = 2;
                cboSemester.SelectedIndex = semester - 1;
            }
            catch (Exception)
            {
                MsgBox.Show("建構學年度學期失敗");
            }
        }

        private void comboBox_TextChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            string text = cb.Text;
            int i;
            if (!int.TryParse(text, out i))
                errorProvider.SetError(cb, "必須是整數");
            else
                errorProvider.SetError(cb, string.Empty);

            btnOK.Enabled = !errorProvider.HasError;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
