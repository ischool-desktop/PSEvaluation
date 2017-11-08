using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using K12.Data;

namespace JHSchool.Evaluation.Mapping
{
    public class PerformanceDegreeMapper
    {
        private Dictionary<int, string> _DegreeToDesc;
        private Dictionary<string, int> _DescToDegree;

        public PerformanceDegreeMapper()
        {
            _DegreeToDesc = new Dictionary<int, string>();
            _DescToDegree = new Dictionary<string, int>();

            K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration["DLBehaviorConfig"];

            #region xml ref
            //<DailyBehavior Name="日常行為表現">
            //    <Item Name="愛整潔" Index="抽屜乾淨"></Item>
            //    <Item Name="有禮貌" Index="懂得向老師,學長敬禮"></Item>
            //    <Item Name="守秩序" Index="自習時間能夠安靜自習"></Item>
            //    <Item Name="責任心" Index="打掃時間,徹底整理自己打掃範圍"></Item>
            //    <Item Name="公德心" Index="不亂丟垃圾"></Item>
            //    <Item Name="友愛關懷" Index="懂得關心同學朋友"></Item>
            //    <Item Name="團隊合作" Index="團體活動能夠遵守相關規定"></Item>
            //    <PerformanceDegree>
            //        <Mapping Degree="5" Desc="完全符合"></Mapping>
            //        <Mapping Degree="4" Desc="大部份符合"></Mapping>
            //        <Mapping Degree="3" Desc="部份符合"></Mapping>
            //        <Mapping Degree="2" Desc="尚再努力"></Mapping>
            //        <Mapping Degree="1" Desc="需再努力"></Mapping>
            //    </PerformanceDegree>
            //</DailyBehavior>
            #endregion

            string cdKey = "DailyBehavior";
            if (cd.Contains(cdKey) && !string.IsNullOrEmpty(cd[cdKey]))
            {
                XmlElement dailyBehavior = XmlHelper.LoadXml(cd[cdKey]);

                foreach (XmlElement item in dailyBehavior.SelectNodes("PerformanceDegree/Mapping"))
                {
                    int degree = int.Parse(item.GetAttribute("Degree"));
                    string desc = item.GetAttribute("Desc");

                    if (!_DegreeToDesc.ContainsKey(degree))
                        _DegreeToDesc.Add(degree, desc);

                    if (!_DescToDegree.ContainsKey(desc))
                        _DescToDegree.Add(desc, degree);
                }
            }
        }

        public string GetDescriptionByDegree(int degree)
        {
            if (_DegreeToDesc.ContainsKey(degree)) return _DegreeToDesc[degree];
            else return string.Empty;
        }

        public int GetDegreeByDescription(string desc)
        {
            // TODO: 暫解
            desc = desc.Replace("份", "分");

            if (_DescToDegree.ContainsKey(desc)) return _DescToDegree[desc];
            else return int.MinValue;
        }

        public Dictionary<int, string> GetMappingTable()
        {
            return _DegreeToDesc;
        }
    }
}
