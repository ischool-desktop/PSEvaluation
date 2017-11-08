using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace KaoHsiung.ReaderScoreImport.Model
{
    internal class RawData
    {
        //學號{7}，班級{3}，座號{2}，試別{2}，科目{5}，成績{6}
        //成績 = 000.00
        // private const string format = @"^(\d{7})(\d{3})\d{2}(\d{2})(\d{5})(([\d| ]\d{2}| [\d| ]\d)\.\d{2})$";        

        private Regex re;

        public string FileName { get; set; }
        public string OriginalString { get; set; }

        public string StudentNumber { get; set; }
        public string ClassCode { get; set; }
        public string SeatNo { get; set; }
        public string ExamCode { get; set; }
        public string SubjectCode { get; set; }
        public string Score { get; set; }

        public RawData(string filename, string original)
        {
            FileName = filename;
            OriginalString = original;

           // re = new Regex(format);
            re = new Regex(Global.TextFormat);
            Match match = re.Match(original);
            if (match.Success)
            {
                StudentNumber = match.Groups[1].Value;
                ClassCode = match.Groups[2].Value;
                ExamCode = match.Groups[3].Value;
                SubjectCode = match.Groups[4].Value;
                Score = match.Groups[5].Value;
            }
            else { } // 理論上，進來的資料格式都應該正確。
        }
    }

    internal class RawDataCollection : List<RawData>
    {
        internal void ConvertFromFiles(List<FileInfo> _files)
        {
            foreach (FileInfo file in _files)
            {
                StreamReader sr = new StreamReader(file.OpenRead());
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        RawData rd = new RawData(file.Name, line);
                        Add(rd);
                    }
                }
            }
        }
    }
}
