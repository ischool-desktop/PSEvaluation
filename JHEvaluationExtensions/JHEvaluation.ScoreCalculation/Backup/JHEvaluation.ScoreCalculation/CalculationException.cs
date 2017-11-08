using System;
using System.Collections.Generic;
using System.Text;

namespace JHEvaluation.ScoreCalculation
{
    internal class CalculationException : Exception
    {
        public CalculationException(string message)
            : base(message)
        {
        }

        public CalculationException(List<StudentScore> students, string message)
            : base(message)
        {
            Students = students;
        }

        public List<StudentScore> Students { get; private set; }
    }
}
