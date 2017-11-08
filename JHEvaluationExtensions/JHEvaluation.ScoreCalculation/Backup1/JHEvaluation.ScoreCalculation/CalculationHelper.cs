using System;
using System.Collections.Generic;
using System.Text;
using JHEvaluation.ScoreCalculation.BigFunction;
using JHSchool.Data;
using JHEvaluation.ScoreCalculation.ScoreStruct;

namespace JHEvaluation.ScoreCalculation
{
    public static class CalculationHelper
    {
        /// <summary>
        /// 讀取修課成績相關資料，包含課程成績、評量成績。
        /// </summary>
        /// <param name="students">學生清單。</param>
        /// <param name="schoolYear">學年度。</param>
        /// <param name="semester">學期。</param>
        /// <param name="filterSubject">讀取資料的科目。</param>
        /// <param name="reporter">進度回報。</param>
        public static void ReadAttendScore(this List<StudentScore> students,
            int schoolYear,
            int semester,
            IEnumerable<string> filterSubject,
            IStatusReporter reporter)
        {
            new AttendScoreReader(students, schoolYear, semester, filterSubject, GetReporter(reporter)).Read();
            GC.Collect();
        }

        /// <summary>
        /// 計算課程成績。
        /// </summary>
        /// <returns>完全沒有修課的學生。</returns>
        public static List<StudentScore> CalcuateAttendScore(this List<StudentScore> students, IStatusReporter reporter)
        {
            GetReporter(reporter).Feedback("計算成績...", 0);
            return new AttendScoreCalculator(students, GetReporter(reporter)).Calculate();
        }

        /// <summary>
        /// 儲存課程成績。
        /// </summary>
        public static void SaveAttendScore(this List<StudentScore> students,
            IEnumerable<string> filterSubject,
            IStatusReporter reporter)
        {
            new AttendScoreSaver(students, filterSubject, reporter).Save();
        }

        /// <summary>
        /// 讀取單一學期成績，包含科目成績、領域成績。
        /// </summary>
        public static void ReadSemesterScore(this List<StudentScore> students,
            int schoolYear,
            int semester,
            IStatusReporter reporter)
        {
            new SemesterScoreReader(students, schoolYear, semester, GetReporter(reporter)).Read();
        }

        /// <summary>
        /// 讀取所有學期成績，包含科目成績、領域成績。
        /// </summary>
        public static void ReadSemesterScore(this List<StudentScore> students, IStatusReporter reporter)
        {
            new SemestersScoreReader(students, GetReporter(reporter)).Read();
        }

        /// <summary>
        /// 讀取成績計算規則。
        /// </summary>
        /// <param name="students"></param>
        /// <returns>沒有設定計算規則的學生。</returns>
        public static List<StudentScore> ReadCalculationRule(this List<StudentScore> students, IStatusReporter reporter)
        {
            return new CalculationRuleReader(students, GetReporter(reporter)).Read();
        }

        /// <summary>
        /// 計算科目成績。
        /// </summary>
        public static void CalcuateSubjectSemesterScore(this List<StudentScore> students, IEnumerable<string> filterSubject)
        {
            new SemesterScoreCalculator(students, filterSubject).CalculateSubjectScore();
        }

        /// <summary>
        /// 計算領域成績。
        /// </summary>
        public static void CalcuateDomainSemesterScore(this List<StudentScore> students,
            IEnumerable<string> filterDomain)
        {
            new SemesterScoreCalculator(students, filterDomain).CalculateDomainScore(new ScoreCalculator(null));
        }

        /// <summary>
        /// 計算領域成績。
        /// </summary>
        public static void CalcuateDomainSemesterScore(this List<StudentScore> students,
            IEnumerable<string> filterDomain,
            ScoreCalculator defaultRule)
        {
            new SemesterScoreCalculator(students, filterDomain).CalculateDomainScore(defaultRule);
        }

        /// <summary>
        /// 計算學習領域成績
        /// </summary>
        public static void CalculateLearningDomainSemesterScore(this List<StudentScore> students,
            IEnumerable<string> filterDomain)
        {
            new SemesterScoreCalculator(students, filterDomain).CalculateLearningDomainScore(new ScoreCalculator(null));
        }

        /// <summary>
        /// 計算學習領域成績
        /// </summary>
        public static void CalculateLearningDomainSemesterScore(this List<StudentScore> students,
            IEnumerable<string> filterDomain,ScoreCalculator defaultRule)
        {
            new SemesterScoreCalculator(students, filterDomain).CalculateLearningDomainScore(defaultRule);
        }

        /// <summary>
        /// 計算學習領域成績
        /// </summary>
        public static void SumDomainTextScore(this List<StudentScore> students,
            IEnumerable<string> filterDomain)
        {
            new SemesterScoreCalculator(students, filterDomain).SumDomainTextScore();
        }

        /// <summary>
        /// 儲存單學期成績，包含科目、領域(僅會儲存 SemesterData.Empty 學期)。
        /// </summary>
        /// <param name="students"></param>
        public static void SaveSemesterScore(this List<StudentScore> students, IStatusReporter reporter)
        {
            new SemesterScoreSaver(students, GetReporter(reporter)).Save();
        }

        /// <summary>
        /// 讀取學生的學期歷程資料。
        /// </summary>
        public static void ReadSemesterHistory(this List<StudentScore> students, IStatusReporter reporter)
        {
            new SemesterHistoryReader(students, GetReporter(reporter)).Read();
        }

        /// <summary>
        /// 檢查學生學期歷程是否有完整六個學期。
        /// </summary>
        /// <param name="students"></param>
        /// <returns>回傳學期歷程不完整的學生清單。</returns>
        public static List<StudentScore> VaidSixSemesterHistory(this List<StudentScore> students)
        {
            return new SixSemesterHistoryValidator(students).Valid();
        }

        /// <summary>
        /// 檢查六學期成績是否完整。
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        public static List<StudentScore> ValidSixSemesterDomainScore(this List<StudentScore> students)
        {
            return new SixSemesterDomainScoreValidator(students).Valid();
        }

        /// <summary>
        /// 讀取畢業成績。
        /// </summary>
        public static void ReadGraduateScore(this List<StudentScore> students, IStatusReporter reporter)
        {
            new GradScoreReader(students, GetReporter(reporter)).Read();
        }

        /// <summary>
        /// 計算畢業成績。
        /// </summary>
        public static void CalculateGraduateScore(this List<StudentScore> students)
        {
            new GradScoreCalculator(students).Calculate();
        }

        /// <summary>
        /// 儲存畢業成績。
        /// </summary>
        public static void SaveGraduateScore(this List<StudentScore> students, IStatusReporter reporter)
        {
            new GradScoreSaver(students, GetReporter(reporter)).Save();
        }

        private static IStatusReporter GetReporter(IStatusReporter reporter)
        {
            IStatusReporter rep = reporter;
            if (reporter == null) rep = new EmptyStatusReport();
            return rep;
        }
    }
}
