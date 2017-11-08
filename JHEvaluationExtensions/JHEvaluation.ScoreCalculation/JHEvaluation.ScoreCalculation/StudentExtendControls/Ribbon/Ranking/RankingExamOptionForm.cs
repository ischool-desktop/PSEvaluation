using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Framework;
using System.Xml;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking
{
    public partial class RankingExamOptionForm : FISCA.Presentation.Controls.BaseForm
    {
        public bool Sequence { get { return rbSequence.Checked; } }
        public bool UseZero { get { return rbUseZero.Checked; } }
        public int Carry { get { return integerInput1.Value; } }

        public RankingExamOptionForm()
        {
            InitializeComponent();

            //School.Configuration.Sync("排名設定");
            ConfigData cd = School.Configuration["排名設定"];

            if (cd.GetBoolean("Sequence", false))
                rbSequence.Checked = true;
            else
                rbNoSequence.Checked = true;

            integerInput1.Value = cd.GetInteger("Carry", 2);

            if (cd.GetBoolean("UseZero", false))
                rbUseZero.Checked = true;
            else
                rbNoRanking.Checked = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ConfigData cd = School.Configuration["排名設定"];
            cd.SetBoolean("Sequence", rbSequence.Checked);
            cd.SetInteger("Carry", integerInput1.Value);
            cd.SetBoolean("UseZero", rbUseZero.Checked);
            cd.Save();

            this.DialogResult = DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
