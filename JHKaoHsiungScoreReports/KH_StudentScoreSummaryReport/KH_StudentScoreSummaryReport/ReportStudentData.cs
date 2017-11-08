using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KH_StudentScoreSummaryReport
{
    /// <summary>
    /// 高中匯入檔案用的Class
    /// </summary>
    public class ReportStudentData
    {
        /// <summary>
        /// 班級
        /// </summary> 
        public string ClassName { get; set; }

        /// <summary>
        /// 座號
        /// </summary> 
        public string SeatNo { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 家長姓名
        /// </summary> 
        public string ParentName { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 性別代碼
        /// </summary>
        public string GenderCode { get; set; }

        /// <summary>
        /// 身分證字號
        /// </summary>
        public string IDNumber { get; set; }
        
        /// <summary>
        /// 郵區號
        /// </summary>
        public string ZipCode { get; set; }
        
        /// <summary>
        /// 聯絡地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 電話
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 手機
        /// </summary>
        public string CellPhone { get; set; }

        /// <summary>
        /// 特種身分(原民未持1、原民認證2、身心障礙3、其他4)
        /// </summary>
        public int? SpceTypeCode { get; set; }

        /// <summary>
        /// 一上名次
        /// </summary> 
        public int? Rank11 { get; set; }

        /// <summary>
        /// 一上名次百分比
        /// </summary> 
        public int? RankPercent11 { get; set; }

        /// <summary>
        /// 一下名次
        /// </summary> 
        public int? Rank12 { get; set; }

        /// <summary>
        /// 一下名次百分比
        /// </summary> 
        public int? RankPercent12 { get; set; }

        // 二上名次
        public int? Rank21 { get; set; }

        // 二上名次百分比
        public int? RankPercent21 { get; set; }

        // 二下名次
        public int? Rank22 { get; set; }

        // 二下名次百分比
        public int? RankPercent22 { get; set; }

        // 三上名次
        public int? Rank31 { get; set; }

        // 三上名次百分比
        public int? RankPercent31 { get; set; }

        // 五學期名次
        public int? AvgRank5 { get; set; }

        // 五學期名次百分比
        public int? AvgRankPercent5 { get; set; }

        /// <summary>
        /// 轉入異動日期
        /// </summary>
        public string UpdateDate3 { get; set; }
    }
}
