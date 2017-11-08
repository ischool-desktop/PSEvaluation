using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Words;

namespace HsinChu.TransferReport
{
    class SemesterHistoryProcessor
    {
        public SemesterHistoryProcessor(DocumentBuilder builder, SemesterMap map)
        {
            List<string> fieldName = new List<string>();
            List<string> fieldValue = new List<string>();

            int index = 0;
            foreach (string name in new string[] { "一上", "一下", "二上", "二下", "三上", "三下" })
            {
                fieldName.Add(name);
                if (map.SchoolYearMapping.ContainsKey(index))
                    fieldValue.Add(map.SchoolYearMapping[index] + "學年度");
                else
                    fieldValue.Add("");
                index++;
            }

            builder.Document.MailMerge.Execute(fieldName.ToArray(), fieldValue.ToArray());
        }
    }
}
