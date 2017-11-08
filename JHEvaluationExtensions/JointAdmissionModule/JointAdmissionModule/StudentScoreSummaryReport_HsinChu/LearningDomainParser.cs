using System;
using System.Collections.Generic;
using System.Text;
using JHEvaluation.ScoreCalculation;
using JHEvaluation.ScoreCalculation.ScoreStruct;
using System.Linq;

namespace JointAdmissionModule.StudentScoreSummaryReport_HsinChu
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

        /// <summary>
        /// 計算學習領域算術平均,當沒有成績當0分算。
        /// </summary>
        /// <param name="semesters">要平均的學期1。</param>
        /// <returns></returns>
        private decimal AvgLearningDomainScore(SemesterScoreCollection score ,IEnumerable<SemesterData> semesters)
        {
            decimal sum = 0;

            foreach (SemesterData sems in semesters)
            {
                if (score.Contains(sems)) //判斷是否包含此學期的成績。
                {
                    if (score[sems].LearnDomainScore.HasValue)
                        sum += score[sems].LearnDomainScore.Value;
                }
            }

            if (sum > 0)
                return Math.Round( sum/RequestSemesters.Count, 2, MidpointRounding.AwayFromZero);
            else
                return 0;
        }

        public decimal? GetScore(ReportStudent student)
        {
            SemesterDataCollection semesters = new SemesterDataCollection();
            foreach (SemesterData each in student.SHistory.GetGradeYearSemester().GetSemesters(RequestSemesters))
                semesters.Add(new SemesterData(0, each.SchoolYear, each.Semester));

            decimal result = AvgLearningDomainScore(student.SemestersScore, semesters);

            return result;
        }

        public const string PlaceName = "學習領域";

        public string Name
        {
            get { return PlaceName; }
        }

        #endregion

        #region IScoresParser<ReportStudent> 成員

        //public List<decimal> GetSecondScores(ReportStudent student)
        //{
        //    // 當學期總平均同分時比較用
        //    List<decimal > retVal = new List<decimal> ();
        //    retVal.Add(student.LearnDomainScore91);
        //    retVal.Add(student.LearnDomainScore82);
        //    retVal.Add(student.LearnDomainScore81);
        //    return retVal;

        //    //List<decimal> retVal = new List<decimal>(new decimal[] { 0, 0, 0 });
        //    //List<ReportStudent> rptStud = new List<ReportStudent>();
        //    //if (Report.StudentAvgScoreRank5RptParse(rptStud).Count > 0)
        //    //{
        //    //    retVal = new List<decimal>();

        //    //    // 九上
        //    //    if (Report.StudentAvgScoreRank5RptParse(rptStud)[0].DomainScore91.HasValue)
        //    //        retVal.Add(Report.StudentAvgScoreRank5RptParse(rptStud)[0].DomainScore91.Value);
        //    //    else
        //    //        retVal.Add(0);

        //    //    // 八下
        //    //    if (Report.StudentAvgScoreRank5RptParse(rptStud)[0].DomainScore82.HasValue)
        //    //        retVal.Add(Report.StudentAvgScoreRank5RptParse(rptStud)[0].DomainScore82.Value);
        //    //    else
        //    //        retVal.Add(0);

        //    //    // 八上
        //    //    if (Report.StudentAvgScoreRank5RptParse(rptStud)[0].DomainScore81.HasValue)
        //    //        retVal.Add(Report.StudentAvgScoreRank5RptParse(rptStud)[0].DomainScore81.Value);
        //    //    else
        //    //        retVal.Add(0);

        //    //}

            
        //}

        #endregion
    }
}
