using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using System.Xml;
using JHSchool.Data;
using K12.Data.Configuration;
using JHSchool.Behavior.BusinessLogic;

namespace HsinChu.StudentRecordReport
{
    class StudentMoralProcessor
    {
        //private const int TotalRow = 48;
        private const int TotalRow = 45;
        private const int FirstRowIndex = 2;

        private Cell _cell;
        private Cell _current;
        private Run _run;
        private SemesterMap _map = new SemesterMap();
        private Dictionary<string, List<string>> _types;

        public Cell GetCurrentCell()
        {
            return _current;
        }

        public StudentMoralProcessor(DocumentBuilder builder, SemesterMap map, List<JHPeriodMappingInfo> periodList, List<JHAbsenceMappingInfo> absenceList)
        {
            _cell = builder.CurrentParagraph.ParentNode as Cell;
            _current = _cell;

            _run = new Run(_cell.Document);
            _run.Font.Name = builder.Font.Name;
            _run.Font.Size = builder.Font.Size;
            _run.Text = string.Empty;

            _map = map;
            _types = new Dictionary<string, List<string>>();

            #region 取得假別設定
            ConfigData cd = K12.Data.School.Configuration["學籍表"];
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
            ProcessAttendance(autoSummaryList);
            ProcessMerit(autoSummaryList);
            ProcessDemerit(autoSummaryList);
            ProcessMoralScore(autoSummaryList);
        }

        private void ProcessAttendance(List<AutoSummaryRecord> autoSummaryList)
        {
            string postfix = "節數";

            Dictionary<string, Dictionary<string, ValueItemRow>> items = new Dictionary<string, Dictionary<string, ValueItemRow>>();

            foreach (string type in _types.Keys)
            {
                if (!items.ContainsKey(type))
                    items.Add(type, new Dictionary<string, ValueItemRow>());
                foreach (string name in _types[type])
                {
                    if (!items[type].ContainsKey(name + postfix))
                        items[type].Add(name + postfix, new ValueItemRow(name + postfix));
                }
            }

            foreach (AutoSummaryRecord record in autoSummaryList)
            {
                SemesterData semester = new SemesterData("" + record.SchoolYear, "" + record.Semester);

                foreach (AbsenceCountRecord absence in record.AbsenceCounts)
                {
                    if (items.ContainsKey(absence.PeriodType) && items[absence.PeriodType].ContainsKey(absence.Name + postfix))
                        items[absence.PeriodType][absence.Name + postfix].Add(semester, absence.Count);
                }
            }

            Cell cell = _current;

            foreach (string type in items.Keys)
            {
                int count = items[type].Count;
                MergeVerticalCell(cell, count);

                Write(cell, type);

                Cell itemCell = GetMoveRightCell(cell, 1);
                foreach (ValueItemRow row in items[type].Values)
                {
                    WriteValueItemRow(itemCell, row);
                    itemCell = GetMoveDownCell(itemCell, 1);
                    if (itemCell == null) break;
                }

                Cell tempCell = GetMoveDownCell(cell, count);
                if (tempCell == null) break;
                cell = tempCell;
            }

            _current = cell;
        }

        private void ProcessMerit(List<AutoSummaryRecord> autoSummaryList)
        {
            string postfix = "次數";

            Dictionary<string, ValueItemRow> items = new Dictionary<string, ValueItemRow>();

            foreach (string name in new string[] { "嘉獎", "小功", "大功" })
                items.Add(name + postfix, new ValueItemRow(name + postfix));

            foreach (AutoSummaryRecord record in autoSummaryList)
            {
                SemesterData semester = new SemesterData("" + record.SchoolYear, "" + record.Semester);

                items["大功" + postfix].Add(semester, record.MeritA);
                items["小功" + postfix].Add(semester, record.MeritB);
                items["嘉獎" + postfix].Add(semester, record.MeritC);
            }

            Cell cell = _current;

            MergeVerticalCell(cell, 3);
            Write(cell, "獎勵");

            Cell itemCell = GetMoveRightCell(cell, 1);
            foreach (string type in items.Keys)
            {
                WriteValueItemRow(itemCell, items[type]);
                itemCell = GetMoveDownCell(itemCell, 1);
                if (itemCell == null) break;
            }

            Cell tempCell = GetMoveDownCell(cell, 3);
            if (tempCell != null) cell = tempCell;
            _current = cell;
        }

