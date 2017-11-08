using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation
{
    public class TempData
    {
        /// <summary>
        /// 暫存領域成績使用，主要處理高雄國中國語文與英語成績分開顯示。
        /// </summary>
        public static Dictionary<string, Dictionary<string, decimal>> tmpStudDomainScoreDict = new Dictionary<string, Dictionary<string, decimal>>();

        /// 暫存領域成績使用，主要處理高雄國中國語文與英語成績分開顯示。
        /// </summary>
        public static Dictionary<string, Dictionary<string, decimal>> tmpStudDomainCreditDict = new Dictionary<string, Dictionary<string, decimal>>();

        
        /// <summary>
        /// 暫存功過明細統計
        /// </summary>
        public static Dictionary<string, Dictionary<string, Dictionary<string, int>>> tmpStudentDemeritAmountAllDict = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();

        /// <summary>
        /// 暫存學生缺曠明細統計
        /// </summary>
        public static Dictionary<string, Dictionary<string,Dictionary<string, int>>> tmpStudentAbsenceAmountAllDict = new Dictionary<string,Dictionary<string,Dictionary<string,int>>> ();

    }
}
