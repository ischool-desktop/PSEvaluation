using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Data;
using System.Data;

namespace KaoHsiung.ClassExamScoreReportV2
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
                int cot = 0,i;
                if(int.TryParse(dr["count"].ToString(),out i))
                    cot=i;

                retVal.Add(dr["id"].ToString(), cot);
            }

            return retVal;
        }

        /// <summary>
        /// 取得系統內學生類別,群組用[]表示,沒有群組直接名稱
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetStudentTagRefDict()
        {
            // 學生類別,StudentID
            Dictionary<string, List<string>> retVal = new Dictionary<string, List<string>>();
            QueryHelper qh = new QueryHelper();
            string query = "select tag.prefix,tag.name,ref_student_id from tag left join tag_student on tag.id = tag_student.ref_tag_id order by tag.prefix,tag.name";
            DataTable dt = new DataTable();
            dt = qh.Select(query);

            foreach (DataRow dr in dt.Rows)
            {
                string strP = "", key = "", StudID = "";

                if (dr["prefix"] != null)
                    strP = dr["prefix"].ToString();

                if (string.IsNullOrEmpty(strP))
                    key = dr["name"].ToString();
                else
                    key = "[" + strP + "]";

                if (dr["ref_student_id"] != null)
                    StudID = dr["ref_student_id"].ToString();

                if (!retVal.ContainsKey(key))
                    retVal.Add(key, new List<string>());

                if (!string.IsNullOrEmpty(StudID))
                    retVal[key].Add(StudID);

            }
            return retVal;
        }

        /// <summary>
        /// 取得學生清單，[]表示Prefix
        /// </summary>
        /// <returns></returns>
        public static List<string> GetStudentTagList()
        {
            List<string> retVal = new List<string>();
            QueryHelper qh = new QueryHelper();
            string query = "select  distinct tag.prefix,tag.name from tag where category='Student' order by tag.prefix,tag.name;";
            DataTable dt = qh.Select(query);
            foreach (DataRow dr in dt.Rows)
            {
                string prefix = "",name="";
                if (dr["prefix"] != null)
                    prefix = dr["prefix"].ToString();

                name=dr["name"].ToString();

                if (string.IsNullOrEmpty(prefix))
                {
                    if (!retVal.Contains(name))
                        retVal.Add(name);
                }
                else
                {
                    prefix = "[" + prefix + "]";
                    if (!retVal.Contains(prefix))
                        retVal.Add(prefix);
                }
            }

            retVal.Sort();

            return retVal;
        }

        /// <summary>
        /// 不排名學生ID
        /// </summary>
        public static List<string> notRankStudentIDList = new List<string>();


        public static List<string> GetNotRankStudentIDList(string StudTag)
        {
            List<string> retVal = new List<string>();

            Dictionary<string, List<string>> mapDict = new Dictionary<string, List<string>>();
            QueryHelper qh = new QueryHelper();
            string query = @"select tag.prefix,tag.name,ref_student_id as sid from tag inner join tag_student on tag.id=tag_student.ref_tag_id  where category='Student' order by tag.prefix,tag.name";
            DataTable dt = qh.Select(query);
            foreach (DataRow dr in dt.Rows)
            {
                string prefix = "", name = "";
                if (dr["prefix"] != null)
                    prefix = dr["prefix"].ToString();

                name = dr["name"].ToString();
                string sid = dr["sid"].ToString();

                if (string.IsNullOrEmpty(prefix))
                {
                    if (!mapDict.ContainsKey(name))
                        mapDict.Add(name, new List<string>());

                    mapDict[name].Add(sid);
                }
                else
                {
                    prefix = "[" + prefix + "]";
                    if (!mapDict.ContainsKey(prefix))
                        mapDict.Add(prefix, new List<string>());

                    mapDict[prefix].Add(sid);
                }
            }

            if (mapDict.ContainsKey(StudTag))
                retVal = mapDict[StudTag];

            return retVal;
        }
    }
}
