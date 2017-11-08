using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Cells;
using JHSchool.Evaluation.RatingFramework;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating
{
    internal class OutputSeparate
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scopes"></param>
        /// <param name="ratingNames">排名項目名稱，例：國文。</param>
        /// <param name="param"></param>
        internal void Output(List<RatingScope<RatingStudent>> scopes, List<string> ratingNames, FilterParameter filter)
        {
            Workbook book = new Workbook();
            book.Worksheets.Clear();
            Worksheet sheet = book.Worksheets[book.Worksheets.Add()];
            int rowOffset = 0, columnOffset = 0;

            foreach (string field in new string[] { "班級 ", "座號", "姓名", "學號", "名稱", "分數", "排名" })
                sheet.Cells[0, columnOffset++].PutValue(field);

            foreach (string eachRating in ratingNames) //每一種排名。
            {
                foreach (RatingScope<RatingStudent> eachScope in scopes) //每一種範圍。
                {
                    RatingScope<RatingStudent> eachFiltered;
                    if (filter.Mode == FilterMode.None)
                        eachFiltered = eachScope;
                    else if (filter.Mode == FilterMode.Place)
                        eachFiltered = eachScope.GetTopPlaces(eachRating, filter.Top);
                    else if (filter.Mode == FilterMode.Percentage)
                        eachFiltered = eachScope.GetTopPercentage(eachRating, filter.Top);
                    else
                        throw new ArgumentException("無此種取名次方式。");

                    eachFiltered.Sort(new PlaceComparer(eachRating));

                    foreach (RatingStudent student in eachFiltered)
                    {
                        columnOffset = 0;
                        rowOffset++;

                        foreach (string field in new string[] { student.ClassName, student.SeatNo, student.Name, student.StudentNumber })
                            sheet.Cells[rowOffset, columnOffset++].PutValue(field);

                        string score = "", level = "";

                        //去除 00 大絕招。
                        score = ((double)student.Places[eachRating].Score).ToString();
                        level = student.Places[eachRating].Level.ToString();

                        sheet.Cells[rowOffset, columnOffset++].PutValue(eachRating);
                        sheet.Cells[rowOffset, columnOffset++].PutValue(score);
                        sheet.Cells[rowOffset, columnOffset++].PutValue(level);
                    }
                }
            }

            RatingUtils.Save(book, "分別排名結果.xls");
        }
    }
}
