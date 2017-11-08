using System;
using System.Collections.Generic;
using System.Text;
using JHEvaluation.ScoreCalculation;
using Aspose.Words;
using System.IO;
using Aspose.Words.Reporting;
using JHEvaluation.ScoreCalculation.ScoreStruct;
using Campus.Rating;
using System.Xml;
using Subj = JHSchool.Evaluation.Subject;

namespace JHEvaluation.StudentScoreSummaryReport
{
    internal class Report
    {
        private ReportPreference PrintSetting { get; set; }

        private List<ReportStudent> Students { get; set; }

        private Dictionary<string, List<string>> PrintAbsences { get; set; }

        private List<string> DetailDomain { get; set; }

        private const string LearningDomainName = "學習領域";

        public Report(List<ReportStudent> students, ReportPreference printSetting)
        {
            Students = students;
            Students.Sort(delegate(ReportStudent x, ReportStudent y)
            {
                return x.OrderString.CompareTo(y.OrderString);
            });

            PrintSetting = printSetting;

            DetailDomain = new List<string>();

            //新竹的要把語文詳列。
            if (Program.Mode == ModuleMode.HsinChu)
                DetailDomain.Add("語文");

            DetailDomain.Add("彈性課程");
            DetailDomain.Add("");

            PrintAbsences = printSetting.AcceptAbsences.PeriodOptionsFromString();
        }

        public Document Print()
        {
            Document doc = PrintSetting.Template.ToDocument();

            doc.MailMerge.MergeField += new Aspose.Words.Reporting.MergeFieldEventHandler(MailMerge_MergeField);

            doc.MailMerge.Execute(new MergeDataSource(Students, PrintSetting));

            return doc;
        }

