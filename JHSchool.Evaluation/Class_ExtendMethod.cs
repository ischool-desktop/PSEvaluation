using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation
{

    /// <summary>
    /// 由 Class 類別所提供的 Extend Method
    /// </summary>
    public static class Class_ExtendMethod
    {

        /// <summary>
        /// 取得班級修課清單。
        /// </summary>
        public static List<CourseRecord> GetCourses(this ClassRecord classrecord)
        {
            List<CourseRecord> courses = new List<CourseRecord>();

            if (classrecord == null)
                return courses;

            foreach (CourseRecord course in Course.Instance.Items)
                if (course.Class!=null && course.Class.Name.Equals(classrecord.Name))
                    courses.Add(course);

            return courses;
        }
    }
}