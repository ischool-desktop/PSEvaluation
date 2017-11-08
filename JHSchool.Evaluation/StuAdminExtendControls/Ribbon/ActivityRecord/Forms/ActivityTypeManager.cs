using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord.DAL;
using JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord.Data;
using DevComponents.DotNetBar;

namespace JHSchool.Evaluation.StuAdminExtendControls.Ribbon
{
    public partial class ActivityTypeManager : FISCA.Presentation.Controls.BaseForm
    {
        private List<string> _types;

        public ActivityTypeManager()
        {
            InitializeComponent();

            _types = ActivityRecordDAL.GetActivityRecordTypes();

            foreach (string type in _types)
            {
                cboType.Items.Add(type);
            }
            dgEditor.Columns.Add("細項", "細項");
            dgEditor.Columns.Add("數量", "數量");
            cboType.SelectedIndex = 0;
        }

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgEditor.Rows.Clear();

            foreach (MappingItem item in ActivityRecordDAL.GetActivityRecordMappingItems(cboType.Text))
            {
                if (item.Type != cboType.Text) continue;

                int rowIndex = dgEditor.Rows.Add();
                dgEditor.Rows[rowIndex].Cells["細項"].Value = item.Item;
                dgEditor.Rows[rowIndex].Cells["數量"].Value = item.Count;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgEditor.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!string.IsNullOrEmpty(cell.ErrorText))
                    {
                        MessageBoxEx.Show("輸入資料有誤, 請修正後再行儲存!","錯誤",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            List<MappingItem> items = new List<MappingItem>();

            foreach (DataGridViewRow row in dgEditor.Rows)
            {
                MappingItem item = new MappingItem();

                string value = string.Empty;
                DataGridViewCell cell = row.Cells["細項"];

                if (cell.Value == null)
                    continue;
                value = cell.Value.ToString();
                if (value == string.Empty)
                    continue;

                string countString;
                int count;
                cell = row.Cells["數量"];
                if (cell.Value == null)
                {
                    count = 1;
                }
                else
                {
                    countString = cell.Value.ToString();
                    if (!int.TryParse(countString, out count))
                        count = 1;
                }
                item.Item = value;
                item.Count = count;
                items.Add(item);
            }
            ActivityRecordDAL.SetActivityRecordMapping(cboType.Text, items.ToArray());
            MessageBoxEx.Show(cboType.Text + "類別已儲存完畢!");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgEditor_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewColumn column = dgEditor.Columns[e.ColumnIndex];
            if (column.Name == "數量")
            {
                DataGridViewCell cell = dgEditor.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.ErrorText = string.Empty;
                string value = string.Empty;
                if (cell.Value != null)
                    value = cell.Value.ToString();
                int count;
                if (!int.TryParse(value, out count))
                {
                    cell.ErrorText = "必須為數字";
                }
            }
        }
    }
}
