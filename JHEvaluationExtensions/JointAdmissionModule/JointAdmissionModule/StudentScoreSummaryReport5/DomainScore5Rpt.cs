using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JointAdmissionModule.StudentScoreSummaryReport5
{
    /// <summary>
    /// 各領域成績八上、八下、九上
    /// </summary>
    class DomainScore5Rpt
    {
        /// <summary>
        /// 領域名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 八上成績
        /// </summary>
        public decimal? Score81 { get; set; }

        /// <summary>
        /// 八下成績
        /// </summary>
        public decimal? Score82 { get; set; }

        /// <summary>
        /// 九上成績
        /// </summary>
        public decimal? Score91 { get; set; }
    }
}
