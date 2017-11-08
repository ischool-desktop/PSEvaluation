using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace JHEvaluation.ScoreCalculation
{
    internal class FunctionSpliter<IN, OUT>
    {
        private MultiThreadWorker<IN> Worker;
        private int totalCount = 0, progress = 0;

        private List<OUT> Result = new List<OUT>();

        public FunctionSpliter(int splitSize, int maxThreads)
        {
            Worker = new MultiThreadWorker<IN>();
            Worker.MaxThreads = maxThreads;
            Worker.PackageSize = splitSize;
            Worker.PackageWorker += new EventHandler<PackageWorkEventArgs<IN>>(Worker_PackageWorker);
        }

        private void Worker_PackageWorker(object sender, PackageWorkEventArgs<IN> e)
        {
            if (Function == null)
                throw new ArgumentException("請指定 RunPart 屬性。");

            List<OUT> result = Function(e.List);
            lock (Result)
            {
                progress += e.List.Count;
                Result.AddRange(result);
            }

            if (ProgressChange != null)
                ProgressChange(progress);
        }

        /// <summary>
        /// 執行方法，內部會以多執行緒執行，但是只有當所有執行緒完成時，才會回傳。
        /// </summary>
        /// <param name="parameters">參數。</param>
        /// <returns>執行結果。</returns>
        public List<OUT> Execute(List<IN> parameters)
        {
            totalCount = parameters.Count;
            Worker.Run(parameters);
            return Result;
        }

        /// <summary>
        /// 要分批執行的動作。
        /// </summary>
        public Func<List<IN>, List<OUT>> Function { get; set; }

        /// <summary>
        /// 分批執行進度，以每完成一個執行緒為單位回報。
        /// </summary>
        public Action<int> ProgressChange { get; set; }

        #region MultiThreadWorker,PackageWorkEventArgs<T>
        /// <summary>
        /// 切割封包並以多執行緒模式逐一處理各個封包
        /// </summary>
        private class MultiThreadWorker<T>
        {
            private int _PackageSize = 500;
            private int _MaxThreads = 2;
            private void doWork(object obj)
            {
                List<PackageWorkEventArgs<T>> packages = (List<PackageWorkEventArgs<T>>)obj;
                foreach (PackageWorkEventArgs<T> package in packages)
                {
                    try
                    {
                        if (PackageWorker != null)
                            PackageWorker.Invoke(this, package);
                    }
                    catch (Exception ex)
                    {
                        package.Exception = ex;
                        package.HasException = true;

                        //CurrentUser user = CurrentUser.Instance;
                        //SmartSchool.ExceptionHandler.BugReporter.ReportException("SmartSchool", user.SystemVersion, ex, false);
                    }
                }
            }
            /// <summary>
            /// 每個封包的最大容量
            /// </summary>
            public int PackageSize { get { return _PackageSize; } set { _PackageSize = value; } }
            /// <summary>
            /// 同時執行的最大執行緒數量
            /// </summary>
            public int MaxThreads { get { return _MaxThreads; } set { if (value <= 0)throw new Exception("最好是可以小魚0啦"); _MaxThreads = value; } }
            /// <summary>
            /// 處理單一封包
            /// </summary>
            public event EventHandler<PackageWorkEventArgs<T>> PackageWorker;
            /// <summary>
            /// 執行
            /// </summary>
            /// <param name="list">要處理的資料</param>
            /// <param name="argument">額外的參數</param>
            public List<PackageWorkEventArgs<T>> Run(IEnumerable<T> list, object argument)
            {
                #region 切封包執行
                List<PackageWorkEventArgs<T>>[] packages = new List<PackageWorkEventArgs<T>>[_MaxThreads];
                for (int i = 0; i < packages.Length; i++)
                {
                    packages[i] = new List<PackageWorkEventArgs<T>>();
                }
                List<T> package = null;
                int packagecount = 0;
                int p = 0;
                foreach (T var in list)
                {
                    if (packagecount == 0)
                    {
                        package = new List<T>(_PackageSize);
                        packagecount = _PackageSize;
                        PackageWorkEventArgs<T> pw = new PackageWorkEventArgs<T>();
                        pw.List = package;
                        pw.Argument = argument;
                        packages[p % _MaxThreads].Add(pw);
                        p++;
                    }
                    package.Add(var);
                    packagecount--;
                }
                #region 開多個執行緒跑
                List<Thread> otherThreads = new List<Thread>();
                for (int i = 1; i < _MaxThreads; i++)
                {
                    if (packages[i].Count > 0)
                    {
                        Thread backThread = new Thread(new ParameterizedThreadStart(doWork));
                        backThread.IsBackground = true;
                        backThread.Start(packages[i]);
                        otherThreads.Add(backThread);
                    }
                }
                if (packages[0].Count > 0)
                    doWork(packages[0]);
                foreach (Thread thread in otherThreads)
                {
                    thread.Join();
                }
                #endregion
                List<PackageWorkEventArgs<T>> result = new List<PackageWorkEventArgs<T>>();
                foreach (List<PackageWorkEventArgs<T>> var in packages)
                {
                    result.AddRange(var);
                }
                return result;
                #endregion
            }
            /// <summary>
            /// 執行
            /// </summary>
            /// <param name="list">要處理的資料</param>
            public List<PackageWorkEventArgs<T>> Run(IEnumerable<T> list)
            {
                return Run(list, null);
            }
        }
        /// <summary>
        /// 處理單一封包事件傳遞
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class PackageWorkEventArgs<T> : EventArgs
        {
            private bool _HasException = false;
            private Exception _Exception = null;
            private List<T> _List = new List<T>();
            private object _Result = null;
            private object _Argument = null;
            /// <summary>
            /// 是否發生錯誤
            /// </summary>
            public bool HasException { get { return _HasException; } set { _HasException = value; } }
            /// <summary>
            /// 發生錯誤時的錯誤內容
            /// </summary>
            public Exception Exception { get { return _Exception; } set { _Exception = value; } }
            /// <summary>
            /// 封包內容
            /// </summary>
            public List<T> List { get { return _List; } set { _List = value; } }
            /// <summary>
            /// 回傳
            /// </summary>
            public object Result { get { return _Result; } set { _Result = value; } }
            /// <summary>
            /// 額外的參數
            /// </summary>
            public object Argument { get { return _Argument; } set { _Argument = value; } }

        }
        #endregion
    }
}
