using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaoHsiung.ClassExamScoreAvgComparison.DAL
{
    class DALTransfer
    {
        /// <summary>
        /// 班級依DisplayOrder排序(沒有 DisplayOrder按班級名稱
        /// </summary>
        /// <param name="classRecordList"></param>
        /// <returns></returns>
        public static List<JHSchool.Data.JHClassRecord> ClassRecordSortByDisplayOrder(List<JHSchool.Data.JHClassRecord> classRecordList)
        {
            List<JHSchool.Data.JHClassRecord> HasClassDisplayOrder = new List<JHSchool.Data.JHClassRecord>();
            List<JHSchool.Data.JHClassRecord> NoClassDisplayOrder = new List<JHSchool.Data.JHClassRecord>();
            List<JHSchool.Data.JHClassRecord> returnValue = new List<JHSchool.Data.JHClassRecord>();

            foreach (JHSchool.Data.JHClassRecord cr in classRecordList)
            {
                if (string.IsNullOrEmpty(cr.DisplayOrder))
                    NoClassDisplayOrder.Add(cr);
                else
                    HasClassDisplayOrder.Add(cr);
            }
            HasClassDisplayOrder.Sort(new Comparison<JHSchool.Data.JHClassRecord>(ClassReocrdSorter1));
            NoClassDisplayOrder.Sort(new Comparison<JHSchool.Data.JHClassRecord>(ClassReocrdSorter2));

            foreach (JHSchool.Data.JHClassRecord cr in HasClassDisplayOrder)
                returnValue.Add(cr);

            foreach (JHSchool.Data.JHClassRecord cr in NoClassDisplayOrder)
                returnValue.Add(cr);

            return returnValue;
        }

        private  static int ClassReocrdSorter1(JHSchool.Data.JHClassRecord x, JHSchool.Data.JHClassRecord y)
        {
            int intX;
            int intY;
            int.TryParse(x.DisplayOrder , out intX);
            int.TryParse(y.DisplayOrder, out intY);
            return intX.CompareTo(intY);
        }

        private static int ClassReocrdSorter2(JHSchool.Data.JHClassRecord x, JHSchool.Data.JHClassRecord y)
        {
            return x.Name.CompareTo(y.Name);        
        }

    }
}
