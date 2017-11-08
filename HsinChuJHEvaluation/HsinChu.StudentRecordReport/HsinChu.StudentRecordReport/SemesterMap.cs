using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;

namespace HsinChu.StudentRecordReport
{
    class SemesterMap
    {
        private Dictionary<SemesterData, int> _semsMapping;
        private Dictionary<int, string> _schoolYearMapping;

        public SemesterMap()
        {
            _semsMapping = new Dictionary<SemesterData, int>();
            _schoolYearMapping = new Dictionary<int, string>();
        }

        public Dictionary<SemesterData, int> SemesterMapping
        {
            get { return _semsMapping; }
        }

        public Dictionary<int, string> SchoolYearMapping
        {
            get { return _schoolYearMapping; }
        }

        public void SetData(List<K12.Data.SemesterHistoryItem> items)
        {
            foreach (var item in items)
                Add(item);

            RebuildSemesterMap();
        }

        private void Add(K12.Data.SemesterHistoryItem item)
        {
            SemesterData semester = new SemesterData("" + item.SchoolYear, "" + item.Semester);
            if (!_semsMapping.ContainsKey(semester))
            {
                int gradeYear = item.GradeYear;
                if (gradeYear > 6) gradeYear -= 6;

                int index = (gradeYear - 1) * 2 + item.Semester;
                _semsMapping.Add(semester, index - 1);
            }
        }

        private void RebuildSemesterMap()
        {
            Dictionary<int, SemesterData> indexes = new Dictionary<int, SemesterData>();

            foreach (SemesterData sems in _semsMapping.Keys)
            {
                if (!indexes.ContainsKey(_semsMapping[sems]))
                    indexes.Add(_semsMapping[sems], sems);
                else
                {
                    SemesterData orig = indexes[_semsMapping[sems]];
                    if (int.Parse(sems.SchoolYear) > int.Parse(orig.SchoolYear))
                        indexes[_semsMapping[sems]] = sems;
                }
            }

            _semsMapping.Clear();
            _schoolYearMapping.Clear();
            foreach (int index in indexes.Keys)
            {
                SemesterData sems = indexes[index];
                _semsMapping.Add(sems, index);
                _schoolYearMapping.Add(index, sems.SchoolYear);
            }
        }
    }

    class SemesterData
    {
        public string SchoolYear { get; private set; }
        public string Semester { get; private set; }

        public SemesterData(string schoolYear, string semester)
        {
            SchoolYear = schoolYear;
            Semester = semester;
        }

        public override bool Equals(object obj)
        {
            if (obj is SemesterData)
            {
                SemesterData other = obj as SemesterData;
                if (this.SchoolYear == other.SchoolYear && this.Semester == other.Semester)
                    return true;
                return false;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return SchoolYear.GetHashCode() ^ Semester.GetHashCode();
        }
    }
}
