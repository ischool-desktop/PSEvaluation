using System;
using System.Linq;
using System.Collections.Generic;
using Aspose.Words;
using JHSchool.Data;
using JHSchool.Evaluation;
using JHSchool.Evaluation.Calculation;
using KaoHsiung.JHEvaluation.Data;
using Aspose.Words.Tables;

namespace KaoHsiung.StudentExamScoreReport.Processor
{
    internal class StudentExamScore
    {        
        private DocumentBuilder _builder;
        private Font _font;
        private Config _config;
        private Dictionary<string, JHCourseRecord> _courseDict;
        // 科目與課程對應
        private Dictionary<string, JHCourseRecord> _SubjCourseDict;
        
        public bool PrintText { get; set; }
        public bool PrintPeriod { get; set; }
        public bool PrintCredit { get; set; }
        public bool PrintAssignment { get; set; }
        public bool PrintScore { get; set; }
        public bool PrintEffort { get; set; }
        /// <summary>
        /// 科目成績加權平均
        /// </summary>
        public decimal SubjAvgScore = 0;
        /// <summary>
        /// 科目成績加權總分
        /// </summary>
        public decimal SubjSumScore = 0;
        /// <summary>
        /// 科目學分加總
        /// </summary>
        public decimal SubjSumCredit = 0;

        /// <summary>
        /// 領域成績加權平均
        /// </summary>
        public decimal DomainAvgScore = 0;
        /// <summary>
        /// 領域成績加權總分
        /// </summary>
        public decimal DomainSumScore = 0;
        /// <summary>
        /// 領域學分加總
        /// </summary>
        public decimal DomainSumCredit = 0;

        private EffortMapper _effortMapper;

        private DomainManager _manager;
        private ScoreCalculator _calculator;

        // TODO: 有風險
        private List<string> _deleteSubjectList;

        public StudentExamScore(DocumentBuilder builder, Config config, Dictionary<string, JHCourseRecord> courseDict)
        {
            _builder = builder;
            _font = builder.Font;
            _config = config;
            _courseDict = courseDict;
            PrintText = true;
            _SubjCourseDict = new Dictionary<string, JHCourseRecord>();
            SubjAvgScore = 0;
            SubjSumCredit = 0;
            DomainAvgScore = 0;
            DomainSumCredit = 0;
            _deleteSubjectList = new List<string>();
            _manager = new DomainManager();
            _manager.SetConfig(_config);
            _manager.SetCourseDictionary(_courseDict);

            foreach (JHCourseRecord course in courseDict.Values)
            {
                if (!_config.HasExam(course))
                {
                    if (!_deleteSubjectList.Contains(course.Subject))
                        _deleteSubjectList.Add(course.Subject);
                }                
            }
        }

        internal void SetEffortMap(EffortMapper effortMapper)
        {
            _effortMapper = effortMapper;
        }

        public void SetAssignmentData(List<JHSCAttendRecord> assignments)
        {
            foreach (JHSCAttendRecord a in assignments)
                _manager.AddAssignment(a);
        }

        public void SetData(List<KH.JHSCETakeRecord> sces)
        {
            foreach (var sce in sces)
                _manager.Add(sce);
        }

        private void RemoveNoScoreRows()
        {
            _manager.RemoveNoScoreRows(_deleteSubjectList);
        }

        public void FillScore()
        {
            //RemoveNoScoreRows();

            _builder.MoveToMergeField("成績");
            _font = _builder.Font;
            Cell indexCell = _builder.CurrentParagraph.ParentNode as Cell;

            _SubjCourseDict.Clear();
            // 建立對照
            foreach (JHCourseRecord cr in _courseDict.Values)
                if (!_SubjCourseDict.ContainsKey(cr.Subject))
                    _SubjCourseDict.Add(cr.Subject, cr);
            //_SubjCourseDict = _courseDict.Values.Distinct().ToDictionary(x=>x.Subject);

            List<string> HasExamDomain = new List<string>();
            

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
                    {
                        //// 判斷是否有考試
                        //display = _config.PrintDomains[domain.DomainName];
                        display = CheckDisplayDomain(domain);

                    }
                    else
                        display = domain.Display;

                    if (_config.DomainSubjectSetup == "Domain")
                    {
                        if (display)
                        {
                            Cell temp = indexCell;
                            indexCell = WordHelper.GetMoveDownCell(indexCell, 1);
                            WriteDomainRowOnly(temp, domain);
                            //WordHelper.MergeHorizontalCell(temp, 2);
                            if (indexCell == null) break;
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
                            if (indexCell == null) break;
                        }
                    }
                }
                // 算科目加權平均                
                if(SubjSumCredit >0)                
                    SubjAvgScore = Math.Round(SubjSumScore / SubjSumCredit,2,MidpointRounding.AwayFromZero);

