using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;

namespace HsinChu.JHEvaluation.Utility
{
    internal static class StudentInfoConvertor
    {
        public static string GetInfoWithClass(JHStudentRecord student)
        {
            string info = string.Empty;
            StringBuilder builder = new StringBuilder("");
            if (student.Class != null)
            {
                builder.Append(student.Class.Name);
                if (!string.IsNullOrEmpty("" + student.SeatNo))
                    builder.Append("(" + student.SeatNo + "號)");
                builder.Append(" ");
            }
            if (!string.IsNullOrEmpty(student.StudentNumber))
                builder.Append(student.StudentNumber + " ");
            if (string.IsNullOrEmpty(builder.ToString()))
                builder.Append("學生：");
            builder.Append(student.Name);
            return builder.ToString();
        }
    }
}
