using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Framework;

namespace JHSchool.Evaluation
{
    public class GradScoreRecord
    {
        public string RefStudentID { get; private set; }
        public Dictionary<string, GradDomainScore> Domains { get; private set; }
        public decimal? LearnDomainScore { get; private set; }
        public decimal? CourseLearnScore { get; private set; }

        public StudentRecord Student
        {
            get { return JHSchool.Student.Instance[RefStudentID]; }
        }

        public GradScoreRecord(XmlElement element)
        {
            XmlHelper helper = new XmlHelper(element);

            RefStudentID = helper.GetString("@ID");

            Domains = new Dictionary<string, GradDomainScore>();
            foreach (var domainElement in helper.GetElements("GradScore/GradScore/Domain"))
            {
                GradDomainScore domainScore = new GradDomainScore(domainElement);
                Domains.Add(domainScore.Domain, domainScore);
            }

            decimal score;
            if (decimal.TryParse(helper.GetString("GradScore/GradScore/LearnDomainScore"), out score))
                LearnDomainScore = score;
            if (decimal.TryParse(helper.GetString("GradScore/GradScore/CourseLearnScore"), out score))
                CourseLearnScore = score;
        }

        public GradScoreRecord()
        {
        }
    }

    public class GradDomainScore : ICloneable
    {
        public string Domain { get; private set; }
        public decimal? Score { get; set; }

        public GradDomainScore(XmlElement element)
        {
            Domain = element.GetAttribute("Name");
            decimal d;
            if (decimal.TryParse(element.GetAttribute("Score"), out d))
                Score = d;
        }

        public GradDomainScore(string domain)
        {
            Domain = domain;
        }


        #region ICloneable 成員

        public object Clone()
        {
            GradDomainScore newScore = new GradDomainScore(Domain);
            newScore.Score = this.Score;
            return newScore;
        }

        #endregion
    }
}