        private void ProcessDemerit(List<AutoSummaryRecord> autoSummaryList)
        {
            string postfix = "次數";

            Dictionary<string, ValueItemRow> items = new Dictionary<string, ValueItemRow>();

            foreach (string name in new string[] { "警告", "小過", "大過" })
                items.Add(name + postfix, new ValueItemRow(name + postfix));

            foreach (var record in autoSummaryList)
            {
                SemesterData semester = new SemesterData("" + record.SchoolYear, "" + record.Semester);

                items["大過" + postfix].Add(semester, record.DemeritA);
                items["小過" + postfix].Add(semester, record.DemeritB);
                items["警告" + postfix].Add(semester, record.DemeritC);
            }

            Cell cell = _current;

            MergeVerticalCell(cell, 3);
            Write(cell, "懲戒");

            Cell itemCell = GetMoveRightCell(cell, 1);
            foreach (string type in items.Keys)
            {
                WriteValueItemRow(itemCell, items[type]);
                itemCell = GetMoveDownCell(itemCell, 1);
                if (itemCell == null) break;
            }

            Cell tempCell = GetMoveDownCell(cell, 3);
            if (tempCell != null) cell = tempCell;
            _current = cell;
        }

        private void ProcessMoralScore(List<AutoSummaryRecord> autoSummaryList)
        {
            Dictionary<string, MoralItemRow> items = new Dictionary<string, MoralItemRow>();
            string recommendName = string.Empty, OtherRecommendName=string.Empty;

            // 綜合評語使用
            MoralItemRow recommendRow = new MoralItemRow(new string[] { });
            // 其他評語使用
            MoralItemRow OtherrecommendRow = new MoralItemRow(new string[] { });

            foreach (AutoSummaryRecord asRecord in autoSummaryList)
            {
                if (asRecord.MoralScore == null) continue;
                JHMoralScoreRecord record = asRecord.MoralScore;
                SemesterData semester = new SemesterData("" + record.SchoolYear, "" + record.Semester);

                XmlElement dailyBehavior = (XmlElement)record.TextScore.SelectSingleNode("DailyBehavior");
                if (dailyBehavior != null)
                {
                    foreach (XmlElement item in dailyBehavior.SelectNodes("Item"))
                    {
                        string name = item.GetAttribute("Name");
                        string index = item.GetAttribute("Index");
                        string degree = item.GetAttribute("Degree");

                        string[] lines = index.Split(new string[] { "；", ";" }, StringSplitOptions.RemoveEmptyEntries);

                        if (!items.ContainsKey(name))
                            items.Add(name, new MoralItemRow(lines));

                        items[name].Add(semester, degree);
                    }
                }

                // 綜和評語
                XmlElement dailyLifeRecommend = (XmlElement)record.TextScore.SelectSingleNode("DailyLifeRecommend");
                // 使用系統設定
                recommendName = GetDLString("綜合評語");
                if (dailyLifeRecommend != null)
                {
                    if (string.IsNullOrEmpty(recommendName)) recommendName = dailyLifeRecommend.GetAttribute("Name");
                    recommendRow.Add(semester, dailyLifeRecommend.GetAttribute("Description"));
                }
               


                // 其他
                XmlElement OtherRecommend = (XmlElement)record.TextScore.SelectSingleNode("OtherRecommend");
                // 使用系統設定
                OtherRecommendName = GetDLString("其它表現");
                if (OtherRecommend != null)
                {
                    if (string.IsNullOrEmpty(OtherRecommendName)) OtherRecommendName = OtherRecommend.GetAttribute("Name");
                    OtherrecommendRow.Add(semester, OtherRecommend.GetAttribute("Description"));
                }                
            }

            Cell cell = _current;
            int totalCount = 0;

            foreach (string key in items.Keys)
            {
                Write(cell, key);
                int lineCount = items[key].LineCount;

                if (lineCount > 0)
                {
                    MergeVerticalCell(cell, lineCount);

                    Cell otherCell = GetMoveRightCell(cell, 1);
                    MergeVerticalCell(otherCell, lineCount);
                    for (int i = 0; i < 6; i++)
                    {
                        otherCell = GetMoveRightCell(otherCell, 1);
                        if (otherCell == null) break;
                        MergeVerticalCell(otherCell, lineCount);
                    }
                }
                else
                    lineCount = 1;

                totalCount += lineCount;

                Cell indexCell = GetMoveRightCell(cell, 1);
                WriteMoralItemRow(indexCell, items[key]);

                cell = GetMoveDownCell(cell, lineCount);
            }

            cell.CellFormat.HorizontalMerge = CellMerge.First;
            GetMoveRightCell(cell, 1).CellFormat.HorizontalMerge = CellMerge.Previous;
            
            // 綜合評語
            Write(cell, recommendName);
            
            if (cell != null)
            {
                Row row = cell.ParentRow;
                int shift = row.IndexOf(cell) + 2;

                foreach (SemesterData sems in recommendRow.Items.Keys)
                {
                    string text = recommendRow.Items[sems];
                    int index = -1;
                    if (_map.SemesterMapping.ContainsKey(sems))
                        index = _map.SemesterMapping[sems];

                    if (index < 0) continue;

                    index = index + shift;
                    Write(row.Cells[index], text);
                }
            }
            // 往下走一格
            cell = GetMoveDownCell(cell, 1);

            int current_row_index = _current.ParentRow.ParentTable.IndexOf(_current.ParentRow);
            current_row_index += totalCount;
            int count = TotalRow + FirstRowIndex - current_row_index;

            Cell tempCell = cell;
            //MergeVerticalCell(tempCell, count);
            //tempCell = GetMoveRightCell(tempCell, 1);
            //MergeVerticalCell(tempCell, count);
            //for (int i = 0; i < 6; i++)
            //{
            //    tempCell = GetMoveRightCell(tempCell, 1);
            //    MergeVerticalCell(tempCell, count);
            //}

            tempCell = cell;
            for (int i = 0; i < count; i++)
            {
                tempCell.CellFormat.HorizontalMerge = CellMerge.First;
                GetMoveRightCell(tempCell, 1).CellFormat.HorizontalMerge = CellMerge.Previous;
                tempCell = GetMoveDownCell(tempCell, 1);
                if (tempCell == null) break;
            }

            // 其他
            Write(cell, OtherRecommendName);

            if (cell != null)
            {
                Row row = cell.ParentRow;
                int shift = row.IndexOf(cell) + 2;

                foreach (SemesterData sems in OtherrecommendRow.Items.Keys)
                {
                    string text = OtherrecommendRow.Items[sems];
                    int index = -1;
                    if (_map.SemesterMapping.ContainsKey(sems))
                        index = _map.SemesterMapping[sems];

                    if (index < 0) continue;

                    index = index + shift;
                    Write(row.Cells[index], text);
                }
            }

            //int current_row_index = _current.ParentRow.ParentTable.IndexOf(_current.ParentRow);
            //current_row_index += totalCount;
            //int count = TotalRow + FirstRowIndex - current_row_index;

            //Cell tempCell = cell;
            //MergeVerticalCell(tempCell, count);
            //tempCell = GetMoveRightCell(tempCell, 1);
            //MergeVerticalCell(tempCell, count);
            //for (int i = 0; i < 6; i++)
            //{
            //    tempCell = GetMoveRightCell(tempCell, 1);
            //    MergeVerticalCell(tempCell, count);
            //}

            //tempCell = cell;
            //for (int i = 0; i < count; i++)
            //{
            //    tempCell.CellFormat.HorizontalMerge = CellMerge.First;
            //    GetMoveRightCell(tempCell, 1).CellFormat.HorizontalMerge = CellMerge.Previous;
            //    tempCell = GetMoveDownCell(tempCell, 1);
            //    if (tempCell == null) break;
            //}

          

            _current = cell;
        }

