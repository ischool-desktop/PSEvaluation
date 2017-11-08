//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using JHSchool.Data;
//using System.Xml;
//using K12.Data;
//using JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReportControls;

//namespace JHSchool.Evaluation.Calculation.GraduationConditions
//{
//    /// <summary>
//    /// 所有學習領域符合畢業總平均成績規範。
//    /// </summary>
//    internal class GraduateDomainEval : IEvaluative
//    {
//        private EvaluationResult _result;
//        private int _domain_count = 0;
//        private decimal _score = 0;

//        /// <summary>
//        /// XML參數建構式
//        /// <![CDATA[ 
//        /// <條件 Checked="True" Type="GraduateDomain" 學習領域="3" 等第="甲"/>
//        /// ]]>
//        /// </summary>
//        /// <param name="element"></param>
//        public GraduateDomainEval(XmlElement element)
//        {
//            _result = new EvaluationResult();

//            _domain_count = int.Parse(element.GetAttribute("學習領域"));
//            string degree = element.GetAttribute("等第");

//            JHSchool.Evaluation.Mapping.DegreeMapper mapper = new JHSchool.Evaluation.Mapping.DegreeMapper();
//            decimal? d = mapper.GetScoreByDegree(degree);
//            if (d.HasValue) _score = d.Value;
//        }

//        public Dictionary<string, bool> Evaluate(IEnumerable<StudentRecord> list)
//        {
//            _result.Clear();

//            Dictionary<string, bool> passList = new Dictionary<string, bool>();

//            Dictionary<string, List<JHSemesterScoreRecord>> studentSemesterScoreCache = new Dictionary<string, List<JHSemesterScoreRecord>>();
//            foreach (JHSemesterScoreRecord record in JHSemesterScore.SelectByStudentIDs(list.AsKeyList()))
//            {
//                if (!studentSemesterScoreCache.ContainsKey(record.RefStudentID))
//                    studentSemesterScoreCache.Add(record.RefStudentID, new List<JHSemesterScoreRecord>());
//                studentSemesterScoreCache[record.RefStudentID].Add(record);
//            }

//            //學生能被承認的學年度學期對照
//            Dictionary<string, List<string>> studentSYSM = new Dictionary<string, List<string>>();

//            #region 學期歷程
//            foreach (K12.Data.SemesterHistoryRecord shr in K12.Data.SemesterHistory.SelectByStudentIDs(list.Select(x => x.ID)))
//            {
//                if (!studentSYSM.ContainsKey(shr.RefStudentID))
//                    studentSYSM.Add(shr.RefStudentID, new List<string>());

//                Dictionary<string, K12.Data.SemesterHistoryItem> check = new Dictionary<string, K12.Data.SemesterHistoryItem>() { 
//                {"1a",null },
//                {"1b",null },
//                {"2a",null },
//                {"2b",null },
//                {"3a",null },
//                {"3b",null }
//                };

//                foreach (K12.Data.SemesterHistoryItem item in shr.SemesterHistoryItems)
//                {
//                    string grade = item.GradeYear + "";

//                    if (grade == "7") grade = "1";
//                    if (grade == "8") grade = "2";
//                    if (grade == "9") grade = "3";

//                    if (grade == "1" || grade == "2" || grade == "3")
//                    {
//                        string key = "";
//                        if (item.Semester == 1)
//                            key = grade + "a";
//                        else if (item.Semester == 2)
//                            key = grade + "b";
//                        else
//                            continue;

//                        //相同年級取較新的學年度
//                        if (check[key] == null)
//                            check[key] = item;
//                        else if (item.SchoolYear > check[key].SchoolYear)
//                            check[key] = item;
//                    }
//                }

//                foreach (string key in check.Keys)
//                {
//                    if (check[key] == null)
//                        continue;

//                    K12.Data.SemesterHistoryItem item = check[key];

//                    studentSYSM[shr.RefStudentID].Add(item.SchoolYear + "_" + item.Semester);
//                }
//            }

//            #endregion

//            foreach (StudentRecord each in list)
//            {
//                List<ResultDetail> resultList = new List<ResultDetail>();

//                // 有學期成績
//                if (studentSemesterScoreCache.ContainsKey(each.ID))
//                {
//                    // 存放畢業領域成績
//                    Dictionary<string, decimal> graduateDomainScoreDict = new Dictionary<string, decimal>();
//                    Dictionary<string, decimal> graduateDomainCreditDict = new Dictionary<string, decimal>();
//                    Dictionary<string, int> graduateDomainCountDict = new Dictionary<string, int>();
//                    // 存放符合標準畢業領域成績
//                    List<decimal> passScoreList = new List<decimal>();
//                    // 取得學生學生領域成績填入計算畢業成績用
//                    foreach (JHSemesterScoreRecord record in studentSemesterScoreCache[each.ID])
//                    {
//                        string key = record.SchoolYear + "_" + record.Semester;

//                        if (!studentSYSM.ContainsKey(each.ID) || !studentSYSM[each.ID].Contains(key))
//                            continue;

