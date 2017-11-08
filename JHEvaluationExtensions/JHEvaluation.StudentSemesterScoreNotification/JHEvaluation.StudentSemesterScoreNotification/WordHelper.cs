using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Words;
using Aspose.Words.Tables;
using Aspose.Words.Fonts;
using System.IO;

namespace JHEvaluation.StudentSemesterScoreNotification
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
            Paragraph para = (Paragraph)cell.GetChild(NodeType.Paragraph, 0, true);
            font = para.ParagraphBreakFont;
            cell.Paragraphs.Clear();
            
            foreach (var word in words)
            {
                cell.Paragraphs.Add(new Paragraph(cell.Document));
                run = new Run(cell.Document);
                FontClone(run.Font, font);
                run.Text = word;

                cell.LastParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                cell.LastParagraph.Runs.Add(run.Clone(true));
            }
        }

        private static void FontClone(Font f1, Font f2)
        {
            f1.Bold = f2.Bold;
            f1.Color = f2.Color;
            f1.Italic = f2.Italic;
            f1.Name = f2.Name;
            f1.Size = f2.Size;
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
