using System.Collections.Generic;
using System.Text;
using System.Xml;
using JHSchool.Behavior.BusinessLogic;

namespace JHSchool.Evaluation.Calculation.GraduationConditions
{
    /// <summary>
    /// 所有學期功過相抵條件
    /// </summary>
    internal class DemeritAmountGraduateEval:IEvaluative
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
        public DemeritAmountGraduateEval(XmlElement element)
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

        public Dictionary<string, bool> Evaluate(IEnumerable<StudentRecord> list)
        {
            _result.Clear();

            Dictionary<string, bool> passList = new Dictionary<string, bool>();

            list.SyncSemesterHistoryCache();

            Dictionary<string, List<AutoSummaryRecord>> morals = new Dictionary<string, List<AutoSummaryRecord>>();
            foreach (AutoSummaryRecord record in AutoSummary.Select(list.AsKeyList(), null))
            {
                if (!morals.ContainsKey(record.RefStudentID))
                    morals.Add(record.RefStudentID, new List<AutoSummaryRecord>());
                morals[record.RefStudentID].Add(record);
            }

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

                if (!morals.ContainsKey(student.ID)) continue;
                string keyStr = "所有學期";
                Dictionary<string, DisTestABC> counter = new Dictionary<string, DisTestABC>();
                foreach (AutoSummaryRecord record in morals[student.ID])
                {
                    if (!counter.ContainsKey(keyStr))
                        counter.Add(keyStr, new DisTestABC());

                    if (_balance)
                    {
                        decimal total_merit = _meritConverter.BtoC(_meritConverter.AtoB(record.MeritA) + record.MeritB) + record.MeritC;
                        decimal total_demerit = _demeritConverter.BtoC(_demeritConverter.AtoB(record.DemeritA) + record.DemeritB) + record.DemeritC;
                        total_demerit -= total_merit;
                        if (total_demerit > 0)
                        {
                            counter[keyStr].ABC = _demeritConverter.Change(total_demerit);
                        }
                        total_demerit = _demeritConverter.BtoA(_demeritConverter.CtoB(total_demerit));
                        counter[keyStr].demeritINT = total_demerit;
                    }
                    else
                    {
                        decimal total_demerit = record.DemeritA + _demeritConverter.BtoA(record.DemeritB) + _demeritConverter.BtoA(_demeritConverter.CtoB(record.DemeritC));
                        decimal total_demerit_ABC = _demeritConverter.BtoC(_demeritConverter.AtoB(record.DemeritA) + record.DemeritB) + record.DemeritC;
                        if (total_demerit_ABC > 0)
                        {
                            counter[keyStr].ABC = _demeritConverter.Change(total_demerit_ABC);
                        }
                        counter[keyStr].demeritINT = total_demerit;
                    }
                }

                List<ResultDetail> resultList = new List<ResultDetail>();
                if (counter[keyStr].demeritINT >= _amount)
                {
                    ResultDetail rd = new ResultDetail(student.ID, "0", "0");
                    rd.AddMessage("懲戒表現不符合畢業規範");
                    rd.AddDetail("懲戒表現不符合畢業規範(" + counter[keyStr].ABC + ")");
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
            public string ABC { get; set; }
            /// <summary>
            /// 懲戒運算值
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

                //警告換算為小過支數
                int a1 = decimal.ToInt32(k) / decimal.ToInt32(_btoc);

                //餘數為警告支數
                int a2 = decimal.ToInt32(k) % decimal.ToInt32(_btoc);

                //小過換算為大過支數
                int b1 = a1 / decimal.ToInt32(_atob);

                //餘數為小過支數
                int b2 = a1 % decimal.ToInt32(_atob);

                sb.Append("大過" + b1.ToString());
                sb.Append("小過" + b2.ToString());
                sb.Append("警告" + a2.ToString());
                return sb.ToString();
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