                // 算領域加權平均
                if (DomainSumCredit > 0)
                    DomainAvgScore = Math.Round(DomainSumScore / DomainSumCredit, 2, MidpointRounding.AwayFromZero);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 判斷是否要顯示領域
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private bool CheckDisplayDomain(DomainRow dr)
        {
            bool retVal = false;
            foreach (var sr in dr.Subjects)
                if (_SubjCourseDict.ContainsKey(sr.Value.SubjectName))
                {
                    foreach (var subjectName in dr.Subjects.Keys)
                    {
                        if (_SubjCourseDict.ContainsKey(subjectName))
                            if (_config.HasExam(_SubjCourseDict[subjectName]))
                            {
                                retVal = true;
                                
                            }
                    }
                }
            return retVal;
        }


        private int WriteDomainRow(Cell indexCell, DomainRow domain)
        {
            // 判斷領域名稱是否需要顯示
            bool DisplayDomain = CheckDisplayDomain(domain );

            if(DisplayDomain )            
                Write(indexCell, string.IsNullOrEmpty(domain.DomainName) ? "彈性課程" : domain.DomainName);

            Cell subjectCell = WordHelper.GetMoveRightCell(indexCell, 1);
            int count = 0;
            foreach (var subjectName in domain.Subjects.Keys)
            {
                // 判斷是否需要顯示科目
                bool DisplaySubject = true;

                if (_SubjCourseDict.ContainsKey(subjectName))
                    if (_config.HasExam(_SubjCourseDict[subjectName]) == false)
                        DisplaySubject = false;

                if (DisplaySubject == false)
                    continue;

                SubjectRow row = domain.Subjects[subjectName];
                
                if (row.Display)
                {
                    Cell temp = subjectCell;
                    Write(temp, row.SubjectName);
                    temp = temp.NextSibling as Cell;
                    Write(temp, GetPCDisplay(row.PeriodCredit));

                    if (PrintScore)
                    {
                        temp = temp.NextSibling as Cell;
                        // 依照成績計算規則
                        if (row.Score.HasValue)
                            Write(temp, "" + _calculator.ParseSubjectScore(row.Score.Value));
                        else
                            Write(temp, "");
                    }
                    if (PrintEffort)
                    {
                        temp = temp.NextSibling as Cell;
                        string effortText = string.Empty;
                        if (row.Effort.HasValue) effortText = _effortMapper.GetTextByCode(row.Effort.Value);
                        Write(temp, effortText);
                    }

                    if (PrintAssignment)
                    {
                        temp = temp.NextSibling as Cell;
                        if(row.AssignmentScore.HasValue )
                            Write(temp, "" + _calculator.ParseSubjectScore(row.AssignmentScore.Value));
                        else
                            Write(temp, "");
                    }

                    if (PrintText)
                    {
                        temp = temp.NextSibling as Cell;
                        Write(temp, "" + row.Text);
                    }

                    subjectCell = WordHelper.GetMoveDownCell(subjectCell, 1);
                    count++;

                    if (row.Score.HasValue)
                    {
                        SubjSumScore += (row.Score.Value * row.Credit);
                        SubjSumCredit += row.Credit;
                    }

                    if (subjectCell == null) break;
                }
            }
            return count;
        }

