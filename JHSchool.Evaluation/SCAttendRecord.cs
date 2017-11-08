using System;
using System.Collections.Generic;
using System.Text;
using Framework;

namespace JHSchool.Evaluation
{
    /// <summary>
    /// 學生修課資訊
    /// </summary>
    public class SCAttendRecord
    {
        //internal string OverrideRequiredBy { get;private set; }
        //internal bool? OverrideRequired { get; private set; }

        internal string RefStudentID { get; private set; }
        internal string RefCourseID { get; private set; }
        /// <summary>
        /// 取得學生
        /// </summary>
        public StudentRecord Student { get { return JHSchool.Student.Instance.Items[RefStudentID]; } }
        /// <summary>
        /// 取得課程
        /// </summary>
        public CourseRecord Course { get { return JHSchool.Course.Instance.Items[RefCourseID]; } }
        /// <summary>
        /// 取得修課資料系統編號
        /// </summary>
        public string ID { get; private set; }
        /// <summary>
        /// 取得修課總成績
        /// </summary>
        public decimal? Score { get; private set; }
        /// <summary>
        /// 取得修課努力程度
        /// </summary>
        public int? Effort { get; private set; }
        /// <summary>
        /// 取得修課文字描述
        /// </summary>
        public string Text { get; private set; }

        internal SCAttendRecord(string studentID, string courseID, string id, string score, string effort, string text)
        {
            RefStudentID = studentID;
            RefCourseID = courseID;
            ID = id;
            decimal d = 0;
            if ( decimal.TryParse(score, out d) )
                Score = d;
            else
                Score = null;

            int i;
            if (int.TryParse(effort, out i))
                Effort = i;
            else
                Effort = null;

            Text = text;
        }
    }
}
