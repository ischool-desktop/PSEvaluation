using JHSchool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHEvaluation.ExamScoreCopy
{
    /// <summary>
    /// 使用者選擇的設定值
    /// </summary>
    internal class UserConfig
    {
        public UserConfig()
        {
            TargetSubjects = new List<string>();
        }

        /// <summary>
        /// 選擇的學年度
        /// </summary>
        public int SchoolYear { get; set; }
        /// <summary>
        /// 選擇的學期
        /// </summary>
        public int Semester { get; set; }
        /// <summary>
        /// 選擇的試別
        /// </summary>
        public JHExamRecord Exam { get; set; }
        /// <summary>
        /// 選擇的來源科目
        /// </summary>
        public string SourceSubject { get; set; }
        /// <summary>
        /// 選擇的目的科目
        /// </summary>
        public List<string> TargetSubjects { get; set; }
    }
}
