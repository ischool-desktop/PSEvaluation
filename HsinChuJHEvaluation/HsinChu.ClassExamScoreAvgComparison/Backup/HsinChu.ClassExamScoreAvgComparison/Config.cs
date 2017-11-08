using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data.Configuration;
using K12.Data;
using System.Xml;
using JHSchool.Data;

namespace HsinChu.ClassExamScoreAvgComparison
{
    internal class Config
    {
        public List<string> PrintItems { get; set; }
        public string RankMethod { get; set; }
        public bool HasValue { get; private set; }
        public int SchoolYear { get; set; }
        public int Semester { get; set; }

        private string _name;

        public Config(string name)
        {
            _name = name;
            PrintItems = new List<string>();
            Load();
        }

        public void Save()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Config><RankMethod>" + RankMethod + "</RankMethod><PrintItems/></Config>");
            XmlElement print = (XmlElement)doc.DocumentElement.SelectSingleNode("PrintItems");
            foreach (string item in PrintItems)
            {
                XmlElement eItem = doc.CreateElement("Item");
                eItem.InnerText = item;
                print.AppendChild(eItem);
            }

            ConfigData cd = School.Configuration[_name];
            cd["xml"] = doc.DocumentElement.OuterXml;
            cd.Save();
        }

        public void Load()
        {
            ConfigData cd = School.Configuration[_name];
            if (cd.Contains("xml") && !string.IsNullOrEmpty(cd["xml"]))
            {
                HasValue = true;
                XmlElement element = XmlHelper.LoadXml(cd["xml"]);
                RankMethod = element.SelectSingleNode("RankMethod").InnerText;
                PrintItems.Clear();
                foreach (XmlElement item in element.SelectNodes("PrintItems/Item"))
                    PrintItems.Add(item.InnerText);

                //<Config>
                //  <RankMethod></RankMethod>
                //  <PrintItems>
                //      <Item/>
                //      <Item/>
                //  </PrintItems>
                //</Config>
            }
            else
            {
                HasValue = false;
            }
        }
    }
}
