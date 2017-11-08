using System;
using System.Collections.Generic;
using System.Text;

namespace KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizard
{
    interface IProgressUI
    {
        void ReportProgress(string message, int progress);

        void Cancel();

        bool Cancellation { get;}
    }
}
