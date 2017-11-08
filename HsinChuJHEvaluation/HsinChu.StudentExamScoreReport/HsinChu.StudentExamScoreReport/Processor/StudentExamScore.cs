using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using HsinChu.JHEvaluation.Data;
using JHSchool.Evaluation;
using JHSchool.Data;
using JHSchool.Evaluation.Calculation;
using System.Linq;

namespace HsinChu.StudentExamScoreReport.Processor
{
    internal class StudentExamScore
    {
        private DocumentBuilder _builder;
        private Config _config;
        private Dictionary<string, JHCourseRecord> _courseDict;
        // 科目與課程對應
        private Dictionary<string, JHCourseRecord> _SubjCourseDict;
        public bool PrintText { get; set; }
        public bool PrintPeriod { get; set; }
        public bool PrintCredit { get; set; }

        /// <summary>
        /// 定期評量
        /// </summary>
        public bool PrintScore { get; set;}

        /// <summary>
        /// 平時評量
        /// </summary>
        public bool PrintAssScore { get; set; }

        /// <summary>
        /// 定期學習評量總成績
        /// </summary>
        public bool PrintTotalScore { get; set; }

        private DomainManager _manager;
        private ScoreCalculator _calculator;

        /// <summary>
        /// 科目成績加權平均
        /// </summary>
        public decimal SubjAvgScore = 0;

        /// <summary>
        /// 科目平時量加權平均
        /// </summary>
        public decimal SubjAvgAssignmentScore = 0;
        /// <summary>
        /// 領域平時量加權平均
        /// </summary>
        public decimal DomainAvgAssignmentScore = 0;
        /// <summary>
        /// 科目學習總成績加權平均
        /// </summary>
        public decimal SubjAvgFinalScore=0;
        /// <summary>
        /// 領域學習總成績加權平均
        /// </summary>
        public decimal DomainAvgFinalScore = 0;

        /// <summary>
        /// 科目成績加權總分
        /// </summary>
        public decimal SubjSumScore = 0;
        /// <summary>
        /// 科目學分加總
        /// </summary>
        public decimal SubjSumCredit = 0;

        /// <summary>
        /// 科目平時評量加總
        /// </summary>
        public decimal SubjSumCreditAss = 0;

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

        /// <summary>
        /// 領域平時學分加總
        /// </summary>
        public decimal DomainSumCreditAss = 0;

        /// <summary>
        /// 科目平時評量加權總分
        /// </summary>
        public decimal SubjSumAssignmentScore = 0;

        /// <summary>
        /// 領域平時評量加權總分
        /// </summary>
        public decimal DomainSumAssignmentScore = 0;

        /// <summary>
        /// 科目學習總成績加權總分
        /// </summary>
        public decimal SubjSumFinalScore = 0;

        /// <summary>
        /// 科目學習總成績加權學分
        /// </summary>
        public decimal SubjSumFinalCredit = 0;

        /// <summary>
        /// 領域學習總成績加權總分
        /// </summary>
        public decimal DomainSumFinalScore = 0;

        /// <summary>
        /// 領域學習總成績加權學分數
        /// </summary>
        public decimal DomainSumFinalCredit = 0;

        // TODO: 有風險
        private List<string> _deleteSubjectList;

