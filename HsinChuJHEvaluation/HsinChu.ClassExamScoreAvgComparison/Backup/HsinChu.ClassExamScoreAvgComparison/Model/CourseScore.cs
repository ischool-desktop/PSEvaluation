using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using HsinChu.JHEvaluation.Data;

namespace HsinChu.ClassExamScoreAvgComparison.Model
{
    internal class CourseScore
    {
        public string CourseID { get; private set; }
        public decimal? Score { get; private set; }

        private decimal? _score;
        private decimal? _assignment_score;

        public CourseScore(string courseID, decimal? score, decimal? assignmentScore)
        {
            _score = score;
            _assignment_score = assignmentScore;
            CourseID = courseID;
        }

        public void CalculateScore(HC.JHAEIncludeRecord ae, string type)
        {
            //if (ae.UseScore) Score = _score;

            if (type == "定期")
            {
                if (ae.UseScore) Score = _score;
            }
            else if (type == "定期加平時")
            {
                if (ae.UseScore && ae.UseAssignmentScore)
                {
                    if (_score.HasValue && _assignment_score.HasValue)
                        Score = (_score.Value + _assignment_score.Value) / 2m;
                    else if (_score.HasValue) Score = _score;
                    else if (_assignment_score.HasValue) Score = _assignment_score;
                }
                else if (ae.UseScore)
                    Score = _score;
                else if (ae.UseAssignmentScore)
                    Score = _assignment_score;
            }
        }
    }
}
