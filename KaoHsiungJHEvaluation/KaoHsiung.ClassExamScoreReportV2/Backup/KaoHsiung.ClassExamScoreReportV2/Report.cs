using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;
using Aspose.Cells;
using System.IO;

namespace KaoHsiung.ClassExamScoreReportV2
{
    internal class Report
    {
        public string SchoolName { get; set; }

        public string SchoolYear { get; set; }

        public string Semester { get; set; }

        public Dictionary<string, ReportStudent> AllStudents { get; set; }

        public List<JHClassRecord> Classes { get; set; }

        public ReportPreference Perference { get; set; }

        public List<string> SelectedSubject { get; set; }

        public List<string> SelectedDomain { get; set; }

        public Dictionary<string, string> SubjectDomainMap { get; set; }

        private Workbook Template { get; set; }

        private Workbook OutputBook { get; set; }

        private Worksheet OutputSheet { get; set; }

        private int _MaxClassStudCot = 0;

        public string ExamName { get; set; }
        //public const int ScoreHeaderCount = 35;

        private Dictionary<string, JHSchool.Evaluation.Calculation.ScoreCalculator> Calcs { get; set; }
        private JHSchool.Evaluation.Calculation.ScoreCalculator DefaultCalc { get; set; }

        public Report()
        {
            Template = new Workbook();
            //Template.Open(new MemoryStream(Prc.班級評量成績單B4));
            DefaultCalc = new JHSchool.Evaluation.Calculation.ScoreCalculator(null);
            Calcs = new Dictionary<string, JHSchool.Evaluation.Calculation.ScoreCalculator>();
            foreach (var record in JHScoreCalcRule.SelectAll())
            {
                if (!Calcs.ContainsKey(record.ID))
                    Calcs.Add(record.ID, new JHSchool.Evaluation.Calculation.ScoreCalculator(record));
            }
        }

