using System.Collections.Generic;
using JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReportControls;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon
{
    class GraduationPredictReport
    {
        public GraduationPredictReport(List<StudentRecord> students)
        {
            GraduationPredictReportForm form = new GraduationPredictReportForm(students);
            form.ShowDialog();
        }
    }
}
