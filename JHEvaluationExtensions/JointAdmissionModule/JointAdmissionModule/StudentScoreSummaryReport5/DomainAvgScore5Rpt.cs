using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JointAdmissionModule.StudentScoreSummaryReport5
{
    class DomainAvgScore5Rpt
    {
        /// <summary>
        /// 領域名稱
        /// </summary>
        public string DomainName { get; set; }
        /// <summary>
        /// 領域成績
        /// </summary>
        public decimal? DomainScore { get; set; }
        /// <summary>
        /// 領域成績(加分後)
        /// </summary>
        public decimal? DomainScoreAdd { get; set; }
        /// <summary>
        /// 領域學期成績排名百分比(int)
        /// </summary>
        public int? DomainScoreRankPercent { get; set; }

        /// <summary>
        /// 領域學期成績排名百分比加分後(int)
        /// </summary>
        public int? DomainScoreRankPercentAdd { get; set; }

    
    }
}
