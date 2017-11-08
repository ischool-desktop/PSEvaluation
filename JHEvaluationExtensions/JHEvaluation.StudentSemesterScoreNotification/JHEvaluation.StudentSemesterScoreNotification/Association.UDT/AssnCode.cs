using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace JHEvaluation.StudentSemesterScoreNotification.Association.UDT
{
    [TableName("JHSchool.Association.UDT")]
    class AssnCode : ActiveRecord
    {
        //Key ID
        /// <summary>
        /// ID
        /// </summary>
        [Field(Field = "StudentID", Indexed = true)]
        public string StudentID { get; set; }

        /// <summary>
        /// Key 學年度
        /// </summary>
        [Field(Field = "SchoolYear", Indexed = false)]
        public string SchoolYear { get; set; }

        /// <summary>
        /// Key 學期
        /// </summary>
        [Field(Field = "Semester", Indexed = false)]
        public string Semester { get; set; }

        /// <summary>
        /// 課程成績相關資訊
        /// </summary>
        [Field(Field = "Scores", Indexed = false)]
        public string Scores { get; set; }
    }

    class AssnScore
    {
        public string Score { get; set; }
        public string Effort { get; set; }
        public string Text { get; set; }
    }
}
