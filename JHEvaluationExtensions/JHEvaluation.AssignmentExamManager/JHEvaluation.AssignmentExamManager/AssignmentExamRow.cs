using System.Windows.Forms;

namespace JHEvaluation.AssignmentExamManager
{
    public class AssignmentExamRow : DataGridViewRow
    {
        /// <summary>
        /// 領域名稱
        /// </summary>
        public string DomainName { get; private set; }
        /// <summary>
        /// 課程名稱
        /// </summary>
        public string CourseName { get; private set; }
        /// <summary>
        /// 授課教師姓名
        /// </summary>
        public string TeacherName { get; private set; }
        /// <summary>
        /// 小考次數
        /// </summary>
        public int ExamCount { get; set; }
        /// <summary>
        /// 完整輸入次數
        /// </summary>
        public int FinishedCount { get; set; }
        /// <summary>
        /// 學年度
        /// </summary>
        public string SchoolYear { get { return "" + Course.SchoolYear; } }
        /// <summary>
        /// 學期
        /// </summary>
        public string Semester { get { return "" + Course.Semester; } }

        private AssignmentExamCourseRecord Course { get; set; }

        public AssignmentExamRow(AssignmentExamCourseRecord course)
        {
            
            Course = course;
            DomainName = course.Domain;
            CourseName = course.Name;
            ExamCount = course.ExamCount;
            FinishedCount = course.FinishedCount;

            if (course.Teachers.Count > 0) TeacherName = course.Teachers[0].TeacherName;
        }

        /// <summary>
        /// 更新畫面顯示的值
        /// </summary>
        public void UpdateDisplayValues()
        {
            this.SetValues(DomainName, CourseName, TeacherName, ExamCount, FinishedCount);
        }
    }
}