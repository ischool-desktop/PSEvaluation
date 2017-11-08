using System;
using System.Collections.Generic;
using System.Text;
using SemesterSubjectScore = JHEvaluation.ScoreCalculation.ScoreStruct.SemesterSubjectScore;
using SemesterDomainScore = JHEvaluation.ScoreCalculation.ScoreStruct.SemesterDomainScore;
using SCSemsScore = JHEvaluation.ScoreCalculation.ScoreStruct.SemesterScore;
using K12.Data;
using JHSchool.Data;
using JHEvaluation.ScoreCalculation.ScoreStruct;

namespace JHEvaluation.ScoreCalculation.BigFunction
{
    class SemesterScoreSaver
    {
        private IStatusReporter Reporter { get; set; }

        private List<StudentScore> Students { get; set; }

        public SemesterScoreSaver(List<StudentScore> students, IStatusReporter reporter)
        {
            Reporter = reporter;
            Students = students;
        }

        public void Save()
        {
            List<JHSemesterScoreRecord> addSemsScore = new List<JHSemesterScoreRecord>();
            List<JHSemesterScoreRecord> updateSemsScore = new List<JHSemesterScoreRecord>();
            SubjectScoreLogFormater subjLogFormater = new SubjectScoreLogFormater();
            DomainScoreLogFormater domainLogFormater = new DomainScoreLogFormater();

            foreach (StudentScore student in Students)
            {
                #region 決定要新增還是更新。
                JHSemesterScoreRecord JHScore = GetJHSemesterScore(student.Id, student.SemestersScore[SemesterData.Empty]);
                SCSemsScore SCScore = student.SemestersScore[SemesterData.Empty];

                if (string.IsNullOrEmpty(JHScore.ID))
                    addSemsScore.Add(JHScore);
                else
                    updateSemsScore.Add(JHScore);
                #endregion

                #region 產生科目資料。
                JHScore.Subjects.Clear();
                foreach (string strSubject in SCScore.Subject)
                {
                    SemesterSubjectScore objSCSubject = SCScore.Subject[strSubject];
                    SubjectScore objJHSubject = GetJHSubjectScore(strSubject, objSCSubject);
                    LogData subjLog = new LogData(strSubject);
                    subjLog.Formater = subjLogFormater;

                    decimal? score = objSCSubject.Value.HasValue ? (decimal?)(double)objSCSubject.Value : null;

                    //記錄 Log
                    subjLog.Add(new LogData("成績", objJHSubject.Score + "", score.ToString()));
                    subjLog.Add(new LogData("權重", objJHSubject.Credit + "", objSCSubject.Weight + ""));
                    subjLog.Add(new LogData("節數", objJHSubject.Period + "", objSCSubject.Period + ""));
                    if (Program.Mode == ModuleMode.KaoHsiung)
                        subjLog.Add(new LogData("努力程度", objJHSubject.Effort + "", objSCSubject.Effort + ""));
                    subjLog.Add(new LogData("文字評量", objJHSubject.Text + "", objSCSubject.Text));
                    subjLog.Add(new LogData("領域", objJHSubject.Domain + "", objSCSubject.Domain));
                    SCScore.Subject.Log.Add(subjLog);

                    objJHSubject.Score = score;
                    objJHSubject.Credit = objSCSubject.Weight;
                    objJHSubject.Period = objSCSubject.Period;
                    objJHSubject.Effort = objSCSubject.Effort;
                    objJHSubject.Text = objSCSubject.Text;
                    objJHSubject.Domain = objSCSubject.Domain;

                    JHScore.Subjects.Add(strSubject, objJHSubject);
                }

                //排序科目名稱。
                Dictionary<string, SubjectScore> orderSubject = new Dictionary<string, SubjectScore>(JHScore.Subjects);
                JHScore.Subjects.Clear();
                foreach (string subjName in Util.SortSubjectDomain(orderSubject.Keys))
                    JHScore.Subjects.Add(subjName, orderSubject[subjName]);
                #endregion

                #region 產生領域資料。
                JHScore.Domains.Clear();
                foreach (string strDomain in SCScore.Domain)
                {
                    //彈性課程不記錄領域領域。
                    if (Util.IsVariableDomain(strDomain)) continue;

                    SemesterDomainScore objSCDomain = SCScore.Domain[strDomain];
                    DomainScore objJHDomain = GetJHDomainScore(strDomain, objSCDomain);
                    LogData domainLog = new LogData(strDomain);
                    domainLog.Formater = subjLogFormater;

                    decimal? score = objSCDomain.Value.HasValue ? (decimal?)(double)objSCDomain.Value : null;

                    //記錄 Log
                    domainLog.Add(new LogData("成績", objJHDomain.Score + "", score + ""));
                    domainLog.Add(new LogData("權重", objJHDomain.Credit + "", objSCDomain.Weight + ""));
                    domainLog.Add(new LogData("節數", objJHDomain.Period + "", objSCDomain.Period + ""));
                    if (Program.Mode == ModuleMode.KaoHsiung)
                        domainLog.Add(new LogData("努力程度", objJHDomain.Effort + "", objSCDomain.Effort + ""));
                    domainLog.Add(new LogData("文字評量", objJHDomain.Text + "", objSCDomain.Text));
                    SCScore.Domain.Log.Add(domainLog);

                    objJHDomain.Score = score;
                    objJHDomain.Credit = objSCDomain.Weight;
                    objJHDomain.Period = objSCDomain.Period;
                    objJHDomain.Effort = objSCDomain.Effort;
                    objJHDomain.Text = objSCDomain.Text;

                    JHScore.Domains.Add(strDomain, objJHDomain);
                }

                //記錄 Log
                SCScore.LearningLog.Formater = domainLogFormater;
                SCScore.LearningLog.OriginValue = JHScore.CourseLearnScore + "";
                SCScore.LearningLog.NewValue = SCScore.LearnDomainScore + "";
                SCScore.CourseLog.Formater = domainLogFormater;
                SCScore.CourseLog.OriginValue = JHScore.CourseLearnScore + "";
                SCScore.CourseLog.NewValue = SCScore.CourseLearnScore + "";

                JHScore.LearnDomainScore = SCScore.LearnDomainScore;
                JHScore.CourseLearnScore = SCScore.CourseLearnScore;

                //排序領域名稱。
                Dictionary<string, DomainScore> orderDomain = new Dictionary<string, DomainScore>(JHScore.Domains);
                JHScore.Domains.Clear();
                foreach (string domainName in Util.SortSubjectDomain(orderDomain.Keys))
                    JHScore.Domains.Add(domainName, orderDomain[domainName]);
                #endregion
            }

            #region 新增科目成績
            FunctionSpliter<JHSemesterScoreRecord, JHSemesterScoreRecord> addSpliter =
                new FunctionSpliter<JHSemesterScoreRecord, JHSemesterScoreRecord>(500, 5);
            addSpliter.Function = delegate(List<JHSemesterScoreRecord> part)
            {
                // 加入檢查當科目與領域成績筆數0不新增
                List<JHSemesterScoreRecord> insertPart= new List<JHSemesterScoreRecord> ();

                foreach (JHSemesterScoreRecord rec in part)
                {
                    // 沒有任何領域或科目成績
                    if (rec.Domains.Count == 0 && rec.Subjects.Count == 0)
                        continue;

                    insertPart.Add(rec);
                }

                if(insertPart.Count >0)
                    JHSemesterScore.Insert(insertPart);

                return new List<JHSemesterScoreRecord>();
            };
            addSpliter.ProgressChange = delegate(int progress)
            {
                Reporter.Feedback("新增科目成績...", Util.CalculatePercentage(addSemsScore.Count, progress));
            };
            addSpliter.Execute(addSemsScore);
            #endregion

            #region 更新科目成績
            FunctionSpliter<JHSemesterScoreRecord, JHSemesterScoreRecord> updateSpliter =
                new FunctionSpliter<JHSemesterScoreRecord, JHSemesterScoreRecord>(500, 5);
            updateSpliter.Function = delegate(List<JHSemesterScoreRecord> part)
            {
                JHSemesterScore.Update(part);
                return new List<JHSemesterScoreRecord>();
            };
            updateSpliter.ProgressChange = delegate(int progress)
            {
                Reporter.Feedback("更新科目成績...", Util.CalculatePercentage(updateSemsScore.Count, progress));
            };
            updateSpliter.Execute(updateSemsScore);
            #endregion
        }

        private static SubjectScore GetJHSubjectScore(string strSubject, SemesterSubjectScore scSubject)
        {
            SubjectScore subject = scSubject.RawScore;

            if (subject == null)
            {
                subject = new SubjectScore();
                subject.Subject = strSubject;
            }

            return subject;
        }

        private static DomainScore GetJHDomainScore(string strDomain, SemesterDomainScore scDomain)
        {
            DomainScore domain = scDomain.RawScore;

            if (domain == null)
            {
                domain = new DomainScore();
                domain.Domain = strDomain;
            }

            return domain;
        }

        private static JHSemesterScoreRecord GetJHSemesterScore(string studentID, SCSemsScore scsemsscore)
        {
            JHSemesterScoreRecord semsscore = scsemsscore.RawScore;

            if (semsscore == null)
            {
                semsscore = new JHSemesterScoreRecord();
                semsscore.RefStudentID = studentID;
                semsscore.SchoolYear = scsemsscore.SchoolYear;
                semsscore.Semester = scsemsscore.Semester;
            }

            return semsscore;
        }
    }
}
