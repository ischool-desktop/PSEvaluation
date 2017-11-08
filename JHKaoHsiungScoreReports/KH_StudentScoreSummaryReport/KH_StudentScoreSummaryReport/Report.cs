using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Aspose.Words;
using Aspose.Words.Reporting;
using Campus.Rating;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using JHEvaluation.ScoreCalculation;
using JHEvaluation.ScoreCalculation.ScoreStruct;
using JHSchool.Data;
using Subj = JHSchool.Evaluation.Subject;

namespace KH_StudentScoreSummaryReport
{
    internal class Report
    {
        private ReportPreference PrintSetting { get; set; }

        private List<ReportStudent> Students { get; set; }

        private Dictionary<string, List<string>> PrintAbsences { get; set; }

        private List<string> DetailDomain { get; set; }

        ///// <summary>
        ///// 是否產生匯入檔
        ///// </summary>
        //public static bool CheckExportFile = false;

        // 學生名次與名次百分比(UDT)
        private Dictionary<string, List<UserDefData>> _StudRankData;

        private const string LearningDomainName = "學期成績平均";
        private const string LearningDomainNameSpcAdd = "特種身分加分後之學期成績平均";
        private const string LearningDomainNameP = "學期成績平均排名/百分比";
        private const string LearningDomainNameSpcAddP = "特種身分加分後之學期成績平均排名/百分比";
        private const string CalcMessage = "樂學計分";
        private const string StudSpcName1 = "原住民(持文化及語言能力證明)";
        private const string StudSpcName2 = "原住民(未持文化及語言能力證明)";
        private const string StudSpcName3 = "境外優秀科學技術人才子女";
        private const string StudSpcName4 = "政府派赴國外工作人員子女";
        private const string StudSpcName5 = "蒙藏生";
        private const string StudSpcName6 = "身心障礙生";
        private const string StudSpcName7 = "資賦優異縮短修業年限學生";
        private const string StudSpcName8 = "回國僑生";
        private const string StudSpcName9 = "港澳生";

        // 服務學習
        Dictionary<string, List<SLRecord>> _SLRecordDict = new Dictionary<string, List<SLRecord>>();

        // 幹部紀錄統計
        Dictionary<string, List<itemCount>> _itemCountDict = new Dictionary<string, List<itemCount>>();

        // 獎勵明細
        Dictionary<string, List<JHMeritRecord>> _JHMeritRecordDict = new Dictionary<string, List<JHMeritRecord>>();

        // 體適能
        Dictionary<string, List<StudentFitnessRecord>> _StudentFitnessRecordDict = new Dictionary<string, List<StudentFitnessRecord>>();

        public Report(List<ReportStudent> students, ReportPreference printSetting)
        {
            Students = students;
            Students.Sort(delegate(ReportStudent x, ReportStudent y)
            {
                return x.OrderString.CompareTo(y.OrderString);
            });
            List<string> studenIDList = new List<string>();
            foreach (ReportStudent stud in students)
                studenIDList.Add(stud.StudentID);

            // 取得學生服務學習
            _SLRecordDict = Util.GetStudentSLRecordDictByStudentIDList(studenIDList);

            // 取得學生幹部紀錄
            _itemCountDict = Util.GetStudentCount1ByStudentIDList(studenIDList);

            // 取得獎勵紀錄
            _JHMeritRecordDict = Util.GetStudentMeritRecordDict(studenIDList);

            // 取得體適能
            _StudentFitnessRecordDict = Util.GetStudentFitnessRecordDictByStudentIDList(studenIDList);

            //載入積分資料。
            AccessHelper access = new AccessHelper();
            string sql = string.Format("ref_student_id in ({0})", students.ToPrimaryKeyStringList());
            List<ExcessCredits> credits = access.Select<ExcessCredits>(sql);
            Dictionary<string, ReportStudent> studentLookup = Students.ToDictionary(x => x.StudentID);
            foreach (ExcessCredits credit in credits)
            {
                if (!studentLookup.ContainsKey(credit.StudentID + ""))
                    continue;

                ReportStudent rs = studentLookup[credit.StudentID + ""];

                rs.CreditDomainScore = credit.Balanced;
                rs.CreditServiceLearning = credit.Services;
                rs.CreditFitness = credit.Fitness;
                rs.CreditCadre = credit.Term;
            }

            PrintSetting = printSetting;

            DetailDomain = new List<string>();

            DetailDomain.Add("彈性課程");
            DetailDomain.Add("");

            PrintAbsences = printSetting.AcceptAbsences.PeriodOptionsFromString();

            // 取得學生ID
            List<string> sidList = (from xx in students select xx.StudentID).ToList();
            // 取得學生放在UDT的排名資料
            _StudRankData = UDTTransfer.GetDataFromUDTDict(sidList);

        }


        public Document Print()
        {
            Document doc = PrintSetting.Template.ToDocument();

            doc.MailMerge.MergeField += new Aspose.Words.Reporting.MergeFieldEventHandler(MailMerge_MergeField);

            doc.MailMerge.Execute(new MergeDataSource(Students, PrintSetting));

            // 上傳電子報表
            if(PrintSetting.isUploadEPaper)
            {
                try
                {
                    TempData._ePaperMemStreamList.Clear();
                    foreach (Section section in doc.Sections)
                    {
                        Document document = new Document();
                        document.Sections.Clear();
                        document.Sections.Add(document.ImportNode(section, true));
                        MemoryStream ms = new MemoryStream();
                        document.Save(ms, SaveFormat.Doc);
                        TempData._ePaperMemStreamList.Add(ms);
                    }
                    
                }catch(Exception ex)
                {
                    
                }
                
            }

            return doc;
        }

