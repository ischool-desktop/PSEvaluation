using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace JHSchool.Evaluation
{
    internal class SemesterScore : CacheManager<List<SemesterScoreRecord>>
    {
        private static SemesterScore _Instance = null;
        public static SemesterScore Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new SemesterScore();
                return _Instance;
            }
        }

        private SemesterScore() { }

        protected override Dictionary<string, List<SemesterScoreRecord>> GetAllData()
        {
            return new Dictionary<string, List<SemesterScoreRecord>>();
        }

        protected override Dictionary<string, List<SemesterScoreRecord>> GetData(IEnumerable<string> primaryKeys)
        {
            Dictionary<string, List<SemesterScoreRecord>> group = new Dictionary<string, List<SemesterScoreRecord>>();
            foreach (var item in Feature.QueryStudent.GetSemesterScores(primaryKeys))
            {
                if (!group.ContainsKey(item.RefStudentID))
                    group.Add(item.RefStudentID, new List<SemesterScoreRecord>());
                group[item.RefStudentID].Add(item);
            }

            Dictionary<string, List<SemesterScoreRecord>> result = new Dictionary<string, List<SemesterScoreRecord>>();
            foreach (string primaryKey in primaryKeys)
            {
                result.Add(primaryKey, (group.ContainsKey(primaryKey) ? group[primaryKey] : new List<SemesterScoreRecord>()));
            }
            return result;
        }
    }

    public static class SemesterScore_ExtendMethods
    {
        /// <summary>
        /// 取得學生學期成績資料。
        /// </summary>
        public static List<SemesterScoreRecord> GetSemesterScores(this StudentRecord studentRec)
        {
            return SemesterScore.Instance[studentRec.ID];
        }

        /// <summary>
        /// 批次同步學生學期成績資料。
        /// </summary>
        public static void SyncSemesterScoreCache(this IEnumerable<StudentRecord> studentRecs)
        {
            List<string> primaryKeys = new List<string>();
            foreach (var item in studentRecs)
            {
                primaryKeys.Add(item.ID);
            }
            SemesterScore.Instance.SyncDataBackground(primaryKeys);
        }
    }
}
