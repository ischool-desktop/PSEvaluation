using System;
using System.Collections.Generic;
using System.Text;
using JHEvaluation.ScoreCalculation;
using Aspose.Words;
using System.IO;
using Aspose.Words.Reporting;
using JHEvaluation.ScoreCalculation.ScoreStruct;
using Campus.Rating;
using System.Xml;
using System.Globalization;
using Aspose.Words.Drawing;
using System.Drawing;
using Subj = JHSchool.Evaluation.Subject;



namespace JointAdmissionModule.StudentScoreSummaryReport
{
    internal class ReportEnglish
    {
        private const int DataRowOffset = 2;

        private ReportPreference PrintSetting { get; set; }

        private List<ReportStudent> Students { get; set; }

        private Dictionary<string, List<string>> PrintAbsences { get; set; }

        private List<string> DetailDomain { get; set; }

        //英文成績單用。
        private bool PrintScore { get; set; }

        public ReportEnglish(List<ReportStudent> students, ReportPreference printSetting)
        {
            Students = students;
            Students.Sort(delegate(ReportStudent x, ReportStudent y)
            {
                return x.OrderString.CompareTo(y.OrderString);
            });

            PrintSetting = printSetting;

            DetailDomain = new List<string>();

            //新竹的要把語文詳列。
            if (Program.Mode == ModuleMode.HsinChu)
                DetailDomain.Add("語文");

            DetailDomain.Add("彈性課程");
            DetailDomain.Add("");

            PrintAbsences = printSetting.AcceptAbsences.PeriodOptionsFromString();
        }

        public Document Print(bool printScore)
        {
            PrintScore = printScore;
            Document doc = PrintSetting.Template.ToDocument();

            doc.MailMerge.MergeField += new Aspose.Words.Reporting.MergeFieldEventHandler(MailMerge_MergeField);

            doc.MailMerge.Execute(new MergeDataSource(Students, PrintSetting));

            return doc;
        }

        //最好這程式有人能維護的了.......
        private void MailMerge_MergeField(object sender, MergeFieldEventArgs e)
        {
            //不是 Fix 開頭的合併欄位不處理。
            if (!e.FieldName.ToUpper().StartsWith("Fix".ToUpper())) return;

            DocumentBuilder builder = new DocumentBuilder(e.Document);

            ReportStudent student = e.FieldValue as ReportStudent;

            //如果合併值不是 ReportStudent 就跳過...意思是有問題...。
            if (student == null) return;

            builder.MoveToField(e.Field, true);
            e.Field.Remove();

            if (e.FieldName == "Fix:科目資訊")
            {
                #region 列印年級學期資訊(有點複雜)。
                SemesterDataCollection semses = student.SHistory.GetGradeYearSemester();
                //Row SemesterRow = builder.CurrentParagraph.ParentNode.ParentNode.NextSibling as Row; //下一個 Row。
                //Paragraph originParagraph = builder.CurrentParagraph;

                int count = 0;//, offset = 1;

                foreach (SemesterData each in semses)
                {
                    //string currentGradeyear = Util.GetGradeyearString(each.GradeYear.ToString());

                    ////如果沒有年級，就跳過。
                    //if (string.IsNullOrEmpty(currentGradeyear)) continue;

                    //builder.Write(currentGradeyear + "年級");
                    Paragraph nextPh = Util.NextCell(builder.CurrentParagraph);
                    if (nextPh == null) break; //沒有下一個 Cell ，就不印資料了。
                    builder.MoveTo(nextPh);

                    //Paragraph resetParagraph = builder.CurrentParagraph;
                    //SemesterRow.Cells[count + offset].Write(builder, GetSemesterString(each));

                    SemesterData semester = new SemesterData(0, each.SchoolYear, each.Semester);
                    if (!student.HeaderList.ContainsKey(semester))
                        student.HeaderList.AddRaw(each, count); //不要懷疑，這是對的。

                    //builder.MoveTo(resetParagraph);
                    count++;
                }

                //builder.MoveTo(originParagraph);
                //Paragraph nextParagrap = originParagraph;
                //string previousGradeyear = GetText(originParagraph);
                //while ((nextParagrap = Util.NextCell(nextParagrap)) != null)
                //{
                //    if (GetText(nextParagrap) == previousGradeyear)
                //        (nextParagrap.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;

                //    previousGradeyear = GetText(nextParagrap);
                //}
                #endregion

                #region 列印科目資料(爆炸複雜)

                Row template = builder.CurrentParagraph.ParentNode.ParentNode as Row;
                Table table = template.ParentNode as Table;

                if (PrintSetting.ListMethod == ListMethod.DomainOnly)
                    PrintDomainOnly(builder, student, template, table);
                else
                    PrintSubjectOnly(builder, student, template, table);

                //設定表格下方線寬。
                double borderWidth = (template.NextSibling as Row).Cells[0].CellFormat.Borders.Bottom.LineWidth;
                foreach (Cell each in (template.PreviousSibling as Row).Cells)
                    each.CellFormat.Borders.Bottom.LineWidth = borderWidth;

                template.NextSibling.Remove();
                template.Remove();
                #endregion
            }
            if (e.FieldName == "Fix:照片")
            {
                if (student.GraduatePhoto != null)
                {
                    Shape photo = builder.InsertImage(student.GraduatePhoto);
                    Cell cell = builder.CurrentParagraph.ParentNode as Cell;
                    Row row = cell.ParentRow;

                    double rectHeight = row.RowFormat.Height, rectWidth = cell.CellFormat.Width;

                    double heightRate = (rectHeight / photo.Height);
                    double widthRate = (rectWidth / photo.Width);
                    double rate = 0;
                    if (heightRate < widthRate)
                        rate = heightRate;
                    else
                        rate = widthRate;

                    photo.Width = photo.Width * rate;
                    photo.Height = photo.Height * rate;
                }
            }
        }

