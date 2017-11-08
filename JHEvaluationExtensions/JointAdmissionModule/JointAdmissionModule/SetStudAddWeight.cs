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
    public partial class SetStudAddWeight : FISCA.Presentation.Controls.BaseForm
    {
        BackgroundWorker _wbWork;

        List<string> _StudentTypeList;
        List<string> _JoinStudSpceTypeList;
        List<DAL.UserDefData_StudTypeWeight> _udd;
        public SetStudAddWeight()
        {
            InitializeComponent();
            this.MaximumSize = this.MinimumSize = this.Size;
            _StudentTypeList = new List<string>();
            _JoinStudSpceTypeList = new List<string>();
            _udd = new List<JointAdmissionModule.DAL.UserDefData_StudTypeWeight>();

            _wbWork = new BackgroundWorker();
            _wbWork.DoWork += new DoWorkEventHandler(_wbWork_DoWork);
            _wbWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_wbWork_RunWorkerCompleted);
            _wbWork.RunWorkerAsync();
        }

        void _wbWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string ColName0 = "JoinStudType", HeadText0 = "考生特種身分別", ColName05 = "JoinStudType5";
            string ColName = "StudType", HeadText = "學生類別", ColName1 = "StudType5";
            DataGridViewComboBoxColumn dgvcbcSH0 = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn dgvcbcSH05 = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn dgvcbcSH = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn dgvcbcSH5 = new DataGridViewComboBoxColumn();

            // 高中
            dgvSHWeight.Rows.Clear();
            if (dgvSHWeight.Columns.Contains(ColName0))
                dgvSHWeight.Columns.Remove(ColName0);
            dgvcbcSH0.Items.Add("");
            dgvcbcSH0.Items.AddRange(_JoinStudSpceTypeList.ToArray());
            dgvcbcSH0.Name = ColName0;
            dgvcbcSH0.HeaderText = HeadText0;
            dgvcbcSH0.Width = 200;
            dgvcbcSH0.AutoComplete = true;
            dgvSHWeight.Columns.Insert(0, dgvcbcSH0);


            // 五專
            dgvSH5Weight.Rows.Clear();
            if (dgvSH5Weight.Columns.Contains(ColName05))
                dgvSH5Weight.Columns.Remove(ColName05);
            dgvcbcSH05.Items.Add("");
            dgvcbcSH05.Items.AddRange(_JoinStudSpceTypeList.ToArray());
            dgvcbcSH05.Name = ColName05;
            dgvcbcSH05.HeaderText = HeadText0;
            dgvcbcSH05.Width = 200;
            dgvcbcSH05.AutoComplete = true;
            dgvSH5Weight.Columns.Insert(0, dgvcbcSH05);


            // 高中
            dgvSHWeight.Rows.Clear();
            if (dgvSHWeight.Columns.Contains(ColName))
                dgvSHWeight.Columns.Remove(ColName);
            dgvcbcSH.Items.Add("");
            dgvcbcSH.Items.AddRange(_StudentTypeList.ToArray());
            dgvcbcSH.Name = ColName;
            dgvcbcSH.HeaderText = HeadText;
            dgvcbcSH.Width = 250;
            dgvcbcSH.AutoComplete = true;
            dgvSHWeight.Columns.Insert(1, dgvcbcSH);
            dgvSHWeight.DataError += new DataGridViewDataErrorEventHandler(dgvSHWeight_DataError);

            // 五專
            dgvSH5Weight.Rows.Clear();
            if (dgvSH5Weight.Columns.Contains(ColName1))
                dgvSH5Weight.Columns.Remove(ColName1);
            dgvcbcSH5.Items.Add("");
            dgvcbcSH5.Items.AddRange(_StudentTypeList.ToArray());
            dgvcbcSH5.Name = ColName1;
            dgvcbcSH5.HeaderText = HeadText;
            dgvcbcSH5.Width = 250;
            dgvcbcSH5.AutoComplete = true;
            dgvSH5Weight.Columns.Insert(1, dgvcbcSH5);
            dgvSH5Weight.DataError += new DataGridViewDataErrorEventHandler(dgvSH5Weight_DataError);

            LoadDataToDataGridView();
        }

        void dgvSHWeight_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        void dgvSH5Weight_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        void _wbWork_DoWork(object sender, DoWorkEventArgs e)
        {
            // 取得學生類別完整名稱
            _StudentTypeList = DAL.DALTransfer.GetStudTypeConfigFullNameList();
            // 取得特種身分名稱
            _JoinStudSpceTypeList = DAL.DALTransfer.GetJoinStudSpecTypeName();

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 驗證資料
            foreach (DataGridViewRow dgvr in dgvSHWeight.Rows)
                if (dgvr.Cells[AddWeight.Index].ErrorText != "")
                {
                    FISCA.Presentation.Controls.MsgBox.Show("資料有誤，無法儲存!");
                    return;
                }

            foreach (DataGridViewRow dgvr in dgvSH5Weight.Rows)
                if (dgvr.Cells[AddWeight5.Index].ErrorText != "")
                {
                    FISCA.Presentation.Controls.MsgBox.Show("資料有誤，無法儲存!");
                    return;
                }

            // 清除舊資料
            foreach (DAL.UserDefData_StudTypeWeight ud in _udd)
                ud.Deleted = true;
            DAL.UDTTransfer.DeleteDataToUDT_StudTypeWeight(_udd);

            // 新增資料
            List<DAL.UserDefData_StudTypeWeight> InsertData = new List<JointAdmissionModule.DAL.UserDefData_StudTypeWeight>();
            foreach (DataGridViewRow dgvr in dgvSHWeight.Rows)
            {
                if (dgvr.IsNewRow)
                    continue;
                DAL.UserDefData_StudTypeWeight ud = new JointAdmissionModule.DAL.UserDefData_StudTypeWeight();
                ud.SchoolType = "高中";
                if (dgvr.Cells[JoinStudType.Index].Value != null)
                    ud.JoinStudType = dgvr.Cells[JoinStudType.Index].Value.ToString();
                if (dgvr.Cells[StudType.Index].Value != null)
                    ud.StudentType = dgvr.Cells[StudType.Index].Value.ToString();
                decimal dd; bool bl;
                if (dgvr.Cells[AddWeight.Index].Value != null)
                    if (decimal.TryParse(dgvr.Cells[AddWeight.Index].Value.ToString(), out dd))
                        ud.AddWeight = dd;
                if (dgvr.Cells[checkNonRank.Index].Value != null)
                    if (bool.TryParse(dgvr.Cells[checkNonRank.Index].Value.ToString(), out bl))
                        ud.CheckNonRank = bl;

                if (dgvr.Cells[JoinStudTypeCode.Index].Value != null)
                    ud.JoinStudTypeCode = dgvr.Cells[JoinStudTypeCode.Index].Value.ToString();

                InsertData.Add(ud);
            }

            foreach (DataGridViewRow dgvr in dgvSH5Weight.Rows)
            {
                if (dgvr.IsNewRow)
                    continue;
                DAL.UserDefData_StudTypeWeight ud = new JointAdmissionModule.DAL.UserDefData_StudTypeWeight();
                ud.SchoolType = "五專";
                if (dgvr.Cells[JoinStudType5.Index].Value != null)
                    ud.JoinStudType = dgvr.Cells[JoinStudType5.Index].Value.ToString();

                if (dgvr.Cells[StudType5.Index].Value != null)
                    ud.StudentType = dgvr.Cells[StudType5.Index].Value.ToString();

                decimal dd; bool bl;
                if (dgvr.Cells[AddWeight5.Index].Value != null)
                    if (decimal.TryParse(dgvr.Cells[AddWeight5.Index].Value.ToString(), out dd))
                        ud.AddWeight = dd;
                if (dgvr.Cells[checkNonRank5.Index].Value != null)
                    if (bool.TryParse(dgvr.Cells[checkNonRank5.Index].Value.ToString(), out bl))
                        ud.CheckNonRank = bl;

                if (dgvr.Cells[JoinStudTypeCode5.Index].Value != null)
                    ud.JoinStudTypeCode = dgvr.Cells[JoinStudTypeCode5.Index].Value.ToString();

                InsertData.Add(ud);
            }

            if (InsertData.Count > 0)
            {
                DAL.UDTTransfer.InsertDataToUDT_StudTypeWeight(InsertData);
                FISCA.Presentation.Controls.MsgBox.Show("儲存完成");
                this.Close();
            }
        }

        /// <summary>
        /// 載入畫面資料
        /// </summary>
        private void LoadDataToDataGridView()
        {
            // 取得 UDT 內的設定值
            _udd = DAL.UDTTransfer.GetDataFromUDT_StudTypeWeight();

            //// 清除舊資料
            //foreach (DAL.UserDefData_StudTypeWeight ud in _udd)
            //    ud.Deleted = true;
            //DAL.UDTTransfer.DeleteDataToUDT_StudTypeWeight(_udd);
            try
            {
                int rowIdx = 0;
                // 高中
                foreach (DAL.UserDefData_StudTypeWeight ud in _udd.Where(x => x.SchoolType == "高中"))
                {
                    dgvSHWeight.Rows.Add();
                    dgvSHWeight.Rows[rowIdx].Cells[JoinStudType.Index].Value = ud.JoinStudType.ToString();
                    dgvSHWeight.Rows[rowIdx].Cells[StudType.Index].Value = ud.StudentType.ToString();
                    dgvSHWeight.Rows[rowIdx].Cells[AddWeight.Index].Value = ud.AddWeight.ToString();
                    dgvSHWeight.Rows[rowIdx].Cells[checkNonRank.Index].Value = ud.CheckNonRank;
                    dgvSHWeight.Rows[rowIdx].Cells[JoinStudTypeCode.Index].Value = ud.JoinStudTypeCode;
                    rowIdx++;
                }

                // 五專
                rowIdx = 0;
                foreach (DAL.UserDefData_StudTypeWeight ud in _udd.Where(x => x.SchoolType == "五專"))
                {
                    dgvSH5Weight.Rows.Add();
                    dgvSH5Weight.Rows[rowIdx].Cells[JoinStudType5.Index].Value = ud.JoinStudType.ToString();
                    dgvSH5Weight.Rows[rowIdx].Cells[StudType5.Index].Value = ud.StudentType.ToString();
                    dgvSH5Weight.Rows[rowIdx].Cells[AddWeight5.Index].Value = ud.AddWeight.ToString();
                    dgvSH5Weight.Rows[rowIdx].Cells[checkNonRank5.Index].Value = ud.CheckNonRank;
                    dgvSH5Weight.Rows[rowIdx].Cells[JoinStudTypeCode5.Index].Value = ud.JoinStudTypeCode;
                    rowIdx++;
                }
            }
            catch (Exception ex)
            { }
        }

        private void dgvSHWeight_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgvSHWeight.EndEdit();

            if (dgvSHWeight.CurrentCell.ColumnIndex == AddWeight.Index)
            {
                if (dgvSHWeight.CurrentCell.Value != null)
                {
                    decimal d;
                    if (decimal.TryParse(dgvSHWeight.CurrentCell.Value.ToString(), out d))
                        dgvSHWeight.CurrentCell.ErrorText = "";
                    else
                        dgvSHWeight.CurrentCell.ErrorText = "必須數值";
                }
            }

            //foreach (DataGridViewRow dgvr in dgvSHWeight.Rows)
            //{
            //    decimal d;
            //    if (dgvr.Cells[AddWeight.Index].Value != null)
            //        if (decimal.TryParse(dgvr.Cells[AddWeight.Index].Value.ToString(), out d))
            //            dgvr.Cells[AddWeight.Index].ErrorText = "";
            //        else
            //            dgvr.Cells[AddWeight.Index].ErrorText = "必須數值";
            //}

            dgvSHWeight.BeginEdit(false);
        }

        private void dgvSH5Weight_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgvSH5Weight.EndEdit();

            if (dgvSH5Weight.CurrentCell.ColumnIndex == AddWeight5.Index)
            {
                if (dgvSH5Weight.CurrentCell.Value != null)
                {
                    decimal d;
                    if (decimal.TryParse(dgvSH5Weight.CurrentCell.Value.ToString(), out d))
                        dgvSH5Weight.CurrentCell.ErrorText = "";
                    else
                        dgvSH5Weight.CurrentCell.ErrorText = "必須數值";
                }
            }

            //foreach (DataGridViewRow dgvr in dgvSH5Weight.Rows)
            //{
            //    decimal d;
            //    if (dgvr.Cells[AddWeight5.Index].Value != null)
            //        if (decimal.TryParse(dgvr.Cells[AddWeight5.Index].Value.ToString(), out d))
            //            dgvr.Cells[AddWeight5.Index].ErrorText = "";
            //        else
            //            dgvr.Cells[AddWeight5.Index].ErrorText = "必須數值";
            //}

            dgvSH5Weight.BeginEdit(false);
        }


    }
}