using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Rating;

namespace KaoHsiung.ClassExamScoreReportV2
{
    /// <summary>
    /// 代表單一學期相關成績的運算邏輯。
    /// </summary>
    internal class ScoreCalculator
    {
        private int RoundPosition { get; set; }

        /// <summary>
        /// 運算完成時儲存成績的 Token。
        /// </summary>
        private string TargetToken { get; set; }

        public IEnumerable<string> Subjects { get; set; }

        private string SubjectToken { get; set; }

        public IEnumerable<string> Domains { get; set; }

        private string DomainToken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="round">進位。</param>
        /// <param name="targetToken">完成計算後的成績儲存 Token。</param>
        public ScoreCalculator(int round, string targetToken)
        {
            RoundPosition = round;
            TargetToken = targetToken;

            Subjects = new List<string>();
            Domains = new List<string>();
            SubjectToken = Utilities.SubjectToken;
            DomainToken = Utilities.DomainToken;

        }

        #region IScoreParser<ReportStudent> 成員

        public void CalculateScore(ReportStudent student)
        {
            ItemWeightCollection subjectWeight = new ItemWeightCollection();
            foreach (string each in Subjects)
            {
                if (student.Scores[SubjectToken].Contains(each))
                    subjectWeight.Add(each, student.Scores[SubjectToken].Weights[each]);
            }

            ItemWeightCollection domainWeight = new ItemWeightCollection();
            foreach (string each in Domains)
            {
                if (student.Scores[DomainToken].Contains(each))
                    domainWeight.Add(each, student.Scores[DomainToken].Weights[each]);
            }

            if (subjectWeight.Count <= 0 && domainWeight.Count <0) return;

            student.Scores[TargetToken].Clear();
            student.Scores[TargetToken].Add("加權平均", 加權平均(student, subjectWeight, domainWeight), 0);
            student.Scores[TargetToken].Add("加權總分", 加權總分(student, subjectWeight, domainWeight), 0);
            student.Scores[TargetToken].Add("合計總分", 合計總分(student, subjectWeight, domainWeight), 0);
            student.Scores[TargetToken].Add("算術平均", 算術平均(student, subjectWeight, domainWeight), 0);
        }

        private decimal 算術平均(ReportStudent student, ItemWeightCollection subjectWeight, ItemWeightCollection domainWeight)
        {
            decimal sum = 0, weight = subjectWeight.Count + domainWeight.Count;

            foreach (string scoreItem in subjectWeight.Keys)
                sum += student.Scores[SubjectToken][scoreItem];

            foreach (string scoreItem in domainWeight.Keys)
                sum += student.Scores[DomainToken][scoreItem];

            if (weight == 0)
                return 0;
            else
                return Round(sum / weight);
        }

        private decimal 合計總分(ReportStudent student, ItemWeightCollection subjectWeight, ItemWeightCollection domainWeight)
        {
            decimal sum = 0;

            foreach (string scoreItem in subjectWeight.Keys)
                sum += student.Scores[SubjectToken][scoreItem];

            foreach (string scoreItem in domainWeight.Keys)
                sum += student.Scores[DomainToken][scoreItem];

            return sum;
        }

        private decimal 加權總分(ReportStudent student, ItemWeightCollection subjectWeight, ItemWeightCollection domainWeight)
        {
            decimal sum = 0;

            foreach (string scoreItem in subjectWeight.Keys)
                sum += (student.Scores[SubjectToken][scoreItem] * subjectWeight[scoreItem]);

            foreach (string scoreItem in domainWeight.Keys)
                sum += (student.Scores[DomainToken][scoreItem] * domainWeight[scoreItem]);

            return sum;
        }

        private decimal 加權平均(ReportStudent student, ItemWeightCollection subjectWeight, ItemWeightCollection domainWeight)
        {
            decimal sum = 0, weight = subjectWeight.GetWeightSum() + domainWeight.GetWeightSum();

            foreach (string scoreItem in subjectWeight.Keys)
                sum += (student.Scores[SubjectToken][scoreItem] * subjectWeight[scoreItem]);

            foreach (string scoreItem in domainWeight.Keys)
                sum += (student.Scores[DomainToken][scoreItem] * domainWeight[scoreItem]);

            if (weight == 0)
                return 0;
            else
                return Round(sum / weight);
        }

        private decimal Round(decimal score)
        {
                return Math.Round(score, RoundPosition, MidpointRounding.AwayFromZero);

            //return score;

            //if (!score.HasValue) return null;

            //decimal seed = Convert.ToDecimal(Math.Pow(0.1, Convert.ToDouble(2)));

            //decimal s = score / seed;
            //s = decimal.Floor(s);
            //s *= seed;

            //return s;
        }
        #endregion

        public enum CalcMethod
        {
            加權總分,
            加權平均,
            合計總分,
            算術平均
        }
    }
}
