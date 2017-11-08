using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace KaoHsiung.ReaderScoreImport
{
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
            Import(dgv, fields, new Dictionary<string,IColumnValidator>());
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
}
