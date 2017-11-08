using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JointAdmissionModule
{
    public partial class StudSemScoreRankPInput : FISCA.Presentation.Controls.BaseForm
    {        

        private string _StudentID;

        public StudSemScoreRankPInput(string StudentID)
        {
            InitializeComponent();
            this.MaximumSize = this.MinimumSize = this.Size;            
            _StudentID = StudentID;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ucSocreRankItem1.SetStudentID(_StudentID);
            ucSocreRankItem1.Save();
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void StudSemScoreRankPInput_Load(object sender, EventArgs e)
        {
            // 資料放置畫面
            ucSocreRankItem1.SetStudentID(_StudentID);
            ucSocreRankItem1.LoadData();
        }
    }
}
