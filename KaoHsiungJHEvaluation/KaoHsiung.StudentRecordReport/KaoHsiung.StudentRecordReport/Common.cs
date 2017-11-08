using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Words;
using System.Text.RegularExpressions;

namespace KaoHsiung.StudentRecordReport
{
    public class Global
    {
        public static string ReportName = "學籍表";
        /// <summary>
        /// 承辦人員，給報表用
        /// </summary>
        public static string TransferName = string.Empty;
        /// <summary>
        /// 註冊組長，給報表用
        /// </summary>
        public static string RegManagerName = string.Empty;

        /// <summary>
        /// 2012/9/2 - 單檔儲存設定
        /// </summary>
        public static string OneFileSave = "學籍表_檔案儲存設定";

        /// <summary>
        /// 服務學時數暫存使用
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> _SLRDict = new Dictionary<string, Dictionary<string, string>>();
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

    class SemesterMap
    {
        private Dictionary<SemesterData, int> _semsMapping;
        private Dictionary<int, string> _schoolYearMapping;

        public SemesterMap()
        {
            _semsMapping = new Dictionary<SemesterData, int>();
            _schoolYearMapping = new Dictionary<int, string>();
        }

        public Dictionary<SemesterData, int> SemesterMapping
        {
            get { return _semsMapping; }
        }

        public Dictionary<int, string> SchoolYearMapping
        {
            get { return _schoolYearMapping; }
        }

        public void SetData(List<K12.Data.SemesterHistoryItem> items)
        {
            foreach (var item in items)
                Add(item);

            RebuildSemesterMap();
        }

        private void Add(K12.Data.SemesterHistoryItem item)
        {
            SemesterData semester = new SemesterData("" + item.SchoolYear, "" + item.Semester);
            if (!_semsMapping.ContainsKey(semester))
            {
                int gradeYear = item.GradeYear;
                if (gradeYear > 6) gradeYear -= 6;

                int index = (gradeYear - 1) * 2 + item.Semester;
                _semsMapping.Add(semester, index - 1);
            }
        }

        private void RebuildSemesterMap()
        {
            Dictionary<int, SemesterData> indexes = new Dictionary<int, SemesterData>();

            foreach (SemesterData sems in _semsMapping.Keys)
            {
                if (!indexes.ContainsKey(_semsMapping[sems]))
                    indexes.Add(_semsMapping[sems], sems);
                else
                {
                    SemesterData orig = indexes[_semsMapping[sems]];
                    if (int.Parse(sems.SchoolYear) > int.Parse(orig.SchoolYear))
                        indexes[_semsMapping[sems]] = sems;
                }
            }

            _semsMapping.Clear();
            _schoolYearMapping.Clear();
            foreach (int index in indexes.Keys)
            {
                SemesterData sems = indexes[index];
                _semsMapping.Add(sems, index);
                _schoolYearMapping.Add(index, sems.SchoolYear);
            }
        }
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
