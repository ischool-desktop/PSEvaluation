using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using System.Text.RegularExpressions;

namespace KaoHsiung.ReaderScoreImport
{
    public partial class ValidationErrorViewer : BaseForm
    {
        private const string format2 = @"^%(.*):(\d+)%(.*)";
        private const string format3 = @"^\d{1}.*";
        private Regex reline;
        private Regex re3;

        public ValidationErrorViewer()
        {
            InitializeComponent();

            reline = new Regex(format2);
            re3 = new Regex(format3);
        }

        internal void SetTextFileError(Dictionary<int, string> lineIndexes, List<int> errorFormatLineIndexes, List<int> duplicateLineIndexes)
        {
            foreach (int index in lineIndexes.Keys)
            {
                string line = lineIndexes[index];
                Match m = reline.Match(line);
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    rtbView.SelectionColor = Color.Black;
                    rtbView.AppendText("\t" + line + "\r\n");
                }
                else if (errorFormatLineIndexes.Contains(index))
                {
                    if (m.Success)
                    {
                        string fileIndex = m.Groups[2].Value;
                        string content = m.Groups[3].Value;
                        rtbView.SelectionColor = Color.Red;
                        rtbView.AppendText(fileIndex + "\t" + content + "\r\n");
                    }
                }
                else if (duplicateLineIndexes.Contains(index))
                {
                    if (m.Success)
                    {
                        string fileIndex = m.Groups[2].Value;
                        string content = m.Groups[3].Value;
                        rtbView.SelectionColor = Color.Blue;
                        rtbView.AppendText(fileIndex + "\t" + content + "\r\n");
                    }
                }
                //else
                //    color = Color.Black;

                //rtbView.SelectionColor = color;
                //rtbView.AppendText(index + "\t" + lineIndexes[index] + "\r\n");
            }

            rtbView.SelectionStart = 0;
        }

        internal void SetErrorLines(List<string> errorMsg)
        {
            rtbView.SelectionColor = Color.Black;

            List<string> msgs = new List<string>();
            foreach (string line in errorMsg)
            {
                string line2 = line;
                if (!line.EndsWith("\r\n"))
                    line2 += "\r\n";
                if (!msgs.Contains(line2))
                    msgs.Add(line2);
            }

            foreach (string line in msgs)
                rtbView.AppendText(line);
            
            rtbView.SelectionStart = 0;
        }
    }
}
