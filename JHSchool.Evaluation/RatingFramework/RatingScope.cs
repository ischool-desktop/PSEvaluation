using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Evaluation.RatingFramework
{
    /// <summary>
    /// 代表排名範圍。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RatingScope<T> : IEnumerable<T> where T : IStudent
    {
        private Dictionary<string, T> _students = new Dictionary<string, T>();
        private List<T> _ordered_students = new List<T>();

        /// <summary>
        /// 建構式。
        /// </summary>
        /// <param name="name">名次範圍名稱，例如：101、102、一年級...</param>
        public RatingScope(string name)
        {
            Name = name;
            PlaceNamespace = string.Empty;
        }

        /// <summary>
        /// 建構式。
        /// </summary>
        /// <param name="name">名次範圍名稱，例如：101、102、一年級...</param>
        /// <param name="placeNS">名次分類名稱，例如：班排名、年排名。</param>
        public RatingScope(string name, string placeNS)
            : this(name)
        {
            PlaceNamespace = placeNS;
        }

        /// <summary>
        /// 名次範圍名稱，例如：101、102、一年級...
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 名次分類名稱，例如：班排名、年排名。
        /// </summary>
        public string PlaceNamespace { get; private set; }

        /// <summary>
        /// 進行排名。
        /// </summary>
        /// <param name="scoreParser">要被排名的成績計算邏輯。</param>
        /// <param name="option">排名選項，接序 or 不接序排名。</param>
        /// <param name="placeNamespace">存放排名結果的 Namespace。</param>
        public void Rank(IScoreParser<T> scoreParser, PlaceOptions option)
        {
            //取得有成績的學生。
            List<ScoredStudent> students = GetScoredStudents(scoreParser);

            //排序名次。
            students.Sort(delegate(ScoredStudent x, ScoredStudent y)
            {
                decimal X = x.Score;
                decimal Y = y.Score;

                //反過來排，由大到小。
                return Y.CompareTo(X);
            });

            //決定排序。
            IPlaceAlgorithm algorithm = null;
            if (option == PlaceOptions.Sequence)
                algorithm = new Sequence();
            else
                algorithm = new Unsequence();

            int radix = students.Count;
            string scorename = scoreParser.Name;
            foreach (ScoredStudent each in students)
            {
                int level = algorithm.NextLevel(each.Score);

                T student = _students[each.Id];

                PlaceCollection places = GetPlaceCollection(student);

                if (places.Contains(scorename))
                    places[scorename] = new Place(level, each.Score, radix);
                else
                    places.Add(scorename, new Place(level, each.Score, radix));
            }
        }

        /// <summary>
        /// 取得前 n 名次的學生。
        /// </summary>
        /// <param name="ratingName">排名的名稱，例如：國文。(與IScoreParser 實作之實體 Name 屬性值相同)</param>
        /// <param name="top">要取得名次。</param>
        /// <param name="placeNamespace">名次存放的 Namespace。</param>
        public RatingScope<T> GetTopPlaces(string ratingName, int top)
        {
            RatingScope<T> result = new RatingScope<T>(Name);
            foreach (T each in _students.Values)
            {
                PlaceCollection places = GetPlaceCollection(each);

                if (places.Contains(ratingName))
                {
                    if (places[ratingName].Level <= top)
                        result.Add(each);
                }
            }
            return result;
        }

        public RatingScope<T> GetTopPercentage(string ratingName, int top)
        {
            RatingScope<T> result = new RatingScope<T>(Name);
            RatingScope<T> radixStuds = new RatingScope<T>(Name);
            decimal filterPercentage = top;

            //把有那項排名的學生獨立出來。
            foreach (T each in _students.Values)
            {
                PlaceCollection places = GetPlaceCollection(each);

                if (places.Contains(ratingName))
                    radixStuds.Add(each);
            }
            //計算要取的 Percentage。
            decimal extractPlace = Math.Ceiling((filterPercentage / 100m) * radixStuds.Count);

            foreach (T each in radixStuds)
            {
                PlaceCollection places = GetPlaceCollection(each);

                if (places[ratingName].Level <= extractPlace)
                    result.Add(each);
            }

            //之前的取百分比名次做法。
            //foreach (RatingStudent each in students)
            //{
            //    if (each.Places.Contains(ratingName))
            //    {
            //        Place p = each.Places[ratingName];
            //        decimal percentage = (100m * ((decimal)p.Level / (decimal)p.Radix));
            //        if (percentage <= 0) percentage = 1;
            //        if (percentage <= filterPercentage)
            //        {
            //            result.Add(each);
            //        }
            //    }
            //}

            return result;
        }

        private PlaceCollection GetPlaceCollection(T each)
        {
            PlaceCollection places = null;
            if (string.IsNullOrEmpty(PlaceNamespace))
                places = each.Places;
            else
                places = each.Places.NS(PlaceNamespace);
            return places;
        }

        public void Sort(Comparison<T> comparison)
        {
            _ordered_students.Sort(comparison);
        }

        public void Sort(IComparer<T> comparer)
        {
            _ordered_students.Sort(comparer);
        }

        public T this[string Id]
        {
            get { return _students[Id]; }
        }

        public void Add(T student)
        {
            _students.Add(student.Id, student);
            _ordered_students.Add(student);
        }

        public bool Contains(string Id)
        {
            return _students.ContainsKey(Id);
        }

        public void Remove(string Id)
        {
            _students.Remove(Id);
            _ordered_students.Remove(_students[Id]);
        }

        public int Count { get { return _students.Count; } }

        #region IEnumerable<T> 成員

        public IEnumerator<T> GetEnumerator()
        {
            return _ordered_students.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成員

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _ordered_students.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// 取得有成績的學生清單，並且轉型成 StudentInternal。
        /// </summary>
        /// <returns></returns>
        /// <remarks>如果該學生沒有成績，就不會列入排名。</remarks>
        private List<ScoredStudent> GetScoredStudents(IScoreParser<T> scoreParser)
        {
            List<ScoredStudent> students = new List<ScoredStudent>();
            foreach (T each in this)
            {
                decimal? score = scoreParser.GetScore(each);

                if (score.HasValue)
                    students.Add(new ScoredStudent(each.Id, score.Value));
            }

            return students;
        }

        /// <summary>
        /// 代表有指定成績的學生。
        /// </summary>
        private struct ScoredStudent
        {
            public ScoredStudent(string id, decimal score)
                : this()
            {
                Id = id;
                Score = score;
            }

            public string Id { get; private set; }

            public decimal Score { get; private set; }
        }

        #region 接序與不接序排名法。

        interface IPlaceAlgorithm
        {
            int NextLevel(decimal score);
        }

        /// <summary>
        /// 不接序排名法。
        /// </summary>
        private class Unsequence : IPlaceAlgorithm
        {
            decimal _previous_score;
            int _previous_place;
            int _sequence;

            public Unsequence()
            {
                _previous_score = int.MaxValue;
                _previous_place = 0;
                _sequence = 0;
            }

            public int NextLevel(decimal score)
            {
                bool nextRequired = IsNextRequired(score);
                int place;

                if (nextRequired)
                {
                    place = ++_sequence;
                    _previous_place = _sequence;
                }
                else
                {
                    place = _previous_place;
                    ++_sequence;
                }

                _previous_score = score;
                return place;
            }

            private bool IsNextRequired(decimal score)
            {
                //小於 0 score < _previous_score
                return score.CompareTo(_previous_score) < 0;
            }
        }

        /// <summary>
        /// 接序排名法。
        /// </summary>
        private class Sequence : IPlaceAlgorithm
        {
            decimal _previous_score;
            int _previous_place;

            public Sequence()
            {
                _previous_score = int.MaxValue;
                _previous_place = 0;
            }

            public int NextLevel(decimal score)
            {
                bool nextRequired = IsNextRequired(score);
                int place;

                if (nextRequired)
                    place = ++_previous_place;
                else
                    place = _previous_place;

                _previous_score = score;
                return place;
            }

            private bool IsNextRequired(decimal score)
            {
                //小於 0 score < _previous_score
                return score.CompareTo(_previous_score) < 0;
            }
        }

        #endregion
    }
}
