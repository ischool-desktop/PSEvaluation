using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Rating;

namespace JHEvaluation.Rating
{
    /// <summary>
    /// 代表多學期科目(領域)成績解析邏輯。
    /// </summary>
    internal class MultiScoreParser : IScoreParser<RatingStudent>
    {
        private List<string> Tokens { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">科目或領域名稱</param>
        /// <param name="tokens">學期識別資訊。</param>
        public MultiScoreParser(ScoreItem item, List<string> tokens)
        {
            Item = item;
            Name = item.Name;
            Tokens = tokens;
        }

        #region IScoreParser<RatingStudent> 成員

        public ScoreItem Item { get; private set; }

        public string Name { get; private set; }

        public decimal? GetScore(RatingStudent student)
        {
            decimal count = 0, sum = 0;

            foreach (string token in Tokens)
            {
                if (student.Scores.Contains(Item.Regulation(token)))
                {
                    if (student.Scores[Item.Regulation(token)].Contains(Name))
                    {
                        count++;
                        sum += student.Scores[Item.Regulation(token)][Name];
                    }
                }
            }

            if (count <= 0)
                return null;
            else
                return Math.Round(sum / count, 2, MidpointRounding.AwayFromZero);
        }

        #endregion
    }
}
