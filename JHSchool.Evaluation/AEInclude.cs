using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace JHSchool.Evaluation
{
    public class AEInclude : CacheManager<AEIncludeRecord>
    {
        private static AEInclude _Instance = null;
        /// <summary>
        /// 取得唯一實體
        /// </summary>
        public static AEInclude Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new AEInclude();
                return _Instance;
            }
        }

        private AEInclude()
        {
        }

        protected override Dictionary<string, AEIncludeRecord> GetAllData()
        {
            Dictionary<string, AEIncludeRecord> results = new Dictionary<string, AEIncludeRecord>();

            foreach (var item in JHSchool.Evaluation.Feature.QueryAEInclude.GetAllAEIncludeRecords())
            {
                results.Add(item.ID, item);
            }
            return results;
        }

        protected override Dictionary<string, AEIncludeRecord> GetData(IEnumerable<string> primaryKeys)
        {
            Dictionary<string, AEIncludeRecord> results = new Dictionary<string, AEIncludeRecord>();
            foreach (var item in JHSchool.Evaluation.Feature.QueryAEInclude.GetAEIncludeRecords(primaryKeys))
            {
                results.Add(item.ID, item);
            }
            return results;
        }
    }

    public static class AEInclude_ExtendMethods
    {
        public static List<AEIncludeRecord> GetAEIncludes(this AssessmentSetupRecord record)
        {
            List<AEIncludeRecord> result = new List<AEIncludeRecord>();
            foreach (var item in AEInclude.Instance.Items)
            {
                if (item.RefAssessmentSetupID == record.ID)
                    result.Add(item);
            }
            return result;
        }
    }
}
