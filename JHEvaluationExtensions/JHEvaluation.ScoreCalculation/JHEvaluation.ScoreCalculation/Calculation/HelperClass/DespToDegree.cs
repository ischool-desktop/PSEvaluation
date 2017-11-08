using JHSchool.Evaluation.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.Calculation.HelperClass
{
    /// <summary>
    /// 日常生活表現轉換
    /// </summary>
    class DespToDegree
    {
        private PerformanceDegreeMapper _mapper;
        /// <summary>
        /// 日常生活表現對照表(Desp,Degree)
        /// </summary>
        private Dictionary<string, int> _Dic;

        /// <summary>
        /// 建構子
        /// </summary>
        public DespToDegree()
        {
            _mapper = new PerformanceDegreeMapper();

            //取得日常生活表現MappingTable
            Dictionary<int, string> mapping = _mapper.GetMappingTable();
            _Dic = new Dictionary<string, int>();

            //建立DescToDegree字典
            foreach (int degree in mapping.Keys)
            {
                if (!_Dic.ContainsKey(mapping[degree]))
                {
                    _Dic.Add(mapping[degree], degree);
                }
            }
        }

        /// <summary>
        /// 取得Degree
        /// </summary>
        public int GetDegree(string s)
        {
            return _Dic.ContainsKey(s) ? _Dic[s] : int.MinValue;
        }
    }
}
