using System;
using System.Collections.Generic;
using System.Text;

namespace HsinChu.ClassExamScoreReportV21
{
    public class SubjectSorter
    {
        private static List<string> _list = new List<string>(new string[] { "國語文", "國文", "英文", "英語", "數學", "歷史", "地理", "公民", "理化", "生物" });

        public static int Sort(string x, string y)
        {
            int ix = _list.IndexOf(x);
            int iy = _list.IndexOf(y);

            if (ix >= 0 && iy >= 0) return ix.CompareTo(iy);
            else if (ix >= 0) return -1;
            else if (iy >= 0) return 1;
            else return x.CompareTo(y);
        }
    }
}
