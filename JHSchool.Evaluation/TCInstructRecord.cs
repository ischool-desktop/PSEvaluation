using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation
{
    public class TCInstructRecord
    {
        internal string RefTeacherID { get; private set; }
        internal string RefCourseID { get; private set; }

        /// <summary>
        /// 取得學生。
        /// </summary>
        public TeacherRecord Teacher { get { return JHSchool.Teacher.Instance.Items[RefTeacherID]; } }
        /// <summary>
        /// 取得課程。
        /// </summary>
        public CourseRecord Course { get { return JHSchool.Course.Instance.Items[RefCourseID]; } }
        /// <summary>
        /// 取得教師順序。
        /// </summary>
        public string Sequence { get; private set; }
        /// <summary>
        /// 取得修課資料系統編號
        /// </summary>
        internal string ID { get; private set; }

        internal TCInstructRecord(string teacherID, string courseID, string id, string sequence)
        {
            ID = id;
            RefTeacherID = teacherID;
            RefCourseID = courseID;
            Sequence = sequence;
        }
    }
}
