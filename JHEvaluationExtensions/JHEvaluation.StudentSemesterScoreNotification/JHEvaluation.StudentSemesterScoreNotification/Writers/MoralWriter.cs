using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using Aspose.Words;
using System.Xml;
using Campus.Report;
using JHSchool.Behavior.BusinessLogic;
using Aspose.Words.Tables;

namespace JHEvaluation.StudentSemesterScoreNotification.Writers
{
    internal class MoralWriter
    {
        private Dictionary<string, List<string>> _types;
        private ReportConfiguration _rc;

        public AutoSummaryRecord AutoSummaryRecord { get; set; }

        public MoralWriter()
        {
            _types = new Dictionary<string, List<string>>();
            _rc = new ReportConfiguration(Global.ReportName);

            if (_rc.Contains("假別設定"))
            {
                XmlElement config = K12.Data.XmlHelper.LoadXml(_rc.GetString("假別設定", "<假別設定/>"));
                foreach (XmlElement type in config.SelectNodes("Type"))
                {
                    string typeName = type.GetAttribute("Text");
                    foreach (XmlElement absence in type.SelectNodes("Absence"))
                    {
                        string absenceName = absence.GetAttribute("Text");
                        if (!_types.ContainsKey(typeName))
                            _types.Add(typeName, new List<string>());
                        _types[typeName].Add(absenceName);
                    }
                }
            }
        }

