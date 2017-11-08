using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Cells;
using JHSchool.Evaluation.Ranking;
using JHSchool.Data;

namespace JHSchool.Evaluation.Ranking
{
    internal class CombineReport : AbstractReport
    {
        public bool PrintByClass { get; set; }
        public string Display { get; set; }

        public CombineReport(List<RankData> rankData, Dictionary<string, JHStudentRecord> studentDict)
        {
            _rankData = rankData;
            _studentDict = studentDict;
            PrintByClass = false;
            Display = string.Empty;
            RankType = RankType.Class;
        }

        public override void Export()
        {
            Workbook book = new Workbook();
            Worksheet ws = book.Worksheets[0];
            ws.Name = ReportName;

            int colIndex = 0;
            ws.Cells[0, colIndex++].PutValue("班級");
            ws.Cells[0, colIndex++].PutValue("座號");
            ws.Cells[0, colIndex++].PutValue("學號");
            ws.Cells[0, colIndex++].PutValue("姓名");

            Dictionary<string, int> itemColIndex = new Dictionary<string, int>();

            Dictionary<string, Dictionary<string, RankScore>> printData = new Dictionary<string, Dictionary<string, RankScore>>();
            Dictionary<string, Dictionary<string, RankScore>> tagPrintData = new Dictionary<string, Dictionary<string, RankScore>>();

            List<RankData> rankData = new List<RankData>();
            foreach (RankData data in _rankData)
            {
                foreach (RankData tagData in (data.Tag as Dictionary<string, RankData>).Values)
                    rankData.Add(tagData);
            }

            foreach (RankData data in rankData)
            {
                string name = data.Name;
                itemColIndex.Add(name, colIndex);
                ws.Cells[0, colIndex++].PutValue(name);

                foreach (string id in data.Keys)
                {
                    if (!tagPrintData.ContainsKey(id))
                        tagPrintData.Add(id, new Dictionary<string, RankScore>());

                    if (!tagPrintData[id].ContainsKey(name))
                        tagPrintData[id].Add(name, data[id]);
                }
            }

            foreach (RankData data in _rankData)
            {
                string name = data.Name;
                itemColIndex.Add(name, colIndex);
                ws.Cells[0, colIndex++].PutValue(name);
                if (RankType == RankType.Class)
                    ws.Cells[0, colIndex++].PutValue("班排名");
                else if (RankType == RankType.GradeYear)
                    ws.Cells[0, colIndex++].PutValue("年排名");

                foreach (string id in data.Keys)
                {
                    if (!printData.ContainsKey(id))
                    {
                        printData.Add(id, new Dictionary<string, RankScore>());
                        if (tagPrintData.ContainsKey(id))
                        {
                            foreach (string tagName in tagPrintData[id].Keys)
                                printData[id].Add(tagName, tagPrintData[id][tagName]);
                        }
                    }

                    if (!printData[id].ContainsKey(name))
                        printData[id].Add(name, data[id]);
                }
            }

            //List<StudentRecord> list = Student.Instance.GetStudents(new List<string>(printData.Keys).ToArray());
            List<JHStudentRecord> list = JHStudent.SelectByIDs(new List<string>(printData.Keys));
            list.Sort(SortStudents);

            int rowIndex = 0;
            bool first = true;
            string lastClassName = string.Empty;
            foreach (JHStudentRecord student in list)
            {
                rowIndex++;
                string className = (student.Class != null) ? student.Class.Name : "";
                ws.Cells[rowIndex, 0].PutValue(className);
                ws.Cells[rowIndex, 1].PutValue(student.SeatNo);
                ws.Cells[rowIndex, 2].PutValue(student.StudentNumber);
                ws.Cells[rowIndex, 3].PutValue(student.Name);

                foreach (string name in printData[student.ID].Keys)
                {
                    ws.Cells[rowIndex, itemColIndex[name]].PutValue("" + printData[student.ID][name].Score);
                    ws.Cells[rowIndex, itemColIndex[name] + 1].PutValue("" + printData[student.ID][name].Rank);
                }

                if (PrintByClass && !first && lastClassName != className)
                {
                    ws.HPageBreaks.Add(rowIndex, 0);
                }
                lastClassName = className;
                first = false;
            }

            Save(book);
        }
    }
}
