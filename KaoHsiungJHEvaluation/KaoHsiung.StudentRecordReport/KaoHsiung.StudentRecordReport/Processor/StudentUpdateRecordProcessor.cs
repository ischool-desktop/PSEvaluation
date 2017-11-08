using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;

using JHSchool.Data;

namespace KaoHsiung.StudentRecordReport.Processor
{
    class StudentUpdateRecordProcessor
    {
        private DocumentBuilder _builder;
        private Cell _cell;

        public StudentUpdateRecordProcessor(DocumentBuilder builder)
        {
            _builder = builder;
            _builder.MoveToMergeField("學籍狀況");

            _cell = _builder.CurrentParagraph.ParentNode as Cell;
        }

        public void SetData(List<JHUpdateRecordRecord> records)
        {
            records.Sort(delegate(JHUpdateRecordRecord x, JHUpdateRecordRecord y)
            {
                return DateTime.Parse(x.UpdateDate).CompareTo(DateTime.Parse(y.UpdateDate));

                //DateTime? xd, yd;
                //xd = Framework.DateTimeHelper.Parse(x.UpdateDate);
                //yd = Framework.DateTimeHelper.Parse(y.UpdateDate);

                //if (xd.HasValue && yd.HasValue)
                //    return xd.Value.CompareTo(yd.Value);
                //else if (yd.HasValue)
                //    return -1;
                //else if (xd.HasValue)
                //    return 1;
                //else
                //    return x.UpdateDate.CompareTo(y.UpdateDate);
            });

            Cell cell = _cell;            

            // 處理異動資料超過7筆，自動新增Row
            int AddRowCount = records.Count - 7;
            if (AddRowCount > 0)
            {
                Row rrRow = _cell.ParentNode as Row;
                for (int i = 1; i <= AddRowCount; i++)
                {
                    Row newRow = (Row)rrRow.Clone(true);
                    rrRow.ParentTable.InsertAfter(newRow, rrRow);
                }
            }

            // 填入資料
            foreach (var record in records)
            {            
                WriteUpdateRecord(cell, record);                
                cell = WordHelper.GetMoveDownCell(cell, 1);
                if (cell == null) break;
            }
        }

        private void WriteUpdateRecord(Cell cell, JHUpdateRecordRecord record)
        {
            WordHelper.Write(cell, GetUpdateType(record), _builder);

            cell = cell.NextSibling as Cell;
            if (record.SchoolYear.HasValue)
                WordHelper.Write(cell, "" + record.SchoolYear.Value, _builder);
            else
            { 
                DateTime dt;
                if(DateTime.TryParse(record.UpdateDate, out dt))
                    WordHelper.Write(cell, "" + (dt.Year-1911), _builder);            
            }

            cell = cell.NextSibling as Cell;
            if (record.Semester.HasValue)
                WordHelper.Write(cell, "" + record.Semester.Value, _builder);
            else
            {
                DateTime dt;
                if (DateTime.TryParse(record.UpdateDate, out dt))
                {
                    if(dt.Month >8 && dt.Month <3)
                        WordHelper.Write(cell, "1" , _builder);
                    else
                        WordHelper.Write(cell, "2", _builder);
                }
            }

            cell = cell.NextSibling as Cell;
            // 當新生異動或畢業異動使用自己學校
            if (record.UpdateCode == "1" ||record.UpdateCode =="2")
                WordHelper.Write(cell,JHSchoolInfo.ChineseName, _builder);
            else
                WordHelper.Write(cell, record.ImportExportSchool, _builder);

            cell = cell.NextSibling as Cell;
            WordHelper.Write(cell, record.UpdateDate, _builder);

            cell = cell.NextSibling as Cell;
            WordHelper.Write(cell, record.ADDate, _builder);

            cell = cell.NextSibling as Cell;
            WordHelper.Write(cell, record.ADNumber, _builder);
        }

        private string GetUpdateType(JHUpdateRecordRecord node)
        {
            string updateType = "";

            if (node.UpdateCode == "1")
                updateType = "新生";
            else if (node.UpdateCode == "2")
                updateType = "畢(修)業";
            else if (node.UpdateCode == "3")
                updateType = "轉入";
            else if (node.UpdateCode == "4")
                updateType = "轉出";
            else if (node.UpdateCode == "5")
                updateType = "休學";
            else if (node.UpdateCode == "6")
                updateType = "復學";
            else if (node.UpdateCode == "7")
                updateType = "中輟";
            else if (node.UpdateCode == "8")
                updateType = "續讀";
            else if (node.UpdateCode == "9")
                updateType = "更正學籍";
            else if (node.UpdateCode == "11")
                updateType = "死亡";
            
            return updateType;
        }
    }
}
