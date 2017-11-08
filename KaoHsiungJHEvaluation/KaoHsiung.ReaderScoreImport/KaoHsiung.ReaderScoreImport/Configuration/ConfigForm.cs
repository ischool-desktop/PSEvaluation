using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using FISCA.UDT;
using KaoHsiung.ReaderScoreImport.UDT;

namespace KaoHsiung.ReaderScoreImport
{
    public partial class ConfigForm : BaseForm
    {
        protected BackgroundWorker _worker;
        protected AccessHelper _accessHelper;

        public ConfigForm()
        {
            InitializeComponent();
            InitAccessHelper();
            InitWorker();
        }

        private void InitAccessHelper()
        {
            _accessHelper = new AccessHelper();
        }

        private void InitWorker()
        {
            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
            picWaiting.Visible = true;
            //_worker.RunWorkerAsync();
        }

        public void Startup()
        {
            _worker.RunWorkerAsync();
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            DoWork(sender, e);
        }

        protected virtual void DoWork(object sender, DoWorkEventArgs e) { }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            picWaiting.Visible = false;
            RunWorkerCompleted(sender, e);
        }

        protected virtual void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) { }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        protected virtual void Save() { }

        protected bool IsValid(DataGridView dgv)
        {
            bool valid = true;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!string.IsNullOrEmpty(cell.ErrorText))
                    {
                        valid &= false;
                        break;
                    }
                }
            }

            return valid;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected void ValidateDuplication(DataGridView dgv, int index, string message)
        {
            List<string> codeList = new List<string>();
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                DataGridViewCell runningCell = row.Cells[index];
                if (string.IsNullOrEmpty("" + runningCell.Value)) continue;

                if (!codeList.Contains("" + runningCell.Value))
                {
                    codeList.Add("" + runningCell.Value);
                    runningCell.ErrorText = string.Empty;
                }
                else if (string.IsNullOrEmpty(runningCell.ErrorText))
                    runningCell.ErrorText = message;
            }
        }

        protected void ValidateCodeFormat(DataGridViewCell cell, int num)
        {
            int i;
            string code = "" + cell.Value;
            if (code.Length != num)
                cell.ErrorText = "代碼必須為 " + num + " 位數";
            else if (!int.TryParse(code, out i))
                cell.ErrorText = "代碼必須為數字";
            else
                cell.ErrorText = "";
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Export();
        }

        protected virtual void Export()
        {
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Import();
        }

        protected virtual void Import()
        {
        }
    }
}
