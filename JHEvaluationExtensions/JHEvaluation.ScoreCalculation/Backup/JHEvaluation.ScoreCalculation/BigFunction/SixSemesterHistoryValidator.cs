using System;
using System.Collections.Generic;
using System.Text;

namespace JHEvaluation.ScoreCalculation.BigFunction
{
    /// <summary>
    /// 此類別主要是檢查學生的學期歷程是否有滿六個學期，不滿六學期不能計算成績。
    /// https://sites.google.com/a/ischool.com.tw/jh-requirement-library/Home/cheng-ji-xu-qiu-wen-jian-1/cheng-ji-bao-biao-zheng-li/xue-sheng-bi-ye-cheng-ji-ji-suan
    /// </summary>
    internal class SixSemesterHistoryValidator
    {
        private List<StudentScore> Students { get; set; }

        public SixSemesterHistoryValidator(List<StudentScore> students)
        {
            Students = students;
        }

        public List<StudentScore> Valid()
        {
            List<StudentScore> noValid = new List<StudentScore>();

            foreach (StudentScore student in Students)
            {
                //取得六個學期的學期資訊。
                SemesterDataCollection sixSemester = student.SHistory.GetGradeYearSemester();

                //依學生的起始年級整理出六個學期應該有的資料。
                Dictionary<int, bool> validSet = GetValidSet(sixSemester.FirstSemester.GradeYear);

                //對六個學期進行檢查。
                foreach (SemesterData sems in sixSemester)
                {
                    //只用年級、學期檢查。
                    int key = (sems.GradeYear << 24) + (sems.Semester);
                    if (validSet.ContainsKey(key)) validSet[key] = true; //將正確的年級、學期標示為 True。
                }

                bool success = true;
                foreach (bool flag in validSet.Values)
                    success &= flag; //只要有一個學期沒有正確標示，表示學期歷程資訊不完整。

                if (!success) noValid.Add(student);
            }

            return noValid;
        }

        private static Dictionary<int, bool> GetValidSet(int firstGradeYear)
        {
            int gradeYear = firstGradeYear;

            Dictionary<int, bool> validTemplate = new Dictionary<int, bool>();
            validTemplate.Add(((gradeYear) << 24) + (1), false); //一上
            validTemplate.Add(((gradeYear) << 24) + (2), false); //一下
            validTemplate.Add(((++gradeYear) << 24) + (1), false); //二上
            validTemplate.Add(((gradeYear) << 24) + (2), false); //二下
            validTemplate.Add(((++gradeYear) << 24) + (1), false); //三上
            validTemplate.Add(((gradeYear) << 24) + (2), false); //三下
            return validTemplate;
        }
    }
}
