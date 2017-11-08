using System;
using System.Collections.Generic;
using System.Text;
using SemesterSubjectScore = JHEvaluation.ScoreCalculation.ScoreStruct.SemesterSubjectScore;
using SemesterDomainScore = JHEvaluation.ScoreCalculation.ScoreStruct.SemesterDomainScore;
using SCSemsScore = JHEvaluation.ScoreCalculation.ScoreStruct.SemesterScore;
using K12.Data;
using JHSchool.Data;
using JHEvaluation.ScoreCalculation.ScoreStruct;

namespace JHEvaluation.ScoreCalculation.BigFunction
{
    internal class GradScoreSaver
    {
        private IStatusReporter Reporter { get; set; }

        private List<StudentScore> Students { get; set; }

        public GradScoreSaver(List<StudentScore> students, IStatusReporter reporter)
        {
            Reporter = reporter;
            Students = students;
        }

        public void Save()
        {
            List<GradScoreRecord> updateSemsScore = new List<GradScoreRecord>();
            DomainScoreLogFormater logFormater = new DomainScoreLogFormater();

            foreach (StudentScore student in Students)
            {
                GraduateScoreCollection gscore = student.GraduateScore; //畢業成績(自定 Object)。
                GradScoreRecord grecord = gscore.RawScore;   //畢業成績記錄(DAL)。

                //如果該筆成績沒有對應的畢業成績，就新增一筆。
                if (grecord == null)
                {
                    grecord = new GradScoreRecord();
                    grecord.RefStudentID = student.Id;
                }

                #region 複製領域成績。
                foreach (string strDomain in gscore)
                {
                    GraduateScore gs = gscore[strDomain];

                    if (!grecord.Domains.ContainsKey(strDomain))
                        grecord.Domains.Add(strDomain, new GradDomainScore(strDomain));

                    gscore.Log.Add(new LogData(strDomain, grecord.Domains[strDomain].Score + "", gs.Value + ""));

                    grecord.Domains[strDomain].Score = gs.Value;
                }

                foreach (LogData each in gscore.Log)
                    each.Formater = logFormater;

                gscore.LearningLog.Formater = logFormater;
                gscore.LearningLog.OriginValue = grecord.LearnDomainScore + "";
                gscore.LearningLog.NewValue = gscore.LearnDomainScore + "";
                gscore.CourseLog.Formater = logFormater;
                gscore.CourseLog.OriginValue = grecord.CourseLearnScore + "";
                gscore.CourseLog.NewValue = gscore.CourseLearnScore + "";

                grecord.LearnDomainScore = gscore.LearnDomainScore;
                grecord.CourseLearnScore = gscore.CourseLearnScore;
                #endregion

                //新增到「更新」清單中。
                updateSemsScore.Add(grecord);
            }

            #region 更新科目成績
            FunctionSpliter<GradScoreRecord, GradScoreRecord> updateData =
                new FunctionSpliter<GradScoreRecord, GradScoreRecord>(300, 5);
            updateData.Function = delegate(List<GradScoreRecord> ps)
            {
                GradScore.Update(ps);
                return new List<GradScoreRecord>();
            };
            updateData.ProgressChange = delegate(int progress)
            {
                Reporter.Feedback("更新畢業成績...", Util.CalculatePercentage(updateSemsScore.Count, progress));
            };
            updateData.Execute(updateSemsScore);
            #endregion

        }
    }
}
