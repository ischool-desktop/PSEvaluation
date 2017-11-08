using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HsinChu.StudentRecordReport
{
    public class DomainSorter
    {
        private static List<string> _list = new List<string>(new string[] { "語文","數學","社會","藝術與人文","自然與生活科技","健康與體育","綜合活動","彈性課程"});

        public static int Sort1(string x, string y)
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
