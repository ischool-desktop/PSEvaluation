using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Framework;
using FISCA.DSAUtil;

namespace JHSchool.Evaluation.Feature
{
    /// <summary>
    /// 取得日常生活表現成績
    /// </summary>
    [FISCA.Authentication.AutoRetryOnWebException()]
    internal static class QueryMoralScore
    {
        private const string GET_MORAL_SCORE = "SmartSchool.Score.GetSemesterMoralScore";

        /// <summary>
        /// 取得指定的學生的紀錄
        /// </summary>
        /// <param name="primaryKeys">學生ID的集合</param>
        /// <returns></returns>
        public static List<MoralScoreRecord> GetMoralScoreRecords(IEnumerable<string> primaryKeys)
        {
            bool haskey = false;

            StringBuilder req = new StringBuilder("<Request><Field><ID/><RefStudentID/><SchoolYear/><Semester/><TextScore/></Field><Condition><StudentIDList>");
            
            foreach (string key in primaryKeys)
            {
                req.Append("<ID>" + key + "</ID>");
                haskey = true;
            }

            req.Append("</StudentIDList></Condition></Request>");

            List<MoralScoreRecord> result = new List<MoralScoreRecord>();
            
            if (haskey)
            {
                foreach (XmlElement item in FISCA.Authentication.DSAServices.CallService(GET_MORAL_SCORE, new DSRequest(req.ToString())).GetContent().GetElements("SemesterMoralScore"))
                {
                    result.Add(new MoralScoreRecord(item));
                }
            }

            return result;
        }
    }
}
