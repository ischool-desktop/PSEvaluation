using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Collections;

namespace Campus
{
    /// <summary>
    /// 提供可被索引的強型別資料的快取管理
    /// </summary>
    /// <typeparam name="T">快取管理的型別</typeparam>
    public abstract class CacheManager<T>
    {
        /// <summary>
        /// 一次取得所有資料項目
        /// </summary>
        /// <returns>傳回索引鍵跟快取資料的查詢</returns>
        abstract protected Dictionary<string, T> GetAllData();

        /// <summary>
        /// 一次取得部份指定鍵值的資料。
        /// </summary>
        /// <param name="primaryKeys">要取得的鍵值</param>
        /// <returns>傳回索引鍵跟快取資料的查詢</returns>
        abstract protected Dictionary<string, T> GetData(IEnumerable<string> primaryKeys);

        /// <summary>
        /// 驗證輸入的鍵值是否合法，當要求查尋資料時若鍵值不合法則不進行查尋
        /// 預設驗證方法為是否可轉化為int
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <returns>是否合法</returns>
        protected virtual bool ValidateKey(string key)
        {
            int a;
            return int.TryParse(key, out a);
        }

        /// <summary>
        /// 取得指定索引的項目，若指定的鍵值不存在則會先嚐試進行查尋。Indexer。
        /// </summary>
        /// <param name="primaryKey">取得項目的鍵值</param>
        /// <returns>該鍵值的項目，若傳入鍵值沒有對應項目則傳回default(T)</returns>
        public T this[string primaryKey]
        {
            get { _Loading.WaitOne(); return Items[primaryKey]; }
        }

        private bool _Loaded = false;
        private bool _CanSort = false;
        private bool _CanGenericCompare = false;
        private ManualResetEvent _Loading = new ManualResetEvent(true);
        private BackgroundWorker _DataLoader = new BackgroundWorker();
        private Dictionary<string, T> _List = new Dictionary<string, T>();
        private CacheItemCollection _Items = null;
        private List<string> _RemovedKeys = new List<string>();
        private SortedList<string, int> _SortedOrder = new SortedList<string, int>();

        private void _DataLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _List.Clear();
                Dictionary<string, T> list = GetAllData();
                _SortedOrder.Clear();
                int index = 0;
                if ( _CanSort )
                {
                    Dictionary<T, string> items = new Dictionary<T, string>();
                    List<T> sorter = new List<T>();
                    foreach ( var key in list.Keys )
                    {
                        items.Add(list[key], key);
                        sorter.Add(list[key]);
                    }
                    sorter.Sort();
                    foreach ( var item in sorter )
                    {
                        _List.Add(items[item], item);
                        _SortedOrder.Add(items[item], index++);
                    }
                }

