using System;
using System.Collections.Generic;
using System.Text;
using SCSemsScore = JHEvaluation.ScoreCalculation.ScoreStruct.SemesterScore;
using SCSemsDomainScore = JHEvaluation.ScoreCalculation.ScoreStruct.SemesterDomainScore;
using JHEvaluation.ScoreCalculation.ScoreStruct;

namespace JHEvaluation.ScoreCalculation.BigFunction
{
    /// <summary>
    /// 計算畢業成績。
    /// https://sites.google.com/a/ischool.com.tw/jh-requirement-library/Home/cheng-ji-xu-qiu-wen-jian-1/cheng-ji-bao-biao-zheng-li/xue-sheng-bi-ye-cheng-ji-ji-suan
    /// </summary>
    internal class GradScoreCalculator
    {
        private List<StudentScore> Students { get; set; }

        private const string CourseLearning = "課程學習~~~";
        private const string LearnDomain = "學習領域~~~";

        public GradScoreCalculator(List<StudentScore> students)
        {
            Students = students;
        }

        public void Calculate()
        {
            foreach (StudentScore student in Students)
            {
                //成績計算規則。
                ScoreCalculator calcRule = student.CalculationRule;

                //沒有成績計算規則的不計算。
                if (calcRule == null) continue;

                //領域總分。
                Dictionary<string, decimal> domainTotal = new Dictionary<string, decimal>();
                //學期數計數。
                Dictionary<string, decimal> semsCount = new Dictionary<string, decimal>();
                //計算六學期的總分。
                foreach (SemesterData sems in student.SHistory.GetGradeYearSemester())
                {
                    SemesterData sd = new SemesterData(0, sems.SchoolYear, sems.Semester);

                    //學生不包含該學期的成績，直接跳過。
                    if (!student.SemestersScore.Contains(sd)) continue;

                    //將該學期的成績資料取出。
                    SCSemsScore scscore = student.SemestersScore[sd];

                    //加總各領域成績。
                    foreach (string strDomain in scscore.Domain)
                    {
                        SCSemsDomainScore objDomain = scscore.Domain[strDomain];

                        if (!objDomain.Value.HasValue) continue; //沒有成績不計算。

                        SumDomainScore(domainTotal, semsCount, strDomain, objDomain.Value.Value);
                    }
                    //學習領域。
                    if (scscore.LearnDomainScore.HasValue)
                        SumDomainScore(domainTotal, semsCount, LearnDomain, scscore.LearnDomainScore.Value);
                    //課程學習。
                    if (scscore.CourseLearnScore.HasValue)
                        SumDomainScore(domainTotal, semsCount, CourseLearning, scscore.CourseLearnScore.Value);
                }

                //計算總分的算數平均。
                //2017/5/9 穎驊修正 ，因應 高雄 [08-05][03] 畢業資格判斷成績及格標準調整 項目，
                // 領域 分數超過60分 ，以 四捨五入取到小數第二位 ， 低於60分 採用 無條件進位至整數 (EX : 59.01 =60)
                // (只有高雄版有如此機制，新竹版照舊不管分數高低都是四捨五入)
                foreach (string strDomain in domainTotal.Keys)
                {
                    //學期數是「0」的不計算。
                    if (semsCount[strDomain] <= 0) continue;

                    decimal score = 0;

                    if (JHEvaluation.ScoreCalculation.Program.Mode == JHEvaluation.ScoreCalculation.ModuleMode.HsinChu)
                    {
                        score = calcRule.ParseGraduateScore(domainTotal[strDomain] / semsCount[strDomain]);
                    }

                    if (JHEvaluation.ScoreCalculation.Program.Mode == JHEvaluation.ScoreCalculation.ModuleMode.KaoHsiung)
                    {
                        if ((domainTotal[strDomain] / semsCount[strDomain]) >= 60)
                        {
                            score = calcRule.ParseGraduateScore(domainTotal[strDomain] / semsCount[strDomain]);
                        }
                        else
                        {
                            score = Math.Ceiling(domainTotal[strDomain] / semsCount[strDomain]);
                        }                    
                    }

                    
                    

                    if (strDomain == LearnDomain)
                        student.GraduateScore.LearnDomainScore = score;
                    else if (strDomain == CourseLearning)
                        student.GraduateScore.CourseLearnScore = score;
                    else
                    {
                        if (!student.GraduateScore.Contains(strDomain))
                            student.GraduateScore.Add(strDomain, new GraduateScore());

                        student.GraduateScore[strDomain].Value = score;
                    }
                }
            }
        }

        /// <summary>
        /// 加總領域成績。
        /// </summary>
        private static void SumDomainScore(Dictionary<string, decimal> domainTotal,
            Dictionary<string, decimal> semsCount,
            string domainName,
            decimal score)
        {
            if (!domainTotal.ContainsKey(domainName))
            {
                domainTotal.Add(domainName, 0);
                semsCount.Add(domainName, 0);
            }
            domainTotal[domainName] += score;
            semsCount[domainName]++;
        }
    }
}
