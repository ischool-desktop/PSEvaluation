using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.RatingFramework;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating
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
        public MultiScoreParser(string name, List<string> tokens)
        {
            Name = name;
            Tokens = tokens;
        }

        #region IScoreParser<RatingStudent> 成員

        public string Name { get; private set; }

        public decimal? GetScore(RatingStudent student)
        {
            decimal count = 0, sum = 0;

            foreach (string each in Tokens)
            {
                if (student.Scores.Contains(each))
                {
                    if (student.Scores[each].Contains(Name))
                    {
                        count++;
                        sum += student.Scores[each][Name];
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
