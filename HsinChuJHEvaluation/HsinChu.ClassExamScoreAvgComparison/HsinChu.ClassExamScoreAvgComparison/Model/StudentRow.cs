using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HsinChu.JHEvaluation.Data;
using JHSchool.Data;
using JHSchool.Evaluation.Ranking;
using JHEvaluation.ScoreCalculation;

namespace HsinChu.ClassExamScoreAvgComparison.Model
{
    internal class StudentRow
    {
        public List<HC.JHSCETakeRecord> RawScoreList { get; private set; }
        public List<CourseScore> CourseScoreList { get; private set; }
        public List<string> CourseIDs { get; private set; }
        public FinalScores Scores { get; private set; }
        public RankScore RankResult { get; private set; }
        public StudentScore StudentScore { get; private set; }

        public StudentRow(StudentScore studentScore)
        {
            RawScoreList = new List<HC.JHSCETakeRecord>();
            CourseScoreList = new List<CourseScore>();
            CourseIDs = new List<string>();
            Scores = new FinalScores();
            RankResult = new RankScore(decimal.Zero, null);
            StudentScore = studentScore;
        }

        public void AddRawScore(JHSCETakeRecord score)
        {
            RawScoreList.Add(new HC.JHSCETakeRecord(score));
        }

        public List<string> ConvertToCourseScores(List<string> courseIDs, string examID)
        {
            List<string> validIDs = new List<string>();
            foreach (var score in RawScoreList)
            {
                if (score.RefExamID != examID) continue;
                if (courseIDs.Contains(score.RefCourseID))
                {
                    if (!CourseIDs.Contains(score.RefCourseID))
                        CourseIDs.Add(score.RefCourseID);

                    CourseScore courseScore = new CourseScore(score.RefCourseID, score.Score, score.AssignmentScore);
                    CourseScoreList.Add(courseScore);

                    validIDs.Add(score.RefCourseID);
                }
            }
            return validIDs;
        }

        internal void Clear()
        {
            CourseScoreList.Clear();
            CourseIDs.Clear();
            Scores.Clear();
            RankResult = new RankScore(decimal.Zero, null);
        }
    }

    internal class StudentRows : Dictionary<string, StudentRow> { }
}
