using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;
using JHSchool.Data;
using HsinChu.JHEvaluation.Data;
using JHEvaluation.ScoreCalculation;
using JHEvaluation.ScoreCalculation.ScoreStruct;

namespace HsinChu.ClassExamScoreReportV21
{
    internal class InternalExamScore
    {
        public static List<InternalExamScoreRecord> Select(IEnumerable<string> StudentIDs, UserOptions Options)
        {
            //key:CourseID
            Dictionary<string, JHCourseRecord> dictCourses = Utilities.GetCourseDict(Options.SchoolYear, Options.Semester);
            //key:AssessmentSetupID
            Dictionary<string, JHAEIncludeRecord> dictAEIncludes = Utilities.GetAEIncludeDict(dictCourses.Values.ToAssessmentSetupIDs(), Options.Exam);

            //類似學期成績的結構…
            Dictionary<string, InternalExamScoreRecord> dictStudentScores = new Dictionary<string, InternalExamScoreRecord>();

            // 取得評量比例
            Utilities.ScorePercentageHSDict = Utilities.GetScorePercentageHS();

            #region 取得及轉換評量科目成績
            int size = 200;
            int thread = 5;
            FunctionSpliter<string, JHSCETakeRecord> spliter = new FunctionSpliter<string, JHSCETakeRecord>(size, thread);
            spliter.Function = delegate(List<string> studentKeysPart)
            {
                return JHSCETake.Select(null, studentKeysPart, new string[] { Options.Exam.ID }, null, null);
            };

            foreach (JHSCETakeRecord sce in spliter.Execute(StudentIDs.ToList()))
            {
                if (!dictCourses.ContainsKey(sce.RefCourseID)) continue; //評量成績所屬課程非本學期，跳過
                if (Options.Exam.ID != sce.RefExamID) continue; //評量成績的試別不符，跳過
                JHCourseRecord course = dictCourses[sce.RefCourseID];
                if (!dictAEIncludes.ContainsKey(course.RefAssessmentSetupID)) continue; //如果課程沒有評量設定，跳過
                JHAEIncludeRecord ae = dictAEIncludes[course.RefAssessmentSetupID];

                //每個學生一個 InternalExamScoreRecord
                if (!dictStudentScores.ContainsKey(sce.RefStudentID))
                    dictStudentScores.Add(sce.RefStudentID, new InternalExamScoreRecord(sce.RefStudentID));

                if (!dictStudentScores[sce.RefStudentID].Subjects.ContainsKey(course.Subject))
                {
                    SubjectScore subjectScore = new SubjectScore();
                    subjectScore.Domain = course.Domain;
                    subjectScore.Subject = course.Subject;
                    subjectScore.Period = course.Period;
                    subjectScore.Credit = course.Credit;
                    subjectScore.Score = Utilities.GetScore(new HC.JHSCETakeRecord(sce), new HC.JHAEIncludeRecord(ae), Options.ScoreSource);

                    if (subjectScore.Score.HasValue)
                        dictStudentScores[sce.RefStudentID].Subjects.Add(course.Subject, subjectScore);
                }
            }
            #endregion

            #region 計算評量領域成績
            JHSchool.Evaluation.Calculation.ScoreCalculator defaultCalculator = new JHSchool.Evaluation.Calculation.ScoreCalculator(null);

            StudentScore.SetClassMapping();
            List<StudentScore> Students = ToStudentScore(StudentIDs);
            Students.ReadCalculationRule(null);
            foreach (StudentScore student in Students)
            {
                student.SemestersScore.Add(SemesterData.Empty, new global::JHEvaluation.ScoreCalculation.ScoreStruct.SemesterScore(Options.SchoolYear, Options.Semester));

                if (!dictStudentScores.ContainsKey(student.Id)) continue;

                global::JHEvaluation.ScoreCalculation.ScoreStruct.SemesterScore semesterScore = student.SemestersScore[SemesterData.Empty];
                foreach (SubjectScore score in dictStudentScores[student.Id].Subjects.Values)
                {
                    //科目成績偷偷進位
                    if (score.Score.HasValue)
                    {
                        if (Options.ScoreSource == "定期加平時平均")
                        {
                            if (student.CalculationRule != null)
                                score.Score = student.CalculationRule.ParseSubjectScore(score.Score.Value);
                            else
                                score.Score = defaultCalculator.ParseSubjectScore(score.Score.Value);
                        }
                    }

                    if (!semesterScore.Subject.Contains(score.Subject))
                        semesterScore.Subject.Add(score.Subject, new SemesterSubjectScore(score));
                }
            }

            Students.CalcuateDomainSemesterScore(new string[] { });
            #endregion

            foreach (StudentScore student in Students)
            {
                if (!dictStudentScores.ContainsKey(student.Id))
                    dictStudentScores.Add(student.Id, new InternalExamScoreRecord(student.Id));
                InternalExamScoreRecord examScore = dictStudentScores[student.Id];

                examScore.Subjects = ToSubjects(student.SemestersScore[SemesterData.Empty].Subject);
                examScore.Domains = ToDomains(student.SemestersScore[SemesterData.Empty].Domain);
            }

            return new List<InternalExamScoreRecord>(dictStudentScores.Values);
        }

        private static Dictionary<string, SubjectScore> ToSubjects(SemesterSubjectScoreCollection semesterSubjectScoreCollection)
        {
            Dictionary<string, SubjectScore> dictSubjectScore = new Dictionary<string, SubjectScore>();

            foreach (string key in semesterSubjectScoreCollection)
            {
                SemesterSubjectScore score = semesterSubjectScoreCollection[key];
                SubjectScore subjectScore = new SubjectScore();
                subjectScore.Domain = score.Domain;
                subjectScore.Subject = key;
                subjectScore.Period = score.Period;
                subjectScore.Credit = score.Weight;
                subjectScore.Score = score.Value;
                dictSubjectScore.Add(subjectScore.Subject, subjectScore);
            }

            return dictSubjectScore;
        }

        private static Dictionary<string, DomainScore> ToDomains(SemesterDomainScoreCollection semesterDomainScoreCollection)
        {
            Dictionary<string, DomainScore> dictDomainScore = new Dictionary<string, DomainScore>();

            foreach (string key in semesterDomainScoreCollection)
            {
                SemesterDomainScore score = semesterDomainScoreCollection[key];
                DomainScore domainScore = new DomainScore();
                domainScore.Domain = key;
                domainScore.Period = score.Period;
                domainScore.Credit = score.Weight;
                domainScore.Score = score.Value;
                dictDomainScore.Add(domainScore.Domain, domainScore);
            }

            return dictDomainScore;
        }

        private static List<StudentScore> ToStudentScore(IEnumerable<string> studentIDs)
        {
            List<StudentScore> students = new List<StudentScore>();
            foreach (JHStudentRecord each in JHStudent.SelectByIDs(studentIDs))
                students.Add(new StudentScore(each));
            return students;
        }
    }

    internal class InternalExamScoreRecord
    {
        public InternalExamScoreRecord(string studentID)
        {
            RefStudentID = studentID;
            Subjects = new Dictionary<string, SubjectScore>();
        }

        public string RefStudentID { get; set; }
        public Dictionary<string, SubjectScore> Subjects { get; set; }
        public Dictionary<string, DomainScore> Domains { get; set; }
    }
}
