using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Words;
using JHSchool.Data;
using System.Xml;
using JHSchool.Behavior.BusinessLogic;

namespace KaoHsiung.TransferReport
{
    class StudentDisciplineProcessor
    {
        private Cell _cell;
        private SemesterMap _map;
        private Run _run;

        private List<string> _sortOrder;

        private List<AutoSummaryRecord> _list;

        public StudentDisciplineProcessor(DocumentBuilder builder, SemesterMap map)
        {
            builder.MoveToMergeField("獎懲");
            _cell = builder.CurrentParagraph.ParentNode as Cell;
            _map = map;

            _run = new Run(_cell.Document);
            _run.Font.Name = builder.Font.Name;
            _run.Font.Size = builder.Font.Size;
            _run.Text = string.Empty;

            _sortOrder = new List<string>(new string[] { "大功", "小功", "嘉獎", "大過", "小過", "警告" });
        }

        public void SetData(List<AutoSummaryRecord> autoSummaryList)
        {
            _list = autoSummaryList;

            ProcessDiscipline();
        }

        private void ProcessDiscipline()
        {
            Dictionary<string, DisciplineItemRow> items = new Dictionary<string, DisciplineItemRow>();

            foreach (string name in _sortOrder)
                items.Add(name, new DisciplineItemRow(name));

            foreach (var record in _list)
            {
                SemesterData semester = new SemesterData("" + record.SchoolYear, "" + record.Semester);

                items["大功"].Add(semester, record.MeritA);
                items["小功"].Add(semester, record.MeritB);
                items["嘉獎"].Add(semester, record.MeritC);
                items["大過"].Add(semester, record.DemeritA);
                items["小過"].Add(semester, record.DemeritB);
                items["警告"].Add(semester, record.DemeritC);
            }

            Cell cell = _cell;
            Cell itemCell = _cell;

            foreach (string name in _sortOrder)
            {
                WriteDisciplineItemRow(itemCell, items[name]);
                itemCell = WordHelper.GetMoveDownCell(itemCell, 1);
                if (itemCell == null) break;
            }
        }

        private void WriteDisciplineItemRow(Cell cell, DisciplineItemRow itemRow)
        {
            //Write(cell, itemRow.Name);
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

    class DisciplineItemRow
    {
        public string Name { get; private set; }
        private Dictionary<SemesterData, int> _items;
        public Dictionary<SemesterData, int> Items
        {
            get { return _items; }
        }

        public DisciplineItemRow(string name)
        {
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
