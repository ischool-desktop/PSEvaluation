using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JointAdmissionModule.StudentScoreSummaryReport5
{
    /// <summary>
    /// 學生學期成績平均與排名
    /// </summary>
    class StudentAvgScoreRank5Rpt
    {
        /// <summary>
        /// 就讀國中
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 就讀國中代碼
        /// </summary>
        public string SchoolCode { get; set; }
        /// <summary>
        /// 班級
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 身分證統一編號
        /// </summary>
        public string IDNumber { get; set; }
        /// <summary>
        /// 學號
        /// </summary>
        public string StudentNumber { get; set; }
        /// <summary>
        /// 考生特種身分別
        /// </summary>
        public string StudSpecType { get; set; }
        /// <summary>
        /// 考生特種身分代碼
        /// </summary>
        public string StudSpecTypeCode { get; set; }

        /// <summary>
        /// 座號
        /// </summary>
        public string SeatNo { get; set;}

        /// <summary>
        /// 性別代碼 1:男,2:女
        /// </summary>
        public string GenderNo { get; set; }

        /// <summary>
        /// 西元生日
        /// </summary>
        public DateTime? BirthDay { get; set; }
        /// <summary>
        /// 加分比
        /// </summary>
        public decimal? AddWeight { get; set; }
        /// <summary>
        /// 領域成績平均
        /// </summary>
        public Dictionary<string, DomainAvgScore5Rpt> StudDomainscoreAvgDict = new Dictionary<string, DomainAvgScore5Rpt>();
        /// <summary>
        /// 學期成績平均八上
        /// </summary>
        public decimal? DomainScore81 { get; set; }
        /// <summary>
        /// 學期成績平均八下
        /// </summary>
        public decimal? DomainScore82 { get; set; }
        /// <summary>
        /// 學期成績平均九上
        /// </summary>
        public decimal? DomainScore91 { get; set; }
        /// <summary>
        /// 學期成績總平均
        /// </summary>
        public decimal? DomainScoreAvg { get; set; }
        /// <summary>
        /// 學期成績總平均排名百分比(int)
        /// </summary>
        public int? DomainScoreAvgRankPercent { get; set; }

        /// <summary>
        /// 加分後學期成績平均八上
        /// </summary>
        public decimal? DomainScore81Add { get; set; }
        /// <summary>
        /// 加分後學期成績平均八下
        /// </summary>
        public decimal? DomainScore82Add { get; set; }
        /// <summary>
        /// 加分後學期成績平均九上
        /// </summary>
        public decimal? DomainScore91Add { get; set; }
        /// <summary>
        /// 加分後學期成績總平均
        /// </summary>
        public decimal? DomainScoreAvgAdd { get; set; }
        /// <summary>
        /// 加分後學期成績總平均排名百分比(int)
        /// </summary>
        public int? DomainScoreAvgRankPercentAdd { get; set; }

        /// <summary>
        /// 學生ID
        /// </summary>
        public string StudentID { get; set; }

        /// <summary>
        /// 領域成績
        /// </summary>
        public Dictionary<string, DomainScore5Rpt> StudDomainSocreDict = new Dictionary<string, DomainScore5Rpt>();
    }
}