        //最好這程式有人能維護的了.......
        private void MailMerge_MergeField(object sender, MergeFieldEventArgs e)
        {
            //不是 Fix 開頭的合併欄位不處理。
            if (!e.FieldName.ToUpper().StartsWith("Fix".ToUpper())) return;

            DocumentBuilder builder = new DocumentBuilder(e.Document);

            ReportStudent student = e.FieldValue as ReportStudent;

            //如果合併值不是 ReportStudent 就跳過...意思是有問題...。
            if (student == null) return;

            builder.MoveToField(e.Field, true);
            e.Field.Remove();

            if (e.FieldName == "Fix:年級學期")
            {
                #region 列印年級學期資訊(有點複雜)。
                SemesterDataCollection semesters = student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters);
                Row SemesterRow = builder.CurrentParagraph.ParentNode.ParentNode.NextSibling as Row; //下一個 Row。
                Paragraph originParagraph = builder.CurrentParagraph;

                int count = 0, offset = 1;

                foreach (SemesterData each in semesters)
                {
                    string currentGradeyear = Util.GetGradeyearString(each.GradeYear.ToString());

                    //如果沒有年級，就跳過。
                    if (string.IsNullOrEmpty(currentGradeyear)) continue;

                    builder.Write(currentGradeyear + "年級");
                    Paragraph nextPh = Util.NextCell(builder.CurrentParagraph);
                    if (nextPh == null) break; //沒有下一個 Cell ，就不印資料了。
                    builder.MoveTo(nextPh);

                    Paragraph resetParagraph = builder.CurrentParagraph;
                    SemesterRow.Cells[count + offset].Write(builder, GetSemesterString(each));

                    SemesterData semester = new SemesterData(0, each.SchoolYear, each.Semester);
                    if (!student.HeaderList.ContainsKey(semester))
                        student.HeaderList.AddRaw(each, count); //不要懷疑，這是對的。

                    builder.MoveTo(resetParagraph);
                    count++;
                }

                builder.MoveTo(originParagraph);
                (originParagraph.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                Paragraph nextParagrap = originParagraph;
                string previousGradeyear = GetText(originParagraph);
                while ((nextParagrap = Util.NextCell(nextParagrap)) != null)
                {
                    if (GetText(nextParagrap) == previousGradeyear && !string.IsNullOrEmpty(previousGradeyear))
                    {
                        (nextParagrap.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;
                        (nextParagrap.ParentNode as Cell).Paragraphs[0].Runs.Clear();
                    }
                    else
                        (nextParagrap.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;

                    previousGradeyear = GetText(nextParagrap);
                }
                #endregion
            }
            else if (e.FieldName == "Fix:科目資訊")
            {
                #region 列印科目資料(爆炸複雜)

                Row template = builder.CurrentParagraph.ParentNode.ParentNode as Row;
                Table table = template.ParentNode as Table;

                if (PrintSetting.ListMethod == ListMethod.DomainOnly)
                {
                    #region 列印領域
                    UniqueSet<RowHeader> RowIndexs = new UniqueSet<RowHeader>();
                    //Environment.OSVersion.Platform
                    #region 列印 RowHeader
                    foreach (SemesterData semester in student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters))
                    {
                        SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);

                        //如果不包含該學期成績資料，就跳過。
                        if (!student.SemestersScore.Contains(sysems)) continue;

                        SemesterScore semsscore = student.SemestersScore[sysems];

                        //準備彈性課程的科目(要詳列出來)。
                        foreach (string strSubject in semsscore.Subject)
                        {
                            SemesterSubjectScore subject = semsscore.Subject[strSubject];

                            if (DetailDomain.Contains(subject.Domain))
                            {
                                RefineDomain(subject);

                                RowHeader header = new RowHeader(subject.Domain, strSubject);
                                header.IsDomain = false;

                                if (!RowIndexs.Contains(header))
                                    RowIndexs.Add(header);
                            }
                        }

                        //準備領域資料。
                        foreach (string strDomain in semsscore.Domain)
                        {
                            if (!Subj.Domains.Contains(strDomain)) continue;

                            SemesterDomainScore domain = semsscore.Domain[strDomain];

                            if (!DetailDomain.Contains(strDomain))
                            {
                                RowHeader header = new RowHeader(strDomain, string.Empty);
                                header.IsDomain = true;

                                if (!RowIndexs.Contains(header))
                                    RowIndexs.Add(header);
                            }
                        }
                    }

                    RowHeader lheader = new RowHeader(LearningDomainName, string.Empty);
                    lheader.IsDomain = true;
                    RowIndexs.Add(lheader);

                    List<RowHeader> sortedHeaders = SortHeader(RowIndexs.ToList());

                    //產生 Row。
                    List<RowHeader> indexHeaders = new List<RowHeader>();
                    Row current = template;
                    int rowIndex = 0;
                    foreach (RowHeader header in sortedHeaders)
                    {
                        RowHeader indexH = header;
                        indexH.Index = rowIndex++;
                        indexHeaders.Add(indexH);
                        bool hasGroup = !string.IsNullOrEmpty(Subj.GetDomainGroup(header.Domain));
                        string groupName = Subj.GetDomainGroup(header.Domain);

                        Row datarow = table.InsertBefore(template.Clone(true), current) as Row;

                        if (header.Domain == LearningDomainName)
                        {
                            string headerName = string.Empty;

                            if (PrintSetting.PrintRank)
                                headerName = "學習領域排名";

                            if (PrintSetting.PrintRankPercentage)
                                headerName = "學習領域百分比";

                            if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                                headerName = "學習領域排名/百分比";

                            if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                            {
                                Row lrow = table.InsertAfter(template.Clone(true), datarow) as Row;
                                lrow.Cells[0].Write(builder, headerName);
                                rowIndex++;
                            }
                        }

                        if (header.IsDomain)
                        {
                            if (hasGroup)
                            {
                                datarow.Cells[0].Write(builder, groupName);
                                datarow.Cells[1].Write(builder, header.Domain);
                            }
                            else
                                datarow.Cells[0].Write(builder, header.Domain);
                        }
                        else
                        {
                            //把空白的領域當成「彈性課程」。
                            string domain = IsFlexible(header.Domain) ? "彈性課程" : header.Domain;
                            datarow.Cells[0].Write(builder, domain);
                            datarow.Cells[1].Write(builder, header.Subject);
                        }
                    }
                    #endregion

                    #region 填資料
                    Row RatingRow = null;
                    foreach (RowHeader header in indexHeaders)
                    {
                        SemesterDataCollection semesters = new SemesterDataCollection();
                        Row row = table.Rows[header.Index + 3];
                        foreach (SemesterData semester in student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters))
                        {
                            SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);
                            semesters.Add(sysems);

                            if (!student.SemestersScore.Contains(sysems)) continue;
                            if (!student.HeaderList.ContainsKey(sysems)) continue;

                            int columnIndex = student.HeaderList[sysems];
                            SemesterScore semsscore = student.SemestersScore[sysems];

                            decimal? score = null;
                            decimal? weight = null;

                            if (header.IsDomain)
                            {
                                if (semsscore.Domain.Contains(header.Domain))
                                {
                                    score = semsscore.Domain[header.Domain].Value;
                                    weight = semsscore.Domain[header.Domain].Weight;
                                }
                            }
                            else
                            {
                                if (semsscore.Subject.Contains(header.Subject))
                                {
                                    score = semsscore.Subject[header.Subject].Value;
                                    weight = semsscore.Subject[header.Subject].Weight;
                                }
                            }

                            if (header.Domain == LearningDomainName)
                            {
                                score = semsscore.LearnDomainScore;

                                row.Cells[columnIndex * 3 + 2].CellFormat.FitText = false;
                                row.Cells[columnIndex * 3 + 2].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[columnIndex * 3 + 3].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (!score.HasValue) continue;

                                row.Cells[columnIndex * 3 + 2].Write(builder, ((double)score) + "");
                                row.Cells[columnIndex * 3 + 4].Write(builder, (Util.GetDegree(score.Value)));

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                    RatingRow = row.NextSibling as Row;
                            }
                            else
                            {
                                if (!score.HasValue) continue;
                                if (!weight.HasValue) weight = 0;

                                row.Cells[columnIndex * 3 + 2].Write(builder, ((double)weight) + "");
                                row.Cells[columnIndex * 3 + 3].Write(builder, ((double)score) + "");
                                row.Cells[columnIndex * 3 + 4].Write(builder, (Util.GetDegree(score.Value)));
                            }
                        }

                        //算平均...
                        decimal? avgScore = null;
                        if (header.IsDomain)
                        {
                            if (header.Domain == LearningDomainName)
                            {
                                avgScore = student.SemestersScore.AvgLearningDomainScore(semesters);

                                if (!avgScore.HasValue) continue;

                                if (student.CalculationRule == null)
                                    avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                                else
                                    avgScore = student.CalculationRule.ParseLearnDomainScore(avgScore.Value);
                            }
                            else
                            {
                                avgScore = student.SemestersScore.AvgDomainScore(semesters, header.Domain);

                                if (!avgScore.HasValue) continue;

                                if (student.CalculationRule == null)
                                    avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                                else
                                    avgScore = student.CalculationRule.ParseDomainScore(avgScore.Value);
                            }
                        }
                        else
                        {
                            avgScore = student.SemestersScore.AvgSubjectScore(semesters, header.Subject);

                            if (!avgScore.HasValue) continue;

                            if (student.CalculationRule == null)
                                avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                            else
                                avgScore = student.CalculationRule.ParseSubjectScore(avgScore.Value);
                        }

                        row.Cells[20].Write(builder, (double)avgScore + "");
                        row.Cells[21].Write(builder, Util.GetDegree(avgScore.Value));
                    }

                    if (RatingRow != null)
                    {
                        PlaceCollection places = student.Places.NS("年排名");

                        foreach (SemesterData semsIndex in student.HeaderList.Keys)
                        {
                            SemesterData raw = student.HeaderList.GetSRaw(semsIndex);

                            if (raw == SemesterData.Empty) continue;

                            string placeKey = SLearningDomainParser.GetSemesterString(raw);

                            Cell datacell = RatingRow.Cells[student.HeaderList[semsIndex] * 3 + 2];

                            if (places.Contains(placeKey))
                                datacell.Write(builder, GetPlaceString(places, placeKey));
                        }

                        if (places.Contains(LearningDomainName))
                            RatingRow.Cells[20].Write(builder, GetPlaceString(places, LearningDomainName));
                    }
                    #endregion

                    #region 合併相關欄位。
                    string previousCellDomain = string.Empty;
                    foreach (RowHeader header in indexHeaders)
                    {
                        bool hasGroup = !string.IsNullOrEmpty(Subj.GetDomainGroup(header.Domain));
                        string groupName = Subj.GetDomainGroup(header.Domain);

                        Row row = table.Rows[header.Index + 3];

                        if (previousCellDomain == row.Cells[0].ToTxt())
                            row.Cells[0].CellFormat.VerticalMerge = CellMerge.Previous;
                        else
                            row.Cells[0].CellFormat.VerticalMerge = CellMerge.First;

                        if (header.IsDomain)
                        {
                            if (header.Domain == LearningDomainName)
                            {
                                #region 學習領域
                                row.Cells[0].CellFormat.FitText = false;
                                row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                {
                                    Row lrow = row.NextSibling as Row;
                                    lrow.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                    lrow.Cells[0].CellFormat.FitText = false;
                                    lrow.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                                    Paragraph mp = lrow.Cells[2].Paragraphs[0];
                                    for (int i = 0; i < (3 * 6); i++)
                                    {
                                        if (i % 3 == 0)
                                        {
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                            (mp.ParentNode as Cell).CellFormat.FitText = false;
                                        }
                                        else
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;

                                        mp = Util.NextCell(mp as Paragraph);
                                    }
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                    mp = Util.NextCell(mp as Paragraph);
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;
                                }
                                #endregion
                            }
                            else
                            {
                                if (!hasGroup)
                                {
                                    row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                    row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                                }
                            }
                        }

                        previousCellDomain = row.Cells[0].ToTxt();
                    }
                    #endregion

                    #endregion
                }
                else if (PrintSetting.ListMethod == ListMethod.SubjectOnly)
                {
                    #region 列印科目
                    UniqueSet<RowHeader> RowIndexs = new UniqueSet<RowHeader>();

                    foreach (SemesterData semester in student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters))
                    {
                        SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);
                        if (!student.SemestersScore.Contains(sysems)) continue;

