using System.Collections.Generic;
using Aspose.Words;
using JHSchool.Data;
using Aspose.Words.Tables;

namespace KaoHsiung.StudentExamScoreReport.Processor
{
    internal class StudentMoralScore
    {
        private DocumentBuilder _builder;
        private DataManager _manager;
        private Dictionary<string, string> _periodMapping;
        //private List<string> _absenceList;

        public StudentMoralScore(DocumentBuilder builder, Config config, Dictionary<string, string> periodMapping, List<string> absenceList)
        {
            _builder = builder;
            _periodMapping = periodMapping;
            //_absenceList = absenceList;

            _manager = new DataManager();
            _manager.SetConfig(config);
        }

        internal void SetData(List<JHMeritRecord> meritList, List<JHDemeritRecord> demeritList, List<JHAttendanceRecord> attendanceList)
        {
            foreach (var record in meritList)
            {
                if (record.MeritA.HasValue)
                    _manager.AddDisciplineData("獎勵", "大功", record.MeritA.Value);
                if (record.MeritB.HasValue)
                    _manager.AddDisciplineData("獎勵", "小功", record.MeritB.Value);
                if (record.MeritC.HasValue)
                    _manager.AddDisciplineData("獎勵", "嘉獎", record.MeritC.Value);
            }

            foreach (var record in demeritList)
            {
                if (record.Cleared == "是") continue;

                if (record.DemeritA.HasValue)
                    _manager.AddDisciplineData("懲戒", "大過", record.DemeritA.Value);
                if (record.DemeritB.HasValue)
                    _manager.AddDisciplineData("懲戒", "小過", record.DemeritB.Value);
                if (record.DemeritC.HasValue)
                    _manager.AddDisciplineData("懲戒", "警告", record.DemeritC.Value);
            }

            foreach (var record in attendanceList)
            {
                foreach (var item in record.PeriodDetail)
                {
                    if (!_periodMapping.ContainsKey(item.Period)) continue;

                    _manager.AddAttendanceData(_periodMapping[item.Period], item.AbsenceType, 1);
                }
            }

            FillData();
        }

        private void FillData()
        {
            _builder.MoveToMergeField("缺曠獎懲");

            Cell cell = _builder.CurrentParagraph.ParentNode as Cell;
            double width = cell.CellFormat.Width;
            int columnCount = _manager.DisciplineCount + _manager.AttendacneCount;
            double miniUnitWitdh = width / (double)columnCount;

            Table table = _builder.StartTable();

            _builder.RowFormat.HeightRule = HeightRule.Exactly;
            _builder.RowFormat.Height = 18.0;

            _builder.InsertCell().CellFormat.Width = miniUnitWitdh * _manager.DisciplineCount;
            _builder.Write("獎懲情形");
            if (_manager.AttendacneCount > 0)
            {
                _builder.InsertCell().CellFormat.Width = miniUnitWitdh * _manager.AttendacneCount;
                //_builder.Write("缺曠情形"); 因高雄小組會議修改
                _builder.Write("缺曠情形(單位節數)");
            }
            _builder.EndRow();

            foreach (var key in _manager.Discipline.Keys)
            {
                _builder.InsertCell().CellFormat.Width = miniUnitWitdh * _manager.Discipline[key].Count;
                _builder.Write(key);
            }
            foreach (var key in _manager.Attendance.Keys)
            {
                _builder.InsertCell().CellFormat.Width = miniUnitWitdh * _manager.Attendance[key].Count;
                _builder.Write(key);
            }
            _builder.EndRow();

            List<int> valueList = new List<int>();
            foreach (var key in _manager.Discipline.Keys)
            {
                foreach (var instance in _manager.Discipline[key].Keys)
                {
                    _builder.InsertCell().CellFormat.Width = miniUnitWitdh;
                    _builder.Write(instance);
                    valueList.Add(_manager.Discipline[key][instance]);
                }
            }
            foreach (var key in _manager.Attendance.Keys)
            {
                foreach (var instance in _manager.Attendance[key].Keys)
                {
                    _builder.InsertCell().CellFormat.Width = miniUnitWitdh;
                    _builder.Write(instance);
                    valueList.Add(_manager.Attendance[key][instance]);
                }
            }
            _builder.EndRow();

            foreach (int times in valueList)
            {
                _builder.InsertCell().CellFormat.Width = miniUnitWitdh;
                //string v = string.Empty;
                //if (times > 0) v = "" + times;
                _builder.Write("" + times);
            }
            _builder.EndRow();
            _builder.EndTable();

            //去除表格四邊的線
            foreach (Cell c in table.FirstRow.Cells)
                c.CellFormat.Borders.Top.LineStyle = LineStyle.None;

            foreach (Cell c in table.LastRow.Cells)
                c.CellFormat.Borders.Bottom.LineStyle = LineStyle.None;

            foreach (Row r in table.Rows)
            {
                r.FirstCell.CellFormat.Borders.Left.LineStyle = LineStyle.None;
                r.LastCell.CellFormat.Borders.Right.LineStyle = LineStyle.None;
            }

            //補上去一條線…
            (table.ParentNode as Cell).CellFormat.Borders.Top.LineStyle = LineStyle.Double;
        }
    }

