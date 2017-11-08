using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.RatingFramework;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating
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
        public ScoreParser(string item, string token)
        {
            Name = item;
            Token = token;
        }

        #region IScoreParser<RatingStudent> 成員

        public string Name { get; private set; }

        public decimal? GetScore(RatingStudent student)
        {
            if (!student.Scores[Token].Contains(Name))
                return null;
            else
                return student.Scores[Token][Name];
        }

        #endregion
    }
}
