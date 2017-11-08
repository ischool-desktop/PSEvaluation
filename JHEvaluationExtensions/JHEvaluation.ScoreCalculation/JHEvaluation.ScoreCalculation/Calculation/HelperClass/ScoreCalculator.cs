using System;
using System.Xml;
using JHSchool.Data;

namespace JHEvaluation.ScoreCalculation
{
    public class ScoreCalculator : JHSchool.Evaluation.Calculation.ScoreCalculator
    {
        public ScoreCalculator(JHScoreCalcRuleRecord record) : base(record)
        {
        }
    }
}
