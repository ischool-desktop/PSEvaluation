using System.Collections.Generic;
using System.Xml;
using JHSchool.Behavior.BusinessLogic;

namespace JHSchool.Evaluation.Calculation.GraduationConditions
{
    /// <summary>
    /// 判斷最後一學期的缺曠節數是否小於某個值
    /// </summary>
    internal class AbsenceAmountLastEval : IEvaluative
    {
        private EvaluationResult _result;
        private Dictionary<string, decimal> _types;
        private int _amount = 0;

        /// <summary>
        /// XML參數建構式
        /// <![CDATA[
        /// <條件 Checked="True" Type="AbsenceAmountEach" 假別="一般:曠課" 節數="50"/>
        /// ]]>
        /// </summary>
        /// <param name="element"></param>
        public AbsenceAmountLastEval(XmlElement element)
        {
            _result = new EvaluationResult();
            _types = new Dictionary<string, decimal>();

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
                foreach (string absence in typeline.Split(':')[1].Split(','))
                    _types.Add(type + ":" + absence, weight);
            }

            _amount = int.Parse(element.GetAttribute("節數"));

            //<條件 Checked="True" Type="AbsenceAmountEach" 假別="一般:曠課" 節數="50"/>
        }

        #region IEvaluative 成員

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

            //foreach (Data.JHMoralScoreRecord moral in Data.JHMoralScore.SelectByStudentIDs(list.AsKeyList()))
            //{
            //    if (!morals.ContainsKey(moral.RefStudentID))
            //        morals.Add(moral.RefStudentID, new List<JHSchool.Data.JHMoralScoreRecord>());
            //    morals[moral.RefStudentID].Add(moral);
            //}

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

                // 產生學期歷程對照
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
                    if (gyMapping.ContainsKey(info) && counter[info] >= _amount)
                    {
                        ResultDetail rd = new ResultDetail(student.ID, "" + gyMapping[info], "" + info.Semester);
                        rd.AddMessage("缺課節數超次");
                        rd.AddDetail("缺課節數超次(累計" + counter[info] + "節)");
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
