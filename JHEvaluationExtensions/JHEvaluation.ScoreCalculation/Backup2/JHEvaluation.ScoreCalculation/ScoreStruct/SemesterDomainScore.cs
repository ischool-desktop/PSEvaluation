using System;
using System.Collections.Generic;
using System.Text;
using K12.Data;

namespace JHEvaluation.ScoreCalculation.ScoreStruct
{
    /// <summary>
    /// 領域成績。
    /// </summary>
    public class SemesterDomainScore : IScore
    {
        public SemesterDomainScore()
        {
            RawScore = null;
            Effort = null;
            Text = string.Empty;
            Period = null;
            Value = null;
            Weight = null;
        }

        public SemesterDomainScore(DomainScore score)
        {
            RawScore = score;
            Effort = score.Effort;
            Text = score.Text;
            Period = score.Period;
            Value = score.Score;
            Weight = score.Credit;
        }

        public DomainScore RawScore { get; private set; }

        /// <summary>
        /// 努力程度。
        /// </summary>
        public int? Effort { get; set; }

        /// <summary>
        /// 文字描述。
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 節數。
        /// </summary>
        public decimal? Period { get; set; }

        #region IScore 成員

        /// <summary>
        /// 原始成績。
        /// </summary>
        public decimal? Value { get; set; }

        /// <summary>
        /// 權重(Credit)。
        /// </summary>
        public decimal? Weight { get; set; }

        #endregion
    }

}
