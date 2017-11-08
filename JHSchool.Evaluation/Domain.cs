using System.Collections.Generic;
using System.Xml;
using K12.Data;
using K12.Data.Configuration;

namespace JHSchool.Evaluation
{
    public class Domain
    {
        private const string ConfigName = "DomainConfiguration";
        private static Dictionary<string, List<string>> _extensions = new Dictionary<string, List<string>>();

        public static List<string> SelectAll()
        {
            List<string> domains = new List<string>();

            domains.AddRange(Select("General"));
            domains.AddRange(SelectExtension("Special"));

            return domains;
        }

        public static List<string> SelectGeneral()
        {
            return new List<string>(Subject.Domains);
            //return Select("General");
        }

        public static List<string> SelectSpecial()
        {
            return new List<string>();
            //return SelectExtension("Special");
        }

        public static void Register(string type, List<string> list)
        {
            if (!_extensions.ContainsKey(type))
                _extensions.Add(type, list);
            else
                _extensions[type] = list;
        }

        private static List<string> Select(string type)
        {
            List<string> domains = new List<string>();

            ConfigData cd = K12.Data.School.Configuration[ConfigName];

            XmlElement element = null;
            if (cd.Contains("DomainList"))
                element = cd.GetXml("DomainList", XmlHelper.LoadXml("<DomainList/>"));
            else
                element = XmlHelper.LoadXml(Properties.Resources.DefaultDomainList);

            foreach (XmlElement domainElement in element.SelectNodes("Domain[@Type='" + type + "']"))
            {
                if (string.IsNullOrEmpty(domainElement.InnerText)) continue;
                domains.Add(domainElement.InnerText);
            }

            return domains;
        }

        private static List<string> SelectExtension(string type)
        {
            if (_extensions.ContainsKey(type))
                return _extensions[type];
            else
                return new List<string>();
        }

        private static void Update(List<string> general, List<string> special)
        {
            ConfigData cd = K12.Data.School.Configuration[ConfigName];

            XmlElement element = XmlHelper.LoadXml("<DomainList/>");

            foreach (string each in general)
            {
                XmlElement domainElement = element.OwnerDocument.CreateElement("Domain");
                domainElement.SetAttribute("Type", "General");
                domainElement.InnerText = each;
                element.AppendChild(domainElement);
            }

            foreach (string each in special)
            {
                XmlElement domainElement = element.OwnerDocument.CreateElement("Domain");
                domainElement.SetAttribute("Type", "Special");
                domainElement.InnerText = each;
                element.AppendChild(domainElement);
            }

            cd.SetXml("DomainList", element);
            cd.Save();
        }

        public static void TestDrive()
        {
            //List<string> list = new List<string>(new string[] { "語文", "數學", "社會", "自然與科技", "健康與體育", "藝術與人文", "綜合活動" });
            //List<string> list2 = new List<string>(new string[] { "實用語文", "實用數學", "社會適應", "生活教育", "休閒教育", "職業教育" });

            //Domain.Update(list, list2);

            //List<JHSchool.Data.JHTeacherRecord> list = new List<JHSchool.Data.JHTeacherRecord>();
            //list.AddRange(JHSchool.Data.JHTeacher.SelectAll());
            //list.Sort();
        }
    }
}
