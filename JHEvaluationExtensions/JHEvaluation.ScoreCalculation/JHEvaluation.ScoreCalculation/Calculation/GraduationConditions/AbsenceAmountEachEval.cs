using System.Collections.Generic;
using System.Xml;
using JHSchool.Behavior.BusinessLogic;

namespace JHSchool.Evaluation.Calculation.GraduationConditions
{
    /// <summary>
    /// 判斷每學期的缺曠節數是否小於某個值
    /// </summary>
    internal class AbsenceAmountEachEval : IEvaluative
    {
        private EvaluationResult _result;
        private Dictionary<string, decimal> _types;
        private int _amount = 0;

        /// <summary>
        /// XML參數建構式
        /// <![CDATA[ 
        /// <條件 Checked="True" Type="AbsenceAmountEach" 假別="一般:事假,病假,曠課" 節數="50"/>
        /// ]]>
        /// </summary>
        /// <param name="element"></param>
        public AbsenceAmountEachEval(XmlElement element)
        {
            _result = new EvaluationResult();
            _types = new Dictionary<string, decimal>();

            //從XML文件中取得假別屬性
            string types = element.GetAttribute("假別");

            #region 最後組成缺曠類別+缺曠名稱，對到權重；細部規格要再看
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
            #endregion

            //從XML文件中取得節數屬性
            _amount = int.Parse(element.GetAttribute("節數"));            
        }

        #region IEvaluative 成員

        /// <summary>
        /// 根據學生判斷是否符合條件
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Dictionary<string, bool> Evaluate(IEnumerable<StudentRecord> list)
        {
            _result.Clear();

            //學生通過或不通過列表
            Dictionary<string, bool> passList = new Dictionary<string, bool>();

            list.SyncSemesterHistoryCache();

            Dictionary<string, List<AutoSummaryRecord>> morals = new Dictionary<string, List<AutoSummaryRecord>>();

            //取得學生的自動缺曠獎懲統計
            foreach (AutoSummaryRecord record in AutoSummary.Select(list.AsKeyList(), null))
            {
                if (!morals.ContainsKey(record.RefStudentID))
                    morals.Add(record.RefStudentID, new List<AutoSummaryRecord>());
                morals[record.RefStudentID].Add(record);
            }

            
            foreach (StudentRecord student in list)
            {
                //先假設每位學生通過判斷
                passList.Add(student.ID, true);

                #region 針對每位學生產生學年度學期對學年度對照
                Dictionary<SemesterInfo, int> gyMapping = new Dictionary<SemesterInfo, int>();
                if (UIConfig._StudentSHistoryRecDict.ContainsKey(student.ID))
                {
                    //針對學生的每筆學期歷程
                    foreach (K12.Data.SemesterHistoryItem shi in UIConfig._StudentSHistoryRecDict[student.ID].SemesterHistoryItems)
                    {
                        //建立學年度學期物件，並將學期歷程的學年度及學期複製過去
                        SemesterInfo info = new SemesterInfo();
                        info.SchoolYear = shi.SchoolYear;
                        info.Semester = shi.Semester;

                        //將學年度學期對年級加入到集合中
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
                #endregion

                if (!morals.ContainsKey(student.ID)) continue;

                Dictionary<SemesterInfo, decimal> counter = new Dictionary<SemesterInfo, decimal>();

                #region 針對學生的每筆自動缺曠獎懲統計，統計缺曠節數
                foreach (AutoSummaryRecord record in morals[student.ID])
                {
                    SemesterInfo info = new SemesterInfo();
                    info.SchoolYear = record.SchoolYear;
                    info.Semester = record.Semester;

                    //針對每筆缺曠名稱的統計
                    foreach (AbsenceCountRecord acRecord in record.AbsenceCounts)
                    {
                        //要缺曠類別在成績計算規則中才統計
                        if (!_types.ContainsKey(acRecord.PeriodType + ":" + acRecord.Name)) continue;

                        //將自動獎懲缺曠統計放入到變數中
                        if (!counter.ContainsKey(info))
                            counter.Add(info, 0);
                        //加總缺曠節次
                        counter[info] += acRecord.Count * _types[acRecord.PeriodType + ":" + acRecord.Name];
                    }
                }
                #endregion

                List<ResultDetail> resultList = new List<ResultDetail>();
                
                foreach (SemesterInfo info in counter.Keys)
                {
                    //判斷統計的數字是否大於成績計算規則設定的數字
                    if (gyMapping.ContainsKey(info) && counter[info] >= _amount)
                    {
                        #region 加入回傳訊息
                        ResultDetail rd = new ResultDetail(student.ID, "" + gyMapping[info], "" + info.Semester);
                        rd.AddMessage("缺課節數超次");
                        rd.AddDetail("缺課節數超次(累計" + counter[info] + "節)");
                        resultList.Add(rd);
                        #endregion
                    }
                }

                //若ResultDetail的列表項目大於0
                if (resultList.Count > 0)
                {
                    //將ResultDetail加入到回傳結果中
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