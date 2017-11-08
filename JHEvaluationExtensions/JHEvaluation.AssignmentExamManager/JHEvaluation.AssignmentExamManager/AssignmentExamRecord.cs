
namespace JHEvaluation.AssignmentExamManager
{
    /// <summary>
    /// 學生小考成績紀錄物件
    /// </summary>
    public class AssignmentExamRecord
    {
        /// <summary>
        /// 學生班級
        /// </summary>
        //public string ClassName { get; set; }

        /// <summary>
        /// 學生座號
        /// </summary>
        //public int? SeatNo { get; set; }
        
        /// <summary>
        /// 學生姓名
        /// </summary>
        //public string Name { get; set; }

        /// <summary>
        /// 學生學號
        /// </summary>
        //public string StudentNumber { get; set; }
    
        /// <summary>
        /// 小考分數
        /// </summary>
        public decimal? Score { get; set; }

        /// <summary>
        /// 小考編號
        /// </summary>
        public string SubExamID { get; set; }
    }
}