        private void PrintSubjectOnly(DocumentBuilder builder, ReportStudent student, Row template, Table table)
        {
            #region 列印科目
            UniqueSet<RowHeader> RowIndexs = new UniqueSet<RowHeader>();

            #region 列印 RowHeader
            Dictionary<string, string> subjToDomain = new Dictionary<string, string>();
            foreach (SemesterData semester in student.SHistory.GetGradeYearSemester())
            {
                SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);
                if (!student.SemestersScore.Contains(sysems)) continue;

                SemesterScore semsscore = student.SemestersScore[sysems];

                foreach (string strSubject in semsscore.Subject)
                {
                    SemesterSubjectScore subject = semsscore.Subject[strSubject];

                    if (!subjToDomain.ContainsKey(strSubject))
                        subjToDomain.Add(strSubject, subject.Domain);

                    RowHeader header = new RowHeader(subjToDomain[strSubject], strSubject);
                    header.IsDomain = false;

                    if (!RowIndexs.Contains(header))
                        RowIndexs.Add(header);
                }
            }

            List<RowHeader> sortedHeaders = RowIndexs.ToList();
            sortedHeaders.Sort(delegate(RowHeader x, RowHeader y)
            {
                Subj xx = new JHSchool.Evaluation.Subject(x.Subject, x.Domain);
                Subj yy = new JHSchool.Evaluation.Subject(y.Subject, y.Domain);

                return xx.CompareTo(yy);
            });

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

                string gn = "";
                //Additional Classes

                string subjCName = header.Subject;
                string subjEName = Subj.GetSubjectEnglish(header.Subject);
                string subjString = header.Subject + (string.IsNullOrEmpty(subjEName) ? "" : " " + subjEName);

                datarow.Cells[0].Write(builder, subjString);

                //if (IsFlexible(header.Domain))
                //{
                //    gn = "彈性課程\nAdditional Classes";
                //    //把空白的領域當成「彈性課程」。
                //    datarow.Cells[0].Write(builder, gn);
                //    datarow.Cells[1].Write(builder, subjString);
                //}
                //else
                //{
                //}
            }
            #endregion