        /// <summary>
        /// 取得日常生活表現名稱
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetDLString(string name)
        {
            string retVal = name;
            if (Global.DLBehaviorConfigNameDict.ContainsKey(name))
                retVal = Global.DLBehaviorConfigNameDict[name];

            return retVal;
        }

        private void MergeVerticalCell(Cell cell, int count)
        {
            try
            {
                count--;
                cell.CellFormat.VerticalMerge = CellMerge.First;

                for (int i = 0; i < count; i++)
                {
                    cell = GetMoveDownCell(cell, 1);
                    cell.CellFormat.VerticalMerge = CellMerge.Previous;
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private Cell GetMoveDownCell(Cell cell, int count)
        {
            Row row = cell.ParentRow;
            int col_index = row.IndexOf(cell);
            Table table = row.ParentTable;
            int row_index = table.Rows.IndexOf(row) + count;

            try
            {
                return table.Rows[row_index].Cells[col_index];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private Cell GetMoveRightCell(Cell cell, int count)
        {
            try
            {
            Row row = cell.ParentRow;
            int col_index = row.IndexOf(cell);
            Table table = row.ParentTable;
            int row_index = table.Rows.IndexOf(row);


                return table.Rows[row_index].Cells[col_index + count];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void WriteValueItemRow(Cell cell, ValueItemRow itemRow)
        {
            Write(cell, itemRow.Name);
            Row row = cell.ParentRow;
            int shift = row.IndexOf(cell) + 1;

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

        private void WriteMoralItemRow(Cell cell, MoralItemRow itemRow)
        {
            WriteLines(cell, itemRow.Lines);
            Row row = cell.ParentRow;
            int shift = row.IndexOf(cell) + 1;

            foreach (SemesterData sems in itemRow.Items.Keys)
            {
                string text = itemRow.Items[sems];
                int index = -1;
                if (_map.SemesterMapping.ContainsKey(sems))
                    index = _map.SemesterMapping[sems];

                if (index < 0) continue;

                index = index + shift;
                Write(row.Cells[index], text);
            }
        }

        private void WriteLines(Cell cell, string[] texts)
        {
            cell.Paragraphs.Clear();
            foreach (string text in texts)
            {
                cell.Paragraphs.Add(new Paragraph(cell.Document));
                _run.Text = text;
                cell.LastParagraph.Runs.Add(_run.Clone(true));
            }
        }

        private void Write(Cell cell, string text)
        {
            try
            {
                if (cell.FirstParagraph == null)
                    cell.Paragraphs.Add(new Paragraph(cell.Document));
                cell.FirstParagraph.Runs.Clear();
                _run.Text = text;
                cell.FirstParagraph.Runs.Add(_run.Clone(true));
            }
            catch (Exception ex)
            { 
                
            }
        }
    }

    class ValueItemRow
    {
        public string Name { get; private set; }
        private Dictionary<SemesterData, int> _items;
        public Dictionary<SemesterData, int> Items
        {
            get { return _items; }
        }

        public ValueItemRow(string name)
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

    class MoralItemRow
    {
        public int LineCount { get; private set; }
        private string[] _lines;
        public string[] Lines { get { return _lines; } }
        private Dictionary<SemesterData, string> _items;
        public Dictionary<SemesterData, string> Items
        {
            get { return _items; }
        }

        public MoralItemRow(string[] lines)
        {
            _lines = lines;
            LineCount = lines.Length;
            _items = new Dictionary<SemesterData, string>();
        }

        public bool Add(SemesterData semester, string text)
        {
            if (!_items.ContainsKey(semester))
            {
                _items.Add(semester, text);
                return true;
            }
            else
                return false;
        }
    }
}
