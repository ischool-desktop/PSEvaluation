using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;

namespace KaoHsingReExamScoreReport.DAO
{
    public class StudentData
    {
        public string StudentID { get; set; }

        /// <summary>
        /// 學生姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 班級名稱
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 學號
        /// </summary>
        public string StudentNumber { get; set; }

        /// <summary>
        /// 年級
        /// </summary>
        public string GradeYear { get; set; }

        /// <summary>
        /// 座號
        /// </summary>
        public string SeatNo { get; set; }

        public string ClassID { get; set; }
        /// <summary>
        /// 學期成績
        /// </summary>
        public JHSemesterScoreRecord SemesterScoreRecord = new JHSemesterScoreRecord();

        /// <summary>
        /// 判斷領域成績是否及格，及格 True,
        /// </summary>
        public Dictionary<string, bool> DomainScorePassDict = new Dictionary<string, bool>();

        /// <summary>
        /// 領域成績
        /// </summary>
        public Dictionary<string, decimal> DomainScoreDict = new Dictionary<string, decimal>();

        /// <summary>
        /// 學生單學期成績
        /// </summary>
        public JHSemesterScoreRecord StudSemesterScoreRecord;

        /// <summary>
        /// 取得需要補考數
        /// </summary>
        /// <returns></returns>
        public int GetReDomainCount()
        {
            int retVal = 0;
            // 01-06 小組會議討論，國語文、英語 屬語文，只有看語文領域，共7大領域
            foreach (string name in DomainScorePassDict.Keys)
            {
                if (name == "國語文" || name == "英語")
                    continue;

                if (DomainScorePassDict[name] == false)
                    retVal++;
            }
            return retVal;
        }
    }
}
