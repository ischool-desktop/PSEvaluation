using System;
using System.Collections.Generic;

namespace JHSchool.Evaluation.Calculation
{
    /// <summary>
    /// 畢業判斷傳回結果細節
    /// </summary>
    public class ResultDetail : IComparable<ResultDetail>
    {
        /// <summary>
        /// 學生系統編號
        /// </summary>
        public string StudentID { get; private set; }
        /// <summary>
        /// 年級
        /// </summary>
        public string GradeYear { get; private set; }
        /// <summary>
        /// 學期
        /// </summary>
        public string Semester { get; private set; }
        /// <summary>
        /// 訊息清單
        /// </summary>
        public List<string> Messages { get; private set; }
        /// <summary>
        /// 訊息細節
        /// </summary>
        public List<string> Details { get; private set; }

        /// <summary>
        /// 建構式，傳入學生系統編號、年級及學期
        /// </summary>
        /// <param name="student_id">學生系統編號</param>
        /// <param name="gradeYear">年級</param>
        /// <param name="semester">學期</param>
        public ResultDetail(string student_id, string gradeYear, string semester)
        {
            StudentID = student_id;
            GradeYear = gradeYear;
            Semester = semester;
            Messages = new List<string>();
            Details = new List<string>();
        }

        /// <summary>
        /// 增加單一訊息
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(string message)
        {
            if (!Messages.Contains(message))
                Messages.Add(message);
        }

        /// <summary>
        /// 增加多個訊息
        /// </summary>
        /// <param name="messages"></param>
        public void AddMessages(IEnumerable<string> messages)
        {
            foreach (string message in messages)
                AddMessage(message);
        }

        /// <summary>
        /// 增加細節訊息
        /// </summary>
        /// <param name="detail"></param>
        public void AddDetail(string detail)
        {
            if (!Details.Contains(detail))
                Details.Add(detail);
        }

        /// <summary>
        /// 增加多個細節訊息
        /// </summary>
        /// <param name="details"></param>
        public void AddDetails(IEnumerable<string> details)
        {
            foreach (string detail in details)
                AddDetail(detail);
        }

        #region IComparable<ResultDetail> 成員

        /// <summary>
        /// 比較訊息是否相同，以年級及學期來做判斷。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ResultDetail other)
        {
            if (this.GradeYear.Equals(other.GradeYear))
                return this.Semester.CompareTo(other.Semester);
            else
                return this.GradeYear.CompareTo(other.GradeYear);
        }

        #endregion
    }
}