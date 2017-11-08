using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHEvaluation.StudentSemesterScoreReport.Association.UDT;
using Aspose.Words;

namespace JHEvaluation.StudentSemesterScoreReport.Writers
{
    internal class AssnScoreWriter
    {
        public AssnScore Score { get; set; }

        public AssnScoreWriter()
        {
        }

        public void Writer(Document doc)
        {
            DocumentBuilder builder = new DocumentBuilder(doc);
            bool hasAssnField = builder.MoveToMergeField("社團活動");
            if (!hasAssnField) return;

            Font font = builder.Font;

            Cell cell = builder.CurrentParagraph.ParentNode as Cell;

            Run run1 = new Run(doc);
            run1.Font.Name = font.Name;
            run1.Font.Size = font.Size;
            run1.Font.Bold = true;
            run1.Text = "社團活動：";
            Run run2 = new Run(doc);
            run2.Font.Name = font.Name;
            run2.Font.Size = font.Size;

            if (Score != null)
            {
                List<string> list = new List<string>();
                if (!string.IsNullOrEmpty(Score.Score)) list.Add(Score.Score);
                if (!string.IsNullOrEmpty(Score.Effort)) list.Add(Score.Effort);
                if (!string.IsNullOrEmpty(Score.Text)) list.Add(Score.Text);

                run2.Text = string.Join(",", list.ToArray());
            }
            else
                run2.Text = "";

            cell.Paragraphs.RemoveAt(0);
            cell.Paragraphs.Add(new Paragraph(doc));
            cell.LastParagraph.Runs.Add(run1);
            cell.LastParagraph.Runs.Add(run2);
        }
    }
}
