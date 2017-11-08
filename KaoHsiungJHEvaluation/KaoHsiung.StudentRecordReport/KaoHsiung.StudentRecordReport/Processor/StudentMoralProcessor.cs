using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using System.Xml;
using JHSchool.Data;
using K12.Data.Configuration;
using JHSchool.Behavior.BusinessLogic;
using KaoHsiung.StudentRecordReport.Association.UDT;

namespace KaoHsiung.StudentRecordReport.Processor
{
    class StudentMoralProcessor
    {
        private SemesterMap _map = new SemesterMap();
        private Dictionary<string, List<string>> _types;
        private DocumentBuilder _builder;
        private Run _run;
        private Dictionary<string, List<AssnScore>> _assnScoreCache;

        private static List<AssnCode> AssociationUDTCache { get; set; }

        /// <summary>
        /// 清除社團快取資料。
        /// </summary>
        public static void ResetCache()
        {
            AssociationUDTCache = null;
        }

        public StudentMoralProcessor(DocumentBuilder builder, SemesterMap map)
        {
            _builder = builder;

            _map = map;
            _types = new Dictionary<string, List<string>>();

            _run = WordHelper.CreateRun(_builder);

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

            #region 取得社團成績
            _assnScoreCache = new Dictionary<string, List<AssnScore>>();

            FISCA.UDT.AccessHelper ah = new FISCA.UDT.AccessHelper();
//            string condition = string.Format("SchoolYear='{0}' and Semester='{1}'", options.SchoolYear, options.Semester);
//            List<AssnCode> list = ah.Select<AssnCode>(condition);

            if (AssociationUDTCache == null)
                AssociationUDTCache = ah.Select<AssnCode>();

            List<AssnCode> list = AssociationUDTCache;
            foreach (AssnCode record in list)
            {
                //if (!_assnScoreCache.ContainsKey(record.StudentID))
                //{
                    XmlElement scores = K12.Data.XmlHelper.LoadXml(record.Scores);
                    XmlElement itemElement = (XmlElement)scores.SelectSingleNode("Item");
                    if (itemElement != null)
                    {
                        AssnScore assnScore = new AssnScore()
                        {
                            Score = itemElement.GetAttribute("Score"),
                            Effort = itemElement.GetAttribute("Effort"),
                            Text = itemElement.GetAttribute("Text"),
                            SchoolYear = record.SchoolYear,
                            Semester = record.Semester
                        };
                        if (_assnScoreCache.ContainsKey(record.StudentID))
                            _assnScoreCache[record.StudentID].Add(assnScore);
                        else
                        {
                            List<AssnScore> lis = new List<AssnScore>();
                            lis.Add(assnScore);
                            _assnScoreCache.Add(record.StudentID, lis);
                        }
                    }
                //}
            }

            //<Content>
            //<Item AssociationName="籃球社" Score="" Effort="" Text=""></Item>
            //</Content>
            #endregion
        }

        public void SetData(List<AutoSummaryRecord> autoSummaryList)
        {
            ProcessAttendance(autoSummaryList);
            ProcessDiscipline(autoSummaryList);
            ProcessMoralScore(autoSummaryList);
        }

        private void ProcessAttendance(List<AutoSummaryRecord> autoSummaryList)
        {
            #region 定位，產生Row
            _builder.MoveToMergeField("缺曠");
            Cell currentCell = _builder.CurrentParagraph.ParentNode as Cell;
            Row currentRow = currentCell.ParentRow;
            Table table = currentRow.ParentTable;

            if (_types.Count <= 0)
            {
                currentRow.Remove();
                return;
            }
            else
            {
                int rowCount = 0;
                foreach (string type in _types.Keys)
                    rowCount += _types[type].Count;

                int insertIndex = table.Rows.IndexOf(currentRow) + 1;
                for (int i = 1; i < rowCount; i++)
                    table.Rows.Insert(insertIndex, currentRow.Clone(true));
            }
            #endregion

            string postfix = "節數";

            #region 寫入缺曠資料
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

            Cell cell = currentCell;

            foreach (string type in items.Keys)
            {
                int count = items[type].Count;
                WordHelper.MergeVerticalCell(cell, count);

                WordHelper.Write(cell, type, _builder);

                Cell itemCell = WordHelper.GetMoveRightCell(cell, 1);
                foreach (ValueItemRow row in items[type].Values)
                {
                    WriteValueItemRow(itemCell, row);
                    itemCell = WordHelper.GetMoveDownCell(itemCell, 1);
                    if (itemCell == null) break;
                }

                Cell tempCell = WordHelper.GetMoveDownCell(cell, count);
                if (tempCell == null) break;
                cell = tempCell;
            }
            #endregion
        }

