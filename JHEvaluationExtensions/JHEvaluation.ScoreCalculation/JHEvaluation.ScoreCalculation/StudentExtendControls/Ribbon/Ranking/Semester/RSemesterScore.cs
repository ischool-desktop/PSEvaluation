using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking.Semester
{
    class RSemesterScore
    {
        public const string LearnDomainName = "學習領域";
        public const string CourseLearnName = "課程學習";

        public RSemesterScore(JHSemesterScoreRecord record)
        {
            Subjects = new List<RSubjectScore>();
            Domains = new List<RDomainScore>();

            RefStudentID = record.RefStudentID;
            SchoolYear = record.SchoolYear;
            Semester = record.Semester;

            foreach (K12.Data.SubjectScore each in record.Subjects.Values)
                Subjects.Add(new RSubjectScore(each));

            foreach (K12.Data.DomainScore each in record.Domains.Values)
                Domains.Add(new RDomainScore(each));

            Domains.Add(new RDomainScore(LearnDomainName, record.LearnDomainScore));
            Domains.Add(new RDomainScore(CourseLearnName, record.CourseLearnScore));
        }

        public string RefStudentID { get; private set; }

        public int SchoolYear { get; private set; }

        public int Semester { get; private set; }

        public List<RSubjectScore> Subjects { get; private set; }

        public List<RDomainScore> Domains { get; private set; }
    }
}
