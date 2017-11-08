using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool.Evaluation
{
    public class ScoreCalcRuleRecord
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public XmlElement Content { get; private set; }

        public ScoreCalcRuleRecord(XmlElement data)
        {
            DSXmlHelper helper = new DSXmlHelper(data);

            ID = helper.GetText("@ID");
            Name = helper.GetText("Name");
            Content = helper.GetElement("Content/ScoreCalcRule");
        }
    }
}
