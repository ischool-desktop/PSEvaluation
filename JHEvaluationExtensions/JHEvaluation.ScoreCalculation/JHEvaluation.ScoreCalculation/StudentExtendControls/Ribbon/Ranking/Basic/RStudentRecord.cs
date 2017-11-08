using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking.Basic
{
    internal class RStudentRecord
    {
        public static List<RStudentRecord> LoadStudents(IEnumerable<string> studIds)
        {
            List<K12.Data.StudentRecord> k12students = K12.Data.Student.SelectByIDs(studIds);
            List<RStudentRecord> students = new List<RStudentRecord>();
            K12.Data.Class.SelectAll();

            foreach (K12.Data.StudentRecord each in k12students)
            {
                if (each.Status == K12.Data.StudentRecord.StudentStatus.一般 ||
                    each.Status == K12.Data.StudentRecord.StudentStatus.延修 ||
                    each.Status == K12.Data.StudentRecord.StudentStatus.輟學)
                    students.Add(new RStudentRecord(each));
            }

            return students;
        }

        public RStudentRecord(K12.Data.StudentRecord record)
        {
            ID = record.ID;
            Name = record.Name;
            SeatNo = record.SeatNo.HasValue ? record.SeatNo.Value.ToString() : "";
            StudentNumber = record.StudentNumber;
            ClassName = string.Empty;
            GradeYear = string.Empty;

            K12.Data.ClassRecord cr = record.Class;
            if (cr != null)
            {
                ClassName = record.Class.Name;
                GradeYear = record.Class.GradeYear.HasValue ? record.Class.GradeYear.Value.ToString() : "";
            }
        }

        public string ID { get; private set; }

        public string Name { get; private set; }

        public string ClassName { get; private set; }

        public string GradeYear { get; private set; }

        public string SeatNo { get; private set; }

        public string StudentNumber { get; private set; }
    }

    internal static class RStudentRecordExtens
    {
        public static List<string> AsKeyList(this IEnumerable<RStudentRecord> students)
        {
            List<string> studIds = new List<string>();
            foreach (RStudentRecord each in students)
                studIds.Add(each.ID);

            return studIds;
        }
    }
}
