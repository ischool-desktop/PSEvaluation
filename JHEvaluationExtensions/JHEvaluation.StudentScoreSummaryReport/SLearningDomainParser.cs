using System;
using System.Collections.Generic;
using System.Text;
using JHEvaluation.ScoreCalculation;
using JHEvaluation.ScoreCalculation.ScoreStruct;

namespace JHEvaluation.StudentScoreSummaryReport
{
    /// <summary>
    /// 代表單一學期的學習領域成績排名成績解析邏輯。
    /// </summary>
    internal class SLearningDomainParser : Campus.Rating.IScoreParser<ReportStudent>
    {
        private SemesterData Semester { get; set; }

        public SLearningDomainParser(int gradeYear, int semester)
        {
            Semester = new SemesterData(gradeYear, 0, semester);
        }

        #region IScoreParser<ReportStudent> 成員

        public decimal? GetScore(ReportStudent student)
        {
            SemesterScore score = null;

            foreach (SemesterData each in student.SHistory.GetGradeYearSemester())
            {
                SemesterData gysemester = new SemesterData(each.GradeYear, 0, each.Semester);
                if (gysemester == Semester)
                {
                    SemesterData sd = new SemesterData(0, each.SchoolYear, each.Semester);

                    if (student.SemestersScore.Contains(sd))
                        score = student.SemestersScore[sd];

                    break;
                }
            }

            if (score == null)
                return null;
            else
                return score.LearnDomainScore;
        }

        public string Name
        {
            get { return GetSemesterString(Semester); }
        }

        public static string GetSemesterString(SemesterData semester)
        {
            return string.Format("學習領域({0}:{1})", semester.GradeYear, semester.Semester);
        }

        #endregion
    }
}
