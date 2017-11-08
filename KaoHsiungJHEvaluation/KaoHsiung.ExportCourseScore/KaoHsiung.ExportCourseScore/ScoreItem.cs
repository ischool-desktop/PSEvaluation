using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaoHsiung.ExportCourseScore
{
    /// <summary>
    /// 成績項目
    /// </summary>
    public class ScoreItem
    {
        /// <summary>
        /// 成績名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分數
        /// </summary>
        public decimal? Score { get; set; }

    }
}