        private void ProcessDiscipline(List<AutoSummaryRecord> autoSummaryList)
        {
            #region 定位，產生Row
            _builder.MoveToMergeField("獎懲");
            Cell currentCell = _builder.CurrentParagraph.ParentNode as Cell;
            Row currentRow = currentCell.ParentRow;
            Table table = currentRow.ParentTable;

            int rowCount = 6;
            int insertIndex = table.Rows.IndexOf(currentRow) + 1;
            for (int i = 1; i < rowCount; i++)
                table.Rows.Insert(insertIndex, currentRow.Clone(true));
            #endregion

            string postfix = "次數";

            #region 寫入獎勵資料
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

            Cell cell = currentCell;

            WordHelper.MergeVerticalCell(cell, 3);
            WordHelper.Write(cell, "獎勵", _builder);

            Cell itemCell = WordHelper.GetMoveRightCell(cell, 1);
            foreach (string type in items.Keys)
            {
                WriteValueItemRow(itemCell, items[type]);
                itemCell = WordHelper.GetMoveDownCell(itemCell, 1);
                if (itemCell == null) break;
            }

            Cell tempCell = WordHelper.GetMoveDownCell(cell, 3);
            if (tempCell != null) cell = tempCell;
            currentCell = cell;
            #endregion

            #region 寫入懲戒資料
            items.Clear();

            foreach (string name in new string[] { "警告", "小過", "大過" })
                items.Add(name + postfix, new ValueItemRow(name + postfix));

            foreach (var record in autoSummaryList)
            {
                SemesterData semester = new SemesterData("" + record.SchoolYear, "" + record.Semester);

                items["大過" + postfix].Add(semester, record.DemeritA);
                items["小過" + postfix].Add(semester, record.DemeritB);
                items["警告" + postfix].Add(semester, record.DemeritC);
            }

            cell = currentCell;

            WordHelper.MergeVerticalCell(cell, 3);
            WordHelper.Write(cell, "懲戒", _builder);

            itemCell = WordHelper.GetMoveRightCell(cell, 1);
            foreach (string type in items.Keys)
            {
                WriteValueItemRow(itemCell, items[type]);
                itemCell = WordHelper.GetMoveDownCell(itemCell, 1);
                if (itemCell == null) break;
            }

            tempCell = WordHelper.GetMoveDownCell(cell, 3);
            if (tempCell != null) cell = tempCell;
            #endregion
        }

