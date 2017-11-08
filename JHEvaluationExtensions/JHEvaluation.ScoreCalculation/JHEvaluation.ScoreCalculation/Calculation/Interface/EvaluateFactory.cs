using System.Collections.Generic;
using System.Xml;

namespace JHSchool.Evaluation.Calculation.GraduationConditions
{
    /// <summary>
    /// 實際建立畢業判斷條件介面
    /// </summary>
    internal class EvaluateFactory : IEvaluateFactory
    {
        /// <summary>
        /// 傳回成績計算系統編號對實際判斷介面
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, IEvaluative> CreateEvaluativeEntities()
        {
            ScoreCalcRule.Instance.SyncAllBackground();

            Dictionary<string, IEvaluative> evals = new Dictionary<string,IEvaluative>();

            // 清存領域成績暫存區
            TempData.tmpStudDomainScoreDict.Clear();
            TempData.tmpStudDomainCreditDict.Clear();

            //針對每項成績計算規則建立實際的判斷介面
            foreach (ScoreCalcRuleRecord record in ScoreCalcRule.Instance.Items)
            {
                //若畢業判斷有內容才繼續
                if (record.Content == null) continue;

                AndEval allAndEval = new AndEval();

                XmlElement element;

                OrEval orScore = new OrEval();
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/學業成績畢業條件/條件[@Type='LearnDomainEach']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orScore.Add(new LearnDomainEachEval(element));
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/學業成績畢業條件/條件[@Type='LearnDomainLast']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orScore.Add(new LearnDomainLastEval(element));
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/學業成績畢業條件/條件[@Type='GraduateDomain']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orScore.Add(new GraduateDomainEval(element));

                OrEval orDaily1 = new OrEval();
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/日常生活表現畢業條件/條件[@Type='AbsenceAmountEach']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orDaily1.Add(new AbsenceAmountEachEval(element));
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/日常生活表現畢業條件/條件[@Type='AbsenceAmountLast']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orDaily1.Add(new AbsenceAmountLastEval(element));
                //added by Cloud 2014.2.13
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/日常生活表現畢業條件/條件[@Type='AbsenceAmountAll']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orDaily1.Add(new AbsenceAmountAllEval(element));

                OrEval orDaily2 = new OrEval();
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/日常生活表現畢業條件/條件[@Type='AbsenceAmountEachFraction']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orDaily2.Add(new AbsenceAmountEachFractionEval(element));
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/日常生活表現畢業條件/條件[@Type='AbsenceAmountLastFraction']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orDaily2.Add(new AbsenceAmountLastFractionEval(element));
                //added by Cloud 2014.2.13
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/日常生活表現畢業條件/條件[@Type='AbsenceAmountAllFraction']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orDaily2.Add(new AbsenceAmountAllFractionEval(element));

                OrEval orDaily3 = new OrEval();
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/日常生活表現畢業條件/條件[@Type='DemeritAmountEach']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orDaily3.Add(new DemeritAmountEachEval(element));
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/日常生活表現畢業條件/條件[@Type='DemeritAmountLast']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orDaily3.Add(new DemeritAmountLastEval(element));
                //added by Cloud 2014.2.13
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/日常生活表現畢業條件/條件[@Type='DemeritAmountAll']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orDaily3.Add(new DemeritAmountAllEval(element));

                OrEval orDaily4 = new OrEval();
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/日常生活表現畢業條件/條件[@Type='DailyBehavior']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orDaily4.Add(new DailyBehaviorEval(element));
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/日常生活表現畢業條件/條件[@Type='DailyBehaviorLast']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orDaily4.Add(new DailyBehaviorLastEval(element));
                //added by Cloud 2014.2.13
                element = (XmlElement)record.Content.SelectSingleNode("畢業條件/日常生活表現畢業條件/條件[@Type='DailyBehaviorAll']");
                if (element != null && bool.Parse(element.GetAttribute("Checked"))) orDaily4.Add(new DailyBehaviorAllEval(element));

                allAndEval.Add(orScore);
                allAndEval.Add(orDaily1);
                allAndEval.Add(orDaily2);
                allAndEval.Add(orDaily3);
                allAndEval.Add(orDaily4);

                //將畢業判斷介面加入到結果中
                evals.Add(record.ID, allAndEval);
            }

            return evals;
        }
    }
}