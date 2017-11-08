using System.Collections.Generic;
using System.Linq;

namespace JHEvaluation.AssignmentExamManager
{
    public class AssignmentExamSCAttend : JHSchool.Data.JHSCAttend
    {
        public static List<AssignmentExamSCAttendRecord> SelectByCourseIDs(IEnumerable<string> CourseIDs)
        {
            return SelectByCourseIDs<AssignmentExamSCAttendRecord>(CourseIDs).Where(x=>x.Student.Status == K12.Data.StudentRecord.StudentStatus.一般).ToList();
        }

        public static List<AssignmentExamSCAttendRecord> SelectByCourseID(string CourseID)
        {
            return SelectByCourseIDs(new List<string>() { CourseID });
        }
    }
}