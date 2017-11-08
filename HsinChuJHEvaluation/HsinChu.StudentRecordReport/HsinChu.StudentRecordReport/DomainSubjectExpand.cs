using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HsinChu.StudentRecordReport
{
    /// <summary>
    /// 表示領域或科目列印時是否要展開
    /// </summary>
    public class DomainSubjectExpand
    {
        /// <summary>
        /// 展開: 詳列該領域下所有科目成績
        /// </summary>
        public static bool 展開 = false;
        /// <summary>
        /// 不展開: 只列該領域成績
        /// </summary>
        public static bool 不展開 = true;
    }
}
