using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KH_StudentScoreSummaryReport
{
    class StudRankScore
    {
        /// <summary>
        /// 學生系統編號
        /// </summary>
        public string StudentID { get; set; }

        /// <summary>
        /// 成績
        /// </summary>
        public Campus.Rating.Place Place { get; set; }

    }
}
