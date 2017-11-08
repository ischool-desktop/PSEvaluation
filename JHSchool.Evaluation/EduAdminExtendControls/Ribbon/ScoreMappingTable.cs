using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Framework;
using System.IO;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon
{
    public partial class ScoreMappingTable : FISCA.Presentation.Controls.BaseForm
    {
        public ScoreMappingTable()
        {
            InitializeComponent();
        }

        private void ScoreMappingTable_Load(object sender, EventArgs e)
        {
            dgv.Rows.Clear();

            ConfigData cd = School.Configuration["等第對照表"];
            if (string.IsNullOrEmpty(cd["xml"]))
                cd["xml"] = GetDefault();

            XmlElement element = XmlHelper.LoadXml(cd["xml"]);

            foreach (XmlElement each in element.SelectNodes("ScoreMapping"))
            {
                string name = each.GetAttribute("Name");
                string EngName = each.GetAttribute("EngName");
                string score = each.GetAttribute("Score");

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, name,EngName, score);
                dgv.Rows.Add(row);

            }

            FillDescription();
        }

        private string GetDefault()
        {
            return XmlHelper.LoadXml(Properties.Resources.預設等第對照表).OuterXml;
        }

        public static void CheckDefault()
        {
            ConfigData cd = School.Configuration["等第對照表"];
            if (string.IsNullOrEmpty(cd["xml"]))
            {
                cd["xml"] = XmlHelper.LoadXml(Properties.Resources.預設等第對照表).OuterXml;
                cd.Save();
            }
        }

        private void FillDescription()
        {
            string last = string.Empty;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                row.Cells[chScore.Index].Style.ForeColor = Color.Black;

                if (string.IsNullOrEmpty(last))
                    row.Cells[chDesc.Index].Value = "" + row.Cells[chScore.Index].Value + "分以上";
                else
                    row.Cells[chDesc.Index].Value = "" + row.Cells[chScore.Index].Value + "分以上未滿" + last + "分";

                if (row.Index != dgv.Rows.Count - 2)
                    last = "" + row.Cells[chScore.Index].Value;
            }

            if (dgv.Rows.Count > 2)
            {
                DataGridViewRow lastRow = dgv.Rows[dgv.Rows.Count - 2];
                lastRow.Cells[chDesc.Index].Value = "未滿" + last + "分";
                lastRow.Cells[chScore.Index].Style.ForeColor = Color.LightGray;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValid())
            {
                MsgBox.Show("請先修正錯誤再儲存。");
                return;
            }

            if (dgv.Rows.Count <= 2)
            {
                MsgBox.Show("對照表中至少要有兩筆以上資料。");
                return;
            }

            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement("ScoreMappingList");

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                XmlElement each = doc.CreateElement("ScoreMapping");
                if(row.Cells[chName.Index] !=null )
                    each.SetAttribute("Name", "" + row.Cells[chName.Index].Value);
                if(row.Cells[chEngName.Index] !=null )
                    each.SetAttribute("EngName", "" + row.Cells[chEngName.Index].Value);
                if(row.Cells[chScore.Index] !=null )
                    each.SetAttribute("Score", "" + row.Cells[chScore.Index].Value);

                if (row.Index == dgv.Rows.Count - 2)
                    each.SetAttribute("Score", "");

                element.AppendChild(each);
            }

            ConfigData cd = School.Configuration["等第對照表"];
            cd["xml"] = element.OuterXml;

            cd.Save();

            this.Close();
        }

        private bool IsValid()
        {
            bool error = false;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!string.IsNullOrEmpty(cell.ErrorText))
                    {
                        error = true;
                        break;
                    }
                }
            }

            return !error;
        }

        private bool ValidateAll()
        {
            bool rowError = false;
            bool rowOrderError = false;

            decimal high = 100;
            List<string> nameList = new List<string>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                rowError |= ValidateRow(row);

                string code = "" + row.Cells[chName.Index].Value;
                if (nameList.Contains(code))
                {
                    row.Cells[chName.Index].ErrorText = "等第不能重覆";
                    row.Cells[chEngName.Index].ErrorText = "等第不能重覆";
                    rowOrderError = true;
                }
                else
                {
                    nameList.Add(code);
                    row.Cells[chName.Index].ErrorText = "";
                    row.Cells[chEngName.Index].ErrorText = "";
                }

                if (!rowError && row.Index != dgv.Rows.Count - 2)
                {
                    decimal score = decimal.Parse("" + row.Cells[chScore.Index].Value);
                    if (score >= high)
                    {
                        row.Cells[chScore.Index].ErrorText = "最低分必須比上一筆低";
                        rowOrderError = true;
                    }
                    else
                        row.Cells[chScore.Index].ErrorText = "";
                    high = score;
                }
            }

            return !(rowError | rowOrderError);
        }

        private bool ValidateRow(DataGridViewRow row)
        {
            bool error = false;

            if (string.IsNullOrEmpty("" + row.Cells[chName.Index].Value))
            {
                row.Cells[chName.Index].ErrorText = "等第不可為空白";
                row.Cells[chEngName.Index].ErrorText = "等第不可為空白";
                error = true;
            }
            else
            {
                row.Cells[chName.Index].ErrorText = "";
                row.Cells[chEngName.Index].ErrorText = "";
            }

            if (row.Index != dgv.Rows.Count - 2)
            {
                decimal d;
                if (!decimal.TryParse("" + row.Cells[chScore.Index].Value, out d))
                {
                    row.Cells[chScore.Index].ErrorText = "最低分必須為數字";
                    error = true;
                }
                else
                    row.Cells[chScore.Index].ErrorText = "";
            }

            return error;
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != chName.Index && e.ColumnIndex != chScore.Index) return;
            if (e.RowIndex < 0) return;

            dgv.BeginEdit(true);
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            ValidateAll();
            FillDescription();
        }

        private void dgv_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ValidateAll();
            FillDescription();
        }

        private void dgv_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            ValidateAll();
            FillDescription();
        }
    }
}
