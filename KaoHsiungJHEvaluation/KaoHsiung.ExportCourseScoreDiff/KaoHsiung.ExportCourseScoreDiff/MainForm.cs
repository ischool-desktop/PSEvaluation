using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace KaoHsiung.ExportCourseScoreDiff
{
    public partial class MainForm :BaseForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.MinimumSize = this.Size;

            // 放入初始值
            int sy, ss;
            if (int.TryParse(K12.Data.School.DefaultSchoolYear, out sy))
                iptSchoolYear.Value = sy;

            if (int.TryParse(K12.Data.School.DefaultSemester, out ss))
                iptSemester.Value = ss;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            this.btnPrint.Enabled = false;

            try
            {
                ExportScore es = new ExportScore(iptSchoolYear.Value, iptSemester.Value);
                es.Export();
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
            this.btnPrint.Enabled = true;
        }
    }
}
