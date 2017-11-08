using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Campus.Windows;//Framework.Security.FeatureCodeAttribute;
using FISCA.Presentation;
using FCode = FISCA.Permission.FeatureCodeAttribute;

namespace JointAdmissionModule
{
    [FCode("JHSchool.Student.JointAdmissionModule", "學期成績排名與排名百分比")]
    public partial class UCSocreRankItem : DetailContent 
    {
        private string _StudentID;
        private List<DAL.UserDefData> _UserDefDataList;
        private bool _isBusy = false;
        private BackgroundWorker _BGWorker;
        private ChangeListener ChangeManager = new ChangeListener();

        public UCSocreRankItem()
        {
            InitializeComponent();
            Group = "學期成績排名與排名百分比";
            _BGWorker = new BackgroundWorker();
            _UserDefDataList = new List<JointAdmissionModule.DAL.UserDefData>();
            _BGWorker.DoWork += new DoWorkEventHandler(_BGWorker_DoWork);
            _BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWorker_RunWorkerCompleted);
            ChangeManager.Add(new DataGridViewSource(dgv));
            ChangeManager.StatusChanged += delegate(object sender, ChangeEventArgs e)
            {
                this.CancelButtonVisible = (e.Status == ValueStatus.Dirty);
                this.SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            };           
        }

        void _BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (_isBusy)
            {
                _isBusy = false;
                _BGWorker.RunWorkerAsync();
                return;
            }

            int RowIdx = 0;
            foreach (DAL.UserDefData udd in _UserDefDataList)
            {                
                dgv.Rows.Add();
                dgv.Rows[RowIdx].Tag = udd.UID;
                dgv.Rows[RowIdx].Cells[SchoolYear.Index].Value = udd.SchoolYear.ToString();
                dgv.Rows[RowIdx].Cells[GradeYear.Index].Value = udd.GradeYear.ToString();
                dgv.Rows[RowIdx].Cells[Semester.Index].Value = udd.Semester.ToString();
                dgv.Rows[RowIdx].Cells[GradeRank.Index].Value = udd.GradeRank.ToString();
                dgv.Rows[RowIdx].Cells[GradeRankAdd.Index].Value = udd.GradeRankAdd.ToString();
                dgv.Rows[RowIdx].Cells[GradeRankPercent.Index].Value = udd.GradeRankPercent.ToString();
                dgv.Rows[RowIdx].Cells[GradeRankPercentAdd.Index].Value = udd.GradeRankPercentAdd.ToString();
                RowIdx++;
            }
            ChangeManager.Reset();
            ChangeManager.ResumeListen();

