using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using JHSchool.Data;
using JHSchool.Evaluation.Mapping;
using System.Linq;

namespace KaoHsiung.StudentRecordReport.Processor
{
    class StudentSemesterScoreProcessor
    {
        private DocumentBuilder _builder;
        private DomainRowManager _manager;
        private SemesterMap _map = new SemesterMap();
        private Dictionary<string, bool> _domains = new Dictionary<string, bool>();

        public DegreeMapper DegreeMapper { get; set; }
        public bool PrintPeriod { get; set; }
        public bool PrintCredit { get; set; }

        private K12.Data.GradScoreRecord _StudGradScore;

        private Cell _cell;

        public StudentSemesterScoreProcessor(DocumentBuilder builder, SemesterMap map, string type,Dictionary<string,bool> domains,K12.Data.GradScoreRecord StudGradScore)
        {
            builder.MoveToMergeField("成績");
            _builder = builder;
            _manager = new DomainRowManager( type);
            _cell = builder.CurrentParagraph.ParentNode as Cell;
            _map = map;
            _domains = domains;
            _StudGradScore = StudGradScore;
        }

        public void SetData(List<JHSemesterScoreRecord> semesterScoreList)
        {
            Dictionary<string, DomainRow> list = new Dictionary<string, DomainRow>();

            foreach (JHSemesterScoreRecord record in semesterScoreList)
                _manager.Add(record);

            FillDomainRows();
        }

        private void FillDomainRows()
        {
            Cell cell = _cell;
            Row row = _cell.ParentRow;
            Table table = row.ParentTable;
            int deleteIndex = table.IndexOf(row);
            int rowIndex = table.IndexOf(row);

            foreach (DomainRow domainRow in _manager.DomainRows)
            {
                // 透過對照表查詢領域是否需要展開
                bool display = DomainSubjectExpand.展開;
                if (_domains.ContainsKey(domainRow.Domain))
                    display = _domains[domainRow.Domain];
                
                // 不展開，列印領域
                if (display == DomainSubjectExpand.不展開)
                {
                    if (domainRow.Scores.Count <= 0) continue;

                    table.Rows.Add(row.Clone(true));
                    Row tempRow = table.LastRow;
                    WordHelper.MergeHorizontalCell(tempRow.Cells[0], 2);
                    WordHelper.Write(tempRow.Cells[0], (string.IsNullOrEmpty(domainRow.Domain) ? "彈性課程" : domainRow.Domain), _builder);
                    WriteDomain(tempRow.Cells[1], domainRow);
                }
                // 展開，列印科目
                else
                {
                    int subjectCount = 0;
                    foreach (string subject in domainRow.SubjectScores.Keys)
                    {
                        subjectCount++;

                        table.Rows.Add(row.Clone(true));
                        Row tempRow = table.LastRow;

                        WordHelper.Write(tempRow.Cells[1], subject, _builder);
                        WriteSubject(tempRow.Cells[1], domainRow.SubjectScores[subject]);
                    }
                    // 當只有領域成績沒有科目成績時
                    if (subjectCount > 0)
                    {
                        Row startRow = table.Rows[table.Rows.Count - subjectCount];
                        WordHelper.Write(startRow.Cells[0], (string.IsNullOrEmpty(domainRow.Domain) ? "彈性課程" : domainRow.Domain), _builder);
                        if (subjectCount > 1)
                            WordHelper.MergeVerticalCell(startRow.Cells[0], subjectCount);
                    }
                }

                //// 畢業成績  (畢業成績讀取法，因為目前高雄沒有畢業成績，新竹有，所以暫註)
                //if (_StudGradScore.Domains.ContainsKey(domainRow.Domain))
                //{
                //    Row tmpRow = table.LastRow;
                //    if (_StudGradScore.Domains[domainRow.Domain].Score.HasValue)
                //    {
                //        // 分數
                //        WordHelper.Write(tmpRow.Cells[20], _StudGradScore.Domains[domainRow.Domain].Score.Value.ToString(), _builder);
                //        // 等第
                //        WordHelper.Write(tmpRow.Cells[21], DegreeMapper.GetDegreeByScore(_StudGradScore.Domains[domainRow.Domain].Score.Value), _builder);
                //    }
                //}     
                
            }

            table.Rows[deleteIndex].Remove();
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
                WordHelper.Write(row.Cells[index], GetPeriodCredit(data.GetPeriodCredit()), _builder);
                WordHelper.Write(row.Cells[index + 1], data.Score, _builder);
                WordHelper.Write(row.Cells[index + 2], GetDegree(data.Score), _builder);
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
                WordHelper.Write(row.Cells[index], GetPeriodCredit(data.GetPeriodCredit()), _builder);
                WordHelper.Write(row.Cells[index + 1], data.Score, _builder);
                WordHelper.Write(row.Cells[index + 2], GetDegree(data.Score), _builder);
            }
        }

        private string GetPeriodCredit(string orig)
        {
            JHSchool.Evaluation.PeriodCredit pc = new JHSchool.Evaluation.PeriodCredit();
            pc.Parse(orig);
            if (PrintPeriod && PrintCredit)
                return pc.ToString();
            else if (PrintPeriod)
                return "" + pc.Period;
            else if (PrintCredit)
                return "" + pc.Credit;
            else
                return string.Empty;
        }

        private string GetDegree(string p)
        {
            decimal d;
            if (decimal.TryParse(p, out d))
                return DegreeMapper.GetDegreeByScore(d);
            else
                return p;
        }
    }

    class DomainRowManager
    {
        private Dictionary<string, DomainRow> _domains;

        private string _type;

        public DomainRowManager( string type)
        {
            _domains = new Dictionary<string, DomainRow>();
            
            _type = type;
        }

        public List<DomainRow> DomainRows
        {
            get
            {
                List<DomainRow> rows = new List<DomainRow>(_domains.Values);
                List<DomainRow> retVal = new List<DomainRow>();
                //foreach (string key in _config.Keys)
                //{
                //    if (_domains.ContainsKey(key))
                //        rows.Add(_domains[key]);
                //    else
                //        rows.Add(new DomainRow(key));
                //}

                List<string> sortList = (from x in rows select x.Domain).ToList();
                sortList.Sort(DomainSorter.Sort1);

                foreach (string str in sortList)
                    foreach (DomainRow dr in rows)
                        if (str == dr.Domain)
                            retVal.Add(dr);
                //return rows;
                return retVal;
            }
        }

       

        public void Add(JHSemesterScoreRecord record)
        {
            foreach (K12.Data.DomainScore domain in record.Domains.Values)
            {
                //if (!_config.ContainsKey(domain.Domain)) continue;

                if (!_domains.ContainsKey(domain.Domain))
                    _domains.Add(domain.Domain, new DomainRow(domain.Domain));

                DomainRow row = _domains[domain.Domain];

                row.Add(new SemesterData("" + record.SchoolYear, "" + record.Semester), domain);
            }

            foreach (K12.Data.SubjectScore subject in record.Subjects.Values)
            {
                //if (!_config.ContainsKey(subject.Domain)) continue;
                //if (_type == "Domain")
                //{
                //    if (!_config[subject.Domain].Contains(subject.Subject)) continue;
                //}

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
            return Period + "/" + Credit;
        }
    }
}
