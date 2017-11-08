using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaoHsiung.ExportCourseScoreDiff
{
    /// <summary>
    /// 課程學生
    /// </summary>
    public class CourseStudentsScore
    {
        /// <summary>
        /// 課程編號
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 課程名稱
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// 班級
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 座號
        /// </summary>
        public int? SeatNo { get; set; }

        /// <summary>
        /// 學號
        /// </summary>
        public string StudentNumber { get; set; }

        /// <summary>
        /// 學生系統編號
        /// </summary>
        public string StudentID { get; set; }

        /// <summary>
        /// 學生姓名
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// 成績內容
        /// </summary>
        public Dictionary<string, ScoreItem> ScoreItems = new Dictionary<string, ScoreItem>();
    }
}
