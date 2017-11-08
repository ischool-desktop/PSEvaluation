using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Cells;
using Campus.Rating;

namespace JHEvaluation.Rating
{
    internal class OutputSeparatePivot
    {
        /// <summary>
        /// Pivot 列法，基本上名次一定是全部列出才能適用此種列法。
        /// </summary>
        /// <param name="scopes"></param>
        /// <param name="ratingNames">排名項目名稱清單，例：國文、英文...</param>
        /// <param name="filter"></param>
        public void Output(List<RatingScope<RatingStudent>> scopes, List<string> ratingNames)
        {
            Workbook book = new Workbook();
            book.Worksheets.Clear();
            Worksheet sheet = book.Worksheets[book.Worksheets.Add()];
            int rowOffset = 0, columnOffset = 0;

            List<RatingStudent> students = new List<RatingStudent>();
            foreach (RatingScope<RatingStudent> each in scopes)
                students.AddRange(each);

            //排序學生資料。
            students.Sort(new Comparison<RatingStudent>(RatingUtils.RatingStudentComparison));

            foreach (string field in new string[] { "班級", "座號", "姓名", "學號" })
                sheet.Cells[0, columnOffset++].PutValue(field);

            foreach (string field in ratingNames)
            {
                sheet.Cells[0, columnOffset++].PutValue(field);
                sheet.Cells[0, columnOffset++].PutValue("排名");
                sheet.Cells[0, columnOffset++].PutValue("百分排名");
            }

            foreach (RatingStudent student in students)
            {
                columnOffset = 0;
                rowOffset++;

                foreach (string field in new string[] { student.ClassName, student.SeatNo, student.Name, student.StudentNumber })
                    sheet.Cells[rowOffset, columnOffset++].PutValue(field);

                foreach (string subject in ratingNames)
                {
                    string score = "", level = "", percentage = "";

                    if (student.Places.Contains(subject))
                    {
                        //去除 00 大絕招。
                        score = ((double)student.Places[subject].Score).ToString();
                        level = student.Places[subject].Level.ToString();
                        percentage = student.Places[subject].Percentage.ToString();
                    }

                    sheet.Cells[rowOffset, columnOffset++].PutValue(score);
                    sheet.Cells[rowOffset, columnOffset++].PutValue(level);
                    sheet.Cells[rowOffset, columnOffset++].PutValue(percentage);
                }
            }

            RatingUtils.Save(book, "分別排名結果.xls");
        }
    }
}