    class DataManager
    {
        private DisciplineData _disData;
        private AttendanceData _attData;

        public DataManager()
        {
            _disData = new DisciplineData();
            _attData = new AttendanceData();
        }

        internal void AddDisciplineData(string type, string instance, int times)
        {
            _disData.Add(type, instance, times);
        }

        internal void AddAttendanceData(string type, string instance, int times)
        {
            _attData.Add(type, instance, times);
        }

        internal int DisciplineCount { get { return _disData.Count; } }

        internal int AttendacneCount { get { return _attData.Count; } }

        internal Dictionary<string, Dictionary<string, int>> Discipline { get { return _disData.Data; } }

        internal Dictionary<string, Dictionary<string, int>> Attendance { get { return _attData.Data; } }

        internal void SetConfig(Config config)
        {
            _attData.SetConfig(config);
        }
    }

    class DisciplineData
    {
        private Dictionary<string, Dictionary<string, int>> _data;
        public Dictionary<string, Dictionary<string, int>> Data { get { return _data; } }

        public DisciplineData()
        {
            _data = new Dictionary<string, Dictionary<string, int>>();

            _data.Add("獎勵", new Dictionary<string, int>());
            _data.Add("懲戒", new Dictionary<string, int>());

            foreach (var item in new string[] { "大功", "小功", "嘉獎" })
                _data["獎勵"].Add(item, 0);
            foreach (var item in new string[] { "大過", "小過", "警告" })
                _data["懲戒"].Add(item, 0);
        }

        internal void Add(string type, string instance, int times)
        {
            if (!_data.ContainsKey(type)) return;
            if (!_data[type].ContainsKey(instance)) return;

            _data[type][instance] += times;
        }

        internal int Count
        {
            get
            {
                int result = 0;
                foreach (var type in _data.Keys)
                    result += _data[type].Count;
                return result;
            }
        }
    }

    class AttendanceData
    {
        private Dictionary<string, Dictionary<string, int>> _data;
        public Dictionary<string, Dictionary<string, int>> Data { get { return _data; } }

        public AttendanceData()
        {
            _data = new Dictionary<string, Dictionary<string, int>>();
        }

        internal void Add(string type, string instance, int times)
        {
            if (!_data.ContainsKey(type)) return;
            if (!_data[type].ContainsKey(instance)) return;

            _data[type][instance] += times;
        }

        internal void SetConfig(Config config)
        {
            foreach (var type in config.PrintTypes.Keys)
            {
                _data.Add(type, new Dictionary<string, int>());

                foreach (var instance in config.PrintTypes[type])
                    _data[type].Add(instance, 0);
            }
        }

        internal int Count
        {
            get
            {
                int result = 0;
                foreach (var type in _data.Keys)
                    result += _data[type].Count;
                return result;
            }
        }
    }
}
