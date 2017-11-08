using System;
using System.Collections.Generic;
using System.Text;
using EvalSubject = JHSchool.Evaluation.Subject;

namespace KaoHsiung.ClassExamScoreReportV2
{
    internal class ScoreHeaderIndexer : IEnumerable<Header>
    {
        private Dictionary<string, Header> ItemIndexs = new Dictionary<string, Header>();

        public ScoreHeaderIndexer()
        {
        }

        public void Add(string name, bool isDomain, decimal credit)
        {
            ItemIndexs.Add(GetUniqueKey(name, isDomain), new Header(name, isDomain, credit));
        }

        public bool Contains(string name, bool isDomain)
        {
            return ItemIndexs.ContainsKey(GetUniqueKey(name, isDomain));
        }

        public Header this[string name, bool isDomain]
        {
            get { return ItemIndexs[GetUniqueKey(name, isDomain)]; }
        }

        public int Count { get { return ItemIndexs.Count; } }

        #region IEnumerable<Header> 成員

        public IEnumerator<Header> GetEnumerator()
        {
            return ItemIndexs.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成員

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ItemIndexs.Values.GetEnumerator();
        }

        #endregion

        public static string GetUniqueKey(string name, bool isDomain)
        {
            return name + ":" + isDomain;
        }

        public void Sort(Dictionary<string, string> subjectDomainMap)
        {
            List<Header> headers = new List<Header>(ItemIndexs.Values);

            headers.Sort(delegate(Header x, Header y)
            {
                EvalSubject xx, yy;

                string xDomain = subjectDomainMap.ContainsKey(x.Name) ? subjectDomainMap[x.Name] : "";
                string yDomain = subjectDomainMap.ContainsKey(y.Name) ? subjectDomainMap[y.Name] : "";

                if (x.IsDomain)
                    xx = new EvalSubject(string.Empty, x.Name);
                else
                    xx = new EvalSubject(x.Name, xDomain);

                if (y.IsDomain)
                    yy = new EvalSubject(string.Empty, y.Name);
                else
                    yy = new EvalSubject(y.Name, yDomain);

                return xx.CompareTo(yy);
            });

            ItemIndexs.Clear();
            foreach (Header each in headers)
                ItemIndexs.Add(GetUniqueKey(each.Name, each.IsDomain), each);
        }
    }

    internal class Header
    {
        public Header(string name, bool isDomain, decimal firstCredit)
        {
            Name = name;
            Credits = new List<decimal>();
            Credits.Add(firstCredit);
            ColumnIndex = -1;
            IsDomain = isDomain;
        }

        public string Name { get; set; }

        public List<decimal> Credits { get; set; }

        public string GetDisplayCredit()
        {
            if (Credits.Count <= 0) return string.Empty;

            return ((double)Credits[0]).ToString();
        }

        /// <summary>
        /// 是否為領域標題，預設是 False。
        /// </summary>
        public bool IsDomain { get; private set; }

        public int ColumnIndex { get; set; }
    }

}
