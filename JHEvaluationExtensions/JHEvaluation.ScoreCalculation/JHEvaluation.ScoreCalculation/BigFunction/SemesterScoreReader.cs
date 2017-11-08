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
    class SemesterScoreReader
    {
        private IStatusReporter Reporter { get; set; }

        private Dictionary<string, StudentScore> Students { get; set; }

        private int SchoolYear { get; set; }

        private int Semester { get; set; }

        public SemesterScoreReader(List<StudentScore> students, int schoolYear, int semester, IStatusReporter reporter)
        {
            Students = students.ToDictionary();
            SchoolYear = schoolYear;
            Semester = semester;
            Reporter = reporter;
        }

        public void Read()
        {
            List<JHSemesterScoreRecord> SemesterScores = ReadSemesterScore();

            //讓每個學生先預設一個空的資料。
            foreach (StudentScore each in Students.Values)
            {
                each.SemestersScore.Clear();
                each.SemestersScore.Add(SemesterData.Empty, new SCSemsScore(SchoolYear, Semester));
            }

            //將有資料的學生填上資料。
            foreach (JHSemesterScoreRecord semsscore in SemesterScores)
            {
                if (!Students.ContainsKey(semsscore.RefStudentID)) continue;
                StudentScore student = Students[semsscore.RefStudentID];
                student.SemestersScore[SemesterData.Empty] = new SCSemsScore(semsscore);
            }
        }

        private List<JHSemesterScoreRecord> ReadSemesterScore()
        {
            List<string> studIds = Students.Values.ToKeys();
            FunctionSpliter<string, JHSemesterScoreRecord> spliter = new FunctionSpliter<string, JHSemesterScoreRecord>(300, Util.MaxThread);
            spliter.Function = delegate(List<string> studIdPart)
            {
                return JHSemesterScore.SelectBySchoolYearAndSemester(studIdPart, SchoolYear, Semester);
            };
            spliter.ProgressChange = delegate(int progress)
            {
                Reporter.Feedback("讀取學期成績資料...", Util.CalculatePercentage(studIds.Count, progress));
            };
            return spliter.Execute(studIds);
        }
    }
}
