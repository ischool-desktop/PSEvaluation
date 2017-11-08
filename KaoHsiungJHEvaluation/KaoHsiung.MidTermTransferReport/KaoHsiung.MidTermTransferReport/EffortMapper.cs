using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace KaoHsiung.MidTermTransferReport
{
    /// <summary>
    /// 努力程度文字對照
    /// 
    /// <![CDATA[
    /// <EffortList>
    ///     <Effort Code="5" Name="表現優異" Score="90"/>
    ///     <Effort Code="4" Name="表現良好" Score="80"/>
    ///     <Effort Code="3" Name="表現尚可" Score="70"/>
    ///     <Effort Code="2" Name="需在加油" Score="60"/>
    ///     <Effort Code="1" Name="有待改進" Score=""/>
    /// </EffortList>
    /// ]]>
    /// 
    /// </summary>
    internal class EffortMapper
    {
        private Dictionary<decimal, string> _effortList = new Dictionary<decimal, string>();
        private Dictionary<int, string> _effortText = new Dictionary<int, string>();
        private List<decimal> _scoreList = new List<decimal>();

        public EffortMapper()
        {
            #region 努力程度文字對照
            K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration["努力程度對照表"];
            if (cd.Contains("xml") && !string.IsNullOrEmpty(cd["xml"]))
            {
                XmlElement element = K12.Data.XmlHelper.LoadXml(cd["xml"]);

                foreach (XmlElement each in element.SelectNodes("Effort"))
                {
                    int code = int.Parse(each.GetAttribute("Code"));
                    string text = each.GetAttribute("Name");

                    decimal score;
                    if (!decimal.TryParse(each.GetAttribute("Score"), out score))
                        score = 0;

                    if (!_effortList.ContainsKey(score))
                        _effortList.Add(score, text);

                    if (!_effortText.ContainsKey(code))
                        _effortText.Add(code, text);
                }

                _scoreList = new List<decimal>();
                _scoreList.AddRange(_effortList.Keys);
                _scoreList.Sort(delegate(decimal a, decimal b)
                {
                    return b.CompareTo(a);
                });
            }
            #endregion
        }

        /// <summary>
        /// 從分數取得努力程度文字描述
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public string GetTextByDecimal(decimal d)
        {
            foreach (decimal score in _scoreList)
                if (d >= score) return _effortList[score];

            return string.Empty;
        }

        /// <summary>
        /// 從努力程度代碼取得努力程度文字描述
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public string GetTextByInt(int i)
        {
            if (_effortText.ContainsKey(i))
                return _effortText[i];

            return string.Empty;
        }
    }
}
