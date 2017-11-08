using System;
using System.Collections.Generic;

namespace JHSchool.Evaluation.Calculation
{

    /// <summary>
    /// 代表年級、學期。
    /// </summary>
    public struct SemesterData
    {
        public static SemesterData Empty { get { return new SemesterData(0, 0, 0); } }

        /// <summary>
        /// 建構式。
        /// </summary>
        /// <param name="gradeYear">年級。</param>
        /// <param name="schoolyear">學年度。</param>
        /// <param name="semester">學期。</param>
        /// <remarks>參數可傳「0」。</remarks>
        public SemesterData(int gradeYear, int schoolyear, int semester)
            : this()
        {
            GradeYear = gradeYear;
            SchoolYear = schoolyear;
            Semester = semester;
        }

        /// <summary>
        /// 學年度。
        /// </summary>
        public int SchoolYear { get; private set; }

        /// <summary>
        /// 學期。
        /// </summary>
        public int Semester { get; private set; }

        /// <summary>
        /// 年級。
        /// </summary>
        public int GradeYear { get; private set; }

        /// <summary>
        /// 順序。
        /// </summary>
        public int Order { get { return (GradeYear << 24) + (SchoolYear << 16) + Semester; } }

        /// <summary>
        /// 計算下一個學期。
        /// </summary>
        /// <param name="semester"></param>
        /// <returns></returns>
        public SemesterData NextSemester()
        {
            int sy = SchoolYear;
            int ss = Semester;
            int gy = GradeYear;

            if (ss == 1)
                ss = 2;
            else
            {
                ss = 1;
                sy++;
                gy++;
            }

            return new SemesterData(gy, sy, ss);
        }
    }

    /// <summary>
    /// 代表學期資訊集合。
    /// </summary>
    public class SemesterDataCollection : List<SemesterData>
    {
        public SemesterDataCollection()
        {
        }

        public SemesterDataCollection(IEnumerable<SemesterData> semesters)
            : base(semesters)
        {
        }

        /// <summary>
        /// 取得排序後第一個學期的資訊。
        /// </summary>
        public SemesterData FirstSemester
        {
            get
            {
                if (Count <= 0) throw new InvalidOperationException("無法取得第一個學期的資訊，因為沒有任何學期資訊。");
                return this[0];
            }
        }

        /// <summary>
        /// 取得各年級的學期，如果有重覆學期以較新的為主。
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 資料：
        /// 98,1 一年級
        /// 98,2 一年級
        /// 99,2 一年級
        /// 結果：
        /// 98,1 一年級
        /// 99,2 一年級
        /// </remarks>
        public SemesterDataCollection GetGradeYearSemester()
        {
            List<SemesterData> allsems = new List<SemesterData>(this);

            //以年級、學年度、學期排序。
            allsems.Sort(delegate(SemesterData x, SemesterData y) { return x.Order.CompareTo(y.Order); });

            Dictionary<int, SemesterData> gradeyears = new Dictionary<int, SemesterData>();
            foreach (SemesterData each in allsems)
            {
                int intGy = (each.GradeYear << 24) + each.Semester;
                if (!gradeyears.ContainsKey(intGy))
                    gradeyears.Add(intGy, each);
                else
                    gradeyears[intGy] = each;
            }

            return new SemesterDataCollection(gradeyears.Values);
        }
    }
}
