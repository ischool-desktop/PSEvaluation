using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;
using JHEvaluation.ScoreCalculation.ScoreStruct;
using JHSchool.Evaluation.Calculation;

namespace JHEvaluation.ScoreCalculation
{
    /// <summary>
    /// 代表通用成績系統的學生。
    /// </summary>
    public class StudentScore : JHEvaluation.ScoreCalculation.IStudent
    {
        /// <summary>
        /// 用於對照班級資訊。
        /// </summary>
        private static Dictionary<string, JHClassRecord> Classes { get; set; }

        private static Dictionary<string, ScoreCalculator> Rules { get; set; }

        /// <summary>
        /// 設定班級對照表。
        /// </summary>
        public static void SetClassMapping()
        {
            JHClass.RemoveAll();
            IEnumerable<JHClassRecord> classes = JHClass.SelectAll();
            Classes = new Dictionary<string, JHClassRecord>();
            foreach (JHClassRecord each in classes)
                Classes.Add(each.ID, each);
        }

        internal static void SetRuleMapping(IEnumerable<JHScoreCalcRuleRecord> rules)
        {
            Rules = new Dictionary<string, ScoreCalculator>();
            foreach (JHScoreCalcRuleRecord each in rules)
                Rules.Add(each.ID, new ScoreCalculator(each));
        }

        #region 基本資料屬性方法。
        public StudentScore(JHStudentRecord student)
        {
            AttendScore = new AttendScoreCollection();
            SemestersScore = new SemesterScoreCollection();
            GraduateScore = new GraduateScoreCollection();
            SHistory = new SemesterDataCollection();

            Id = student.ID;
            Name = student.Name;
            RefClassID = student.RefClassID;
            SeatNo = student.SeatNo + "";
            StudentNumber = student.StudentNumber;
            RefCalculationRuleID = student.OverrideScoreCalcRuleID + "";
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public string RefClassID { get; private set; }

        /// <summary>
        /// 班級。
        /// </summary>
        public JHClassRecord Class
        {
            get
            {
                if (Classes.ContainsKey(RefClassID))
                    return Classes[RefClassID];
                else
                    return null;
            }
        }

        public string ClassName
        {
            get
            {
                if (Classes.ContainsKey(RefClassID))
                    return Classes[RefClassID].Name;
                else
                    return string.Empty;
            }
        }

        public string ClassOrderString
        {
            get
            {
                if (Classes.ContainsKey(RefClassID))
                {
                    JHClassRecord cr = Classes[RefClassID];
                    return (cr.GradeYear + "").PadLeft(3, 'Z') + cr.DisplayOrder.PadLeft(3, 'Z') + cr.Name;
                }
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// 排序的依據。
        /// </summary>
        public string OrderString
        {
            get { return ClassOrderString + SeatNo.PadLeft(3, '0'); }
        }

        public string GradeYear
        {
            get
            {
                if (Classes.ContainsKey(RefClassID))
                    return Classes[RefClassID].GradeYear + "";
                else
                    return string.Empty;
            }
        }

        public string SeatNo { get; private set; }

        public string StudentNumber { get; private set; }
        #endregion

        #region 成績計算規則
        /// <summary>
        /// 學生身上的成績計算規則編號。
        /// </summary>
        public string RefCalculationRuleID { get; set; }

        /// <summary>
        /// 成績計算規則。
        /// </summary>
        public ScoreCalculator CalculationRule
        {
            get
            {
                if (Rules.ContainsKey(RefCalculationRuleID))
                    return Rules[RefCalculationRuleID];

                if (Classes.ContainsKey(RefClassID))
                {
                    JHClassRecord crecord = Classes[RefClassID];
                    if (Rules.ContainsKey(crecord.RefScoreCalcRuleID))
                        return Rules[crecord.RefScoreCalcRuleID];
                }

                return null;
            }
        }
        #endregion

        /// <summary>
        /// 學期歷程資訊。
        /// </summary>
        public SemesterDataCollection SHistory { get; private set; }

        /// <summary>
        /// 取得學生修課相關成績，僅包含評量設定中所要求的成績，不包含所有的成績。
        /// </summary>
        public AttendScoreCollection AttendScore { get; set; }

        /// <summary>
        /// 取得學生學期成績(多學期)。
        /// </summary>
        public SemesterScoreCollection SemestersScore { get; set; }

        /// <summary>
        /// 取得學生畢業相關成績。
        /// </summary>
        public GraduateScoreCollection GraduateScore { get; set; }
    }
}
