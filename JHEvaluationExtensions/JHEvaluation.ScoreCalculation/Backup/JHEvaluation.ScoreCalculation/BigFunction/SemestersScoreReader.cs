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
    class SemestersScoreReader
    {
        private IStatusReporter Reporter { get; set; }

        private Dictionary<string, StudentScore> Students { get; set; }


        public SemestersScoreReader(List<StudentScore> students, IStatusReporter reporter)
        {
            Students = students.ToDictionary();
            Reporter = reporter;
        }

        public void Read()
        {
            List<JHSemesterScoreRecord> SemesterScores = ReadSemesterScore();

            //將每位學生的資料清除。
            foreach (StudentScore each in Students.Values)
                each.SemestersScore.Clear();

            //將有資料的學生填上資料。
            foreach (JHSemesterScoreRecord semsscore in SemesterScores)
            {
                if (!Students.ContainsKey(semsscore.RefStudentID)) continue;
                StudentScore student = Students[semsscore.RefStudentID];

                SemesterData sd = new SemesterData(0, semsscore.SchoolYear, semsscore.Semester);
                student.SemestersScore.Add(sd, new SCSemsScore(semsscore));
            }
        }

        private List<JHSemesterScoreRecord> ReadSemesterScore()
        {
            List<string> studIds = Students.Values.ToKeys();
            FunctionSpliter<string, JHSemesterScoreRecord> spliter = new FunctionSpliter<string, JHSemesterScoreRecord>(100, Util.MaxThread);
            spliter.Function = delegate(List<string> studIdPart)
            {
                return JHSemesterScore.SelectBySchoolYearAndSemester(studIdPart, null, null);
            };
            spliter.ProgressChange = delegate(int progress)
            {
                Reporter.Feedback("讀取學期成績資料...", Util.CalculatePercentage(studIds.Count, progress));
            };
            return spliter.Execute(studIds);
        }
    }
}
