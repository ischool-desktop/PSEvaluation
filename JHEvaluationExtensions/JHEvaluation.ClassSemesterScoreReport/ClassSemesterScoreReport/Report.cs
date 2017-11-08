using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;
using Aspose.Cells;
using System.IO;

namespace JHEvaluation.ClassSemesterScoreReport
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

        //public const int ScoreHeaderCount = 18;

        public Report()
        {
            Template = new Workbook();
            //Template.Open(new MemoryStream(Prc.班級學期成績單));
        }

        private Stream GetTemplateStream()
        {

            if (_MaxClassStudCot > 45)
            {
                if (Perference.PaperSize == "B4")
                    return new MemoryStream(Prc.班級學期成績單B4_60);
                else
                    return new MemoryStream(Prc.班級學期成績單60);
            }
            else
            {
                if (Perference.PaperSize == "B4")
                    return new MemoryStream(Prc.班級學期成績單B4);
                else
                    return new MemoryStream(Prc.班級學期成績單);
            
            }
        }

        public Workbook Output()
        {
            // 取得班級最大人數
            _MaxClassStudCot = 0;

            // 取得一般與輟學人數
            Dictionary<string, int> classStudCount = Utility.GetClassStudentCount18();

            // 取得所選班級最大人數
            foreach (JHClassRecord classRec in Classes)
                if (classStudCount.ContainsKey(classRec.ID))
                    if (classStudCount[classRec.ID] >= _MaxClassStudCot)
                        _MaxClassStudCot = classStudCount[classRec.ID];


            OutputBook = new Workbook();
            OutputBook.Open(GetTemplateStream());
            OutputSheet = OutputBook.Worksheets[0];

            Template.Open(GetTemplateStream());

            int ScoreHeaderCount = (Perference.PaperSize == "B4") ? 32 : 18;

            int ClassOffset = 0; //整個班級的 Offset。
            int RowOffset = 0;
            int ColumnOffset = 0;
            int AvgRowOffset = 0;

            // 計算平均與及格人數用
            Dictionary<string, decimal> ClassSubjSumScoreDict = new Dictionary<string, decimal>();
            Dictionary<string, decimal> ClassDomainSumScoreDict = new Dictionary<string, decimal>();
            Dictionary<string, decimal> ClassSubjPeopleDict = new Dictionary<string, decimal>();
            Dictionary<string, decimal> ClassDomainPeopleDict = new Dictionary<string, decimal>();
            Dictionary<string, int> ClassSubjPassDict = new Dictionary<string, int>();
            Dictionary<string, int> ClassDomainPassDict = new Dictionary<string, int>();

            Range all = Template.Worksheets.GetRangeByName("All");

            Range printDate = Template.Worksheets.GetRangeByName("PrintDate");
            printDate[0, 0].PutValue(DateTime.Today.ToString("yyyy/MM/dd"));

            Range title = Template.Worksheets.GetRangeByName("Title");
            Range feedback = Template.Worksheets.GetRangeByName("Feedback");
            Range rowHeaders = Template.Worksheets.GetRangeByName("RowHeaders");
            Range columnHeaders = Template.Worksheets.GetRangeByName("ColumnHeaders");
            Range rankColumnHeader = Template.Worksheets.GetRangeByName("RankColumnHeader");
            Range AverageRowHeader = Template.Worksheets.GetRangeByName("AverageRowHeaders");

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

                ClassDomainPassDict.Clear();
                ClassDomainPeopleDict.Clear();
                ClassDomainSumScoreDict.Clear();
                ClassSubjPassDict.Clear();
                ClassSubjPeopleDict.Clear();
                ClassSubjSumScoreDict.Clear();

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
                AvgRowOffset = AverageRowHeader.FirstRow + ClassOffset;
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

                            decimal ss=(decimal)scores[eachSubj];
                            decimal rss = scores.GetReExamScore(eachSubj);

                            OutputSheet.Cells[RowOffset, headerIndex].PutValue(ss);

                            if (Perference.UserSelScoreType == "原始補考擇優")
                                if (rss >= ss)
                                    OutputSheet.Cells[RowOffset, headerIndex].PutValue(Perference.ReScoreMark+ss);
                            

                            // 科目平均與及格人數
                            if (!ClassSubjSumScoreDict.ContainsKey(eachSubj))
                                ClassSubjSumScoreDict.Add(eachSubj, 0);

                            if (!ClassSubjPeopleDict.ContainsKey(eachSubj))
                                ClassSubjPeopleDict.Add(eachSubj, 0);

                            if (!ClassSubjPassDict.ContainsKey(eachSubj))
                                ClassSubjPassDict.Add(eachSubj, 0);

                            ClassSubjSumScoreDict[eachSubj] += ss;
                            ClassSubjPeopleDict[eachSubj]++;
                            if (ss >= 60)
                                ClassSubjPassDict[eachSubj]++;
                        }
                    }

                    //填入領域成績。
                    foreach (string eachDomain in SelectedDomain)
                    {
                        ScoreCollection scores = eachStudent.Scores[Utilities.DomainToken];

                        if (scores.Contains(eachDomain) && headers.Contains(eachDomain, true))// scores.Weights[eachDomain]))
                        {
                            int headerIndex = headers[eachDomain, true].ColumnIndex;
                            
                            decimal dd =(decimal)scores[eachDomain];
                            decimal rdd = scores.GetReExamScore(eachDomain);

                            OutputSheet.Cells[RowOffset, headerIndex].PutValue(dd);

                            if (Perference.UserSelScoreType == "原始補考擇優")
                                if (rdd >= dd)
                                    OutputSheet.Cells[RowOffset, headerIndex].PutValue(Perference.ReScoreMark + dd);
                                                       

                            // 領域平均與及格人數
                            if (!ClassDomainSumScoreDict.ContainsKey(eachDomain))
                                ClassDomainSumScoreDict.Add(eachDomain, 0);

                            if (!ClassDomainPeopleDict.ContainsKey(eachDomain))
                                ClassDomainPeopleDict.Add(eachDomain, 0);

                            if (!ClassDomainPassDict.ContainsKey(eachDomain))
                                ClassDomainPassDict.Add(eachDomain, 0);

                            ClassDomainSumScoreDict[eachDomain] += dd;
                            ClassDomainPeopleDict[eachDomain]++;
                            if (dd >= 60)
                                ClassDomainPassDict[eachDomain]++;
                        }
                    }

                    //填入運算成績。
                    int summaryDataOffset = (columnHeaders.FirstColumn + ScoreHeaderCount) - Perference.PrintItems.Count;
                    ScoreCollection summaryScores = eachStudent.Scores[Utilities.SummaryToken];
                    foreach (string eachSummary in Perference.PrintItems)
                    {
                        if (summaryScores.Contains(eachSummary))
                        {
                            decimal su = summaryScores[eachSummary];
                            OutputSheet.Cells[RowOffset, summaryDataOffset].PutValue(su);

                            if (!ClassDomainSumScoreDict.ContainsKey(eachSummary))
                                ClassDomainSumScoreDict.Add(eachSummary, 0);

                            if (!ClassDomainPeopleDict.ContainsKey(eachSummary))
                                ClassDomainPeopleDict.Add(eachSummary, 0);

                            if (!ClassDomainPassDict.ContainsKey(eachSummary))
                                ClassDomainPassDict.Add(eachSummary, 0);

                            ClassDomainSumScoreDict[eachSummary] += su;
                            ClassDomainPeopleDict[eachSummary]++;
                            if (su >= 60)
                                ClassDomainPassDict[eachSummary]++;
                        }
                        summaryDataOffset++;
                    }

                    //填入排名資料。
                    string rm = Perference.RankMethod; //排名依據。
                    if (eachStudent.Places.NS("班排名").Contains(rm))
                        OutputSheet.Cells[RowOffset, rankColumnHeader.FirstColumn].PutValue(eachStudent.Places.NS("班排名")[rm].Level);
                    if (eachStudent.Places.NS("年排名").Contains(rm))
                        OutputSheet.Cells[RowOffset, rankColumnHeader.FirstColumn + 1].PutValue(eachStudent.Places.NS("年排名")[rm].Level);

                    RowOffset++;
