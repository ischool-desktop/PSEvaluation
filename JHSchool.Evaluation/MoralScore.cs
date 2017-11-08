using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Evaluation.Feature;

namespace JHSchool.Evaluation
{
    /// <summary>
    /// Cache manager of MoralScore
    /// </summary>
    public class MoralScore : Framework.CacheManager<List<MoralScoreRecord>>
    {
        private static MoralScore _Instance = null;

        private MoralScore() { }

        public static MoralScore Instance { get { if (_Instance == null)_Instance = new MoralScore(); return _Instance; } }

        /// <summary>
        /// 取得所有的紀錄
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<string, List<MoralScoreRecord>> GetAllData()
        {
            Dictionary<string, List<MoralScoreRecord>> oneToMany = new Dictionary<string, List<MoralScoreRecord>>();

            return oneToMany;
        }

        /// <summary>
        /// 取得指定的學生的紀錄
        /// </summary>
        /// <param name="primaryKeys">學生ID的集合</param>
        /// <returns></returns>
        protected override Dictionary<string, List<MoralScoreRecord>> GetData(IEnumerable<string> primaryKeys)
        {
            Dictionary<string, List<MoralScoreRecord>> oneToMany = new Dictionary<string, List<MoralScoreRecord>>();

            foreach (MoralScoreRecord each in QueryMoralScore.GetMoralScoreRecords(primaryKeys))
            {
                if (!oneToMany.ContainsKey(each.RefStudentID))
                    oneToMany.Add(each.RefStudentID, new List<MoralScoreRecord>());

                oneToMany[each.RefStudentID].Add(each);
            }

            foreach (string each in primaryKeys)
            {
                if (!oneToMany.ContainsKey(each))
                    oneToMany.Add(each, new List<MoralScoreRecord>());
            }

            return oneToMany;
        }
    }
}