            #region 填資料
            //填資料
            foreach (RowHeader header in indexHeaders)
            {
                SemesterDataCollection semesters = new SemesterDataCollection();
                Row row = table.Rows[header.Index + DataRowOffset];
                foreach (SemesterData semester in student.SHistory.GetGradeYearSemester())
                {
                    SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);
                    semesters.Add(sysems);

                    if (!student.SemestersScore.Contains(sysems)) continue;
                    if (!student.HeaderList.ContainsKey(sysems)) continue;

                    int columnIndex = student.HeaderList[sysems];
                    SemesterScore semsscore = student.SemestersScore[sysems];

                    decimal? score = null;
                    decimal? weight = null;

                    //if (header.IsDomain)
                    //{
                    //    if (semsscore.Domain.Contains(header.Domain))
                    //    {
                    //        score = semsscore.Domain[header.Domain].Value;
                    //        weight = semsscore.Domain[header.Domain].Weight;
                    //    }
                    //}
                    //else
                    //{
                    if (semsscore.Subject.Contains(header.Subject))
                    {
                        score = semsscore.Subject[header.Subject].Value;
                        weight = semsscore.Subject[header.Subject].Weight;
                    }
                    //}

                    if (!score.HasValue) continue;
                    if (!weight.HasValue) weight = 0;

                    if (PrintScore)
                        row.Cells[columnIndex + 2].Write(builder, score.Value + "");
                    else
                        row.Cells[columnIndex + 2].Write(builder, Util.GetDegreeEnglish(score.Value));
                }
            }
            #endregion

            #region 合併相關欄位。
            string previousCellDomain = string.Empty;
            foreach (RowHeader header in indexHeaders)
            {
                Row row = table.Rows[header.Index + DataRowOffset];

                //if (IsFlexible(header.Domain))
                //{
                //    if (previousCellDomain == row.Cells[0].ToTxt())
                //        row.Cells[0].CellFormat.VerticalMerge = CellMerge.Previous;
                //    else
                //        row.Cells[0].CellFormat.VerticalMerge = CellMerge.First;
                //}
                //else
                {
                    row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                    row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                }

                previousCellDomain = row.Cells[0].ToTxt();
            }
            #endregion

            #endregion
        }

        private void PrintDomainOnly(DocumentBuilder builder, ReportStudent student, Row template, Table table)
        {
            #region 列印領域
            UniqueSet<RowHeader> RowIndexs = new UniqueSet<RowHeader>();

            #region 列印 RowHeader
            foreach (SemesterData semester in student.SHistory.GetGradeYearSemester())
            {
                SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);

                //如果不包含該學期成績資料，就跳過。
                if (!student.SemestersScore.Contains(sysems)) continue;

                SemesterScore semsscore = student.SemestersScore[sysems];

                //準備彈性課程的科目(要詳列出來)。
                foreach (string strSubject in semsscore.Subject)
                {
                    SemesterSubjectScore subject = semsscore.Subject[strSubject];

                    if (DetailDomain.Contains(subject.Domain))
                    {
                        RowHeader header = new RowHeader(subject.Domain, strSubject);
                        header.IsDomain = false;

                        if (!RowIndexs.Contains(header))
                            RowIndexs.Add(header);
                    }
                }

                //準備領域資料。
                foreach (string strDomain in semsscore.Domain)
                {
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
                    string domainCName = header.Domain;
                    string domainEName = Subj.GetDomainEnglish(header.Domain);
                    string domainString = domainCName + (string.IsNullOrEmpty(domainEName) ? "" : domainEName);

                    if (hasGroup)
                    {
                        string gn = groupName;

                        if (groupName == "語文")
                            gn = "語文\nLanguage";

                        datarow.Cells[0].Write(builder, gn);
                        datarow.Cells[1].Write(builder, domainString);
                    }
                    else
                        datarow.Cells[0].Write(builder, domainString);
                }
                else
                {
                    string gn = "";
                    //Additional Classes

                    if (IsFlexible(header.Domain))
                        gn = "彈性課程\nAdditional Classes";
                    else
                        gn = header.Domain;

                    string subjCName = header.Subject;
                    string subjEName = Subj.GetSubjectEnglish(header.Subject);
                    string subjString = header.Subject + (string.IsNullOrEmpty(subjEName) ? "" : "\n" + subjEName);

                    //把空白的領域當成「彈性課程」。
                    string domain = gn;
                    datarow.Cells[0].Write(builder, domain);
                    datarow.Cells[1].Write(builder, subjString);
                }
            }
            #endregion

            #region 填資料
            //Row RatingRow = null;
            foreach (RowHeader header in indexHeaders)
            {
                SemesterDataCollection semesters = new SemesterDataCollection();
                Row row = table.Rows[header.Index + DataRowOffset];
                foreach (SemesterData semester in student.SHistory.GetGradeYearSemester())
                {
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

                    if (!score.HasValue) continue;
                    if (!weight.HasValue) weight = 0;

                    if (PrintScore)
                        row.Cells[columnIndex + 2].Write(builder, score.Value + "");
                    else
                        row.Cells[columnIndex + 2].Write(builder, Util.GetDegreeEnglish(score.Value));
                }

            }

            #endregion

            #region 合併相關欄位。
            string previousCellDomain = string.Empty;
            foreach (RowHeader header in indexHeaders)
            {
                bool hasGroup = !string.IsNullOrEmpty(Subj.GetDomainGroup(header.Domain));
                string groupName = Subj.GetDomainGroup(header.Domain);

                Row row = table.Rows[header.Index + DataRowOffset];

                if (previousCellDomain == row.Cells[0].ToTxt())
                    row.Cells[0].CellFormat.VerticalMerge = CellMerge.Previous;
                else
                    row.Cells[0].CellFormat.VerticalMerge = CellMerge.First;

                if (header.IsDomain)
                {
                    if (!hasGroup)
                    {
                        row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                        row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                    }
                }

                previousCellDomain = row.Cells[0].ToTxt();
            }
            #endregion

            #endregion
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
        /// 將領域、彈性課程分開排序。
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<RowHeader> SortHeader(List<RowHeader> list)
        {
            List<RowHeader> domains = new List<RowHeader>();
            List<RowHeader> subjects = new List<RowHeader>();

            foreach (RowHeader each in list)
            {
                if (Program.Mode == ModuleMode.HsinChu) //新竹..... = =''
                {
                    if (each.IsDomain || each.Domain == "語文")
                        domains.Add(each);
                    else
                        subjects.Add(each);
                }
                else
                {
                    if (each.IsDomain)
                        domains.Add(each);
                    else
                        subjects.Add(each);
                }
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
            result.AddRange(subjects);

            return result;
        }

        private string GetPlaceString(PlaceCollection places, string placeKey)
        {
            Place place = places[placeKey];
            decimal percentage = (100m * ((decimal)place.Level / (decimal)place.Radix));

            string result = string.Empty;

            if (PrintSetting.PrintRank)
                result = place.Level.ToString();

            if (PrintSetting.PrintRankPercentage)
                result = Math.Round(percentage, 0, MidpointRounding.AwayFromZero) + "%";

            if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                result = string.Format("{0}/{1}%", place.Level, Math.Round(percentage, 0, MidpointRounding.AwayFromZero));

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

            private string PrintDate = DateTime.Now.ToString(Util.EnglishFormat, Util.USCulture);

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

                if (fieldName.ToUpper().StartsWith("Fix:".ToUpper()))
                {
                    fieldValue = student;
                    return true;
                }

                switch (fieldName)
                {
                    //case "學號":
                    //    fieldValue = student.StudentNumber;
                    //    break;
                    //case "班級":
                    //    fieldValue = student.ClassName;
                    //    break;
                    //case "座號":
                    //    fieldValue = student.SeatNo;
                    //    break;
                    case "姓名":
                        fieldValue = student.Name;
                        break;
                    case "英文姓名":
                        fieldValue = student.EnglishName;
                        break;
                    case "身分證號":
                        fieldValue = student.IDNumber;
                        break;
                    //case "性別":
                    //    fieldValue = student.Gender;
                    //    break;
                    case "生日":
                        DateTime dtBirthday;

                        if (DateTime.TryParse(student.Birthday, out dtBirthday))
                            fieldValue = dtBirthday.ToString(Util.EnglishFormat, Util.USCulture);
                        else
                            fieldValue = string.Empty;

                        break;
                    case "列印日期":
                        DateTime dtPrintDate;

                        if (DateTime.TryParse(PrintDate, out dtPrintDate))
                            fieldValue = dtPrintDate.ToString(Util.EnglishFormat, Util.USCulture);
                        else
                            fieldValue = string.Empty;
                        break;
                    case "學校名稱":
                        fieldValue = K12.Data.School.ChineseName;
                        break;
                    case "英文學校名稱":
                        fieldValue = K12.Data.School.EnglishName;
                        break;
                    case "入學日期":
                        if (string.IsNullOrEmpty(Preference.EntranceDate))
                            fieldValue = student.EngEntranceDate;
                        else
                        {
                            DateTime engdt;
                            if (DateTime.TryParse(Preference.EntranceDate, out engdt))
                                fieldValue = engdt.ToString(Util.EnglishFormat, Util.USCulture);
                            else
                                fieldValue = Preference.EntranceDate;
                        }
                        break;
                    case "畢業日期":
                        if (string.IsNullOrEmpty(Preference.GraduateDate))
                            fieldValue = student.EngGraduateDate;
                        else
                        {
                            DateTime engdt;
                            if (DateTime.TryParse(Preference.GraduateDate, out engdt))
                                fieldValue = engdt.ToString(Util.EnglishFormat, Util.USCulture);
                            else
                                fieldValue = Preference.GraduateDate;
                        }
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
}
