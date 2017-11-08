using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KH_StudentScoreSummaryReport
{
    /// <summary>
    /// 計算用
    /// </summary>
    public class itemCount
    {
        /// <summary>
        /// 學年度
        /// </summary>
        public int SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        public int Semester { get; set; }

        /// <summary>
        /// 計數
        /// </summary>
        public int Count = 0;
    }
}
