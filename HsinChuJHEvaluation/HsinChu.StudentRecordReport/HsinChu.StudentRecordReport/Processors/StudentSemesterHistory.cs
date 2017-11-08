using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using K12.Data;

namespace HsinChu.StudentRecordReport
{
    class StudentSemesterHistory
    {
        private List<string> _names;
        private Dictionary<string, string> _data;
        private Dictionary<int, string> _map;

        public StudentSemesterHistory()
        {
            InitializeField();
        }

        private void ClearField()
        {
            foreach (string name in new List<string>(_data.Keys))
                _data[name] = "";
        }

        public void SetData(List<SemesterHistoryItem> records)
        {
            ClearField();

            records.Sort(delegate(SemesterHistoryItem a, SemesterHistoryItem b)
            {
                if (a.SchoolYear == b.SchoolYear)
                    return a.Semester.CompareTo(b.Semester);
                else
                    return a.SchoolYear.CompareTo(b.SchoolYear);
            });

            foreach (SemesterHistoryItem record in records)
            {
                int gradeYear = record.GradeYear;
                if (gradeYear > 6) gradeYear -= 6;

                int index = (gradeYear - 1) * 2 + record.Semester;
                if (_map.ContainsKey(index))
                {
                    string key = _map[index];

                    string classSeatNo = string.Empty;
                    if (!string.IsNullOrEmpty(record.ClassName))
                        classSeatNo += record.ClassName + "班";
                    if (!string.IsNullOrEmpty("" + record.SeatNo))
                        classSeatNo += record.SeatNo + "號";
                    _data[key + "導師"] = record.Teacher;
                    _data[key + "班級"] = classSeatNo;

                    _data[key] = record.GradeYear + (record.Semester == 1 ? "上" : "下");
                }
            }
        }

        private void InitializeField()
        {
            _data = new Dictionary<string, string>();
            _data.Add("一上班級", "");
            _data.Add("一下班級", "");
            _data.Add("二上班級", "");
            _data.Add("二下班級", "");
            _data.Add("三上班級", "");
            _data.Add("三下班級", "");
            _data.Add("一上導師", "");
            _data.Add("一下導師", "");
            _data.Add("二上導師", "");
            _data.Add("二下導師", "");
            _data.Add("三上導師", "");
            _data.Add("三下導師", "");
            _data.Add("一上", "");
            _data.Add("一下", "");
            _data.Add("二上", "");
            _data.Add("二下", "");
            _data.Add("三上", "");
            _data.Add("三下", "");

            _map = new Dictionary<int, string>();
            _map.Add(1, "一上");
            _map.Add(2, "一下");
            _map.Add(3, "二上");
            _map.Add(4, "二下");
            _map.Add(5, "三上");
            _map.Add(6, "三下");

            _names = new List<string>(_data.Keys);
        }

        public string[] GetFieldName()
        {
            return _names.ToArray();
        }

        public string[] GetFieldValue()
        {
            List<string> values = new List<string>();
            foreach (string name in _names)
                values.Add(_data[name]);
            return values.ToArray();
        }
    }
}
