using System.Collections.Generic;
using System.Xml;
using JHSchool.Behavior.BusinessLogic;
using JHSchool.Data;

namespace JHSchool.Evaluation.Calculation.GraduationConditions
{
    /// <summary>
    /// 判斷最後一學期的缺曠節數是否小於上課時數的某百分比
    /// </summary>
    internal class AbsenceAmountLastFractionEval : IEvaluative
    {
        private EvaluationResult _result;
        private Dictionary<string, decimal> _types, _typeWeight;
        //private Dictionary<string, int> _periodMapping;
        List<string> _SelectedType;
        private decimal _amount = 100;
        private decimal _dayPeriod;

        /// <summary>
        /// XML參數建構式
        /// <![CDATA[
        /// <條件 Checked="True" Type="AbsenceAmountEachFraction" 假別="一般:事假,病假,曠課" 節數="1/2"/>
        /// ]]>
        /// </summary>
        /// <param name="element"></param>
        public AbsenceAmountLastFractionEval(XmlElement element)
        {
            _result = new EvaluationResult();
            _types = new Dictionary<string, decimal>();
            _typeWeight = new Dictionary<string, decimal>();

            //_periodMapping = new Dictionary<string, int>();
            //foreach (JHPeriodMappingInfo info in JHSchool.Data.JHPeriodMapping.SelectAll())
            //{
            //    if (!_periodMapping.ContainsKey(info.Type))
            //        _periodMapping.Add(info.Type, 0);
            //    _periodMapping[info.Type]++;
            //}

            //每日節數
            _dayPeriod = 7;
            decimal d;
            if (decimal.TryParse(element.GetAttribute("每日節數"), out d))
                _dayPeriod = d;

            _SelectedType = new List<string>();
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

                _SelectedType.Add(type);
                foreach (string absence in typeline.Split(':')[1].Split(','))
                {
                    _types.Add(type + ":" + absence, weight);

                    //各節次type的權重
                    if (!_typeWeight.ContainsKey(type))
                    {
                        _typeWeight.Add(type, weight);
                    }
                }
                    
            }

            //_dayPeriod = 0;
            //foreach (string type in _SelectedType)
            //{
            //    if (_periodMapping.ContainsKey(type))
            //        _dayPeriod += _periodMapping[type];
            //}

            //foreach (int gy in new List<int>(_gradeYearMapping.Keys))
            //{
            //    decimal num = _gradeYearMapping[gy];
            //    num = num * dayPeriod;
            //    num = num * _amount / 100;
            //    _gradeYearMapping[gy] = num;
            //}

            string amount = element.GetAttribute("節數");
            if (amount.Contains("/"))
            {
                decimal f1 = decimal.Parse(amount.Split('/')[0]);
                decimal f2 = decimal.Parse(amount.Split('/')[1]);

                _amount = f1 / f2 * 100;
            }
            else if (amount.EndsWith("%"))
                _amount = decimal.Parse(amount.Substring(0, amount.Length - 1));
            //SchoolDayCountG1
            //SCHOOL_HOLIDAY_CONFIG_STRING
            //CONFIG_STRING
            //<條件 Checked="True" Type="AbsenceAmountEachFraction"
            //      假別="一般:事假,病假,曠課" 節數="1/2"/>
        }

        #region IEvaluative 成員

        Dictionary<string, bool> IEvaluative.Evaluate(IEnumerable<StudentRecord> list)
        {
            _result.Clear();

            Dictionary<string, bool> passList = new Dictionary<string, bool>();

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
            //Dictionary<string, JHSchool.Data.JHSemesterHistoryRecord> studentHistories = new Dictionary<string, JHSchool.Data.JHSemesterHistoryRecord>();
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

            //    studentHistories.Add(rec.RefStudentID, rec);
            //}


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

            foreach (StudentRecord student in list)
            {
                passList.Add(student.ID, true);

                Dictionary<SemesterInfo, int> gyMapping = new Dictionary<SemesterInfo, int>();
                Dictionary<SemesterInfo, decimal> schoolDayMapping = new Dictionary<SemesterInfo, decimal>();
                foreach (K12.Data.SemesterHistoryItem item in UIConfig._StudentSHistoryRecDict[student.ID].SemesterHistoryItems)
                {
                    SemesterInfo info = new SemesterInfo();
                    info.SchoolYear = item.SchoolYear;
                    info.Semester = item.Semester;
                    if (!gyMapping.ContainsKey(info))
                    {
                        gyMapping.Add(info, item.GradeYear);

                        if (item.SchoolDayCount.HasValue)
                        {
                            decimal num = (decimal)item.SchoolDayCount.Value;

                            //設定臨界值
                            decimal count = 0;

                            count += num * _dayPeriod;
                            //foreach (string type in _SelectedType)
                            //{
                            //    count += num * _periodMapping[type] * _typeWeight[type];
                            //}
                            count = count * _amount / 100;
                            schoolDayMapping.Add(info, count);

                            //num *= _dayPeriod;
                            //num = num * _amount / 100;
                            //schoolDayMapping.Add(info, num);
                        }
                    }
                }

                if (!morals.ContainsKey(student.ID)) continue;
                Dictionary<SemesterInfo, decimal> counter = new Dictionary<SemesterInfo, decimal>();
                foreach (AutoSummaryRecord record in morals[student.ID])
                {
                    SemesterInfo info = new SemesterInfo();
                    info.SchoolYear = record.SchoolYear;
                    info.Semester = record.Semester;

                    if (gyMapping.ContainsKey(info) &&
                        !((gyMapping[info] == 3 || gyMapping[info] == 9) && info.Semester == 2))
                        continue;

                    //foreach (XmlElement itemElement in record.Summary.SelectNodes("AttendanceStatistics/Absence"))
                    foreach (AbsenceCountRecord acRecord in record.AbsenceCounts)
                    {
                        //string name = itemElement.GetAttribute("Name");
                        //string periodType = itemElement.GetAttribute("PeriodType");

                        if (!_types.ContainsKey(acRecord.PeriodType + ":" + acRecord.Name)) continue;

                        //decimal count;
                        //if (!decimal.TryParse(itemElement.GetAttribute("Count"), out count))
                        //    count = 0;
                        //else
                        //    count *= _types[periodType + ":" + name];

                        if (!counter.ContainsKey(info))
                            counter.Add(info, 0);
                        counter[info] += acRecord.Count * _types[acRecord.PeriodType + ":" + acRecord.Name];
                    }
                }

                List<ResultDetail> resultList = new List<ResultDetail>();
                foreach (SemesterInfo info in counter.Keys)
                {
                    if (!gyMapping.ContainsKey(info)) continue;
                    if (!schoolDayMapping.ContainsKey(info)) continue;
                    if (counter[info] > schoolDayMapping[info])
                    {
                        ResultDetail rd = new ResultDetail(student.ID, "" + gyMapping[info], "" + info.Semester);
                        rd.AddMessage("上課天數不足");
                        rd.AddDetail("上課天數不足(累計" + counter[info] + "節)");
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
    }
}