        // 這段在處理只有列印領域
        private void WriteDomainRowOnly(Cell indexCell, DomainRow domain)
        {
            // 判斷領域名稱是否需要顯示
            bool DisplayDomain1 = CheckDisplayDomain(domain);

            if (DisplayDomain1)
            {
                // 當領域是空白另外處理
                if (string.IsNullOrEmpty(domain.DomainName))
                {
                    int co = 0;
                    Cell subjectCell = WordHelper.GetMoveRightCell(indexCell, 1);
                    foreach (var subjectName in domain.Subjects.Keys)
                    {
                        // 判斷是否需要顯示科目
                        bool DisplaySubject = true;

                        if (_SubjCourseDict.ContainsKey(subjectName))
                            if (_config.HasExam(_SubjCourseDict[subjectName]) == false)
                                DisplaySubject = false;

                        if (DisplaySubject == false)
                            continue;


                        SubjectRow row = domain.Subjects[subjectName];
                        

                        if (row.Display)
                        {
                            Cell temp1 = subjectCell;
                            Write(temp1, row.SubjectName);
                            temp1 = temp1.NextSibling as Cell;
                            Write(temp1, GetPCDisplay(row.PeriodCredit));
                            if (PrintScore)
                            {
                                temp1 = temp1.NextSibling as Cell;
                                // 依照成績計算規則設定
                                if (row.Score.HasValue)
                                    Write(temp1, "" + _calculator.ParseSubjectScore(row.Score.Value));
                                else
                                    Write(temp1, "");
                            }

                            if (PrintEffort)
                            {
                                temp1 = temp1.NextSibling as Cell;
                                string effortText1 = string.Empty;
                                if (row.Effort.HasValue) effortText1 = _effortMapper.GetTextByCode(row.Effort.Value);
                                Write(temp1, effortText1);
                            }
                            if (PrintAssignment)
                            {
                                temp1 = temp1.NextSibling as Cell;
                                // 依照成績計算規則設定
                                if(row.AssignmentScore.HasValue)
                                    Write(temp1, "" + _calculator.ParseSubjectScore(row.AssignmentScore.Value));
                                else
                                    Write(temp1, "");
                            }

                            if (PrintText)
                            {
                                temp1 = temp1.NextSibling as Cell;
                                Write(temp1, "" + row.Text);
                            }
                            co++;

                            // 算領域加權平均
                            if (row.Score.HasValue)
                            {

                                DomainSumScore += row.Score.Value * row.Credit;
                                DomainSumCredit += row.Credit;
                            }


                            subjectCell = WordHelper.GetMoveDownCell(subjectCell, 1);
                            if (subjectCell == null) break;
                        }
                    }
                    WordHelper.MergeVerticalCell(indexCell, co);
                    Write(indexCell, "彈性課程");
                }
                else
                {
                    WordHelper.MergeHorizontalCell(indexCell, 2);
                    Write(indexCell, domain.DomainName);

                    Cell temp = indexCell.NextSibling as Cell;
                    temp = temp.NextSibling as Cell;
                    Write(temp, GetPCDisplay(domain.PeriodCredit));

                    if (PrintScore)
                    {
                        temp = temp.NextSibling as Cell;
                        Write(temp, (domain.Score.HasValue ? "" + _calculator.ParseDomainScore(domain.Score.Value) : ""));
                    }

                    if (PrintEffort)
                    {
                        temp = temp.NextSibling as Cell;
                        string effortText = string.Empty;
                        if (domain.Score.HasValue) effortText = _effortMapper.GetTextByScore(domain.Score.Value);
                        Write(temp, effortText);
                    }

                    if (PrintAssignment)
                    {
                        temp = temp.NextSibling as Cell;
                        Write(temp, (domain.AssignmentScore.HasValue ? "" + _calculator.ParseDomainScore(domain.AssignmentScore.Value) : ""));
                    }
                    if (PrintText)
                    {
                        temp = temp.NextSibling as Cell;
                        Write(temp, "" + domain.Text);
                    }

                    // 算領域加權平均
                    if (domain.Score.HasValue )
                    {
                        decimal crd;
                        if(decimal.TryParse(GetPCDisplay(domain.PeriodCredit),out crd))
                        {
                            DomainSumScore += (domain.Score.Value * crd);
                            DomainSumCredit += crd;
                        }
                    }

                }
            }
        }

