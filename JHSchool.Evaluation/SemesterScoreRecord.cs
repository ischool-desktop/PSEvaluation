using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Framework;

namespace JHSchool.Evaluation
{
    public class SemesterScoreRecord
    {
        internal string RefStudentID { get; private set; }
        public string ID { get; private set; }
        public int SchoolYear { get; private set; }
        public int Semester { get; private set; }
        [Obsolete("學期成績上的成績年級已不再使用。")]
        public int GradeYear { get; private set; }
        public decimal? LearnDomainScore { get; private set; }
        public decimal? CourseLearnScore { get; private set; }
        public Dictionary<string, SubjectScore> Subjects { get; private set; }
        public Dictionary<string, DomainScore> Domains { get; private set; }

        public StudentRecord Student { get { return JHSchool.Student.Instance[RefStudentID]; } }

        internal SemesterScoreRecord(XmlElement element)
        {
            XmlHelper helper = new XmlHelper(element);

            RefStudentID = helper.GetString("RefStudentId");
            ID = helper.GetString("@ID");

            SchoolYear = helper.GetInteger("SchoolYear", 0);
            Semester = helper.GetInteger("Semester", 0);
            GradeYear = helper.GetInteger("GradeYear", 0);

            Subjects = new Dictionary<string, SubjectScore>();
            foreach (var subjectElement in helper.GetElements("ScoreInfo/SemesterSubjectScoreInfo/Subject"))
            {
                SubjectScore subjectScore = new SubjectScore(subjectElement);
                Subjects.Add(subjectScore.Subject, subjectScore);
            }

            Domains = new Dictionary<string, DomainScore>();
            foreach (var domainElement in helper.GetElements("ScoreInfo/Domains/Domain"))
            {
                DomainScore domainScore = new DomainScore(domainElement);
                Domains.Add(domainScore.Domain, domainScore);
            }

            decimal score;
            if (decimal.TryParse(helper.GetString("ScoreInfo/LearnDomainScore"), out score))
                LearnDomainScore = score;
            if (decimal.TryParse(helper.GetString("ScoreInfo/CourseLearnScore"), out score))
                CourseLearnScore = score;
        }
    }

    /// <summary>
    /// 科目成績
    /// </summary>
    public class SubjectScore : ICloneable
    {
        public string Domain { get; set; }
        public string Subject { get; set; }
        public int Period { get; set; }
        public int Credit { get; set; }
        public decimal? Score { get; set; }
        public int? Effort { get; set; }
        public string Text { get; set; }
        public string Comment { get; set; }

        public SubjectScore(XmlElement subject)
        {
            int i = 0;
            decimal d = 0;
            Domain = subject.GetAttribute("領域");
            Subject = subject.GetAttribute("科目");
            Period = int.TryParse(subject.GetAttribute("節數"), out i) ? i : 0;
            Credit = int.TryParse(subject.GetAttribute("權數"), out i) ? i : 0;
            Score = decimal.TryParse(subject.GetAttribute("成績"), out d) ? (decimal?)d : null;
            Effort = int.TryParse(subject.GetAttribute("努力程度"), out i) ? (int?)i : null;
            Text = subject.GetAttribute("文字描述");
            Comment = subject.GetAttribute("註記");
        }

        public SubjectScore() { }

        #region ICloneable 成員

        public object Clone()
        {
            SubjectScore cloneSubjectScore = new SubjectScore();
            cloneSubjectScore.Domain = this.Domain;
            cloneSubjectScore.Subject = this.Subject;
            cloneSubjectScore.Period = this.Period;
            cloneSubjectScore.Credit = this.Credit;
            cloneSubjectScore.Score = this.Score;
            cloneSubjectScore.Effort = this.Effort;
            cloneSubjectScore.Text = this.Text;
            cloneSubjectScore.Comment = this.Comment;
            return cloneSubjectScore;
        }

        #endregion
    }

    /// <summary>
    /// 領域成績
    /// </summary>
    public class DomainScore : ICloneable
    {
        public string Domain { get; set; }
        public int? Period { get; set; }
        public int? Credit { get; set; }
        public decimal? Score { get; set; }
        public int? Effort { get; set; }
        public string Text { get; set; }
        public string Comment { get; set; }

        public DomainScore(XmlElement domain)
        {
            int i;
            decimal d;
            Domain = domain.GetAttribute("領域");
            Period = int.TryParse(domain.GetAttribute("節數"), out i) ? (int?)i : null;
            Credit = int.TryParse(domain.GetAttribute("權數"), out i) ? (int?)i : null;
            Score = decimal.TryParse(domain.GetAttribute("成績"), out d) ? (decimal?)d : null;
            Effort = int.TryParse(domain.GetAttribute("努力程度"), out i) ? (int?)i : null;
            Text = domain.GetAttribute("文字描述");
            Comment = domain.GetAttribute("註記");
        }

        public DomainScore() { }

        #region ICloneable 成員

        public object Clone()
        {
            DomainScore cloneDomainScore = new DomainScore();
            cloneDomainScore.Domain = this.Domain;
            cloneDomainScore.Period= this.Period;
            cloneDomainScore.Credit = this.Credit;
            cloneDomainScore.Score = this.Score;
            cloneDomainScore.Effort = this.Effort;
            cloneDomainScore.Text = this.Text;
            cloneDomainScore.Comment = this.Comment;
            return cloneDomainScore;
        }

        #endregion
    }
}
