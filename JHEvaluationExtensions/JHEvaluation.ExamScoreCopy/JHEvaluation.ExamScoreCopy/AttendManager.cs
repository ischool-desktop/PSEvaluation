using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using JHSchool.Data;

namespace JHEvaluation.ExamScoreCopy
{
    /// <summary>
    /// 學生修課及成績資料管理
    /// </summary>
    internal class AttendManager
    {
        private static AttendManager _Instance;
        public static AttendManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new AttendManager();
                return _Instance;
            }
        }

        private List<JHStudentRecord> Students { get; set; }
        private List<JHSCAttendRecord> Attends { get; set; }

        private BackgroundWorker ScoreWorker { get; set; }

        private Dictionary<string, StudentData> Data { get; set; }
        public UserConfig Config { get; set; }

        private AttendManager()
        {
            Students = new List<JHStudentRecord>();
            Attends = new List<JHSCAttendRecord>();
        }

        public void Set(List<JHStudentRecord> students, List<JHSCAttendRecord> attends)
        {
            Students = students;
            Attends = attends;
            Data = null;
        }

        public void ClearData()
        {
            Data = null;
        }

        /// <summary>
        /// 取得學生修課及成績資料
        /// </summary>
        /// <returns></returns>
        public List<StudentData> GetStudentDataList()
        {
            if (Data != null) return new List<StudentData>(Data.Values);

            Data = new Dictionary<string, StudentData>();

            foreach (JHStudentRecord student in Students)
            {
                StudentData sd = new StudentData(student);
                if (!Data.ContainsKey(student.ID))
                    Data.Add(student.ID, sd);
            }

            foreach (JHSCAttendRecord attend in Attends)
            {
                if (Data.ContainsKey(attend.RefStudentID))
                    Data[attend.RefStudentID].Attends.Add(attend);
            }

            return new List<StudentData>(Data.Values);
        }

        private List<string> GetAttendIDs(List<string> subjects)
        {
            List<string> IDs = new List<string>();
            foreach (StudentData sd in Data.Values)
                IDs.AddRange(sd.GetAttendIDs(subjects));
            return IDs;
        }

        internal void DeleteIgnoreStudents(List<string> ignoreIDs)
        {
            foreach (string id in ignoreIDs)
                Data.Remove(id);
        }

        /// <summary>
        /// 讀取評量成績
        /// </summary>
        /// <param name="reporter"></param>
        internal void ReadScores(IReporter reporter)
        {
            List<string> subjects = new List<string>(Config.TargetSubjects);
            subjects.Add(Config.SourceSubject);

            List<string> attendIDs = GetAttendIDs(subjects);

            FunctionSpliter<string, JHSCETakeRecord> spliter = new FunctionSpliter<string, JHSCETakeRecord>(200, Util.MaxThread);
            spliter.Function = delegate(List<string> part)
            {
                return JHSCETake.Select(null, null, new string[] { Config.Exam.ID }, null, part);
            };
            spliter.ProgressChange = delegate(int progress)
            {
                reporter.Feedback("讀取評量成績中...", Util.CalculatePercentage(attendIDs.Count, progress));
            };

            foreach(JHSCETakeRecord score in spliter.Execute(attendIDs))
            {
                if (Data.ContainsKey(score.RefStudentID))
                    Data[score.RefStudentID].Scores.Add(score);
            }
        }
    }

    /// <summary>
    /// 學生資料，包含修課及評量成績
    /// </summary>
    internal class StudentData
    {
        public JHStudentRecord Student { get; set; }
        public AttendData Attends { get; private set; }
        public ScoreData Scores { get; private set; }

        public StudentData(JHStudentRecord student)
        {
            Student = student;
            Attends = new AttendData();
            Scores = new ScoreData();
        }

        public List<string> GetAttendIDs(List<string> subjects)
        {
            List<string> IDs = new List<string>();
            foreach (string subject in subjects)
            {
                if (Attends.SubjectExists(subject))
                    IDs.Add(Attends[subject].ID);
            }
            return IDs;
        }

        public List<JHSCETakeRecord> GetCopiedScores(string source, List<string> targets)
        {
            if (!Scores.SubjectExists(source)) return new List<JHSCETakeRecord>();

            List<JHSCETakeRecord> list = new List<JHSCETakeRecord>();

            SCETakeData sourceScore = new SCETakeData(Scores[source]);
            foreach (string subject in targets)
            {
                if (Scores.SubjectExists(subject))
                {
                    SCETakeData sce = new SCETakeData(Scores[subject]);
                    sce.Score = sourceScore.Score;
                    sce.Text = sourceScore.Text;
                    sce.AssignmentScore = sourceScore.AssignmentScore;
                    sce.Effort = sourceScore.Effort;

                    list.Add(sce.AsJHSCETakeRecord());
                }
                else
                {
                    SCETakeData sce = new SCETakeData(new JHSCETakeRecord());
                    JHSCAttendRecord attend = Attends[subject];
                    sce.Origin.RefCourseID = attend.RefCourseID;
                    sce.Origin.RefExamID = sourceScore.Origin.RefExamID;
                    sce.Origin.RefSCAttendID = attend.ID;
                    sce.Origin.RefStudentID = attend.RefStudentID;
                    sce.Score = sourceScore.Score;
                    sce.Text = sourceScore.Text;
                    sce.AssignmentScore = sourceScore.AssignmentScore;
                    sce.Effort = sourceScore.Effort;

                    list.Add(sce.AsJHSCETakeRecord());
                }
            }
            return list;
        }
    }

    /// <summary>
    /// 修課成績資料
    /// </summary>
    internal class AttendData
    {
        Dictionary<string, JHSCAttendRecord> Attends { get; set; }

        public AttendData()
        {
            Attends = new Dictionary<string, JHSCAttendRecord>();
        }

        public void Add(JHSCAttendRecord attend)
        {
            string subject = JHCourse.SelectByID(attend.RefCourseID).Subject;
            if (!Attends.ContainsKey(subject))
                Attends.Add(subject, attend);
        }

        public bool SubjectExists(string subject)
        {
            return Attends.ContainsKey(subject);
        }

        public JHSCAttendRecord this[string subject]
        {
            get { return Attends[subject]; }
        }
    }

    /// <summary>
    /// 評量成績資料
    /// </summary>
    internal class ScoreData
    {
        Dictionary<string, JHSCETakeRecord> Scores { get; set; }

        public ScoreData()
        {
            Scores = new Dictionary<string, JHSCETakeRecord>();
        }

        public void Add(JHSCETakeRecord score)
        {
            string subject = JHCourse.SelectByID(score.RefCourseID).Subject;
            if (!Scores.ContainsKey(subject))
                Scores.Add(subject, score);
        }

        public bool SubjectExists(string subject)
        {
            return Scores.ContainsKey(subject);
        }

        public JHSCETakeRecord this[string subject]
        {
            get { return Scores[subject]; }
        }
    }
}
