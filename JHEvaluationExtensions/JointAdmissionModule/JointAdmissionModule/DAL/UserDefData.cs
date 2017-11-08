using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace JointAdmissionModule.DAL
{
    // 使用者自訂資料欄位
    [TableName("Student.ScoreRankData")]
    public class UserDefData : ActiveRecord
    {
        /// <summary>
        /// 學年度
        /// </summary>
        [Field(Field = "SchoolYear", Indexed = false)]
        public int SchoolYear { get; set; }

        /// <summary>
        /// 年級
        /// </summary>
        [Field(Field = "GradeYear", Indexed = false)]
        public int GradeYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        [Field(Field = "Semester", Indexed = false)]
        public int Semester { get; set; }

        /// <summary>
        /// 年排名
        /// </summary>
        [Field(Field = "GradeRank", Indexed = false)]
        public int GradeRank { get; set; }

        /// <summary>
        /// 加分後年排名
        /// </summary>
        [Field(Field = "GradeRankAdd", Indexed = false)]
        public int GradeRankAdd { get; set; }

        /// <summary>
        /// 年排名百分比
        /// </summary>
        [Field(Field = "GradeRankPercent", Indexed = false)]
        public int GradeRankPercent { get; set; }

        /// <summary>
        /// 加分後排名百分比
        /// </summary>
        [Field(Field = "GradeRankPercentAdd", Indexed = false)]
        public int GradeRankPercentAdd { get; set; }

        /// <summary>
        /// 參照ID(ex.StudentID)
        /// </summary>
        [Field(Field = "RefID", Indexed = false)]
        public string RefID { get; set; }


    }

}
