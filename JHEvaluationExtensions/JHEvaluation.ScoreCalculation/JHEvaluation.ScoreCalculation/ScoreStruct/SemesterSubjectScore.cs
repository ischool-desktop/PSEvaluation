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
        private const decimal MakeupLimit = 60; //補考分數上限。

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

            ScoreOrigin = score.ScoreOrigin;
            ScoreMakeup = score.ScoreMakeup;
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

        /// <summary>
        /// 原始成績。
        /// </summary>
        public decimal? ScoreOrigin { get; set; }

        /// <summary>
        /// 補考成績。
        /// </summary>
        public decimal? ScoreMakeup { get; set; }

        /// <summary>
        /// 將最好的成績寫入 Value 屬性。
        /// </summary>
        public void BetterScoreSelection()
        {
            decimal? betterScore = SubjectScore.GetBetterScore(ScoreOrigin, ScoreMakeup);

            //decimal newScore = betterScore.HasValue ? betterScore.Value : 0;
            //decimal oldScore = Value.HasValue ? Value.Value : 0;

            //if (newScore >= oldScore)
            //    Value = newScore;

            Value = betterScore;
        }

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
