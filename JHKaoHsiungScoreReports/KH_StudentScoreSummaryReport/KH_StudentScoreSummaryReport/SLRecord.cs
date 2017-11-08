using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace KH_StudentScoreSummaryReport
{
    [TableName("K12.Service.Learning.Record")]
    public class SLRecord : ActiveRecord
    {
        /// <summary>
        /// 學生參考
        /// </summary>
        [Field(Field = "ref_student_id", Indexed = true)]
        public string RefStudentID { get; set; }

        /// <summary>
        /// 學年度
        /// </summary>
        [Field(Field = "school_year", Indexed = false)]
        public int SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        [Field(Field = "semester", Indexed = false)]
        public int Semester { get; set; }

        /// <summary>
        /// 發生日期
        /// </summary>
        [Field(Field = "occur_date", Indexed = false)]
        public DateTime OccurDate { get; set; }

        /// <summary>
        /// 事由
        /// </summary>
        [Field(Field = "reason", Indexed = false)]
        public string Reason { get; set; }

        /// <summary>
        /// 時數
        /// </summary>
        [Field(Field = "hours", Indexed = false)]
        public decimal Hours { get; set; }

        /// <summary>
        /// 主辦單位
        /// </summary>
        [Field(Field = "organizers", Indexed = false)]
        public string Organizers { get; set; }

        /// <summary>
        /// 登錄日期
        /// </summary>
        [Field(Field = "register_date", Indexed = false)]
        public DateTime RegisterDate { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [Field(Field = "remark", Indexed = false)]
        public string Remark { get; set; }
    }
}
