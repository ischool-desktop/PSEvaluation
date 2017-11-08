using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HsinChu.ClassExamScoreAvgComparison
{
    class Global
    {
        public static string ReportName = "班級評量成績平均比較表";

        /// <summary>
        /// 使用者選擇學年度
        /// </summary>
        public static string UserSelectSchoolYear = "";

        /// <summary>
        /// 使用者選擇學期
        /// </summary>
        public static string UserSelectSemester = "";

        /// <summary>
        /// 取得評量比例設定
        /// </summary>
        public static Dictionary<string, decimal> GetScorePercentageHS()
        {
            Dictionary<string, decimal> returnData = new Dictionary<string, decimal>();
            FISCA.Data.QueryHelper qh1 = new FISCA.Data.QueryHelper();
            string query1 = @"select id,CAST(regexp_replace( xpath_string(exam_template.extension,'/Extension/ScorePercentage'), '^$', '0') as integer) as ScorePercentage  from exam_template";
            System.Data.DataTable dt1 = qh1.Select(query1);

            foreach (System.Data.DataRow dr in dt1.Rows)
            {
                string id = dr["id"].ToString();
                decimal sp = 50;
                if (decimal.TryParse(dr["ScorePercentage"].ToString(), out sp))
                {
                    returnData.Add(id, sp);
                }
                else
                    returnData.Add(id, 50);


            }
            return returnData;
        }

        /// <summary>
        /// 評量比例暫存
        /// </summary>
        public static Dictionary<string, decimal> ScorePercentageHSDict = new Dictionary<string, decimal>();
    }
}
