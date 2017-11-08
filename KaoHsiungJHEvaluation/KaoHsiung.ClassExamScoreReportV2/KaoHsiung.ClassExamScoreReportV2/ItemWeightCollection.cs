using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaoHsiung.ClassExamScoreReportV2
{
    /// <summary>
    /// 代表科目或領域的權重集合。
    /// </summary>
    internal class ItemWeightCollection
    {
        private Dictionary<string, decimal> _weights;

        public ItemWeightCollection()
        {
            _weights = new Dictionary<string, decimal>();
        }

        public void Add(string item, decimal weight)
        {
            //if (weight <= 0)
            //    throw new ArgumentException("計算比例必須是大於零的數字。");

            _weights.Add(item, weight);
        }

        public bool Contains(string item)
        {
            return _weights.ContainsKey(item);
        }

        public decimal this[string item]
        {
            get { return _weights[item]; }
        }

        public int Count { get { return _weights.Count; } }

        public IEnumerable<string> Keys { get { return _weights.Keys; } }

        public decimal GetWeightSum()
        {
            decimal sum = 0;
            foreach (decimal each in _weights.Values)
                sum += each;
            return sum;
        }
    }
}
