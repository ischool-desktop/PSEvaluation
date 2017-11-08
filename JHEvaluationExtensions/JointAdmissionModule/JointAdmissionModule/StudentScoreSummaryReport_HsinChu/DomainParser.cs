using System;
using JHEvaluation.ScoreCalculation;
using JHEvaluation.ScoreCalculation.ScoreStruct;

namespace JointAdmissionModule.StudentScoreSummaryReport_HsinChu
{
    /// <summary>
    /// 五專用領域平均排名,四捨五入到小數下第2位
    /// 回傳 Null 不列入排名
    /// </summary>
    internal class DomainParser : Campus.Rating.IScoreParser<ReportStudent>
    {
        private SemesterData Semester { get; set; }
        private string _DomainName;

        public DomainParser(string DomainName,int GradeYear)
        {
            _DomainName = DomainName;
            Semester = new SemesterData(GradeYear, 0, 0);
        }

        #region IScoreParser<ReportStudent> 成員

        public decimal? GetScore(ReportStudent student)
        {
           // SemesterScore score = null;
            // 學期加總
            decimal SumScore = 0;
            int GrSc2 = 0, GrSc3 = 0;

            foreach (SemesterData each in student.SHistory.GetGradeYearSemester())
            {
                if ((each.GradeYear == 1 && each.Semester == 1) || (each.GradeYear == 1 && each.Semester == 2) || (each.GradeYear == 3 && each.Semester == 2))
                    continue;

                if ((each.GradeYear == 7 && each.Semester == 1) || (each.GradeYear == 7 && each.Semester == 2) || (each.GradeYear == 9 && each.Semester == 2))
                    continue;

                if (each.GradeYear == 3 || each.GradeYear==9 )
                    GrSc3 = each.SchoolYear;

                if (each.GradeYear== 2 || each.GradeYear ==8)
                    GrSc2 = each.SchoolYear;
            }

            foreach (SemesterScore each in student.SemestersScore )
            {
                if (each.Domain.Contains(_DomainName))
                {
                    if (each.SchoolYear == GrSc2 && each.Domain[_DomainName].Value.HasValue)
                        SumScore += each.Domain[_DomainName].Value.Value;

                    if (each.SchoolYear == GrSc3 && each.Semester == 1 && each.Domain[_DomainName].Value.HasValue)
                        SumScore += each.Domain[_DomainName].Value.Value;                    
                }
                else
                { 
                    // 當新竹需處理
                    if (Program.Mode == ModuleMode.HsinChu)
                    {
                        decimal Score = 0;

                        if (_DomainName.Equals("國語文"))
                            Score += each.GetDomainScoreFromSubject("國").Item1;
                        else if (_DomainName.Equals("英語"))
                            Score += each.GetDomainScoreFromSubject("英").Item1;

                        if (each.SchoolYear == GrSc2 && Score > 0)
                            SumScore += Score;

                        if (each.SchoolYear == GrSc3 && each.Semester == 1 && Score>0)
                            SumScore += Score;

                        //string scKey = string.Empty;

                        //foreach (string str in each.Subject)
                        //{
                        //    if (_DomainName == "國語文")
                        //        if (str.IndexOf("國") > -1)
                        //            if (each.Subject[str].Domain.Trim() == "語文")
                        //            {
                        //                scKey = str;
                        //                break;
                        //            }
                        //    if (_DomainName == "英語")
                        //        if (str.IndexOf("英") > -1)
                        //        {
                        //            if (each.Subject[str].Domain.Trim() == "語文")
                        //            {
                        //                scKey = str;
                        //                break;
                        //            }
                        //        }
                        //}

                        //if (!string.IsNullOrEmpty(scKey))
                        //{
                        //    if (each.SchoolYear == GrSc2 && each.Subject[scKey].Value.HasValue)
                        //        SumScore += each.Subject[scKey].Value.Value;

                        //    if (each.SchoolYear == GrSc3 && each.Semester == 1 && each.Subject[scKey].Value.HasValue)
                        //        SumScore += each.Subject[scKey].Value.Value;
                        //}                    
                    }
                }
            }

            if (SumScore <=0)
                return 0;
            else
            {
                // 回傳小數下第2位四捨五入
                return Math.Round(SumScore / 3, 2, MidpointRounding.AwayFromZero);
            }
        }

        public string Name
        {
            get { return _DomainName+GetSemesterString(); }
        }

        public static string GetSemesterString()
        {
            //return string.Format("領域({0}:{1})", semester.GradeYear, semester.Semester);
            return "領域3學期平均";
        }

        public int Grade { get { return Semester.GradeYear; } }

        #endregion
    }
}