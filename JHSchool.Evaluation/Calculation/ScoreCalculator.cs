using System;
using System.Xml;
using JHSchool.Data;

namespace JHSchool.Evaluation.Calculation
{
    public class ScoreCalculator
    {
        private Round _subjectRound = new Round();
        private Round _domainRound = new Round();
        private Round _learnDomainRound = new Round();
        private Round _graduateRound = new Round();

        public ScoreCalculator(Data.JHScoreCalcRuleRecord record)
        {
            if (record == null) return;

            XmlElement content = record.Content;

            XmlElement subject = (XmlElement)content.SelectSingleNode("成績計算規則/各項成績計算位數/科目成績計算");
            XmlElement domain = (XmlElement)content.SelectSingleNode("成績計算規則/各項成績計算位數/領域成績計算");
            XmlElement learnDomain = (XmlElement)content.SelectSingleNode("成績計算規則/各項成績計算位數/學習領域成績計算");
            XmlElement graduate = (XmlElement)content.SelectSingleNode("成績計算規則/各項成績計算位數/畢業成績計算");

            if (subject != null)
                _subjectRound.SetData(int.Parse(subject.GetAttribute("位數")), subject.GetAttribute("進位方式"));
            if (domain != null)
                _domainRound.SetData(int.Parse(domain.GetAttribute("位數")), domain.GetAttribute("進位方式"));
            if (learnDomain != null)
                _learnDomainRound.SetData(int.Parse(learnDomain.GetAttribute("位數")), learnDomain.GetAttribute("進位方式"));
            if (graduate != null)
                _graduateRound.SetData(int.Parse(graduate.GetAttribute("位數")), graduate.GetAttribute("進位方式"));

            //ScoreCalcRule>
            //            <成績計算規則>
            //                <各項成績計算位數>
            //                    <學期科目成績計算 位數="2" 進位方式="四捨五入"/>
            //                    <學期領域成績計算 位數="2" 進位方式="四捨五入"/>
            //                    <學期學習領域成績計算 位數="2" 進位方式="四捨五入"/>
            //                    <畢業成績計算 位數="2" 進位方式="四捨五入"/>
        }

        public decimal ParseSubjectScore(decimal score)
        {
            return _subjectRound.GetRoundScore(score);
        }

        public decimal ParseDomainScore(decimal score)
        {
            return _domainRound.GetRoundScore(score);
        }

        public decimal ParseLearnDomainScore(decimal score)
        {
            return _learnDomainRound.GetRoundScore(score);
        }

        public decimal ParseGraduateScore(decimal score)
        {
            return _graduateRound.GetRoundScore(score);
        }

        private class Round
        {
            private int _decimals;
            private RoundMode _method;

            public Round()
            {
                _decimals = 2;
                _method = RoundMode.四捨五入;
            }

            public void SetData(int decimals, string method)
            {
                _decimals = decimals;
                if (method == "四捨五入")
                    _method = RoundMode.四捨五入;
                else if (method == "無條件捨去")
                    _method = RoundMode.無條件捨去;
                else if (method == "無條件進位")
                    _method = RoundMode.無條件進位;
                else
                    _method = RoundMode.四捨五入;
            }

            private enum RoundMode { 四捨五入, 無條件進位, 無條件捨去 }

            public decimal GetRoundScore(decimal score)
            {
                return GetRoundScore(score, _decimals, _method);
            }

            private decimal GetRoundScore(decimal score, int decimals, RoundMode mode)
            {
                decimal seed = Convert.ToDecimal(Math.Pow(0.1, Convert.ToDouble(decimals)));
                switch (mode)
                {
                    default:
                    case RoundMode.四捨五入:
                        score = decimal.Round(score, decimals, MidpointRounding.AwayFromZero);
                        break;
                    case RoundMode.無條件捨去:
                        score /= seed;
                        score = decimal.Floor(score);
                        score *= seed;
                        break;
                    case RoundMode.無條件進位:
                        decimal d2 = GetRoundScore(score, decimals, RoundMode.無條件捨去);
                        if (d2 != score)
                            score = d2 + seed;
                        else
                            score = d2;
                        break;
                }
                string ss = "0.";
                for (int i = 0; i < decimals; i++)
                {
                    ss += "0";
                }
                return Convert.ToDecimal(Math.Round(score, decimals).ToString(ss));
            }
        }
    }
}
