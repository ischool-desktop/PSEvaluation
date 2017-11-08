using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Words;

namespace JHEvaluation.StudentSemesterScoreReport
{
    internal class WordHelper
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
