using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using Aspose.Words;
using Campus.Report;
using K12.Data;
using JHSchool.Evaluation.Mapping;
using FISCA.Presentation.Controls;

namespace JHEvaluation.StudentSemesterScoreReport.Writers
{
    internal class ScoreWriter
    {
        private ReportConfiguration _config;
        private bool _printPeriod;
        private bool _printCredit;
        private bool _printLearnDomain;
        private List<string> _domainsWithoutDetail;
        private DegreeMapper _degreeMapper;
        private bool _warned;
        private Dictionary<string, string> _domainText;

        public JHSemesterScoreRecord SemesterScoreRecord { get; set; }

        public ScoreWriter()
        {
            _config = new ReportConfiguration(Global.ReportName);
            _printPeriod = _config.GetBoolean("列印節數", true);
            _printCredit = _config.GetBoolean("列印權數", false);
            _printLearnDomain = _config.GetBoolean("列印學習領域總成績", true);
            _domainsWithoutDetail = new List<string>();
            _degreeMapper = new DegreeMapper();
            _warned = false;
            _domainText = new Dictionary<string, string>();

            string printScore = _config.GetString("領域科目設定", "Domain");
            if (printScore == "Domain")
            {
                List<string> list = Global.GetDomainList();
                if (Global.Params["Mode"] == "HsinChu") //新竹市，語文跟彈性科目分列
                    if (list.Contains("語文")) list.Remove("語文");

                if (list.Contains("彈性課程")) list.Remove("彈性課程");
                _domainsWithoutDetail.AddRange(list);
                //_domainsWithoutDetail.AddRange(new string[] { "數學", "社會", "藝術與人文", "自然與生活科技", "健康與體育", "綜合活動" });
            }
        }

