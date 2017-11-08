using Aspose.Words;
using K12.Data;

namespace KaoHsiung.StudentExamScoreReport.Processor
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
