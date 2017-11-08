using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using JHSchool.Data;

namespace JHEvaluation.Rating
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

        private void btnRank_Click(object sender, EventArgs e)
        {

        }
    }
}
