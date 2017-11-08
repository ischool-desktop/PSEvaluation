using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FISCA.UDT;

namespace KH_StudentScoreSummaryReport
{
    /// <summary>
    /// 學生體適能UDT, 把"學生系統編號"/"學年度"/"測驗日期"當作Key
    /// </summary>
    [TableName("ischool_student_fitness")]
    public class StudentFitnessRecord : ActiveRecord
    {
        /// <summary>
        ///  學生系統編號
        /// </summary>
        [Field(Field = "ref_student_id", Indexed = false)]
        public string StudentID { get; set; }

        /// <summary>
        ///  學年度
        /// </summary>
        [Field(Field = "school_year", Indexed = false)]
        public int SchoolYear { get; set; }

        /// <summary>
        ///  測驗日期
        /// </summary>
        [Field(Field = "test_date", Indexed = false)]
        public DateTime TestDate { get; set; }
        
        /// <summary>
        ///  學校類別
        /// </summary>
        [Field(Field = "school_category", Indexed = false)]
        public string SchoolCategory { get; set; }

        /// <summary>
        ///  身高
        /// </summary>
        [Field(Field = "height", Indexed = false)]
        public string Height { get; set; }

        /// <summary>
        ///  身高常模
        /// </summary>
        [Field(Field = "height_degree", Indexed = false)]
        public string HeightDegree { get; set; }

        /// <summary>
        ///  體重
        /// </summary>
        [Field(Field = "weight", Indexed = false)]
        public string Weight { get; set; }

        /// <summary>
        ///  體重常模
        /// </summary>
        [Field(Field = "weight_degree", Indexed = false)]
        public string WeightDegree { get; set; }

        /// <summary>
        ///  坐姿體前彎
        /// </summary>
        [Field(Field = "sit_and_reach", Indexed = false)]
        public string SitAndReach { get; set; }

        /// <summary>
        ///  坐姿體前彎常模
        /// </summary>
        [Field(Field = "sit_and_reach_degree", Indexed = false)]
        public string SitAndReachDegree { get; set; }

        /// <summary>
        ///  立定跳遠
        /// </summary>
        [Field(Field = "standing_long_jump", Indexed = false)]
        public string StandingLongJump { get; set; }

        /// <summary>
        ///  立定跳遠常模
        /// </summary>
        [Field(Field = "standing_long_jump_degree", Indexed = false)]
        public string StandingLongJumpDegree { get; set; }
        
        /// <summary>
        ///  仰臥起坐
        /// </summary>
        [Field(Field = "sit_up", Indexed = false)]
        public string SitUp { get; set; }

        /// <summary>
        ///  仰臥起坐常模
        /// </summary>
        [Field(Field = "sit_up_degree", Indexed = false)]
        public string SitUpDegree { get; set; }

        /// <summary>
        ///  心肺適能
        /// </summary>
        [Field(Field = "cardiorespiratory", Indexed = false)]
        public string Cardiorespiratory { get; set; }

        /// <summary>
        ///  心肺適能常模
        /// </summary>
        [Field(Field = "cardiorespiratory_degree", Indexed = false)]
        public string CardiorespiratoryDegree { get; set; }

    }
}
