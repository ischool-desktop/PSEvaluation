using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHEvaluation.ExamScoreCopy
{
    /// <summary>
    /// Util...
    /// </summary>
    internal class Util
    {
        public const int MaxThread = 5;

        public static int CalculatePercentage(int totalCount, int currentCount)
        {
            decimal dTotalCount = totalCount;
            decimal dCurrentCount = currentCount;

            if (dCurrentCount <= 0) return 100;

            return (int)Math.Round((dCurrentCount / dTotalCount) * 100m, 0m);
        }
    }
}
