using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using HsinChu.JHEvaluation.Data;
using JHSchool.Evaluation;
using JHSchool.Data;
using FISCA.Presentation.Controls;
using JHSchool.Evaluation.Calculation;

namespace HsinChu.MidTermTransferReport.Processor
{
    internal class StudentExamScore
    {
        private DocumentBuilder _builder;
        private Font _font;
        private Config _config;
        private Dictionary<string, JHCourseRecord> _courseDict;

        private Dictionary<string, int> _columnMapping;

        private DomainManager _manager;
        private ScoreCalculator _calculator;

        public StudentExamScore(DocumentBuilder builder, Config config, Dictionary<string, JHCourseRecord> courseDict)
        {
            _builder = builder;

            _config = config;
            _courseDict = courseDict;

            _manager = new DomainManager();
            _manager.SetCourseDictionary(_courseDict);
            _manager.SetConfig(_config);
        }

        internal void SetColumnMap(Dictionary<string, int> columnMapping)
        {
            _columnMapping = columnMapping;
        }

        public void SetData(List<HC.JHSCETakeRecord> sces)
        {
            foreach (var sce in sces)
                _manager.Add(sce);

            FillScore();
        }

        private void FillScore()
        {
            _builder.MoveToMergeField("成績");
            _font = _builder.Font;
            Cell indexCell = _builder.CurrentParagraph.ParentNode as Cell;

            //排序
            List<string> domains = new List<string>(_manager.Domains.Keys);
            domains.Sort(JHSchool.Evaluation.Subject.CompareDomainOrdinal);

            foreach (var domainName in domains)
            {
                DomainRow domain = _manager.Domains[domainName];

                bool display = false;
                if (_config.PrintDomains.ContainsKey(domain.DomainName))
                    display = _config.PrintDomains[domain.DomainName];
                else
                    display = domain.Display;

                if (display)
                {
                    Cell temp = indexCell;
                    indexCell = WordHelper.GetMoveDownCell(indexCell, 1);
                    WriteDomainRowOnly(temp, domain);
                    WordHelper.MergeHorizontalCell(temp, 2);
                    if (indexCell == null)
                    {

                        break;
                    }
                }
                else
                {
                    if (domain.Subjects.Count > 0)
                    {
                        int count = WriteDomainRow(indexCell, domain);
                        Cell temp = indexCell;
                        indexCell = WordHelper.GetMoveDownCell(indexCell, count);
                        WordHelper.MergeVerticalCell(temp, count);
                        if (indexCell == null)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private int WriteDomainRow(Cell indexCell, DomainRow domain)
        {
            WordHelper.Write(indexCell, _font, string.IsNullOrEmpty(domain.DomainName) ? "彈性課程" : domain.DomainName);

            Cell subjectCell = WordHelper.GetMoveRightCell(indexCell, 1);
            int count = 0;
            foreach (var subjectName in domain.Subjects.Keys)
            {
                SubjectRow row = domain.Subjects[subjectName];
                //if (row.Display)
                //{
                    Cell temp = subjectCell;
                    WordHelper.Write(temp, _font, row.SubjectName);
                    temp = temp.NextSibling as Cell;
                    WordHelper.Write(temp, _font, row.PeriodCredit);

                    foreach (string examID in row.Scores.Keys)
                    {
                        ScoreData data = row.Scores[examID];
                        temp = WordHelper.GetMoveRightCell(subjectCell, _columnMapping[examID] - 1);
                        WordHelper.Write(temp, _font, (data.Score.HasValue ? "" + _calculator.ParseSubjectScore(data.Score.Value) : ""));
                        WordHelper.Write(temp.NextSibling as Cell, _font, "" + data.AssignmentScore);
                        WordHelper.Write(temp.NextSibling.NextSibling as Cell, _font, "" + row.GetFinalScore(examID));
                    }

                    subjectCell = WordHelper.GetMoveDownCell(subjectCell, 1);
                    if (subjectCell == null)
                    {
                        break;
                    }
                    count++;
                //}
            }
            return count;
        }

        private void WriteDomainRowOnly(Cell indexCell, DomainRow domain)
        {
            WordHelper.Write(indexCell, _font, domain.DomainName);
            Cell temp = indexCell.NextSibling as Cell;
            temp = temp.NextSibling as Cell;
            WordHelper.Write(temp, _font, domain.PeriodCredit);

            foreach (var examID in domain.ExamIDs)
            {
                decimal? score = domain.GetScore(examID);
                decimal? assignmentScore = domain.GetAssignmentScore(examID);
                decimal? finalScore = domain.GetFinalScore(examID);

                temp = WordHelper.GetMoveRightCell(indexCell, _columnMapping[examID]);
                WordHelper.Write(temp, _font, (score.HasValue ? "" + _calculator.ParseDomainScore(score.Value) : ""));
                WordHelper.Write(temp.NextSibling as Cell, _font, (assignmentScore.HasValue ? "" + _calculator.ParseDomainScore(assignmentScore.Value) : ""));
                WordHelper.Write(temp.NextSibling.NextSibling as Cell, _font, (finalScore.HasValue ? "" + _calculator.ParseDomainScore(finalScore.Value) : ""));
            }
        }

        internal void SetCalculator(JHSchool.Evaluation.Calculation.ScoreCalculator studentCalculator)
        {
            _calculator = studentCalculator;
        }

        internal void SetSubjects(List<string> list)
        {
            _manager.SetSubjects(list);
        }
    }

    class DomainManager
    {
        private Config _config;
        public Dictionary<string, DomainRow> Domains { get; set; }
        private Dictionary<string, JHCourseRecord> _courseDict;

        public DomainManager()
        {
            Domains = new Dictionary<string, DomainRow>();
        }

        internal void Add(HC.JHSCETakeRecord sce)
        {
            if (!_courseDict.ContainsKey(sce.RefCourseID)) return; //如果評量成績的課程不存在，return。
            JHCourseRecord course = _courseDict[sce.RefCourseID];

            if (!Domains.ContainsKey(course.Domain))
                Domains.Add(course.Domain, new DomainRow(course.Domain));

            DomainRow row = Domains[course.Domain];
            if (_config.DomainSubjectSetup == DomainSubjectSetup.Subject)
                row.Display = false;
            else
                row.Display = true;
            row.AddExamID(sce.RefExamID);

            if (!row.Subjects.ContainsKey(course.Subject))
                row.Subjects.Add(course.Subject, new SubjectRow(course.Subject));
            SubjectRow subjectRow = row.Subjects[course.Subject];
            subjectRow.SetPeriodCredit(course.Period, course.Credit);
            subjectRow.AddScore(sce.RefExamID, sce.Score, sce.AssignmentScore);
            //if (_config.DomainSubjectSetup == DomainSubjectSetup.Subject)
            //    subjectRow.Display = true;
        }

        internal void SetCourseDictionary(Dictionary<string, JHCourseRecord> courseDict)
        {
            _courseDict = courseDict;
        }

        internal void SetConfig(Config config)
        {
            _config = config;
            //foreach (var domain in config.PrintSubjects.Keys)
            //{
            //    if (!Domains.ContainsKey(domain))
            //        Domains.Add(domain, new DomainRow(domain));

            //    DomainRow domainRow = Domains[domain];
            //    domainRow.SetConfig(config);
            //    domainRow.Display = config.PrintDomains.ContainsKey(domain) ? config.PrintDomains[domain] : true;
            //    foreach (var subject in config.PrintSubjects[domain])
            //    {
            //        domainRow.AddSubject(subject);
            //        domainRow.Subjects[subject].SetConfig(config);
            //        domainRow.Subjects[subject].Display = true;
            //    }
            //}
        }

        internal void SetSubjects(List<string> list)
        {
            List<JHCourseRecord> courseList = new List<JHCourseRecord>();

            foreach (string courseID in list)
            {
                if (!_courseDict.ContainsKey(courseID)) continue;
                courseList.Add(_courseDict[courseID]);
            }

            courseList.Sort(delegate(JHCourseRecord x, JHCourseRecord y)
            {
                return JHSchool.Evaluation.Subject.CompareSubjectOrdinal(x.Subject, y.Subject);
            });

            foreach (JHCourseRecord course in courseList)
            {
                string domain = course.Domain;
                string subject = course.Subject;

                if (!Domains.ContainsKey(domain))
                    Domains.Add(domain, new DomainRow(domain));

                DomainRow domainRow = Domains[domain];

                if (!domainRow.Subjects.ContainsKey(subject))
                    domainRow.AddSubject(subject);
                domainRow.Subjects[subject].SetPeriodCredit(course.Period, course.Credit);
                //domainRow.Subjects[subject].Display = true;
            }
        }
    }

    class DomainRow
    {
        public string DomainName { get; set; }
        public string PeriodCredit
        {
            get
            {
                PeriodCredit pc = new PeriodCredit();

                foreach (var subjectRow in Subjects.Values)
                {
                    pc.Period += subjectRow.Period;
                    pc.Credit += subjectRow.Credit;
                }

                if (_printPeriod && _printCredit)
                    return pc.ToString();
                else if (_printPeriod)
                    return pc.Period.ToString();
                else if (_printCredit)
                    return pc.Credit.ToString();
                else
                    return string.Empty;
            }
        }

        private Dictionary<string, decimal?> _scoreCache;
        private List<string> _scoreCacheExamIDs;

        private Dictionary<string, decimal?> _assignmentScoreCache;
        private List<string> _assignmentScoreCacheExamIDs;

        public bool Display { get; set; }
        public Dictionary<string, SubjectRow> Subjects { get; set; }
        public List<string> ExamIDs;
        private bool _printPeriod;
        private bool _printCredit;

        public decimal? GetScore(string examID)
        {
            if (_scoreCacheExamIDs.Contains(examID) && _scoreCache.ContainsKey(examID))
                return _scoreCache[examID];
            else
            {
                _scoreCacheExamIDs.Add(examID);
                decimal s = decimal.Zero;
                decimal c = decimal.Zero;
                bool hasScore = false;

                foreach (var subject in Subjects.Values)
                {
                    if (!subject.Scores.ContainsKey(examID)) continue;
                    ScoreData data = subject.Scores[examID];

                    if (!data.Score.HasValue) continue;

                    s += data.Score.Value * subject.Credit;
                    c += subject.Credit;
                    hasScore = true;
                }

                if (hasScore && c > decimal.Zero)
                {
                    if (!_scoreCache.ContainsKey(examID))
                        _scoreCache.Add(examID, null);
                    _scoreCache[examID] = s / c;
                    return _scoreCache[examID];
                }
                else
                    return null;
            }
        }

        public decimal? GetAssignmentScore(string examID)
        {
            if (_assignmentScoreCacheExamIDs.Contains(examID) && _assignmentScoreCache.ContainsKey(examID))
                return _assignmentScoreCache[examID];
            else
            {
                _assignmentScoreCacheExamIDs.Add(examID);
                decimal s = decimal.Zero;
                decimal c = decimal.Zero;
                bool hasScore = false;

                foreach (var subject in Subjects.Values)
                {
                    if (!subject.Scores.ContainsKey(examID)) continue;
                    ScoreData data = subject.Scores[examID];

                    if (!data.AssignmentScore.HasValue) continue;

                    s += data.AssignmentScore.Value * subject.Credit;
                    c += subject.Credit;
                    hasScore = true;
                }

                if (hasScore && c > decimal.Zero)
                {
                    if (!_assignmentScoreCache.ContainsKey(examID))
                        _assignmentScoreCache.Add(examID, null);
                    _assignmentScoreCache[examID] = s / c;
                    return _assignmentScoreCache[examID];
                }
                else
                    return null;
            }
        }

        public decimal? GetFinalScore(string examID)
        {
            decimal? score = GetScore(examID);
            decimal? assignmentScore = GetAssignmentScore(examID);

            if (score.HasValue && assignmentScore.HasValue)
                return (score + assignmentScore) / 2;
            else if (score.HasValue)
                return score.Value;
            else if (assignmentScore.HasValue)
                return assignmentScore.Value;
            else
                return null;
        }

        public DomainRow(string name)
        {
            DomainName = name;
            Subjects = new Dictionary<string, SubjectRow>();
            Display = false;
            ExamIDs = new List<string>();
            _printPeriod = true;
            _printCredit = true;

            _scoreCache = new Dictionary<string, decimal?>();
            _scoreCacheExamIDs = new List<string>();

            _assignmentScoreCache = new Dictionary<string, decimal?>();
            _assignmentScoreCacheExamIDs = new List<string>();
        }

        internal void AddSubject(string subject)
        {
            if (!Subjects.ContainsKey(subject))
            {
                Subjects.Add(subject, new SubjectRow(subject));
            }
        }

        internal void AddExamID(string examID)
        {
            if (!ExamIDs.Contains(examID))
                ExamIDs.Add(examID);
        }

        internal void SetConfig(Config config)
        {
            _printPeriod = config.PrintPeriod;
            _printCredit = config.PrintCredit;
        }
    }

    class SubjectRow
    {
        //public bool Display { get; set; }
        public string SubjectName { get; set; }
        public string PeriodCredit
        {
            get
            {
                if (_printPeriod && _printCredit)
                    return _pc.ToString();
                else if (_printPeriod)
                    return _pc.Period.ToString();
                else if (_printCredit)
                    return _pc.Credit.ToString();
                else
                    return string.Empty;
            }
        }
        public Dictionary<string, ScoreData> Scores;
        public decimal? GetFinalScore(string examID)
        {
            if (Scores.ContainsKey(examID))
            {
                ScoreData sd = Scores[examID];
                if (sd.Score.HasValue && sd.AssignmentScore.HasValue)
                    return (sd.Score + sd.AssignmentScore) / 2;
                else if (sd.Score.HasValue)
                    return sd.Score.Value;
                else if (sd.AssignmentScore.HasValue)
                    return sd.AssignmentScore.Value;
                else
                    return null;
            }
            return null;
        }
        //public string Text { get; set; }

        private bool _printPeriod;
        private bool _printCredit;
        private PeriodCredit _pc;

        public SubjectRow(string name)
        {
            SubjectName = name;
            //Display = false;
            _printPeriod = true;
            _printCredit = true;
            _pc = new PeriodCredit();

            Scores = new Dictionary<string, ScoreData>();
        }

        public void AddScore(string examID, decimal? score, decimal? assignmentScore)
        {
            if (!Scores.ContainsKey(examID))
                Scores.Add(examID, new ScoreData());
            ScoreData data = Scores[examID];
            data.Score = score;
            data.AssignmentScore = assignmentScore;
        }

        public void SetConfig(Config config)
        {
            _printPeriod = config.PrintPeriod;
            _printCredit = config.PrintCredit;
        }

        internal void SetPeriodCredit(decimal? period, decimal? credit)
        {
            PeriodCredit pc = new PeriodCredit();
            if (period.HasValue) pc.Period = period.Value;
            if (credit.HasValue) pc.Credit = credit.Value;
            _pc = pc;
        }

        internal decimal Period { get { return _pc.Period; } }
        internal decimal Credit { get { return _pc.Credit; } }
    }

    class ScoreData
    {
        public decimal? Score { get; set; }
        public decimal? AssignmentScore { get; set; }
    }
}
