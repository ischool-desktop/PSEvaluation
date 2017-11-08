using System;
using System.Collections.Generic;
using System.Text;

namespace JHEvaluation.ScoreCalculation
{
    internal class UniqueSet<T>
    {
        private Dictionary<T, T> _set = null;

        public UniqueSet()
        {
            _set = new Dictionary<T, T>();
        }

        public UniqueSet(IEnumerable<T> data)
            : this()
        {
            foreach (T each in data)
                Add(each);
        }

        public void Add(T item)
        {
            _set.Add(item, default(T));
        }

        public bool Contains(T item)
        {
            return _set.ContainsKey(item);
        }

        public void Remove(T item)
        {
            _set.Remove(item);
        }

        public void Clear()
        {
            _set.Clear();
        }

        public int Count { get { return _set.Count; } }

        public List<T> ToList()
        {
            return new List<T>(_set.Keys);
        }
    }
}
