using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using KaoHsiung.JHEvaluation.Data;
using JHSchool.Evaluation;
using JHSchool.Data;
using FISCA.Presentation.Controls;
using JHSchool.Evaluation.Calculation;

namespace KaoHsiung.MidTermTransferReport.Processor
{
    internal class StudentExamScore
    {
        private DocumentBuilder _builder;
        private Font _font;
        private Config _config;
        private Dictionary<string, JHCourseRecord> _courseDict;

        private EffortMapper _effortMapper;
        private Dictionary<string, int> _columnMapping;

        private DomainManager _manager;
        private ScoreCalculator _calculator;
        private Dictionary<string, JHSCAttendRecord> _AttendDict;
        private Dictionary<string, decimal?> _DomainScoreDict;
        private Dictionary<string, decimal> _DomainScoreDictTT;

//        public StudentExamScore(DocumentBuilder builder, Config config, Dictionary<string, JHCourseRecord> courseDict, Dictionary<string, JHSCAttendRecord> attendDict, Dictionary<string, decimal> DomainScoreDict, Dictionary<string, decimal> DomainScoreDictTT)
        public StudentExamScore(DocumentBuilder builder, Config config, Dictionary<string, JHCourseRecord> courseDict, Dictionary<string, JHSCAttendRecord> attendDict, Dictionary<string, decimal?> DomainScoreDict)
        {
            _builder = builder;

            _config = config;
            _courseDict = courseDict;
            // 課程成績
            _AttendDict = attendDict;
            _manager = new DomainManager();
            _manager.SetCourseDictionary(_courseDict);
            _manager.SetConfig(_config);
            _DomainScoreDict = DomainScoreDict;
//            _DomainScoreDictTT = DomainScoreDictTT;
        }

        internal void SetEffortMapper(EffortMapper effortMapper)
        {
            _effortMapper = effortMapper;
        }

        internal void SetColumnMap(Dictionary<string, int> columnMapping)
        {
            _columnMapping = columnMapping;
        }

        public void SetData(List<KH.JHSCETakeRecord> sces)
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

