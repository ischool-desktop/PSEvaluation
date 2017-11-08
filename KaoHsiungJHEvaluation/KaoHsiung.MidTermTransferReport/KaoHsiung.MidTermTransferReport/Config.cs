using System;
using System.Collections.Generic;
using System.Text;
using K12.Data.Configuration;
using System.Xml;
using JHSchool.Data;

namespace KaoHsiung.MidTermTransferReport
{
    /// <summary>
    /// 表示領域或科目
    /// </summary>
    public enum DomainSubjectSetup { Domain, Subject }

    /// <summary>
    /// 表示領域或科目列印時是否要展開
    /// </summary>
    public struct DomainSubjectExpand
    {
        /// <summary>
        /// 展開: 詳列該領域下所有科目成績
        /// </summary>
        public static bool 展開 = false;
        /// <summary>
        /// 不展開: 只列該領域成績
        /// </summary>
        public static bool 不展開 = true; 
    }    

    /// <summary>
    /// 使用者設定資訊
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 使用者設定是否列印節數
        /// </summary>
        public bool PrintPeriod { get; private set; }
        /// <summary>
        /// 使用者設定是否列印權數
        /// </summary>
        public bool PrintCredit { get; private set; }
        /// <summary>
        /// 使用者設定缺曠獎懲統計開始日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 使用者設定缺曠獎懲統計結束日期
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 使用者設定要列印的假別
        /// </summary>
        public Dictionary<string, List<string>> PrintTypes { get; private set; }
        /// <summary>
        /// 使用者設定學年度
        /// </summary>
        public int SchoolYear { get; set; }
        /// <summary>
        /// 使用者設定學期
        /// </summary>
        public int Semester { get; set; }

        /// <summary>
        /// 使用者選取的學生
        /// </summary>
        public List<JHStudentRecord> Students { get; set; }

        /// <summary>
        /// 使用者設定依領域或科目進行列印
        /// </summary>
        public DomainSubjectSetup DomainSubjectSetup { get;  set; }

        /// <summary>
        /// 判斷領域的科目是否要展開
        /// 1.True:不展開、False:展開
        /// 2.領域名稱為空白(彈性課程)皆展開
        /// </summary>
        public Dictionary<string, bool> PrintDomains { get; set; }

        public Config()
        {
            PrintTypes = new Dictionary<string, List<string>>();
            DomainSubjectSetup = DomainSubjectSetup.Domain;           
        }

        /// <summary>
        /// 學生單一學期學期歷程
        /// </summary>
        public static Dictionary<string, K12.Data.SemesterHistoryItem> _StudSemesterHistoryItemDict = new Dictionary<string, K12.Data.SemesterHistoryItem>();

        internal void Load()
        {
            ConfigData cd = K12.Data.School.Configuration[Global.ReportName];

            #region 讀取使用者的節權設定
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

            #region 讀取使用者的假別設定
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

            #region 讀取使用者的領域科目設定
            if (cd.Contains("領域科目設定") && !string.IsNullOrEmpty(cd["領域科目設定"]))
            {
                XmlElement xml = K12.Data.XmlHelper.LoadXml(cd["領域科目設定"]);
                XmlElement staticNode = (XmlElement)xml.SelectSingleNode("Static");
                if (staticNode != null)
                {
                    try
                    {
                        DomainSubjectSetup = (DomainSubjectSetup)Enum.Parse(DomainSubjectSetup.GetType(), staticNode.InnerText, true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 學生服務學習時數累計
        /// </summary>
        public static Dictionary<string, decimal> _SRDict = new Dictionary<string, decimal>();
    }
}
