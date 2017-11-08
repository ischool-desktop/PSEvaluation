using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using System.Text.RegularExpressions;

namespace KaoHsiung.MidTermTransferReport
{
    class Global
    {
        public static string ReportName = "期中轉學成績證明書";
    }

    /// <summary>
    /// 日期轉換
    /// </summary>
    class DateConvert
    {
        /// <summary>
        /// 將西元年轉民國年
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 將西元年分隔符號轉成中文表示
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
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

    /// <summary>
    /// 表示一個學期
    /// </summary>
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

    /// <summary>
    /// 簡化 Word 操作小工具
    /// </summary>
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

        /// <summary>
        /// 將文字寫入 Cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="font"></param>
        /// <param name="words"></param>
        internal static void Write(Cell cell, Font font, params string[] words)
        {
            cell.Paragraphs.Clear();

            foreach (var word in words)
            {
                cell.Paragraphs.Add(new Paragraph(cell.Document));
                run = new Run(cell.Document);
                run.Font.Name = font.Name;
                run.Font.Size = font.Size;
                run.Text = word;

                cell.LastParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                cell.LastParagraph.Runs.Add(run.Clone(true));
            }
        }

        /// <summary>
        /// 垂直合併
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="count"></param>
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

        /// <summary>
        /// 水平合併
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="count"></param>
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

        /// <summary>
        /// 取得往下第N個Cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="count"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 取得往左第N個Cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="count"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 取得往右第N個Cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="count"></param>
        /// <returns></returns>
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
