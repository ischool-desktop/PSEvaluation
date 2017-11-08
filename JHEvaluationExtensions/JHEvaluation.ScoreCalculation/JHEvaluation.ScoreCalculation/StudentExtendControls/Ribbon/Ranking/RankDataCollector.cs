using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking;
using JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking.Basic;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking
{
    internal class RankDataCollector
    {
        private Dictionary<string, RStudentRecord> _studentDict;
        private List<RankData> _rankData;
        private Rank _ranker;

        public RankType RankType { get; set; }
        public string Category { get; set; }

        public RankDataCollector(List<RStudentRecord> students, Rank ranker)
        {
            _studentDict = new Dictionary<string, RStudentRecord>();
            _rankData = new List<RankData>();
            _ranker = ranker;

            RankType = RankType.Class;

            foreach (RStudentRecord student in students)
                _studentDict.Add(student.ID, student);
        }

        public void AddRankData(RankData data)
        {
            _rankData.Add(data);
        }

        public void Perform()
        {
            List<RankData> rankData = new List<RankData>();
            foreach (RankData data in _rankData)
            {
                if (RankType == RankType.Class)
                {
                    #region 班排名
                    Dictionary<string, RankData> classRankData = new Dictionary<string, RankData>();
                    foreach (string id in data.Keys)
                    {
                        RStudentRecord student = _studentDict[id];
                        string className = string.Empty;

                        className = student.ClassName;

                        if (!classRankData.ContainsKey(className))
                            classRankData.Add(className, new RankData());

                        RankData eachClassRankData = classRankData[className];
                        eachClassRankData.Add(id, data[id]);
                    }

                    RankData all = new RankData();
                    all.Name = data.Name;
                    all.Tag = data.Tag;
                    foreach (RankData each in classRankData.Values)
                    {
                        all.Merge(_ranker.Perform(each));
                    }
                    rankData.Add(all);
                    #endregion
                }
                else if (RankType == RankType.GradeYear)
                {
                    #region 年排名
                    Dictionary<string, RankData> gradeRankData = new Dictionary<string, RankData>();
                    foreach (string id in data.Keys)
                    {
                        RStudentRecord student = _studentDict[id];
                        string gradeYear = student.GradeYear;

                        if (!gradeRankData.ContainsKey(gradeYear))
                            gradeRankData.Add(gradeYear, new RankData());
                        RankData eachGradeRankData = gradeRankData[gradeYear];
                        eachGradeRankData.Add(id, data[id]);
                    }

                    RankData all = new RankData();
                    all.Name = data.Name;
                    all.Tag = data.Tag;
                    foreach (RankData each in gradeRankData.Values)
                    {
                        all.Merge(_ranker.Perform(each));
                    }
                    rankData.Add(all);
                    #endregion
                }
            }
            _rankData.Clear();
            _rankData = rankData;
        }

        public List<RankData> GetTopRankData(List<RankData> rankData, string topRank)
        {
            List<RankData> newRankData = new List<RankData>();
            foreach (RankData data in _rankData)
            {
                if (topRank.EndsWith("%"))
                {
                    int i;
                    if (!int.TryParse(topRank.Substring(0, topRank.Length - 1), out i))
                        i = int.MaxValue;
                    newRankData.Add(data.GetTopByPercent(i));
                }
                else
                {
                    int i;
                    if (!int.TryParse(topRank, out i))
                        i = int.MaxValue;
                    newRankData.Add(data.GetTopByRank(i));
                }
            }
            return newRankData;
        }

        public void Export(bool isCombine, bool allRank, string topRank, bool printByClass, string reportName)
        {
            _rankData.Sort();

            if (isCombine)
            {
                List<RankData> rankData = new List<RankData>();
                if (allRank)
                    rankData = _rankData;
                else
                    rankData = GetTopRankData(_rankData, topRank);

                CombineReport report = new CombineReport(rankData, _studentDict);
                report.PrintByClass = printByClass;
                if (topRank.EndsWith("%")) report.Display = "%";
                report.RankType = RankType;
                report.ReportName = reportName;
                report.Export();
            }
            else if (allRank)
            {
                SeparateAllReport report = new SeparateAllReport(_rankData, _studentDict);
                report.PrintByClass = printByClass;
                report.RankType = RankType;
                report.ReportName = reportName;
                report.Export();
            }
            else
            {
                List<RankData> rankData = new List<RankData>();
                rankData = GetTopRankData(_rankData, topRank);
                SeparateTopReport report = new SeparateTopReport(rankData, _studentDict);
                if (topRank.EndsWith("%")) report.Display = "%";
                report.RankType = RankType;
                report.ReportName = reportName;
                report.Category = Category;
                report.Export();
            }

            FISCA.LogAgent.ApplicationLog.Log("成績系統.排名", reportName, string.Format("產生{0}資料", reportName));
        }

        public void Clear()
        {
            _rankData.Clear();
        }
    }

    public class RankScore
    {
        public RankScore(decimal score, int? rank)
        {
            Score = score;
            Rank = rank;
        }

        public decimal Score { get; set; }
        public int? Rank { get; set; }
    }

    public class RankData : Dictionary<string, RankScore>, IComparable
    {
        public string Name { get; set; }
        public object Tag { get; set; }

        public RankData()
        {
            Name = string.Empty;
        }

        public void Merge(RankData other)
        {
            foreach (string id in other.Keys)
                this.Add(id, other[id]);
        }

        public RankData GetTopByPercent(int percent)
        {
            RankData result = new RankData();
            result.Name = this.Name;
            if (this.Tag != null) result.Tag = this.Tag;

            foreach (string id in this.Keys)
            {
                decimal rank = (decimal)this[id].Rank;
                rank = rank / this.Count * 100;
                rank = decimal.Round(rank, 0, MidpointRounding.AwayFromZero);
                rank = rank <= 0 ? 1 : rank;
                this[id].Rank = (int)rank;

                if (rank <= percent)
                    result.Add(id, this[id]);
            }

            return result;
        }

        public RankData GetTopByRank(int rank)
        {
            RankData result = new RankData();
            result.Name = this.Name;
            if (this.Tag != null) result.Tag = this.Tag;

            foreach (string id in this.Keys)
            {
                if (this[id].Rank <= rank)
                    result.Add(id, this[id]);
            }

            return result;
        }

        #region IComparable 成員

        public int CompareTo(object obj)
        {
            RankData data = obj as RankData;
            List<string> list = new List<string>(new string[] { "語文", "國語文", "國文", "國語", "英文", "英語", "英語文", "數學" });
            int a = list.IndexOf(this.Name);
            int b = list.IndexOf(data.Name);
            if (a < 0) a = int.MaxValue;
            if (b < 0) b = int.MaxValue;
            return a.CompareTo(b);
        }

        #endregion
    }

    public enum RankType { Class, GradeYear }
}
