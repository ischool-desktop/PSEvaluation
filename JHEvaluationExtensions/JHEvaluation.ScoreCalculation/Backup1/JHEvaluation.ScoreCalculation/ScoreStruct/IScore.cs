using System;
using System.Collections.Generic;
using System.Text;

namespace JHEvaluation.ScoreCalculation.ScoreStruct
{
    public interface IScore
    {
        decimal? Value { get; }

        decimal? Weight { get; }
    }
}