                        SemesterScore semsscore = student.SemestersScore[sysems];

                        foreach (string strSubject in semsscore.Subject)
                        {
                            SemesterSubjectScore subject = semsscore.Subject[strSubject];

                            RowHeader header;
                            if (IsFlexible(subject.Domain))
                                header = new RowHeader("彈性課程", strSubject);
                            else
                                header = new RowHeader(subject.Domain, strSubject);

                            header.IsDomain = false;

                            if (!RowIndexs.Contains(header))
                                RowIndexs.Add(header);
                        }
                    }

                    RowHeader lheader = new RowHeader(LearningDomainName, string.Empty);
                    lheader.IsDomain = true;
                    //RowIndexs.Add(lheader);

                    //List<RowHeader> sortedHeaders = SortHeader(RowIndexs.ToList());
                    List<RowHeader> sortedHeaders = RowIndexs.ToList();

                    sortedHeaders.Sort(delegate(RowHeader x, RowHeader y)
                    {
                        Subj xx = new JHSchool.Evaluation.Subject(x.Subject, x.Domain);
                        Subj yy = new JHSchool.Evaluation.Subject(y.Subject, y.Domain);

                        return xx.CompareTo(yy);
                    });
                    //sortedHeaders.Sort(Util.SortSubject);
                    //sortedHeaders.Sort(Util.SortDomain);

