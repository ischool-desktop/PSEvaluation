using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Framework;

namespace JHSchool.Evaluation
{
    /// <summary>
    /// 代表「評量設定」資料。
    /// </summary>
    public class AssessmentSetupRecord
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        //public string StartTime { get; private set; }
        //public string EndTime { get; private set; }
        //public string AllowUpload { get; private set; }

        public AssessmentSetupRecord()
        {
            ID = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
        }

        public AssessmentSetupRecord(XmlElement data)
            : this()
        {
            XmlHelper xmldata = new XmlHelper(data);

            ID = xmldata.GetString("@ID");
            Name = xmldata.GetString("TemplateName");
            Description = xmldata.GetString("Description");
        }
    }
}
