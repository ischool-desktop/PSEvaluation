using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.CalculationWizard
{
    interface IProgressUI
    {
        void ReportProgress(string message, int progress);

        void Cancel();

        bool Cancellation { get;}
    }
}
