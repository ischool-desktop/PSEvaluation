using System;
using System.Collections.Generic;
using System.Text;

namespace JHEvaluation.ScoreCalculation.ScoreStruct
{
    public class GraduateScore : IScore
    {
        #region IScore 成員

        /// <summary>
        /// 成績。
        /// </summary>
        public decimal? Value { get; set; }

        /// <summary>
        /// 權重。
        /// </summary>
        public decimal? Weight { get; set; }

        #endregion
    }

}
