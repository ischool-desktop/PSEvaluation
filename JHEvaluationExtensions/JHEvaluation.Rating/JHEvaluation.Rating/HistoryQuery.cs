using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus;
using JHSchool.Data;
using K12.Data;

namespace JHEvaluation.Rating
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

        /// <summary>
        /// 判斷是否有該學生的學期歷程。
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="schoolYear"></param>
        /// <param name="semester"></param>
        /// <returns></returns>
        public bool Contains(string studentId, int schoolYear, int semester)
        {
            if (Histories.ContainsKey(studentId))
            {
                List<SemesterData> distinct = DistinctSemester(studentId);

                SemesterData found = distinct.Find(sd =>
                {
                    if (sd.SchoolYear == schoolYear && sd.Semester == semester)
                        return true;
                    else
                        return false;
                });

                return found != new SemesterData();
            }

            return false;
        }

        /// <summary>
        /// 將重讀的處理掉，以最新學期為主。
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        private List<SemesterData> DistinctSemester(string studentId)
        {
            List<SemesterData> semesters = new List<SemesterData>();

            //將所有學期歷程先放入 semesters 清單。
            foreach (KeyValuePair<int, Dictionary<int, SemesterHistoryItem>> sy in Histories[studentId])
            {
                int sYear = sy.Key;
                foreach (KeyValuePair<int, SemesterHistoryItem> ss in sy.Value)
                {
                    int sSems = ss.Key;
                    int gradeYear = ss.Value.GradeYear;

                    semesters.Add(new SemesterData(gradeYear, sYear, sSems));
                }
            }

            //去掉重讀的。
            List<SemesterData> distinct = SemesterData.DistinctGradeYear(semesters.ToArray()).ToList();

            return distinct;
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
