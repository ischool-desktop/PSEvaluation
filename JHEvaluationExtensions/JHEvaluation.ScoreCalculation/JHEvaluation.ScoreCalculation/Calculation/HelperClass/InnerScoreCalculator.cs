using System.Collections.Generic;
using System.Text;
using System.Xml;
using Framework;

namespace JHSchool.Evaluation.Calculation
{
    internal class InnerScoreCalculator
    {
        //努力程度對照表
        private Dictionary<decimal, int> _effortDict;
        private List<decimal> _effortScoreList;

        private InnerScoreCalculator()
        {
            #region 取得努力程度
            _effortDict = new Dictionary<decimal, int>();
            _effortScoreList = new List<decimal>();

            ConfigData cd = School.Configuration["努力程度對照表"];
            if (!string.IsNullOrEmpty(cd["xml"]))
            {
                XmlElement element = XmlHelper.LoadXml(cd["xml"]);

                foreach (XmlElement each in element.SelectNodes("Effort"))
                {
                    int code = int.Parse(each.GetAttribute("Code"));
                    decimal score;
                    if (!decimal.TryParse(each.GetAttribute("Score"), out score))
                        score = 0;

                    if (!_effortDict.ContainsKey(score))
                        _effortDict.Add(score, code);
                }

                _effortScoreList.AddRange(_effortDict.Keys);
                _effortScoreList.Sort(delegate(decimal a, decimal b)
                {
                    return b.CompareTo(a);
                });
            }
            #endregion
        }

        private List<K12.Data.DomainScore> CalculateDomainScore(List<K12.Data.SubjectScore> subjectScoreList)
        {
            Dictionary<string, DomainCalc> domainScores = new Dictionary<string, DomainCalc>();

            foreach (K12.Data.SubjectScore subject in subjectScoreList)
            {
                if (!domainScores.ContainsKey(subject.Domain))
                    domainScores.Add(subject.Domain, new DomainCalc());

                domainScores[subject.Domain].AddScore(subject.Score, (decimal)subject.Credit, (decimal)subject.Period);
                domainScores[subject.Domain].AddText(subject.Subject, subject.Text);
            }

            List<K12.Data.DomainScore> list = new List<K12.Data.DomainScore>();

            foreach (string key in domainScores.Keys)
            {
                K12.Data.DomainScore domain = new K12.Data.DomainScore();
                domain.Domain = key;
                domain.Credit = domainScores[key].Credit;
                domain.Period = domainScores[key].Period;
                domain.Text = domainScores[key].GetText();
                domain.Score = domainScores[key].GetScore();
                domain.Effort = GetEffortCode((decimal)domain.Score);

                list.Add(domain);
            }

            return list;
        }

        private List<K12.Data.SubjectScore> CalculateSubjectScore(List<SCAttendRecord> scattendList)
        {
            Dictionary<string, SubjectCalc> subjectScores = new Dictionary<string, SubjectCalc>();

            foreach (SCAttendRecord attend in scattendList)
            {
                CourseRecord course = attend.Course;
                if (!subjectScores.ContainsKey(course.Subject))
                    subjectScores.Add(course.Subject, new SubjectCalc(course.Domain, attend.Score, attend.Effort, course.Credit, course.Period, attend.Text));
            }

            List<K12.Data.SubjectScore> list = new List<K12.Data.SubjectScore>();

            foreach (string key in subjectScores.Keys)
            {
                K12.Data.SubjectScore subject = new K12.Data.SubjectScore();
                subject.Subject = key;
                subject.Domain = subjectScores[key].Domain;
                subject.Credit = subjectScores[key].Credit;
                subject.Period = subjectScores[key].Period;
                subject.Text = subjectScores[key].Text;
                subject.Score = subjectScores[key].Score;
                subject.Effort = subjectScores[key].Effort;

                list.Add(subject);
            }

            return list;
        }

        private int GetEffortCode(decimal d)
        {
            foreach (decimal score in _effortScoreList)
                if (d >= score) return _effortDict[score];

            return _effortDict[_effortScoreList[_effortScoreList.Count - 1]];
        }

        private decimal CalculateTotalDomainScore(List<K12.Data.DomainScore> domainScoreList)
        {
            decimal score = decimal.Zero;
            decimal credit = 0;

            foreach (K12.Data.DomainScore domain in domainScoreList)
            {
                score += (decimal)domain.Score * (decimal)domain.Credit;
                credit += (decimal)domain.Credit;
            }

            if (credit > 0) return score / credit;
            else return decimal.Zero;
        }

        class DomainCalc
        {
            private decimal Score { get; set; }
            public decimal Credit { get; private set; }
            public decimal Period { get; private set; }
            private Dictionary<string, string> _texts = new Dictionary<string, string>();

            public DomainCalc()
            {
                Score = decimal.Zero;
                Credit = decimal.Zero;
                Period = decimal.Zero;
            }

            public void AddScore(decimal? score, decimal credit, decimal period)
            {
                Score += (decimal)score * credit;
                Credit += credit;
                Period += period;
            }

            public decimal GetScore()
            {
                if (Credit > 0) return Score / Credit;
                else return decimal.Zero;
            }

            public void AddText(string subject, string text)
            {
                if (!_texts.ContainsKey(subject))
                    _texts.Add(subject, text);
            }

            public string GetText()
            {
                StringBuilder builder = new StringBuilder("");

                foreach (string text in _texts.Keys)
                {
                    if (!string.IsNullOrEmpty(_texts[text]))
                    {
                        builder.Append("<" + text + ">");
                        builder.Append(_texts[text]);
                    }
                }

                return builder.ToString();
            }
        }

        class SubjectCalc
        {
            public string Domain { get; private set; }
            public decimal Score { get; private set; }
            public int Effort { get; private set; }
            public decimal Credit { get; private set; }
            public decimal Period { get; private set; }
            public string Text { get; private set; }

            public SubjectCalc()
            {
                Domain = "彈性課程";
                Score = decimal.Zero;
                Effort = 0;
                Credit = decimal.Zero;
                Period = decimal.Zero;
                Text = string.Empty;
            }

            public SubjectCalc(string domain, decimal? score, int? effort, string credit, string period, string text)
                : this()
            {
                Domain = domain;
                if (score.HasValue)
                    Score = score.Value;
                if (effort.HasValue)
                    Effort = effort.Value;
                decimal i;
                if (decimal.TryParse(credit, out i))
                    Credit = i;
                if (decimal.TryParse(period, out i))
                    Period = i;
                Text = text;
            }
        }
    }
}
