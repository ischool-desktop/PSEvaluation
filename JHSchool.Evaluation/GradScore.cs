using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace JHSchool.Evaluation
{
    public class GradScore : CacheManager<GradScoreRecord>
    {
        private static GradScore _Instance = null;
        public static GradScore Instance { get { if (_Instance == null)_Instance = new GradScore(); return _Instance; } }
        private GradScore()
        {
        }

        protected override Dictionary<string, GradScoreRecord> GetAllData()
        {
            return new Dictionary<string, GradScoreRecord>();
        }

        protected override Dictionary<string, GradScoreRecord> GetData(IEnumerable<string> primaryKeys)
        {
            Dictionary<string, GradScoreRecord> result = new Dictionary<string, GradScoreRecord>();
            foreach (var item in Feature.QueryStudent.GetGradScores(primaryKeys))
            {
                if (!result.ContainsKey(item.RefStudentID))
                    result.Add(item.RefStudentID, item);
            }
            return result;
        }
    }
}