        private void ProcessMoralScore(List<AutoSummaryRecord> autoSummaryList)
        {
            Dictionary<string, MoralItemRow> items = new Dictionary<string, MoralItemRow>();

            //團體活動
            MoralItemRow groupActivityRow = null;

            //公共服務
            MoralItemRow publicServiceRow = null;

            //校內外特殊表現
            MoralItemRow schoolSpecialRow = null;
            //具體建議
            MoralItemRow recommendRow = null;

            #region 建立適合列印的資料結構
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

                XmlElement dailyLifeRecommend = (XmlElement)record.TextScore.SelectSingleNode("DailyLifeRecommend");
                if (dailyLifeRecommend != null)
                {
                    string GroupName = "";
                    if (Utility.MorItemDict.ContainsKey("DailyLifeRecommend"))
                        GroupName = Utility.MorItemDict["DailyLifeRecommend"];

                    if (recommendRow == null)
                        recommendRow = new MoralItemRow(new string[] { GroupName });
                    recommendRow.Add(semester, dailyLifeRecommend.GetAttribute("Description"));
                }

                // 團體活動
                XmlElement groupActivity = (XmlElement)record.TextScore.SelectSingleNode("GroupActivity");
                if (groupActivity != null)
                {
                    string GroupName = "";
                    if (Utility.MorItemDict.ContainsKey("GroupActivity"))
                        GroupName = Utility.MorItemDict["GroupActivity"];

                    if (groupActivityRow == null)
                        groupActivityRow = new MoralItemRow(new string[] { GroupName });

                    string text = string.Empty;
                    
                    foreach (XmlElement item in groupActivity.SelectNodes("Item"))
                    {
                        string degree = item.GetAttribute("Degree");
                        string desc = item.GetAttribute("Description");
                        string name = item.GetAttribute("Name");

                        if (string.IsNullOrEmpty(degree) && string.IsNullOrEmpty(desc)) continue;

                        string line = name + ":" + (string.IsNullOrEmpty(degree) ? "" : degree + "/") + (string.IsNullOrEmpty(desc) ? "" : desc);
                        if (line.EndsWith("/")) line = line.Substring(0, line.Length - 1);
                        text += line;
                    }
                    
                    // 加入社團
                    if (_assnScoreCache.ContainsKey(record.RefStudentID))
                    {
                        string sc = record.SchoolYear.ToString();
                        string ss = record.Semester.ToString();
                        foreach (AssnScore asc in _assnScoreCache[record.RefStudentID])
                        {
                            if (asc.SchoolYear == sc && asc.Semester == ss)
                                text += "社團活動:" + asc.Score + "," + asc.Effort +"/"+ asc.Text;
                        }
                    
                    }
                    groupActivityRow.Add(semester, text);
                }



                XmlElement publicService = (XmlElement)record.TextScore.SelectSingleNode("PublicService");
                if (publicService != null)
                {
                    string GroupName = "";
                    if (Utility.MorItemDict.ContainsKey("PublicService"))
                        GroupName = Utility.MorItemDict["PublicService"];

                    if (publicServiceRow == null)
                        publicServiceRow = new MoralItemRow(new string[] { GroupName });

                    string text = string.Empty;

                    foreach (XmlElement item in publicService.SelectNodes("Item"))
                    {
                        string degree = item.GetAttribute("Degree");
                        string desc = item.GetAttribute("Description");
                        string name = item.GetAttribute("Name");

                        if (string.IsNullOrEmpty(degree) && string.IsNullOrEmpty(desc)) continue;

                        string line = name + ":" + (string.IsNullOrEmpty(degree) ? "" : degree + "/") + (string.IsNullOrEmpty(desc) ? "" : desc);
                        if (line.EndsWith("/")) line = line.Substring(0, line.Length - 1);
                        text += line;
                    }

                    publicServiceRow.Add(semester, text);
                }

                XmlElement schoolSpecial = (XmlElement)record.TextScore.SelectSingleNode("SchoolSpecial");
                if (schoolSpecial != null)
                {
                    string GroupName = "";
                    if (Utility.MorItemDict.ContainsKey("SchoolSpecial"))
                        GroupName = Utility.MorItemDict["SchoolSpecial"];

                    if (schoolSpecialRow == null)
                        schoolSpecialRow = new MoralItemRow(new string[] { GroupName });

                    string text = string.Empty;

                    foreach (XmlElement item in schoolSpecial.SelectNodes("Item"))
                    {
                        string degree = item.GetAttribute("Degree");
                        string desc = item.GetAttribute("Description");
                        string name = item.GetAttribute("Name");

                        if (string.IsNullOrEmpty(degree) && string.IsNullOrEmpty(desc)) continue;

                        string line = name + ":" + (string.IsNullOrEmpty(degree) ? "" : degree + "/") + (string.IsNullOrEmpty(desc) ? "" : desc);
                        if (line.EndsWith("/")) line = line.Substring(0, line.Length - 1);
                        text += line;
                    }

                    schoolSpecialRow.Add(semester, text);
                }
            }
            #endregion

            #region 定位，產生Row
            _builder.MoveToMergeField("日常行為表現");
            Cell currentCell = _builder.CurrentParagraph.ParentNode as Cell;
            Row currentRow = currentCell.ParentRow;
            Table table = currentRow.ParentTable;

