using System;
using System.Collections.Generic;
using System.Text;

namespace JHEvaluation.ScoreCalculation
{
    public interface IStatusReporter
    {
        void Feedback(string message, int percentage);

    }

    internal class EmptyStatusReport : IStatusReporter
    {
        #region IStatusReporter 成員

        public void Feedback(string message, int percentage)
        {
        }
        #endregion
    }
}