        private string GetPCDisplay(string p)
        {
            PeriodCredit pc = new PeriodCredit();
            pc.Parse(p);
            if (PrintPeriod && PrintCredit)
                return pc.ToString();
            else if (PrintPeriod)
                return "" + pc.Period;
            else if (PrintCredit)
                return "" + pc.Credit;
            else
                return string.Empty;
        }

        private void Write(Cell cell, string text)
        {
            WordHelper.Write(cell, _font, text);
        }

        internal void SetScoreCalculator(ScoreCalculator studentCalculator)
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
            if (!row.Subjects.ContainsKey(course.Subject))
                row.AddSubject(course.Subject);

            SubjectRow subjectRow = row.Subjects[course.Subject];
            subjectRow.SetPeriodCredit(course.Period, course.Credit);
            subjectRow.Score = sce.Score;
            subjectRow.Effort = sce.Effort;
            subjectRow.Text = sce.Text;
            subjectRow.Display = true;
        }

        internal void AddAssignment(JHSCAttendRecord assignment)
        {
            if (!_courseDict.ContainsKey(assignment.RefCourseID)) return;
            JHCourseRecord course = _courseDict[assignment.RefCourseID];

            if (!Domains.ContainsKey(course.Domain)) return;

            DomainRow row = Domains[course.Domain];
            if (!row.Subjects.ContainsKey(course.Subject)) return;

            SubjectRow subjectRow = row.Subjects[course.Subject];
            subjectRow.AssignmentScore = assignment.OrdinarilyScore;
        }

        internal void SetCourseDictionary(Dictionary<string, JHCourseRecord> courseDict)
        {
            _courseDict = courseDict;
            //foreach (JHCourseRecord course in _courseDict.Values)
            //{
            //    string domain = course.Domain;
            //    string subject = course.Subject;

            //    if (!Domains.ContainsKey(domain))
            //        Domains.Add(domain, new DomainRow(domain));

            //    DomainRow domainRow = Domains[domain];
            //    if (!domainRow.Subjects.ContainsKey(subject))
            //        domainRow.AddSubject(subject);
            //    domainRow.Subjects[subject].SetPeriodCredit(course.Period, course.Credit);
            //}
        }

        internal void SetConfig(Config config)
        {
            _config = config;

            //foreach (var domain in config.PrintSubjects.Keys)
            //{
            //    if (!Domains.ContainsKey(domain))
            //        Domains.Add(domain, new DomainRow(domain));

            //    DomainRow domainRow = Domains[domain];
            //    domainRow.Display = config.PrintDomains[domain];
            //    foreach (var subject in config.PrintSubjects[domain])
            //    {
            //        domainRow.AddSubject(subject);
            //        domainRow.Subjects[subject].Display = true;
            //    }
            //}
        }

        internal void RemoveNoScoreRows(List<string> deleteList)
        {
            foreach (string domainKey in this.Domains.Keys)
            {
                if (Domains[domainKey].Subjects.Count > 0)
                {
                    List<string> subjectKeyList = new List<string>();
                    foreach (string subjectKey in Domains[domainKey].Subjects.Keys)
                    {
                        if (deleteList.Contains(subjectKey))
                        {
                            if (!subjectKeyList.Contains(subjectKey))
                                subjectKeyList.Add(subjectKey);
                        }
                    }
                    foreach (string subjectKey in subjectKeyList)
                        Domains[domainKey].Subjects.Remove(subjectKey);
                }
            }

            List<string> domainKeyList = new List<string>();
            foreach (string domainKey in this.Domains.Keys)
            {
                if (Domains[domainKey].Subjects.Count <= 0)
                {
                    if (!domainKeyList.Contains(domainKey))
                        domainKeyList.Add(domainKey);
                }
            }
            foreach (string domainKey in domainKeyList)
                Domains.Remove(domainKey);
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

            
            //courseList = from course in courseList orderby course.Subject select course;
            
            foreach (JHCourseRecord course in courseList)
            {
                //if (!_courseDict.ContainsKey(courseID)) continue;
                //JHCourseRecord course = _courseDict[courseID];

                string domain = course.Domain;
                string subject = course.Subject;

                if (!Domains.ContainsKey(domain))
                    Domains.Add(domain, new DomainRow(domain));

                DomainRow domainRow = Domains[domain];
                if (_config.DomainSubjectSetup == "Subject")
                    domainRow.Display = false;

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

                return pc.ToString();
                //if (_printPeriod && _printCredit)
                //    return pc.ToString();
                //else if (_printPeriod)
                //    return pc.Period.ToString();
                //else if (_printCredit)
                //    return pc.Credit.ToString();
                //else
                //    return string.Empty;
            }
        }
        private decimal? _scoreCache;
        private bool _is_score_cached = false;

