using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;

namespace JHEvaluation.ScoreCalculation.BigFunction
{
    internal class CalculationRuleReader
    {
        private IStatusReporter Reporter { get; set; }

        private List<StudentScore> Students { get; set; }

        public CalculationRuleReader(List<StudentScore> students, IStatusReporter reporter)
        {
            this.Students = students;
            this.Reporter = reporter;
        }

        public List<StudentScore> Read()
        {
            #region 讀取成績計算規則。
            IStatusReporter Rep = Reporter;
            Rep.Feedback("讀取成績計算規則...", 0);

            UniqueSet<string> ruleids = new UniqueSet<string>();
            foreach (StudentScore each in Students)
            {
                if (!string.IsNullOrEmpty(each.RefCalculationRuleID))
                {
                    if (!ruleids.Contains(each.RefCalculationRuleID))
                        ruleids.Add(each.RefCalculationRuleID);
                }

                JHClassRecord cr = each.Class;
                if (cr != null)
                {
                    if (string.IsNullOrEmpty(cr.RefScoreCalcRuleID)) continue;

                    if (!ruleids.Contains(cr.RefScoreCalcRuleID))
                        ruleids.Add(cr.RefScoreCalcRuleID);
                }
            }

            FunctionSpliter<string, JHScoreCalcRuleRecord> spliter = new FunctionSpliter<string, JHScoreCalcRuleRecord>(10, Util.MaxThread);
            spliter.Function = delegate(List<string> idsPart)
            {
                return JHScoreCalcRule.SelectByIDs(idsPart);
            };
            spliter.ProgressChange = delegate(int progress)
            {
                Rep.Feedback("讀取成績計算規則...", Util.CalculatePercentage(ruleids.Count, progress));
            };
            StudentScore.SetRuleMapping(spliter.Execute(ruleids.ToList()));

            List<StudentScore> noRule = new List<StudentScore>();
            foreach (StudentScore each in Students)
            {
                if (each.CalculationRule == null)
                    noRule.Add(each);
            }
            return noRule;
            #endregion
        }
    }
}