            this.Loading = false;
        }

        void _BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            _UserDefDataList = DAL.UDTTransfer.GetDataFromUDT(_StudentID);
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            SetStudentID(PrimaryKey);
            LoadData();
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            if (FISCA.Permission.UserAcl.Current["JHSchool.Student.JointAdmissionModule"].Editable)
                Save();
            else
                MessageBox.Show("此帳號無編輯權限...", "ischool");

            this.SaveButtonVisible = false;
            this.CancelButtonVisible = false;

        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            LoadData();
            this.SaveButtonVisible = false;
            this.CancelButtonVisible = false;
        }

        /// <summary>
        /// 設定學生ID
        /// </summary>
        /// <param name="StudentID"></param>
        public void SetStudentID(string StudentID)
        {
            _StudentID = StudentID;
        }

        /// <summary>
        /// 載入UDT資料到DataGridView 畫面
        /// </summary>
        public void LoadData()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(LoadData));
            }
            else
            {
                if(_BGWorker.IsBusy)
                    _isBusy = true;
                else
                {
                    ChangeManager.SuspendListen();
                    dgv.Rows.Clear();
                    this.Loading = true;
                    this.CancelButtonVisible = false;
                    this.SaveButtonVisible = false;

                    _BGWorker.RunWorkerAsync();

                }
            }
        }        

        /// <summary>
        /// 儲存 DataGridView資料到UDT
        /// </summary>
        public void Save()
        {
            List<DAL.UserDefData> InsertData = new List<JointAdmissionModule.DAL.UserDefData>();

            // 檢查資料

            int ssy,sy, ss, sr,srAdd,srp,srpAdd;
            // 檢查年級學期是否重複用
            List<string> chkGrStr = new List<string>();
            bool hasError = false;
            // 檢查資料並加入組合
            foreach (DataGridViewRow dgr in dgv.Rows)
            {

                if (dgr.IsNewRow)
                    continue;

                DAL.UserDefData udd = new JointAdmissionModule.DAL.UserDefData();
                udd.RefID = _StudentID;

                // 學年度
                if (dgr.Cells[SchoolYear.Index].Value != null)
                    if (int.TryParse(dgr.Cells[SchoolYear.Index].Value.ToString(), out ssy) == false)
                    {
                        dgr.Cells[SchoolYear.Index].ErrorText = "請填整數!";
                    }
                    else
                    {
                        udd.SchoolYear = ssy;
                        dgr.Cells[SchoolYear.Index].ErrorText = "";
                    }


                // 年級
                if (dgr.Cells[GradeYear.Index].Value != null)
                    if (int.TryParse(dgr.Cells[GradeYear.Index].Value.ToString(), out sy) == false)
                    {
                        dgr.Cells[GradeYear.Index].ErrorText = "請填整數!";
                    }
                    else
                    {
                        udd.GradeYear = sy;
                        dgr.Cells[GradeYear.Index].ErrorText = "";
                    }


                // 學期
                if (dgr.Cells[Semester.Index].Value != null)
                    if (int.TryParse(dgr.Cells[Semester.Index].Value.ToString(), out ss) == false)
                    {
                        dgr.Cells[Semester.Index].ErrorText = "請填整數!";
                    }
                    else
                    {
                        if (ss >= 1 && ss <= 2)
                        {
                            udd.Semester = ss;
                            dgr.Cells[Semester.Index].ErrorText = "";
                        }
                        else
                            dgr.Cells[Semester.Index].ErrorText = "學期必須1或2";
                    }

                // 年排名
                if (dgr.Cells[GradeRank.Index].Value != null)
                    if (int.TryParse(dgr.Cells[GradeRank.Index].Value.ToString(), out sr) == false)
                    {
                        dgr.Cells[GradeRank.Index].ErrorText = "請填整數!";
                    }
                    else
                    {
                        udd.GradeRank = sr;
                        dgr.Cells[GradeRank.Index].ErrorText = "";
                    }

                // 加分後年排名
                if (dgr.Cells[GradeRankAdd.Index].Value != null)
                    if (int.TryParse(dgr.Cells[GradeRankAdd.Index].Value.ToString(), out srAdd) == false)
                    {
                        dgr.Cells[GradeRankAdd.Index].ErrorText = "請填整數!";
                    }
                    else
                    {
                        udd.GradeRankAdd = srAdd;
                        dgr.Cells[GradeRankAdd.Index].ErrorText = "";
                    }

                // 年排名百分比
                if (dgr.Cells[GradeRankPercent.Index].Value != null)
                    if (int.TryParse(dgr.Cells[GradeRankPercent.Index].Value.ToString(), out srp) == false)
                    {
                        dgr.Cells[GradeRankPercent.Index].ErrorText = "請填整數!";
                    }
                    else
                    {
                        udd.GradeRankPercent = srp;
                        dgr.Cells[GradeRankPercent.Index].ErrorText = "";
                    }

                // 加分後年排名百分比
                if (dgr.Cells[GradeRankPercentAdd.Index].Value != null)
                    if (int.TryParse(dgr.Cells[GradeRankPercentAdd.Index].Value.ToString(), out srpAdd) == false)
                    {
                        dgr.Cells[GradeRankPercentAdd.Index].ErrorText = "請填整數!";
                    }
                    else
                    {
                        udd.GradeRankPercentAdd = srpAdd;
                        dgr.Cells[GradeRankPercentAdd.Index].ErrorText = "";
                    }

                // 檢查是否有錯誤
                if(dgr.Cells[GradeYear.Index].ErrorText !="" || dgr.Cells[Semester.Index].ErrorText != "" || dgr.Cells[GradeRank.Index].ErrorText != "" || dgr.Cells[GradeRankPercent.Index].ErrorText != "")
                    hasError =true ;

                // 檢查年級學期是否重複
                if (dgr.Cells[GradeYear.Index].Value != null && dgr.Cells[Semester.Index].Value != null)
                {
                    string strSy = dgr.Cells[SchoolYear.Index].Value.ToString();
                    string strG = dgr.Cells[GradeYear.Index].Value.ToString();
                    string strS=dgr.Cells[Semester.Index].Value.ToString();
                    string str = strSy+strG + strS; 
                    if (!chkGrStr.Contains(str))
                    {
                        InsertData.Add(udd);
                        chkGrStr.Add(str);
                    }
                    else
                    {                        
                        FISCA.Presentation.Controls.MsgBox.Show(strSy+"學年度"+strG +"年級第"+strS+"學期有重複，無法儲存。");
                        hasError = true;
                        break;
                    }
                }
            }

            if (hasError)
            {
                FISCA.Presentation.Controls.MsgBox.Show("資料有誤,無法儲存!");
                return;
            }

            // 檢查沒問題後
            // 刪除舊資料
            // 標要刪除
            foreach (DAL.UserDefData udd in _UserDefDataList)
                udd.Deleted = true;
            DAL.UDTTransfer.DeleteDataToUDT(_UserDefDataList);

            // 新增新資料
            DAL.UDTTransfer.InsertDataToUDT(InsertData);
            //FISCA.Presentation.Controls.MsgBox.Show("儲存完成。");
        }


        private void dgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgv.EndEdit();
            bool validated = true;

            int ssy, sy, ss, sr,srAdd, srp, srpAdd;
            // 檢查年級學期是否重複用
            List<string> chkGrStr = new List<string>();
            // 檢查資料並加入組合
            foreach (DataGridViewRow dgr in dgv.Rows)
            {

                if (dgr.IsNewRow)
                    continue;


                // 學年度
                if (dgr.Cells[SchoolYear.Index].Value != null)
                    if (int.TryParse(dgr.Cells[SchoolYear.Index].Value.ToString(), out ssy) == false)
                        dgr.Cells[SchoolYear.Index].ErrorText = "請填整數!";
                    else
                        dgr.Cells[SchoolYear.Index].ErrorText = "";

                // 年級
                if (dgr.Cells[GradeYear.Index].Value != null)
                    if (int.TryParse(dgr.Cells[GradeYear.Index].Value.ToString(), out sy) == false)                    
                        dgr.Cells[GradeYear.Index].ErrorText = "請填整數!";
                    else
                        dgr.Cells[GradeYear.Index].ErrorText = "";


                // 學期
                if (dgr.Cells[Semester.Index].Value != null)
                    if (int.TryParse(dgr.Cells[Semester.Index].Value.ToString(), out ss) == false)
                        dgr.Cells[Semester.Index].ErrorText = "請填整數!";
                    else
                    {
                        if(ss>=1 && ss <=2)                        
                            dgr.Cells[Semester.Index].ErrorText = "";
                        else
                            dgr.Cells[Semester.Index].ErrorText = "學期必須1或2";
                    }

                // 年排名
                if (dgr.Cells[GradeRank.Index].Value != null)
                    if (int.TryParse(dgr.Cells[GradeRank.Index].Value.ToString(), out sr) == false)
                        dgr.Cells[GradeRank.Index].ErrorText = "請填整數!";
                    else
                        dgr.Cells[GradeRank.Index].ErrorText = "";

                // 加分後年排名
                if (dgr.Cells[GradeRankAdd.Index].Value != null)
                    if (int.TryParse(dgr.Cells[GradeRankAdd.Index].Value.ToString(), out srAdd) == false)
                        dgr.Cells[GradeRankAdd.Index].ErrorText = "請填整數!";
                    else
                        dgr.Cells[GradeRankAdd.Index].ErrorText = "";

                // 年排名百分比
                if (dgr.Cells[GradeRankPercent.Index].Value != null)
                    if (int.TryParse(dgr.Cells[GradeRankPercent.Index].Value.ToString(), out srp) == false)
                        dgr.Cells[GradeRankPercent.Index].ErrorText = "請填整數!";
                    else
                        dgr.Cells[GradeRankPercent.Index].ErrorText = "";

                // 加分後年排名百分比
                if (dgr.Cells[GradeRankPercentAdd.Index].Value != null)
                    if (int.TryParse(dgr.Cells[GradeRankPercentAdd.Index].Value.ToString(), out srpAdd) == false)
                        dgr.Cells[GradeRankPercentAdd.Index].ErrorText = "請填整數!";
                    else
                        dgr.Cells[GradeRankPercentAdd.Index].ErrorText = "";

                // 檢查年級學期是否重複
                if (dgr.Cells[GradeYear.Index].Value != null && dgr.Cells[Semester.Index].Value != null)
                {
                    string strSy = dgr.Cells[SchoolYear.Index].Value.ToString();
                    string strG = dgr.Cells[GradeYear.Index].Value.ToString();
                    string strS = dgr.Cells[Semester.Index].Value.ToString();
                    string str = strG + strS;
                    if (!chkGrStr.Contains(str))
                    {
                        chkGrStr.Add(str);
                    }
                    else
                    {
                        FISCA.Presentation.Controls.MsgBox.Show(strSy+"學年度"+strG + "年級第" + strS + "學期有重複，無法儲存。");
                        return;
                    }
                }
            }
            dgv.BeginEdit(false);
            this.ContentValidated = validated;
            if (validated)
            {
                this.SaveButtonVisible = true;
                this.CancelButtonVisible = true;
            }
            else
            {
                this.SaveButtonVisible = true;
                this.CancelButtonVisible = true;
            }  
        }
    }
}
