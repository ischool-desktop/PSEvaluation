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
        private const decimal MakeupLimit = 60; //補考分數上限。

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
            ScoreOrigin = score.ScoreOrigin;
            ScoreMakeup = score.ScoreMakeup;
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
            BetterScoreSelection(true);
        }


        //2016/1/26 恩正與穎驊改寫幫原本的BetterScoreSelection()方法加入新的分數篩汰邏輯，
        //因本class 為public 為了怕有其他地方有引用原本的舊方法， 故在上面設一個轉移至新方法的方法

        public void BetterScoreSelection(bool limit60)
        {
            decimal originScore = 0;
            decimal makeupScore = 0;

            if (ScoreOrigin.HasValue)
                originScore = ScoreOrigin.Value;

            if (limit60 && ScoreMakeup.HasValue && ScoreMakeup.Value > 60)
                makeupScore = 60;
            else if (ScoreMakeup.HasValue)
                makeupScore = ScoreMakeup.Value;

            if (originScore >= makeupScore)
                Value = originScore;
            else
                Value = makeupScore;
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
