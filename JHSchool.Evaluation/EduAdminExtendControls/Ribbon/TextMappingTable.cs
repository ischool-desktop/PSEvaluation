using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Framework;
using System.Xml;
using Aspose.Cells;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon
{
    public partial class TextMappingTable : FISCA.Presentation.Controls.BaseForm
    {
        private const string ConfigName = "文字描述代碼表";

        private const string Catalog = "Catalog";

        public TextMappingTable()
        {
            InitializeComponent();
            InitializeTextCode();
        }

        private void InitializeTextCode()
        {
            try
            {
                ConfigData cd = School.Configuration[ConfigName];
                if (!string.IsNullOrEmpty(cd["xml"]))
                {
                    dgv.SuspendLayout();
                    dgv.Rows.Clear();
                    XmlHelper helper = new XmlHelper(XmlHelper.LoadXml(cd["xml"]));
                    foreach (XmlElement item in helper.GetElements("Item"))
                    {
                        string code = item.GetAttribute("Code");
                        string content = item.GetAttribute("Content");
                        string catalog = item.GetAttribute(Catalog);

                        DataGridViewRow row = new DataGridViewRow();
                        row.CreateCells(dgv, catalog, code, content);
                        dgv.Rows.Add(row);
                    }
                    dgv.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show("文字描述代碼表讀取失敗。" + ex.Message);
            }
        }

        private void dgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgv.EndEdit();

            foreach (DataGridViewCell cell in dgv.CurrentRow.Cells)
            {
                cell.ErrorText = string.Empty;

                if (cell.OwningColumn == chCode)
                {
                    //int i;
                    if (string.IsNullOrEmpty("" + cell.Value))
                        cell.ErrorText = "代碼不能為空白";
                    //else if (!int.TryParse("" + cell.Value, out i))
                    //    cell.ErrorText = "代碼必須為數字";
                }
                else if (cell.OwningColumn == chContent)
                {
                    if (string.IsNullOrEmpty("" + cell.Value))
                        cell.ErrorText = "評語內容不能為空白";
                }
            }

            dgv.BeginEdit(false);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValid()) return;

            try
            {
                ConfigData cd = School.Configuration[ConfigName];

                XmlDocument doc = new XmlDocument();
                XmlElement xml = doc.CreateElement("TextMappingTable");
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;
                    string code = "" + row.Cells[chCode.Index].Value;
                    string content = "" + row.Cells[chContent.Index].Value;
                    string catalog = "" + row.Cells[chCatalog.Index].Value;

                    XmlElement item = doc.CreateElement("Item");
                    item.SetAttribute(Catalog, catalog);
                    item.SetAttribute("Code", code);
                    item.SetAttribute("Content", content);

                    xml.AppendChild(item);
                }

                cd["xml"] = xml.OuterXml;
                cd.Save();
            }
            catch (Exception ex)
            {
                MsgBox.Show("文字描述代碼表儲存失敗。" + ex.Message);
            }
            this.DialogResult = DialogResult.OK;
        }

        private bool IsValid()
        {
            bool valid = true;

            List<string> codeList = new List<string>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    DataGridViewColumn owner = cell.OwningColumn;

                    if (owner == chContent)
                    {
                        if (string.IsNullOrEmpty("" + cell.Value))
                        {
                            valid &= false;
                            cell.ErrorText = "評語內容不能空白";
                        }
                        else
                            cell.ErrorText = string.Empty;
                    }

                    if (owner == chCode)
                    {
                        if (!codeList.Contains("" + cell.Value))
                        {
                            codeList.Add("" + cell.Value);
                            cell.ErrorText = string.Empty;
                        }
                        else
                        {
                            valid &= false;
                            cell.ErrorText = "代碼不能重覆";
                        }
                    }
                }
            }

            return valid;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ImportExport.Export(dgv, "匯出" + ConfigName);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            ImportExport.Import(dgv, new List<string>() { "分類", "代碼", "評語內容" });
            IsValid();
        }
    }

    internal class ImportExport
    {
        internal static void Export(DataGridView dgv, string name)
        {
            Workbook book = new Workbook();
            book.Worksheets.Clear();
            Worksheet ws = book.Worksheets[book.Worksheets.Add()];
            ws.Name = name;

            int index = 0;
            Dictionary<string, int> map = new Dictionary<string, int>();

            #region 建立標題
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                DataGridViewColumn col = dgv.Columns[i];
                ws.Cells[index, i].PutValue(col.HeaderText);
                map.Add(col.HeaderText, i);
            }
            index++;
            #endregion

            #region 填入內容
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    int column = map[cell.OwningColumn.HeaderText];
                    ws.Cells[index, column].PutValue("" + cell.Value);
                }
                index++;
            }
            #endregion

            SaveFileDialog sd = new SaveFileDialog();
            sd.FileName = name;
            sd.Filter = "Excel檔案(*.xls)|*.xls";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    book.Save(sd.FileName, FileFormatType.Excel2003);
                    MsgBox.Show("匯出完成。");
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存失敗。" + ex.Message);
                }
            }
        }

        internal static void Import(DataGridView dgv, List<string> fields)
        {
            Import(dgv, fields, new Dictionary<string, IColumnValidator>());
        }

        internal static void Import(DataGridView dgv, List<string> fields, Dictionary<string, IColumnValidator> validators)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Excel檔案(*.xls)|*.xls";
            if (od.ShowDialog() != DialogResult.OK) return;

            Workbook book = new Workbook();
            book.Open(od.FileName);
            Worksheet ws = book.Worksheets[0];

            int index = 0;
            Dictionary<string, int> colmap = new Dictionary<string, int>();
            Dictionary<int, int> map = new Dictionary<int, int>();

            #region 檢查標題是否正確
            for (int i = 0; i <= ws.Cells.MaxDataColumn; i++)
            {
                Cell cell = ws.Cells[index, i];
                string value = "" + cell.Value;

                int dgvColIndex;
                if (fields.Contains(value) && TryGetColumnIndex(dgv, value, out dgvColIndex))
                {
                    colmap.Add(value, i);
                    map.Add(i, dgvColIndex);
                    fields.Remove(value);
                }
            }

            if (fields.Count > 0)
            {
                StringBuilder builder = new StringBuilder("");
                builder.AppendLine("匯入資料有誤。");
                builder.Append("缺少欄位：");
                foreach (var f in fields)
                    builder.Append(f + "、");
                string msg = builder.ToString();
                if (msg.EndsWith("、")) msg = msg.Substring(0, msg.Length - 1);
                MsgBox.Show(msg);
                return;
            }
            #endregion

            #region 檢查欄位是否有效
            bool valid = true;
            foreach (string header in validators.Keys)
            {
                if (!colmap.ContainsKey(header)) continue;

                int colIndex = colmap[header];
                IColumnValidator validator = validators[header];
                for (int i = 1; i <= ws.Cells.MaxDataRow; i++)
                {
                    Cell cell = ws.Cells[i, colIndex];
                    if (!validator.IsValid("" + cell.Value))
                    {
                        //ws.Comments[cell.Name].Note = validator.GetErrorMessage();
                        valid &= false;
                    }
                }
            }
            if (!valid)
            {
                MsgBox.Show("資料有誤。");
                return;
            }
            #endregion

            #region 填入 DataGridView
            if (MsgBox.Show("匯入動作會將覆蓋目前畫面上的設定，請問是否要繼續？", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            dgv.SuspendLayout();
            dgv.Rows.Clear();
            for (int i = 1; i <= ws.Cells.MaxDataRow; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv);
                foreach (int colIndex in map.Keys)
                {
                    Cell cell = ws.Cells[i, colIndex];
                    row.Cells[map[colIndex]].Value = "" + cell.Value;
                }
                dgv.Rows.Add(row);
            }
            dgv.ResumeLayout();
            #endregion

            MsgBox.Show("匯入完成，請記得儲存設定。");
        }

        private static bool TryGetColumnIndex(DataGridView dgv, string headerText, out int colIndex)
        {
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.HeaderText == headerText)
                {
                    colIndex = col.Index;
                    return true;
                }
            }
            colIndex = 0;
            return false;
        }
    }

    internal interface IColumnValidator
    {
        bool IsValid(string input);
        string GetErrorMessage();
    }
}
