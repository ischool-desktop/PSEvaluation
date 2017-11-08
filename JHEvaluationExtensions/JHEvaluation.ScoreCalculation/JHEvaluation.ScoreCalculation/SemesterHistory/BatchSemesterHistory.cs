using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using K12.Data;

namespace JHEvaluation.ScoreCalculation.SemesterHistory
{
    public partial class BatchSemesterHistory : FISCA.Presentation.Controls.BaseForm 
    {
        private BackgroundWorker _BKWork;
        //private bool isBGWorkBusy;
        private List<DAL.SemesterHistoryItemEntity> SemesterHistoryItemEntityList;
        int SchoolYear, Semester;
        List<string> _StudentIDList;
        List<int> _GradeYear;
        List<JHSchool.Data.JHStudentRecord> _GetStudRecList;

        public BatchSemesterHistory()
        {
            InitializeComponent();

            SemesterHistoryItemEntityList = new List<DAL.SemesterHistoryItemEntity>();
            _GradeYear = new List<int>();
            _GetStudRecList = new List<JHSchool.Data.JHStudentRecord>();
            _StudentIDList = new List<string>();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            // 檢查使用者是否有選年級
            if (lstGradeYear.CheckedItems.Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("請選擇年級.");
                return;
            }

            int.TryParse(School.DefaultSchoolYear, out SchoolYear);
            int.TryParse(School.DefaultSemester, out Semester);

            _StudentIDList.Clear();

            //取得所選年級與轉換
            List<int> selectGradeYear = new List<int>();
            foreach (ListViewItem lvi in lstGradeYear.CheckedItems)
            {
                int gr;
                if (int.TryParse(lvi.Text, out gr))
                    selectGradeYear.Add(gr);            
            }

            // 加入所選年級學生 StudentID
            foreach (JHSchool.Data.JHStudentRecord studRec in _GetStudRecList)
            {
                if (selectGradeYear.Contains(studRec.Class.GradeYear.Value))
                    _StudentIDList.Add(studRec.ID);
            
            }

            //StudentIDList = K12.Presentation.NLDPanels.Student.SelectedSource;

            btnRun.Enabled = false;
            _BKWork = new BackgroundWorker();
            _BKWork.DoWork += new DoWorkEventHandler(_BKWork_DoWork);
            _BKWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BKWork_RunWorkerCompleted);
            _BKWork.RunWorkerAsync();
       

        }

        void _BKWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_BKWork.IsBusy)
            {
                _BKWork.RunWorkerAsync();
                return;
            }
            btnRun.Enabled = true;
            FISCA.Presentation.Controls.MsgBox.Show("產生完成.");
            this.Close();
            JHSchool.Student.Instance.SyncAllBackground();
        }

        void _BKWork_DoWork(object sender, DoWorkEventArgs e)
        {
            // 取得畫面所選 
            if (SchoolYear > 0 && Semester > 0)
            {
                SemesterHistoryItemEntityList = DAL.DALTransfer.GetSemesterHistoryItemEntityList(SchoolYear, Semester, _StudentIDList);
                DAL.DALTransfer.SetSemesterHistoryItemEntityList(SemesterHistoryItemEntityList, _StudentIDList);

                // Log                
                StringBuilder sb = new StringBuilder();
                sb.Append("產生" + SchoolYear + "學年度第" + Semester + "學期 學期歷程，學生資訊：");

                foreach (DAL.SemesterHistoryItemEntity shi in SemesterHistoryItemEntityList)
                    sb.Append("學號:" + shi.StudentNumber + ",姓名:" + shi.Name + ";");

                sb.Append("共" + SemesterHistoryItemEntityList.Count + "筆");

                PermRecLogProcess prlp = new PermRecLogProcess();
                prlp.SaveLog("學籍系統", "批次產生學生學期歷程", sb.ToString());
            }    
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BatchSemesterHistory_Load(object sender, EventArgs e)
        {
            // 讀取一般與輟學生
            _GetStudRecList = DAL.DALTransfer.GetJHStudentRecordListBy1();
            labelX1.Text = School.DefaultSchoolYear + "學年度 第" + School.DefaultSemester + "學期";
            foreach (JHSchool.Data.JHStudentRecord stud in _GetStudRecList)
                if (!_GradeYear.Contains(stud.Class.GradeYear.Value))
                    _GradeYear.Add(stud.Class.GradeYear.Value);
            _GradeYear.Sort();
            
            foreach (int g in _GradeYear)
                lstGradeYear.Items.Add(g + "");
        }
    }
}
