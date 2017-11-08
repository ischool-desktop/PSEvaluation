using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using Aspose.Words;

namespace KaoHsiung.StudentRecordReport.Processor
{
    class StudentHistoryProcessor
    {
        private Dictionary<int, string> _gradeMap;
        private Dictionary<string, string> _data;

        public StudentHistoryProcessor(DocumentBuilder builder, SemesterMap map, JHSemesterHistoryRecord record)
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

            _gradeMap = new Dictionary<int, string>();
            _gradeMap.Add(1, "一上");
            _gradeMap.Add(2, "一下");
            _gradeMap.Add(3, "二上");
            _gradeMap.Add(4, "二下");
            _gradeMap.Add(5, "三上");
            _gradeMap.Add(6, "三下");

            List<string> fieldName = new List<string>();
            List<string> fieldValue = new List<string>();

            int index = 0;
            //foreach (string name in new string[] { "一上", "一下", "二上", "二下", "三上", "三下" })
            foreach (string name in new string[] { "一年級學年度", "二年級學年度", "三年級學年度" })
            {
                fieldName.Add(name);
                if (map.SchoolYearMapping.ContainsKey(index))
                    fieldValue.Add(map.SchoolYearMapping[index] + "學年度");
                else
                    fieldValue.Add("");
                index += 2;
            }

            foreach (var item in record.SemesterHistoryItems)
            {
                int gradeYear = item.GradeYear;
                if (gradeYear > 6) gradeYear -= 6;

                index = (gradeYear - 1) * 2 + item.Semester;
                if (_gradeMap.ContainsKey(index))
                {
                    string key = _gradeMap[index];

                    string classSeatNo = string.Empty;
                    if (!string.IsNullOrEmpty(item.ClassName))
                        classSeatNo += item.ClassName + "班";
                    if (!string.IsNullOrEmpty("" + item.SeatNo))
                        classSeatNo += item.SeatNo + "號";

                    _data[key + "導師"] = item.Teacher;
                    _data[key + "班級"] = classSeatNo;
                }
            }

            fieldName.AddRange(_data.Keys);
            fieldValue.AddRange(_data.Values);

            builder.Document.MailMerge.Execute(fieldName.ToArray(), fieldValue.ToArray());
        }
    }
}