        public StudentExamScore(DocumentBuilder builder, Config config, Dictionary<string, JHCourseRecord> courseDict)
        {
            _builder = builder;
            _config = config;
            _courseDict = courseDict;
            PrintText = true;

            _deleteSubjectList = new List<string>();
            _SubjCourseDict = new Dictionary<string, JHCourseRecord>();
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

        public void SetData(List<HC.JHSCETakeRecord> sces)
        {
            foreach (var sce in sces)
                _manager.Add(sce);

            FillScore();
        }

        private void RemoveNoScoreRows()
        {
            //foreach (DomainRow row in _manager.Domains.Values)
            //{
            //    SubjectRow r;
            //    r.FinalScore
            //}
            _manager.RemoveNoScoreRows(_deleteSubjectList);
        }

        private void FillScore()
        {
            RemoveNoScoreRows();

            // 建立對照
            foreach (JHCourseRecord cr in _courseDict.Values)
            {
                if (!_SubjCourseDict.ContainsKey(cr.Subject))
                    _SubjCourseDict.Add(cr.Subject, cr);            
            }

            //_SubjCourseDict = _courseDict.Values.Distinct().ToDictionary(x => x.Subject);

            _builder.MoveToMergeField("成績");
            Cell indexCell = _builder.CurrentParagraph.ParentNode as Cell;

            try
            {
                //排序
                List<string> domains = new List<string>(_manager.Domains.Keys);
                domains.Sort(JHSchool.Evaluation.Subject.CompareDomainOrdinal);

                foreach (var domainName in _manager.Domains.Keys)
                {
                    DomainRow domain = _manager.Domains[domainName];


                    bool display = false;
                    if (_config.PrintDomains.ContainsKey(domain.DomainName))
                    {
                        display = CheckDisplayDomain(domain);
                     //   display = _config.PrintDomains[domain.DomainName];
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
                           // WordHelper.MergeHorizontalCell(temp, 2);
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
                if (SubjSumCredit > 0)
                {
                    // 定期
                    //SubjAvgScore = Math.Round(SubjSumScore / SubjSumCredit, 2, MidpointRounding.AwayFromZero);
                    SubjAvgScore = _calculator.ParseSubjectScore(SubjSumScore / SubjSumCredit);
                }

                if (SubjSumCreditAss > 0)
                {
                    // 平時
                    SubjAvgAssignmentScore = _calculator.ParseSubjectScore(SubjSumAssignmentScore / SubjSumCreditAss);
                }                
                    // 總分
                 if (SubjSumFinalCredit>0)
                        SubjAvgFinalScore = _calculator.ParseSubjectScore(SubjSumFinalScore/SubjSumFinalCredit);


                // 算領域加權平均
                if (DomainSumCredit > 0)
                {
                    //DomainAvgScore = Math.Round(DomainSumScore / DomainSumCredit, 2, MidpointRounding.AwayFromZero);
                    DomainAvgScore = _calculator.ParseDomainScore(DomainSumScore / DomainSumCredit);
                }
                if(DomainSumCreditAss>0)
                {
                    DomainAvgAssignmentScore = _calculator.ParseDomainScore(DomainSumAssignmentScore / DomainSumCreditAss);
                }
                if(DomainSumFinalCredit>0)
                        DomainAvgFinalScore = _calculator.ParseDomainScore((DomainSumFinalScore)/DomainSumFinalCredit);               

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int WriteDomainRow(Cell indexCell, DomainRow domain)
        {
            try
            {
                // 判斷領域名稱是否需要顯示
                bool DisplayDomain = CheckDisplayDomain(domain);
        
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
                        
                        // 依照成績計算規則設定
                        // 定期評量
                        if (PrintScore)
                        {
                            temp = temp.NextSibling as Cell;
                            if (row.Score.HasValue)
                                Write(temp, "" + _calculator.ParseSubjectScore(row.Score.Value));
                            else
                                Write(temp, "");
                            
                        }

                        // 平時評量
                        if (PrintAssScore)
                        {
                            temp = temp.NextSibling as Cell;
                            if (row.AssignmentScore.HasValue)
                                Write(temp, "" + _calculator.ParseSubjectScore(row.AssignmentScore.Value));
                            else
                                Write(temp, "");
                            
                        }

                        // 定期學習評量總成績
                        if (PrintTotalScore)
                        {
                            temp = temp.NextSibling as Cell;
                            if (row.FinalScore.HasValue)
                                Write(temp, "" + _calculator.ParseSubjectScore(row.FinalScore.Value));
                            else
                                Write(temp, "");
                        }

                        // 文字評語
                        if (PrintText)
                        {
                            temp = temp.NextSibling as Cell;
                            Write(temp, "" + row.Text);
                        }

                        subjectCell = WordHelper.GetMoveDownCell(subjectCell, 1);
                        count++;
                        
                        // 定期評量加權平均
                        if (row.Score.HasValue)
                        {
                            SubjSumScore += (row.Score.Value * row.Credit);
                            SubjSumCredit += row.Credit;
                        }

                        // 平時評量加權平均
                        if (row.AssignmentScore.HasValue)
                        {
                            SubjSumAssignmentScore += (row.AssignmentScore.Value * row.Credit);
                            SubjSumCreditAss += row.Credit;
                        }
                        // 學習總成績加權平均
                        if (row.FinalScore.HasValue)
                        {
                            SubjSumFinalScore += (row.FinalScore.Value * row.Credit);
                            SubjSumFinalCredit += row.Credit;
                        }
                        if (subjectCell == null) break;
                    }
                }
                return count;
            }
            catch (Exception)
            {

                throw;
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
                                break;
                            }
                    }
                }
            return retVal;
        }

        /// <summary>
        /// 處理領域顯示
        /// </summary>
        /// <param name="indexCell"></param>
        /// <param name="domain"></param>
        private void WriteDomainRowOnly(Cell indexCell, DomainRow domain)
        {
            try
            {
                // 判斷領域名稱是否需要顯示
                bool DisplayDomain1 = CheckDisplayDomain(domain);

                if (DisplayDomain1)
                {
                    // 當領域為空白時需要另外處理
                    if (string.IsNullOrEmpty(domain.DomainName))
                    {
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
                                Cell temp1 = subjectCell;
                                Write(temp1, row.SubjectName);
                                temp1 = temp1.NextSibling as Cell;
                                Write(temp1, GetPCDisplay(row.PeriodCredit));
                                
                                // 依照成績計算規則
                                // 定期評量
                                if (PrintScore)
                                {
                                    temp1 = temp1.NextSibling as Cell;
                                    if (row.Score.HasValue)
                                        Write(temp1, "" + _calculator.ParseSubjectScore(row.Score.Value));
                                    else
                                        Write(temp1, "");
                                }

                                // 平時評量
                                if (PrintAssScore)
                                {
                                    temp1 = temp1.NextSibling as Cell;
                                    if (row.AssignmentScore.HasValue)
                                        Write(temp1, "" + _calculator.ParseSubjectScore(row.AssignmentScore.Value));
                                    else
                                        Write(temp1, "");
                                }
                                // 定期學習評量總成績
                                if (PrintTotalScore)
                                {
                                    temp1 = temp1.NextSibling as Cell;
                                    if (row.FinalScore.HasValue)
                                        Write(temp1, "" + _calculator.ParseSubjectScore(row.FinalScore.Value));
                                    else
                                        Write(temp1, "");
                                }

                                // 文字評語
                                if (PrintText)
                                {
                                    temp1 = temp1.NextSibling as Cell;
                                    Write(temp1, "" + row.Text);
                                }

                                subjectCell = WordHelper.GetMoveDownCell(subjectCell, 1);
                                count++;

                                // 算領域加權平均
                                if (row.Score.HasValue)
                                {
                                    DomainSumScore += row.Score.Value * row.Credit;
                                    DomainSumCredit += row.Credit;
                                }

                                // 算領域平時評量加權平均
                                if (row.AssignmentScore.HasValue)
                                {
                                    DomainSumAssignmentScore += row.AssignmentScore.Value * row.Credit;
                                    DomainSumCreditAss += row.Credit;
                                }

                                // 算領域學習總分加權平均
                                if (row.FinalScore.HasValue)
                                {
                                    DomainSumFinalScore += row.FinalScore.Value * row.Credit;
                                    DomainSumFinalCredit += row.Credit;
                                }
                                if (subjectCell == null) break;
                            }
                        }
                        WordHelper.MergeVerticalCell(indexCell, count);
                        Write(indexCell, "彈性課程");                        
                    }
                    else
                    {
                        WordHelper.MergeHorizontalCell(indexCell, 2);
                        Write(indexCell, domain.DomainName);
                        Cell temp = indexCell.NextSibling as Cell;
                        temp = temp.NextSibling as Cell;
                        Write(temp, GetPCDisplay(domain.PeriodCredit));

                        // 定期評量
                        if (PrintScore)
                        {
                            temp = temp.NextSibling as Cell;
                            Write(temp, (domain.Score.HasValue ? "" + _calculator.ParseDomainScore(domain.Score.Value) : ""));
                        }

                        // 平時評量
                        if (PrintAssScore)
                        {
                            temp = temp.NextSibling as Cell;
                            Write(temp, (domain.AssignmentScore.HasValue ? "" + _calculator.ParseDomainScore(domain.AssignmentScore.Value) : ""));
                        }

                        // 定期學習評量總成績
                        if (PrintTotalScore)
                        {
                            temp = temp.NextSibling as Cell;
                            Write(temp, (domain.FinalScore.HasValue ? "" + _calculator.ParseDomainScore(domain.FinalScore.Value) : ""));
                        }

                        // 文字評語
                        if (PrintText)
                        {
                            temp = temp.NextSibling as Cell;
                            Write(temp, domain.Text);
                        }

                        // 算領域加權平均
                        if (domain.Score.HasValue)
                        {
                            decimal crd;
                            if (decimal.TryParse(GetPCDisplay(domain.PeriodCredit), out crd))
                            {
                                DomainSumScore += (domain.Score.Value * crd);
                                DomainSumCredit += crd;
                            }
                        }

                        // 算領域平時評量加權平均
                        if (domain.AssignmentScore.HasValue)
                        {
                            decimal crd;
                            if (decimal.TryParse(GetPCDisplay(domain.PeriodCredit), out crd))
                            {
                                DomainSumAssignmentScore += (domain.AssignmentScore.Value * crd);
                                DomainSumCreditAss += crd;
                            }
                        }

                        // 算領域學習總成績加權平均
                        if (domain.FinalScore.HasValue)
                        {
                            decimal crd;
                            if (decimal.TryParse(GetPCDisplay(domain.PeriodCredit), out crd))
                            {
                                DomainSumFinalScore += (domain.FinalScore.Value * crd);
                                DomainSumFinalCredit += crd;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
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
            WordHelper.Write(cell, text, _builder);
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

        internal void Add(HC.JHSCETakeRecord sce)
        {
            if (!_courseDict.ContainsKey(sce.RefCourseID)) return;
            JHCourseRecord course = _courseDict[sce.RefCourseID];

            if (!Domains.ContainsKey(course.Domain))
                Domains.Add(course.Domain, new DomainRow(course.Domain));

            DomainRow row = Domains[course.Domain];
            if (!row.Subjects.ContainsKey(course.Subject))
                row.AddSubject(course.Subject);

            SubjectRow subjectRow = row.Subjects[course.Subject];
            subjectRow.RefAssessmentSetupID = course.RefAssessmentSetupID;

            subjectRow.SetPeriodCredit(course.Period, course.Credit);
            subjectRow.Score = sce.Score;
            subjectRow.AssignmentScore = sce.AssignmentScore;
            subjectRow.Text = sce.Text;
            subjectRow.Display = true;
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
            
            foreach (var domain in config.PrintSubjects.Keys)
            {
                if (!Domains.ContainsKey(domain))
                    Domains.Add(domain, new DomainRow(domain));

                DomainRow domainRow = Domains[domain];
                domainRow.Display = config.PrintDomains[domain];
                foreach (var subject in config.PrintSubjects[domain])
                {
                    domainRow.AddSubject(subject);
                    domainRow.Subjects[subject].Display = true;
                }
            }
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
        public decimal? Score
        {
            get
            {
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
                    return s / c;
                else
                    return null;
            }
        }
        public decimal? AssignmentScore
        {
            get
            {
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
                    return s / c;
                else
                    return null;
            }
        }
        public decimal? FinalScore
        {
            get
            {
                decimal s = decimal.Zero;
                decimal c = decimal.Zero;
                bool hasScore = false;

                foreach (var subject in Subjects.Values)
                {
                    if (!subject.FinalScore.HasValue) continue;

                    s += subject.FinalScore.Value * subject.Credit;
                    c += subject.Credit;
                    hasScore = true;
                }

                if (hasScore && c > decimal.Zero)
                    return s / c;
                else
                    return null;
            }
        }
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

        private decimal? _assignment_score;
        public decimal? AssignmentScore
        {
            get { return _assignment_score; }
            set
            {
                _assignment_score = value;
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

        public decimal? FinalScore
        {
            get
            {
                if (Score.HasValue && AssignmentScore.HasValue)
                {
                    // 使用評量設定比例，如果沒有預設 50,50
                    if (Utility.ScorePercentageHSDict.ContainsKey(RefAssessmentSetupID))
                    {
                        decimal f = Score.Value * Utility.ScorePercentageHSDict[RefAssessmentSetupID] * 0.01M;
                        decimal a = AssignmentScore.Value * (100 - Utility.ScorePercentageHSDict[RefAssessmentSetupID]) * 0.01M;
                        return f + a;
                    }
                    else
                        return Score.Value * 0.5M + AssignmentScore.Value * 0.5M;

                    //return (Score + AssignmentScore) / 2;
                }
                else if (Score.HasValue)
                    return Score.Value;
                else if (AssignmentScore.HasValue)
                    return AssignmentScore.Value;
                else
                    return null;
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

        /// <summary>
        /// 評量樣版ID
        /// </summary>
        public string RefAssessmentSetupID { get; set; }
    }
}
