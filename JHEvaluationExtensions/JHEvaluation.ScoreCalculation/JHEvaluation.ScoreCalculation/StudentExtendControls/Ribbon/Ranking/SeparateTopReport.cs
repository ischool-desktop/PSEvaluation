using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Cells;
using JHSchool.Evaluation.Ranking;
using JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking.Basic;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking
{
    internal class SeparateTopReport : AbstractReport
    {
        public string Display { get; set; }
        public string Category { get; set; }

        public SeparateTopReport(List<RankData> rankData, Dictionary<string, RStudentRecord> studentDict)
        {
            _rankData = rankData;
            _studentDict = studentDict;
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
            ws.Cells[0, colIndex++].PutValue(Category + "名稱");
            ws.Cells[0, colIndex++].PutValue("成績");
            ws.Cells[0, colIndex++].PutValue((RankType == RankType.Class) ? "班排名/%" : "年排名/%");


            Dictionary<string, Dictionary<string, RankScore>> printData = new Dictionary<string, Dictionary<string, RankScore>>();
            foreach (RankData data in _rankData)
            {
                string name = data.Name;
                if (!printData.ContainsKey(name))
                    printData.Add(name, new Dictionary<string, RankScore>());

                foreach (string id in data.Keys)
                {
                    if (!printData[name].ContainsKey(id))
                        printData[name].Add(id, data[id]);
                }
            }

            int rowIndex = 0;
            foreach (string name in printData.Keys)
            {
                foreach (string id in printData[name].Keys)
                {
                    RStudentRecord student = _studentDict[id];
                    rowIndex++;
                    colIndex = 0;
                    ws.Cells[rowIndex, colIndex++].PutValue(student.ClassName);
                    ws.Cells[rowIndex, colIndex++].PutValue(student.SeatNo);
                    ws.Cells[rowIndex, colIndex++].PutValue(student.StudentNumber);
                    ws.Cells[rowIndex, colIndex++].PutValue(student.Name);
                    ws.Cells[rowIndex, colIndex++].PutValue(name);
                    ws.Cells[rowIndex, colIndex++].PutValue("" + printData[name][id].Score);
                    ws.Cells[rowIndex, colIndex].PutValue("" + printData[name][id].Rank + Display);
                }
            }

            Save(book);
        }
    }
}
