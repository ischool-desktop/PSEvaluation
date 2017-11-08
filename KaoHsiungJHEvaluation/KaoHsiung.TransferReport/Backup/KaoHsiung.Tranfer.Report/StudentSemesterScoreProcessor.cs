using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using JHSchool.Data;

namespace KaoHsiung.TransferReport
{
    class StudentSemesterScoreProcessor
    {
        private DocumentBuilder _builder;
        private DomainRowManager _manager;
        private SemesterMap _map = new SemesterMap();
        private Config _config;

        private Cell _cell;
        private Run _run;

        public StudentSemesterScoreProcessor(DocumentBuilder builder, SemesterMap map, Config config)
        {
            builder.MoveToMergeField("成績");
            _builder = builder;

            _cell = builder.CurrentParagraph.ParentNode as Cell;
            _run = new Run(_cell.Document);
            _run.Font.Name = builder.Font.Name;
            _run.Font.Size = builder.Font.Size;
            _run.Text = string.Empty;
            _map = map;

            _config = config;
            _manager = new DomainRowManager(_config);
        }

        public void SetData(List<JHSemesterScoreRecord> semesterScoreList)
        {
            Dictionary<string, DomainRow> list = new Dictionary<string, DomainRow>();

            foreach (JHSemesterScoreRecord record in semesterScoreList)
            {
                _manager.Add(record);

                //foreach (K12.Data.SubjectScore subject in record.Subjects.Values)
                //{
                //    if (subject.Domain != "彈性課程") continue;

                //    if (!list.ContainsKey(subject.Subject))
                //        list.Add(subject.Subject, new DomainRow(subject.Subject));

                //    DomainRow row = list[subject.Subject];

                //    row.Add(new SemesterData("" + record.SchoolYear, "" + record.Semester), subject);
                //}
            }

            FillDomainRows();
            FillElasticCourse(list);
        }

        private void FillElasticCourse(Dictionary<string, DomainRow> list)
        {
            //_builder.MoveToMergeField("彈性課程");

            //if (list.Count == 0) return;

            //Cell cell = _builder.CurrentParagraph.ParentNode as Cell;
            //Row row = cell.ParentRow;
            //Table table = row.ParentTable;

            //int count = list.Count;
            //List<string> sort = new List<string>(list.Keys);
            //sort.Sort();
            //foreach (string key in sort)
            //{
            //    count--;
            //    if (count > 0)
            //        table.Rows.Add(row.Clone(true));

            //    DomainRow domainRow = list[key];

            //    Write(cell, domainRow.Domain);
            //    WriteDomain(WordHelper.GetMoveRightCell(cell, 1), domainRow);
            //    cell = WordHelper.GetMoveDownCell(cell, 1);

            //    if (cell == null) break;
            //}
        }

