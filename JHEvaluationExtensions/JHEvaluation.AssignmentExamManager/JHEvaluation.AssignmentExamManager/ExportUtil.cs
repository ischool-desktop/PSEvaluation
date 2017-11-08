using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aspose.Cells;
using FISCA.Presentation.Controls;
using System.Diagnostics;

namespace JHEvaluation.AssignmentExamManager
{
    internal class ExportUtil
    {
        /// <summary>
        /// Export DataGridView Contents
        /// </summary>
        /// <param name="dgv">DataGridView Control</param>
        /// <param name="name">Export Report Name</param>
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
                    MsgBox.Show("儲存失敗。\n" + ex.Message);
                    return;
                }

                try
                {
                    if (MsgBox.Show("是否立即開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        Process.Start(sd.FileName);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("開啟失敗。\n" + ex.Message);
                    return;
                }
            }
        }
    }
}
