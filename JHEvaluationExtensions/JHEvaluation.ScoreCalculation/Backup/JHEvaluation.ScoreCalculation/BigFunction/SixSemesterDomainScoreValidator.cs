using System;
using System.Collections.Generic;
using System.Text;

namespace JHEvaluation.ScoreCalculation.BigFunction
{
    /// <summary>
    /// 此類別主要是檢查學生的學期成績是否有滿六個學期，不滿六學期不能計算成績。
    /// https://sites.google.com/a/ischool.com.tw/jh-requirement-library/Home/cheng-ji-xu-qiu-wen-jian-1/cheng-ji-bao-biao-zheng-li/xue-sheng-bi-ye-cheng-ji-ji-suan
    /// </summary>
    internal class SixSemesterDomainScoreValidator
    {
        private List<StudentScore> Students { get; set; }

        public SixSemesterDomainScoreValidator(List<StudentScore> students)
        {
            Students = students;
        }

        public List<StudentScore> Valid()
        {
            List<StudentScore> noValid = new List<StudentScore>();

            foreach (StudentScore student in Students)
            {
                foreach (SemesterData sems in student.SHistory.GetGradeYearSemester())
                {
                    SemesterData sd = new SemesterData(0, sems.SchoolYear, sems.Semester);
                    if (!student.SemestersScore.Contains(sd)) //只要不包含該學期的學期成績，表示資料不完整。
                    {
                        noValid.Add(student);
                        break;
                    }
                }
            }

            return noValid;
        }
    }
}