        public Workbook Output()
        {
            // 取得班級最大人數
            _MaxClassStudCot=0;

            foreach (JHClassRecord classRec in Classes)
                if (classRec.Students.Count >= _MaxClassStudCot)
                    _MaxClassStudCot = classRec.Students.Count;            



            OutputBook = new Workbook();
            OutputBook.Open(GetTemplateStream());
            OutputSheet = OutputBook.Worksheets[0];

            Template.Open(GetTemplateStream());

            int ScoreHeaderCount = (Perference.PaperSize == "B4") ? 32 : 18;

            int ClassOffset = 0; //整個班級的 Offset。
            int RowOffset = 0;
            int ColumnOffset = 0;

            Range all = Template.Worksheets.GetRangeByName("All");

            //Range printDate = Template.Worksheets.GetRangeByName("PrintDate");
            //printDate[0, 0].PutValue(DateTime.Today.ToString("yyyy/MM/dd"));

            Range title = Template.Worksheets.GetRangeByName("Title");
            //Range feedback = Template.Worksheets.GetRangeByName("Feedback");
            Range rowHeaders = Template.Worksheets.GetRangeByName("RowHeaders");
            Range columnHeaders = Template.Worksheets.GetRangeByName("ColumnHeaders");
            Range rankColumnHeader = Template.Worksheets.GetRangeByName("RankColumnHeader");

            int RowNumber = all.RowCount;
            int DataRowNumber = rowHeaders.RowCount;

            //排序班級。
            Classes.Sort(new Comparison<JHClassRecord>(Utilities.JHClassRecordComparison));

            int summaryHeaderOffset = ScoreHeaderCount - Perference.PrintItems.Count;
            foreach (string each in Perference.PrintItems)
            {
                columnHeaders[0, summaryHeaderOffset].PutValue(each);
                summaryHeaderOffset++;
            }

            foreach (JHClassRecord each in Classes)
            {
                OutputSheet.Cells.CreateRange(ClassOffset, RowNumber, false).Copy(all);

                //轉換成 ReportStudent。
                List<ReportStudent> pageStudent = GetPreparedStudent(each.Students);
                pageStudent.Sort(delegate(ReportStudent x, ReportStudent y)
                {//將學生按座號排序。
                    return x.OrderSeatNo.CompareTo(y.OrderSeatNo);
                });

                //描述要印出來的科目、領域有哪些。
                ScoreHeaderIndexer headers = new ScoreHeaderIndexer();
                foreach (ReportStudent eachStu in pageStudent)
                {
                    foreach (string eachSubj in SelectedSubject)
                    {
                        if (eachStu.Scores[Utilities.SubjectToken].Contains(eachSubj))
                        {
                            if (!headers.Contains(eachSubj, false)) // eachStu.Scores[Utilities.SubjectToken].Weights[eachSubj]))
                                headers.Add(eachSubj, false, eachStu.Scores[Utilities.SubjectToken].Weights[eachSubj]);
                        }
                    }

                    foreach (string eachDomain in SelectedDomain)
                    {
                        if (eachStu.Scores[Utilities.DomainToken].Contains(eachDomain))
                        {
                            if (!headers.Contains(eachDomain, true)) //eachStu.Scores[Utilities.DomainToken].Weights[eachDomain]))
                                headers.Add(eachDomain, true, eachStu.Scores[Utilities.DomainToken].Weights[eachDomain]);
                        }
                    }
                }

                headers.Sort(SubjectDomainMap); //排序資料。

                //填入科目、領域資料。
                RowOffset = columnHeaders.FirstRow + ClassOffset;
                ColumnOffset = columnHeaders.FirstColumn;
                foreach (Header eachHeacher in headers)
                {
                    if (eachHeacher.IsDomain)
                        OutputSheet.Cells[RowOffset, ColumnOffset].Style.Font.IsBold = true;

                    OutputSheet.Cells[RowOffset, ColumnOffset].PutValue(eachHeacher.Name);
                    OutputSheet.Cells[RowOffset + 1, ColumnOffset].PutValue(eachHeacher.GetDisplayCredit());
                    eachHeacher.ColumnIndex = ColumnOffset;
                    ColumnOffset++;
                }

                //填入學生清單、成績與排名資料。
                RowOffset = rowHeaders.FirstRow + ClassOffset;
                foreach (ReportStudent eachStudent in pageStudent)
                {
                    OutputSheet.Cells[RowOffset, 0].PutValue(eachStudent.SeatNo);
                    OutputSheet.Cells[RowOffset, 1].PutValue(eachStudent.Name);

                    //填入科目成績。
                    foreach (string eachSubj in SelectedSubject)
                    {
                        ScoreCollection scores = eachStudent.Scores[Utilities.SubjectToken];

                        if (scores.Contains(eachSubj) && headers.Contains(eachSubj, false)) // scores.Weights[eachSubj]))
                        {
                            int headerIndex = headers[eachSubj, false].ColumnIndex;
                            OutputSheet.Cells[RowOffset, headerIndex].PutValue((double)scores[eachSubj]);
                        }
                    }

                    //填入領域成績。
                    foreach (string eachDomain in SelectedDomain)
                    {
                        ScoreCollection scores = eachStudent.Scores[Utilities.DomainToken];

                        if (scores.Contains(eachDomain) && headers.Contains(eachDomain, true))// scores.Weights[eachDomain]))
                        {
                            int headerIndex = headers[eachDomain, true].ColumnIndex;
                            double value = 0;
                            string ruleID = GetScoreCalcRuleID(JHStudent.SelectByID(eachStudent.Id));
                            if (string.IsNullOrEmpty(ruleID))
                                value = (double)DefaultCalc.ParseDomainScore(scores[eachDomain]);
                            else if (Calcs.ContainsKey(ruleID))
                                value = (double)Calcs[ruleID].ParseDomainScore(scores[eachDomain]);
                            OutputSheet.Cells[RowOffset, headerIndex].PutValue(value);
                        }
                    }

                    //填入運算成績。
                    int summaryDataOffset = (columnHeaders.FirstColumn + ScoreHeaderCount) - Perference.PrintItems.Count;
                    ScoreCollection summaryScores = eachStudent.Scores[Utilities.SummaryToken];
                    foreach (string eachSummary in Perference.PrintItems)
                    {
                        if (summaryScores.Contains(eachSummary))
                            // 當0分布顯示
                            if (summaryScores[eachSummary] <= 0)
                                OutputSheet.Cells[RowOffset, summaryDataOffset].PutValue("");
                            else
                                OutputSheet.Cells[RowOffset, summaryDataOffset].PutValue(summaryScores[eachSummary]);
                        summaryDataOffset++;
                    }

                    //填入排名資料。
                    string rm = Perference.RankMethod; //排名依據。
                    if (eachStudent.Places.NS("班排名").Contains(rm))
                        OutputSheet.Cells[RowOffset, rankColumnHeader.FirstColumn].PutValue(eachStudent.Places.NS("班排名")[rm].Level);
                    if (eachStudent.Places.NS("年排名").Contains(rm))
                        OutputSheet.Cells[RowOffset, rankColumnHeader.FirstColumn + 1].PutValue(eachStudent.Places.NS("年排名")[rm].Level);

                    RowOffset++;
                    //if ((RowOffset - (rowHeaders.FirstRow + ClassOffset)) >= 45) break; //超過45個學生就不印了。
                }

                #region 填入標題及回條
                //Ex. 新竹市立光華國民中學 97 學年度第 1 學期    101  第1次平時評量成績單
                OutputSheet.Cells[title.FirstRow + ClassOffset, title.FirstColumn].PutValue(string.Format("{0} {1}學年度第{2}學期 {4} {3} 班級評量成績單", SchoolName, SchoolYear, Semester, ExamName, each.Name));
                //Ex. 101 第1次平時評量回條 (家長意見欄)
                //OutputSheet.Cells[feedback.FirstRow + ClassOffset, feedback.FirstColumn].PutValue(string.Format("{0} 班級評量成績單回條  (家長意見欄)", each.Name));
                #endregion

                ClassOffset += RowNumber;
                OutputSheet.HPageBreaks.Add(ClassOffset, 0);
            }

            //OutputSheet.Cells.HideColumn((byte)rankColumnHeader.FirstColumn - 1);

            //Utilities.Save(OutputBook, "班級學期成績單.xls");
            return OutputBook;
        }

        private string GetScoreCalcRuleID(JHStudentRecord student)
        {
            if (!string.IsNullOrEmpty(student.OverrideScoreCalcRuleID))
                return student.OverrideScoreCalcRuleID;
            else if (student.Class != null)
                return student.Class.RefScoreCalcRuleID;
            else
                return string.Empty;
        }

        private Stream GetTemplateStream()
        {
            // 超過45人
            if (_MaxClassStudCot > 45)
            {
                if (Perference.PaperSize == "B4")
                    return new MemoryStream(Prc.班級評量成績單60B4);
                else
                    return new MemoryStream(Prc.班級評量成績單60);            
            }
            else
            {
                if (Perference.PaperSize == "B4")
                    return new MemoryStream(Prc.班級評量成績單B4);
                else
                    return new MemoryStream(Prc.班級評量成績單);
            }
        }

        private List<ReportStudent> GetPreparedStudent(IEnumerable<JHStudentRecord> students)
        {
            List<ReportStudent> result = new List<ReportStudent>();
            foreach (JHStudentRecord each in students)
            {
                if (AllStudents.ContainsKey(each.ID))
                    result.Add(AllStudents[each.ID]);
            }
            return result;
        }
    }
}
