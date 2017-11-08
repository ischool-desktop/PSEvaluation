using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHEvaluation.ScoreCalculation.SemesterHistory.DAL
{
    class SemesterHistoryItemEntity
    {
        public enum _EditMode
        { Insert,Update,Delete}

        /// <summary>
        /// 修改狀態
        /// </summary>
        public _EditMode EditMode { get; set; }

        /// <summary>
        /// 是否有學習歷程
        /// </summary>
        public bool HasSemsterHistoryRec { get; set; }

        /// <summary>
        /// 學生編號
        /// </summary>
        public string StudentID { get; set; }

        /// <summary>
        /// 學年度
        /// </summary>
        public int Schoolyear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        public int Semester { get; set; }

        /// <summary>
        /// 年級
        /// </summary>
        public int GradeYear { get; set; }

        /// <summary>
        /// 班級
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 座號
        /// </summary>
        public int SeatNo { get; set; }

        /// <summary>
        /// 班導師
        /// </summary>
        public string TeacherName { get; set; }

        /// <summary>
        /// 上課天數
        /// </summary>
        public int SchoolDayCount { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 學號
        /// </summary>
        public string StudentNumber { get; set; }
    }
}
