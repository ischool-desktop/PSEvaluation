using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;
using K12.Data;
using SCSemsSubjectScore = JHEvaluation.ScoreCalculation.ScoreStruct.SemesterSubjectScore;
using SCSemsDomainScore = JHEvaluation.ScoreCalculation.ScoreStruct.SemesterDomainScore;
using SCSemsScore = JHEvaluation.ScoreCalculation.ScoreStruct.SemesterScore;
using JHEvaluation.ScoreCalculation.ScoreStruct;

namespace JHEvaluation.ScoreCalculation.BigFunction
{
    internal class GradScoreReader
    {
        private IStatusReporter Reporter { get; set; }

        private Dictionary<string, StudentScore> Students { get; set; }

        private int SchoolYear { get; set; }

        private int Semester { get; set; }

        public GradScoreReader(List<StudentScore> students, IStatusReporter reporter)
        {
            Students = students.ToDictionary();
            Reporter = reporter;
        }

        public void Read()
        {
            List<GradScoreRecord> GraduateScores = ReadGraduateScore();

            //讓每個學生先預設一個空的資料。
            foreach (StudentScore each in Students.Values)
                each.GraduateScore.Clear();

            //將有資料的學生填上資料。
            foreach (GradScoreRecord gradscore in GraduateScores)
            {
                if (!Students.ContainsKey(gradscore.RefStudentID)) continue;
                StudentScore student = Students[gradscore.RefStudentID];

                student.GraduateScore.RawScore = gradscore;
                foreach (GradDomainScore dscore in gradscore.Domains.Values)
                {
                    if (!student.GraduateScore.Contains(dscore.Domain))
                        student.GraduateScore.Add(dscore.Domain, new GraduateScore());

                    student.GraduateScore[dscore.Domain].Value = dscore.Score;
                    student.GraduateScore[dscore.Domain].Weight = 1; //算術平均。
                }
                student.GraduateScore.LearnDomainScore = gradscore.LearnDomainScore; //學習領域。
                student.GraduateScore.CourseLearnScore = gradscore.CourseLearnScore; //課程學習。
            }
        }

        private List<GradScoreRecord> ReadGraduateScore()
        {
            List<string> studIds = Students.Values.ToKeys();
            FunctionSpliter<string, GradScoreRecord> spliter = new FunctionSpliter<string, GradScoreRecord>(300, Util.MaxThread);
            spliter.Function = delegate(List<string> ps)
            {
                return GradScore.SelectByIDs<GradScoreRecord>(ps);
            };
            spliter.ProgressChange = delegate(int progress)
            {
                Reporter.Feedback("讀取畢業成績資料...", Util.CalculatePercentage(studIds.Count, progress));
            };
            return spliter.Execute(studIds);
        }
    }
}
