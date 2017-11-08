using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Editor;

namespace JHSchool.Evaluation.Editor
{
    public class GradScoreRecordEditor
    {
        public bool Remove { get; set; }
        public string RefStudentID { get; set; }
        public Dictionary<string, GradDomainScore> Domains { get; set; }
        public decimal? LearnDomainScore { get; set; }
        public decimal? CourseLearnScore { get; set; }

        public StudentRecord Student
        {
            get { return JHSchool.Student.Instance[RefStudentID]; }
        }

        public EditorStatus EditorStatus
        {
            get
            {
                if (GradScoreRecord == null)
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
                    if (GradScoreRecord.LearnDomainScore != LearnDomainScore ||
                        GradScoreRecord.CourseLearnScore != CourseLearnScore ||
                        GradScoreRecord.Domains != Domains
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
                Feature.EditStudent.SaveGradScoreRecordEditor(new GradScoreRecordEditor[] { this });
        }

        internal GradScoreRecord GradScoreRecord { get; set; }

        internal GradScoreRecordEditor(GradScoreRecord info)
        {
            GradScoreRecord = info;

            RefStudentID = info.RefStudentID;
            LearnDomainScore = info.LearnDomainScore;
            CourseLearnScore = info.CourseLearnScore;

            Dictionary<string, GradDomainScore> domainList = new Dictionary<string, GradDomainScore>();
            foreach (var domain in info.Domains.Values)
                domainList.Add(domain.Domain, domain.Clone() as GradDomainScore);
            Domains = domainList;
        }

        public GradScoreRecordEditor(StudentRecord student)
        {
            RefStudentID = student.ID;
            Domains = new Dictionary<string, GradDomainScore>();
        }
    }

    public static class GradScoreRecord_ExtendFunctions
    {
        public static GradScoreRecordEditor GetEditor(this GradScoreRecord gradScoreRecord)
        {
            return new GradScoreRecordEditor(gradScoreRecord);
        }
        public static void SaveAllEditors(this IEnumerable<GradScoreRecordEditor> editors)
        {
            Feature.EditStudent.SaveGradScoreRecordEditor(editors);
        }
    }
}
