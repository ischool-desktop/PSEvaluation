using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Rating;

namespace JHEvaluation.Rating
{
    /// <summary>
    /// 代表單一學期相關成績的運算排名解析邏輯。
    /// </summary>
    internal class CalculationScoreParser : IScoreParser<RatingStudent>
    {
        private ItemWeightCollection Items { get; set; }

        private CalcMethod Method { get; set; }

        private int RoundPosition { get; set; }

        private string Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="items"></param>
        /// <param name="method"></param>
        /// <param name="round">計算到小數第幾位。</param>
        public CalculationScoreParser(string name,
            ItemWeightCollection items,
            CalcMethod method,
            int round,
            string token)
        {
            Name = name;
            Items = items;
            Method = method;
            RoundPosition = round;
            Token = token;
        }

        #region IScoreParser<RatingStudent> 成員

        public string Name { get; private set; }

        public decimal? GetScore(RatingStudent student)
        {
            ItemWeightCollection exists = new ItemWeightCollection();
            foreach (ScoreItem each in Items)
            {
                if (student.Scores[each.Regulation(Token)].Contains(each.Name))
                    exists.Add(each, Items[each]);
            }

            if (exists.Count <= 0) return null;

            if (Method == CalcMethod.加權平均)
                return 加權平均(student, exists);
            else if (Method == CalcMethod.加權總分)
                return 加權總分(student, exists);
            else if (Method == CalcMethod.合計總分)
                return 合計總分(student, exists);
            else
                return 算術平均(student, exists);
        }

        private decimal 算術平均(RatingStudent student, ItemWeightCollection exists)
        {
            decimal sum = 0, weight = exists.Count;

            foreach (ScoreItem each in exists)
                sum += student.Scores[each.Regulation(Token)][each.Name];

            return Round(sum / weight);
        }

        private decimal 合計總分(RatingStudent student, ItemWeightCollection exists)
        {
            decimal sum = 0;

            foreach (ScoreItem each in exists)
                sum += student.Scores[each.Regulation(Token)][each.Name];

            return sum;
        }

        private decimal 加權總分(RatingStudent student, ItemWeightCollection exists)
        {
            decimal sum = 0;

            foreach (ScoreItem each in exists)
                sum += (student.Scores[each.Regulation(Token)][each.Name] * exists[each]);

            return sum;
        }

        private decimal 加權平均(RatingStudent student, ItemWeightCollection exists)
        {
            decimal sum = 0, weight = exists.GetWeightSum();

            foreach (ScoreItem each in exists)
                sum += (student.Scores[each.Regulation(Token)][each.Name] * exists[each]);

            return Round(sum / weight);
        }

        private decimal Round(decimal score)
        {
            return Math.Round(score, RoundPosition, MidpointRounding.AwayFromZero);
        }
        #endregion

        public enum CalcMethod
        {
            加權總分,
            加權平均,
            合計總分,
            算術平均
        }
    }
}
