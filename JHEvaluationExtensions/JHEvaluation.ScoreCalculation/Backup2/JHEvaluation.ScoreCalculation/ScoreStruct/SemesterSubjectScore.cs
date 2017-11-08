using System;
using System.Collections.Generic;
using System.Text;
using K12.Data;
using JHSchool.Data;

namespace JHEvaluation.ScoreCalculation.ScoreStruct
{
    /// <summary>
    /// 科目成績。
    /// </summary>
    public class SemesterSubjectScore : IScore
    {
        public SemesterSubjectScore()
        {
            RawScore = null;
            Effort = null;
            Text = string.Empty;
            Period = null;
            Value = null;
            Domain = string.Empty;
            Weight = null;
        }

        public SemesterSubjectScore(SubjectScore score)
        {
            RawScore = score;
            Effort = score.Effort;
            Text = score.Text;
            Period = score.Period;
            Value = score.Score;
            Domain = score.Domain;
            Weight = score.Credit;
        }

        public SubjectScore RawScore { get; private set; }

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

        /// <summary>
        /// 所屬領域。
        /// </summary>
        public string Domain { get; set; }

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