        private void FillDomainRows()
        {
            Cell cell = _cell;
            Row row = _cell.ParentRow;
            Table table = row.ParentTable;
            int rowIndex = table.IndexOf(row);

            foreach (DomainRow domainRow in _manager.DomainRows)
            {
                // 當有領域沒有科目成績
                if (domainRow.SubjectScores.Count <= 0)
                {
                    // 沒有領域成績
                    if (domainRow.Scores.Count <= 0) continue;

                    // 當使用者選擇科目又沒科目成績
                    if (_config.DomainSubjectSetup == "Subject" && domainRow.SubjectScores.Count == 0)
                        continue;

                    table.Rows.Add(row.Clone(true));
                    Row tempRow = table.LastRow;
                    WordHelper.MergeHorizontalCell(tempRow.Cells[0], 2);
                    Write(tempRow.Cells[0], (string.IsNullOrEmpty(domainRow.Domain) ? "彈性課程" : domainRow.Domain));

                    // 當使用者選科目時，不列印領領域成績
                    if (_config.DomainSubjectSetup == "Domain")
                        WriteDomain(tempRow.Cells[1], domainRow);
                }
                else
                {
                    int subjectCount = 0;
                    foreach (string subject in domainRow.SubjectScores.Keys)
                    {
                        subjectCount++;

                        //table.Rows.Insert(rowIndex++, table.Rows[deleteIndex].Clone(true) as Row);
                        //Row tempRow = table.Rows[rowIndex];
                        table.Rows.Add(row.Clone(true));
                        Row tempRow = table.LastRow;

                        Write(tempRow.Cells[1], subject);
                        WriteSubject(tempRow.Cells[1], domainRow.SubjectScores[subject]);
                    }
                    Row startRow = table.Rows[table.Rows.Count - subjectCount];
                    Write(startRow.Cells[0], string.IsNullOrEmpty(domainRow.Domain) ? "彈性課程" : domainRow.Domain);
                    if (subjectCount > 1)
                        WordHelper.MergeVerticalCell(startRow.Cells[0], subjectCount);
                }
            }

            row.Remove();

            //Cell typeCell = table.Rows[rowIndex - typeCount + 1].Cells[0];
            //WordHelper.MergeVerticalCell(typeCell, typeCount);
            //Write(typeCell, "缺曠資料");

            //Cell cell = _cell;
            //int first_row_index = _cell.ParentRow.ParentTable.IndexOf(_cell.ParentRow);

            //int count = 0;
            //foreach (DomainRow row in _manager.DomainRows)
            //{
            //    WriteDomain(cell, row);
            //    cell = WordHelper.GetMoveDownCell(cell, 1);

            //    if (cell == null) break;

            //    if (++count == 2)
            //        cell = WordHelper.GetMoveLeftCell(cell, 1);
            //}
        }

        private string GetPeriodCredit(string pc)
        {
            if (_config.PrintPeriod && _config.PrintCredit) return pc;
            else if (_config.PrintPeriod) return pc.Split('/')[0];
            else if (_config.PrintCredit) return pc.Split('/')[1];
            else return string.Empty;
        }

        private void WriteSubject(Cell cell, Dictionary<SemesterData, ScoreData> dictionary)
        {
            Row row = cell.ParentRow;
            int shift = row.IndexOf(cell) + 1;
            int fields = 3;
            foreach (SemesterData sems in dictionary.Keys)
            {
                ScoreData data = dictionary[sems];
                int index = -1;
                if (_map.SemesterMapping.ContainsKey(sems))
                    index = _map.SemesterMapping[sems];

                if (index < 0) continue;

                index = index * fields + shift;
                Write(row.Cells[index], GetPeriodCredit(data.GetPeriodCredit()));
                Write(row.Cells[index + 1], data.Score);
                Write(row.Cells[index + 2], data.GetDegree());
            }
        }

