using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHEvaluation.ExamScoreCopy
{
    internal class Reporter : IReporter
    {
        #region IReporter 成員

        public void Feedback(string message, int progress)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage(message, progress);
        }

        #endregion
    }
}
