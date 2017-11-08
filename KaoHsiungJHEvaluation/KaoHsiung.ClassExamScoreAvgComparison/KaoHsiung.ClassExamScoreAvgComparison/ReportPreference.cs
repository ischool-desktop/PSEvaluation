using System;
using System.Collections.Generic;
using System.Text;
using Campus.Report;

namespace KaoHsiung.ClassExamScoreAvgComparison
{
    /// <summary>
    /// 代表班級學期成績單所需要的組態資料。
    /// </summary>
    internal class ReportPreference
    {
        public const string ConfigName = "ClassExamScoreAvgComparisonConfiguration";
        private ReportConfiguration _config;

        public ReportPreference()
        {
            _config = new ReportConfiguration(ConfigName);
        
            NotRankTag = _config.GetString("NotRankTag", "");
        }

        /// <summary>
        /// 學生不排名類別
        /// </summary>
        public string NotRankTag { get; set; }

        public void Save()
        {           
            _config.SetString("NotRankTag", NotRankTag);
            _config.Save();
        }
    }
}
