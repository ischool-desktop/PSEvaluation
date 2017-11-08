using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using K12.Data.Configuration;
using System.Xml;

namespace KaoHsiung.TransferReport
{
    public class Config
    {
        public bool PrintPeriod { get; private set; }
        public bool PrintCredit { get; private set; }
        public Dictionary<string, List<string>> PrintTypes { get; private set; }
        public Dictionary<string, List<string>> PrintSubjects { get; private set; }
        public Dictionary<string, bool> PrintDomains { get; private set; }
        public string DomainSubjectSetup { get; private set; }

        public string ReportName { get; private set; }
        public List<JHStudentRecord> Students { get; private set; }

        public Config(string name)
        {
            ReportName = name;
            PrintTypes = new Dictionary<string, List<string>>();
            DomainSubjectSetup = "Domain";
        }

        internal void SetStudents(List<JHStudentRecord> students)
        {
            Students = students;
        }

        internal void Load()
        {
            ConfigData cd = K12.Data.School.Configuration[ReportName];

            #region 讀取節權設定
            if (cd.Contains("節權設定") && !string.IsNullOrEmpty(cd["節權設定"]))
            {
                XmlElement config = K12.Data.XmlHelper.LoadXml(cd["節權設定"]);
                PrintPeriod = bool.Parse(config.SelectSingleNode("Period").InnerText);
                PrintCredit = bool.Parse(config.SelectSingleNode("Credit").InnerText);
            }
            else
            {
                PrintPeriod = false;
                PrintCredit = false;
            }
            #endregion

            #region 假別設定
            if (cd.Contains("假別設定") && !string.IsNullOrEmpty(cd["假別設定"]))
            {
                XmlElement config = K12.Data.XmlHelper.LoadXml(cd["假別設定"]);
                foreach (XmlElement type in config.SelectNodes("Type"))
                {
                    string typeName = type.GetAttribute("Text");
                    if (!PrintTypes.ContainsKey(typeName))
                        PrintTypes.Add(typeName, new List<string>());

                    foreach (XmlElement absence in type.SelectNodes("Absence"))
                    {
                        string absenceName = absence.GetAttribute("Text");
                        if (!PrintTypes[typeName].Contains(absenceName))
                            PrintTypes[typeName].Add(absenceName);
                    }
                }
            }
            #endregion

            #region 領域科目設定
            if (cd.Contains("領域科目設定") && !string.IsNullOrEmpty(cd["領域科目設定"]))
            {
                XmlElement xml = K12.Data.XmlHelper.LoadXml(cd["領域科目設定"]);
                XmlElement staticNode = (XmlElement)xml.SelectSingleNode("Static");
                if (staticNode != null)
                {
                    if (staticNode.InnerText == "Domain")
                        DomainSubjectSetup = "Domain";
                    else
                        DomainSubjectSetup = "Subject";
                }
            }
            #endregion
        }

        internal void SetPrintDomains(Dictionary<string, bool> domains)
        {
            PrintDomains = domains;
        }

        internal void SetPrintSubjects(Dictionary<string, List<string>> subjects)
        {
            PrintSubjects = subjects;
        }

        /// <summary>
        /// 服務學時數暫存使用
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> _SLRDict = new Dictionary<string, Dictionary<string, string>>();
    }
}
