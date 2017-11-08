using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.RatingFramework
{
    /// <summary>
    /// 代表名次資料的集合。
    /// </summary>
    public class PlaceCollection : IEnumerable<Place>
    {
        private Dictionary<string, Place> _places = new Dictionary<string, Place>();
        private Dictionary<string, PlaceCollection> _ns_places = new Dictionary<string, PlaceCollection>();

        public PlaceCollection()
        {
        }

        /// <summary>
        /// 取得指定排名的名次。
        /// </summary>
        /// <param name="name">排名的名稱。</param>
        /// <returns>名次資料。</returns>
        public Place this[string name]
        {
            get { return _places[name]; }
            set { _places[name] = value; }
        }

        /// <summary>
        /// 取得特定 Namespace 的排名資料。
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public PlaceCollection NS(string ns)
        {
            if (!_ns_places.ContainsKey(ns))
                _ns_places.Add(ns, new PlaceCollection());

            return _ns_places[ns];
        }

        /// <summary>
        /// 新增排名資料。
        /// </summary>
        /// <param name="name">排名名稱。</param>
        /// <param name="place">排名資料。</param>
        public void Add(string name, Place place)
        {
            _places.Add(name, place);
        }

        /// <summary>
        /// 移除指定的排名資料。
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            _places.Remove(name);
        }

        public void Clear()
        {
            _places.Clear();
        }

        /// <summary>
        /// 檢查是否包含指定排名。
        /// </summary>
        /// <param name="name">排名名稱。</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return _places.ContainsKey(name);
        }

        /// <summary>
        /// 取得排名資料的計數。
        /// </summary>
        public int Count { get { return _places.Count; } }

        #region IEnumerable<Place> 成員

        public IEnumerator<Place> GetEnumerator()
        {
            return _places.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成員

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _places.Values.GetEnumerator();
        }

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, Place> each in _places)
            {
                sb.AppendLine(string.Format("{0}:{1}({2})", each.Key, each.Value.Level, each.Value.Score));
            }

            return sb.ToString();
        }
    }
}