        public void Writer(Document doc)
        {
            DocumentBuilder builder = new DocumentBuilder(doc);

            _warned = false;
            _domainText.Clear();
            if (SemesterScoreRecord == null)
            {
                builder.MoveToMergeField("成績_start");
                builder.MoveToMergeField("成績_end");
                builder.MoveToMergeField("文字描述");
                return;
            }

            builder.MoveToMergeField("成績_start");
            Cell startCell = builder.CurrentParagraph.ParentNode as Cell;
            builder.MoveToMergeField("成績_end");
            Cell endCell = builder.CurrentParagraph.ParentNode as Cell;

            Table table = startCell.ParentRow.ParentTable;
            int rowCount = table.IndexOf(endCell.ParentRow) - table.IndexOf(startCell.ParentRow) + 1;
            Font font = builder.Font;

            #region 填入學習領域總成績
            if (_printLearnDomain)
            {
                rowCount--;
                WordHelper.MergeHorizontalCell(endCell, 2);
                Cell scoreCell = endCell.NextSibling.NextSibling as Cell;
                WordHelper.MergeHorizontalCell(scoreCell, 2);

                WordHelper.Write(endCell, font, "學習領域總成績");
                WordHelper.Write(scoreCell, font, "" + SemesterScoreRecord.LearnDomainScore);
                WordHelper.Write(scoreCell.NextSibling.NextSibling as Cell, font, SemesterScoreRecord.LearnDomainScore.HasValue ? _degreeMapper.GetDegreeByScore(SemesterScoreRecord.LearnDomainScore.Value) : "");
            }
            #endregion

            #region 整理成 Domain -> List<Subject>
            Dictionary<string, List<SubjectScore>> domainSubjects = new Dictionary<string, List<SubjectScore>>();

            foreach (var subject in SemesterScoreRecord.Subjects.Values)
            {
                if (!domainSubjects.ContainsKey(subject.Domain))
                    domainSubjects.Add(subject.Domain, new List<SubjectScore>());
                domainSubjects[subject.Domain].Add(subject);
            }
            string printsocre = _config.GetString("領域科目設定", "Domain");
            if (printsocre == "Domain")
            {
                // 沒有科目成績只有領域成績時加入領域名稱(轉學生用到)            
                foreach (DomainScore domin in SemesterScoreRecord.Domains.Values)
                {
                    if (!domainSubjects.ContainsKey(domin.Domain))
                        domainSubjects.Add(domin.Domain, new List<SubjectScore>());

                    // 處理新竹語文呈現
                    if (domin.Domain == "語文")
                    {
                        int co = 0;
                        foreach (SubjectScore ss in SemesterScoreRecord.Subjects.Values)
                        {
                            if (ss.Domain == "語文")
                                co++;
                        }
                        if (co == 0)
                        if (!_domainsWithoutDetail.Contains(domin.Domain))
                            _domainsWithoutDetail.Add(domin.Domain);
                    }
                }
            }

            foreach (List<SubjectScore> subjectScoreList in domainSubjects.Values)
            {
                subjectScoreList.Sort(delegate(SubjectScore x, SubjectScore y)
                {
                    return JHSchool.Evaluation.Subject.CompareSubjectOrdinal(x.Subject, y.Subject);
                });
            }
            #endregion

            #region 檢查成績項目是否超出可列印行數
            int count = 0;
            foreach (string domain in Global.GetDomainList())
            {
                if (_domainsWithoutDetail.Contains(domain))
                    count++;
                else if (domainSubjects.ContainsKey(domain))
                    count += domainSubjects[domain].Count;
            }

            if (count > rowCount && _warned == false)
            {
                MsgBox.Show("成績項目超出可列印行數，部分成績將不會列在報表上。");
                _warned = true;
            }
            #endregion

            #region 填學期成績
            Cell currentCell = startCell;
            List<string> list = new List<string>(domainSubjects.Keys);

            // 當沒有學期領域
            if (list.Count == 0)
            {
                foreach (var domain in SemesterScoreRecord.Domains.Values)
                    list.Add(domain.Domain);
            }

            list.Sort(JHSchool.Evaluation.Subject.CompareDomainOrdinal);
            foreach (string domain in list)
            {
                if (_domainsWithoutDetail.Contains(domain))
                {
                    #region 只列印領域成績
                    if (!SemesterScoreRecord.Domains.ContainsKey(domain)) continue;
                    rowCount--;

                    DomainScore domainScore = SemesterScoreRecord.Domains[domain];

                    string group = JHSchool.Evaluation.Subject.GetDomainGroup(domain);
                    if (Global.Params["Mode"] == "KaoHsiung" &&
                        !string.IsNullOrEmpty(group))
                    {
                        currentCell.CellFormat.VerticalMerge = CellMerge.First;
                        WordHelper.Write(currentCell, font, group);
                        WordHelper.Write(currentCell.NextSibling as Cell, font, domain);
                    }
                    else
                    {
                        WordHelper.MergeHorizontalCell(currentCell, 2);
                        WordHelper.Write(currentCell, font, domain);
                    }
                    Cell scoreCell = currentCell.NextSibling.NextSibling as Cell;

                    WordHelper.Write(scoreCell, font, GetPeriodCredit(domainScore.Period, domainScore.Credit));
                    scoreCell = scoreCell.NextSibling as Cell;
                    WordHelper.Write(scoreCell, font, "" + domainScore.Score);
                    scoreCell = scoreCell.NextSibling as Cell;
                    WordHelper.Write(scoreCell, font, (domainScore.Score.HasValue ? _degreeMapper.GetDegreeByScore(domainScore.Score.Value) : ""));

                    if (!_domainText.ContainsKey(domain))
                        _domainText.Add(domain, domainScore.Text);

                    if (rowCount <= 0) break;
                    currentCell = WordHelper.GetMoveDownCell(currentCell, 1);
                    #endregion
                }
                else
                {
                    #region 詳列科目成績
                    if (!domainSubjects.ContainsKey(domain)) continue;

                    Cell subjectCell = currentCell;
                    int subjectCount = 0;
                    foreach (SubjectScore subjectScore in domainSubjects[domain])
                    {
                        rowCount--;
                        subjectCount++;

                        Cell temp = subjectCell.NextSibling as Cell;
                        WordHelper.Write(temp, font, subjectScore.Subject);
                        temp = temp.NextSibling as Cell;
                        WordHelper.Write(temp, font, GetPeriodCredit(subjectScore.Period, subjectScore.Credit));
                        temp = temp.NextSibling as Cell;
                        WordHelper.Write(temp, font, "" + subjectScore.Score);
                        temp = temp.NextSibling as Cell;
                        WordHelper.Write(temp, font, (subjectScore.Score.HasValue ? _degreeMapper.GetDegreeByScore(subjectScore.Score.Value) : ""));

                        if (!_domainText.ContainsKey(subjectScore.Subject))
                            _domainText.Add(subjectScore.Subject, subjectScore.Text);

                        if (rowCount <= 0) break;
                        subjectCell = WordHelper.GetMoveDownCell(subjectCell, 1);
                    }
                    if (subjectCount > 1)
                        WordHelper.MergeVerticalCell(currentCell, subjectCount);
                    WordHelper.Write(currentCell, font, string.IsNullOrEmpty(domain) ? "彈性課程" : domain);

                    if (rowCount <= 0) break;
                    currentCell = WordHelper.GetMoveDownCell(currentCell, subjectCount);
                    #endregion
                }
            }
            currentCell = startCell;
            string lastGroup = string.Empty;
            while (currentCell != endCell)
            {
                string currentGroup = currentCell.ToTxt().Replace("\r\n", "");
                if (currentGroup == lastGroup && !string.IsNullOrEmpty(lastGroup))
                    currentCell.CellFormat.VerticalMerge = CellMerge.Previous;
                lastGroup = currentGroup;
                currentCell = WordHelper.GetMoveDownCell(currentCell, 1);
                if (currentCell == null) break;
            }
            #endregion

            #region 填學習領域文字描述
            builder.MoveToMergeField("文字描述");
            Cell textCell = builder.CurrentParagraph.ParentNode as Cell;

            textCell.Paragraphs.Clear();
            foreach (string header in _domainText.Keys)
            {
                if (string.IsNullOrEmpty(_domainText[header])) continue;

                textCell.Paragraphs.Add(new Paragraph(doc));
                Run run1 = new Run(doc);
                run1.Font.Name = font.Name;
                run1.Font.Size = font.Size;
                run1.Font.Bold = true;
                run1.Text = header + "：";
                Run run2 = new Run(doc);
                run2.Font.Name = font.Name;
                run2.Font.Size = font.Size;
                run2.Text = _domainText[header];

                textCell.LastParagraph.Runs.Add(run1);
                textCell.LastParagraph.Runs.Add(run2);
            }
            #endregion
        }

        private string GetPeriodCredit(decimal? p, decimal? c)
        {
            JHSchool.Evaluation.PeriodCredit pc = new JHSchool.Evaluation.PeriodCredit();
            pc.Parse(p + "/" + c);
            if (_printPeriod && _printCredit)
                return pc.ToString();
            else if (_printPeriod)
                return "" + pc.Period;
            else if (_printCredit)
                return "" + pc.Credit;
            else
                return string.Empty;
        }
    }
}