        //最好這程式有人能維護的了.......
        private void MailMerge_MergeField(object sender, MergeFieldEventArgs e)
        {
            //不是 Fix 開頭的合併欄位不處理。
            //不是「積分:」開頭的合併欄位不處理。
            if (!e.FieldName.ToUpper().StartsWith("Fix".ToUpper()))
                return;

            DocumentBuilder builder = new DocumentBuilder(e.Document);
            ReportStudent student = e.FieldValue as ReportStudent;

            //如果合併值不是 ReportStudent 就跳過...意思是有問題...。
            if (student == null) return;

            builder.MoveToField(e.Field, true);
            e.Field.Remove();

            if (e.FieldName == "Fix:年級學期")
            {
                #region 列印年級學期資訊(有點複雜)。
                SemesterDataCollection semesters = student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters);
                Row SemesterRow = builder.CurrentParagraph.ParentNode.ParentNode.NextSibling as Row; //下一個 Row。
                Paragraph originParagraph = builder.CurrentParagraph;

                int count = 0, offset = 1;

                foreach (SemesterData each in semesters)
                {
                    string currentGradeyear = Util.GetGradeyearString(each.GradeYear.ToString());

                    //如果沒有年級，就跳過。
                    if (string.IsNullOrEmpty(currentGradeyear)) continue;

                    builder.Write(currentGradeyear + "年級");
                    Paragraph nextPh = Util.NextCell(builder.CurrentParagraph);
                    if (nextPh == null) break; //沒有下一個 Cell ，就不印資料了。
                    builder.MoveTo(nextPh);

                    Paragraph resetParagraph = builder.CurrentParagraph;
                    SemesterRow.Cells[count + offset].Write(builder, GetSemesterString(each));

                    SemesterData semester = new SemesterData(0, each.SchoolYear, each.Semester);
                    if (!student.HeaderList.ContainsKey(semester))
                        student.HeaderList.AddRaw(each, count); //不要懷疑，這是對的。

                    builder.MoveTo(resetParagraph);
                    count++;
                }

                // 假如學年度學期非2的倍數, 在最後補上x年級
                if (semesters.Count % 2 != 0)
                    builder.Write(Util.GetGradeyearString(semesters[semesters.Count - 1].GradeYear.ToString()) + "年級");

                builder.MoveTo(originParagraph);
                (originParagraph.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                Paragraph nextParagrap = originParagraph;
                string previousGradeyear = GetText(originParagraph);
                while ((nextParagrap = Util.NextCell(nextParagrap)) != null)
                {
                    if (GetText(nextParagrap) == previousGradeyear && !string.IsNullOrEmpty(previousGradeyear))
                    {
                        (nextParagrap.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;
                        (nextParagrap.ParentNode as Cell).Paragraphs[0].Runs.Clear();
                    }
                    else
                        (nextParagrap.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;

                    previousGradeyear = GetText(nextParagrap);
                }
                #endregion
            }
            else if (e.FieldName == "Fix:科目資訊")
            {
                #region 列印科目資料(爆炸複雜)

                Row template = builder.CurrentParagraph.ParentNode.ParentNode as Row;
                Table table = template.ParentNode as Table;


                if (PrintSetting.ListMethod == ListMethod.DomainOnly)
                {
                    #region 列印領域
                    UniqueSet<RowHeader> RowIndexs = new UniqueSet<RowHeader>();
                    //Environment.OSVersion.Platform
                    #region 列印 RowHeader
                    foreach (SemesterData semester in student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters))
                    {
                        SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);

                        //如果不包含該學期成績資料，就跳過。
                        if (!student.SemestersScore.Contains(sysems)) continue;

                        SemesterScore semsscore = student.SemestersScore[sysems];

                        ////準備彈性課程的科目(要詳列出來)。
                        //foreach (string strSubject in semsscore.Subject)
                        //{
                        //    SemesterSubjectScore subject = semsscore.Subject[strSubject];

                        //    if (DetailDomain.Contains(subject.Domain))
                        //    {
                        //        RefineDomain(subject);

                        //        RowHeader header = new RowHeader(subject.Domain, strSubject);
                        //        header.IsDomain = false;

                        //        if (!RowIndexs.Contains(header))
                        //            RowIndexs.Add(header);
                        //    }
                        //}
                        List<string> ddName = new List<string>();
                        ddName.Add("藝術與人文");
                        ddName.Add("健康與體育");
                        ddName.Add("綜合活動");
                        //準備領域資料。
                        foreach (string strDomain in semsscore.Domain)
                        {
                            if (!ddName.Contains(strDomain))
                                continue;

                            if (!Subj.Domains.Contains(strDomain)) continue;

                            SemesterDomainScore domain = semsscore.Domain[strDomain];

                            if (!DetailDomain.Contains(strDomain))
                            {
                                RowHeader header = new RowHeader(strDomain, string.Empty);
                                header.IsDomain = true;

                                if (!RowIndexs.Contains(header))
                                    RowIndexs.Add(header);
                            }
                        }
                    }

                    //RowHeader lheader = new RowHeader(LearningDomainName, string.Empty);
                    //RowHeader lheader1 = new RowHeader(LearningDomainNameSpcAdd, string.Empty);
                    //RowHeader lheader2 = new RowHeader(LearningDomainNameP, string.Empty);
                    //RowHeader lheader3 = new RowHeader(LearningDomainNameSpcAddP, string.Empty);
                    //RowHeader lheader4 = new RowHeader(CalcMessage, string.Empty);

                    //lheader.IsDomain = true;
                    //lheader1.IsDomain = true;
                    //lheader2.IsDomain = true;
                    //lheader3.IsDomain = true;
                    //lheader4.IsDomain = true;
                    //RowIndexs.Add(lheader);
                    //RowIndexs.Add(lheader1);
                    //RowIndexs.Add(lheader2);
                    //RowIndexs.Add(lheader3);
                    //RowIndexs.Add(lheader4);

                    List<RowHeader> sortedHeaders = SortHeader(RowIndexs.ToList());

                    //產生 Row。
                    List<RowHeader> indexHeaders = new List<RowHeader>();
                    Row current = template;
                    int rowIndex = 0;
                    foreach (RowHeader header in sortedHeaders)
                    {
                        RowHeader indexH = header;
                        indexH.Index = rowIndex++;
                        indexHeaders.Add(indexH);
                        bool hasGroup = !string.IsNullOrEmpty(Subj.GetDomainGroup(header.Domain));
                        string groupName = Subj.GetDomainGroup(header.Domain);

                        Row datarow = table.InsertBefore(template.Clone(true), current) as Row;


                        if (header.IsDomain)
                        {
                            if (hasGroup)
                            {
                                datarow.Cells[1].Write(builder, groupName);
                                datarow.Cells[2].Write(builder, header.Domain);
                            }
                            else
                            {
                                datarow.Cells[1].Write(builder, header.Domain);
                            }
                        }
                        else
                        {
                            //if (header.Domain == LearningDomainNameP)
                            //{
                            //    string headerName = string.Empty;

                            //    if (PrintSetting.PrintRank)
                            //        headerName = "學期成績平均排名";

                            //    if (PrintSetting.PrintRankPercentage)
                            //        headerName = "學期成績平均百分比";

                            //    if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                            //        headerName = "學期成績平均排名/百分比";

                            //    if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                            //    {
                            //        Row lrow = table.InsertAfter(template.Clone(true), datarow) as Row;
                            //        lrow.Cells[0].Write(builder, headerName);
                            //        rowIndex++;
                            //    }
                            //}

                            //if (header.Domain == LearningDomainNameSpcAddP)
                            //{
                            //    string headerName = string.Empty;

                            //    if (PrintSetting.PrintRank)
                            //        headerName = "特種身分加分後之學期成績平均排名";

                            //    if (PrintSetting.PrintRankPercentage)
                            //        headerName = "特種身分加分後之學期成績平均百分比";

                            //    if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                            //        headerName = "特種身分加分後之學期成績平均排名/百分比";

                            //    if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                            //    {
                            //        Row lrow = table.InsertAfter(template.Clone(true), datarow) as Row;
                            //        lrow = table.InsertAfter(template.Clone(true), datarow) as Row;
                            //        lrow.Cells[0].Write(builder, headerName);
                            //        rowIndex++;
                            //    }
                            //}

                            ////把空白的領域當成「彈性課程」。
                            //string domain = IsFlexible(header.Domain) ? "彈性課程" : header.Domain;

                            //// 修改不需要彈性課程，標頭
                            //datarow.Cells[0].Write(builder, "^^");
                            //datarow.Cells[1].Write(builder, "^_^");
                        }
                    }
                    #endregion

                    #region 填資料
                    Row RatingRow = null;
                    // 用在加分後
                    Row RatingRowAdd = null;
                    // 是否已有年排名資料
                    List<UserDefData> uddList = new List<UserDefData>();
                    if (_StudRankData.ContainsKey(student.StudentID))
                        uddList = _StudRankData[student.StudentID];

                    foreach (RowHeader header in indexHeaders)
                    {
                        SemesterDataCollection semesters = new SemesterDataCollection();
                        Row row = table.Rows[header.Index + 3];
                        foreach (SemesterData semester in student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters))
                        {
                            //避開三年級下學期
                            if ((semester.GradeYear == 3 || semester.GradeYear == 9) && semester.Semester == 2)
                                continue;

                            SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);
                            semesters.Add(sysems);

                            if (!student.SemestersScore.Contains(sysems)) continue;
                            if (!student.HeaderList.ContainsKey(sysems)) continue;

                            int columnIndex = student.HeaderList[sysems];
                            SemesterScore semsscore = student.SemestersScore[sysems];

                            decimal? score = null;
                            decimal? weight = null;

                            if (header.IsDomain)
                            {
                                if (semsscore.Domain.Contains(header.Domain))
                                {
                                    score = semsscore.Domain[header.Domain].Value;
                                    weight = semsscore.Domain[header.Domain].Weight;
                                }
                            }
                            else
                            {
                                // 這段在顯示彈性課程成績，先註。
                                //if (semsscore.Subject.Contains(header.Subject))
                                //{
                                //    score = semsscore.Subject[header.Subject].Value;
                                //    weight = semsscore.Subject[header.Subject].Weight;
                                //}
                            }

                            if (header.Domain == LearningDomainName)
                            {
                                score = semsscore.LearnDomainScore;

                                row.Cells[columnIndex * 3 + 3].CellFormat.FitText = false;
                                row.Cells[columnIndex * 3 + 3].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[columnIndex * 3 + 4].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (!score.HasValue) continue;

                                row.Cells[columnIndex * 3 + 3].Write(builder, ((double)score) + "");
                                row.Cells[columnIndex * 3 + 5].Write(builder, (Util.GetDegree(score.Value)));

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                {
                                    RatingRow = row.NextSibling as Row;
                                    RatingRow = RatingRow.NextSibling as Row;
                                }
                            }
                            else if (header.Domain == LearningDomainNameSpcAdd)
                            {
                                // 加分後填入值
                                score = semsscore.LearnDomainScore;

                                row.Cells[columnIndex * 3 + 2].CellFormat.FitText = false;
                                row.Cells[columnIndex * 3 + 2].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[columnIndex * 3 + 3].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (!score.HasValue) continue;
                                // 處理加分
                                decimal sc = score.Value;
                                if (student.AddWeight.HasValue)
                                    sc = Math.Round(sc * student.AddWeight.Value, 2, MidpointRounding.AwayFromZero); ;

                                // 一般生填空白
                                if (string.IsNullOrEmpty(student.SpcStudTypeName))
                                {
                                    row.Cells[columnIndex * 3 + 2].Write(builder, "");
                                    row.Cells[columnIndex * 3 + 4].Write(builder, "");
                                }
                                else
                                {
                                    // 特種身
                                    row.Cells[columnIndex * 3 + 2].Write(builder, sc + "");
                                    row.Cells[columnIndex * 3 + 4].Write(builder, (Util.GetDegree(sc)));
                                }

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                {
                                    if (RatingRow != null)
                                    {
                                        RatingRowAdd = RatingRow.NextSibling as Row;


                                    }
                                }
                            }
                            else
                            {
                                if (!score.HasValue) continue;
                                if (!weight.HasValue) weight = 0;

                                row.Cells[columnIndex * 3 + 3].Write(builder, ((double)weight) + "");
                                row.Cells[columnIndex * 3 + 4].Write(builder, ((double)score) + "");
                                row.Cells[columnIndex * 3 + 5].Write(builder, (Util.GetDegree(score.Value)));
                            }
                        }

                        //算平均...
                        decimal? avgScore = null;
                        if (header.IsDomain)
                        {
                            if (header.Domain == LearningDomainName)
                            {
                                avgScore = student.SemestersScore.AvgLearningDomainScore(semesters);

                                if (!avgScore.HasValue) continue;

                                if (student.CalculationRule == null)
                                    avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                                else
                                    avgScore = student.CalculationRule.ParseLearnDomainScore(avgScore.Value);
                            }
                            else
                            {
                                avgScore = student.SemestersScore.AvgDomainScore(semesters, header.Domain);

                                if (!avgScore.HasValue) continue;

                                if (student.CalculationRule == null)
                                    avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                                else
                                    avgScore = student.CalculationRule.ParseDomainScore(avgScore.Value);
                            }
                        }
                        else
                        {
                            // 這段在處理彈性課程平均，先註。
                            //avgScore = student.SemestersScore.AvgSubjectScore(semesters, header.Subject);

                            //if (!avgScore.HasValue) continue;

                            //if (student.CalculationRule == null)
                            //    avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                            //else
                            //    avgScore = student.CalculationRule.ParseSubjectScore(avgScore.Value);
                        }

                        if (avgScore.HasValue)
                        {
                            row.Cells[21].Write(builder, (double)avgScore + "");
                            row.Cells[22].Write(builder, Util.GetDegree(avgScore.Value));
                            //decimal scAddScore = 0;
                            //// 特種身分
                            //if (student.AddWeight.HasValue)
                            //{
                            //    Row RowSp = row.NextSibling as Row;
                            //    scAddScore = student.AddWeight.Value * avgScore.Value;
                            //    RowSp.Cells[20].Write(builder, (double)scAddScore + "");
                            //    RowSp.Cells[21].Write(builder, Util.GetDegree(scAddScore));                               

                            //}
                        }
                    }

                    // 處理特種分身平均顯示
                    decimal scAddScore = 0;
                    // 特種身分
                    int rrIdx = 0;
                    foreach (RowHeader rh in indexHeaders)
                    {
                        if (rh.Domain == LearningDomainNameSpcAdd)
                        {
                            rrIdx = rh.Index + 3;
                            break;
                        }
                    }

                    if (student.AddWeight.HasValue && rrIdx > 0)
                    {
                        // 顯示平均
                        if (student.Places.NS("年排名").Contains("學習領域"))
                        {
                            scAddScore = Math.Round(student.Places.NS("年排名")["學習領域"].Score * student.AddWeight.Value, 2, MidpointRounding.AwayFromZero);
                            table.Rows[rrIdx].Cells[21].Write(builder, (double)scAddScore + "");
                            table.Rows[rrIdx].Cells[22].Write(builder, Util.GetDegree(scAddScore));
                        }


                    }

                    // 處理年排名與百分比
                    if (RatingRow != null)
                    {
                        PlaceCollection places = student.Places.NS("年排名");

                        foreach (SemesterData semsIndex in student.HeaderList.Keys)
                        {
                            SemesterData raw = student.HeaderList.GetSRaw(semsIndex);

                            if (raw == SemesterData.Empty) continue;

                            string placeKey = SLearningDomainParser.GetSemesterString(raw);

                            Cell datacell = RatingRow.Cells[student.HeaderList[semsIndex] * 3 + 2];

                            bool UseRatingRank = true;
                            // 處理已有年排名(在UDT有存資料)
                            if (uddList.Count > 0 && student.LastEnterSemester.HasValue)
                            {
                                foreach (UserDefData udd in uddList)
                                {
                                    if (udd.SchoolYear == semsIndex.SchoolYear && udd.Semester == semsIndex.Semester)
                                    {
                                        datacell.Write(builder, GetPlaceString2(udd.GradeRank, udd.GradeRankPercent));
                                        UseRatingRank = false;
                                    }
                                }
                            }

                            // 判斷轉入生
                            if (student.LastEnterGradeYear.HasValue && student.LastEnterSchoolyear.HasValue)
                            {
                                if (semsIndex.SchoolYear <= student.LastEnterSchoolyear.Value)
                                {

                                    bool clear = true;

                                    // 當有資料
                                    foreach (UserDefData udd in (from data in uddList where data.SchoolYear == semsIndex.SchoolYear && data.Semester == semsIndex.Semester select data))
                                        clear = false;

                                    if (clear)
                                    {
                                        // 和異動同年同學期不動
                                        if (semsIndex.SchoolYear == student.LastEnterSchoolyear && semsIndex.Semester == student.LastEnterSemester)
                                            clear = false;
                                    }

                                    if (clear)
                                    {
                                        // 當同一學年第2學期，如果維持不清空
                                        if (semsIndex.SchoolYear == student.LastEnterSchoolyear.Value && semsIndex.Semester == 2)
                                            clear = false;
                                    }


                                    if (clear)
                                    {
                                        datacell.Write(builder, "");
                                        UseRatingRank = false;
                                    }

                                }


                                if (uddList.Count == 0)
                                {
                                    if (semsIndex.SchoolYear <= student.LastEnterSchoolyear.Value)
                                    {
                                        if (semsIndex.SchoolYear < student.LastEnterSchoolyear.Value)
                                        {
                                            datacell.Write(builder, "");
                                            UseRatingRank = false;
                                        }
                                        else if (semsIndex.SchoolYear == student.LastEnterSchoolyear.Value && semsIndex.Semester < student.LastEnterSemester)
                                        {
                                            datacell.Write(builder, "");
                                            UseRatingRank = false;
                                        }
                                    }
                                }
                            }

                            // 使用即時運算排名與百分比
                            if (UseRatingRank)
                                if (places.Contains(placeKey))
                                    datacell.Write(builder, GetPlaceString(places, placeKey));
                        }

                        // 顯示平均
                        if (places.Contains("學習領域"))
                            RatingRow.Cells[21].Write(builder, GetPlaceString(places, "學習領域"));
                    }

                    int? LevelAdd = null, PercentageAdd = null;
                    // 處理年排名與百分比(加分後)
                    if (RatingRowAdd != null)
                    {
                        PlaceCollection places = student.Places.NS("年排名");

                        foreach (SemesterData semsIndex in student.HeaderList.Keys)
                        {
                            SemesterData raw = student.HeaderList.GetSRaw(semsIndex);

                            if (raw == SemesterData.Empty) continue;

                            string placeKey = SLearningDomainParser.GetSemesterString(raw);

                            //if (!places.Contains(placeKey))
                            //    continue;

                            Cell datacell = RatingRowAdd.Cells[student.HeaderList[semsIndex] * 3 + 2];

                            // 如果是一般生直接填空白
                            if (string.IsNullOrEmpty(student.SpcStudTypeName))
                            {
                                datacell.Write(builder, "");
                                continue;
                            }
                            int Level = 1, Percentage = 1;

                            // 處理加分後
                            if (places.Contains(placeKey))
                            {
                                if (student.AddWeight.HasValue)
                                {
                                    List<Place> PList = new List<Place>();
                                    decimal sc = places[placeKey].Score * student.AddWeight.Value;
                                    if (DALTransfer.StudRankScoreDict.ContainsKey(student.GradeYear))
                                    {
                                        if (DALTransfer.StudRankScoreDict[student.GradeYear].ContainsKey(placeKey))
                                        {
                                            PList = (from data in DALTransfer.StudRankScoreDict[student.GradeYear][placeKey] where data.Place.Score >= sc orderby data.Place.Score ascending select data.Place).ToList();
                                            if (PList.Count > 0)
                                            {
                                                PList.OrderBy(x => x.Score);
                                                Level = PList[0].Level;
                                                Percentage = PList[0].Percentage;
                                            }
                                            else
                                            {
                                                Level = 1;
                                                Percentage = 1;
                                            }
                                        }
                                    }
                                }
                            }


                            bool UseRatingRank = true;
                            // 處理已有年排名(UDT)
                            if (uddList.Count > 0 && student.LastEnterSemester.HasValue)
                            {
                                foreach (UserDefData udd in uddList)
                                {
                                    if (udd.SchoolYear == semsIndex.SchoolYear && udd.Semester == semsIndex.Semester)
                                    {
                                        datacell.Write(builder, GetPlaceString2(udd.GradeRankAdd, udd.GradeRankPercentAdd));
                                        UseRatingRank = false;
                                    }
                                }
                            }

                            // 判斷轉入生
                            if (student.LastEnterGradeYear.HasValue && student.LastEnterSchoolyear.HasValue)
                            {
                                if (semsIndex.SchoolYear <= student.LastEnterSchoolyear.Value)
                                {

                                    bool clear = true;

                                    // 當有資料
                                    foreach (UserDefData udd in (from data in uddList where data.SchoolYear == semsIndex.SchoolYear && data.Semester == semsIndex.Semester select data))
                                        clear = false;

                                    if (clear)
                                    {
                                        // 和異動同年同學期不動
                                        if (semsIndex.SchoolYear == student.LastEnterSchoolyear && semsIndex.Semester == student.LastEnterSemester)
                                            clear = false;
                                    }

                                    if (clear)
                                    {
                                        // 當同一學年第2學期，如果維持不清空
                                        if (semsIndex.SchoolYear == student.LastEnterSchoolyear.Value && semsIndex.Semester == 2)
                                            clear = false;
                                    }



                                    if (clear)
                                    {
                                        datacell.Write(builder, "");
                                        UseRatingRank = false;
                                    }

                                }


                                if (uddList.Count == 0)
                                {
                                    if (semsIndex.SchoolYear <= student.LastEnterSchoolyear.Value)
                                    {
                                        if (semsIndex.SchoolYear < student.LastEnterSchoolyear.Value)
                                        {
                                            datacell.Write(builder, "");
                                            UseRatingRank = false;
                                        }
                                        else if (semsIndex.SchoolYear == student.LastEnterSchoolyear.Value && semsIndex.Semester < student.LastEnterSemester)
                                        {
                                            datacell.Write(builder, "");
                                            UseRatingRank = false;
                                        }
                                    }
                                }
                            }

                            //// 使用即時運算排名與百分比
                            if (UseRatingRank)
                            {
                                if (places.Contains(placeKey))
                                {
                                    if (student.AddWeight.HasValue)
                                        datacell.Write(builder, GetPlaceString2(Level, Percentage));
                                    else
                                        datacell.Write(builder, GetPlaceString(places, placeKey));

                                }
                            }

                            //// 每學期加分後
                            //if (places.Contains(placeKey))
                            //    datacell.Write(builder, "^_^");

                        }

                        //這是加分後平均
                        //if (places.Contains(LearningDomainNameSpcAddP))                            
                        //    RatingRowAdd.Cells[20].Write(builder, GetPlaceString(places, LearningDomainNameSpcAddP));


                        // 加分後
                        if (student.AddWeight.HasValue)
                        {
                            decimal sc1 = 0;
                            PercentageAdd = null; LevelAdd = null;
                            List<Place> PList = new List<Place>();
                            if (places.Contains("學習領域"))
                                sc1 = places["學習領域"].Score * student.AddWeight.Value;
                            if (DALTransfer.StudRankScoreDict.ContainsKey(student.GradeYear))
                            {
                                if (DALTransfer.StudRankScoreDict[student.GradeYear].ContainsKey("學期總平均"))
                                {
                                    PList = (from data in DALTransfer.StudRankScoreDict[student.GradeYear]["學期總平均"] where data.Place.Score >= sc1 orderby data.Place.Score ascending select data.Place).ToList();
                                    if (PList.Count > 0)
                                    {
                                        PList.OrderBy(x => x.Score);
                                        LevelAdd = PList[0].Level;
                                        PercentageAdd = PList[0].Percentage;
                                    }
                                    else
                                    {
                                        LevelAdd = 1;
                                        PercentageAdd = 1;
                                    }
                                }
                                if (LevelAdd.HasValue && PercentageAdd.HasValue)
                                    RatingRowAdd.Cells[21].Write(builder, GetPlaceString2(LevelAdd.Value, PercentageAdd.Value));
                            }
                        }

                    }

                    // 樂學計分方式
                    if (RatingRowAdd != null)
                    {
                        //string str = "單一學期學習領域成績計分=100-(名次百分比)×100+1";
                        Row RowStr = RatingRowAdd.NextSibling as Row;
                        //RowStr.Cells[2].Write(builder, str);

                        PlaceCollection places = student.Places.NS("年排名");
                        foreach (SemesterData semsIndex in student.HeaderList.Keys)
                        {
                            SemesterData raw = student.HeaderList.GetSRaw(semsIndex);

                            if (raw == SemesterData.Empty) continue;

                            string placeKey = SLearningDomainParser.GetSemesterString(raw);

                            Cell datacell = RowStr.Cells[student.HeaderList[semsIndex] * 3 + 2];

                            bool UseRatingRank = true;

                            // 處理已有年排名(在UDT有存資料)
                            if (uddList.Count > 0 && student.LastEnterSemester.HasValue)
                            {
                                foreach (UserDefData udd in uddList)
                                {
                                    if (udd.SchoolYear == semsIndex.SchoolYear && udd.Semester == semsIndex.Semester)
                                    {
                                        if (student.AddWeight.HasValue)
                                            datacell.Write(builder, GetPlaceString3(udd.GradeRankPercentAdd));
                                        else
                                            datacell.Write(builder, GetPlaceString3(udd.GradeRankPercent));
                                        UseRatingRank = false;
                                    }
                                }
                            }

                            // 判斷轉入生
                            if (student.LastEnterGradeYear.HasValue && student.LastEnterSchoolyear.HasValue)
                            {
                                if (semsIndex.SchoolYear <= student.LastEnterSchoolyear.Value)
                                {

                                    bool clear = true;

                                    // 當有資料
                                    foreach (UserDefData udd in (from data in uddList where data.SchoolYear == semsIndex.SchoolYear && data.Semester == semsIndex.Semester select data))
                                        clear = false;

                                    if (clear)
                                    {
                                        // 和異動同年同學期不動
                                        if (semsIndex.SchoolYear == student.LastEnterSchoolyear && semsIndex.Semester == student.LastEnterSemester)
                                            clear = false;
                                    }

                                    if (clear)
                                    {
                                        // 當同一學年第2學期，如果維持不清空
                                        if (semsIndex.SchoolYear == student.LastEnterSchoolyear.Value && semsIndex.Semester == 2)
                                            clear = false;
                                    }



                                    if (clear)
                                    {
                                        datacell.Write(builder, "");
                                        UseRatingRank = false;
                                    }

                                }

                                if (uddList.Count == 0)
                                {
                                    if (semsIndex.SchoolYear <= student.LastEnterSchoolyear.Value)
                                    {
                                        if (semsIndex.SchoolYear < student.LastEnterSchoolyear.Value)
                                        {
                                            datacell.Write(builder, "");
                                            UseRatingRank = false;
                                        }
                                        else if (semsIndex.SchoolYear == student.LastEnterSchoolyear.Value && semsIndex.Semester < student.LastEnterSemester)
                                        {
                                            datacell.Write(builder, "");
                                            UseRatingRank = false;
                                        }
                                    }
                                }
                            }

                            if (UseRatingRank)
                            {

                                // 處理樂學計算
                                if (student.AddWeight.HasValue == false)
                                    if (places.Contains(placeKey))
                                        datacell.Write(builder, GetPlaceString3(places[placeKey].Percentage));


                                int Level = 1, Percentage = 1;

                                // 處理加分後
                                if (places.Contains(placeKey))
                                {
                                    if (student.AddWeight.HasValue)
                                    {
                                        List<Place> PList = new List<Place>();
                                        decimal sc = places[placeKey].Score * student.AddWeight.Value;
                                        if (DALTransfer.StudRankScoreDict.ContainsKey(student.GradeYear))
                                        {
                                            if (DALTransfer.StudRankScoreDict[student.GradeYear].ContainsKey(placeKey))
                                            {
                                                PList = (from data in DALTransfer.StudRankScoreDict[student.GradeYear][placeKey] where data.Place.Score >= sc orderby data.Place.Score ascending select data.Place).ToList();
                                                if (PList.Count > 0)
                                                {
                                                    PList.OrderBy(x => x.Score);
                                                    Level = PList[0].Level;
                                                    Percentage = PList[0].Percentage;
                                                }
                                                else
                                                {
                                                    Level = 1;
                                                    Percentage = 1;
                                                }
                                            }
                                        }
                                        datacell.Write(builder, GetPlaceString3(Percentage));
                                    }
                                }
                            }

                        }

                        //// 顯示平均
                        //Place p;
                        //if (places.Contains("學習領域"))
                        //{
                        //if (student.AddWeight.HasValue)
                        //{
                        //    if(PercentageAdd.HasValue )
                        //        RowStr.Cells[20].Write(builder, GetPlaceString3(PercentageAdd.Value));
                        //}
                        //else
                        //{
                        //        p = places["學習領域"];
                        //        RowStr.Cells[20].Write(builder, GetPlaceString3(p.Percentage));
                        //    //}
                        //}
                    }
                    #endregion

                    #region 合併相關欄位。
                    string previousCellDomain = string.Empty;
                    int ccount = 0;
                    foreach (RowHeader header in indexHeaders)
                    {
                        bool hasGroup = !string.IsNullOrEmpty(Subj.GetDomainGroup(header.Domain));
                        string groupName = Subj.GetDomainGroup(header.Domain);

                        Row row = table.Rows[header.Index + 3];

                        if (previousCellDomain == row.Cells[1].ToTxt())
                            row.Cells[1].CellFormat.VerticalMerge = CellMerge.Previous;
                        else
                            row.Cells[1].CellFormat.VerticalMerge = CellMerge.First;

                        if (header.IsDomain)
                        {
                            if (header.Domain == LearningDomainName)
                            {
                                #region 學習領域
                                // 這段在處理 header
                                row.Cells[1].CellFormat.FitText = false;
                                row.Cells[1].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[2].CellFormat.HorizontalMerge = CellMerge.Previous;

                                #endregion
                            }
                            else if (header.Domain == LearningDomainNameSpcAdd)
                            {
                                #region 學習領域(加分後)
                                row.Cells[0].CellFormat.FitText = false;
                                row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;


                                #endregion
                            }
                            else if (header.Domain == LearningDomainNameP)
                            {
                                // 學成成績排名與百分比
                                row.Cells[0].CellFormat.FitText = false;
                                row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                {

                                    //Row lrow = row.NextSibling as Row;
                                    Row lrow = row;
                                    lrow.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                    lrow.Cells[0].CellFormat.FitText = false;
                                    lrow.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                                    Paragraph mp = lrow.Cells[2].Paragraphs[0];
                                    for (int i = 0; i < (3 * 6); i++)
                                    {
                                        if (i % 3 == 0)
                                        {
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                            (mp.ParentNode as Cell).CellFormat.FitText = false;
                                        }
                                        else
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;

                                        mp = Util.NextCell(mp as Paragraph);
                                    }
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                    mp = Util.NextCell(mp as Paragraph);
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;
                                }

                            }
                            else if (header.Domain == LearningDomainNameSpcAddP)
                            {
                                // 學習成績排名與百分比(加分後)
                                row.Cells[0].CellFormat.FitText = false;
                                row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                {
                                    //Row lrow = row.NextSibling as Row;

                                    Row lrow = row;
                                    lrow.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                    lrow.Cells[0].CellFormat.FitText = false;
                                    lrow.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                                    Paragraph mp = lrow.Cells[2].Paragraphs[0];
                                    for (int i = 0; i < (3 * 6); i++)
                                    {
                                        if (i % 3 == 0)
                                        {
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                            (mp.ParentNode as Cell).CellFormat.FitText = false;
                                        }
                                        else
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;

                                        mp = Util.NextCell(mp as Paragraph);
                                    }
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                    mp = Util.NextCell(mp as Paragraph);
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;
                                }

                            }
                            else if (header.Domain == CalcMessage)
                            {
                                // 文字字串
                                row.Cells[0].CellFormat.FitText = false;
                                row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                                Row lrow = row;
                                lrow.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                lrow.Cells[0].CellFormat.FitText = false;
                                lrow.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                                Paragraph mp = lrow.Cells[2].Paragraphs[0];
                                for (int i = 0; i < (3 * 6); i++)
                                {
                                    // if (i % 18 == 0)
                                    if (i % 3 == 0)
                                    {
                                        (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                        (mp.ParentNode as Cell).CellFormat.FitText = false;
                                    }
                                    else
                                        (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;

                                    mp = Util.NextCell(mp as Paragraph);
                                }
                                (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                mp = Util.NextCell(mp as Paragraph);
                                (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;

                            }
                            else
                            {
                                if (!hasGroup)
                                {
                                    row.Cells[1].CellFormat.HorizontalMerge = CellMerge.First;
                                    row.Cells[2].CellFormat.HorizontalMerge = CellMerge.Previous;
                                }
                            }
                        }

                        previousCellDomain = row.Cells[1].ToTxt();
                        ccount++;
                    }
                    // 主要合併均衡學習
                    for (int i = (ccount + 2); i > 3; i--)
                    {
                        table.Rows[i - 1].Cells[0].CellFormat.VerticalMerge = CellMerge.First;
                        table.Rows[i].Cells[0].CellFormat.VerticalMerge = CellMerge.Previous;
                    }

                    table.Rows[3].Cells[0].Write(builder, "均衡學習");

                    #endregion

                    #endregion
                }
                else if (PrintSetting.ListMethod == ListMethod.SubjectOnly)
                {
                    #region 列印科目
                    UniqueSet<RowHeader> RowIndexs = new UniqueSet<RowHeader>();

                    foreach (SemesterData semester in student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters))
                    {
                        SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);
                        if (!student.SemestersScore.Contains(sysems)) continue;

                        SemesterScore semsscore = student.SemestersScore[sysems];

                        foreach (string strSubject in semsscore.Subject)
                        {
                            SemesterSubjectScore subject = semsscore.Subject[strSubject];

                            RowHeader header;
                            if (IsFlexible(subject.Domain))
                                header = new RowHeader("彈性課程", strSubject);
                            else
                                header = new RowHeader(subject.Domain, strSubject);

                            header.IsDomain = false;

                            if (!RowIndexs.Contains(header))
                                RowIndexs.Add(header);
                        }
                    }

                    RowHeader lheader = new RowHeader(LearningDomainName, string.Empty);
                    lheader.IsDomain = true;
                    //RowIndexs.Add(lheader);

                    //List<RowHeader> sortedHeaders = SortHeader(RowIndexs.ToList());
                    List<RowHeader> sortedHeaders = RowIndexs.ToList();

                    sortedHeaders.Sort(delegate(RowHeader x, RowHeader y)
                    {
                        Subj xx = new JHSchool.Evaluation.Subject(x.Subject, x.Domain);
                        Subj yy = new JHSchool.Evaluation.Subject(y.Subject, y.Domain);

                        return xx.CompareTo(yy);
                    });
                    //sortedHeaders.Sort(Util.SortSubject);
                    //sortedHeaders.Sort(Util.SortDomain);

                    //把學習領域放在正確的地方。
                    foreach (RowHeader eachHeader in sortedHeaders.ToArray())
                    {
                        if (IsFlexible(eachHeader.Domain))
                        {
                            int index = sortedHeaders.IndexOf(eachHeader);
                            sortedHeaders.Insert(index, lheader);
                            break;
                        }
                    }
                    if (sortedHeaders.IndexOf(lheader) < 0)
                        sortedHeaders.Add(lheader);

                    //產生 Row。
                    List<RowHeader> indexHeaders = new List<RowHeader>();
                    Row current = template;
                    int rowIndex = 0;
                    foreach (RowHeader header in sortedHeaders)
                    {
                        RowHeader indexH = header;
                        indexH.Index = rowIndex++;
                        indexHeaders.Add(indexH);

                        Row datarow = table.InsertBefore(template.Clone(true), current) as Row;

                        if (header.Domain == LearningDomainName)
                        {
                            string headerName = string.Empty;

                            if (PrintSetting.PrintRank)
                                headerName = "學習領域排名";

                            if (PrintSetting.PrintRankPercentage)
                                headerName = "學習領域百分比";

                            if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                                headerName = "學習領域排名/百分比";

                            if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                            {
                                Row lrow = table.InsertAfter(template.Clone(true), datarow) as Row;
                                lrow.Cells[0].Write(builder, headerName);
                                rowIndex++;
                            }
                        }

                        if (IsFlexible(header.Domain))
                        {
                            datarow.Cells[1].Write(builder, header.Domain);
                            datarow.Cells[2].Write(builder, header.Subject);
                        }
                        else if (header.Domain == LearningDomainName)
                            datarow.Cells[1].Write(builder, header.Domain);
                        else
                            datarow.Cells[1].Write(builder, header.Subject);
                    }

                    //填資料
                    Row RatingRow = null;
                    foreach (RowHeader header in indexHeaders)
                    {
                        SemesterDataCollection semesters = new SemesterDataCollection();
                        Row row = table.Rows[header.Index + 3];
                        foreach (SemesterData semester in student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters))
                        {
                            //避開三年級下學期
                            if ((semester.GradeYear == 3 || semester.GradeYear == 9) && semester.Semester == 2)
                                continue;

                            SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);
                            semesters.Add(sysems);

                            if (!student.SemestersScore.Contains(sysems)) continue;
                            if (!student.HeaderList.ContainsKey(sysems)) continue;

                            int columnIndex = student.HeaderList[sysems];
                            SemesterScore semsscore = student.SemestersScore[sysems];

                            decimal? score = null;
                            decimal? weight = null;

                            if (header.IsDomain)
                            {
                                if (semsscore.Domain.Contains(header.Domain))
                                {
                                    score = semsscore.Domain[header.Domain].Value;
                                    weight = semsscore.Domain[header.Domain].Weight;
                                }
                            }
                            else
                            {
                                if (semsscore.Subject.Contains(header.Subject))
                                {
                                    score = semsscore.Subject[header.Subject].Value;
                                    weight = semsscore.Subject[header.Subject].Weight;
                                }
                            }

                            if (header.Domain == LearningDomainName)
                            {
                                score = semsscore.LearnDomainScore;

                                row.Cells[columnIndex * 3 + 3].CellFormat.FitText = false;
                                row.Cells[columnIndex * 3 + 3].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[columnIndex * 3 + 4].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (!score.HasValue) continue;

                                row.Cells[columnIndex * 3 + 3].Write(builder, ((double)score) + "");
                                row.Cells[columnIndex * 3 + 5].Write(builder, (Util.GetDegree(score.Value)));

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                    RatingRow = row.NextSibling as Row;
                            }
                            else
                            {
                                if (!score.HasValue) continue;
                                if (!weight.HasValue) weight = 0;

                                row.Cells[columnIndex * 3 + 3].Write(builder, ((double)weight) + "");
                                row.Cells[columnIndex * 3 + 4].Write(builder, ((double)score) + "");
                                row.Cells[columnIndex * 3 + 5].Write(builder, (Util.GetDegree(score.Value)));
                            }
                        }

                        //算平均...
                        decimal? avgScore = null;
                        if (header.IsDomain)
                        {
                            if (header.Domain == LearningDomainName)
                            {
                                avgScore = student.SemestersScore.AvgLearningDomainScore(semesters);

                                if (!avgScore.HasValue) continue;

                                if (student.CalculationRule == null)
                                    avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                                else
                                    avgScore = student.CalculationRule.ParseLearnDomainScore(avgScore.Value);
                            }
                            else
                            {
                                avgScore = student.SemestersScore.AvgDomainScore(semesters, header.Domain);

                                if (!avgScore.HasValue) continue;

                                if (student.CalculationRule == null)
                                    avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                                else
                                    avgScore = student.CalculationRule.ParseDomainScore(avgScore.Value);
                            }
                        }
                        else
                        {
                            avgScore = student.SemestersScore.AvgSubjectScore(semesters, header.Subject);

                            if (!avgScore.HasValue) continue;

                            if (student.CalculationRule == null)
                                avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                            else
                                avgScore = student.CalculationRule.ParseSubjectScore(avgScore.Value);
                        }

                        row.Cells[21].Write(builder, (double)avgScore + "");
                        row.Cells[22].Write(builder, Util.GetDegree(avgScore.Value));
                    }
                    if (RatingRow != null)
                    {
                        PlaceCollection places = student.Places.NS("年排名");

                        foreach (SemesterData semsIndex in student.HeaderList.Keys)
                        {
                            SemesterData raw = student.HeaderList.GetSRaw(semsIndex);

                            if (raw == SemesterData.Empty) continue;

                            string placeKey = SLearningDomainParser.GetSemesterString(raw);

                            Cell datacell = RatingRow.Cells[student.HeaderList[semsIndex] * 3 + 2];

                            if (places.Contains(placeKey))
                                datacell.Write(builder, GetPlaceString(places, placeKey));
                        }

                        if (places.Contains(LearningDomainName))
                            RatingRow.Cells[21].Write(builder, GetPlaceString(places, LearningDomainName));
                    }

                    //合併相關欄位。
                    string previousCellDomain = string.Empty;
                    foreach (RowHeader header in indexHeaders)
                    {
                        Row row = table.Rows[header.Index + 3];

                        if (header.IsDomain)
                        {
                            if (header.Domain == LearningDomainName)
                            {
                                row.Cells[1].CellFormat.FitText = false;
                                row.Cells[1].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[2].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                {
                                    Row lrow = row.NextSibling as Row;
                                    lrow.Cells[1].CellFormat.HorizontalMerge = CellMerge.First;
                                    lrow.Cells[1].CellFormat.FitText = false;
                                    lrow.Cells[2].CellFormat.HorizontalMerge = CellMerge.Previous;
                                    Paragraph mp = lrow.Cells[3].Paragraphs[0];
                                    for (int i = 0; i < (3 * 6); i++)
                                    {
                                        if (i % 3 == 0)
                                        {
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                            (mp.ParentNode as Cell).CellFormat.FitText = false;
                                        }
                                        else
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;

                                        mp = Util.NextCell(mp as Paragraph);
                                    }
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                    mp = Util.NextCell(mp as Paragraph);
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;
                                }
                            }
                        }
                        else if (IsFlexible(header.Domain))
                        {
                            if (previousCellDomain == header.Domain)
                                row.Cells[1].CellFormat.VerticalMerge = CellMerge.Previous;

                            else
                                row.Cells[1].CellFormat.VerticalMerge = CellMerge.First;

                            previousCellDomain = header.Domain;
                        }
                        else
                        {
                            //row.Cells[0].CellFormat.FitText = true;
                            row.Cells[1].CellFormat.HorizontalMerge = CellMerge.First;
                            row.Cells[2].CellFormat.HorizontalMerge = CellMerge.Previous;
                        }
                    }
                    #endregion
                }

