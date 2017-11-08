using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHEvaluation.Rating
{
    /// <summary>
    /// 代表科目或領域的權重集合。
    /// </summary>
    internal class ItemWeightCollection : IEnumerable<ScoreItem>
    {
        private Dictionary<ScoreItem, decimal> _weights;

        public ItemWeightCollection()
        {
            _weights = new Dictionary<ScoreItem, decimal>();
        }

        public void Add(ScoreItem item, decimal weight)
        {
            if (weight <= 0)
                throw new ArgumentException("計算比例必須是大於零的數字。");

            _weights.Add(item, weight);
        }

        public bool Contains(ScoreItem item)
        {
            return _weights.ContainsKey(item);
        }

        public decimal this[ScoreItem item]
        {
            get { return _weights[item]; }
        }

        public int Count { get { return _weights.Count; } }

        public IEnumerable<ScoreItem> Keys { get { return _weights.Keys; } }

        public IEnumerable<ScoreItem> Where(ScoreType type)
        {
            List<ScoreItem> items = new List<ScoreItem>();

            foreach (ScoreItem each in this)
                if (each.Type == type) items.Add(each);

            return items;
        }

        public decimal GetWeightSum()
        {
            decimal sum = 0;
            foreach (decimal each in _weights.Values)
                sum += each;
            return sum;
        }

        #region IEnumerable<ScoreItem> 成員

        public IEnumerator<ScoreItem> GetEnumerator()
        {
            return _weights.Keys.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成員

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _weights.Keys.GetEnumerator();
        }

        #endregion
    }
}