//                    if ((RowOffset - (rowHeaders.FirstRow + ClassOffset)) >= 45) break; //超過45個學生就不印了。
                }


                #region 填入平均與人數

                // 科目及格人數
                foreach (string key in ClassSubjPassDict.Keys)
                {
                    if (headers.Contains(key, false))
                    {
                        int headerIndex = headers[key, false].ColumnIndex;
                        OutputSheet.Cells[AvgRowOffset + 1, headerIndex].PutValue(ClassSubjPassDict[key]);
                    }
                }
                // 科目平均
                foreach(string key in ClassSubjSumScoreDict.Keys)
                {
                    if (headers.Contains(key, false))
                    {
                        int headerIndex = headers[key, false].ColumnIndex;
                        decimal ss = ClassSubjSumScoreDict[key] / ClassSubjPeopleDict[key];
                        OutputSheet.Cells[AvgRowOffset, headerIndex].PutValue(ss);
                    }
                }

                // 領域及格人數
                foreach (string key in ClassDomainPassDict.Keys)
                {
                    if (headers.Contains(key, true))
                    {
                        int headerIndex = headers[key, true].ColumnIndex;
                        OutputSheet.Cells[AvgRowOffset + 1, headerIndex].PutValue(ClassDomainPassDict[key]);
                    }
                }
                // 領域平均
                foreach (string key in ClassDomainSumScoreDict.Keys)
                {
                    if (headers.Contains(key, true))
                    {
                        int headerIndex = headers[key, true].ColumnIndex;
                        decimal dd = ClassDomainSumScoreDict[key] / ClassDomainPeopleDict[key];
                        OutputSheet.Cells[AvgRowOffset, headerIndex].PutValue(dd);
                    }
                }

                int suDataOffset = (columnHeaders.FirstColumn + ScoreHeaderCount) - Perference.PrintItems.Count;
                // 總分平均,人數
                foreach (string eachSummary in Perference.PrintItems)
                {                    
                    
                    if(ClassDomainSumScoreDict.ContainsKey(eachSummary))
                    {
                        decimal dd = ClassDomainSumScoreDict[eachSummary] / ClassDomainPeopleDict[eachSummary];
                        OutputSheet.Cells[AvgRowOffset, suDataOffset].PutValue(dd);
                    }
                    
                    if(ClassDomainPassDict.ContainsKey(eachSummary))
                        OutputSheet.Cells[AvgRowOffset + 1, suDataOffset].PutValue(ClassDomainPassDict[eachSummary]);
                    suDataOffset++;
                }

                #endregion

                #region 填入標題及回條
                //Ex. 新竹市立光華國民中學 97 學年度第 1 學期    101  第1次平時評量成績單
                OutputSheet.Cells[title.FirstRow + ClassOffset, title.FirstColumn].PutValue(string.Format("{0} {1}學年度第{2}學期 {3} 班級學期成績單", SchoolName, SchoolYear, Semester, each.Name));
                //Ex. 101 第1次平時評量回條 (家長意見欄)
                OutputSheet.Cells[feedback.FirstRow + ClassOffset, feedback.FirstColumn].PutValue(string.Format("{0} 班級學期成績單回條  (家長意見欄)", each.Name));
                #endregion

                ClassOffset += RowNumber;
                OutputSheet.HPageBreaks.Add(ClassOffset, 0);
            }

            //OutputSheet.Cells.HideColumn((byte)rankColumnHeader.FirstColumn - 1);

            //Utilities.Save(OutputBook, "班級學期成績單.xls");
            return OutputBook;
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
