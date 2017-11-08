using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;
using K12.Data;

namespace JHEvaluation.ScoreCalculation.ScoreStruct
{
    /// <summary>
    /// 代表一個學期的相關成績，包含科目成績、領域成績... 。
    /// </summary>
    public class SemesterScore
    {
        public SemesterScore(int schoolYear, int semester)
        {
            RawScore = null;
            Subject = new SemesterSubjectScoreCollection();
            Domain = new SemesterDomainScoreCollection();
            SchoolYear = schoolYear;
            Semester = semester;
            CourseLog = new LogData("課程學習");
            LearningLog = new LogData("學習領域");
            CourseOriginLog = new LogData("課程學習(原始)");
            LearningOriginLog = new LogData("學習領域(原始)");
        }

        public SemesterScore(JHSemesterScoreRecord record)
            : this(record.SchoolYear, record.Semester)
        {
            RawScore = record;

            foreach (SubjectScore eachSubject in RawScore.Subjects.Values)
            {
                string subjName = eachSubject.Subject.Trim();

                if (!Subject.Contains(subjName))
                    Subject.Add(subjName, new SemesterSubjectScore(eachSubject));
            }

            foreach (DomainScore eachDomian in RawScore.Domains.Values)
            {
                string domainName = eachDomian.Domain;

                if (!Domain.Contains(domainName))
                    Domain.Add(domainName, new SemesterDomainScore(eachDomian));
            }

            CourseLearnScore = RawScore.CourseLearnScore;
            LearnDomainScore = RawScore.LearnDomainScore;
            CourseLearnScoreOrigin = RawScore.CourseLearnScoreOrigin;
            LearnDomainScoreOrigin = RawScore.LearnDomainScoreOrigin;
        }

        /// <summary>
        /// 學年度。
        /// </summary>
        public int SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        public int Semester { get; set; }

        /// <summary>
        /// 學期成績的原始資料。
        /// </summary>
        public JHSemesterScoreRecord RawScore { get; private set; }

        /// <summary>
        /// 取得學生學期相關成績。
        /// </summary>
        public SemesterSubjectScoreCollection Subject { get; private set; }

        public bool SubjectScoreExists(string subjName)
        {
            if (!Subject.Contains(subjName)) return false;

            return Subject[subjName].Value.HasValue;
        }

        public decimal GetSubjectScore(string subjName)
        {
            return Subject[subjName].Value.Value;
        }

        /// <summary>
        /// 取得學生學期相關成績。
        /// </summary>
        public SemesterDomainScoreCollection Domain { get; private set; }

        public bool DomainScoreExists(string domainName)
        {
            if (!Domain.Contains(domainName)) return false;

            return Domain[domainName].Value.HasValue;
        }

        public decimal GetDomainScore(string domainName)
        {
            return Domain[domainName].Value.Value;
        }

        /// <summary>
        /// 學習領域成績。
        /// </summary>
        public decimal? LearnDomainScore { get; set; }

        /// <summary>
        /// 課程學期成績。 
        /// </summary>
        public decimal? CourseLearnScore { get; set; }

        /// <summary>
        /// 學習領域原始成績。
        /// </summary>
        public decimal? LearnDomainScoreOrigin { get; set; }

        /// <summary>
        /// 課程學期原始成績。 
        /// </summary>
        public decimal? CourseLearnScoreOrigin { get; set; }

        /// <summary>
        /// 課程學習。
        /// </summary>
        public LogData CourseLog { get; private set; }

        /// <summary>
        /// 學習領域。
        /// </summary>
        public LogData LearningLog { get; private set; }

        /// <summary>
        /// 課程學習(原始)。
        /// </summary>
        public LogData CourseOriginLog { get; private set; }

        /// <summary>
        /// 學習領域(原始)。
        /// </summary>
        public LogData LearningOriginLog { get; private set; }
    }
}