        public void Writer(Document doc)
        {
            DocumentBuilder builder = new DocumentBuilder(doc);

            if (AutoSummaryRecord == null) AutoSummaryRecord = new AutoSummaryRecord();
            XmlElement summary = (AutoSummaryRecord != null && AutoSummaryRecord.MoralScore != null) ? AutoSummaryRecord.MoralScore.Summary : K12.Data.XmlHelper.LoadXml("<Summary/>");
            XmlElement textScore = (AutoSummaryRecord != null && AutoSummaryRecord.MoralScore != null) ? AutoSummaryRecord.MoralScore.TextScore : K12.Data.XmlHelper.LoadXml("<TextScore/>");

            double width;
            double miniUnitWitdh;
            Table table;
            Font font;
            #region 日常生活表現評量

            #region 處理日常生活表現評量的名稱

            /* 日常生活表現評量的名稱key:
                日常行為表現
                其它表現
                日常生活表現具體建議
                團體活動表現
                公共服務表現
                校內外特殊表現
             */

            if (builder.MoveToMergeField("日常行為表現名稱"))
                builder.Write(GetDLString("日常行為表現"));
                
            //if ((XmlElement)textScore.SelectSingleNode("DailyBehavior") == null)
            //    ProcessItemNameIsNull(builder, "日常行為表現名稱", "日常行為表現");
            //else
            //{
            //    XmlElement xm = (XmlElement)textScore.SelectSingleNode("DailyBehavior");
            //    if (xm.GetAttribute("Name") == "")
            //        ProcessItemNameIsNull(builder, "日常行為表現名稱", "日常行為表現");
            //    else
            //        ProcessItemName(builder, "日常行為表現名稱", xm);
            //}


            if (Global.Params["Mode"] == "KaoHsiung")
            {
                #region 高雄                
                if (builder.MoveToMergeField("團體活動表現名稱"))
                    builder.Write(GetDLString("團體活動表現"));
                //if ((XmlElement)textScore.SelectSingleNode("GroupActivity") == null)
                //    ProcessItemNameIsNull(builder, "團體活動表現名稱", "團體活動表現");
                //else
                //{
                //    XmlElement xm =(XmlElement)textScore.SelectSingleNode("GroupActivity");
                //    if(xm.GetAttribute("Name")=="")
                //        ProcessItemNameIsNull(builder, "團體活動表現名稱", "團體活動表現");
                //    else
                //        ProcessItemName(builder, "團體活動表現名稱", xm);
                //}

                if (builder.MoveToMergeField("公共服務表現名稱"))
                    builder.Write(GetDLString("公共服務表現"));
                //if ((XmlElement)textScore.SelectSingleNode("PublicService") == null)
                //    ProcessItemNameIsNull(builder, "公共服務表現名稱", "公共服務表現");
                //else
                //{
                //    XmlElement xm = (XmlElement)textScore.SelectSingleNode("PublicService");
                //    if (xm.GetAttribute("Name") == "")
                //        ProcessItemNameIsNull(builder, "公共服務表現名稱", "公共服務表現");
                //    else
                //        ProcessItemName(builder, "公共服務表現名稱",xm);
                //}
                if (builder.MoveToMergeField("校內外特殊表現名稱"))
                    builder.Write(GetDLString("校內外特殊表現"));
                //if ((XmlElement)textScore.SelectSingleNode("SchoolSpecial") == null)
                //    ProcessItemNameIsNull(builder, "校內外特殊表現名稱", "校內外特殊表現");
                //else
                //{
                //    XmlElement xm = (XmlElement)textScore.SelectSingleNode("SchoolSpecial");
                //    if (xm.GetAttribute("Name") == "")
                //        ProcessItemNameIsNull(builder, "校內外特殊表現名稱", "校內外特殊表現");
                //    else
                //        ProcessItemName(builder, "校內外特殊表現名稱", xm);
                //}

                if (builder.MoveToMergeField("日常生活表現具體建議名稱"))
                    builder.Write(GetDLString("日常生活表現具體建議"));
                //if ((XmlElement)textScore.SelectSingleNode("DailyLifeRecommend") == null)
                //    ProcessItemNameIsNull(builder, "日常生活表現具體建議名稱", "日常生活表現具體建議");
                //else
                //{
                //    XmlElement xm = (XmlElement)textScore.SelectSingleNode("DailyLifeRecommend");
                //    if (xm.GetAttribute("Name") == "")
                //        ProcessItemNameIsNull(builder, "日常生活表現具體建議名稱", "日常生活表現具體建議");
                //    else
                //        ProcessItemName(builder, "日常生活表現具體建議名稱", xm);
                //}
                
                #endregion
            }
            else
            {
                #region 新竹
                if (builder.MoveToMergeField("其他表現名稱"))
                    builder.Write(GetDLString("其它表現"));
                //if ((XmlElement)textScore.SelectSingleNode("OtherRecommend") == null)
                //    ProcessItemNameIsNull(builder, "其他表現名稱", "其他表現");
                //else
                //    ProcessItemName(builder, "其他表現名稱", (XmlElement)textScore.SelectSingleNode("OtherRecommend"));

                if (builder.MoveToMergeField("綜合評語名稱"))
                    builder.Write(GetDLString("綜合評語"));
                //if ((XmlElement)textScore.SelectSingleNode("DailyLifeRecommend") == null)
                //    ProcessItemNameIsNull(builder, "綜合評語名稱", "綜合評語");
                //else
                //    ProcessItemName(builder, "綜合評語名稱", (XmlElement)textScore.SelectSingleNode("DailyLifeRecommend"));
                #endregion
            }
            #endregion

            #region 處理日常生活表現評量的內容
            if (builder.MoveToMergeField("日常行為"))
            {
                font = builder.Font;
                Cell dailyBehaviorCell = builder.CurrentParagraph.ParentNode as Cell;
                if (Global.DLBehaviorConfigItemNameDict.ContainsKey("日常行為表現"))
                {
                    foreach (string itemName in Global.DLBehaviorConfigItemNameDict["日常行為表現"])
                    {
                        WordHelper.Write(dailyBehaviorCell, font,itemName);
                        bool hasDegree = false;
                        foreach (XmlElement item in textScore.SelectNodes("DailyBehavior/Item"))
                        {
                            if (itemName == item.GetAttribute("Name"))
                            {
                                if (dailyBehaviorCell.NextSibling == null) break;
                                WordHelper.Write(dailyBehaviorCell.NextSibling as Cell, font, item.GetAttribute("Degree"));
                                hasDegree = true;
                            }
                        }
                        if (hasDegree == false)
                            WordHelper.Write(dailyBehaviorCell.NextSibling as Cell, font, "");

                        dailyBehaviorCell = WordHelper.GetMoveDownCell(dailyBehaviorCell, 1);
                    }
                }
            }

            if (Global.Params["Mode"] == "KaoHsiung")
            {
                #region 高雄
                if (builder.MoveToMergeField("團體活動"))
                {
                    font = builder.Font;
                    Cell groupActivityCell = builder.CurrentParagraph.ParentNode as Cell;

                    Paragraph para = (Paragraph)groupActivityCell.GetChild(NodeType.Paragraph, 0, true);
                    font = para.ParagraphBreakFont;
                    groupActivityCell.Paragraphs.RemoveAt(0);
                    if (Global.DLBehaviorConfigItemNameDict.ContainsKey("團體活動表現"))
                    {
                        foreach (string itemName in Global.DLBehaviorConfigItemNameDict["團體活動表現"])
                        {
                            groupActivityCell.Paragraphs.Add(new Paragraph(doc));
                            Run run1 = new Run(doc);
                            run1.Font.Name = font.Name;
                            run1.Font.Size = font.Size;
                            run1.Font.Bold = true;
                            run1.Text = itemName + "：";
                            groupActivityCell.LastParagraph.Runs.Add(run1);

                            Run run2 = new Run(doc);
                            run2.Font.Name = font.Name;
                            run2.Font.Size = font.Size;

                            Run run3h = new Run(doc);
                            Run run3 = new Run(doc);

                            foreach (XmlElement item in textScore.SelectNodes("GroupActivity/Item"))
                            {
                                if (itemName == item.GetAttribute("Name"))
                                {
                                    // 是否有文字描述
                                    bool hasText = false;
                                    if (!string.IsNullOrEmpty(item.GetAttribute("Description")))
                                        hasText = true;

                                    if (string.IsNullOrEmpty(item.GetAttribute("Degree")))
                                        run2.Text = item.GetAttribute("Degree");
                                    else
                                        run2.Text = item.GetAttribute("Degree") + "。";
                                    if (hasText)
                                    {
                                        run3h.Font.Name = font.Name;
                                        run3h.Font.Size = font.Size;
                                        run3h.Font.Bold = true;
                                        run3h.Text = item.GetAttribute("Name") + "：";
                                        run3.Font.Name = font.Name;
                                        run3.Font.Size = font.Size;
                                        run3.Text = item.GetAttribute("Description") + "。";
                                    }
                                    else
                                    {
                                        run3h.Font.Name = font.Name;
                                        run3h.Font.Size = font.Size;
                                        run3h.Text = "";
                                        run3.Font.Name = font.Name;
                                        run3.Font.Size = font.Size;
                                        run3.Text = "";

                                    }

                                }
                            }
                            groupActivityCell.LastParagraph.Runs.Add(run2);
                            groupActivityCell.LastParagraph.Runs.Add(run3h);
                            groupActivityCell.LastParagraph.Runs.Add(run3);
                        }
                    }
                }

                if (builder.MoveToMergeField("公共服務"))
                {
                    font = builder.Font;
                    Cell publicServiceCell = builder.CurrentParagraph.ParentNode as Cell;
                    Paragraph para = (Paragraph)publicServiceCell.GetChild(NodeType.Paragraph, 0, true);
                    font = para.ParagraphBreakFont;
                    publicServiceCell.Paragraphs.Clear();

                    if (Global.DLBehaviorConfigItemNameDict.ContainsKey("公共服務表現"))
                    {
                        foreach (string itemName in Global.DLBehaviorConfigItemNameDict["公共服務表現"])
                        {
                            publicServiceCell.Paragraphs.Add(new Paragraph(doc));
                            Run run1 = new Run(doc);
                            run1.Font.Name = font.Name;
                            run1.Font.Size = font.Size;
                            run1.Font.Bold = true;
                            run1.Text = itemName + "：";
                            bool hasDescription = false;
                            publicServiceCell.LastParagraph.Runs.Add(run1);

                            foreach (XmlElement item in textScore.SelectNodes("PublicService/Item"))
                            {
                                if (itemName == item.GetAttribute("Name"))
                                {
                                    Run run2 = new Run(doc);
                                    run2.Font.Name = font.Name;
                                    run2.Font.Size = font.Size;
                                    run2.Text = item.GetAttribute("Description");
                                    publicServiceCell.LastParagraph.Runs.Add(run2);
                                    hasDescription = true;
                                }
                            }

                            if (hasDescription == false)
                            {
                                Run run2 = new Run(doc);
                                run2.Font.Name = font.Name;
                                run2.Font.Size = font.Size;
                                run2.Text = "";
                                publicServiceCell.LastParagraph.Runs.Add(run2);

                            }
                        }
                    }
                }

                if (builder.MoveToMergeField("校內外特殊"))
                {
                    font = builder.Font;
                    Cell schoolSpecialCell = builder.CurrentParagraph.ParentNode as Cell;
                    Paragraph para = (Paragraph)schoolSpecialCell.GetChild(NodeType.Paragraph, 0, true);
                    font = para.ParagraphBreakFont;
                    schoolSpecialCell.Paragraphs.Clear();

                    if (Global.DLBehaviorConfigItemNameDict.ContainsKey("校內外特殊表現"))
                    {
                        foreach (string itemName in Global.DLBehaviorConfigItemNameDict["校內外特殊表現"])
                        {
                            schoolSpecialCell.Paragraphs.Add(new Paragraph(doc));
                            Run run1 = new Run(doc);
                            run1.Font.Name = font.Name;
                            run1.Font.Size = font.Size;
                            run1.Font.Bold = true;
                            run1.Text = itemName + "：";
                            schoolSpecialCell.LastParagraph.Runs.Add(run1);
                            bool hasDescription = false;
                            foreach (XmlElement item in textScore.SelectNodes("SchoolSpecial/Item"))
                            {
                                if (itemName == item.GetAttribute("Name"))
                                {
                                    Run run2 = new Run(doc);
                                    run2.Font.Name = font.Name;
                                    run2.Font.Size = font.Size;
                                    run2.Text = item.GetAttribute("Description");
                                    schoolSpecialCell.LastParagraph.Runs.Add(run2);
                                    hasDescription = true;
                                }
                            }

                            if (hasDescription == false)
                            {
                                Run run2 = new Run(doc);
                                run2.Font.Name = font.Name;
                                run2.Font.Size = font.Size;
                                run2.Text = "";
                                schoolSpecialCell.LastParagraph.Runs.Add(run2);
                            }
                        }
                    }
                }

                if (builder.MoveToMergeField("具體建議"))
                {
                    font = builder.Font;
                    Cell dailyLifeRecommendCell = builder.CurrentParagraph.ParentNode as Cell;
                    XmlElement dailyLifeRecommend = (XmlElement)textScore.SelectSingleNode("DailyLifeRecommend");
                    string dailyLifeRecommendValue = string.Empty;
                    if (dailyLifeRecommend != null)
                        dailyLifeRecommendValue = dailyLifeRecommend.GetAttribute("Description");

                    Paragraph para = (Paragraph)dailyLifeRecommendCell.GetChild(NodeType.Paragraph, 0, true);
                    font = para.ParagraphBreakFont;
                    dailyLifeRecommendCell.Paragraphs.Clear();
                    dailyLifeRecommendCell.Paragraphs.Add(new Paragraph(doc));
                    Run run = new Run(doc);
                    run.Font.Name = font.Name;
                    run.Font.Size = font.Size;
                    run.Text = dailyLifeRecommendValue;

                    dailyLifeRecommendCell.LastParagraph.Runs.Add(run);
                }
                #endregion
            }
            else
            {
                #region 新竹
                if (builder.MoveToMergeField("其他表現"))
                {
                    font = builder.Font;
                    Cell otherRecommendCell = builder.CurrentParagraph.ParentNode as Cell;
                    XmlElement otherRecommend = (XmlElement)textScore.SelectSingleNode("OtherRecommend");
                    string otherRecommendValue = string.Empty;
                    if (otherRecommend != null)
                        otherRecommendValue = otherRecommend.GetAttribute("Description");

                    Paragraph para = (Paragraph)otherRecommendCell.GetChild(NodeType.Paragraph, 0, true);
                    font = para.ParagraphBreakFont;
                    otherRecommendCell.Paragraphs.Clear();
                    otherRecommendCell.Paragraphs.Add(new Paragraph(doc));
                    Run otherRecommendRun = new Run(doc);
                    otherRecommendRun.Font.Name = font.Name;
                    otherRecommendRun.Font.Size = font.Size;
                    otherRecommendRun.Text = otherRecommendValue;
                    otherRecommendCell.LastParagraph.Runs.Add(otherRecommendRun);
                }

                if (builder.MoveToMergeField("綜合評語"))
                {
                    font = builder.Font;
                    Cell dailyLifeRecommendCell = builder.CurrentParagraph.ParentNode as Cell;
                    XmlElement dailyLifeRecommend = (XmlElement)textScore.SelectSingleNode("DailyLifeRecommend");
                    string dailyLifeRecommendValue = string.Empty;
                    if (dailyLifeRecommend != null)
                        dailyLifeRecommendValue = dailyLifeRecommend.GetAttribute("Description");

                    Paragraph para = (Paragraph)dailyLifeRecommendCell.GetChild(NodeType.Paragraph, 0, true);
                    font = para.ParagraphBreakFont;
                    dailyLifeRecommendCell.Paragraphs.Clear();
                    dailyLifeRecommendCell.Paragraphs.Add(new Paragraph(doc));
                    Run dailyLifeRecommendRun = new Run(doc);
                    dailyLifeRecommendRun.Font.Name = font.Name;
                    dailyLifeRecommendRun.Font.Size = font.Size;
                    dailyLifeRecommendRun.Text = dailyLifeRecommendValue;
                    dailyLifeRecommendCell.LastParagraph.Runs.Add(dailyLifeRecommendRun);
                }
                #endregion
            }
            #endregion

            #region TextScore XML格式參考
            //<DailyBehavior Name="日常行為表現">
            //    <Item Degree="大部份符合" Index="抽屜乾淨" Name="愛整潔"/>
            //    <Item Degree="尚再努力" Index="懂得向老師,學長敬禮" Name="有禮貌"/>
            //    <Item Degree="大部份符合" Index="自習時間能夠安靜自習" Name="守秩序"/>
            //    <Item Degree="尚再努力" Index="打掃時間,徹底整理自己打掃範圍" Name="責任心"/>
            //    <Item Degree="需再努力" Index="不亂丟垃圾" Name="公德心"/>
            //    <Item Degree="大部份符合" Index="懂得關心同學朋友" Name="友愛關懷"/>
            //    <Item Degree="大部份符合" Index="團體活動能夠遵守相關規定" Name="團隊合作"/>
            //</DailyBehavior>
            //<GroupActivity Name="團體活動表現">
            //    <Item Degree="表現良好" Description="社團" Name="社團活動"/>
            //    <Item Degree="表現優異" Description="學校" Name="學校活動"/>
            //    <Item Degree="有待改進" Description="自治" Name="自治活動"/>
            //    <Item Degree="需在加油" Description="班級" Name="班級活動"/>
            //</GroupActivity>
            //<PublicService Name="公共服務表現">
            //    <Item Description="校內" Name="校內服務"/>
            //    <Item Description="社區" Name="社區服務"/>
            //</PublicService>
            //<SchoolSpecial Name="校內外特殊表現">
            //    <Item Description="這麼特殊" Name="校外特殊表現"/>
            //    <Item Description="又是校內" Name="校內特殊表現"/>
            //</SchoolSpecial>
            //<DailyLifeRecommend Description="我錯了" Name="日常生活表現具體建議"/>
            #endregion

            #endregion

            #region 缺曠
            //<AttendanceStatistics>
            //    <Absence Count="4" Name="公假" PeriodType="一般"/>
            //    <Absence Count="4" Name="曠課" PeriodType="一般"/>
            //    <Absence Count="4" Name="凹假" PeriodType="一般"/>
            //</AttendanceStatistics>

            builder.MoveToMergeField("缺曠");

            if (_types.Count > 0)
            {
                Dictionary<string, string> attendance = new Dictionary<string, string>();
                foreach (AbsenceCountRecord absence in AutoSummaryRecord.AbsenceCounts)
                {
                    string key = GetKey(absence.PeriodType, absence.Name);
                    if (!attendance.ContainsKey(key))
                        attendance.Add(key, "" + absence.Count);
                }

                double total = 0;
                foreach (List<string> list in _types.Values)
                    total += list.Count;

                Cell attendanceCell = builder.CurrentParagraph.ParentNode as Cell;
                width = attendanceCell.CellFormat.Width;
                miniUnitWitdh = width / total;

                table = builder.StartTable();
                builder.RowFormat.HeightRule = HeightRule.Exactly;
                builder.RowFormat.Height = 17.5;

                foreach (string type in _types.Keys)
                {
                    builder.InsertCell().CellFormat.Width = miniUnitWitdh * _types[type].Count;
                    builder.Write(type);
                }
                builder.EndRow();

                foreach (string type in _types.Keys)
                {
                    foreach (string item in _types[type])
                    {
                        builder.InsertCell().CellFormat.Width = miniUnitWitdh;
                        builder.Write(item);
                    }
                }
                builder.EndRow();

                foreach (string type in _types.Keys)
                {
                    foreach (string item in _types[type])
                    {
                        builder.InsertCell().CellFormat.Width = miniUnitWitdh;
                        string key = GetKey(type, item);
                        string value = attendance.ContainsKey(key) ? attendance[key] : "0";
                        builder.Write(value);
                    }
                }
                builder.EndRow();
                builder.EndTable();

                //去除表格四邊的線
                foreach (Cell c in table.FirstRow.Cells)
                    c.CellFormat.Borders.Top.LineStyle = LineStyle.None;

                foreach (Cell c in table.LastRow.Cells)
                    c.CellFormat.Borders.Bottom.LineStyle = LineStyle.None;

                foreach (Row r in table.Rows)
                {
                    r.FirstCell.CellFormat.Borders.Left.LineStyle = LineStyle.None;
                    r.LastCell.CellFormat.Borders.Right.LineStyle = LineStyle.None;
                }
            }
            #endregion

            #region 獎懲
            //<DisciplineStatistics>
            //    <Merit A="1" B="0" C="0"/>
            //    <Demerit A="12" B="12" C="14"/>
            //</DisciplineStatistics>

            Dictionary<string, string> discipline = new Dictionary<string, string>();
            discipline.Add("大功", "" + AutoSummaryRecord.MeritA);
            discipline.Add("小功", "" + AutoSummaryRecord.MeritB);
            discipline.Add("嘉獎", "" + AutoSummaryRecord.MeritC);
            discipline.Add("大過", "" + AutoSummaryRecord.DemeritA);
            discipline.Add("小過", "" + AutoSummaryRecord.DemeritB);
            discipline.Add("警告", "" + AutoSummaryRecord.DemeritC);

            builder.MoveToMergeField("獎懲");

            Cell disciplineCell = builder.CurrentParagraph.ParentNode as Cell;
            width = disciplineCell.CellFormat.Width;
            miniUnitWitdh = width / 6f;

            table = builder.StartTable();
            builder.RowFormat.HeightRule = HeightRule.Exactly;
            builder.RowFormat.Height = 17.5;

            foreach (string key in discipline.Keys)
            {
                builder.InsertCell().CellFormat.Width = miniUnitWitdh;
                builder.Write(key);
            }
            builder.EndRow();

            foreach (string key in discipline.Keys)
            {
                builder.InsertCell().CellFormat.Width = miniUnitWitdh;
                string value = string.IsNullOrEmpty(discipline[key]) ? "0" : discipline[key];
                builder.Write(value);
            }
            builder.EndRow();
            builder.EndTable();

            //去除表格四邊的線
            foreach (Cell c in table.FirstRow.Cells)
                c.CellFormat.Borders.Top.LineStyle = LineStyle.None;

            foreach (Cell c in table.LastRow.Cells)
                c.CellFormat.Borders.Bottom.LineStyle = LineStyle.None;

            foreach (Row r in table.Rows)
            {
                r.FirstCell.CellFormat.Borders.Left.LineStyle = LineStyle.None;
                r.LastCell.CellFormat.Borders.Right.LineStyle = LineStyle.None;
            }
            #endregion
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

        private void ProcessItemName(DocumentBuilder builder, string mergeFieldName, XmlElement element)
        {
            builder.MoveToMergeField(mergeFieldName);
            if (element != null)
                WordHelper.Write(builder.CurrentParagraph.ParentNode as Cell, builder.Font, element.GetAttribute("Name"));
        }

        // 當Null可以給預設值用
        private void ProcessItemNameIsNull(DocumentBuilder builder, string mergeFieldName, string str)
        {
            builder.MoveToMergeField(mergeFieldName);
                WordHelper.Write(builder.CurrentParagraph.ParentNode as Cell, builder.Font, str);
        }


        private string GetKey(string type, string name)
        {
            return type + "_" + name;
        }

        private string ParseCount(XmlNode xmlNode)
        {
            if (xmlNode != null) return xmlNode.InnerText;
            else return string.Empty;
        }
    }
}
