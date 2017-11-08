using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KH_StudentScoreSummaryReport
{
    public class TempData
    {
        /// <summary>
        /// 電子報表暫存使用
        /// </summary>
        public static List<MemoryStream> _ePaperMemStreamList = new List<MemoryStream>();
    }
}
