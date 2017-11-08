using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Rating;

namespace JHEvaluation.ClassSemesterScoreReport
{
    /// <summary>
    /// 代表單一學期科目成績解析邏輯。
    /// </summary>
    internal class ScoreParser : IScoreParser<ReportStudent>
    {
        private string Token { get; set; }

        private string ScoreName { get; set; }

        /// <summary>
        /// 科目或領域名稱。
        /// </summary>
        /// <param name="item"></param>
        public ScoreParser(string item, string token)
        {
            Name = item;
            ScoreName = item;
            Token = token;
        }

        #region IScoreParser<RatingStudent> 成員

        public string Name { get; private set; }

        public decimal? GetScore(ReportStudent student)
        {
            if (!student.Scores[Token].Contains(ScoreName))
                return null;
            else
                return student.Scores[Token][ScoreName];
        }

        #endregion
    }
}
