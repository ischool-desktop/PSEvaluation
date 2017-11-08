using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Rating;

namespace JHEvaluation.Rating
{
    /// <summary>
    /// 代表單一學期科目成績解析邏輯。
    /// </summary>
    internal class ScoreParser : IScoreParser<RatingStudent>
    {
        private string Token { get; set; }

        /// <summary>
        /// 科目或領域名稱。
        /// </summary>
        /// <param name="item"></param>
        public ScoreParser(ScoreItem item, string token)
        {
            Item = item;
            Name = Item.Name;
            Token = token;
        }

        #region IScoreParser<RatingStudent> 成員

        public ScoreItem Item { get; private set; }

        public string Name { get; private set; }

        public decimal? GetScore(RatingStudent student)
        {
            if (!student.Scores[Item.Regulation(Token)].Contains(Name))
                return null;
            else
                return student.Scores[Item.Regulation(Token)][Name];
        }

        #endregion
    }
}
