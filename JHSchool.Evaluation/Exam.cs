using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using JHSchool.Evaluation.Feature;

namespace JHSchool.Evaluation
{
    public class Exam : CacheManager<ExamRecord>
    {
        private static Exam _Instance = null;
        public static Exam Instance
        {
            get { if (_Instance == null) _Instance = new Exam(); return _Instance; }
        }

        private Exam()
        {
        }

        protected override Dictionary<string, ExamRecord> GetAllData()
        {
            Dictionary<string, ExamRecord> records = new Dictionary<string, ExamRecord>();

            foreach (ExamRecord each in QueryExam.GetAllExams())
                records.Add(each.ID, each);

            return records;
        }

        protected override Dictionary<string, ExamRecord> GetData(IEnumerable<string> primaryKeys)
        {
            Dictionary<string, ExamRecord> records = new Dictionary<string, ExamRecord>();

            foreach (ExamRecord each in QueryExam.GetExams(primaryKeys))
                records.Add(each.ID, each);

            return records;
        }
    }

    public static class Exam_ExtendMethods
    {

    }
}
