using System;
using System.Collections.Generic;
using JHEvaluation.ScoreCalculation.ScoreStruct;
using JointAdmissionModule.DAL;

namespace JointAdmissionModule
{
    public static class SemesterScoreExtension
    {
        /// <summary>
        /// 小數點第二位四捨五入
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static decimal ToTwonPoint(this decimal Value)
        {
            return Math.Round(Value, 2, MidpointRounding.AwayFromZero);
        }

        public static string GetFormat(this int? Value)
        {
            if (Value.HasValue)
                return string.Format("{0:###}", Value.Value);
            else
                return string.Empty;
        }

        public static int GetValue(this int? Value)
        {
            return Value.HasValue ? Value.Value : 0;
        }

        public static string GetFormat(this decimal? Value)
        {
            return string.Format("{0:#00.00}", Value.GetValue());
        }

        public static decimal GetValue(this decimal? Value)
        {
            return Value.HasValue ? Value.Value : 0;
        }

        /// <summary>
        /// 根據科目前置名稱計算領域成績及權重
        /// </summary>
        /// <param name="Score"></param>
        /// <param name="StartName"></param>
        /// <returns></returns>
        public static Tuple<decimal, decimal ,decimal> GetDomainScoreFromSubject(this SemesterScore Score,string StartName)
        {
            decimal resultTotal = 0;
            decimal resultWeight = 0;
            decimal resultAverage = 0;

            foreach (string subject in Score.Subject)
            {
                if (Program.Mode == ModuleMode.HsinChu)
                {
                    if (subject.IndexOf(StartName) > -1 && Score.RawScore.Subjects[subject].Domain.Equals("語文"))
                    {
                        if (Score.Subject[subject].Value.HasValue && Score.Subject[subject].Weight.HasValue)
                        {
                            resultTotal += Score.Subject[subject].Value.Value * Score.Subject[subject].Weight.Value;
                            resultWeight += Score.Subject[subject].Weight.Value;
                        }
                    }
                }
            }

            if (resultWeight > 0)
                resultAverage = Math.Round(resultTotal / resultWeight,2,MidpointRounding.AwayFromZero);

            //傳回領域成績，以及此領域的權重
            return new Tuple<decimal, decimal,decimal>(resultAverage,resultWeight,Math.Round(resultTotal,2,MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// 從學期科目成績，計算核心學習領域成績
        /// </summary>
        /// <param name="Score"></param>
        /// <returns></returns>
        public static decimal GetCoreDomainLearningScore(this SemesterScore Score)
        {
            decimal result = 0;
            decimal resultWeight = 0;

            //取得核心學習領域
            List<string> Domains = DALTransfer.GetCoreDomainNameList();

            //若是新竹版本
            if (Program.Mode == ModuleMode.HsinChu)
            {
                //國語文及英語領域，由科目成績計算
                List<string> ExceptDomains = new List<string>(){"國語文","英語"};

                #region 計算國文領域成績及權重
                Tuple<decimal, decimal,decimal> resultChinese = Score.GetDomainScoreFromSubject("國");
                result += resultChinese.Item3;       //加上國文領域成績
                resultWeight += resultChinese.Item2; //加上國文領域權重
                #endregion

                #region 計算英文領域成績及權重
                Tuple<decimal, decimal,decimal> resultEnglish = Score.GetDomainScoreFromSubject("英");
                result += resultEnglish.Item3;       //加上英文領域成績
                resultWeight += resultEnglish.Item2; //加上英文領域權重
                #endregion

                //加總其他領域成績
                foreach (string Domain in Domains)
                {
                    //假設不在排外清單中
                    if (!ExceptDomains.Contains(Domain) && Score.Domain.Contains(Domain))
                        if (Score.Domain[Domain].Value.HasValue && Score.Domain[Domain].Weight.HasValue)
                        {
                            result += Score.Domain[Domain].Value.Value * Score.Domain[Domain].Weight.Value;
                            resultWeight += Score.Domain[Domain].Weight.Value;
                        }
                }

                if (resultWeight>0)
                    result /= resultWeight;

                return result;
            }else
            {
                //當高雄時直接依照領域判斷
                foreach (string Domain in Domains)
                {
                    if (Score.Domain.Contains(Domain))
                    {
                        if (Score.Domain[Domain].Value.HasValue && Score.Domain[Domain].Weight.HasValue)
                        {
                            result += Score.Domain[Domain].Value.Value * Score.Domain[Domain].Weight.Value; //計算加權總分
                            resultWeight += Score.Domain[Domain].Weight.Value;                              //計算加權分數
                        }
                    }
                }

                if (resultWeight>0)
                    result = result / resultWeight; //加權平均

                return Math.Round(result,2,MidpointRounding.AwayFromZero);
            }
        }
    }
}