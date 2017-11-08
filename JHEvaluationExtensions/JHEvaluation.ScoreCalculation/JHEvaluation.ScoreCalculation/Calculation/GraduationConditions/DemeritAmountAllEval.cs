using System.Collections.Generic;
using System.Text;
using System.Xml;
using JHSchool.Behavior.BusinessLogic;

namespace JHSchool.Evaluation.Calculation.GraduationConditions
{
    /// <summary>
    /// 所有學期功過相抵條件
    /// </summary>
    internal class DemeritAmountAllEval : IEvaluative
    {
        private EvaluationResult _result;
        private decimal _amount = 0;
        private bool _balance = true;
        private DisciplineConverter _meritConverter;
        private DisciplineConverter _demeritConverter;

        /// <summary>
        /// XML參數建構式
        /// <![CDATA[ 
        /// <條件 Checked="True" Type="DemeritAmountEach" 大過="3" 功過相抵="True" 獎勵換算="1:3,1:3" 懲戒換算="1:3,1:3"/>
        /// ]]>
        /// </summary>
        /// <param name="element"></param>
        public DemeritAmountAllEval(XmlElement element)
        {
            _result = new EvaluationResult();
            _amount = decimal.Parse(element.GetAttribute("大過"));
            bool b;
            _balance = bool.TryParse(element.GetAttribute("功過相抵"), out b) ? b : true;

            _meritConverter = new DisciplineConverter(element.GetAttribute("獎勵換算"));
            _demeritConverter = new DisciplineConverter(element.GetAttribute("懲戒換算"));
            //<條件 Checked="True" Type="DemeritAmountEach"
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

            // 獎懲明細統計
            TempData.tmpStudentDemeritAmountAllDict.Clear();

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
                    if (!counter.ContainsKey(info))
                        counter.Add(info, new DisTestABC());

                    string scStr = info.SchoolYear + "學年度第" + info.Semester + "學期";
                    if (!TempData.tmpStudentDemeritAmountAllDict.ContainsKey(student.ID))
                        TempData.tmpStudentDemeritAmountAllDict.Add(student.ID, new Dictionary<string, Dictionary<string, int>>());

                    if (!TempData.tmpStudentDemeritAmountAllDict[student.ID].ContainsKey(scStr))
                    {
                        Dictionary<string, int> value = new Dictionary<string, int>();
                        value.Add("大功", 0);
                        value.Add("小功", 0);
                        value.Add("嘉獎", 0);
                        value.Add("大過", 0);
                        value.Add("小過", 0);
                        value.Add("警告", 0);

                        TempData.tmpStudentDemeritAmountAllDict[student.ID].Add(scStr, value);
                    }

                    TempData.tmpStudentDemeritAmountAllDict[student.ID][scStr]["大功"] += record.MeritA;
                    TempData.tmpStudentDemeritAmountAllDict[student.ID][scStr]["小功"] += record.MeritB;
                    TempData.tmpStudentDemeritAmountAllDict[student.ID][scStr]["嘉獎"] += record.MeritC;
                    TempData.tmpStudentDemeritAmountAllDict[student.ID][scStr]["大過"] += record.DemeritA;
                    TempData.tmpStudentDemeritAmountAllDict[student.ID][scStr]["小過"] += record.DemeritB;
                    TempData.tmpStudentDemeritAmountAllDict[student.ID][scStr]["警告"] += record.DemeritC;

                    decimal total_merit = _meritConverter.BtoC(_meritConverter.AtoB(record.MeritA) + record.MeritB) + record.MeritC;
                    decimal total_demerit = _demeritConverter.BtoC(_demeritConverter.AtoB(record.DemeritA) + record.DemeritB) + record.DemeritC;

                    //獎懲加總
                    counter[info].Merit = total_merit;
                    counter[info].Demerit = total_demerit;
                }

                List<ResultDetail> resultList = new List<ResultDetail>();

                decimal merit = 0;
                decimal demerit = 0;
                decimal demeritINT = 0;

                //存放懲戒詳細數量
                Dictionary<string, int> ABC = new Dictionary<string, int>();
                ABC.Add("大過", 0);
                ABC.Add("小過", 0);
                ABC.Add("警告", 0);

                //加總總懲戒及各項懲戒
                foreach (SemesterInfo info in counter.Keys)
                {
                    merit += counter[info].Merit;
                    demerit += counter[info].Demerit;
                }

                if (_balance)
                {//功過相抵
                    demerit -= merit;
                    if (demerit > 0)
                    {
                        Dictionary<string,int> dic = _demeritConverter.Change(demerit);
                        ABC["大過"] += dic["大過"];
                        ABC["小過"] += dic["小過"];
                        ABC["警告"] += dic["警告"];
                    }
                    demeritINT = _demeritConverter.BtoA(_demeritConverter.CtoB(demerit));
                }
                else
                {
                    if (demerit > 0)
                    {
                        Dictionary<string, int> dic = _demeritConverter.Change(demerit);
                        ABC["大過"] += dic["大過"];
                        ABC["小過"] += dic["小過"];
                        ABC["警告"] += dic["警告"];
                    }
                    demeritINT = _demeritConverter.BtoA(_demeritConverter.CtoB(demerit));
                }

                if (demeritINT >= _amount)
                {
                    ResultDetail rd = new ResultDetail(student.ID, "0", "0");
                    rd.AddMessage("懲戒表現不符合畢業規範");
                    string str = "";
                    //取得懲戒詳細數量
                    foreach(KeyValuePair<string,int> kvp in ABC)
                    {
                        str += kvp.Key + kvp.Value;
                    }
                    rd.AddDetail("懲戒表現不符合畢業規範(" + str + ")");
                    resultList.Add(rd);
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

        private class DisTestABC
        {
            /// <summary>
            /// 懲戒字串
            /// </summary>
            public Dictionary<string,int> ABC { get; set; }
            /// <summary>
            /// 懲戒運算值
            /// </summary>
            public decimal demeritINT { get; set; }

            public decimal Merit, Demerit;
        }

        #endregion

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

            public Dictionary<string,int> Change(decimal k)
            {
                Dictionary<string, int> retVal = new Dictionary<string, int>();

                //警告換算為小過隻數
                int a1 = decimal.ToInt32(k) / decimal.ToInt32(_btoc);

                //餘數為警告隻數
                int a2 = decimal.ToInt32(k) % decimal.ToInt32(_btoc);

                //小過換算為大過隻數
                int b1 = a1 / decimal.ToInt32(_atob);

                //餘數為小過隻數
                int b2 = a1 % decimal.ToInt32(_atob);

                retVal.Add("大過",b1);
                retVal.Add("小過",b2);
                retVal.Add("警告",a2);
                return retVal;
            }

            #region decimal
            /// <summary>
            /// 大過換算為小過
            /// </summary>
            public decimal AtoB(decimal d)
            {
                return d * _atob;
            }

            /// <summary>
            /// 小過換算為大過
            /// </summary>
            public decimal BtoA(decimal d)
            {
                return d / _atob;
            }

            /// <summary>
            /// 小過換算為警告
            /// </summary>
            public decimal BtoC(decimal d)
            {
                return d * _btoc;
            }

            /// <summary>
            /// 警告換算為小過
            /// </summary>
            public decimal CtoB(decimal d)
            {
                return d / _btoc;
            }
            #endregion
        }
    }
}