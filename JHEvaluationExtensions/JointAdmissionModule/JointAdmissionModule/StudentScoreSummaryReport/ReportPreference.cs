using System;
using System.Collections.Generic;
using System.Text;
using Campus.Report;

namespace JointAdmissionModule.StudentScoreSummaryReport
{
    /// <summary>
    /// 代表班級學期成績單所需要的組態資料。
    /// </summary>
    internal class ReportPreference
    {
        private ReportConfiguration _config;

        private byte[] DefaultTemplateData { get; set; }

        public ReportPreference(string configName, byte[] defTemplateData)
        {
            _config = new ReportConfiguration(configName);
            DefaultTemplateData = defTemplateData;

            //SubjectOnly、DomainOnly、SubjectDomain。
            string pt = _config.GetString("PrintType", ListMethod.DomainOnly.ToString());

            ListMethod = (ListMethod)Enum.Parse(typeof(ListMethod), pt);
            PrintRank = _config.GetBoolean("PrintRank", false);
            PrintRankPercentage = _config.GetBoolean("PrintRankPercentage", false);
            AcceptAbsences = _config.GetString("AcceptAbsences", string.Empty);
            EntranceDate = _config.GetString("EntranceDate", "");
            GraduateDate = _config.GetString("GraduateDate", "");
            FilterRankScope = _config.GetBoolean("FilterRankScope", false);
            RankStart = _config.GetInteger("RankStart", 1);
            RankEnd = _config.GetInteger("RankEnd", 10);
            ConvertToPDF = _config.GetBoolean("ConvertToPDF", false);

            PrintSemesters = new List<int>();
            string[] semses = _config.GetString("PrintSemesters", "1,2,3,4,5,6").Split(',');
            foreach (string each in semses)
            {
                if (string.IsNullOrEmpty(each)) continue;

                PrintSemesters.Add(int.Parse(each));
            }
        }

        /// <summary>
        /// 列印類型，支援：SubjectOnly、DomainOnly、SubjectDomain
        /// </summary>
        public ListMethod ListMethod { get; set; }

        /// <summary>
        /// 是否列印排名。
        /// </summary>
        public bool PrintRank { get; set; }

        /// <summary>
        /// 是否列印百分比。
        /// </summary>
        public bool PrintRankPercentage { get; set; }

        /// <summary>
        /// 要列印的 Absence。
        /// </summary>
        public string AcceptAbsences { get; set; }

        /// <summary>
        /// 入學日期。
        /// </summary>
        public string EntranceDate { get; set; }

        /// <summary>
        /// 畢業年月。
        /// </summary>
        public string GraduateDate { get; set; }

        public bool FilterRankScope { get; set; }

        public int RankStart { get; set; }

        public int RankEnd { get; set; }

        public bool ConvertToPDF { get; set; }

        public List<int> PrintSemesters { get; set; }

        public ReportTemplate Template
        {
            get
            {
                if (_config.Template == null)
                {
                    return new ReportTemplate(DefaultTemplateData, TemplateType.Word);
                }
                else
                    return _config.Template;
            }
            set { _config.Template = value; }
        }

        public void Save()
        {
            _config.SetString("PrintType", ListMethod.ToString());
            _config.SetBoolean("PrintRank", PrintRank);
            _config.SetBoolean("PrintRankPercentage", PrintRankPercentage);
            _config.SetString("AcceptAbsences", AcceptAbsences);
            _config.SetString("EntranceDate", EntranceDate);
            _config.SetString("GraduateDate", GraduateDate);
            _config.SetBoolean("FilterRankScope", FilterRankScope);
            _config.SetInteger("RankStart", RankStart);
            _config.SetInteger("RankEnd", RankEnd);
            _config.SetBoolean("ConvertToPDF", ConvertToPDF);

            StringBuilder sb = new StringBuilder();
            foreach (int each in PrintSemesters)
            {
                if (sb.Length <= 0)
                    sb.Append(each);
                else
                    sb.Append("," + each);
            }
            _config.SetString("PrintSemesters", sb.ToString());

            _config.Save();
        }
    }

    internal enum ListMethod
    {
        SubjectOnly,
        DomainOnly,
        SubjectDomain
    }
}
