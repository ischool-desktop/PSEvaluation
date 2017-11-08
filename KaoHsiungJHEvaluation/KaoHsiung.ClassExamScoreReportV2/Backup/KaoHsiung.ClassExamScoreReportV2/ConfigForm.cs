using System;
using FISCA.Presentation.Controls;
using Campus.Report;
using System.Windows.Forms;

namespace KaoHsiung.ClassExamScoreReportV2
{
    internal partial class ConfigForm : BaseForm
    {
        ReportPreference config = null;

        public ConfigForm()
        {
            InitializeComponent();

            config = new ReportPreference();

            DisplayConfiguration();
        }

        /// <summary>
        /// 預設值
        /// </summary>
        private void DisplayConfiguration()
        {
            foreach (Control eachCtl in gpItem.Controls)
            {
                if (!(eachCtl is CheckBox)) continue;

                if (config.PrintItems.Contains(eachCtl.Text))
                    (eachCtl as CheckBox).Checked = true;
            }

            foreach (Control eachCtl in gpRank.Controls)
            {
                if (!(eachCtl is RadioButton)) continue;

                if (eachCtl.Text == config.RankMethod)
                    (eachCtl as RadioButton).Checked = true;
            }

            if (config.PaperSize == "A4")
                rbA4.Checked = true;
            else if (config.PaperSize == "B4")
                rbB4.Checked = true;

            RefreshCheckState();
        }

        #region Events
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            RefreshCheckState();
        }

        private void RefreshCheckState()
        {
            rbTotal.Enabled = chkTotal.Checked;
            rbWeightTotal.Enabled = chkWeightTotal.Checked;
            rbAvg.Enabled = chkAvg.Checked;
            rbWeightAvg.Enabled = chkWeightAvg.Checked;

            foreach (Control eachCtl in gpRank.Controls)
            {
                if (!(eachCtl is RadioButton)) continue;

                if (!eachCtl.Enabled)
                    (eachCtl as RadioButton).Checked = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            config.PrintItems.Clear();
            foreach (Control eachCtl in gpItem.Controls)
            {
                if (!(eachCtl is CheckBox)) continue;

                if ((eachCtl as CheckBox).Checked)
                    config.PrintItems.Add(eachCtl.Text);
            }

            config.RankMethod = string.Empty;
            foreach (Control eachCtl in gpRank.Controls)
            {
                if (!(eachCtl is RadioButton)) continue;

                if ((eachCtl as RadioButton).Checked)
                    config.RankMethod = eachCtl.Text;
            }

            if (rbA4.Checked)
                config.PaperSize = "A4";
            else
                config.PaperSize = "B4";

            config.Save();
            DialogResult = DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
