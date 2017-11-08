using System;
using System.Collections.Generic;
using System.Text;

namespace JHEvaluation.ScoreCalculation
{
    public class LogData : IEnumerable<LogData>
    {
        private static ILogFormater DefFormater = new DefaultFormater();

        private List<LogData> SubData { get; set; }

        public LogData(string itemName)
        {
            SubData = new List<LogData>();
            Formater = DefFormater;
            ItemName = itemName;
        }

        public LogData(string itemName, string orgValue, string newValue)
            : this(itemName)
        {
            OriginValue = orgValue;
            NewValue = newValue;
        }

        /// <summary>
        /// 項目名稱。
        /// </summary>
        public string ItemName { get; set; }

        public string OriginValue { get; set; }

        public string NewValue { get; set; }

        public ILogFormater Formater { get; set; }

        public void Add(LogData log)
        {
            SubData.Add(log);
        }

        public LogData this[int index]
        {
            get { return SubData[index]; }
        }

        public override string ToString()
        {
            return Formater.Format(this);
        }

        internal class DefaultFormater : ILogFormater
        {
            #region ILogFormater 成員

            public string Format(LogData log)
            {
                return string.Format("{0}({1}->{2})\n", log.ItemName, log.OriginValue, log.NewValue);
            }

            #endregion
        }

        #region IEnumerable<LogData> 成員

        public IEnumerator<LogData> GetEnumerator()
        {
            return SubData.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成員

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return SubData.GetEnumerator();
        }

        #endregion
    }

    public interface ILogFormater
    {
        string Format(LogData log);
    }

    public class SubjectScoreLogFormater : ILogFormater
    {
        #region ILogFormater 成員

        public string Format(LogData log)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0} ", log.ItemName));

            foreach (LogData each in log)
                sb.Append(string.Format("({0}：{1}->{2}) ", each.ItemName, each.OriginValue, each.NewValue));

            sb.AppendLine();

            return sb.ToString();
        }

        #endregion
    }

    public class DomainScoreLogFormater : ILogFormater
    {
        #region ILogFormater 成員

        public string Format(LogData log)
        {
            return string.Format("{0}：{1}->{2}\n", log.ItemName, log.OriginValue, log.NewValue);
        }

        #endregion
    }
}
