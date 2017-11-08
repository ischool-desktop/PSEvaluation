using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;

using JHSchool.Data;

namespace HsinChu.StudentRecordReport
{
    class StudentUpdateRecordProcessor
    {
        private const int ColumnCount = 2;
        private const int RowCount = 5;
        private const int Shift = 3;
        private int _rowIndex = 0;
        private int _col = 0;
        private int _row = 0;
        private Table table;
        private Cell _cell;
        private Run _run;

        public StudentUpdateRecordProcessor(DocumentBuilder builder)
        {
            _cell = builder.CurrentParagraph.ParentNode as Cell;

            table = _cell.ParentRow.ParentTable;
            _rowIndex = table.IndexOf(_cell.ParentRow);

            _run = new Run(_cell.Document);
            _run.Font.Name = builder.Font.Name;
            _run.Font.Size = builder.Font.Size;
            _run.Text = string.Empty;
        }

        private bool MoveNext()
        {
            if (_row < RowCount - 1)
            {
                _row++;
                _cell = table.Rows[_rowIndex + _row].Cells[_col * Shift];
                return true;
            }
            else
            {
                _col++;
                _row = 0;
                if (_col >= ColumnCount)
                    return false;
                else
                {
                    _cell = table.Rows[_rowIndex + _row].Cells[_col * Shift];
                    return true;
                }
            }

        }

        public void SetData(List<JHUpdateRecordRecord> records)
        {
            records.Sort(delegate(JHUpdateRecordRecord a, JHUpdateRecordRecord b)
            {
                return DateTime.Parse(a.UpdateDate).CompareTo(DateTime.Parse(b.UpdateDate));
            });

            foreach (JHUpdateRecordRecord record in records)
            {
                string desc = GetDesc(record);
                Cell cell = _cell;
                Write(cell, Global.CDate(record.UpdateDate));
                Write((Cell)cell.NextSibling, desc);
                Write((Cell)cell.NextSibling.NextSibling, record.ADNumber);
                if (!MoveNext())
                    break;
            }
        }

        private string GetDesc(JHUpdateRecordRecord node)
        {
            string updateDesc = "";

            // 用異動代碼長度判斷國高中
            if (node.UpdateCode.Length > 2)
            {
                // 高中
                updateDesc = node.UpdateDescription;
            }
            else
            {
                // 國中
                if (node.UpdateCode == "1")
                    updateDesc = "新生";

                if (node.UpdateCode == "2")
                {
                    if (node.Graduate == "修業")
                        updateDesc = "修業";
                    else
                        updateDesc = "畢業";
                }
                if (node.UpdateCode == "3")
                    updateDesc = "轉入";

                if (node.UpdateCode == "4")
                    updateDesc = "轉出";

                if (node.UpdateCode == "5")
                    updateDesc = "休學";

                if (node.UpdateCode == "6")
                    updateDesc = "復學";

                if (node.UpdateCode == "7")
                    updateDesc = "中輟";

                if (node.UpdateCode == "8")
                    updateDesc = "續讀";

                if (node.UpdateCode == "9")
                    updateDesc = "更正學籍";

                if (node.UpdateCode == "10")
                    updateDesc = "延長修業年限";

                if (node.UpdateCode == "11")
                    updateDesc = "死亡";

                if (node.UpdateDescription.Length > 0)
                    updateDesc += ":" + node.UpdateDescription;
            }

            return updateDesc;
        }

        private void Write(Cell cell, string text)
        {
            if (cell.FirstParagraph == null)
                cell.Paragraphs.Add(new Paragraph(cell.Document));
            cell.FirstParagraph.Runs.Clear();
            _run.Text = text;
            cell.FirstParagraph.Runs.Add(_run.Clone(true));
        }
    }
}