        private decimal? _assignmentCache;
        private bool _is_assignment_score_cached = false;

        public decimal? Score
        {
            get
            {
                if (_is_score_cached)
                    return _scoreCache;
                else
                {
                    _is_score_cached = true;
                    decimal s = decimal.Zero;
                    decimal c = decimal.Zero;
                    bool hasScore = false;

                    foreach (var subject in Subjects.Values)
                    {
                        if (!subject.Score.HasValue) continue;

                        s += subject.Score.Value * subject.Credit;
                        c += subject.Credit;
                        hasScore = true;
                    }

                    if (hasScore && c > decimal.Zero)
                    {
                        _scoreCache = s / c;
                        return _scoreCache;
                    }
                    else
                        return null;
                }
            }
        }

        public decimal? AssignmentScore
        {
            get
            {
                if (_is_assignment_score_cached)
                    return _assignmentCache;
                else
                {
                    _is_assignment_score_cached = true;
                    decimal s = decimal.Zero;
                    decimal c = decimal.Zero;
                    bool hasScore = false;

                    foreach (var subject in Subjects.Values)
                    {
                        if (!subject.AssignmentScore.HasValue) continue;

                        s += subject.AssignmentScore.Value * subject.Credit;
                        c += subject.Credit;
                        hasScore = true;
                    }

                    if (hasScore && c > decimal.Zero)
                    {
                        _assignmentCache = s / c;
                        return _assignmentCache;
                    }
                    else
                        return null;
                }
            }
        }

        //public string Effort
        //{
        //    get
        //    {
        //        if (_effortMapper != null)
        //        {

        //        }
        //        else
        //            return string.Empty;
        //    }
        //}

        public string Text
        {
            get
            {
                string text = string.Empty;

                foreach (var subject in Subjects.Values)
                {
                    if (string.IsNullOrEmpty(subject.Text)) continue;
                    text += subject.SubjectName + ":" + subject.Text + ",";
                }

                if (text.EndsWith(","))
                    text = text.Substring(0, text.Length - 1);
                return text;
            }
        }
        /// <summary>
        /// true 只列領域
        /// false 科目分開列
        /// </summary>
        public bool Display { get; set; }
        public Dictionary<string, SubjectRow> Subjects { get; set; }

        public DomainRow(string name)
        {
            DomainName = name;
            Subjects = new Dictionary<string, SubjectRow>();
            Display = true;
        }

        internal void AddSubject(string subject)
        {
            if (!Subjects.ContainsKey(subject))
            {
                Subjects.Add(subject, new SubjectRow(subject));
            }
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
                return _pc.ToString();
                //if (_printPeriod && _printCredit)
                //    return _pc.ToString();
                //else if (_printPeriod)
                //    return _pc.Period.ToString();
                //else if (_printCredit)
                //    return _pc.Credit.ToString();
                //else
                //    return string.Empty;
            }
        }

        private decimal? _score;
        public decimal? Score
        {
            get { return _score; }
            set
            {
                _score = value;
            }
        }

        public decimal? AssignmentScore { get; set; }

        private int? _effort;
        public int? Effort
        {
            get { return _effort; }
            set
            {
                _effort = value;
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
            }
        }
        private PeriodCredit _pc;

        public SubjectRow(string name)
        {
            SubjectName = name;
            Display = false;
            _pc = new PeriodCredit();
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
}
