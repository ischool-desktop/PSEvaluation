using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.SubjectCombinationRelated
{
    public partial class SubjectContextMenuStrip : ContextMenuStrip
    {
        public DataGridViewCell Cell { get; set; }
        private const string spliter = ", ";

        public SubjectContextMenuStrip()
        {
            InitializeComponent();
        }

        internal void SetSubjects(string sourceSubject, List<string> subjects, List<string> checkList)
        {
            this.SuspendLayout();
            this.Items.Clear();

            foreach (string s in subjects)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(s);
                item.Click += new EventHandler(Item_Click);
                if(checkList.Contains(s)) item.Checked = true;
                if (s == sourceSubject) item.Enabled = item.Checked = false;
                this.Items.Add(item);
            }

            this.ResumeLayout();
        }

        private void Item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            item.Checked = !item.Checked;
            SyncCellValue();
        }

        private void SyncCellValue()
        {
            string value = string.Empty;
            foreach (ToolStripMenuItem item in this.Items)
            {
                if (item.Checked == true)
                    value += item.Text + spliter;
            }
            if (value.EndsWith(spliter)) value = value.Substring(0, value.Length - spliter.Length);

            if (Cell != null) Cell.Value = value;
        }
    }
}
