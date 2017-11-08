using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JHSchool.Evaluation.Legacy
{
    public partial class ErrorViewer : FISCA.Presentation.Controls.BaseForm
    {
        public ErrorViewer()
        {
            InitializeComponent();
        }

        public void SetTitle(string title)
        {
            Text = title;
        }

        public void SetColumns(params string[] columns)
        {
            foreach (var col in columns)
            {
                DataGridViewColumn column = new DataGridViewTextBoxColumn();
                column.HeaderText = col;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgv.Columns.Add(column);
            }
        }

        public void AddRow(params string[] rows)
        {
            List<string> rowList = new List<string>(rows);
            DataGridViewRow row = dgv.Rows[dgv.Rows.Add()];

            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if (column.Index < rowList.Count)
                    row.Cells[column.Index].Value = rowList[column.Index];
            }
        }
    }
}
