using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using JHSchool.Evaluation.Ranking;
using JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking.Basic;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking
{
    internal class SeparateAllReport : AbstractReport
    {
        public bool PrintByClass { get; set; }

        public SeparateAllReport(List<RankData> rankData, Dictionary<string, RStudentRecord> studentDict)
        {
            _rankData = rankData;
            _studentDict = studentDict;
            PrintByClass = false;
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
                        printData.Add(id, new Dictionary<string, RankScore>());

                    if (!printData[id].ContainsKey(name))
                        printData[id].Add(name, data[id]);
                }
            }

            List<RStudentRecord> list = RStudentRecord.LoadStudents(printData.Keys);
            list.Sort(SortStudents);

            int rowIndex = 0;
            bool first = true;
            string lastClassName = string.Empty;
            foreach (RStudentRecord student in list)
            {
                rowIndex++;
                ws.Cells[rowIndex, 0].PutValue(student.ClassName);
                ws.Cells[rowIndex, 1].PutValue(student.SeatNo);
                ws.Cells[rowIndex, 2].PutValue(student.StudentNumber);
                ws.Cells[rowIndex, 3].PutValue(student.Name);

                foreach (string name in printData[student.ID].Keys)
                {
                    ws.Cells[rowIndex, itemColIndex[name]].PutValue("" + printData[student.ID][name].Score);
                    ws.Cells[rowIndex, itemColIndex[name] + 1].PutValue("" + printData[student.ID][name].Rank);
                }

                if (PrintByClass && !first && lastClassName != student.ClassName)
                {
                    ws.HPageBreaks.Add(rowIndex, 0);
                }
                lastClassName = student.ClassName;
                first = false;
            }

            Save(book);
        }
    }
}
