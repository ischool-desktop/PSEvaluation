using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Campus.Rating
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
        /// <param name="provider">當相同名次時決定先後成績資料，所有排名範圍內的學生都應該供相同順序與數量的成績資料，否則會產生無法預期的結果。</param>
        public void Rank(IScoreParser<T> scoreParser, PlaceOptions option)
        {
            //取得有成績的學生。
            List<ScoredStudent> students = GetScoredStudents(scoreParser);

            //沒有成績就不用排名了。
            if (students.Count <= 0) return;

            //排序名次。
            students.Sort(delegate(ScoredStudent x, ScoredStudent y)
            {
                if (scoreParser is IScoresParser<T>)
                {
                    IScoresParser<T> p2 = scoreParser as IScoresParser<T>;

                    if (x.SecondScores.Count <= 0)
                        x.SecondScores = p2.GetSecondScores(x.Source);
                    if (y.SecondScores.Count <= 0)
                        y.SecondScores = p2.GetSecondScores(y.Source);
                }
                return y.CompareTo(x);
            });

            int radix = students.Count; //基數。
            string scorename = scoreParser.Name; //排名的名稱。

            //先把之前的排名結果清掉。
            foreach (T each in this)
            {
                PlaceCollection places = GetPlaceCollection(each);
                if (places.Contains(scorename))
                    places.Remove(scorename);
            }

            //決定排序。
            IPlaceAlgorithm placeAlg = null;
            if (option == PlaceOptions.Sequence)
                placeAlg = new Sequence();
            else
                placeAlg = new Unsequence();

            StandardPercentage percenAlg = new StandardPercentage();
            percenAlg.Initital(students.Count);

            foreach (ScoredStudent each in students)
            {
                int level = placeAlg.NextLevel(each);
                int percen = percenAlg.NextPercentage(level);

                T student = _students[each.Id];

                PlaceCollection places = GetPlaceCollection(student);

                if (places.Contains(scorename))
                    places[scorename] = new Place(level, percen, each.Score, radix, this.Count);
                else
                    places.Add(scorename, new Place(level, percen, each.Score, radix, this.Count));
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

        /// <summary>
        /// 取得後 n 名次的學生。
        /// </summary>
        /// <param name="ratingName">排名的名稱，例如：國文。(與IScoreParser 實作之實體 Name 屬性值相同)</param>
        /// <param name="last">要取得名次。</param>
        public RatingScope<T> GetLastPlaces(string scoreName, int last)
        {
            RatingScope<T> result = new RatingScope<T>(Name);

            List<T> students = new List<T>();

            foreach (T each in _students.Values)
            {
                PlaceCollection places = GetPlaceCollection(each);

                if (places.Contains(scoreName))
                    students.Add(each);
            }

            students.Sort((x, y) =>
            {
                PlaceCollection placesX = GetPlaceCollection(x);
                PlaceCollection placesY = GetPlaceCollection(y);

                return placesY[scoreName].Level.CompareTo(placesX[scoreName].Level);
            });

            if (students.Count <= 0)
                return result;

            //取得最後一名的名次。
            int lastLevel = students[0].Places[scoreName].Level - last;

            foreach (T student in students.Where(x => x.Places[scoreName].Level > lastLevel))
                result.Add(student);

            return result;
        }

        /// <summary>
        /// 使用舊的方式取得前百分之的學生。
        /// </summary>
        /// <param name="scoreName"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        [Obsolete("有新方法取代，請參考 IPercentageAlgorithm 介面。")]
        public RatingScope<T> GetTopPercentage(string scoreName, int top)
        {
            RatingScope<T> result = new RatingScope<T>(Name);
            RatingScope<T> radixStuds = new RatingScope<T>(Name);
            decimal filterPercentage = top;

            //把有那項排名的學生獨立出來。
            foreach (T each in _students.Values)
            {
                PlaceCollection places = GetPlaceCollection(each);

                if (places.Contains(scoreName))
                    radixStuds.Add(each);
            }
            //計算要取的 Percentage。
            decimal extractPlace = Math.Ceiling((filterPercentage / 100m) * radixStuds.Count);

            foreach (T each in radixStuds)
            {
                PlaceCollection places = GetPlaceCollection(each);

                if (places[scoreName].Level <= extractPlace)
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

        /// <summary>
        /// 使用舊的方式取得後百分之的學生。
        /// </summary>
        /// <param name="scoreName"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        [Obsolete("有新方法取代，請參考 IPercentageAlgorithm 介面。")]
        public RatingScope<T> GetLastPercentage(string scoreName, int last)
        {
            RatingScope<T> result = new RatingScope<T>(Name);
            //RatingScope<T> radixStuds = new RatingScope<T>(Name);
            decimal filterPercentage = last;
            List<T> students = new List<T>();

            //把有那項排名的學生獨立出來。
            foreach (T each in _students.Values)
            {
                PlaceCollection places = GetPlaceCollection(each);

                if (places.Contains(scoreName))
                    students.Add(each);
            }
            //計算要取的 Percentage。
            decimal extractPlace = Math.Ceiling((filterPercentage / 100m) * students.Count);
            
            students.Sort((x, y) =>
            {
                PlaceCollection placesX = GetPlaceCollection(x);
                PlaceCollection placesY = GetPlaceCollection(y);

                return placesY[scoreName].Level.CompareTo(placesX[scoreName].Level);
            });

            if (students.Count <= 0)
                return result;

            //取得最後一名的名次百分比。
            decimal  lastLevel = students[0].Places[scoreName].Level - extractPlace;

            foreach (T student in students.Where(x => x.Places[scoreName].Level > lastLevel))
                result.Add(student);

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
        /// 取得有成績的學生清單，並且轉型成 ScoredStudent。
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
                    students.Add(new ScoredStudent(score.Value, each));
            }

            return students;
        }

        /// <summary>
        /// 代表有指定成績的學生。
        /// </summary>
        public class ScoredStudent : IComparable<ScoredStudent>
        {
            public ScoredStudent(decimal score, T scoreSource)
            {
                Id = (scoreSource == null) ? string.Empty : scoreSource.Id;
                Score = score;
                Source = scoreSource;
                SecondScores = new List<decimal>();
            }

            /// <summary>
            /// 相關聯的學生編號。
            /// </summary>
            public string Id { get; private set; }

            /// <summary>
            /// 要排名的成績。
            /// </summary>
            public decimal Score { get; private set; }

            /// <summary>
            /// 同名時，第二排序成績清單。
            /// </summary>
            internal List<decimal> SecondScores { get; set; }

            /// <summary>
            /// 排名成績的來源。
            /// </summary>
            public T Source { get; private set; }

            #region IComparable<ScoredStudent> 成員

            public int CompareTo(ScoredStudent other)
            {
                if (other == null) return 1; //我一定比 Null 大。

                int order = Score.CompareTo(other.Score);

                if (order != 0) //如果比出大小時，就回傳結果。
                    return order;
                else //比不出高底就...再比其他的。
                {
                    if (other.SecondScores.Count != SecondScores.Count)
                        throw new ArgumentException("提供的成績數量不同，無法比較。");

                    for (int idx = 0; idx < SecondScores.Count; idx++)
                    {
                        decimal x = SecondScores[idx];
                        decimal y = other.SecondScores[idx];

                        order = x.CompareTo(y);

                        if (order != 0) return order; //比出大小就回傳結果。
                    }
                    return 0; //比不出大小。
                }
            }

            #endregion
        }

        #region 接序與不接序排名法。

        interface IPlaceAlgorithm
        {
            int NextLevel(ScoredStudent score);
        }

        /// <summary>
        /// 不接序排名法。
        /// </summary>
        private class Unsequence : IPlaceAlgorithm
        {
            ScoredStudent _previous_score;
            int _previous_place;
            int _sequence;

            public Unsequence()
            {
                _previous_score = new RatingScope<T>.ScoredStudent(decimal.MaxValue, default(T));
                _previous_place = 0;
                _sequence = 0;
            }

            public int NextLevel(ScoredStudent score)
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

            private bool IsNextRequired(ScoredStudent score)
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
            ScoredStudent _previous_score;
            int _previous_place;

            public Sequence()
            {
                _previous_score = new RatingScope<T>.ScoredStudent(decimal.MaxValue, default(T)); ;
                _previous_place = 0;
            }

            public int NextLevel(ScoredStudent score)
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

            private bool IsNextRequired(ScoredStudent score)
            {
                //小於 0 score < _previous_score
                return score.CompareTo(_previous_score) < 0;
            }
        }

        #endregion
    }
}
