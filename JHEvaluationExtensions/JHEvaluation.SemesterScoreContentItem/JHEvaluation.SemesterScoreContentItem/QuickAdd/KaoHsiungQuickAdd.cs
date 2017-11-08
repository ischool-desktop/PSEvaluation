using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace JHEvaluation.SemesterScoreContentItem.QuickAdd
{
    public partial class KaoHsiungQuickAdd : BaseForm
    {
        public KaoHsiungQuickAdd()
        {
            InitializeComponent();
            this.Text = "學期成績快速輸入";


            Test();
        }

        private void Test()
        {
            foreach (string domain in JHSchool.Evaluation.Subject.Domains)
            {
                QuickAddRow row = new QuickAddRow();
                row.CreateCells(dgvDomain, domain, "", "", "", "");
                dgvDomain.Rows.Add(row);
            }
        }
    }
}
