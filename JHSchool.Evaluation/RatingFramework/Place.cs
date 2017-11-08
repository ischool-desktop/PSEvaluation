using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.RatingFramework
{
    /// <summary>
    /// 代表名次資訊。
    /// </summary>
    public struct Place
    {
        public Place(int level, decimal score, int radix)
            : this()
        {
            Level = level;
            Score = score;
            Radix = radix;
        }

        /// <summary>
        /// 取得名次。
        /// </summary>
        public int Level { get; internal set; }

        /// <summary>
        /// 取得成績。
        /// </summary>
        public decimal Score { get; internal set; }

        /// <summary>
        /// 取得排名的基數。
        /// </summary>
        public int Radix { get; internal set; }

        /// <summary>
        /// 取得百分名次，計算到整數位。
        /// </summary>
        public decimal GetPercentage()
        {
            decimal rank = GetPercentage(0);
            return rank <= 0 ? 1 : rank;
        }

        /// <summary>
        /// 取得百分名次。
        /// </summary>
        /// <param name="digits">小數精確度。</param>
        public decimal GetPercentage(int digits)
        {
            if (Level <= 0) throw new ArgumentException("名次小於「0」無法計算百分名次。");
            if (Radix <= 0) throw new ArgumentException("基數小於「0 」無法計算百分名次。");

            decimal percentage = (decimal)Level / (decimal)Radix * 100m;
            return decimal.Round(percentage, digits, MidpointRounding.AwayFromZero);
        }
    }
}
