using System.Collections.Generic;
using System.Text;
using System.Xml;
using JHSchool.Behavior.BusinessLogic;

namespace JHSchool.Evaluation.Calculation.GraduationConditions
{
    /// <summary>
    /// 最後學期功過相抵條件
    /// </summary>
    internal class DemeritAmountLastEval : IEvaluative
    {
        private EvaluationResult _result;
        private decimal _amount = 0;
        private bool _balance = true;
        private DisciplineConverter _meritConverter;
        private DisciplineConverter _demeritConverter;

        /// <summary>
        /// XML參數建構式
        /// <![CDATA[ 
        /// <條件 Checked="True" Type="DemeritAmountLast" 大過="3" 功過相抵="True" 獎勵換算="1:3,1:3" 懲戒換算="1:3,1:3"/>
        /// ]]>
        /// </summary>
        /// <param name="element"></param>
        public DemeritAmountLastEval(XmlElement element)
        {
            _result = new EvaluationResult();
            _amount = decimal.Parse(element.GetAttribute("大過"));
            bool b;
            _balance = bool.TryParse(element.GetAttribute("功過相抵"), out b) ? b : true;

            _meritConverter = new DisciplineConverter(element.GetAttribute("獎勵換算"));
            _demeritConverter = new DisciplineConverter(element.GetAttribute("懲戒換算"));
            //<條件 Checked="True" Type="DemeritAmountLast"
            //      大過="3" 功過相抵="True" 獎勵換算="1:3,1:3" 懲戒換算="1:3,1:3"/>
        }

        #region IEvaluative 成員

