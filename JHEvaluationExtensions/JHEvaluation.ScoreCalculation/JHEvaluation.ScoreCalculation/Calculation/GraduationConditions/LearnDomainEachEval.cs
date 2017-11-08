using System.Collections.Generic;
using System.Xml;
using JHSchool.Data;
using System;

namespace JHSchool.Evaluation.Calculation.GraduationConditions
{
    /// <summary>
    /// 學習領域成績是否符合每學期條件
    /// </summary>
    internal class LearnDomainEachEval : IEvaluative
    {
        private EvaluationResult _result;
        private int _domain_count = 0;
        private decimal _score = 0;

        /// <summary>
        /// XML參數建構式
        /// <![CDATA[ 
        /// <條件 Checked="True" Type="LearnDomainEach" 學習領域="2" 等第="丙"/>
        /// ]]>
        /// </summary>
        /// <param name="element"></param>
        public LearnDomainEachEval(XmlElement element)
        {
            _result = new EvaluationResult();

            _domain_count = int.Parse(element.GetAttribute("學習領域"));
            string degree = element.GetAttribute("等第");

            //ConfigData cd = School.Configuration["等第對照表"];
            //if (!string.IsNullOrEmpty(cd["xml"]))
            //{
            //    XmlElement xml = XmlHelper.LoadXml(cd["xml"]);
            //    XmlElement scoreMapping = (XmlElement)xml.SelectSingleNode("ScoreMapping[@Name=\"" + degree + "\"]");
            //    decimal d;
            //    if (scoreMapping != null && decimal.TryParse(scoreMapping.GetAttribute("Score"), out d))
            //        _score = d;
            //}

            JHSchool.Evaluation.Mapping.DegreeMapper mapper = new JHSchool.Evaluation.Mapping.DegreeMapper();
            decimal? d = mapper.GetScoreByDegree(degree);
            if (d.HasValue) _score = d.Value;

            //<條件 Checked="True" Type="LearnDomainEach" 學習領域="2" 等第="丙"/>
        }

        #region IEvaluative 成員

        public Dictionary<string, bool> Evaluate(IEnumerable<StudentRecord> list)
        {
            _result.Clear();

            Dictionary<string, bool> passList = new Dictionary<string, bool>();

            //Dictionary<string, SemesterHistoryUtility> shList = new Dictionary<string, SemesterHistoryUtility>();
            //foreach (Data.JHSemesterHistoryRecord shRecord in Data.JHSemesterHistory.SelectByStudentIDs(list.AsKeyList().ToArray()))
            //{
            //    if (!shList.ContainsKey(shRecord.RefStudentID))
            //        shList.Add(shRecord.RefStudentID, new SemesterHistoryUtility(shRecord));
            //}

            //list.SyncSemesterScoreCache();
            Dictionary<string, List<Data.JHSemesterScoreRecord>> studentSemesterScoreCache = new Dictionary<string, List<JHSchool.Data.JHSemesterScoreRecord>>();
            foreach (Data.JHSemesterScoreRecord record in Data.JHSemesterScore.SelectByStudentIDs(list.AsKeyList()))
            {
                if (!studentSemesterScoreCache.ContainsKey(record.RefStudentID))
                    studentSemesterScoreCache.Add(record.RefStudentID, new List<JHSchool.Data.JHSemesterScoreRecord>());
                studentSemesterScoreCache[record.RefStudentID].Add(record);
            }
            
            foreach (StudentRecord each in list)
            {
                List<ResultDetail> resultList = new List<ResultDetail>();
                //SemesterHistoryUtility shUtil = shList[each.ID];
                JHSemesterHistoryRecord shRec= new JHSemesterHistoryRecord ();
                if(UIConfig._StudentSHistoryRecDict.ContainsKey(each.ID ))
                    shRec = UIConfig._StudentSHistoryRecDict[each.ID];                


                // 有成績學年度學期
                List<string> hasSemsScoreSchoolYearSemester = new List<string>();

                // 有學期成績
                if (studentSemesterScoreCache.ContainsKey(each.ID))
                {
                    foreach (Data.JHSemesterScoreRecord record in studentSemesterScoreCache[each.ID])
                    {
                        hasSemsScoreSchoolYearSemester.Add(record.SchoolYear.ToString() + record.Semester.ToString());

                        // 領域有及格的數量
                        int count = 0;

                        //2017/5/9 穎驊修正 ，因應 高雄 [08-05][03] 畢業資格判斷成績及格標準調整 項目，
                        // 領域 分數超過60分 ，以 四捨五入取到小數第二位 ， 低於60分 採用 無條件進位至整數 (EX : 59.01 =60)
                        // (只有高雄版有如此機制，新竹版照舊不管分數高低都是四捨五入)
                        if (JHEvaluation.ScoreCalculation.Program.Mode == JHEvaluation.ScoreCalculation.ModuleMode.HsinChu)
                        {
                            foreach (K12.Data.DomainScore domain in record.Domains.Values)
                            {
                                if (domain.Score.HasValue)
                                {
                                    if (domain.Score.Value >= _score)
                                    {
                                        count++;
                                    }
                                }
                            }
                        }
                        if (JHEvaluation.ScoreCalculation.Program.Mode == JHEvaluation.ScoreCalculation.ModuleMode.KaoHsiung)
                        {
                            foreach (K12.Data.DomainScore domain in record.Domains.Values)
                            {
                                if (domain.Score.HasValue)
                                {
                                    if (domain.Score.Value >= 60 && domain.Score.Value >= _score)
                                    {
                                        count++;
                                    }
                                    if (domain.Score.Value < 60 && Math.Ceiling(domain.Score.Value) >= _score)
                                    {
                                        count++;
                                    }
                                }
                            }
                        }

                        // 當及格數小於標準數量，當學期標示不符格畢業規範
                        if (count < _domain_count)
                        {
                            // 以學期歷程的年級為主
                            int intGrYear = 0;
                            foreach (K12.Data.SemesterHistoryItem shi in shRec.SemesterHistoryItems)
                                if (shi.SchoolYear == record.SchoolYear && shi.Semester == record.Semester)
                                    intGrYear = shi.GradeYear;

                            ResultDetail rd = new ResultDetail(each.ID, "" + intGrYear, "" + record.Semester);
                            rd.AddMessage("學期領域成績不符合畢業規範");
                            rd.AddDetail("學期領域成績不符合畢業規範");
                            resultList.Add(rd);
                        }
                    }
                }
             
                // 檢查有學期歷程沒有成績
                    foreach (K12.Data.SemesterHistoryItem shi in shRec.SemesterHistoryItems)
                    {
                        if(!hasSemsScoreSchoolYearSemester.Contains(shi.SchoolYear.ToString ()+ shi.Semester.ToString ()))
                        {
                            ResultDetail rd = new ResultDetail(each.ID, shi.GradeYear.ToString (), shi.Semester.ToString ());
                            rd.AddMessage("學期領域成績資料缺漏");
                            rd.AddDetail("學期領域成績資料缺漏");
                            resultList.Add(rd);
                        }                     
                    }

                if (resultList.Count > 0)
                {
                    _result.Add(each.ID, resultList);
                    passList.Add(each.ID, false);
                }
                else
                    passList.Add(each.ID, true);
            }

            return passList;
        }

        public EvaluationResult Result
        {
            get { return _result; }
        }

        #endregion
    }
}
