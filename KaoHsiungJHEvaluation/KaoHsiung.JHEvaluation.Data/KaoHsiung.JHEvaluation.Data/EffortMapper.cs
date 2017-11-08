using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace KaoHsiung.JHEvaluation.Data
{
    /// <summary>
    /// 努力程度對照表
    /// </summary>
    public class EffortMapper
    {
        //分數
        private Dictionary<decimal, int> _decimalToInt = new Dictionary<decimal, int>();
        private Dictionary<int, string> _intToString = new Dictionary<int, string>();

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

                    if (!_decimalToInt.ContainsKey(score))
                        _decimalToInt.Add(score, code);

                    if (!_intToString.ContainsKey(code))
                        _intToString.Add(code, text);
                }

                _scoreList = new List<decimal>();
                _scoreList.AddRange(_decimalToInt.Keys);
                _scoreList.Sort(delegate(decimal a, decimal b)
                {
                    return b.CompareTo(a);
                });
            }
            #endregion
        }

        /// <summary>
        /// 取得努力程度代碼
        /// </summary>
        /// <param name="d">分數</param>
        /// <returns>努力程度代碼</returns>
        public int GetCodeByScore(decimal score)
        {
            foreach (decimal s in _scoreList)
                if (score >= s) return _decimalToInt[s];

            return 0;
        }

        /// <summary>
        /// 取得努力程度文字
        /// </summary>
        /// <param name="d">分數</param>
        /// <returns>努力程度文字</returns>
        public string GetTextByScore(decimal score)
        {
            return GetTextByCode(GetCodeByScore(score));
        }

        /// <summary>
        /// 取得努力程度文字
        /// </summary>
        /// <param name="i">努力程度代碼</param>
        /// <returns>努力程度文字</returns>
        public string GetTextByCode(int code)
        {
            if (_intToString.ContainsKey(code))
                return _intToString[code];

            return string.Empty;
        }
    }
}
