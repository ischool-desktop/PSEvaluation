using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Campus.Rating
{
    /// <summary>
    /// 代表名次資訊。
    /// </summary>
    public struct Place
    {
        internal Place(int level, int percentage, decimal score, int radix, int totalRadix)
            : this()
        {
            Level = level;
            Score = score;
            Radix = radix;
            Percentage = percentage;
            TotalRadix = totalRadix;
        }

        /// <summary>
        /// 取得名次。
        /// </summary>
        public int Level { get; internal set; }

        /// <summary>
        /// 百分排名。
        /// </summary>
        public int Percentage { get; internal set; }

        /// <summary>
        /// 取得成績。
        /// </summary>
        public decimal Score { get; internal set; }

        /// <summary>
        /// 取得排名的基數(僅包含有成績的人)。
        /// </summary>
        public int Radix { get; internal set; }

        /// <summary>
        /// 取得排名的基數(包含所有範圍(RatingScope)內的人)。
        /// </summary>
        public int TotalRadix { get; internal set; }

        /// <summary>
        /// 取得百分名次，計算到整數位。
        /// </summary>
        [Obsolete("有新方法取代，請參考 IPercentageAlgorithm 介面。")]
        public decimal GetPercentage()
        {
            decimal rank = GetPercentage(0);
            return rank <= 0 ? 1 : rank;
        }

        /// <summary>
        /// 取得百分名次。
        /// </summary>
        /// <param name="digits">小數精確度。</param>
        [Obsolete("有新方法取代，請參考 IPercentageAlgorithm 介面。")]
        public decimal GetPercentage(int digits)
        {
            if (Level <= 0) throw new ArgumentException("名次小於「0」無法計算百分名次。");
            if (Radix <= 0) throw new ArgumentException("基數小於「0 」無法計算百分名次。");

            decimal percentage = (decimal)Level / (decimal)Radix * 100m;
            return decimal.Round(percentage, digits, MidpointRounding.AwayFromZero);
        }
    }
}
