using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHEvaluation.ExamScoreCopy
{
    interface IReporter
    {
        void Feedback(string message, int progress);
    }
}
