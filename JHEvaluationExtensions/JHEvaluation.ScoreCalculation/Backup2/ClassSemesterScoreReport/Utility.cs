using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Data;
using System.Data;


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
    }
}
