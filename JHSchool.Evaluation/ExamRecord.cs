using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool.Evaluation
{
    public class ExamRecord
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int DisplayOrder { get; private set; }

        public ExamRecord()
        {
            ID = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
            DisplayOrder = 0;
        }

        public ExamRecord(XmlElement data)
            : this()
        {
            DSXmlHelper helper = new DSXmlHelper(data);
            ID = helper.GetText("@ID");
            Name = helper.GetText("ExamName");
            Description = helper.GetText("Description");
            int i;
            DisplayOrder = int.TryParse(helper.GetText("DisplayOrder"), out i) ? i : 0;
        }

    }
}