            try
            {
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int WriteDomainRow(Cell indexCell, DomainRow domain)
        {
            WordHelper.Write(indexCell, _font, string.IsNullOrEmpty(domain.DomainName) ? "彈性課程" : domain.DomainName);
            int col = 0;
            Cell subjectCell = WordHelper.GetMoveRightCell(indexCell, 1);
            int count = 0;
            foreach (var subjectName in domain.Subjects.Keys)
            {
                SubjectRow row = domain.Subjects[subjectName];
                if (row.Display)
                {
                    Cell temp = subjectCell;
                    WordHelper.Write(temp, _font, row.SubjectName);
                    temp = temp.NextSibling as Cell;
                    WordHelper.Write(temp, _font, row.PeriodCredit);

                    foreach (string examID in row.Scores.Keys)
                    {
                        ScoreData data = row.Scores[examID];
                        string effortText = string.Empty;
                        if (data.Effort.HasValue) effortText = _effortMapper.GetTextByInt(data.Effort.Value);

                        temp = WordHelper.GetMoveRightCell(subjectCell, _columnMapping[examID] - 1);
                        WordHelper.Write(temp, _font, (data.Score.HasValue ? "" + _calculator.ParseSubjectScore(data.Score.Value) : ""));
                        WordHelper.Write(temp.NextSibling as Cell, _font, effortText);
                    }

                    if (_AttendDict.ContainsKey(subjectName))
                    {

                        temp = WordHelper.GetMoveRightCell(subjectCell, _columnMapping["平時評量"]-1);

                        // 平時
                        if (_AttendDict[subjectName].OrdinarilyScore.HasValue)
                            WordHelper.Write(temp, _font, _calculator.ParseSubjectScore(_AttendDict[subjectName].OrdinarilyScore.Value).ToString());
                        else
                            WordHelper.Write(temp, _font, "");

                        if (_AttendDict[subjectName].OrdinarilyEffort.HasValue)
                            WordHelper.Write(temp.NextSibling as Cell, _font, _effortMapper.GetTextByInt(_AttendDict[subjectName].OrdinarilyEffort.Value));
                        else
                            WordHelper.Write(temp.NextSibling as Cell, _font, "");


                        //temp = WordHelper.GetMoveRightCell(indexCell, _columnMapping["課程總成績"]);

                        //// 課程總成績
                        //if (_AttendDict[subjectName].Score.HasValue)
                        //    WordHelper.Write(temp, _font, _calculator.ParseSubjectScore(_AttendDict[subjectName].Score.Value).ToString());
                        //else
                        //    WordHelper.Write(temp, _font, "");

                        //if (_AttendDict[subjectName].Effort.HasValue)
                        //    WordHelper.Write(temp.NextSibling as Cell, _font, _effortMapper.GetTextByInt(_AttendDict[subjectName].Effort.Value));
                        //else
                        //    WordHelper.Write(temp.NextSibling as Cell, _font, "");
                    }
                  
                    subjectCell = WordHelper.GetMoveDownCell(subjectCell, 1);
                
                    if (subjectCell == null)
                    {
                        break;
                    }
                    count++;
                }
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
                string effortText = string.Empty;
                if (score.HasValue) effortText = _effortMapper.GetTextByDecimal(score.Value);

                temp = WordHelper.GetMoveRightCell(indexCell, _columnMapping[examID]);
                //                WordHelper.Write(temp, _font, (score.HasValue ? "" + _calculator.ParseDomainScore(score.Value) : ""));
                if(score.HasValue)
                    WordHelper.Write(temp, _font,_calculator.ParseDomainScore(score.Value).ToString ());
                else
                    WordHelper.Write(temp, _font,"");

                WordHelper.Write(temp.NextSibling as Cell, _font, effortText);
            }
          

            // 平時
            if (_DomainScoreDict.ContainsKey(domain.DomainName))
            {
                temp = WordHelper.GetMoveRightCell(indexCell, _columnMapping["平時評量"]);
                string effortText1 = string.Empty;
                // 當有值
                if (_DomainScoreDict[domain.DomainName].HasValue)
                {
                    effortText1 = _effortMapper.GetTextByDecimal(_DomainScoreDict[domain.DomainName].Value);
                    WordHelper.Write(temp, _font, _calculator.ParseDomainScore(_DomainScoreDict[domain.DomainName].Value).ToString());
                }
                else
                    WordHelper.Write(temp, _font, ""); // 沒有領域成績填空白
                
                WordHelper.Write(temp.NextSibling as Cell, _font, effortText1);            
            }
            //temp = temp.NextSibling as Cell;
            //temp = temp.NextSibling as Cell;

            //// 總
            //if (_DomainScoreDictTT.ContainsKey(domain.DomainName))
            //{
            //    temp = WordHelper.GetMoveRightCell(indexCell, _columnMapping["課程總成績"]);
            //    string effortText2 = string.Empty;
            //    effortText2 = _effortMapper.GetTextByDecimal(_DomainScoreDictTT[domain.DomainName]);

            //    WordHelper.Write(temp, _font, _calculator.ParseDomainScore(_DomainScoreDictTT[domain.DomainName]).ToString());
            //    WordHelper.Write(temp.NextSibling as Cell, _font, effortText2);
            //}

        }

        internal void SetCalculator(ScoreCalculator studentCalculator)
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
        public Dictionary<string, DomainRow> Domains { get; set; }
        private Dictionary<string, JHCourseRecord> _courseDict;
        private Config _config;

        public DomainManager()
        {
            Domains = new Dictionary<string, DomainRow>();
        }

        internal void Add(KH.JHSCETakeRecord sce)
        {
            if (!_courseDict.ContainsKey(sce.RefCourseID)) return;
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
            subjectRow.AddScore(sce.RefExamID, sce.Score, sce.Effort);
            if (_config.DomainSubjectSetup == DomainSubjectSetup.Subject)
                subjectRow.Display = true;
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
                // 濾過社團
                if (string.IsNullOrEmpty(course.Domain) && string.IsNullOrEmpty(course.Subject))
                    continue;

                string domain = course.Domain;
                string subject = course.Subject;

                if (!Domains.ContainsKey(domain))
                    Domains.Add(domain, new DomainRow(domain));

                DomainRow domainRow = Domains[domain];

                if (!domainRow.Subjects.ContainsKey(subject))
                    domainRow.AddSubject(subject);
                domainRow.Subjects[subject].SetPeriodCredit(course.Period, course.Credit);
                domainRow.Subjects[subject].Display = true;
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
        private List<string> _cached;

        public bool Display { get; set; }
        public Dictionary<string, SubjectRow> Subjects { get; set; }
        public List<string> ExamIDs;
        private bool _printPeriod;
        private bool _printCredit;

        public bool HasScore
        {
            get
            {
                foreach (string id in ExamIDs)
                {
                    decimal? score = GetScore(id);
                    if (score.HasValue) return true;
                }
                return false;
            }
        }

        public decimal? GetScore(string examID)
        {
            if (_cached.Contains(examID) && _scoreCache.ContainsKey(examID))
                return _scoreCache[examID];
            else
            {
                _cached.Add(examID);
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

        public DomainRow(string name)
        {
            DomainName = name;
            Subjects = new Dictionary<string, SubjectRow>();
            Display = false;
            ExamIDs = new List<string>();
            _printPeriod = true;
            _printCredit = true;

            _scoreCache = new Dictionary<string, decimal?>();
            _cached = new List<string>();
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
        public bool Display { get; set; }
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

        //public string Text { get; set; }

        private bool _printPeriod;
        private bool _printCredit;
        private PeriodCredit _pc;

        public SubjectRow(string name)
        {
            SubjectName = name;
            Display = false;
            _printPeriod = true;
            _printCredit = true;
            _pc = new PeriodCredit();

            Scores = new Dictionary<string, ScoreData>();
        }

        public void AddScore(string examID, decimal? score, int? effort)
        {
            if (!Scores.ContainsKey(examID))
                Scores.Add(examID, new ScoreData());
            ScoreData data = Scores[examID];
            data.Score = score;
            data.Effort = effort;
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
        public int? Effort { get; set; }
    }
}
