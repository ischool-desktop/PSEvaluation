using System;
using System.Collections.Generic;
using System.Text;
using JHEvaluation.ScoreCalculation;

namespace KH_StudentScoreSummaryReport
{
    /// <summary>
    /// 代表學習領域成績排名成績解析邏輯。
    /// </summary>
    internal class LearningDomainParser : Campus.Rating.IScoreParser<ReportStudent>
    {
        private List<int> RequestSemesters { get; set; }

        public LearningDomainParser(List<int> reqSemesters)
        {
            RequestSemesters = reqSemesters;
        }

        #region IScoreParser<ReportStudent> 成員

        public decimal? GetScore(ReportStudent student)
        {
            SemesterDataCollection semesters = new SemesterDataCollection();
            foreach (SemesterData each in student.SHistory.GetGradeYearSemester().GetSemesters(RequestSemesters))
                semesters.Add(new SemesterData(0, each.SchoolYear, each.Semester));

            if (student.SemestersScore.AvgLearningDomainScore(semesters).HasValue)
                return student.SemestersScore.AvgLearningDomainScore(semesters).Value;
            else
                return 0;
            //SemesterDataCollection semesters = new SemesterDataCollection();
            //foreach (SemesterData each in student.SHistory.GetGradeYearSemester().GetSemesters(RequestSemesters))
            //    semesters.Add(new SemesterData(0, each.SchoolYear, each.Semester));

            //return student.SemestersScore.AvgLearningDomainScore(semesters);
        }

        public const string PlaceName = "學習領域";

        public string Name
        {
            get { return PlaceName; }
        }

        #endregion
    }
}
