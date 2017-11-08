using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JointAdmissionModule.StudentScoreSummaryReport_HsinChu
{
    internal class CoreLearningDomainParser : Campus.Rating.IScoreParser<ReportStudent>
    {
        #region IScoreParser<ReportStudent> 成員

        public decimal? GetScore(ReportStudent student)       
        {
            decimal Score = student.CoreLearnDomainScore81 + student.CoreLearnDomainScore82 + student.CoreLearnDomainScore91;

            if (Score!=0)
                Score /=3;

            return Math.Round(Score, 2, MidpointRounding.AwayFromZero);
        }

        public const string PlaceName = "核心學習領域";

        public string Name
        {
            get { return PlaceName; }
        }
        #endregion
    }
}