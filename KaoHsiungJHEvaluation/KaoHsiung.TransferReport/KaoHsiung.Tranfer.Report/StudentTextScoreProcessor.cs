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
    class StudentTextScoreProcessor
    {
        private Cell _cell;
        private SemesterMap _map;
        private Run _run;
        private DocumentBuilder _builder;

        private List<string> _sortOrder;

        private List<AutoSummaryRecord> _list;

        public StudentTextScoreProcessor(DocumentBuilder builder, SemesterMap map)
        {
            _builder = builder;

            _map = map;

            _run = new Run(builder.Document);
            _run.Font.Name = builder.Font.Name;
            _run.Font.Size = builder.Font.Size;
            _run.Text = string.Empty;

            _sortOrder = new List<string>(new string[] { "愛整潔", "有禮貌", "守秩序", "責任心", "公德心", "友愛關懷", "團隊合作" });
        }

        public void SetData(List<AutoSummaryRecord> autoSummaryList)
        {
            _list = autoSummaryList;

            ProcessDailyBehavior();
            ProcessOtherRecommend();
            ProcessDailyLifeRecommend();
        }

        private void ProcessDailyBehavior()
        {
            _builder.MoveToMergeField("日常行為");
            _cell = _builder.CurrentParagraph.ParentNode as Cell;

            Dictionary<string, TextScoreItemRow> items = new Dictionary<string, TextScoreItemRow>();

            foreach (AutoSummaryRecord asRecord in _list)
            {
                if (asRecord.MoralScore == null) continue;
                JHMoralScoreRecord record = asRecord.MoralScore;
                SemesterData semester = new SemesterData("" + record.SchoolYear, "" + record.Semester);

                XmlElement textscore = record.TextScore;
                if (textscore != null)
                {
                    foreach (string name in _sortOrder)
                    {
                        XmlElement item = (XmlElement)textscore.SelectSingleNode("DailyBehavior/Item[@Name='" + name + "']");
                        if (item != null)
                        {
                            if (!items.ContainsKey(name))
                                items.Add(name, new TextScoreItemRow(name));
                            items[name].Add(semester, item.GetAttribute("Degree"));
                        }
                    }
                }
            }

            Cell itemCell = _cell;

            foreach (string name in _sortOrder)
            {
                if (items.ContainsKey(name))
                    WriteTextScoreItemRow(itemCell, items[name]);
                itemCell = WordHelper.GetMoveDownCell(itemCell, 1);
                if (itemCell == null) break;
            }
        }

        //<DailyBehavior Name="日常行為表現">
        //    <Item Degree="部份符合" Index="抽屜乾淨" Name="愛整潔" />
        //    <Item Degree="部份符合" Index="懂得向老師,學長敬禮" Name="有禮貌" />
        //    <Item Degree="部份符合" Index="自習時間能夠安靜自習" Name="守秩序" />
        //    <Item Degree="部份符合" Index="打掃時間,徹底整理自己打掃範圍" Name="責任心" />
        //    <Item Degree="尚再努力" Index="不亂丟垃圾" Name="公德心" />
        //    <Item Degree="大部份符合" Index="懂得關心同學朋友" Name="友愛關懷" />
        //    <Item Degree="大部份符合" Index="團體活動能夠遵守相關規定" Name="團隊合作" />
        //</DailyBehavior>
        //<DailyLifeRecommend Description="本學期已進步許多,生活細節都符合標準" Name="日常生活表現具體建議" />
        //<OtherRecommend Description="在校外照顧學弟妹,搭校車也能夠準時到站." Name="其他表現" />

        private void ProcessOtherRecommend()
        {
            _builder.MoveToMergeField("其他表現");
            _cell = _builder.CurrentParagraph.ParentNode as Cell;

            TextScoreItemRow itemRow = new TextScoreItemRow("");

            foreach (AutoSummaryRecord asRecord in _list)
            {
                if (asRecord.MoralScore == null) continue;
                JHMoralScoreRecord record = asRecord.MoralScore;
                SemesterData semester = new SemesterData("" + record.SchoolYear, "" + record.Semester);

                XmlElement textscore = record.TextScore;
                if (textscore != null)
                {
                    XmlElement item = (XmlElement)textscore.SelectSingleNode("OtherRecommend");
                    if (item != null)
                        itemRow.Add(semester, item.GetAttribute("Description"));
                }
            }

            WriteTextScoreItemRow(_cell, itemRow);
        }

        private void ProcessDailyLifeRecommend()
        {
            _builder.MoveToMergeField("綜合評語");
            _cell = _builder.CurrentParagraph.ParentNode as Cell;

            TextScoreItemRow itemRow = new TextScoreItemRow("");

            foreach (AutoSummaryRecord asRecord in _list)
            {
                if (asRecord.MoralScore == null) continue;
                JHMoralScoreRecord record = asRecord.MoralScore;
                SemesterData semester = new SemesterData("" + record.SchoolYear, "" + record.Semester);

                XmlElement textscore = record.TextScore;
                if (textscore != null)
                {
                    XmlElement item = (XmlElement)textscore.SelectSingleNode("DailyLifeRecommend");
                    if (item != null)
                        itemRow.Add(semester, item.GetAttribute("Description"));
                }
            }

            WriteTextScoreItemRow(_cell, itemRow);
        }

        private void WriteTextScoreItemRow(Cell cell, TextScoreItemRow itemRow)
        {
            //Write(cell, itemRow.Name);
            Row row = cell.ParentRow;
            int shift = row.IndexOf(cell);

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

    class TextScoreItemRow
    {
        public string Name { get; private set; }
        private Dictionary<SemesterData, string> _items;
        public Dictionary<SemesterData, string> Items
        {
            get { return _items; }
        }

        public TextScoreItemRow(string name)
        {
            Name = name;
            _items = new Dictionary<SemesterData, string>();
        }

        public bool Add(SemesterData semester, string text)
        {
            if (!_items.ContainsKey(semester))
                _items.Add(semester, string.Empty);
            _items[semester] = text;

            return true;
        }
    }
}
