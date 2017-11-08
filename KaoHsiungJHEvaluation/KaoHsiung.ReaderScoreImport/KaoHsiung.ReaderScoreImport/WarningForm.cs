using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace KaoHsiung.ReaderScoreImport
{
    public partial class WarningForm : BaseForm
    {
        public WarningForm()
        {
            InitializeComponent();

            lblTempCount.Text = "" + K12.Presentation.NLDPanels.Student.TempSource.Count;
        }

        public void Add(string id, string itemDisplay, string message)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dgv, id, itemDisplay, message);
            dgv.Rows.Add(row);
        }

        private void btnAddTemp_Click(object sender, EventArgs e)
        {
            List<string> ids = GetSelectedIDs();
            if (ids.Count <= 0) return;

            K12.Presentation.NLDPanels.Student.AddToTemp(ids);
            lblTempCount.Text = "" + K12.Presentation.NLDPanels.Student.TempSource.Count;
        }

        private void btnRemoveTemp_Click(object sender, EventArgs e)
        {
            List<string> ids = GetSelectedIDs();
            if (ids.Count <= 0) return;

            K12.Presentation.NLDPanels.Student.RemoveFromTemp(ids);
            lblTempCount.Text = "" + K12.Presentation.NLDPanels.Student.TempSource.Count;
        }

        private List<string> GetSelectedIDs()
        {
            List<string> ids = new List<string>();
            foreach (DataGridViewRow row in dgv.SelectedRows)
            {
                if (row.IsNewRow) continue;
                string id = "" + row.Cells[chID.Index].Value;
                if (!string.IsNullOrEmpty(id) && !ids.Contains(id)) ids.Add(id);
            }
            return ids;
        }

        private void btnGoOn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void WarningForm_Shown(object sender, EventArgs e)
        {
            MsgBox.Show("有 " + dgv.Rows.Count + " 位學生已有成績，點選「" + btnGoOn.Text + "」會將原有的成績覆蓋。");
            btnGoOn.Focus();
        }
    }
}