                else
                {
                    foreach ( var key in list.Keys )
                    {
                        _List.Add(key, list[key]);
                        _SortedOrder.Add(key, index++);
                    }
                }
            }
            catch ( Exception ex )
            {
                _Loading.Set();
                throw ex;
            }
            _Loaded = true;
            _Loading.Set();
        }
        private void _DataLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ( e.Error != null )
                throw e.Error;
            OnItemLoaded(new EventArgs());
        }
        /// <summary>
        /// 建構子
        /// </summary>
        public CacheManager()
        {
            foreach ( var type in typeof(T).GetInterfaces() )
            {
                if ( type == typeof(IComparable<T>) )
                {
                    _CanSort = true;
                    _CanGenericCompare = true;
                }
                if ( type == typeof(IComparable) )
                {
                    _CanSort = true;
                }
            }
            _DataLoader.DoWork += new DoWorkEventHandler(_DataLoader_DoWork);
            _DataLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_DataLoader_RunWorkerCompleted);
        }
        /// <summary>
        /// 取得管理項目的集合
        /// </summary>
        public CacheItemCollection Items
        {
            get
            {
                //if ( !_Loaded )
                //    ReflashAll();
                _Loading.WaitOne();
                if ( _Items == null )
                {
                    _Items = new CacheItemCollection(this, _List);
                }
                return _Items;
            }
        }
        /// <summary>
        /// 取得所有資料，此方法將於背景執行續進行，並於完成後引發ItemLoaded事件
        /// </summary>
        public void SyncAllBackground()
        {
            _Loaded = false;
            //重新取得所有資料
            if ( !_DataLoader.IsBusy )
            {
                _Loading.WaitOne();
                _Loading.Reset();
                _RemovedKeys.Clear();//清空已刪除清單
                _DataLoader.RunWorkerAsync();
            }
        }
        /// <summary>
        /// 更新快取資料，更新後將會引發ItemUpdated事件
        /// </summary>
        /// <param name="primaryKeys">要更新資料的鍵值</param>
        public void SyncData(params string[] primaryKeys)
        {
            SyncData((IEnumerable<string>)primaryKeys);
        }
        /// <summary>
        /// 更新快取資料，更新後將會引發ItemUpdated事件
        /// </summary>
        /// <param name="primaryKeys">要更新資料的鍵值</param>
        public void SyncData(IEnumerable<string> primaryKeys)
        {
            _Loading.WaitOne();
            _Loading.Reset();
            List<string> pKeys = new List<string>();
            foreach ( var key in primaryKeys )
            {
                if ( !pKeys.Contains(key) )
                    pKeys.Add(key);
            }
            List<string> removeKeys = new List<string>();
            List<string> updatedKeys = new List<string>(pKeys);
            #region 更新快取
            try
            {
                Dictionary<string, T> list = GetData(pKeys);
                Dictionary<T, string> items = new Dictionary<T, string>();
                pKeys.Sort();
                List<T> sortedList = new List<T>();
                List<T> updatedList = new List<T>();
                List<string> insertKeys = new List<string>(pKeys);
                foreach ( var key in _List.Keys )
                {
                    T item;
                    if ( pKeys.BinarySearch(key) >= 0 )
                    {
                        if ( list.ContainsKey(key) )
                        {
                            item = list[key];
                            updatedList.Add(item);
                        }
                        else
                        {
                            removeKeys.Add(key);
                            continue;
                        }
                        insertKeys.Remove(key);
                    }
                    else
                    {
                        item = _List[key];
                        sortedList.Add(item);
                    }
                    items.Add(item, key);
                }
                foreach ( var remove in removeKeys )
                {
                    _List.Remove(remove);
                }
                foreach ( var key in insertKeys )
                {
                    T item;
                    if ( list.ContainsKey(key) )
                    {
                        item = list[key];
                        updatedList.Add(item);
                        items.Add(item, key);
                    }
                    else
                    {
                        //沒有快取也沒有抓到資料的ID(從來就不存在這筆資料)就移出更新清單
                        updatedKeys.Remove(key);
                        continue;
                    }
                }
                if ( updatedList.Count > 0 )
                {
                    if ( _CanSort )
                    {
                        //因為List原本的排序就是對的，所以將原本資料沒有變更的部份加入sortedList後sortedList的排序也是對的，這時insert只要用BinarySearch找到要差入的Index即可保持排序
                        foreach ( var item in updatedList )
                        {
                            int index = 0;
                            int length = sortedList.Count;
                            switch ( length )
                            {
                                case 0:

                                    break;
                                case 1:
                                    if ( _CanGenericCompare )
                                    {
                                        if ( ( (IComparable<T>)item ).CompareTo(sortedList[0]) >= 0 )
                                            index++;
                                    }
                                    else
                                    {
                                        if ( ( (IComparable)item ).CompareTo(sortedList[0]) >= 0 )
                                            index++;
                                    }
                                    break;
                                default:
                                    while ( true )
                                    {
                                        int compare;
                                        if ( length <= 2 )
                                        {
                                            if ( _CanGenericCompare )
                                                compare = ( ( (IComparable<T>)item ).CompareTo(sortedList[index + 1]) );
                                            else
                                                compare = ( ( (IComparable)item ).CompareTo(sortedList[index + 1]) );
                                            if ( compare >= 0 )
                                            {
                                                index++;
                                            }
                                            break;
                                        }
                                        int helfLength = ( length >> 1 ) + ( length & 1 );
                                        if ( _CanGenericCompare )
                                            compare = ( ( (IComparable<T>)item ).CompareTo(sortedList[index + helfLength - 1]) );
                                        else
                                            compare = ( ( (IComparable)item ).CompareTo(sortedList[index + helfLength - 1]) );
                                        if ( compare >= 0 )
                                        {
                                            index += helfLength - 1;
                                        }
                                        length -= helfLength - 1;
                                    }
                                    if ( index == 0 )
                                    {
                                        if ( _CanGenericCompare )
                                        {
                                            if ( ( (IComparable<T>)item ).CompareTo(sortedList[0]) >= 0 )
                                                index++;
                                        }
                                        else
                                        {
                                            if ( ( (IComparable)item ).CompareTo(sortedList[0]) >= 0 )
                                                index++;
                                        }
                                    }
                                    else
                                        index++;
                                    break;
                            }
                            sortedList.Insert(index, item);
                        }
                    }
                    else
                        sortedList.AddRange(updatedList);
                    _List.Clear();
                    lock ( _SortedOrder )
                    {
                        _SortedOrder.Clear();
                        int sortedIndex = 0;
                        foreach ( var item in sortedList )
                        {
                            _List.Add(items[item], item);
                            _SortedOrder.Add(items[item], sortedIndex++);
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                _Loading.Set();
                throw ex;
            }
            #endregion
            _Loading.Set();
            _RemovedKeys.AddRange(removeKeys);
            updatedKeys.AddRange(removeKeys);
            if ( updatedKeys.Count > 0 )
            {
                OnItemUpdated(new ItemUpdatedEventArgs(updatedKeys));
            }
        }
        /// <summary>
        /// 更新快取資料，此方法將於背景執行續進行，並於完成後引發ItemUpdated事件
        /// </summary>
        /// <param name="primaryKeys">要更新資料的鍵值</param>
        public void SyncDataBackground(params string[] primaryKeys)
        {
            SyncDataBackground((IEnumerable<string>)primaryKeys);
        }
        /// <summary>
        /// 更新快取資料，此方法將於背景執行續進行，並於完成後引發ItemUpdated事件
        /// </summary>
        /// <param name="primaryKeys">要更新資料的鍵值</param>
        public void SyncDataBackground(IEnumerable<string> primaryKeys)
        {
            _Loading.WaitOne();
            _Loading.Reset();
            List<string> pKeys = new List<string>();
            foreach ( var key in primaryKeys )
            {
                if ( !pKeys.Contains(key) )
                    pKeys.Add(key);
            }
            #region 如果要求的Key都是已經被Removed就不處理
            if ( _RemovedKeys.Count != 0 )
            {
                bool removed = true;
                foreach ( var item in pKeys )
                {
                    if ( !_RemovedKeys.Contains(item) )
                    {
                        removed = false;
                        break;
                    }
                }
                if ( removed )
                    return;
            }
            #endregion
            List<string> removeKeys = new List<string>();
            List<string> updatedKeys = new List<string>(pKeys);
            BackgroundWorker bkw = new BackgroundWorker();
            bkw.DoWork += delegate
            {
                try
                {
                    #region 更新快取
                    Dictionary<string, T> list = GetData(pKeys);  //取得指定的primary keys 的資料物件
                    Dictionary<T, string> items = new Dictionary<T, string>();
                    pKeys.Sort();
                    List<T> sortedList = new List<T>();
                    List<T> updatedList = new List<T>();
                    List<string> insertKeys = new List<string>(pKeys);
                    foreach ( var key in _List.Keys )   //對於目前Cache Manager 中所存的所有物件的 primary Key
                    {
                        T item;
                        if ( pKeys.BinarySearch(key) >= 0 )
                        {
                            if ( list.ContainsKey(key) )    //如果取得的資料物件中含有這個key，表示是更新
                            {
                                item = list[key];
                                updatedList.Add(item);
                            }
                            else
                            {
                                removeKeys.Add(key);        //否則就是被刪掉了
                                continue;
                            }
                            insertKeys.Remove(key);
                        }
                        else
                        {
                            item = _List[key];
                            sortedList.Add(item);
                        }
                        items.Add(item, key);
                    }
                    foreach ( var remove in removeKeys )    //從Cache Manager 的所含的資料物件中清掉已被從資料庫中刪除的物件。
                    {
                        _List.Remove(remove);
                    }
                    foreach ( var key in insertKeys )
                    {
                        T item;
                        if ( list.ContainsKey(key) )
                        {
                            item = list[key];
                            updatedList.Add(item);
                            items.Add(item, key);
                        }
                        else
                        {
                            //沒有快取也沒有抓到資料的ID(從來就不存在這筆資料)就移出更新清單
                            updatedKeys.Remove(key);
                            continue;
                        }
                    }
                    if ( updatedList.Count > 0 )
                    {
                        if ( _CanSort )
                        {
                            //因為List原本的排序就是對的，所以將原本資料沒有變更的部份加入sortedList後sortedList的排序也是對的，這時insert只要用BinarySearch找到要差入的Index即可保持排序
                            foreach ( var item in updatedList )
                            {
                                int index = 0;
                                int length = sortedList.Count;
                                switch ( length )
                                {
                                    case 0:

                                        break;
                                    case 1:
                                        if ( _CanGenericCompare )
                                        {
                                            if ( ( (IComparable<T>)item ).CompareTo(sortedList[0]) >= 0 )
                                                index++;
                                        }
                                        else
                                        {
                                            if ( ( (IComparable)item ).CompareTo(sortedList[0]) >= 0 )
                                                index++;
                                        }
                                        break;
                                    default:
                                        while ( true )
                                        {
                                            int compare;
                                            if ( length <= 2 )
                                            {
                                                if ( _CanGenericCompare )
                                                    compare = ( ( (IComparable<T>)item ).CompareTo(sortedList[index + 1]) );
                                                else
                                                    compare = ( ( (IComparable)item ).CompareTo(sortedList[index + 1]) );
                                                if ( compare >= 0 )
                                                {
                                                    index++;
                                                }
                                                break;
                                            }
                                            int helfLength = ( length >> 1 ) + ( length & 1 );
                                            if ( _CanGenericCompare )
                                                compare = ( ( (IComparable<T>)item ).CompareTo(sortedList[index + helfLength - 1]) );
                                            else
                                                compare = ( ( (IComparable)item ).CompareTo(sortedList[index + helfLength - 1]) );
                                            if ( compare >= 0 )
                                            {
                                                index += helfLength - 1;
                                            }
                                            length -= helfLength - 1;
                                        }
                                        if ( index == 0 )
                                        {
                                            if ( _CanGenericCompare )
                                            {
                                                if ( ( (IComparable<T>)item ).CompareTo(sortedList[0]) >= 0 )
                                                    index++;
                                            }
                                            else
                                            {
                                                if ( ( (IComparable)item ).CompareTo(sortedList[0]) >= 0 )
                                                    index++;
                                            }
                                        }
                                        else
                                            index++;
                                        break;
                                }
                                sortedList.Insert(index, item);
                            }
                        }
                        else
                            sortedList.AddRange(updatedList);
                        _List.Clear();
                        lock ( _SortedOrder )
                        {
                            _SortedOrder.Clear();
                            int sortedIndex = 0;
                            foreach ( var item in sortedList )
                            {
                                _List.Add(items[item], item);
                                _SortedOrder.Add(items[item], sortedIndex++);
                            }
                        }
                    }
                    _Loading.Set();
                    #endregion
                }
                catch
                {
                    _Loading.Set();
                }
            };
            bkw.RunWorkerCompleted += delegate
            {
                _RemovedKeys.AddRange(removeKeys);
                updatedKeys.AddRange(removeKeys);
                if ( updatedKeys.Count > 0 )
                {
                    OnItemUpdated(new ItemUpdatedEventArgs(updatedKeys));
                }
            };
            bkw.RunWorkerAsync();
        }
        /// <summary>
        /// 引發ItemUpdated事件。
        /// </summary>
        /// <param name="itemUpdatedEventArgs">包含事件資料的ItemUpdatedEventArgs</param>
        protected virtual void OnItemUpdated(ItemUpdatedEventArgs itemUpdatedEventArgs)
        {
            if (ItemUpdated != null)
                ItemUpdated(this, itemUpdatedEventArgs);

            //BigEvent be = new BigEvent("OnItemUpdated", ItemUpdated, this, itemUpdatedEventArgs);
            //be.UIRaise();
        }
        /// <summary>
        /// 引發ItemLoaded事件。
        /// </summary>
        /// <param name="eventArgs">包含事件資料的EventArgs</param>
        protected virtual void OnItemLoaded(EventArgs eventArgs)
        {
            if (ItemLoaded != null)
                ItemLoaded(this, eventArgs);
            //BigEvent be = new BigEvent("ItemLoaded", ItemLoaded, this, eventArgs);
            //be.UIRaise();
        }
        /// <summary>
        /// 重新排序快取資料，快取的資料型別若為IComparable則將自動進行排序
        /// 不需呼叫此方法也會維持順序，唯有當IComparable.CompareTo實作變更時使用此方法重新排序
        /// </summary>
        public void SortItems()
        {
            if ( _CanSort && !_DataLoader.IsBusy )
            {
                _Loading.WaitOne();
                _Loading.Reset();
                _SortedOrder.Clear();
                Dictionary<T, string> items = new Dictionary<T, string>();
                List<T> sorter = new List<T>();
                foreach ( var key in _List.Keys )
                {
                    items.Add(_List[key], key);
                    sorter.Add(_List[key]);
                }
                sorter.Sort();
                _List.Clear();
                int index = 0;
                foreach ( var item in sorter )
                {
                    _List.Add(items[item], item);
                    _SortedOrder.Add(items[item], index++);
                }
                _Loading.Set();
            }
        }
        /// <summary>
        /// 取得資料順序
        /// </summary>
        /// <param name="primaryKey1">第一個項目的鍵值</param>
        /// <param name="primaryKey2">第二個項目的鍵值</param>
        /// <returns>
        /// 小於零：primaryKey1的項目小於 primaryKey2的項目 
        /// 等於零：primaryKey1的項目等於 primaryKey2的項目 
        /// 大於零：primaryKey1的項目大於 primaryKey2的項目 
        /// </returns>
        public int QuickCompare(string primaryKey1, string primaryKey2)
        {
            var v1 = int.MinValue;
            var v2 = int.MinValue;
            _SortedOrder.TryGetValue(primaryKey1, out v1);
            _SortedOrder.TryGetValue(primaryKey2, out v2);
            return v1.CompareTo(v2);
        }
        /// <summary>
        /// 當SyncAllBackground完成時
        /// </summary>
        public event EventHandler ItemLoaded;
        /// <summary>
        /// 當快取資料變更時
        /// </summary>
        public event EventHandler<ItemUpdatedEventArgs> ItemUpdated;
        /// <summary>
        /// 取得是否已經載入(SyncAllBackground)
        /// </summary>
        public bool Loaded { get { return _Loaded; } }
        /// <summary>
        /// 快取資料的集合
        /// </summary>
        public class CacheItemCollection : IEnumerable<T>
        {
            private Dictionary<string, T> _Items;
            private CacheManager<T> _Parent;

            internal CacheItemCollection(CacheManager<T> parent, Dictionary<string, T> items)
            {
                if ( items == null )
                    throw new Exception("不得傳入null");
                _Parent = parent;
                _Items = items;
            }

            /// <summary>
            /// 取得指定索引的項目，若指定的鍵值不存在則會先嚐試進行查尋
            /// </summary>
            /// <param name="primaryKey">取得項目的鍵值</param>
            /// <returns>該鍵值的項目，若傳入鍵值沒有對應項目則傳回default(T)</returns>
            public T this[string ID]
            {
                get
                {
                    lock ( _Parent._SortedOrder )
                    {
                        if ( _Items.ContainsKey(ID) )
                            return _Items[ID];
                        else
                        {
                            if ( _Parent.ValidateKey(ID) )
                            {
                                _Parent.SyncData(ID);
                                if ( _Items.ContainsKey(ID) )
                                    return _Items[ID];
                                else
                                    return default(T);
                            }
                            else
                                return default(T);
                        }
                    }
                }
            }
            /// <summary>
            /// 查尋是否已快取此鍵值的項目
            /// </summary>
            /// <param name="key">要查尋的鍵值</param>
            /// <returns>是否已在快取中</returns>
            public bool ContainsKey(string key)
            {
                return _Items.ContainsKey(key);
            }
            /// <summary>
            /// 查尋是否快取此項目
            /// </summary>
            /// <param name="value">要查尋的項目</param>
            /// <returns>是否在快取資料中</returns>
            public bool ContainsValue(T value)
            {
                return _Items.ContainsValue(value);
            }
            /// <summary>
            /// 取得項目總數
            /// </summary>
            public int Count
            {
                get { return _Items.Count; }
            }
            /// <summary>
            /// 取得包含的值
            /// </summary>
            public Dictionary<string, T>.ValueCollection Values { get { return _Items.Values; } }
            /// <summary>
            /// 取得包含的索引鍵
            /// </summary>
            public Dictionary<string, T>.KeyCollection Keys { get { return _Items.Keys; } }

            #region IEnumerable<ListType> 成員

            public IEnumerator<T> GetEnumerator()
            {
                return ( (IEnumerable<T>)_Items.Values ).GetEnumerator();
            }

            #endregion

            #region IEnumerable 成員

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _Items.Values.GetEnumerator();
            }

            #endregion
        }
    }
    /// <summary>
    /// 提供 ItemUpdated事件的資料
    /// </summary>
    public class ItemUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// 取得被更新資料的鍵值清單
        /// </summary>
        public List<string> PrimaryKeys { get; private set; }
        internal ItemUpdatedEventArgs(IEnumerable<string> primaryKeys)
        {
            PrimaryKeys = new List<string>(primaryKeys);
        }
    }

}
