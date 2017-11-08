using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Campus
{
    public class FunctionSpliter<IN, OUT>
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
    }
}
