using System;
using System.Collections.Generic;
using System.Text;
using Campus.Report;

namespace HsinChu.ClassExamScoreReportV21
{
    /// <summary>
    /// 代表班級學期成績單所需要的組態資料。
    /// </summary>
    internal class ReportPreference
    {
        public const string ConfigName = "ClassExamScoreConfiguration";
        private ReportConfiguration _config;

        public ReportPreference()
        {
            _config = new ReportConfiguration(ConfigName);

            RankMethod = _config.GetString("RankMethod", "加權平均");

            PrintItems = new List<string>();
            foreach (string item in _config.GetString("PrintItems", "合計總分,加權平均").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                PrintItems.Add(item);

            PaperSize = _config.GetString("PaperSize", "A4");
        }

        /// <summary>
        /// 取得排名依據。
        /// </summary>
        public string RankMethod { get; set; }

        /// <summary>
        /// 列印項目。
        /// </summary>
        public List<string> PrintItems { get; set; }

        public string PaperSize { get; set; }

        public void Save()
        {
            _config.SetString("RankMethod", RankMethod);
            _config.SetString("PrintItems", string.Join(",", PrintItems.ToArray()));
            _config.SetString("PaperSize", PaperSize);
            _config.Save();
        }
    }
}