        Dictionary<string, bool> IEvaluative.Evaluate(IEnumerable<StudentRecord> list)
        {
            _result.Clear();

            Dictionary<string, bool> passList = new Dictionary<string, bool>();

            list.SyncSemesterHistoryCache();
            //Dictionary<string, List<Data.JHMoralScoreRecord>> morals = new Dictionary<string, List<JHSchool.Data.JHMoralScoreRecord>>();
            //foreach (Data.JHMoralScoreRecord moral in Data.JHMoralScore.SelectByStudentIDs(list.AsKeyList()))
            //{
            //    if (!morals.ContainsKey(moral.RefStudentID))
            //        morals.Add(moral.RefStudentID, new List<JHSchool.Data.JHMoralScoreRecord>());
            //    morals[moral.RefStudentID].Add(moral);
            //}
            Dictionary<string, List<AutoSummaryRecord>> morals = new Dictionary<string, List<AutoSummaryRecord>>();
            foreach (AutoSummaryRecord record in AutoSummary.Select(list.AsKeyList(), null))
            {
                if (!morals.ContainsKey(record.RefStudentID))
                    morals.Add(record.RefStudentID, new List<AutoSummaryRecord>());
                morals[record.RefStudentID].Add(record);
            }

            //// 取得學生目前班級年級
            //Dictionary<string, int> studGrYearDic = new Dictionary<string, int>();
            //foreach (JHSchool.Data.JHStudentRecord stud in JHSchool.Data.JHStudent.SelectByIDs(list.AsKeyList()))
            //{
            //    if (stud.Class != null)
            //        if (stud.Class.GradeYear.HasValue)
            //            studGrYearDic.Add(stud.ID, stud.Class.GradeYear.Value);
            //}

            //bool checkInsShi = false;

            //// 取得學生學期歷程
            //Dictionary<string, JHSchool.Data.JHSemesterHistoryRecord> studHisRecDic = new Dictionary<string, JHSchool.Data.JHSemesterHistoryRecord>();
            //foreach (JHSchool.Data.JHSemesterHistoryRecord rec in JHSchool.Data.JHSemesterHistory.SelectByStudentIDs(list.AsKeyList()))
            //{
            //    checkInsShi = true;
            //    K12.Data.SemesterHistoryItem shi = new K12.Data.SemesterHistoryItem();
            //    shi.SchoolYear = UIConfig._UserSetSHSchoolYear;
            //    shi.Semester = UIConfig._UserSetSHSemester;
            //    if (studGrYearDic.ContainsKey(rec.RefStudentID))
            //        shi.GradeYear = studGrYearDic[rec.RefStudentID];

            //    foreach (K12.Data.SemesterHistoryItem shiItem in rec.SemesterHistoryItems)
            //        if (shiItem.SchoolYear == shi.SchoolYear && shiItem.Semester == shi.Semester)
            //            checkInsShi = false;
            //    if (checkInsShi)
            //        rec.SemesterHistoryItems.Add(shi);

            //    studHisRecDic.Add(rec.RefStudentID, rec);
            //}

            foreach (StudentRecord student in list)
            {
                passList.Add(student.ID, true);

                Dictionary<SemesterInfo, int> gyMapping = new Dictionary<SemesterInfo, int>();
                if (UIConfig._StudentSHistoryRecDict.ContainsKey(student.ID))
                {
                    foreach (K12.Data.SemesterHistoryItem shi in UIConfig._StudentSHistoryRecDict[student.ID].SemesterHistoryItems)
                    {
                        SemesterInfo info = new SemesterInfo();
                        info.SchoolYear = shi.SchoolYear;
                        info.Semester = shi.Semester;
                        if (!gyMapping.ContainsKey(info))
                        {
                            gyMapping.Add(info, shi.GradeYear);
                        }
                        else
                        {
                            FISCA.Presentation.Controls.MsgBox.Show("學生：" + UIConfig._StudentSHistoryRecDict[student.ID].Student.Name + "　學期歷程重覆");
                        }
                    }
                }

                //foreach (SemesterHistoryRecord record in student.GetSemesterHistories())
                //{
                //    SemesterInfo info = new SemesterInfo();
                //    info.SchoolYear = record.SchoolYear;
                //    info.Semester = record.Semester;
                //    if (!gyMapping.ContainsKey(info))
                //        gyMapping.Add(info, record.GradeYear);
                //}

                if (!morals.ContainsKey(student.ID)) continue;
                Dictionary<SemesterInfo, DisTestABC> counter = new Dictionary<SemesterInfo, DisTestABC>();
                foreach (AutoSummaryRecord record in morals[student.ID])
                {
                    SemesterInfo info = new SemesterInfo();
                    info.SchoolYear = record.SchoolYear;
                    info.Semester = record.Semester;

                    if (gyMapping.ContainsKey(info) &&
                        !((gyMapping[info] == 3 || gyMapping[info] == 9) && info.Semester == 2))
                        continue;

                    if (!counter.ContainsKey(info))
                        counter.Add(info, new DisTestABC());

                    //decimal meritA = 0, meritB = 0, meritC = 0;
                    //decimal demeritA = 0, demeritB = 0, demeritC = 0;

                    //foreach (XmlElement itemElement in record.Summary.SelectNodes("DisciplineStatistics/Merit"))
                    //{
                    //    decimal merit;
                    //    if (decimal.TryParse(itemElement.GetAttribute("A"), out merit))
                    //        meritA = merit;
                    //    if (decimal.TryParse(itemElement.GetAttribute("B"), out merit))
                    //        meritB = merit;
                    //    if (decimal.TryParse(itemElement.GetAttribute("C"), out merit))
                    //        meritC = merit;
                    //}

                    //foreach (XmlElement itemElement in record.Summary.SelectNodes("DisciplineStatistics/Demerit"))
                    //{
                    //    decimal demerit;
                    //    if (decimal.TryParse(itemElement.GetAttribute("A"), out demerit))
                    //        demeritA = demerit;
                    //    if (decimal.TryParse(itemElement.GetAttribute("B"), out demerit))
                    //        demeritB = demerit;
                    //    if (decimal.TryParse(itemElement.GetAttribute("C"), out demerit))
                    //        demeritC = demerit;
                    //}

                    if (_balance)
                    {
                        //counter[info].DemeritA += record.DemeritA;
                        //counter[info].DemeritB += record.DemeritB;
                        //counter[info].DemeritC += record.DemeritC;
                        //counter[info].ABC = "大過:" + record.DemeritA.ToString() + "小過:" + record.DemeritB.ToString() + "警告:" + record.DemeritC.ToString();
                        decimal total_merit = _meritConverter.BtoC(_meritConverter.AtoB(record.MeritA) + record.MeritB) + record.MeritC;
                        decimal total_demerit = _demeritConverter.BtoC(_demeritConverter.AtoB(record.DemeritA) + record.DemeritB) + record.DemeritC;
                        total_demerit -= total_merit;
                        if (total_demerit > 0)
                        {
                            counter[info].ABC = _demeritConverter.Change(total_demerit);
                        }
                        total_demerit = _demeritConverter.BtoA(_demeritConverter.CtoB(total_demerit));
                        counter[info].demeritINT = total_demerit;
                    }
                    else
                    {
                        //counter[info].ABC = "大過:" + record.DemeritA.ToString() + "小過:" + record.DemeritB.ToString() + "警告:" + record.DemeritC.ToString();
                        decimal total_demerit = record.DemeritA+ _demeritConverter.BtoA(record.DemeritB) + _demeritConverter.BtoA(_demeritConverter.CtoB(record.DemeritC));
                        decimal total_demerit_ABC = _demeritConverter.BtoC(_demeritConverter.AtoB(record.DemeritA) + record.DemeritB) + record.DemeritC;
                        if (total_demerit_ABC > 0)
                        {
                            counter[info].ABC = _demeritConverter.Change(total_demerit_ABC);
                        }
                        counter[info].demeritINT = total_demerit;
                    }
                }

                List<ResultDetail> resultList = new List<ResultDetail>();
                foreach (SemesterInfo info in counter.Keys)
                {
                    if (gyMapping.ContainsKey(info) && counter[info].demeritINT >= _amount)
                    {
                        ResultDetail rd = new ResultDetail(student.ID, "" + gyMapping[info], "" + info.Semester);
                        rd.AddMessage("懲戒表現不符合畢業規範");
                        rd.AddDetail("懲戒表現不符合畢業規範(" + counter[info].ABC + ")");
                        //rd.AddDetail("懲戒表現不符合畢業規範(合計" + decimal.Round(counter[info], 2, MidpointRounding.AwayFromZero) + "大過)");
                        resultList.Add(rd);
                    }
                }

                if (resultList.Count > 0)
                {
                    _result.Add(student.ID, resultList);
                    passList[student.ID] = false;
                }
            }

            return passList;
        }

