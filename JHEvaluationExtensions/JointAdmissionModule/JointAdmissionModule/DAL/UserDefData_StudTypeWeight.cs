using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace JointAdmissionModule.DAL
{
    [TableName("Student.ScoreStudAddWeight")]
    class UserDefData_StudTypeWeight : ActiveRecord
    {
        /// <summary>
        /// 學校類別
        /// </summary>
        [Field(Field = "SchoolType", Indexed = false)]
        public string SchoolType { get; set; }
        
        
        /// <summary>
        /// 學生類別(考生特種身分)
        /// </summary>
        [Field(Field = "JoinStudType", Indexed = false)]
        public string JoinStudType { get; set; }


        /// <summary>
        /// 學生類別
        /// </summary>
        [Field(Field = "StudentType", Indexed = false)]
        public string StudentType { get; set; }

        /// <summary>
        /// 加分比(1.)
        /// </summary>
        [Field(Field = "AddWeight", Indexed = false)]
        public decimal  AddWeight { get; set; }

        /// <summary>
        /// 檢查不列入排名。(True 表示不參與排名)
        /// </summary>
        [Field(Field = "CheckNonRank", Indexed = false)]
        public bool CheckNonRank { get; set; }

        /// <summary>
        /// 特種身分代碼
        /// </summary>
        [Field(Field = "JoinStudTypeCode", Indexed = false)]
        public string JoinStudTypeCode { get; set; }

    }
}
