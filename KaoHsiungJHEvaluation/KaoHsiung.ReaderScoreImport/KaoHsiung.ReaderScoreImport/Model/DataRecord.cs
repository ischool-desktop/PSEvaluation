using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaoHsiung.ReaderScoreImport.Mapper;

namespace KaoHsiung.ReaderScoreImport.Model
{
    internal class DataRecord
    {
        private RawData _raw;

        //學號{7}，班級{3}，座號{2}，試別{2}，科目{5}，成績{6}
        public string StudentNumber { get; set; }
        public string Class { get; set; }
        public string SeatNo { get; set; }
        public string Exam { get; set; }
        public List<string> Subjects { get; set; }
        public decimal Score { get; set; }

        public DataRecord(RawData raw)
        {
            _raw = raw;
            Subjects = new List<string>();

            StudentNumber = _raw.StudentNumber;
            Class = ClassCodeMapper.Instance.Map(_raw.ClassCode);
            SeatNo = _raw.SeatNo;
            Exam = ExamCodeMapper.Instance.Map(_raw.ExamCode);
            string subjects = SubjectCodeMapper.Instance.Map(_raw.SubjectCode);
            foreach (string subject in subjects.Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries))
            {
                string s = subject.Trim();
                if(!Subjects.Contains(s))
                    Subjects.Add(s);
            }
            Score = ParseScore(_raw.Score);
        }

        private decimal ParseScore(string p)
        {
            decimal d;
            if (decimal.TryParse(p.Trim(), out d))
                return d;
            else
                throw new Exception();
        }
    }

    internal class DataRecordCollection : List<DataRecord>
    {
        internal void ConvertFromRawData(List<RawData> _raws)
        {
            foreach (RawData raw in _raws)
            {
                DataRecord dr = new DataRecord(raw);
                Add(dr);
            }
        }
    }
}
