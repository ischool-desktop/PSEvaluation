using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Data;
using System.Data;
using K12.Data;
using JHSchool.Data;
using JHSchool.Evaluation.Calculation;

namespace JHEvaluation.ClassSemesterScoreReport
{
    public class Utility
    {
        /// <summary>
        /// 取得班級人數(一般、輟學)
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, int> GetClassStudentCount18()
        {
            Dictionary<string, int> retVal = new Dictionary<string, int>();
            QueryHelper qh = new QueryHelper();
            string strSQL = "select class.id,count(student.id) as count from student inner join class on student.ref_class_id=class.id where student.status in(1,8) group by class.id";
            DataTable dt = qh.Select(strSQL);

            foreach (DataRow dr in dt.Rows)
            {
                int cot = 0, i;
                if (int.TryParse(dr["count"].ToString(), out i))
                    cot = i;

                retVal.Add(dr["id"].ToString(), cot);
            }

            return retVal;
        }

        /// <summary>
        /// 儲存成績計算規則,ruleid,ScoreCalculator
        /// </summary>
        public static Dictionary<string, ScoreCalculator> tmpScoreCalculatorDict = new Dictionary<string, ScoreCalculator>();

        /// <summary>
        /// 取得成績計算規則轉成計算器
        /// </summary>
        public static void LoadtmpScoreCalculatorDict()
        {
            tmpScoreCalculatorDict.Clear();

            foreach (JHScoreCalcRuleRecord rec in JHScoreCalcRule.SelectAll())
            {
                ScoreCalculator sc = new ScoreCalculator(rec);
                if (sc != null)
                {
                    tmpScoreCalculatorDict.Add(rec.ID, sc);
                }
            }
        }

        public static Dictionary<string, string> tmpClassRuleIDDict = new Dictionary<string, string>();

        public static void LoadtmpClassRuleIDDict()
        {
            tmpClassRuleIDDict.Clear();
            foreach (JHClassRecord rec in JHClass.SelectAll())
                if (rec.RefScoreCalcRuleID != null)
                    tmpClassRuleIDDict.Add(rec.ID, rec.RefScoreCalcRuleID);
        }
 
    }
}
