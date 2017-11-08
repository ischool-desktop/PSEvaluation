using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using JHSchool.Evaluation;

namespace JHEvaluation.ExamScoreCopy
{
    internal static class ExtendMethods
    {
        #region Student
        public static List<JHStudentRecord> GetInSchoolStudents(this IEnumerable<JHStudentRecord> students)
        {
            List<JHStudentRecord> list = new List<JHStudentRecord>();
            foreach (JHStudentRecord student in students)
            {
                if (student.Status == K12.Data.StudentRecord.StudentStatus.一般 ||
                    student.Status == K12.Data.StudentRecord.StudentStatus.輟學)
                    list.Add(student);
            }
            return list;
        }

        public static List<string> AsKeyList(this IEnumerable<JHStudentRecord> students)
        {
            var IDs = from student in students select student.ID;
            return IDs.ToList();
        }
        #endregion

        #region Course
        public static List<string> GetSubjects(this IEnumerable<JHCourseRecord> courses)
        {
            List<string> subjects = new List<string>();
            foreach (var course in courses)
            {
                if (string.IsNullOrEmpty(course.Subject)) continue;
                if (!subjects.Contains(course.Subject))
                    subjects.Add(course.Subject);
            }
            subjects.Sort(delegate(string x, string y)
            {
                return Subject.CompareSubjectOrdinal(x, y);
            });
            return subjects;
        }

        public static List<string> AsKeyList(this IEnumerable<JHCourseRecord> courses)
        {
            var IDs = from course in courses select course.ID;
            return IDs.ToList();
        }
        #endregion
    }
}