                    //把學習領域放在正確的地方。
                    foreach (RowHeader eachHeader in sortedHeaders.ToArray())
                    {
                        if (IsFlexible(eachHeader.Domain))
                        {
                            int index = sortedHeaders.IndexOf(eachHeader);
                            sortedHeaders.Insert(index, lheader);
                            break;
                        }
                    }
                    if (sortedHeaders.IndexOf(lheader) < 0)
                        sortedHeaders.Add(lheader);

                    //產生 Row。
                    List<RowHeader> indexHeaders = new List<RowHeader>();
                    Row current = template;
                    int rowIndex = 0;
                    foreach (RowHeader header in sortedHeaders)
                    {
                        RowHeader indexH = header;
                        indexH.Index = rowIndex++;
                        indexHeaders.Add(indexH);

                        Row datarow = table.InsertBefore(template.Clone(true), current) as Row;

                        if (header.Domain == LearningDomainName)
                        {
                            string headerName = string.Empty;

                            if (PrintSetting.PrintRank)
                                headerName = "學習領域排名";

                            if (PrintSetting.PrintRankPercentage)
                                headerName = "學習領域百分比";

                            if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                                headerName = "學習領域排名/百分比";

                            if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                            {
                                Row lrow = table.InsertAfter(template.Clone(true), datarow) as Row;
                                lrow.Cells[0].Write(builder, headerName);
                                rowIndex++;
                            }
                        }

                        if (IsFlexible(header.Domain))
                        {
                            datarow.Cells[0].Write(builder, header.Domain);
                            datarow.Cells[1].Write(builder, header.Subject);
                        }
                        else if (header.Domain == LearningDomainName)
                            datarow.Cells[0].Write(builder, header.Domain);
                        else
                            datarow.Cells[0].Write(builder, header.Subject);
                    }

