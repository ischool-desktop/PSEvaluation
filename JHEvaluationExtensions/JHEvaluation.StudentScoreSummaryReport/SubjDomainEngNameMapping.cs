using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Data;
using System.Xml.Linq;
using System.Data;

namespace JHEvaluation.StudentScoreSummaryReport
{
    /// <summary>
    /// 領域與科目中英文對照
    /// </summary>
    public class SubjDomainEngNameMapping
    {
        private Dictionary<string, string> _DomainNameDict;
        private Dictionary<string, string> _SubjectNameDict;

        public SubjDomainEngNameMapping()
        {
            _DomainNameDict = new Dictionary<string, string>();
            _SubjectNameDict = new Dictionary<string, string>();
            GetData();
        }

        /// <summary>
        /// 取得資料庫內中英文對照資料
        /// </summary>
        private void GetData()
        {
            try
            {
                QueryHelper qh = new QueryHelper();
                // 取得國中領域、科目中英文對照
                string query = "select content from list where name='JHEvaluation_Subject_Ordinal';";
                DataTable dt = qh.Select(query);
                string strXml = "";

                if (dt.Rows.Count > 0)
                {
                    strXml = dt.Rows[0][0].ToString();
                    strXml = strXml.Replace("&lt;", "<");
                    strXml = strXml.Replace("&gt;", ">");
                }
                XElement elmRoot = XElement.Parse(strXml);

                if (elmRoot != null)
                {
                    foreach (XElement elm in elmRoot.Elements("Configuration"))
                    {
                        if (elm.Element("Subjects") != null)
                            foreach (XElement elmS in elm.Element("Subjects").Elements("Subject"))
                            {
                                string key = GetAttribute("Name", elmS);
                                if (!_SubjectNameDict.ContainsKey(key))
                                    _SubjectNameDict.Add(key, GetAttribute("EnglishName", elmS));
                            }

                        if (elm.Element("Domains")!=null)
                            foreach (XElement elmD in elm.Element("Domains").Elements("Domain"))
                            {
                                string key = GetAttribute("Name", elmD);

                                if (!_DomainNameDict.ContainsKey(key))
                                    _DomainNameDict.Add(key, GetAttribute("EnglishName", elmD));
                            }
                    }
                }
            }
            catch (Exception ex)
            { 
                
            }

        }


        private string GetAttribute(string name, XElement elm)
        {
            string retVal = "";
            if (elm.Attribute(name) != null)
                retVal = elm.Attribute(name).Value;

            return retVal;
        }

        /// <summary>
        /// 取得領域英文名稱
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetDomainEngName(string name)
        {
            string retVal = "";
            if (_DomainNameDict.ContainsKey(name))
                retVal = _DomainNameDict[name];
            return retVal;
        }

        /// <summary>
        /// 取得科目英文名稱
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetSubjectEngName(string name)
        {
            string retVal = "";
            if (_SubjectNameDict.ContainsKey(name))
                retVal = _SubjectNameDict[name];
            return retVal;
        }
    }
}
