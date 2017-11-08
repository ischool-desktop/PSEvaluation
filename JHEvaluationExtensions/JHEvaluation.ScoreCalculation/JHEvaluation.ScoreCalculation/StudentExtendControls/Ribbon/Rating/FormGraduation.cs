using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Framework;
using JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking;
using JHSchool.Evaluation.Calculation;
using JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking.Basic;
using JHSchool.Data;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating
{
    public partial class FormGraduation: FormRating
    {
        public FormGraduation()
        {
            InitializeComponent();
        }

        private void FormGraduation_Load(object sender, EventArgs e)
        {
            RatingUtils.DisableControls(this);
            RatingUtils.EnableControls(this);
        }
    }
}
