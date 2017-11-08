using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Evaluation.Ranking;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking
{
    internal class RankReporter
    {
        private RankData _rankData;
        private RankPrintFormat _format;

        public RankReporter(RankData rankData, RankPrintFormat format)
        {
            _rankData = rankData;
            _format = format;
        }
    }

    enum RankPrintFormat { Combine, SeparateAll, SeparateTop }
}
