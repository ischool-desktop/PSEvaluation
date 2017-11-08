using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHEvaluation.Rating
{
    /// <summary>
    /// 代表成績項目，例如科目、領域…。
    /// </summary>
    internal struct ScoreItem
    {
        public ScoreItem(string name, ScoreType type)
            : this()
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// 名稱。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 成績群組。
        /// </summary>
        public ScoreType Type { get; set; }
    }

    internal enum ScoreType
    {
        /// <summary>
        /// 科目。
        /// </summary>
        Subject,
        /// <summary>
        /// 領域。
        /// </summary>
        Domain,
        /// <summary>
        /// 概括型領域，例：學習領域、課程學習。
        /// </summary>
        SummaryDomain
    }
}
