using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;

namespace Campus
{
    /// <summary>
    /// 負載
    /// </summary>
    public enum MultiThreadLoading : int { Heavy = 4, Normal = 3, Light = 2 }
    /// <summary>
    /// 自動分包多續執行的BackgroundWorker
    /// </summary>
    public class MultiThreadBackgroundWorker<T> : BackgroundWorker
    {
        private bool _AutoReportsProgress = true;
        /// <summary>
        /// 是否自動依完成的封包數與總封包數計算及回報進度
        /// </summary>
        public bool AutoReportsProgress { get { return _AutoReportsProgress; } set { _AutoReportsProgress = value; this.WorkerReportsProgress |= value; } }

        private MultiThreadLoading _Loading = MultiThreadLoading.Normal;
        /// <summary>
        /// 負載
        /// </summary>
        public MultiThreadLoading Loading { get { return _Loading; } set { _Loading = value; } }

        private int _PackageSize = 250;
        /// <summary>
        /// 分割時每個封包的大小
        /// </summary>
        public int PackageSize { get { return _PackageSize; } set { _PackageSize = value; } }

        private IEnumerable<T> _List = null;
        /// <summary>
        /// 
        /// </summary>
        public MultiThreadBackgroundWorker()
        {
            this.WorkerReportsProgress = true;
            base.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                IEnumerable<T> list = _List;
                int packageSize = _PackageSize;
                int maxThreads=(int)_Loading;
                #region 切封包執行
                List<PackageDoWorkEventArgs<T>>[] packages = new List<PackageDoWorkEventArgs<T>>[maxThreads];
                for ( int i = 0 ; i < packages.Length ; i++ )
                {
                    packages[i] = new List<PackageDoWorkEventArgs<T>>();
                }
                List<T> package = new List<T>(packageSize);
                List<PackageDoWorkEventArgs<T>> allPackages = new List<PackageDoWorkEventArgs<T>>();
                int packagecount = packageSize;
                int p = 0;
                if ( list != null )
                {
                    foreach ( T var in list )
                    {
                        if ( packagecount == 0 )
                        {
                            PackageDoWorkEventArgs<T> pw = new PackageDoWorkEventArgs<T>(package, e.Argument);
                            allPackages.Add(pw);
                            packages[p % maxThreads].Add(pw);
                            p++;
                            packagecount = packageSize;
                            package = new List<T>(packageSize);
                        }
                        package.Add(var);
                        packagecount--;
                    }
                    if ( package.Count > 0 )
                    {
                        PackageDoWorkEventArgs<T> pw = new PackageDoWorkEventArgs<T>(package, e.Argument);
                        allPackages.Add(pw);
                        packages[p % maxThreads].Add(pw);
                        p++;
                    }
                }
                #region 開多個執行緒跑
                List<Thread> otherThreads = new List<Thread>();
                Progress progress = new Progress();
                progress.PackageProgress = 100m / ( p == 0 ? 1 : p );
                for ( int i = 1 ; i < maxThreads&&i<p ; i++ )
                {
                    if ( packages[i].Count > 0 )
                    {
                        Thread backThread = new Thread(new ParameterizedThreadStart(doWork));
                        backThread.IsBackground = true;
                        backThread.Start(new object[] { packages[i], progress });
                        otherThreads.Add(backThread);
                    }
                }
                if ( packages[0].Count > 0 )
                    doWork(new object[] { packages[0], progress });//packages[0]);
                foreach ( Thread thread in otherThreads )
                {
                    thread.Join();
                }
                #endregion
                foreach ( PackageDoWorkEventArgs<T> var in allPackages )
                {
                    if ( var.Exception != null )
                        throw var.Exception;
                }
                #endregion
            };
        }

        private void doWork(object obj)
        {
            List<PackageDoWorkEventArgs<T>> packages = (List<PackageDoWorkEventArgs<T>>)( ( (object[])obj )[0] );
            Progress progress = (Progress)( ( (object[])obj )[1] );
            foreach ( PackageDoWorkEventArgs<T> package in packages )
            {
                try
                {
                    if ( DoWork != null )
                        DoWork.Invoke(this, package);
                progress.TotleProgress += progress.PackageProgress;
                if ( this.WorkerReportsProgress && this.AutoReportsProgress )
                    this.ReportProgress((int)progress.TotleProgress);
                }
                catch ( Exception ex )
                {
                    package.Exception = ex;
                }
            }
        }
        /// <summary>
        /// 當任何一個封包被處理時發生 
        /// </summary>
        public new event EventHandler<PackageDoWorkEventArgs<T>> DoWork;

        #region RunWorkerAsync
        /// <summary>
        /// 開始執行背景作業。 
        /// </summary>
        public new void RunWorkerAsync()
        {
            _List = null;
            base.RunWorkerAsync();
        }
        /// <summary>
        /// 開始執行背景作業。 
        /// </summary>
        public new void RunWorkerAsync(object argument)
        {
            _List = null;
            base.RunWorkerAsync(argument);
        }
        /// <summary>
        /// 開始執行背景作業。 
        /// </summary>
        public void RunWorkerAsync(IEnumerable<T> list)
        {
            _List = list;
            base.RunWorkerAsync();
            //_List = null;
        }
        /// <summary>
        /// 開始執行背景作業。 
        /// </summary>
        public void RunWorkerAsync(IEnumerable<T> list, object argument)
        {
            _List = list;
            base.RunWorkerAsync(argument);
            //_List = null;
        } 
        #endregion
    }
    internal class Progress
    {
        private decimal _TotleProgress=0;
        internal decimal TotleProgress
        {
            get { return _TotleProgress; }
            set { _TotleProgress = value; }
        }
        private decimal _PackageProgress;
        internal decimal PackageProgress
        {
            get { return _PackageProgress; }
            set { _PackageProgress = value; }
        }
    }
    /// <summary>
    /// 提供 DoWork 事件處理常式的資料。 並提供此封包的內容。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PackageDoWorkEventArgs<T> : DoWorkEventArgs
    {
        private ReadOnlyCollection<T> _Items;
        /// <summary>
        /// 封包的內容
        /// </summary>
        public ReadOnlyCollection<T> Items { get { return _Items; } }
        internal PackageDoWorkEventArgs(IEnumerable<T> list, object arg)
            : base(arg)
        {
            List<T> l = new List<T>();
            l.AddRange(list);
            _Items = l.AsReadOnly();
        }
        internal PackageDoWorkEventArgs(object arg)
            : base(arg)
        {
            _Items = new List<T>(0).AsReadOnly();
        }
        private Exception _Exception=null;
        internal Exception Exception
        {
            get { return _Exception; }
            set { _Exception = value; }
        }
    }
}
