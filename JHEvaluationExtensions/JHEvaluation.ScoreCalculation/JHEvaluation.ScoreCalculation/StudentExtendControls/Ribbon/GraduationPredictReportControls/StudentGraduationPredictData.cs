using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReportControls
{
    /// <summary>
    /// 學生預警報表資料
    /// </summary>
    class StudentGraduationPredictData
    {
        /// <summary>
        /// 學校名稱
        /// </summary>
        public string SchoolName {get;set;}

        /// <summary>
        /// 學校電話
        /// </summary>
        public string SchoolPhone { get; set; }

        /// <summary>
        /// 學校地址
        /// </summary>
        public string SchoolAddress { get; set; }

        /// <summary>
        /// 收件人地址(可選戶籍或聯絡，預設聯絡)
        /// </summary>
        public string AddresseeAddress { get; set; }

        /// <summary>
        /// 收件人姓名(可選父、母、監護人)
        /// </summary>
        public string AddresseeName { get; set; }

        /// <summary>
        /// 班級
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 座號
        /// </summary>
        public string SeatNo { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 學號
        /// </summary>
        public string StudentNumber { get; set; }

        /// <summary>
        /// 一上文字
        /// </summary>
        public string Text11 { get; set; }

        /// <summary>
        /// 一下文字
        /// </summary>
        public string Text12 { get; set; }

        /// <summary>
        /// 二上文字
        /// </summary>
        public string Text21 { get; set; }

        /// <summary>
        /// 二下文字
        /// </summary>
        public string Text22 { get; set; }

        /// <summary>
        /// 三上文字
        /// </summary>
        public string Text31 { get; set; }

        /// <summary>
        /// 三下文字
        /// </summary>
        public string Text32 { get; set; }

        /// <summary>
        /// 發文日期
        /// </summary>
        public string DocDate { get; set; }

        /// <summary>
        /// 所有說明
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 領域說明文字
        /// </summary>
        public string DomainText { get; set; }

        /// <summary>
        /// 功過累計明細文字
        /// </summary>
        public string DemeritText { get; set; }

        /// <summary>
        /// 缺曠累計明細文字
        /// </summary>
        public string AbsenceText { get; set; }
    }
}
