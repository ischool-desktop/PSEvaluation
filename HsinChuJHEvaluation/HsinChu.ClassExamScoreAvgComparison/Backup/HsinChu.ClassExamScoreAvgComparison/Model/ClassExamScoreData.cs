using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;
using HsinChu.JHEvaluation.Data;
using JHEvaluation.ScoreCalculation;

namespace HsinChu.ClassExamScoreAvgComparison.Model
{
    internal class ClassExamScoreData
    {
        public JHClassRecord Class { get; private set; }
        public List<JHStudentRecord> Students { get; private set; }
        public StudentRows Rows { get; private set; }
        public List<string> ValidCourseIDs { get; private set; }

        private Dictionary<string, StudentScore> _StudentScoreDict;
        public ClassExamScoreData(JHClassRecord cla)
        {
            Class = cla;
            //Students = cla.Students;
            Students = new List<JHStudentRecord>();
            _StudentScoreDict = new Dictionary<string, StudentScore>();

            List<StudentScore> studentScores = new List<StudentScore>();
            foreach (JHStudentRecord student in cla.Students)
            {
                if (student.Status == K12.Data.StudentRecord.StudentStatus.一般 ||
                    student.Status == K12.Data.StudentRecord.StudentStatus.輟學)
                {
                    Students.Add(student);
                    studentScores.Add(new StudentScore(student));
                }
            }
            StudentScore.SetClassMapping();
            studentScores.ReadCalculationRule(null);
            _StudentScoreDict = new Dictionary<string, StudentScore>();
            foreach (var score in studentScores)
            {
                if (!_StudentScoreDict.ContainsKey(score.Id))
                    _StudentScoreDict.Add(score.Id, score);
            }

            Rows = new StudentRows();
            ValidCourseIDs = new List<string>();
        }

        public void AddScore(JHSCETakeRecord score)
        {
            if (!Rows.ContainsKey(score.RefStudentID))
                Rows.Add(score.RefStudentID, new StudentRow(_StudentScoreDict[score.RefStudentID]));
            Rows[score.RefStudentID].AddRawScore(score);
        }

        public void ConvertToCourseScores(List<string> courseIDs, string examID)
        {
            foreach (var row in Rows.Values)
            {
                foreach (string id in row.ConvertToCourseScores(courseIDs, examID))
                {
                    if (!ValidCourseIDs.Contains(id))
                        ValidCourseIDs.Add(id);
                }
            }
        }

        internal void Clear()
        {
            ValidCourseIDs.Clear();
            foreach (StudentRow row in Rows.Values)
            {
                row.Clear();
            }
        }

        internal void SortCourseIDs(List<string> courseIDs)
        {
            List<string> sortCourseIDs = new List<string>();
            foreach (var id in courseIDs)
            {
                if (ValidCourseIDs.Contains(id))
                    sortCourseIDs.Add(id);
            }
            ValidCourseIDs = sortCourseIDs;
        }
    }
}