                    //填資料
                    Row RatingRow = null;
                    foreach (RowHeader header in indexHeaders)
                    {
                        SemesterDataCollection semesters = new SemesterDataCollection();
                        Row row = table.Rows[header.Index + 3];
                        foreach (SemesterData semester in student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters))
                        {
                            SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);
                            semesters.Add(sysems);

                            if (!student.SemestersScore.Contains(sysems)) continue;
                            if (!student.HeaderList.ContainsKey(sysems)) continue;

                            int columnIndex = student.HeaderList[sysems];
                            SemesterScore semsscore = student.SemestersScore[sysems];

                            decimal? score = null;
                            decimal? weight = null;

                            if (header.IsDomain)
                            {
                                if (semsscore.Domain.Contains(header.Domain))
                                {
                                    score = semsscore.Domain[header.Domain].Value;
                                    weight = semsscore.Domain[header.Domain].Weight;
                                }
                            }
                            else
                            {
                                if (semsscore.Subject.Contains(header.Subject))
                                {
                                    score = semsscore.Subject[header.Subject].Value;
                                    weight = semsscore.Subject[header.Subject].Weight;
                                }
                            }

                            if (header.Domain == LearningDomainName)
                            {
                                score = semsscore.LearnDomainScore;

                                row.Cells[columnIndex * 3 + 2].CellFormat.FitText = false;
                                row.Cells[columnIndex * 3 + 2].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[columnIndex * 3 + 3].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (!score.HasValue) continue;

                                row.Cells[columnIndex * 3 + 2].Write(builder, ((double)score) + "");
                                row.Cells[columnIndex * 3 + 4].Write(builder, (Util.GetDegree(score.Value)));

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                    RatingRow = row.NextSibling as Row;
                            }
                            else
                            {
                                if (!score.HasValue) continue;
                                if (!weight.HasValue) weight = 0;

                                row.Cells[columnIndex * 3 + 2].Write(builder, ((double)weight) + "");
                                row.Cells[columnIndex * 3 + 3].Write(builder, ((double)score) + "");
                                row.Cells[columnIndex * 3 + 4].Write(builder, (Util.GetDegree(score.Value)));
                            }
                        }

                        //算平均...
                        decimal? avgScore = null;
                        if (header.IsDomain)
                        {
                            if (header.Domain == LearningDomainName)
                            {
                                avgScore = student.SemestersScore.AvgLearningDomainScore(semesters);

                                if (!avgScore.HasValue) continue;

                                if (student.CalculationRule == null)
                                    avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                                else
                                    avgScore = student.CalculationRule.ParseLearnDomainScore(avgScore.Value);
                            }
                            else
                            {
                                avgScore = student.SemestersScore.AvgDomainScore(semesters, header.Domain);

                                if (!avgScore.HasValue) continue;

                                if (student.CalculationRule == null)
                                    avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                                else
                                    avgScore = student.CalculationRule.ParseDomainScore(avgScore.Value);
                            }
                        }
                        else
                        {
                            avgScore = student.SemestersScore.AvgSubjectScore(semesters, header.Subject);

                            if (!avgScore.HasValue) continue;

                            if (student.CalculationRule == null)
                                avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                            else
                                avgScore = student.CalculationRule.ParseSubjectScore(avgScore.Value);
                        }

                        row.Cells[20].Write(builder, (double)avgScore + "");
                        row.Cells[21].Write(builder, Util.GetDegree(avgScore.Value));
                    }
                    if (RatingRow != null)
                    {
                        PlaceCollection places = student.Places.NS("年排名");

                        foreach (SemesterData semsIndex in student.HeaderList.Keys)
                        {
                            SemesterData raw = student.HeaderList.GetSRaw(semsIndex);

                            if (raw == SemesterData.Empty) continue;

                            string placeKey = SLearningDomainParser.GetSemesterString(raw);

                            Cell datacell = RatingRow.Cells[student.HeaderList[semsIndex] * 3 + 2];

                            if (places.Contains(placeKey))
                                datacell.Write(builder, GetPlaceString(places, placeKey));
                        }

                        if (places.Contains(LearningDomainName))
                            RatingRow.Cells[20].Write(builder, GetPlaceString(places, LearningDomainName));
                    }

                    //合併相關欄位。
                    string previousCellDomain = string.Empty;
                    foreach (RowHeader header in indexHeaders)
                    {
                        Row row = table.Rows[header.Index + 3];

                        if (header.IsDomain)
                        {
                            if (header.Domain == LearningDomainName)
                            {
                                row.Cells[0].CellFormat.FitText = false;
                                row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                {
                                    Row lrow = row.NextSibling as Row;
                                    lrow.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                    lrow.Cells[0].CellFormat.FitText = false;
                                    lrow.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                                    Paragraph mp = lrow.Cells[2].Paragraphs[0];
                                    for (int i = 0; i < (3 * 6); i++)
                                    {
                                        if (i % 3 == 0)
                                        {
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                            (mp.ParentNode as Cell).CellFormat.FitText = false;
                                        }
                                        else
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;

                                        mp = Util.NextCell(mp as Paragraph);
                                    }
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                    mp = Util.NextCell(mp as Paragraph);
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;
                                }
                            }
                        }
                        else if (IsFlexible(header.Domain))
                        {
                            if (previousCellDomain == header.Domain)
                                row.Cells[0].CellFormat.VerticalMerge = CellMerge.Previous;
                            else
                                row.Cells[0].CellFormat.VerticalMerge = CellMerge.First;

                            previousCellDomain = header.Domain;
                        }
                        else
                        {
                            //row.Cells[0].CellFormat.FitText = true;
                            row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                            row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                        }
                    }
                    #endregion
                }

                template.NextSibling.Remove();
                template.Remove();
                #endregion
            }
            else if (e.FieldName == "Fix:缺曠獎懲")
            {
                #region 列印獎懲資料
                int Offset = 2;
                Row MeritA = builder.CurrentParagraph.ParentNode.ParentNode as Row;
                Row MeritB = MeritA.NextSibling as Row;
                Row MeritC = MeritB.NextSibling as Row;
                Row DemeritA = MeritC.NextSibling as Row;
                Row DemeritB = DemeritA.NextSibling as Row;
                Row DemeritC = DemeritB.NextSibling as Row;
                Row DisciplineSet = DemeritC.NextSibling as Row;
                Row DisciplineNormal = DisciplineSet.NextSibling as Row;

                foreach (SemesterData each in student.Summaries.Keys)
                {
                    XmlElement summary = student.Summaries[each];

                    if (!student.HeaderList.ContainsKey(each)) continue;

                    int ColumnIndex = student.HeaderList[each];

                    XmlElement xmlmerit = summary.SelectSingleNode("DisciplineStatistics/Merit") as XmlElement;
                    XmlElement xmldemerit = summary.SelectSingleNode("DisciplineStatistics/Demerit") as XmlElement;

                    if (xmlmerit != null)
                    {
                        if (!string.IsNullOrEmpty(GetString(xmlmerit.GetAttribute("A"))))
                            MeritA.Cells[Offset + ColumnIndex].Write(builder, GetString(xmlmerit.GetAttribute("A")));
                        if (!string.IsNullOrEmpty(GetString(xmlmerit.GetAttribute("B"))))
                            MeritB.Cells[Offset + ColumnIndex].Write(builder, GetString(xmlmerit.GetAttribute("B")));
                        if (!string.IsNullOrEmpty(GetString(xmlmerit.GetAttribute("C"))))
                            MeritC.Cells[Offset + ColumnIndex].Write(builder, GetString(xmlmerit.GetAttribute("C")));
                    }

                    if (xmldemerit != null)
                    {
                        if (!string.IsNullOrEmpty(GetString(xmldemerit.GetAttribute("A"))))
                            DemeritA.Cells[Offset + ColumnIndex].Write(builder, GetString(xmldemerit.GetAttribute("A")));
                        if (!string.IsNullOrEmpty(GetString(xmldemerit.GetAttribute("B"))))
                            DemeritB.Cells[Offset + ColumnIndex].Write(builder, GetString(xmldemerit.GetAttribute("B")));
                        if (!string.IsNullOrEmpty(GetString(xmldemerit.GetAttribute("C"))))
                            DemeritC.Cells[Offset + ColumnIndex].Write(builder, GetString(xmldemerit.GetAttribute("C")));
                    }

                    StringBuilder normalString = new StringBuilder();
                    StringBuilder setString = new StringBuilder();

                    foreach (XmlElement absence in summary.SelectNodes("AttendanceStatistics/Absence"))
                    {
                        string count = absence.GetAttribute("Count");
                        string periodType = absence.GetAttribute("PeriodType");
                        string periodName = absence.GetAttribute("Name");

                        if (string.IsNullOrEmpty(count)) continue;
                        if (!PrintAbsences.ContainsKey(periodType)) continue;
                        if (!PrintAbsences[periodType].Contains(periodName)) continue;

                        if (periodType == "一般")
                        {
                            if (normalString.Length > 0) normalString.AppendLine();
                            normalString.Append(periodName + "：" + count);
                        }
                        else if (periodType == "集會")
                        {
                            if (setString.Length > 0) setString.AppendLine();
                            setString.Append(periodName + "：" + count);
                        }
                    }

                    DisciplineNormal.Cells[Offset + ColumnIndex].Write(builder, normalString.ToString());
                    DisciplineSet.Cells[Offset + ColumnIndex].Write(builder, setString.ToString());
                }
                #endregion
            }
        }

        private void RefineDomain(SemesterSubjectScore subject)
        {
            if (subject.Domain.Trim() == "彈性課程")
                subject.Domain = string.Empty;
        }

        private bool IsFlexible(string domainName)
        {
            if (string.IsNullOrEmpty(domainName))
                return true;

            if (domainName == "彈性課程")
                return true;

            return false;
        }

        /// <summary>
        /// 將領域、彈性課程、學習領域分開排序。
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<RowHeader> SortHeader(List<RowHeader> list)
        {
            List<RowHeader> domains = new List<RowHeader>();
            List<RowHeader> subjects = new List<RowHeader>();
            RowHeader? ldomain = null;

            foreach (RowHeader each in list)
            {
                if (each.Domain == LearningDomainName)
                {
                    ldomain = each;
                    continue;
                }

                if (Program.Mode == ModuleMode.HsinChu) //新竹..... = =''
                {
                    if (each.IsDomain || each.Domain == "語文")
                        domains.Add(each);
                    else
                        subjects.Add(each);
                }
                else
                {
                    if (each.IsDomain)
                        domains.Add(each);
                    else
                        subjects.Add(each);
                }
            }
            domains.Sort(delegate(RowHeader x, RowHeader y)
            {
                Subj xx = new JHSchool.Evaluation.Subject(x.Subject, x.Domain);
                Subj yy = new JHSchool.Evaluation.Subject(y.Subject, y.Domain);

                return xx.CompareTo(yy);
            });

            subjects.Sort((x, y) => (Subj.CompareSubjectOrdinal(x.Subject, y.Subject)));

            List<RowHeader> result = new List<RowHeader>();
            result.AddRange(domains);
            if (ldomain != null) result.Add(ldomain.Value);
            result.AddRange(subjects);

            return result;
        }

        private string GetPlaceString(PlaceCollection places, string placeKey)
        {
            Place place = places[placeKey];
            decimal percentage = (100m * ((decimal)place.Level / (decimal)place.Radix));

            //小於1%的話，就是1%。
            if (percentage < 1) percentage = 1;

            string result = string.Empty;

            if (PrintSetting.PrintRank)
                result = place.Level.ToString();

            if (PrintSetting.PrintRankPercentage)
                result = Math.Round(percentage, 0, MidpointRounding.AwayFromZero) + "%";

            if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                result = string.Format("{0}/{1}%", place.Level, Math.Round(percentage, 0, MidpointRounding.AwayFromZero));

            return result;
        }

        private static string GetString(string number)
        {
            int output;

            if (int.TryParse(number, out output))
                if (output <= 0) return string.Empty;

            return number;
        }

        private static string GetText(Paragraph originParagraph)
        {
            return originParagraph.ToTxt().Replace("«Fix:年級學期»", "").Replace("\r\n", "");
        }

        private static string GetSemesterString(SemesterData each)
        {
            return string.Format("{0} {1}", each.SchoolYear.ToString(), each.Semester == 1 ? "上" : "下");
        }

        private class MergeDataSource : IMailMergeDataSource
        {
            public List<ReportStudent> Students { get; private set; }

            private ReportPreference Preference { get; set; }

            private int Index { get; set; }

            private string PrintDate = DateTime.Now.ToString("yyyy/MM/dd");

            public MergeDataSource(List<ReportStudent> students, ReportPreference preference)
            {
                Students = students;
                Preference = preference;
                Index = -1;
            }

            #region IMailMergeDataSource 成員

            public bool GetValue(string fieldName, out object fieldValue)
            {
                fieldValue = string.Empty;
                ReportStudent student = Students[Index];

                if (fieldName.ToUpper().StartsWith("Fix:".ToUpper()))
                {
                    fieldValue = student;
                    return true;
                }

                switch (fieldName)
                {
                    case "學號":
                        fieldValue = student.StudentNumber;
                        break;
                    case "班級":
                        fieldValue = student.ClassName;
                        break;
                    case "座號":
                        fieldValue = student.SeatNo;
                        break;
                    case "姓名":
                        fieldValue = student.Name;
                        break;
                    case "性別":
                        fieldValue = student.Gender;
                        break;
                    case "生日":
                        DateTime dtBirthday;

                        if (DateTime.TryParse(student.Birthday, out dtBirthday))
                            fieldValue = dtBirthday.Year - 1911 + "/" + dtBirthday.Month + "/" + dtBirthday.Day;
                        else
                            fieldValue = string.Empty;

                        break;
                    case "列印日期":
                        DateTime dtPrintDate;

                        if (DateTime.TryParse(PrintDate, out dtPrintDate))
                            fieldValue = dtPrintDate.Year - 1911 + "/" + dtPrintDate.Month + "/" + dtPrintDate.Day;
                        else
                            fieldValue = string.Empty;
                        break;
                    case "學校名稱":
                        fieldValue = K12.Data.School.ChineseName;
                        break;
                    case "入學日期":
                        if (string.IsNullOrEmpty(Preference.EntranceDate))
                            fieldValue = student.EntranceDate;
                        else
                            fieldValue = Preference.EntranceDate;
                        break;
                    case "畢業日期":
                        if (string.IsNullOrEmpty(Preference.GraduateDate))
                            fieldValue = student.GraduateDate;
                        else
                            fieldValue = Preference.GraduateDate;
                        break;
                }

                return true;
            }

            public bool MoveNext()
            {
                Index++;

                return (Index < Students.Count);
            }

            public string TableName { get { return string.Empty; } }

            #endregion
        }
    }
}
