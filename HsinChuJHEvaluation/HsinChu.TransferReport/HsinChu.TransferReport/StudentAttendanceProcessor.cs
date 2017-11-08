using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Words;
using JHSchool.Data;
using K12.Data.Configuration;
using System.Xml;
using JHSchool.Behavior.BusinessLogic;

namespace HsinChu.TransferReport
{
    class StudentAttendanceProcessor
    {
        private Cell _cell;
        private SemesterMap _map;
        private Run _run;

        private List<AutoSummaryRecord> _list;
        private Dictionary<string, List<string>> _types;

        public StudentAttendanceProcessor(DocumentBuilder builder, SemesterMap map)
        {
            builder.MoveToMergeField("缺曠");
            _cell = builder.CurrentParagraph.ParentNode as Cell;
            _map = map;

            _run = new Run(_cell.Document);
            _run.Font.Name = builder.Font.Name;
            _run.Font.Size = builder.Font.Size;
            _run.Text = string.Empty;

            _types = new Dictionary<string, List<string>>();

            #region 取得假別設定
            ConfigData cd = K12.Data.School.Configuration["轉學成績證明書"];
            if (cd.Contains("假別設定"))
            {
                XmlElement config = Framework.XmlHelper.LoadXml(cd["假別設定"]);

                foreach (XmlElement type in config.SelectNodes("Type"))
                {
                    string typeName = type.GetAttribute("Text");
                    if (!_types.ContainsKey(typeName))
                        _types.Add(typeName, new List<string>());

                    foreach (XmlElement absence in type.SelectNodes("Absence"))
                    {
                        string absenceName = absence.GetAttribute("Text");
                        if (!_types[typeName].Contains(absenceName))
                            _types[typeName].Add(absenceName);
                    }
                }
            }
            #endregion
        }

        public void SetData(List<AutoSummaryRecord> autoSummaryList)
        {
            _list = autoSummaryList;

            ProcessAttendance();
        }

        private void ProcessAttendance()
        {
            Dictionary<string, AttendanceItemRow> items = new Dictionary<string, AttendanceItemRow>();

            foreach (AutoSummaryRecord record in _list)
            {
                SemesterData semester = new SemesterData("" + record.SchoolYear, "" + record.Semester);

                foreach (AbsenceCountRecord absence in record.AbsenceCounts)
                {
                    if (!items.ContainsKey(absence.PeriodType + ":" + absence.Name))
                        items.Add(absence.PeriodType + ":" + absence.Name, new AttendanceItemRow(absence.PeriodType, absence.Name));
                    items[absence.PeriodType + ":" + absence.Name].Add(semester, absence.Count);
                }
            }

            Row row = _cell.ParentRow;
            Table table = row.ParentTable;
            int rowIndex = table.IndexOf(row);

            if (_types.Count <= 0)
            {
                row.Remove();
                return;
            }

            int typeCount = 0;
            foreach (string type in _types.Keys)
            {
                int nameCount = 0;
                foreach (string name in _types[type])
                {
                    nameCount++;
                    typeCount++;
                    table.Rows.Insert(++rowIndex, row.Clone(true));
                    Row tempRow = table.Rows[rowIndex];
                    Write(tempRow.Cells[2], name);
                    if (items.ContainsKey(type + ":" + name))
                        WriteAttendanceItemRow(tempRow.Cells[3], items[type + ":" + name]);

                }
                Cell nameCell = table.Rows[rowIndex - nameCount + 1].Cells[1];
                WordHelper.MergeVerticalCell(nameCell, nameCount);
                Write(nameCell, type);
            }
            Cell typeCell = table.Rows[rowIndex - typeCount + 1].Cells[0];
            WordHelper.MergeVerticalCell(typeCell, typeCount);
            Write(typeCell, "缺曠資料");

            row.Remove();
        }

        private void WriteAttendanceItemRow(Cell cell, AttendanceItemRow itemRow)
        {
            Row row = cell.ParentRow;
            int shift = row.IndexOf(cell);

            foreach (SemesterData sems in itemRow.Items.Keys)
            {
                int count = itemRow.Items[sems];
                int index = -1;
                if (_map.SemesterMapping.ContainsKey(sems))
                    index = _map.SemesterMapping[sems];

                if (index < 0) continue;

                index = index + shift;
                Write(row.Cells[index], (count == 0 ? "" : "" + count));
            }
        }



        private void Write(Cell cell, string text)
        {
            if (cell != null)
            {
                if (cell.FirstParagraph == null)
                    cell.Paragraphs.Add(new Paragraph(cell.Document));
                cell.FirstParagraph.Runs.Clear();
                _run.Text = text;
                cell.FirstParagraph.Runs.Add(_run.Clone(true));
            }
        }
    }

    class AttendanceItemRow
    {
        public string Type { get; private set; }
        public string Name { get; private set; }
        private Dictionary<SemesterData, int> _items;
        public Dictionary<SemesterData, int> Items
        {
            get { return _items; }
        }

        public AttendanceItemRow(string type, string name)
        {
            Type = type;
            Name = name;
            _items = new Dictionary<SemesterData, int>();
        }

        public bool Add(SemesterData semester, int count)
        {
            if (!_items.ContainsKey(semester))
                _items.Add(semester, 0);
            _items[semester] += count;

            return true;
        }
    }
}
