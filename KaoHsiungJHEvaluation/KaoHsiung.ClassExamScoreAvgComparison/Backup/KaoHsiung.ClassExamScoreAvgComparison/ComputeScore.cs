using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using KaoHsiung.ClassExamScoreAvgComparison.Model;

namespace KaoHsiung.ClassExamScoreAvgComparison
{
    internal class ComputeScore
    {
        private Dictionary<string, JHCourseRecord> Courses;

        public ComputeScore(Dictionary<string, JHCourseRecord> courses)
        {
            Courses = courses;
        }

        public decimal? Compute(List<CourseScore> courseScoreList, ComputeMethod method)
        {
            if (method == ComputeMethod.加權平均)
            {
                decimal sum = decimal.Zero;
                decimal credits = decimal.Zero;

                foreach (var cs in courseScoreList)
                {
                    JHCourseRecord course = Courses[cs.CourseID];
                    if (!course.Credit.HasValue && course.Credit.Value <= 0) continue;
                    if (!cs.Score.HasValue) continue;

                    sum += course.Credit.Value * cs.Score.Value;
                    credits += course.Credit.Value;
                }

                if (credits > 0)
                    return sum / credits;
                else
                    return null;
            }
            else if (method == ComputeMethod.加權總分)
            {
                decimal sum = decimal.Zero;

                foreach (var cs in courseScoreList)
                {
                    JHCourseRecord course = Courses[cs.CourseID];
                    if (!course.Credit.HasValue && course.Credit.Value <= 0) continue;
                    if (!cs.Score.HasValue) continue;

                    sum += course.Credit.Value * cs.Score.Value;
                }

                return sum;
            }
            else if (method == ComputeMethod.合計總分)
            {
                decimal sum = decimal.Zero;

                foreach (var cs in courseScoreList)
                {
                    if (!cs.Score.HasValue) continue;
                    sum += cs.Score.Value;
                }

                return sum;
            }
            else if (method == ComputeMethod.算術平均)
            {
                decimal sum = decimal.Zero;
                decimal count = 0;

                foreach (var cs in courseScoreList)
                {
                    if (!cs.Score.HasValue) continue;

                    sum += cs.Score.Value;
                    count++;
                }

                if (count > 0)
                    return sum / count;
                else
                    return null;
            }
            else return null;
        }
    }
}
