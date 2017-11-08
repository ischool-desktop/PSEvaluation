using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using System.Text.RegularExpressions;

namespace HsinChu.StudentExamScoreReport
{
    public class Global
    {
        public static string ReportName = "個人評量成績單";

        /// <summary>
        /// 存放課程編號科目Dictionary
        /// </summary>
        public static Dictionary<string, string> _CourseIDSubjectDict = new Dictionary<string, string>();

        /// <summary>
        /// 使用者選擇的課程ID
        /// </summary>
        public static List<string> _selectCourseIDList = new List<string>();
    }

    class DateConvert
    {
        internal static string CDate(string p)
        {
            DateTime d = DateTime.Now;
            if (p != "" && DateTime.TryParse(p, out d))
            {
                return "" + (d.Year - 1911) + "/" + ("" + d.Month).PadLeft(2, '0') + "/" + ("" + d.Day).PadLeft(2, '0');
            }
            else
                return "";
        }

        internal static string ChineseUnitDate(string p)
        {
            Regex re = new Regex(@"(\d+)/(\d+)/(\d+)");
            Match match = re.Match(p);
            if (match.Success)
            {
                if (match.Groups.Count == 4)
                    return string.Format("{0}年{1}月{2}日", match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
            }
            return p;
        }
    }

    class SemesterData : IComparable<SemesterData>
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

        #region IComparable<SemesterData> 成員

        public int CompareTo(SemesterData other)
        {
            return this.SchoolYear == other.SchoolYear ? this.Semester.CompareTo(other.Semester) : this.SchoolYear.CompareTo(other.SchoolYear);
        }

        #endregion
    }

    class WordHelper
    {
        internal static Run CreateRun(DocumentBuilder builder)
        {
            Run run = new Run(builder.Document);
            run.Font.Name = builder.Font.Name;
            run.Font.Size = builder.Font.Size;
            run.Text = string.Empty;
            return run;
        }

        private static Run run;

        internal static void Write(Cell cell, string text, DocumentBuilder builder)
        {
            if (cell.FirstParagraph == null)
                cell.Paragraphs.Add(new Paragraph(cell.Document));

            builder.MoveTo(cell.FirstParagraph);
            run = new Run(builder.Document);
            run.Font.Name = builder.Font.Name;
            run.Font.Size = builder.Font.Size;
            run.Text = text;

            cell.FirstParagraph.Runs.Clear();
            cell.FirstParagraph.Runs.Add(run.Clone(true));
        }

        internal static void MergeVerticalCell(Cell cell, int count)
        {
            count--;
            cell.CellFormat.VerticalMerge = CellMerge.First;

            for (int i = 0; i < count; i++)
            {
                cell = GetMoveDownCell(cell, 1);
                cell.CellFormat.VerticalMerge = CellMerge.Previous;
            }
        }

        internal static void MergeHorizontalCell(Cell cell, int count)
        {
            count--;
            cell.CellFormat.HorizontalMerge = CellMerge.First;

            for (int i = 0; i < count; i++)
            {
                cell = GetMoveRightCell(cell, 1);
                cell.CellFormat.HorizontalMerge = CellMerge.Previous;
            }
        }

        internal static Cell GetMoveDownCell(Cell cell, int count)
        {
            Row row = cell.ParentRow;
            int col_index = row.IndexOf(cell);
            Table table = row.ParentTable;
            int row_index = table.Rows.IndexOf(row) + count;

            try
            {
                return table.Rows[row_index].Cells[col_index];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal static Cell GetMoveLeftCell(Cell cell, int count)
        {
            Row row = cell.ParentRow;
            int col_index = row.IndexOf(cell);
            Table table = row.ParentTable;
            int row_index = table.Rows.IndexOf(row);

            try
            {
                return table.Rows[row_index].Cells[col_index - count];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal static Cell GetMoveRightCell(Cell cell, int count)
        {
            Row row = cell.ParentRow;
            int col_index = row.IndexOf(cell);
            Table table = row.ParentTable;
            int row_index = table.Rows.IndexOf(row);

            try
            {
                return table.Rows[row_index].Cells[col_index + count];
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
