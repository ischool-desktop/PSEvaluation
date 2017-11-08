using FISCA.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JHEvaluation.ExamScoreCopy
{
    internal partial class MessageView : BaseForm
    {
        private MessageItemType Type { get; set; }

        public bool HasMessage { get { return dgv.Rows.Count > 0; } }

        public MessageView(MessageItemType type)
        {
            InitializeComponent();
            Type = type;
            if (type == MessageItemType.Course)
            {
                chItem.HeaderText = "課程";
                lblTempCount.Text = "" + K12.Presentation.NLDPanels.Course.TempSource.Count;
            }
            else
            {
                chItem.HeaderText = "學生";
                lblTempCount.Text = "" + K12.Presentation.NLDPanels.Student.TempSource.Count;
            }
        }

        public void Add(string id, string itemDisplay, string message)
        {
            Add(id, itemDisplay, message, MessageLevel.Normal);
        }

        public void Add(string id, string itemDisplay, string message, MessageLevel level)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dgv, id, itemDisplay, message);
            if (level == MessageLevel.Warning)
            {
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Style.BackColor = Color.Yellow;
            }
            else if (level == MessageLevel.Error)
            {
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Style.BackColor = Color.Red;
            }
            dgv.Rows.Add(row);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnAddTemp_Click(object sender, EventArgs e)
        {
            List<string> ids = GetSelectedIDs();
            if (ids.Count <= 0) return;

            if (Type == MessageItemType.Course)
            {
                K12.Presentation.NLDPanels.Course.AddToTemp(ids);
                lblTempCount.Text = "" + K12.Presentation.NLDPanels.Course.TempSource.Count;
            }
            else if (Type == MessageItemType.Student)
            {
                K12.Presentation.NLDPanels.Student.AddToTemp(ids);
                lblTempCount.Text = "" + K12.Presentation.NLDPanels.Student.TempSource.Count;
            }
        }

        private void btnRemoveTemp_Click(object sender, EventArgs e)
        {
            List<string> ids = GetSelectedIDs();
            if (ids.Count <= 0) return;

            if (Type == MessageItemType.Course)
            {
                K12.Presentation.NLDPanels.Course.RemoveFromTemp(ids);
                lblTempCount.Text = "" + K12.Presentation.NLDPanels.Course.TempSource.Count;
            }
            else if (Type == MessageItemType.Student)
            {
                K12.Presentation.NLDPanels.Student.RemoveFromTemp(ids);
                lblTempCount.Text = "" + K12.Presentation.NLDPanels.Student.TempSource.Count;
            }
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

        internal string Title
        {
            set
            {
                Text = "訊息檢視 - " + value;
            }
        }

        internal string ButtonText
        {
            set { btnGoOn.Text = "" + value; }
        }

        internal bool ButtonVisible
        {
            set { btnGoOn.Visible = value; }
        }

        private void MessageView_Load(object sender, EventArgs e)
        {
            dgv.Sort(chItem, ListSortDirection.Ascending);
            dgv.ClearSelection();
        }
    }

    internal enum MessageItemType
    {
        Course, Student
    }

    internal enum MessageLevel
    {
        Normal, Warning, Error
    }
}
