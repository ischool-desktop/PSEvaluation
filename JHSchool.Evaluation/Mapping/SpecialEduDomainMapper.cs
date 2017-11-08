//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Xml;
//using K12.Data;
//using K12.Data.Configuration;

//namespace JHSchool.Evaluation.Mapping
//{
//    public class SpecialEduDomainMapper
//    {
//        private string TitleName = "特殊教育領域設定";

//        public List<string> GetSpecialEduDomainList()
//        {
//            List<string> list = new List<string>();

//            ConfigData cd = K12.Data.School.Configuration[TitleName];
//            if (cd.Contains("xml") && !string.IsNullOrEmpty(cd["xml"]))
//            {
//                XmlElement element = cd.GetXml("xml", XmlHelper.LoadXml("<SpecialEduDomainList/>"));
//                foreach (XmlElement domainElement in element.SelectNodes("Domain"))
//                {
//                    if (string.IsNullOrEmpty(domainElement.InnerText)) continue;
//                    list.Add(domainElement.InnerText);
//                }
//            }

//            return list;
//        }
//    }
//}
