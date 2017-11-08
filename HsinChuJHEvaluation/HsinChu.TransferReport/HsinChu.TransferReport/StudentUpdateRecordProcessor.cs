using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;

using JHSchool.Data;

namespace HsinChu.TransferReport
{
    class StudentUpdateRecordProcessor
    {
        private Document _doc;

        public StudentUpdateRecordProcessor(Document doc)
        {
            _doc = doc;
        }

        public void SetData(List<JHUpdateRecordRecord> records)
        {
            List<JHUpdateRecordRecord> list = new List<JHUpdateRecordRecord>();

            foreach (JHUpdateRecordRecord record in records)
            {

                if(record.UpdateCode.Length>1)
                {
                    // 高中
                    int intCode;
                    if (int.TryParse(record.UpdateCode, out intCode))
                    {
                        if (intCode < 100)
                            list.Add(record);
                    }
                }
                else
                {
                    // 國中
                    if (record.UpdateCode == "1")
                        list.Add(record);
                }                
            }

            if (list.Count > 1) //有兩筆以上的新生異動…？
            {
                list.Sort(delegate(JHUpdateRecordRecord a, JHUpdateRecordRecord b)
                {
                    return DateTime.Parse(a.UpdateDate).CompareTo(DateTime.Parse(b.UpdateDate));
                });
            }

            string value;
            if (list.Count <= 0)
                value = "";
            else
                value = GetDesc(list[0]);
            _doc.MailMerge.Execute(new string[] { "入學核准文號" }, new object[] { value });
        }

        private string GetDesc(JHUpdateRecordRecord record)
        {
            StringBuilder builder = new StringBuilder("");
            string orig = Common.CDate(record.ADDate);
            if (!string.IsNullOrEmpty(orig))
            {
                builder.Append(orig.Split('/')[0] + " 年 ");
                builder.Append(orig.Split('/')[1] + " 月 ");
                builder.Append(orig.Split('/')[2] + " 日 ");
                builder.Append("  ");
            }
            builder.Append(record.ADNumber);

            return builder.ToString();
        }
    }
}