            int rowCount = items.Count;
            int insertIndex = table.Rows.IndexOf(currentRow) + 1;
            for (int i = 1; i < rowCount; i++)
                table.Rows.Insert(insertIndex, currentRow.Clone(true));
            #endregion

            #region 寫入日常行為表現
            Cell cell = currentCell;

            foreach (string key in items.Keys)
            {
                WordHelper.Write(cell, key, _builder);

                Cell indexCell = WordHelper.GetMoveRightCell(cell, 1);
                WriteMoralItemRow(indexCell, items[key]);

                cell = WordHelper.GetMoveDownCell(cell, 1);
            }
            #endregion

            #region 寫入團體活動表現
            _builder.MoveToMergeField("團體活動");
            if (groupActivityRow != null && groupActivityRow.Items.Count > 0)
            {
                currentCell = _builder.CurrentParagraph.ParentNode as Cell;
                WriteMoralItemRow(currentCell, groupActivityRow);
                if (currentCell.Paragraphs.Count > 0) currentCell.Paragraphs[0].ParagraphFormat.Alignment = ParagraphAlignment.Center;
            }
            else
                (_builder.CurrentParagraph.ParentNode as Cell).ParentRow.Remove();
            #endregion

            #region 寫入公共服務表現
            _builder.MoveToMergeField("公共服務");
            if (publicServiceRow != null && publicServiceRow.Items.Count > 0)
            {
                currentCell = _builder.CurrentParagraph.ParentNode as Cell;
                WriteMoralItemRow(currentCell, publicServiceRow);
                if (currentCell.Paragraphs.Count > 0) currentCell.Paragraphs[0].ParagraphFormat.Alignment = ParagraphAlignment.Center;
            }
            else
                (_builder.CurrentParagraph.ParentNode as Cell).ParentRow.Remove();
            #endregion

            #region 寫入校內外特殊表現
            _builder.MoveToMergeField("校內外特殊");
            if (schoolSpecialRow != null && schoolSpecialRow.Items.Count > 0)
            {
                currentCell = _builder.CurrentParagraph.ParentNode as Cell;
                WriteMoralItemRow(currentCell, schoolSpecialRow);
                if (currentCell.Paragraphs.Count > 0) currentCell.Paragraphs[0].ParagraphFormat.Alignment = ParagraphAlignment.Center;
            }
            else
                (_builder.CurrentParagraph.ParentNode as Cell).ParentRow.Remove();
            #endregion

            #region 寫入具體建議
            _builder.MoveToMergeField("具體建議");
            if (recommendRow != null && recommendRow.Items.Count > 0)
            {
                currentCell = _builder.CurrentParagraph.ParentNode as Cell;
                WriteMoralItemRow(currentCell, recommendRow);
                if (currentCell.Paragraphs.Count > 0) currentCell.Paragraphs[0].ParagraphFormat.Alignment = ParagraphAlignment.Center;
            }
            else
                (_builder.CurrentParagraph.ParentNode as Cell).ParentRow.Remove();
            #endregion
        }

        private void WriteValueItemRow(Cell cell, ValueItemRow itemRow)
        {
            WordHelper.Write(cell, itemRow.Name, _builder);
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
                WordHelper.Write(row.Cells[index], (count == 0 ? "" : "" + count), _builder);
            }
        }

        private void WriteMoralItemRow(Cell cell, MoralItemRow itemRow)
        {
            WriteLines(cell, itemRow.Lines, _builder);
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
                WordHelper.Write(row.Cells[index], text, _builder);
            }
        }

        private void WriteLines(Cell cell, string[] texts, DocumentBuilder builder)
        {
            cell.Paragraphs.Clear();
            Run run = new Run(builder.Document);
            run.Font.Name = builder.Font.Name;
            run.Font.Size = builder.Font.Size;

            foreach (string text in texts)
            {
                cell.Paragraphs.Add(new Paragraph(cell.Document));
                run.Text = text;
                cell.LastParagraph.Runs.Add(run.Clone(true));
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
