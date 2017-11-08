using System.Collections.Generic;
using System.Xml;
using JHSchool.Behavior.BusinessLogic;
using JHSchool.Data;

namespace JHSchool.Evaluation.Calculation.GraduationConditions
{
    /// <summary>
    ///  判斷所有學期的缺曠節數是否小於上課時數的某百分比
    /// </summary>
    internal class AbsenceAmountGraduateFractionEval : IEvaluative
    {
        private EvaluationResult _result;
        private Dictionary<string, decimal> _types;
        private Dictionary<string, int> _periodMapping;
        private decimal _amount = 100;
        private int _dayPeriod;

        /// <summary>
        /// XML參數建構式
        /// <![CDATA[
        /// <條件 Checked="True" Type="AbsenceAmountEachFraction" 假別="一般:事假,病假,曠課" 節數="1/2"/>
        /// ]]>
        /// </summary>
        /// <param name="element"></param>
        public AbsenceAmountGraduateFractionEval(XmlElement element)
        {
            _result = new EvaluationResult();
            _periodMapping = new Dictionary<string, int>();
            _types = new Dictionary<string, decimal>();

            foreach (JHPeriodMappingInfo info in JHSchool.Data.JHPeriodMapping.SelectAll())
            {
                if (!_periodMapping.ContainsKey(info.Type))
                    _periodMapping.Add(info.Type, 0);
                _periodMapping[info.Type]++;
            }

            List<string> typeList = new List<string>();
            string types = element.GetAttribute("假別");
            foreach (string typeline in types.Split(';'))
            {
                if (typeline.Split(':').Length != 2) continue;

                string type = typeline.Split(':')[0];

                decimal weight = 1;
                if (type.EndsWith(")") && type.Contains("("))
                {
                    type = type.Substring(0, type.Length - 1);
                    //weight = type.Split('(')[1];
                    if (!decimal.TryParse(type.Split('(')[1], out weight))
                        weight = 1;
                    type = type.Split('(')[0];
                }

                typeList.Add(type);
                foreach (string absence in typeline.Split(':')[1].Split(','))
                    _types.Add(type + ":" + absence, weight);
            }

            _dayPeriod = 0;
            foreach (string type in typeList)
            {
                if (_periodMapping.ContainsKey(type))
                    _dayPeriod += _periodMapping[type];
            }     

            string amount = element.GetAttribute("節數");
            if (amount.Contains("/"))
            {
                decimal f1 = decimal.Parse(amount.Split('/')[0]);
                decimal f2 = decimal.Parse(amount.Split('/')[1]);

                _amount = f1 / f2 * 100;
            }
            else if (amount.EndsWith("%"))
                _amount = decimal.Parse(amount.Substring(0, amount.Length - 1));
           
        }

        public Dictionary<string, bool> Evaluate(IEnumerable<StudentRecord> list)
        {
            _result.Clear();

            Dictionary<string, bool> passList = new Dictionary<string, bool>();
           
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
                decimal schoolDay = 0;
                // 取得學生上課日期
                foreach (K12.Data.SemesterHistoryItem item in UIConfig._StudentSHistoryRecDict[student.ID].SemesterHistoryItems)
                {
                        if (item.SchoolDayCount.HasValue)
                        {
                            decimal num = (decimal)item.SchoolDayCount.Value;
                            num *= _dayPeriod;
                            num = num * _amount / 100;
                            schoolDay += num;
                        }
                }

                if (!morals.ContainsKey(student.ID)) continue;
                decimal counter = 0;
                foreach (AutoSummaryRecord record in morals[student.ID])
                {                 
                    foreach (AbsenceCountRecord acRecord in record.AbsenceCounts)
                    {

                        if (!_types.ContainsKey(acRecord.PeriodType + ":" + acRecord.Name)) continue;

                        counter += acRecord.Count * _types[acRecord.PeriodType + ":" + acRecord.Name];
                    }
                }

                List<ResultDetail> resultList = new List<ResultDetail>();

                if (counter >= schoolDay)
                {
                    ResultDetail rd = new ResultDetail(student.ID, "0", "0");
                    rd.AddMessage("上課天數不足");
                    rd.AddDetail("上課天數不足(累計" + counter + "節)");
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
    }
}
