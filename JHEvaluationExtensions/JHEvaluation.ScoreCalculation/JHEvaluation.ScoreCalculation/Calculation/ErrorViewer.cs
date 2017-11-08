using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Aspose.Cells;
using Framework;

namespace JHSchool.Evaluation.Calculation_for_JHEvaluation.ScoreCalculation
{
    public partial class ErrorViewer : FISCA.Presentation.Controls.BaseForm
    {
        public ErrorViewer()
        {
            InitializeComponent();
        }

        public void SetHeader(string header)
        {
            chEntity.HeaderText = header;
        }

        public void SetMessage(StudentRecord stu, List<string> messages)
        {
            bool first = true;
            foreach (string msg in messages)
            {
                string s = "";
                if (first)
                {
                    if (stu.Class != null)
                    {
                        s += stu.Class.Name;
                        if (stu.SeatNo != "")
                            s += "(" + stu.SeatNo + "號)";
                        s += " ";
                    }
                    if (stu.StudentNumber != "")
                        s += stu.StudentNumber + " ";
                    if (s == "")
                        s += "學生：";
                    s += stu.Name;
                    dgv.Rows.Add(s, msg);
                    first = false;
                }
                else
                    dgv.Rows.Add("", msg);
            }
            toolStripStatusLabel1.Text = "總計" + dgv.Rows.Count + "個錯誤。";
        }

        public void SetMessage(string entity, List<string> messages)
        {
            bool first = true;
            foreach (string msg in messages)
            {
                if (first)
                {
                    dgv.Rows.Add(entity, msg);
                    first = false;
                }
                else
                    dgv.Rows.Add("", msg);
            }
            toolStripStatusLabel1.Text = "總計" + dgv.Rows.Count + "個錯誤。";
        }

        public void Clear()
        {
            this.Hide();
            this.dgv.Rows.Clear();
            toolStripStatusLabel1.Text = "";
        }

        private void ErrorViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count <= 0) return;

            SaveFileDialog sd = new SaveFileDialog();
            sd.FileName = "匯出訊息";
            sd.Filter = "Excel檔案(*.xls)|*.xls";
            if (sd.ShowDialog() != DialogResult.OK) return;

            Workbook book = new Workbook();
            Worksheet sheet = book.Worksheets[0];
            sheet.Name = "錯誤訊息";

            int colIndex = 0;
            sheet.Cells[0, colIndex++].PutValue(chEntity.HeaderText);
            sheet.Cells[0, colIndex++].PutValue("錯誤訊息");

            int rowIndex = 1;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                sheet.Cells[rowIndex, 0].PutValue("" + row.Cells[chEntity.Index].Value);
                sheet.Cells[rowIndex, 1].PutValue("" + row.Cells[chError.Index].Value);
                rowIndex++;
            }

            sheet.AutoFitColumns();

            try
            {
                book.Save(sd.FileName, FileFormatType.Excel2003);

                if (MsgBox.Show("匯出完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(sd.FileName);
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show("匯出失敗。" + ex.Message);
            }
        }
    }
}