        private void WriteDomain(Cell cell, DomainRow domain)
        {
            Row row = cell.ParentRow;
            int shift = row.IndexOf(cell) + 1;
            int fields = 3;
            foreach (SemesterData sems in domain.Scores.Keys)
            {
                ScoreData data = domain.Scores[sems];
                int index = -1;
                if (_map.SemesterMapping.ContainsKey(sems))
                    index = _map.SemesterMapping[sems];

                if (index < 0) continue;

                index = index * fields + shift;
                Write(row.Cells[index], GetPeriodCredit(data.GetPeriodCredit()));
                Write(row.Cells[index + 1], data.Score);
                Write(row.Cells[index + 2], data.GetDegree());
            }
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

    class DomainRowManager
    {
        private Dictionary<string, DomainRow> _domains;
        private Config _config;

        public DomainRowManager(Config config)
        {
            _domains = new Dictionary<string, DomainRow>();
            _config = config;
        }

        public List<DomainRow> DomainRows
        {
            get
            {
                List<DomainRow> rows = new List<DomainRow>();
                foreach (string key in _config.PrintSubjects.Keys)
                {
                    if (_domains.ContainsKey(key))
                        rows.Add(_domains[key]);
                    else
                        rows.Add(new DomainRow(key));
                }
                return rows;
            }
        }

        public void Add(JHSemesterScoreRecord record)
        {
            foreach (K12.Data.DomainScore domain in record.Domains.Values)
            {
                if (!_config.PrintSubjects.ContainsKey(domain.Domain)) continue;

                if (!_domains.ContainsKey(domain.Domain))
                    _domains.Add(domain.Domain, new DomainRow(domain.Domain));

                DomainRow row = _domains[domain.Domain];

                row.Add(new SemesterData("" + record.SchoolYear, "" + record.Semester), domain);
            }

            foreach (K12.Data.SubjectScore subject in record.Subjects.Values)
            {
                if (!_config.PrintSubjects.ContainsKey(subject.Domain)) continue;
                if (!_config.PrintSubjects[subject.Domain].Contains(subject.Subject)) continue;

                if (!_domains.ContainsKey(subject.Domain))
                    _domains.Add(subject.Domain, new DomainRow(subject.Domain));

                DomainRow row = _domains[subject.Domain];
                row.Add(new SemesterData("" + record.SchoolYear, "" + record.Semester), subject);
            }
        }
    }

    class DomainRow
    {
        public string Domain { get; private set; }

        private Dictionary<SemesterData, ScoreData> _scores;
        public Dictionary<SemesterData, ScoreData> Scores { get { return _scores; } }

        private Dictionary<string, Dictionary<SemesterData, ScoreData>> _subjectScores;
        public Dictionary<string, Dictionary<SemesterData, ScoreData>> SubjectScores { get { return _subjectScores; } }

        public DomainRow(string domain)
        {
            Domain = domain;
            _scores = new Dictionary<SemesterData, ScoreData>();
            _subjectScores = new Dictionary<string, Dictionary<SemesterData, ScoreData>>();
        }

        public bool Add(SemesterData semester, K12.Data.DomainScore domain)
        {
            if (!_scores.ContainsKey(semester))
            {
                _scores.Add(semester, new ScoreData("" + domain.Period, "" + domain.Credit, "" + domain.Score));
                return true;
            }
            return false;
        }

        public bool Add(SemesterData semester, K12.Data.SubjectScore subject)
        {
            if (!_subjectScores.ContainsKey(subject.Subject))
                _subjectScores.Add(subject.Subject, new Dictionary<SemesterData, ScoreData>());
            if (!_subjectScores[subject.Subject].ContainsKey(semester))
            {
                _subjectScores[subject.Subject].Add(semester, new ScoreData("" + subject.Period, "" + subject.Credit, "" + subject.Score));
                return true;
            }
            return false;
        }
    }

    class ScoreData
    {
        public string Period { get; private set; }
        public string Credit { get; private set; }
        public string Score { get; private set; }

        public ScoreData(string period, string credit, string score)
        {
            Period = period;
            Credit = credit;

            bool fix_need = score.Contains(".");
            while (fix_need)
            {
                if (score.EndsWith("0"))
                {
                    score = score.Substring(0, score.Length - 1);
                    fix_need = score.EndsWith("0");
                }
                else
                    fix_need = false;

                if (score.EndsWith("."))
                {
                    score = score.Substring(0, score.Length - 1);
                    fix_need = false;
                }
            }

            Score = score;
        }

        public string GetPeriodCredit()
        {
            //if (Period == Credit)
            //    return Period;
            //else
            return Period + "/" + Credit;
        }

        public string GetDegree()
        {
            decimal d;
            if (!decimal.TryParse(Score, out d))
                return string.Empty;
            else
            {
                if (d >= 90) return "優";
                else if (d >= 80) return "甲";
                else if (d >= 70) return "乙";
                else if (d >= 60) return "丙";
                else return "丁";
            }
        }
    }
}
