using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data.Configuration;

namespace JHEvaluation.WuZhuanRegisterScoreImport
{
    public partial class SubjectCEForm : BaseForm
    {
        private string ConfigName { get; set; }

        public string ChineseSubject { get; private set; }
        public string EnglishSubject { get; private set; }

        private string DefaultCH { get; set; }
        private string DefaultEN { get; set; }

        private DataGridViewCell CellCH { get; set; }
        private DataGridViewCell CellEN { get; set; }

        public SubjectCEForm(string configName, string defaultCH, string defaultEN)
        {
            InitializeComponent();

            DefaultCH = defaultCH;
            DefaultEN = defaultEN;
            InitializeDataGridView();

            ConfigName = configName;
            ConfigData cd = K12.Data.School.Configuration[ConfigName];

            if (cd.Contains("Chinese") && !string.IsNullOrEmpty(cd["Chinese"]))
                CellCH.Value = cd["Chinese"];
            else CellCH.Value = DefaultCH;
            if (cd.Contains("English") && !string.IsNullOrEmpty(cd["English"]))
                CellEN.Value = cd["English"];
            else CellEN.Value = DefaultEN;
        }

        private void InitializeDataGridView()
        {
            int cIndex = dgv.Rows.Add();
            int eIndex = dgv.Rows.Add();
            dgv.Rows[cIndex].Cells[chOnReport.Index].Value = DefaultCH;
            dgv.Rows[eIndex].Cells[chOnReport.Index].Value = DefaultEN;
            CellCH = dgv.Rows[cIndex].Cells[chOnIschool.Index];
            CellEN = dgv.Rows[eIndex].Cells[chOnIschool.Index];
            chOnIschool.Items.AddRange(JHSchool.Evaluation.Subject.Subjects);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            dgv.EndEdit();

            ConfigData cd = K12.Data.School.Configuration[ConfigName];
            cd["Chinese"] = ChineseSubject = "" + CellCH.Value;
            cd["English"] = EnglishSubject = "" + CellEN.Value;
            cd.Save();

            this.DialogResult = DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
