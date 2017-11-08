using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using JHSchool.Data;
using K12.Data;

namespace HsinChu.StudentExamScoreReport.Processor
{
    internal class StudentSemesterHistory
    {
        public StudentSemesterHistory(DocumentBuilder builder, SemesterHistoryItem item)
        {
            builder.MoveToMergeField("班導師");
            builder.Write(item.Teacher);
        }
    }
}
