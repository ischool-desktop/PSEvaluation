using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Editor;
using Framework;

namespace JHSchool.Evaluation.Editor
{
    public class SemesterScoreRecordEditor
    {
        public bool Remove { get; set; }
        internal string RefStudentID { get; set; }
        public string ID { get; private set; }
        internal int SchoolYear { get; set; }
        internal int Semester { get; set; }
        public int GradeYear { get; set; }
        public decimal? LearnDomainScore { get; set; }
        public decimal? CourseLearnScore { get; set; }
        public Dictionary<string, DomainScore> Domains { get; set; }
        public Dictionary<string, SubjectScore> Subjects { get; set; }

        public StudentRecord Student
        {
            get { return JHSchool.Student.Instance[RefStudentID]; }
        }

        public EditorStatus EditorStatus
        {
            get
            {
                if (SemesterSubjectScoreRecord == null)
                {
                    if (!Remove)
                        return EditorStatus.Insert;
                    else
                        return EditorStatus.NoChanged;
                }
                else
                {
                    if (Remove)
                        return EditorStatus.Delete;
                    if (SemesterSubjectScoreRecord.SchoolYear != SchoolYear ||
                        SemesterSubjectScoreRecord.Semester != Semester ||
                        SemesterSubjectScoreRecord.GradeYear != GradeYear ||
                        SemesterSubjectScoreRecord.Domains != Domains ||
                        SemesterSubjectScoreRecord.LearnDomainScore != LearnDomainScore ||
                        SemesterSubjectScoreRecord.CourseLearnScore != CourseLearnScore ||
                        SemesterSubjectScoreRecord.Subjects != Subjects ||
                        SemesterSubjectScoreRecord.Domains != Domains
                        )
                    {
                        return EditorStatus.Update;
                    }
                }
                return EditorStatus.NoChanged;
            }
        }

        public void Save()
        {
            if (EditorStatus != EditorStatus.NoChanged)
                Feature.EditStudent.SaveSemesterScoreRecordEditor(new SemesterScoreRecordEditor[] { this });
        }

        internal SemesterScoreRecord SemesterSubjectScoreRecord { get; set; }

        internal SemesterScoreRecordEditor(SemesterScoreRecord info)
        {
            SemesterSubjectScoreRecord = info;

            RefStudentID = info.RefStudentID;
            ID = info.ID;
            SchoolYear = info.SchoolYear;
            Semester = info.Semester;
            GradeYear = info.GradeYear;
            LearnDomainScore = info.LearnDomainScore;
            CourseLearnScore = info.CourseLearnScore;

            Dictionary<string, SubjectScore> subjectList = new Dictionary<string, SubjectScore>();
            foreach (var subject in info.Subjects.Values)
                subjectList.Add(subject.Subject, subject.Clone() as SubjectScore);
            Subjects = subjectList;

            Dictionary<string, DomainScore> domainList = new Dictionary<string, DomainScore>();
            foreach (var domain in info.Domains.Values)
                domainList.Add(domain.Domain, domain.Clone() as DomainScore);
            Domains = domainList;
        }

        internal SemesterScoreRecordEditor(StudentRecord student, int schoolYear, int semester, int gradeYear)
        {
            RefStudentID = student.ID;
            SchoolYear = schoolYear;
            Semester = semester;
            GradeYear = gradeYear;

            Subjects = new Dictionary<string, SubjectScore>();
            Domains = new Dictionary<string, DomainScore>();
        }
    }

    public static class SemesterSubjectScoreRecord_ExtendFunctions
    {
        public static SemesterScoreRecordEditor GetEditor(this SemesterScoreRecord semesterSubjectScoreRecord)
        {
            return new SemesterScoreRecordEditor(semesterSubjectScoreRecord);
        }
        public static void SaveAllEditors(this IEnumerable<SemesterScoreRecordEditor> editors)
        {
            Feature.EditStudent.SaveSemesterScoreRecordEditor(editors);
        }
        public static SemesterScoreRecordEditor AddSemesterSubjectScore(this StudentRecord studentRec, int schoolYear, int semester, int gradeYear)
        {
            return new SemesterScoreRecordEditor(studentRec, schoolYear, semester, gradeYear);
        }
    }
}