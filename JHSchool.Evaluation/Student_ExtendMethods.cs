using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.Editor;

namespace JHSchool.Evaluation
{

    /// <summary>
    /// 由 Student 類別所提供的 Extend Method
    /// </summary>
    public static class Student_ExtendMethod
    {

        /// <summary>
        /// 加入學生修課。
        /// </summary>
        //public static List<CourseRecord> AddCourses(this StudentRecord student,List<CourseRecord> courses)
        //{
        //    List<CourseRecord> HasAddCourses = new List<CourseRecord>();

        //    if (courses == null)
        //        return HasAddCourses;

        //    if (courses.Count == 0)
        //        return HasAddCourses;

        //    List<SCAttendRecordEditor> scattendEditors = new List<SCAttendRecordEditor>();

        //    List<CourseRecord> studentcourses = student.GetAttendCourses();

        //    foreach (CourseRecord course in courses)
        //    {
        //        bool IsExist = false;

        //        //檢查學生是否已修這門課
        //        foreach (CourseRecord studentcourse in studentcourses)
        //        {
        //            if (studentcourse.ID.Equals(course.ID))
        //            {
        //                IsExist = true;
        //                break;
        //            }
        //        }

        //        if (IsExist == false)
        //        {
        //            SCAttendRecordEditor editor = new SCAttendRecordEditor(student, course);
        //            scattendEditors.Add(editor);
        //            HasAddCourses.Add(course);
        //        }
        //    }

        //    scattendEditors.SaveAllEditors();

        //    return HasAddCourses;
        //}
    }
}