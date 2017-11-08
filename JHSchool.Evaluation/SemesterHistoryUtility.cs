using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Evaluation
{
    public class SemesterHistoryUtility
    {
        private Dictionary<SemesterInfo, int> _gradeYears = new Dictionary<SemesterInfo, int>();
        private string StudentID { get; set; }

        public Data.JHSemesterHistoryRecord SemesterHistoryRecord { get; private set; }

        //public SemesterHistoryUtility(string studentID)
        //{ 
        //}

        //public SemesterHistoryUtility(Data.StudentRecord student)
        //{
        //}

        public SemesterHistoryUtility(Data.JHSemesterHistoryRecord record)
        {
            StudentID = record.RefStudentID;
            SemesterHistoryRecord = record;

            foreach (K12.Data.SemesterHistoryItem item in SemesterHistoryRecord.SemesterHistoryItems)
            {
                SemesterInfo info = new SemesterInfo();
                info.SchoolYear = item.SchoolYear;
                info.Semester = item.Semester;

                if (!_gradeYears.ContainsKey(info))
                    _gradeYears.Add(info, item.GradeYear);
            }
        }

        #region Method: GradeYearExist
        public bool GradeYearExist(string schoolYear, string semester)
        {
            int intSchoolYear, intSemester;
            if (!int.TryParse(schoolYear, out intSchoolYear))
                intSchoolYear = 0;
            if (!int.TryParse(semester, out intSemester))
                intSemester = 0;
            return GradeYearExist(intSchoolYear, intSemester);
        }

        public bool GradeYearExist(int schoolYear, int semester)
        {
            SemesterInfo info = new SemesterInfo();
            info.SchoolYear = schoolYear;
            info.Semester = semester;
            return GradeYearExist(info);
        }

        public bool GradeYearExist(SemesterInfo semesterInfo)
        {
            if (_gradeYears.ContainsKey(semesterInfo))
                return true;
            else
                return false;
        }
        #endregion

        #region Method: GetGradeYear
        public int GetGradeYear(string schoolYear, string semester)
        {
            int intSchoolYear, intSemester;
            if (!int.TryParse(schoolYear, out intSchoolYear))
                throw new Exception("無效的學年度");
            if (!int.TryParse(semester, out intSemester))
                throw new Exception("無效的學期");
            return GetGradeYear(intSchoolYear, intSemester);
        }

        public int GetGradeYear(int schoolYear, int semester)
        {
            SemesterInfo info = new SemesterInfo();
            info.SchoolYear = schoolYear;
            info.Semester = semester;
            return GetGradeYear(info);
        }

        public int GetGradeYear(SemesterInfo semesterInfo)
        {
            if (_gradeYears.ContainsKey(semesterInfo))
                return _gradeYears[semesterInfo];
            else
                return 0;
        }
        #endregion
    }
}