                template.NextSibling.Remove();
                template.Remove();
                #endregion
            }
            else if (e.FieldName == "Fix:缺曠獎懲")
            {
                #region 列印獎懲資料
                int Offset = 2;
                Row MeritA = builder.CurrentParagraph.ParentNode.ParentNode as Row;
                Row MeritB = MeritA.NextSibling as Row;
                Row MeritC = MeritB.NextSibling as Row;
                Row DemeritA = MeritC.NextSibling as Row;
                Row DemeritB = DemeritA.NextSibling as Row;
                Row DemeritC = DemeritB.NextSibling as Row;
                Row DisciplineSet = DemeritC.NextSibling as Row;
                Row DisciplineNormal = DisciplineSet.NextSibling as Row;

                foreach (SemesterData each in student.Summaries.Keys)
                {
                    XmlElement summary = student.Summaries[each];

                    if (!student.HeaderList.ContainsKey(each)) continue;

                    int ColumnIndex = student.HeaderList[each];

                    XmlElement xmlmerit = summary.SelectSingleNode("DisciplineStatistics/Merit") as XmlElement;
                    XmlElement xmldemerit = summary.SelectSingleNode("DisciplineStatistics/Demerit") as XmlElement;

                    if (xmlmerit != null)
                    {
                        if (!string.IsNullOrEmpty(GetString(xmlmerit.GetAttribute("A"))))
                            MeritA.Cells[Offset + ColumnIndex].Write(builder, GetString(xmlmerit.GetAttribute("A")));
                        if (!string.IsNullOrEmpty(GetString(xmlmerit.GetAttribute("B"))))
                            MeritB.Cells[Offset + ColumnIndex].Write(builder, GetString(xmlmerit.GetAttribute("B")));
                        if (!string.IsNullOrEmpty(GetString(xmlmerit.GetAttribute("C"))))
                            MeritC.Cells[Offset + ColumnIndex].Write(builder, GetString(xmlmerit.GetAttribute("C")));
                    }

                    if (xmldemerit != null)
                    {
                        if (!string.IsNullOrEmpty(GetString(xmldemerit.GetAttribute("A"))))
                            DemeritA.Cells[Offset + ColumnIndex].Write(builder, GetString(xmldemerit.GetAttribute("A")));
                        if (!string.IsNullOrEmpty(GetString(xmldemerit.GetAttribute("B"))))
                            DemeritB.Cells[Offset + ColumnIndex].Write(builder, GetString(xmldemerit.GetAttribute("B")));
                        if (!string.IsNullOrEmpty(GetString(xmldemerit.GetAttribute("C"))))
                            DemeritC.Cells[Offset + ColumnIndex].Write(builder, GetString(xmldemerit.GetAttribute("C")));
                    }


                }
                #endregion
            }
            else if (e.FieldName == "Fix:服務學習")
            {
                if (_SLRecordDict.ContainsKey(student.StudentID))
                {
                    List<SLRecord> recList = _SLRecordDict[student.StudentID];

                    #region 列印服務學習
                    SemesterDataCollection semesters = student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters);
                    Row SemesterRow = builder.CurrentParagraph.ParentNode.ParentNode as Row;

                    int count = 0, offset = 1;

                    foreach (SemesterData each in semesters)
                    {
                        decimal hour = 0;
                        string val = "";

                        foreach (SLRecord rec in recList)
                        {
                            if (rec.SchoolYear == each.SchoolYear && rec.Semester == each.Semester)
                                hour += rec.Hours;
                        }

                        if (hour > 0)
                            val = hour.ToString();

                        Paragraph resetParagraph = builder.CurrentParagraph;
                        SemesterRow.Cells[count + offset].Write(builder, val);

                        count++;
                    }
                    #endregion
                }
            }
            else if (e.FieldName == "Fix:幹部任期")
            {
                if (_itemCountDict.ContainsKey(student.StudentID))
                {
                    List<itemCount> recList = _itemCountDict[student.StudentID];

                    #region 列印幹部任期
                    SemesterDataCollection semesters = student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters);
                    Row SemesterRow = builder.CurrentParagraph.ParentNode.ParentNode as Row;

                    int count = 0, offset = 1;

                    foreach (SemesterData each in semesters)
                    {
                        int hour = 0;
                        string val = "";

                        foreach (itemCount rec in recList)
                        {
                            if (rec.SchoolYear == each.SchoolYear && rec.Semester == each.Semester)
                            {
                                hour = rec.Count;
                            }
                        }

                        if (hour > 0)
                            val = hour.ToString();

                        Paragraph resetParagraph = builder.CurrentParagraph;
                        SemesterRow.Cells[count + offset].Write(builder, val);

                        count++;
                    }
                    #endregion
                }

            }
            else if (e.FieldName == "Fix:獎勵明細")
            {
                if (_JHMeritRecordDict.ContainsKey(student.StudentID))
                {
                    List<JHMeritRecord> dataList = (from data in _JHMeritRecordDict[student.StudentID] orderby data.SchoolYear, data.Semester, data.OccurDate select data).ToList();

                    #region 列印獎懲資料
                    // 檢查並動態新增 Table row

                    Row MeritCheckRow = builder.CurrentParagraph.ParentNode.ParentNode as Row;
                    int addRowCount = _JHMeritRecordDict[student.StudentID].Count - 3;

                    if (addRowCount > 0)
                    {
                        for (int i = 1; i <= addRowCount; i++)
                        {
                            Row newRow = (Row)MeritCheckRow.Clone(true);
                            MeritCheckRow.ParentTable.AppendChild(newRow);
                        }
                    }
                    Row MeritRow = builder.CurrentParagraph.ParentNode.ParentNode as Row;
                    int rowIdx = 1;
                    List<string> typeList = new List<string>();
                    List<int> cotList = new List<int>();
                    //學年度學期	類別	次數	事由
                    foreach (JHMeritRecord rec in dataList)
                    {
                        string strSchoolYear = rec.SchoolYear.ToString();
                        if (rec.Semester == 1)
                            strSchoolYear += "上";

                        if (rec.Semester == 2)
                            strSchoolYear += "下";

                        typeList.Clear();
                        cotList.Clear();

                        if (rec.MeritA.HasValue && rec.MeritA.Value > 0)
                        {
                            typeList.Add("大功");
                            cotList.Add(rec.MeritA.Value);
                        }

                        if (rec.MeritB.HasValue && rec.MeritB.Value > 0)
                        {
                            typeList.Add("小功");
                            cotList.Add(rec.MeritB.Value);
                        }

                        if (rec.MeritC.HasValue && rec.MeritC.Value > 0)
                        {

                            typeList.Add("嘉獎");
                            cotList.Add(rec.MeritC.Value);
                        }

                        //builder.Document.FirstSection.Body.Tables[2].Rows[rowIdx].Cells[0].Write(builder, strSchoolYear);
                        //builder.Document.FirstSection.Body.Tables[2].Rows[rowIdx].Cells[1].Write(builder, Type);
                        //builder.Document.FirstSection.Body.Tables[2].Rows[rowIdx].Cells[2].Write(builder, cot.ToString());
                        //builder.Document.FirstSection.Body.Tables[2].Rows[rowIdx].Cells[3].Write(builder, rec.Reason);
                        //rowIdx++;
                        MeritRow.Cells[0].Write(builder, strSchoolYear);

                        if (typeList.Count > 0)
                            MeritRow.Cells[1].Write(builder, string.Join(",", typeList.ToArray()));
                        else
                            MeritRow.Cells[1].Write(builder, "");

                        if (cotList.Count > 0)
                            MeritRow.Cells[2].Write(builder, string.Join(",", cotList.ToArray()));
                        else
                            MeritRow.Cells[2].Write(builder, "");

                        MeritRow.Cells[3].Write(builder, rec.Reason);
                        MeritRow = MeritRow.NextSibling as Row;
                    }
                    #endregion
                }
            }
            else if (e.FieldName == "Fix:體適能")
            {
                if (_StudentFitnessRecordDict.ContainsKey(student.StudentID))
                {
                    List<StudentFitnessRecord> recList = _StudentFitnessRecordDict[student.StudentID];

                    Dictionary<int, StudentFitnessRecord> recordDic = new Dictionary<int, StudentFitnessRecord>();
                    foreach (StudentFitnessRecord record in recList)
                    {
                        if (!recordDic.ContainsKey(record.SchoolYear))
                        {
                            recordDic.Add(record.SchoolYear, record);
                        }
                    }
                    #region 列印體適能
                    //坐姿體前彎
                    //立定跳遠
                    //仰臥起坐
                    //心肺適能
                    //SemesterDataCollection semesters = student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters);
                    SemesterDataCollection semesters = student.SHistory.GetGradeYearSemester();
                    Row FitnessRowA = builder.CurrentParagraph.ParentNode.ParentNode as Row;
                    Row FitnessRowB = FitnessRowA.NextSibling as Row;
                    Row FitnessRowC = FitnessRowB.NextSibling as Row;
                    Row FitnessRowD = FitnessRowC.NextSibling as Row;

                    Dictionary<int, List<int>> schoolYearDic = new Dictionary<int, List<int>>();
                    schoolYearDic.Add(1, new List<int>());
                    schoolYearDic.Add(2, new List<int>());
                    schoolYearDic.Add(3, new List<int>());
                    schoolYearDic.Add(4, new List<int>());
                    schoolYearDic.Add(5, new List<int>());
                    schoolYearDic.Add(6, new List<int>());
                    foreach (SemesterData each in semesters)
                    {
                        if ((each.GradeYear == 1 || each.GradeYear == 7) && each.Semester == 1)
                            if (!schoolYearDic[1].Contains(each.SchoolYear))
                                schoolYearDic[1].Add(each.SchoolYear);

                        if ((each.GradeYear == 1 || each.GradeYear == 7) && each.Semester == 2)
                            if (!schoolYearDic[2].Contains(each.SchoolYear))
                                schoolYearDic[2].Add(each.SchoolYear);

                        if ((each.GradeYear == 2 || each.GradeYear == 8) && each.Semester == 1)
                            if (!schoolYearDic[3].Contains(each.SchoolYear))
                                schoolYearDic[3].Add(each.SchoolYear);

                        if ((each.GradeYear == 2 || each.GradeYear == 8) && each.Semester == 2)
                            if (!schoolYearDic[4].Contains(each.SchoolYear))
                                schoolYearDic[4].Add(each.SchoolYear);

                        if ((each.GradeYear == 3 || each.GradeYear == 9) && each.Semester == 1)
                            if (!schoolYearDic[5].Contains(each.SchoolYear))
                                schoolYearDic[5].Add(each.SchoolYear);

                        if ((each.GradeYear == 3 || each.GradeYear == 9) && each.Semester == 2)
                            if (!schoolYearDic[6].Contains(each.SchoolYear))
                                schoolYearDic[6].Add(each.SchoolYear);
                    }

                    List<int> usedAlready = new List<int>();
                    int count = 0, offset = 2;
                    foreach (int i in PrintSetting.PrintSemesters)
                    {
                        foreach (int schoolYear in schoolYearDic[i])
                        {
                            if (recordDic.ContainsKey(schoolYear) && !usedAlready.Contains(schoolYear))
                            {
                                StudentFitnessRecord rec = recordDic[schoolYear];
                                FitnessRowA.Cells[count / 2 + offset].Write(builder, rec.SitAndReachDegree);
                                FitnessRowB.Cells[count / 2 + offset].Write(builder, rec.StandingLongJumpDegree);
                                FitnessRowC.Cells[count / 2 + offset].Write(builder, rec.SitUpDegree);
                                FitnessRowD.Cells[count / 2 + offset].Write(builder, rec.CardiorespiratoryDegree);
                                usedAlready.Add(schoolYear);
                            }
                        }
                        count++;
                    }
                    //List<int> SchoolYearList = new List<int>();
                    //// 取得學年度
                    //foreach (SemesterData each in semesters)
                    //{
                    //    if (!SchoolYearList.Contains(each.SchoolYear))
                    //        SchoolYearList.Add(each.SchoolYear);
                    //}

                    //foreach (int sc in SchoolYearList)
                    //{
                    //    foreach (StudentFitnessRecord rec in recList.Where(x=>x.SchoolYear==sc))
                    //    {
                    //        FitnessRowA.Cells[count + offset].Write(builder, rec.SitAndReachDegree);
                    //        FitnessRowB.Cells[count + offset].Write(builder, rec.StandingLongJumpDegree);
                    //        FitnessRowC.Cells[count + offset].Write(builder, rec.SitUpDegree);
                    //        FitnessRowD.Cells[count + offset].Write(builder, rec.CardiorespiratoryDegree);                                
                    //    }
                    //    count ++;
                    //}
                    #endregion
                }
            }
        }

        private void RefineDomain(SemesterSubjectScore subject)
        {
            if (subject.Domain.Trim() == "彈性課程")
                subject.Domain = string.Empty;
        }

        private bool IsFlexible(string domainName)
        {
            if (string.IsNullOrEmpty(domainName))
                return true;

            if (domainName == "彈性課程")
                return true;

            return false;
        }

        /// <summary>
        /// 將領域、彈性課程、學習領域分開排序。
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<RowHeader> SortHeader(List<RowHeader> list)
        {
            List<RowHeader> domains = new List<RowHeader>();
            List<RowHeader> subjects = new List<RowHeader>();
            RowHeader? ldomain = null;
            RowHeader? ldomain1 = null;
            RowHeader? ldomain2 = null;
            RowHeader? ldomain3 = null;
            RowHeader? ldomain4 = null;

            foreach (RowHeader each in list)
            {
                if (each.Domain == LearningDomainName)
                {
                    ldomain = each;
                    continue;
                }

                if (each.Domain == LearningDomainNameSpcAdd)
                {
                    ldomain1 = each;
                    continue;
                }

                if (each.Domain == LearningDomainNameP)
                {
                    ldomain2 = each;
                    continue;
                }

                if (each.Domain == LearningDomainNameSpcAddP)
                {
                    ldomain3 = each;
                    continue;
                }

                if (each.Domain == CalcMessage)
                {
                    ldomain4 = each;
                    continue;
                }


                if (each.IsDomain)
                    domains.Add(each);
                else
                    subjects.Add(each);

            }
            domains.Sort(delegate(RowHeader x, RowHeader y)
            {
                Subj xx = new JHSchool.Evaluation.Subject(x.Subject, x.Domain);
                Subj yy = new JHSchool.Evaluation.Subject(y.Subject, y.Domain);

                return xx.CompareTo(yy);
            });

            subjects.Sort((x, y) => (Subj.CompareSubjectOrdinal(x.Subject, y.Subject)));

            List<RowHeader> result = new List<RowHeader>();
            result.AddRange(domains);
            if (ldomain != null) result.Add(ldomain.Value);
            if (ldomain1 != null) result.Add(ldomain1.Value);
            if (ldomain2 != null) result.Add(ldomain2.Value);
            if (ldomain3 != null) result.Add(ldomain3.Value);
            if (ldomain4 != null) result.Add(ldomain4.Value);
            result.AddRange(subjects);

            return result;
        }

        private string GetPlaceString(PlaceCollection places, string placeKey)
        {
            Place place = places[placeKey];
            //decimal percentage = (100m * ((decimal)place.Level / (decimal)place.Radix));


            ////小於1%的話，就是1%。
            //if (percentage < 1) percentage = 1;

            string result = string.Empty;

            if (PrintSetting.PrintRank)
                result = place.Level.ToString();

            if (PrintSetting.PrintRankPercentage)
                result = place.Percentage + "%";
            //result = Math.Round(percentage, 0, MidpointRounding.AwayFromZero) + "%";

            if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                result = string.Format("{0}/{1}%", place.Level, place.Percentage);
            //result = string.Format("{0}/{1}%", place.Level, Math.Round(percentage, 0, MidpointRounding.AwayFromZero));

            return result;
        }

        /// <summary>
        /// 處理加分後
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="Percentage"></param>
        /// <returns></returns>
        private string GetPlaceString2(int Level, int Percentage)
        {
            string result = string.Empty;
            if (PrintSetting.PrintRank)
                result = Level.ToString();

            if (PrintSetting.PrintRankPercentage)
                result = Percentage + "%";
            if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                result = string.Format("{0}/{1}%", Level, Percentage);
            return result;
        }

        /// <summary>
        /// 處理樂學計算
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="Percentage"></param>
        /// <returns></returns>
        private string GetPlaceString3(int Percentage)
        {
            string result = string.Empty;

            int num = 0;
            // 公式：100-名次百分比+1
            num = 100 - Percentage + 1;
            if (num > 0)
                result = string.Format("{0}", num);

            return result;
        }


        private static string GetString(string number)
        {
            int output;

            if (int.TryParse(number, out output))
                if (output <= 0) return string.Empty;

            return number;
        }

        private static string GetText(Paragraph originParagraph)
        {
            return originParagraph.ToTxt().Replace("«Fix:年級學期»", "").Replace("\r\n", "");
        }

        private static string GetSemesterString(SemesterData each)
        {
            return string.Format("{0} {1}", each.SchoolYear.ToString(), each.Semester == 1 ? "上" : "下");
        }

        private class MergeDataSource : IMailMergeDataSource
        {
            public List<ReportStudent> Students { get; private set; }

            private ReportPreference Preference { get; set; }

            private int Index { get; set; }

            private string PrintDate = DateTime.Now.ToString("yyyy/MM/dd");

            public MergeDataSource(List<ReportStudent> students, ReportPreference preference)
            {
                Students = students;
                Preference = preference;
                Index = -1;
            }

            #region IMailMergeDataSource 成員

            public bool GetValue(string fieldName, out object fieldValue)
            {
                fieldValue = string.Empty;
                ReportStudent student = Students[Index];

                //「Fix:」開頭的欄位另外處理。
                if (fieldName.ToUpper().StartsWith("Fix:".ToUpper()))
                {
                    fieldValue = student;
                    return true;
                }

                switch (fieldName)
                {
                    case "電子報表辨識編號": fieldValue = "系統編號{" + student.Id + "}"; break;

                    case "學號":
                        fieldValue = student.StudentNumber;
                        break;
                    case "班級":
                        fieldValue = student.ClassName;
                        break;
                    case "座號":
                        fieldValue = student.SeatNo;
                        break;
                    case "姓名":
                        fieldValue = student.Name;
                        break;
                    case "性別":
                        fieldValue = student.Gender;
                        break;
                    case "生日":
                        DateTime dtBirthday;

                        if (DateTime.TryParse(student.Birthday, out dtBirthday))
                            fieldValue = dtBirthday.Year - 1911 + "/" + dtBirthday.Month + "/" + dtBirthday.Day;
                        else
                            fieldValue = string.Empty;

                        break;
                    case "列印日期":
                        DateTime dtPrintDate;

                        if (DateTime.TryParse(PrintDate, out dtPrintDate))
                            fieldValue = dtPrintDate.Year - 1911 + "/" + dtPrintDate.Month + "/" + dtPrintDate.Day;
                        else
                            fieldValue = string.Empty;
                        break;
                    case "學校名稱":
                        fieldValue = K12.Data.School.ChineseName;
                        break;
                    case "入學日期":
                        if (string.IsNullOrEmpty(Preference.EntranceDate))
                            fieldValue = student.EntranceDate;
                        else
                            fieldValue = Preference.EntranceDate;
                        break;
                    case "畢業日期":
                        if (string.IsNullOrEmpty(Preference.GraduateDate))
                            fieldValue = student.GraduateDate;
                        else
                            fieldValue = Preference.GraduateDate;
                        break;
                    //case "特種身分類別":
                    //    if (string.IsNullOrEmpty(student.SpcStudTypeName))
                    //        fieldValue = "";
                    //    else
                    //        fieldValue = student.SpcStudTypeName;
                    //    break;

                    case "轉入異動日期":
                        if (string.IsNullOrEmpty(student.TransUpdateDateStr))
                            fieldValue = "";
                        else
                            fieldValue = student.TransUpdateDateStr;
                        break;

                        
                    case "最後計算日期":
                        fieldValue = Preference.FinalComputeDate;
                        break;
                    case "特身名稱1":
                        if (string.IsNullOrEmpty(student.SpcStudTypeName))
                        {
                            fieldValue = "□" + StudSpcName1;
                        }
                        else
                        {
                            if (StudSpcName1.Trim() == student.SpcStudTypeName.Trim())
                                fieldValue = "■" + StudSpcName1;
                            else
                                fieldValue = "□" + StudSpcName1;
                        }
                        break;

                    case "特身名稱2":

                        if (string.IsNullOrEmpty(student.SpcStudTypeName))
                        {
                            fieldValue = "□" + StudSpcName2;
                        }
                        else
                        {
                            if (StudSpcName2.Trim() == student.SpcStudTypeName.Trim())
                                fieldValue = "■" + StudSpcName2;
                            else
                                fieldValue = "□" + StudSpcName2;

                        }

                        break;

                    case "特身名稱3":
                        if (string.IsNullOrEmpty(student.SpcStudTypeName))
                        {
                            fieldValue = "□" + StudSpcName3;
                        }
                        else
                        {
                            if (StudSpcName3.Trim() == student.SpcStudTypeName.Trim())
                                fieldValue = "■" + StudSpcName3;
                            else
                                fieldValue = "□" + StudSpcName3;

                        }

                        break;

                    case "特身名稱4":
                        if (string.IsNullOrEmpty(student.SpcStudTypeName))
                        {
                            fieldValue = "□" + StudSpcName4;
                        }
                        else
                        {
                            if (StudSpcName4.Trim() == student.SpcStudTypeName.Trim())
                                fieldValue = "■" + StudSpcName4;
                            else
                                fieldValue = "□" + StudSpcName4;

                        }

                        break;

                    case "特身名稱5":
                        if (string.IsNullOrEmpty(student.SpcStudTypeName))
                        {
                            fieldValue = "□" + StudSpcName5;
                        }
                        else
                        {
                            if (StudSpcName5.Trim() == student.SpcStudTypeName.Trim())
                                fieldValue = "■" + StudSpcName5;
                            else
                                fieldValue = "□" + StudSpcName5;

                        }

                        break;

                    case "特身名稱6":
                        if (string.IsNullOrEmpty(student.SpcStudTypeName))
                        {
                            fieldValue = "□" + StudSpcName6;
                        }
                        else
                        {
                            if (StudSpcName6.Trim() == student.SpcStudTypeName.Trim())
                                fieldValue = "■" + StudSpcName6;
                            else
                                fieldValue = "□" + StudSpcName6;

                        }

                        break;

                    case "特身名稱7":
                        if (string.IsNullOrEmpty(student.SpcStudTypeName))
                        {
                            fieldValue = "□" + StudSpcName7;
                        }
                        else
                        {
                            if (StudSpcName7.Trim() == student.SpcStudTypeName.Trim())
                                fieldValue = "■" + StudSpcName7;
                            else
                                fieldValue = "□" + StudSpcName7;

                        }
                        break;

                    case "特身名稱8":
                        if (string.IsNullOrEmpty(student.SpcStudTypeName))
                        {
                            fieldValue = "□" + StudSpcName8;
                        }
                        else
                        {
                            if (StudSpcName8.Trim() == student.SpcStudTypeName.Trim())
                                fieldValue = "■" + StudSpcName8;
                            else
                                fieldValue = "□" + StudSpcName8;

                        }
                        break;
                    case "特身名稱9":
                        if (string.IsNullOrEmpty(student.SpcStudTypeName))
                        {
                            fieldValue = "□" + StudSpcName9;
                        }
                        else
                        {
                            if (StudSpcName9.Trim() == student.SpcStudTypeName.Trim())
                                fieldValue = "■" + StudSpcName9;
                            else
                                fieldValue = "□" + StudSpcName9;

                        }
                        break;
                    case "積分:均衡學習":
                        fieldValue = student.CreditDomainScore;
                        break;
                    case "積分:服務學習":
                        fieldValue = student.CreditServiceLearning;
                        break;
                    case "積分:體適能":
                        fieldValue = student.CreditFitness;
                        break;
                    case "積分:幹部任期":
                        fieldValue = student.CreditCadre;
                        break;
                }

                return true;
            }

            public bool MoveNext()
            {
                Index++;

                return (Index < Students.Count);
            }

            public string TableName { get { return string.Empty; } }

            #endregion
        }
    }
    #region 積分 UDT
    /// <summary>
    /// 比序績分主檔，在 ischoolJHWishBase 也有一個一樣的。
    /// </summary>
    [TableName("kh.enrolment_excess.credits")]
    public class ExcessCredits : ActiveRecord
    {
        ///<summary>
        /// 學生系統編號
        ///</summary>
        [Field(Field = "ref_student_id", Indexed = false)]
        public int StudentID { get; set; }

        ///<summary>
        /// 均衡學習
        ///</summary>
        [Field(Field = "balanced", Indexed = false)]
        public string Balanced { get; set; }

        ///<summary>
        /// 服務學習
        ///</summary>
        [Field(Field = "services", Indexed = false)]
        public string Services { get; set; }

        ///<summary>
        /// 體適能
        ///</summary>
        [Field(Field = "fitness", Indexed = false)]
        public string Fitness { get; set; }

        ///<summary>
        /// 競賽表現
        ///</summary>
        [Field(Field = "competition", Indexed = false)]
        public string Competition { get; set; }

        ///<summary>
        /// 檢定證照
        ///</summary>
        [Field(Field = "verification", Indexed = false)]
        public string Verification { get; set; }

        ///<summary>
        /// 獎勵紀錄
        ///</summary>
        [Field(Field = "merit", Indexed = false)]
        public string Merit { get; set; }

        ///<summary>
        /// 幹部任期
        ///</summary>
        [Field(Field = "term", Indexed = false)]
        public string Term { get; set; }

        ///<summary>
        /// 詳細資料
        ///</summary>
        [Field(Field = "detail", Indexed = false)]
        public string Detail { get; set; }

    }
    #endregion

    static class ReportStudentExt
    {
        public static string ToPrimaryKeyStringList(this List<ReportStudent> students)
        {
            List<string> pks = students.ConvertAll(x => x.StudentID);

            if (pks.Count <= 0)
                return string.Empty;
            else
                return "'" + string.Join("','", pks.ToArray()) + "'";
        }
    }
}
