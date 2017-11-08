using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.RatingFramework
{
    /// <summary>
    /// 排名選項。
    /// </summary>
    public enum PlaceOptions
    {
        /// <summary>
        /// 接序排名，例如：1,2,3,3,5...
        /// </summary>
        Sequence,

        /// <summary>
        /// 不接序排名，例如：1,2,3,3,4...
        /// </summary>
        Unsequence
    }
}