//                        decimal 語文總分 = 0;
//                        decimal 語文權重 = 0;

//                        foreach (K12.Data.DomainScore domain in record.Domains.Values)
//                        {
//                            // 國語文、英語，轉成語文領域對應
//                            string domainName = domain.Domain;

//                            if (domain.Domain == "國語文" || domain.Domain == "英語")
//                            {
//                                decimal score = domain.Score.HasValue ? domain.Score.Value : 0;
//                                decimal credit = domain.Credit.HasValue ? domain.Credit.Value : 0;

//                                語文總分 += score * credit;
//                                語文權重 += credit;

//                                continue;
//                                //domainName = "語文";

//                                // 處理高雄語文顯示
//                                // 加權總分
//                                //if (!TempData.tmpStudDomainScoreDict.ContainsKey(record.RefStudentID))
//                                //    TempData.tmpStudDomainScoreDict.Add(record.RefStudentID, new Dictionary<string, decimal>());

//                                //if (!TempData.tmpStudDomainCreditDict.ContainsKey(record.RefStudentID))
//                                //    TempData.tmpStudDomainCreditDict.Add(record.RefStudentID, new Dictionary<string, decimal>());

//                                //if (!TempData.tmpStudDomainScoreDict[record.RefStudentID].ContainsKey(domain.Domain))
//                                //    TempData.tmpStudDomainScoreDict[record.RefStudentID].Add(domain.Domain, 0);

//                                //// 學分數
//                                //if (!TempData.tmpStudDomainCreditDict[record.RefStudentID].ContainsKey(domain.Domain))
//                                //    TempData.tmpStudDomainCreditDict[record.RefStudentID].Add(domain.Domain, 0);

//                                //if (domain.Score.HasValue && domain.Credit.HasValue)
//                                //{
//                                //    TempData.tmpStudDomainScoreDict[record.RefStudentID][domain.Domain] += (domain.Score.Value * domain.Credit.Value);
//                                //    TempData.tmpStudDomainCreditDict[record.RefStudentID][domain.Domain] += domain.Credit.Value;
//                                //}
//                            }

//                            if (!graduateDomainScoreDict.ContainsKey(domainName))
//                            {
//                                graduateDomainScoreDict.Add(domainName, 0);
//                                graduateDomainCountDict.Add(domainName, 0);
//                                graduateDomainCreditDict.Add(domainName, 0);
//                            }

//                            if (domain.Score.HasValue && domain.Credit.HasValue)
//                            {
//                                graduateDomainScoreDict[domainName] += (domain.Score.Value * domain.Credit.Value);
//                                graduateDomainCreditDict[domainName] += domain.Credit.Value;
//                                graduateDomainCountDict[domainName]++;
//                            }
//                        }

//                        if (語文權重 > 0)
//                        {
//                            decimal score = Math.Round(語文總分 / 語文權重, 2, MidpointRounding.AwayFromZero);

//                            if (!graduateDomainScoreDict.ContainsKey("語文"))
//                            {
//                                graduateDomainScoreDict.Add("語文", 0);
//                                graduateDomainCountDict.Add("語文", 0);
//                                graduateDomainCreditDict.Add("語文", 0);
//                            }

//                            graduateDomainScoreDict["語文"] += score * 語文權重;
//                        }
//                    }

//                    // 即時計算畢業成績並判斷是否符合
//                    foreach (string name in graduateDomainScoreDict.Keys)
//                    {
//                        decimal grScore = 0;
//                        // if (graduateDomainCountDict[name] > 0)
//                        if (graduateDomainCreditDict[name] > 0) // 小郭, 2013/12/30
//                        {
//                            // 算術平均
//                            grScore = Math.Round(graduateDomainScoreDict[name] / graduateDomainCountDict[name], 2, MidpointRounding.AwayFromZero);

//                            // 加權平均,加權總分,加權學分
//                            //grScore = Math.Round(graduateDomainScoreDict[name] / graduateDomainCreditDict[name], 2, MidpointRounding.AwayFromZero);

//                            if (grScore >= _score)
//                                passScoreList.Add(grScore);

//                            // 小郭, 2013/12/30
//                            StudentDomainResult.AddDomain(each.ID, name, grScore, grScore >= _score);
//                        }
//                    }
//                    // 當及格數小於標準數，標示不符格畢業規範
//                    if (passScoreList.Count < _domain_count)
//                    {
//                        ResultDetail rd = new ResultDetail(each.ID, "0", "0");
//                        rd.AddMessage("領域畢業加權總平均成績不符合畢業規範");
//                        rd.AddDetail("領域畢業加權總平均成績不符合畢業規範");
//                        resultList.Add(rd);
//                    }

//                }

//                if (resultList.Count > 0)
//                {
//                    _result.Add(each.ID, resultList);
//                    passList.Add(each.ID, false);
//                }
//                else
//                    passList.Add(each.ID, true);
//            }

//            return passList;
//        }

//        public EvaluationResult Result
//        {
//            get { return _result; }
//        }
//    }
//}
