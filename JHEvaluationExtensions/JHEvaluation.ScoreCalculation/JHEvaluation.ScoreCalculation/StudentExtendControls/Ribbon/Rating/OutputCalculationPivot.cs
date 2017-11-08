using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Cells;
using JHSchool.Evaluation.RatingFramework;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating
{
    internal class OutputCalculationPivot
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scopes"></param>
        /// <param name="originParsers"></param>
        /// <param name="rankName">排名項目名稱，例如：加權平均…</param>
        /// <param name="filter"></param>
        internal void Output(List<RatingScope<RatingStudent>> scopes, List<ScoreParser> originParsers, string rankName, FilterParameter filter)
        {
            Workbook book = new Workbook();
            book.Worksheets.Clear();
            Worksheet sheet = book.Worksheets[book.Worksheets.Add()];
            int rowOffset = 0, columnOffset = 0;

            Dictionary<string, RatingStudent> dicstudents = new Dictionary<string, RatingStudent>();
            foreach (RatingScope<RatingStudent> eachOrigin in scopes)
            {
                RatingScope<RatingStudent> eachFiltered;
                if (filter.Mode == FilterMode.None)
                    eachFiltered = eachOrigin;
                else if (filter.Mode == FilterMode.Place)
                    eachFiltered = eachOrigin.GetTopPlaces(rankName, filter.Top);
                else if (filter.Mode == FilterMode.Percentage)
                    eachFiltered = eachOrigin.GetTopPercentage(rankName, filter.Top);
                else
                    throw new ArgumentException("無此種取名次方式。");

                foreach (RatingStudent eachStud in eachFiltered)
                {
                    if (!dicstudents.ContainsKey(eachStud.Id))
                        dicstudents.Add(eachStud.Id,eachStud);
                }
            }
            List<RatingStudent> students = new List<RatingStudent>(dicstudents.Values);

            //排序學生資料。
            students.Sort(new Comparison<RatingStudent>(RatingUtils.RatingStudentComparison));

            foreach (string field in new string[] { "班級 ", "座號", "姓名", "學號" })
                sheet.Cells[0, columnOffset++].PutValue(field);

            //列出原始成績的科目欄位。
            foreach (ScoreParser field in originParsers)
                sheet.Cells[0, columnOffset++].PutValue(field.Name);

            sheet.Cells[0, columnOffset++].PutValue(rankName); //運算排名的名稱。
            sheet.Cells[0, columnOffset++].PutValue("名次");  //運算排名的名次。

            foreach (RatingStudent student in students)
            {
                columnOffset = 0;
                rowOffset++;

                foreach (string field in new string[] { student.ClassName, student.SeatNo, student.Name, student.StudentNumber })
                    sheet.Cells[rowOffset, columnOffset++].PutValue(field);

                //列出原始成績。
                foreach (ScoreParser eachParser in originParsers)
                {
                    decimal? originScore = eachParser.GetScore(student);

                    if (originScore.HasValue)
                        sheet.Cells[rowOffset, columnOffset++].PutValue(((double)originScore).ToString());
                    else
                        columnOffset++;
                }

                string score = "", level = "";
                if (student.Places.Contains(rankName))
                {
                    //去除 00 大絕招。
                    score = ((double)student.Places[rankName].Score).ToString();
                    level = student.Places[rankName].Level.ToString();
                }

                sheet.Cells[rowOffset, columnOffset++].PutValue(score);
                sheet.Cells[rowOffset, columnOffset++].PutValue(level);

            }

            RatingUtils.Save(book, "運算排名結果.xls");
        }
    }
}