        public EvaluationResult Result
        {
            get { return _result; }
        }

        #endregion

        private class DisTestABC
        {
            /// <summary>
            /// 獎懲字串
            /// </summary>
            public string ABC { get; set; }

            ///// <summary>
            ///// 大過
            ///// </summary>
            //public int DemeritA { get; set; }
            ///// <summary>
            ///// 小過
            ///// </summary>
            //public int DemeritB { get; set; }
            ///// <summary>
            ///// 警告
            ///// </summary>
            //public int DemeritC { get; set; }

            /// <summary>
            /// 獎懲換算值
            /// </summary>
            public decimal demeritINT { get; set; }
        }

        private class DisciplineConverter
        {
            private decimal _atob = 3;
            private decimal _btoc = 3;

            public DisciplineConverter(string s)
            {
                string atob = s.Split(',')[0];
                string btoc = s.Split(',')[1];

                decimal atob1 = decimal.Parse(atob.Split(':')[0]);
                decimal atob2 = decimal.Parse(atob.Split(':')[1]);
                _atob = atob2 / atob1;

                decimal btoc1 = decimal.Parse(btoc.Split(':')[0]);
                decimal btoc2 = decimal.Parse(btoc.Split(':')[1]);
                _btoc = btoc2 / btoc1;
            }

            public string Change(decimal k)
            {
                StringBuilder sb = new StringBuilder();

                //警告換算為小過隻數
                int a1 = decimal.ToInt32(k) / decimal.ToInt32(_btoc);

                //餘數為警告隻數
                int a2 = decimal.ToInt32(k) % decimal.ToInt32(_btoc);

                //小過換算為大過隻數
                int b1 = a1 / decimal.ToInt32(_atob);

                //餘數為小過隻數
                int b2 = a1 % decimal.ToInt32(_atob);

                sb.Append("大過" + b1.ToString());
                sb.Append("小過" + b2.ToString());
                sb.Append("警告" + a2.ToString());
                return sb.ToString();
            }

            public decimal AtoB(decimal d)
            {
                return d * _atob;
            }

            public decimal BtoA(decimal d)
            {
                return d / _atob;
            }

            public decimal BtoC(decimal d)
            {
                return d * _btoc;
            }

            public decimal CtoB(decimal d)
            {
                return d / _btoc;
            }
        }
    }
}