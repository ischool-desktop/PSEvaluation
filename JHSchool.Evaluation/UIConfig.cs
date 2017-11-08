using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data ;

namespace JHSchool.Evaluation
{
    public class UIConfig
    {
        /// <summary>
        /// 使用者設定畫面存取學年度(學期歷程)
        /// </summary>
        public static int _UserSetSHSchoolYear=0;
        /// <summary>
        /// 使用者畫面存取學期(學期歷程)
        /// </summary>
        public static int _UserSetSHSemester=0;

        /// <summary>
        /// 學生使用的學期歷程
        /// </summary>
        public static Dictionary<string, JHSemesterHistoryRecord> _StudentSHistoryRecDict = new Dictionary<string, JHSemesterHistoryRecord>();
    }
}
