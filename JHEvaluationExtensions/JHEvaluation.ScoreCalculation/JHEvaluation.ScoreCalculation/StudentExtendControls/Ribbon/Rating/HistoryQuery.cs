using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using K12.Data;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating
{
    internal class HistoryQuery
    {
        //學生編號,學年度,學期<學期歷程>
        private Dictionary<string, Dictionary<int, Dictionary<int, SemesterHistoryItem>>> Histories { get; set; }

        public HistoryQuery(List<JHSemesterHistoryRecord> histories)
        {
            Histories = new Dictionary<string, Dictionary<int, Dictionary<int, SemesterHistoryItem>>>();
            foreach (JHSemesterHistoryRecord eachStudent in histories)
            {
                //加一個學生。
                Histories.Add(eachStudent.RefStudentID, new Dictionary<int, Dictionary<int, SemesterHistoryItem>>());
                //把那個學生拿出來。
                Dictionary<int, Dictionary<int, SemesterHistoryItem>> schoolyear = Histories[eachStudent.RefStudentID];

                foreach (SemesterHistoryItem eachSems in eachStudent.SemesterHistoryItems)
                {
                    //加一個學年。
                    if (!schoolyear.ContainsKey(eachSems.SchoolYear))
                        schoolyear.Add(eachSems.SchoolYear, new Dictionary<int, SemesterHistoryItem>());

                    //把那個學年拿出來。
                    Dictionary<int, SemesterHistoryItem> semester = schoolyear[eachSems.SchoolYear];

                    //加一個學期。
                    if (!semester.ContainsKey(eachSems.Semester))
                        semester.Add(eachSems.Semester, eachSems);
                }
            }
        }

        public string GetToken(string studentId, int schoolYear, int semester)
        {
            SemesterHistoryItem item = Histories[studentId][schoolYear][semester];

            return GetGradeString(item.GradeYear) + GetSemesterString(item.Semester);
        }

        public SemesterHistoryItem GetHistoryItem(string studentId, int schoolYear, int semester)
        {
            return Histories[studentId][schoolYear][semester];
        }

        public bool Contains(string studentId, int schoolYear, int semester)
        {
            if (Histories.ContainsKey(studentId))
            {
                if (Histories[studentId].ContainsKey(schoolYear))
                {
                    if (Histories[studentId][schoolYear].ContainsKey(semester))
                        return true;
                }
            }

            return false;
        }

        private string GetSemesterString(int semester)
        {
            if (semester == 1)
                return "上";
            else
                return "下";
        }

        private string GetGradeString(int grade)
        {
            if (grade == 1)
                return "一";
            else if (grade == 2)
                return "二";
            else if (grade == 3)
                return "三";
            else if (grade == 7)
                return "七";
            else if (grade == 8)
                return "八";
            else if (grade == 9)
                return "九";
            else
                return string.Empty;
        }

    }
}
