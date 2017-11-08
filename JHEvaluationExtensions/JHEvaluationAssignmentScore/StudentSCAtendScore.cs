using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;

namespace JHEvaluation.AssignmentScore
{
    public class StudentSCAtendScore
    {
        /// <summary>
        /// 學生系統編號
        /// </summary>
        public string StudentID { get; set; }
        /// <summary>
        /// 學生系統編號
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 班級系統編號, 小郭, 2013/12/27
        /// </summary>
        public string ClassId { get; set; }
        /// <summary>
        /// 班級名稱
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 座號
        /// </summary>
        public int SeatNo { get; set; }
        /// <summary>
        /// 學號
        /// </summary>
        public string StudentNumber { get; set; }
        /// <summary>
        /// 修課成績紀錄
        /// </summary>
        public JHSCAttendRecord SCAtendRec {get;set;}

        /// <summary>
        /// 取得平時評量分數文字
        /// </summary>
        /// <returns></returns>
        public string GetOrdinarilyScore()
        {
            if (SCAtendRec.OrdinarilyScore.HasValue)
                return SCAtendRec.OrdinarilyScore.Value.ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// 取得平時評量努力程度文字
        /// </summary>
        /// <returns></returns>
        public string GetOrdinarilyEffort()
        {
            if (SCAtendRec.OrdinarilyEffort.HasValue)
                return SCAtendRec.OrdinarilyEffort.Value.ToString ();
            else
                return string.Empty;        
        }


    }
}
