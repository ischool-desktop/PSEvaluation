using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Aspose.Words;
using FISCA.Presentation;
using FISCA.Presentation.Controls;
using JHEvaluation.ScoreCalculation;
using Campus.Rating;
using JHSchool.Data;
using Campus.Report;
using K12.BusinessLogic;
using System.Linq;
using System.Reflection;
using Aspose.Cells;
using Campus.ePaper;
using System.IO;

namespace KH_StudentScoreSummaryReport
{
    public partial class PrintForm : BaseForm, IStatusReporter
    {
        //internal const string ConfigName = "StudentScoreSummaryReport";
        internal const string ConfigName = "KH_StudentScoreSummaryReport";

        private List<string> StudentIDs { get; set; }

        private ReportPreference Preference { get; set; }

        private BackgroundWorker MasterWorker = new BackgroundWorker();

        private BackgroundWorker ConvertToPDF_Worker = new BackgroundWorker();

        private List<ReportStudent> PrintStudents = new List<ReportStudent>();

        private string fbdPath = "";

        private DoWorkEventArgs e_For_ConvertToPDF_Worker;

        public PrintForm(List<string> studentIds)
        {
            InitializeComponent();

            StudentIDs = studentIds;
            Preference = new ReportPreference(ConfigName, Properties.Resources.高雄學生免試入學高中在校成績證明書);
            MasterWorker.DoWork += new DoWorkEventHandler(MasterWorker_DoWork);
            MasterWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(MasterWorker_RunWorkerCompleted);

            ConvertToPDF_Worker.DoWork += new DoWorkEventHandler(ConvertToPDF_Worker_DoWork);
            ConvertToPDF_Worker.WorkerReportsProgress = true;
            ConvertToPDF_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ConvertToPDF_Worker_RunWorkerCompleted);

            ConvertToPDF_Worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage(e.UserState.ToString(), e.ProgressPercentage);
            };

            //rbDomainOnly.Checked = (Preference.ListMethod == ListMethod.DomainOnly);            
            //rbSubjectOnly.Checked = (Preference.ListMethod == ListMethod.SubjectOnly);
            //chkRank.Checked = Preference.PrintRank;
            //chkPercentage.Checked = Preference.PrintRankPercentage;
            txtGraduateDate.Text = Preference.GraduateDate;
            txtEntranceDate.Text = Preference.EntranceDate;
            txtFinalComputeDate.Text = Preference.FinalComputeDate;
            //chkRankFilter.Checked = Preference.FilterRankScope;
            //intRankStart.Value = Preference.RankStart;
            //intRankEnd.Value = Preference.RankEnd;
            rtnPDF.Checked = Preference.ConvertToPDF;
            chkUploadEpaper.Checked = Preference.isUploadEPaper;

            OneFileSave.Checked = Preference.OneFileSave;

            chk1Up.Checked = false;
            chk1Down.Checked = false;
            chk2Up.Checked = false;
            chk2Down.Checked = false;
            chk3Up.Checked = false;
            chk3Down.Checked = false;
            foreach (int each in Preference.PrintSemesters)
            {
                if (each == 1) chk1Up.Checked = true;
                if (each == 2) chk1Down.Checked = true;
                if (each == 3) chk2Up.Checked = true;
                if (each == 4) chk2Down.Checked = true;
                if (each == 5) chk3Up.Checked = true;
                if (each == 6) chk3Down.Checked = true;
            }

            //intRankStart.Enabled = chkRankFilter.Checked;
            //intRankEnd.Enabled = chkRankFilter.Checked;
            // 只能印領域
            //rbDomainOnly.Checked = true;
            //checkExportFile.Checked = true;

            string DALMessage = "『";
            foreach (Assembly Assembly in AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.Equals("KH_StudentScoreSummaryReport")))
                DALMessage += Assembly.GetName().Version;
            DALMessage += "』";
            this.Text += DALMessage;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //三下勾選時出現提醒成績不被計算
            if (chk3Down.Checked)
                MsgBox.Show("「均衡學習」成績依規定只採計到9上，故9下成績不被計算。","ischool");

            //Report.CheckExportFile = checkExportFile.Checked;

            DALTransfer._SchoolType = DALTransfer.SchoolType.高中;
            //if (chkRankFilter.Checked)
            //{
            //    if (intRankStart.Value > intRankEnd.Value)
            //    {
            //        MsgBox.Show("請輸入正確的名次範圍。");
            //        return;
            //    }
            //}

            //if (rbDomainOnly.Checked)
                Preference.ListMethod = ListMethod.DomainOnly;
            //else
            //    Preference.ListMethod = ListMethod.SubjectOnly;

            //Preference.PrintRank = chkRank.Checked;
            //Preference.PrintRankPercentage = chkPercentage.Checked;
            Preference.GraduateDate = txtGraduateDate.Text;
            Preference.EntranceDate = txtEntranceDate.Text;
            Preference.FinalComputeDate = txtFinalComputeDate.Text;
            //Preference.FilterRankScope = chkRankFilter.Checked;
            //Preference.RankStart = intRankStart.Value;
            //Preference.RankEnd = intRankEnd.Value;
            Preference.ConvertToPDF = rtnPDF.Checked;
            Preference.isUploadEPaper = chkUploadEpaper.Checked;

            Preference.OneFileSave = OneFileSave.Checked;

            Preference.PrintSemesters.Clear();
            if (chk1Up.Checked) Preference.PrintSemesters.Add(1);
            if (chk1Down.Checked) Preference.PrintSemesters.Add(2);
            if (chk2Up.Checked) Preference.PrintSemesters.Add(3);
            if (chk2Down.Checked) Preference.PrintSemesters.Add(4);
            if (chk3Up.Checked) Preference.PrintSemesters.Add(5);
            if (chk3Down.Checked) Preference.PrintSemesters.Add(6);

            Preference.Save(); //儲存設定值。

            Util.DisableControls(this);
            MasterWorker.RunWorkerAsync();
        }

        private void MasterWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            StudentScore.SetClassMapping();

            // 取得學生類別List
//            List<K12.Data.StudentTagRecord> StudTagRecList = K12.Data.StudentTag.SelectByStudentIDs(StudentIDs);
            List<K12.Data.StudentTagRecord> StudTagRecList = K12.Data.StudentTag.SelectAll();

            // 過濾不排學生ID
            List<string> NonStudentIDList = DALTransfer.GetNonRankStudentIDFromUDTByStudentTag(StudTagRecList, DALTransfer.SchoolType.高中);
            foreach (string id in NonStudentIDList)
                if (StudentIDs.Contains(id))
                    StudentIDs.Remove(id);

             PrintStudents = StudentIDs.ToReportStudent();

            if (Preference.PrintRank || Preference.PrintRankPercentage || Preference.FilterRankScope)
            {
                #region 如果要排名。
                //List<ReportStudent> RatingStudents = Util.GetAllStudents();
                List<ReportStudent> RatingStudents = Util.GetStudentsDef(NonStudentIDList);

                RatingStudents.ToSC().ReadSemesterScore(this);
                RatingStudents.ToSC().ReadSemesterHistory(this);

                List<IScoreParser<ReportStudent>> parsers = new List<IScoreParser<ReportStudent>>();
                List<SLearningDomainParser> allsems = new List<SLearningDomainParser>();

                parsers.Add(new LearningDomainParser(Preference.PrintSemesters));
                allsems.Add(new SLearningDomainParser(1, 1));
                allsems.Add(new SLearningDomainParser(1, 2));
                allsems.Add(new SLearningDomainParser(2, 1));
                allsems.Add(new SLearningDomainParser(2, 2));
                allsems.Add(new SLearningDomainParser(3, 1));
                allsems.Add(new SLearningDomainParser(3, 2));
                allsems.Add(new SLearningDomainParser(7, 1));
                allsems.Add(new SLearningDomainParser(7, 2));
                allsems.Add(new SLearningDomainParser(8, 1));
                allsems.Add(new SLearningDomainParser(8, 2));
                allsems.Add(new SLearningDomainParser(9, 1));
                allsems.Add(new SLearningDomainParser(9, 2));
                foreach (SLearningDomainParser each in allsems)
                    parsers.Add(each);

                // 將學生加入年排名
                List<RatingScope<ReportStudent>> scopes = RatingStudents.ToGradeYearScopes();
                foreach (RatingScope<ReportStudent> each in scopes)
                    foreach (IScoreParser<ReportStudent> parser in parsers)
                        each.Rank(parser, PlaceOptions.Unsequence);

                Dictionary<string, StudentScore> DicPrintStudents = PrintStudents.ToSC().ToDictionary();

                foreach (ReportStudent each in RatingStudents)
                {
                    if (DicPrintStudents.ContainsKey(each.Id))
                        DicPrintStudents[each.Id] = each;

                    //each.Places["學習領域"].Percentage
                    //each.Places["學習領域"].GetPercentage();
                    //each.Places["學習領域"].Score
                }

                // 整理全部學生年排名
                DALTransfer.StudRankScoreDict.Clear();

                // 建立Key
                foreach (RatingScope<ReportStudent> scope in scopes)
                {
                    DALTransfer.StudRankScoreDict.Add(scope.Name, new Dictionary<string, List<StudRankScore>>());
                    DALTransfer.StudRankScoreDict[scope.Name].Add("學期總平均", new List<StudRankScore>());
                    foreach (SLearningDomainParser semsIndex in allsems)
                        DALTransfer.StudRankScoreDict[scope.Name].Add(semsIndex.Name, new List<StudRankScore>());
                }

                foreach (RatingScope<ReportStudent> scope in scopes)
                {
                    // 學習領域
                    foreach (ReportStudent stud in (from xx in scope where xx.Places.NS("年排名").Contains("學習領域") select xx).ToList())
                    {
                        StudRankScore srs = new StudRankScore();
                        srs.StudentID = stud.StudentID;
                        srs.Place = stud.Places.NS("年排名")["學習領域"];
                        DALTransfer.StudRankScoreDict[scope.Name]["學期總平均"].Add(srs);
                    }

                    foreach (SLearningDomainParser semsIndex in allsems)
                    {
                        foreach (ReportStudent stud in (from xx in scope where xx.Places.NS("年排名").Contains(semsIndex.Name) select xx).ToList())
                        {
                            StudRankScore srs = new StudRankScore();
                            srs.StudentID = stud.StudentID;
                            srs.Place = stud.Places.NS("年排名")[semsIndex.Name];
                            DALTransfer.StudRankScoreDict[scope.Name][semsIndex.Name].Add(srs);
                        }
                    }
                }

                #region 註解掉的程式
                //foreach (ReportStudent stud in RatingStudents)
                //{
                //-----------------
                //foreach (RatingScope<ReportStudent> scope in scopes)
                //{
                //    if (!DALTransfer.StudRankScoreDict.ContainsKey(scope.Name))
                //        DALTransfer.StudRankScoreDict.Add(scope.Name, new Dictionary<string, List<StudRankScore>>());

                //    foreach (ReportStudent stud in scope)
                //    {                        
                //        // 學期總平均
                //            if (stud.Places.NS("年排名").Contains("學習領域"))
                //            {
                //                StudRankScore srs = new StudRankScore();
                //                srs.StudentID = stud.StudentID;
                //                srs.Place = stud.Places.NS("年排名")["學習領域"];
                //                if (DALTransfer.StudRankScoreDict[scope.Name].ContainsKey("學期總平均"))
                //                {
                //                    DALTransfer.StudRankScoreDict[scope.Name]["學期總平均"].Add(srs);
                //                }
                //                else
                //                {
                //                    List<StudRankScore> srsList = new List<StudRankScore>();
                //                    srsList.Add(srs);
                //                    DALTransfer.StudRankScoreDict[scope.Name].Add("學期總平均", srsList);
                //                }
                //            }

                //            foreach (SLearningDomainParser semsIndex in allsems)
                //            {
                               
                //                    if (stud.Places.NS("年排名").Contains(semsIndex.Name))
                //                    {
                //                        StudRankScore srs = new StudRankScore();
                //                        srs.StudentID = stud.StudentID;
                //                        srs.Place = stud.Places.NS("年排名")[semsIndex.Name];
                //                        if (DALTransfer.StudRankScoreDict[scope.Name].ContainsKey(semsIndex.Name))
                //                        {
                //                            DALTransfer.StudRankScoreDict[scope.Name][semsIndex.Name].Add(srs);
                //                        }
                //                        else
                //                        {
                //                            List<StudRankScore> srsList = new List<StudRankScore>();
                //                            srsList.Add(srs);
                //                            DALTransfer.StudRankScoreDict[scope.Name].Add(semsIndex.Name, srsList);
                //                        }


                //                    }
                //            }
                        
                //    }
                //}
                //------------------------------------
                //}

                //// 排序對照年排名資料 debug 用
                //for (int i = 1; i <= 9; i++)
                //{
                //    if (DALTransfer.StudRankScoreDict.ContainsKey(i.ToString()))
                //    {
                //        string idx = i.ToString();
                //        foreach (SLearningDomainParser semsIndex in allsems)
                //        {
                //            if (DALTransfer.StudRankScoreDict[idx].ContainsKey(semsIndex.Name))
                //            {
                //                var x = from xx in DALTransfer.StudRankScoreDict[idx][semsIndex.Name] orderby xx.Place.Score descending select xx;
                //                DALTransfer.StudRankScoreDict[idx][semsIndex.Name] = x.ToList();

                //            }

                //        }
                //    }
                //}
                #endregion

                #region Debug 用區斷
                Workbook wb = new Workbook();
                Dictionary<string, ReportStudent> students = RatingStudents.ToDictionary(x => x.Id);
                int wstCot = 0;
                // 排序對照年排名資料 debug 用

                foreach (RatingScope<ReportStudent> scope in scopes)
                {
                    if (DALTransfer.StudRankScoreDict.ContainsKey(scope.Name))
                    {
                        string idx = scope.Name;
                        
                        string AvggName = "學期總平均";
                        if (DALTransfer.StudRankScoreDict[idx].ContainsKey(AvggName))
                        {
                            int row = 1;

                            wb.Worksheets.Add();
                            wb.Worksheets[wstCot].Name = idx + AvggName;
                            wb.Worksheets[wstCot].Cells[0, 0].PutValue("成績");
                            wb.Worksheets[wstCot].Cells[0, 1].PutValue("年排名");
                            wb.Worksheets[wstCot].Cells[0, 2].PutValue("分類");
                            wb.Worksheets[wstCot].Cells[0, 3].PutValue("年級");
                            wb.Worksheets[wstCot].Cells[0, 4].PutValue("SID");
                            wb.Worksheets[wstCot].Cells[0, 5].PutValue("StudCount");

                            foreach (StudRankScore xx in DALTransfer.StudRankScoreDict[idx][AvggName].OrderByDescending(x => x.Place.Score))
                            {
                                wb.Worksheets[wstCot].Cells[row, 0].PutValue(xx.Place.Score);
                                wb.Worksheets[wstCot].Cells[row, 1].PutValue(xx.Place.Percentage);
                                wb.Worksheets[wstCot].Cells[row, 2].PutValue(AvggName);
                                wb.Worksheets[wstCot].Cells[row, 3].PutValue(idx);
                                wb.Worksheets[wstCot].Cells[row, 4].PutValue(xx.StudentID);
                                wb.Worksheets[wstCot].Cells[row, 5].PutValue(xx.Place.Radix);
                                wb.Worksheets[wstCot].Cells[row, 6].PutValue(students[xx.StudentID].GradeYear);
                                row++;
                            }
                            wstCot++;
                        }

                        foreach (SLearningDomainParser semsIndex in allsems)
                        {
                            int row = 1;

                            wb.Worksheets.Add();
                            wb.Worksheets[wstCot].Name = idx + semsIndex.Name.Replace(":", "-");
                            wb.Worksheets[wstCot].Cells[0, 0].PutValue("成績");
                            wb.Worksheets[wstCot].Cells[0, 1].PutValue("年排名");
                            wb.Worksheets[wstCot].Cells[0, 2].PutValue("分類");
                            wb.Worksheets[wstCot].Cells[0, 3].PutValue("年級");
                            wb.Worksheets[wstCot].Cells[0, 4].PutValue("SID");
                            wb.Worksheets[wstCot].Cells[0, 5].PutValue("StudCount");

                            if (DALTransfer.StudRankScoreDict[idx].ContainsKey(semsIndex.Name))
                            {
                                foreach (StudRankScore xx in DALTransfer.StudRankScoreDict[idx][semsIndex.Name].OrderByDescending(x => x.Place.Score))
                                {
                                    wb.Worksheets[wstCot].Cells[row, 0].PutValue(xx.Place.Score);
                                    wb.Worksheets[wstCot].Cells[row, 1].PutValue(xx.Place.Percentage);
                                    wb.Worksheets[wstCot].Cells[row, 2].PutValue(semsIndex.Name);
                                    wb.Worksheets[wstCot].Cells[row, 3].PutValue(semsIndex.Grade);
                                    wb.Worksheets[wstCot].Cells[row, 4].PutValue(xx.StudentID);
                                    wb.Worksheets[wstCot].Cells[row, 5].PutValue(xx.Place.Radix);
                                    wb.Worksheets[wstCot].Cells[row, 6].PutValue(students[xx.StudentID].GradeYear);
                                    row++;
                                }

                            }
                            wstCot++;

                        }
                    }
                }
                
                for (int i = 0; i < wb.Worksheets.Count; i++)
                {
                    if (wb.Worksheets[i].Cells.MaxDataRow < 2)
                        wb.Worksheets.RemoveAt(i);
                }

                try
                {
                    wb.Save(Application.StartupPath + "\\樂學成績debug.xls", FileFormatType.Excel2003);

                   // System.Diagnostics.Process.Start(Application.StartupPath + "\\樂學成績debug.xls");
                }
                catch (Exception ex)
                {

                }
                #endregion

                if (Preference.FilterRankScope)
                {
                    List<ReportStudent> filteredStudent = new List<ReportStudent>();
                    foreach (ReportStudent each in DicPrintStudents.Values.ToSS())
                    {
                        //看是否有「學習領域」的年排名。
                        if (each.Places.NS("年排名").Contains(LearningDomainParser.PlaceName))
                        {
                            Place place = each.Places.NS("年排名")[LearningDomainParser.PlaceName];
                            if (place.Level >= Preference.RankStart && place.Level <= Preference.RankEnd)
                                filteredStudent.Add(each);
                        }
                        PrintStudents = filteredStudent;
                    }
                }
                else
                    PrintStudents = new List<ReportStudent>(DicPrintStudents.Values.ToSS());
                #endregion
            }
            else
            {
                PrintStudents.ToSC().ReadSemesterScore(this);
                PrintStudents.ToSC().ReadSemesterHistory(this);
            }

            List<StudentScore> CalcStudents;
            if (PrintStudents.Count <= 0)
            {
                Feedback("", -1);  //把 Status bar Reset...
                throw new ArgumentException("沒有任何學生資料可列印。");
            }
            else
            {
                CalcStudents = PrintStudents.ToSC();
                CalcStudents.ReadCalculationRule(this); //讀取成績計算規則。

                #region 讀取缺曠獎懲。
                //List<JHMoralScoreRecord> jhmorals = JHSchool.Data.JHMoralScore.SelectByStudentIDs(StudentIDs);
                List<AutoSummaryRecord> jhsummary = AutoSummary.Select(StudentIDs, null);

                Dictionary<string, ReportStudent> DicStudents = PrintStudents.ToDictionary();
                //foreach (JHMoralScoreRecord each in jhmorals)
                foreach (AutoSummaryRecord each in jhsummary)
                {
                    if (!DicStudents.ContainsKey(each.RefStudentID)) continue;

                    SemesterData semester = new SemesterData(0, each.SchoolYear, each.Semester);
                    ReportStudent student = DicStudents[each.RefStudentID];

                    if (!student.Summaries.ContainsKey(semester))
                        student.Summaries.Add(semester, each.AutoSummary);
                }
                #endregion

                PrintStudents.ReadUpdateRecordDate(this);

                e.Result = new Report(PrintStudents, Preference).Print();

                e_For_ConvertToPDF_Worker = e;

                Feedback("列印完成", -1);
            }
        }

        private void MasterWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Util.EnableControls(this);

            if (e.Error == null)
            {
                Document doc = e.Result as Document;

                //單檔列印
                if (OneFileSave.Checked)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    fbd.Description = "請選擇儲存資料夾";
                    fbd.ShowNewFolderButton = true;

                    if (fbd.ShowDialog() == DialogResult.Cancel) return;

                    fbdPath = fbd.SelectedPath;


                    Util.DisableControls(this);
                    ConvertToPDF_Worker.RunWorkerAsync();
                                       
                }
                else
                {
                    if (Preference.ConvertToPDF )
                    {
                        MotherForm.SetStatusBarMessage("正在轉換PDF格式... 請耐心等候"); 
                    }

                    Util.DisableControls(this);
                    ConvertToPDF_Worker.RunWorkerAsync();


                    
                }


                // 檢查是否上傳電子報表
                if (chkUploadEpaper.Checked)
                {
                    try
                    {
                        ConvertAspose.Update_ePaperWordV14(TempData._ePaperMemStreamList, "高雄市免試入學在校成績證明書", PrefixStudent.系統編號);
                    }
                    catch (Exception ex)
                    {
                        MsgBox.Show("上傳電子報表發生錯誤," + ex.Message);
                    }
                }

            }
            else 
            {
                MsgBox.Show(e.Error.Message);
            }

            if (Preference.ConvertToPDF )
            {
                MotherForm.SetStatusBarMessage("正在轉換PDF格式", 0);
            }
            else 
            {
                MotherForm.SetStatusBarMessage("產生完成", 100);
            }
            
        }


        private void ConvertToPDF_Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Document doc = e_For_ConvertToPDF_Worker.Result as Document;

            if (!OneFileSave.Checked)
            {
                Util.Save(doc, "學生免試入學在校成績證明單", Preference.ConvertToPDF);
            }
            else
            {
                int i = 0;

                foreach (Section section in doc.Sections)
                {
                    // 依照 學號_身分字號_班級_座號_姓名 .doc 來存檔
                    string fileName = "";

                    Document document = new Document();
                    document.Sections.Clear();
                    document.Sections.Add(document.ImportNode(section, true));

                    fileName = PrintStudents[i].StudentNumber;

                    fileName += "_" + PrintStudents[i].IDNumber;

                    if (!string.IsNullOrEmpty(PrintStudents[i].RefClassID))
                    {
                        fileName += "_" + PrintStudents[i].Class.Name;
                    }
                    else
                    {
                        fileName += "_";
                    }

                    fileName += "_" + PrintStudents[i].SeatNo;

                    fileName += "_" + PrintStudents[i].Name;

                    //document.Save(fbd.SelectedPath + "\\" +fileName+ ".doc");

                    if (Preference.ConvertToPDF)
                    {
                        //string fPath = fbd.SelectedPath + "\\" + fileName + ".pdf";

                        string fPath = fbdPath + "\\" + fileName + ".pdf";

                        FileInfo fi = new FileInfo(fPath);

                        DirectoryInfo folder = new DirectoryInfo(Path.Combine(fi.DirectoryName, Path.GetRandomFileName()));
                        if (!folder.Exists) folder.Create();

                        FileInfo fileinfo = new FileInfo(Path.Combine(folder.FullName, fi.Name));

                        string XmlFileName = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".xml";
                        string PDFFileName = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".pdf";

                        document.Save(XmlFileName, Aspose.Words.SaveFormat.AsposePdf);

                        Aspose.Pdf.Pdf pdf1 = new Aspose.Pdf.Pdf();

                        pdf1.BindXML(XmlFileName, null);
                        pdf1.Save(PDFFileName);

                        if (File.Exists(fPath))
                            File.Delete(Path.Combine(fi.DirectoryName, fi.Name));

                        File.Move(PDFFileName, fPath);
                        folder.Delete(true);

                        int percent = (((i + 1) * 100 / doc.Sections.Count));

                        ConvertToPDF_Worker.ReportProgress(percent, "PDF轉換中...進行到" + (i + 1) + "/" + doc.Sections.Count + "個檔案");
                    }
                    else
                    {
                        document.Save(fbdPath + "\\" + fileName + ".doc");

                        int percent = (((i + 1) * 100 / doc.Sections.Count));

                        ConvertToPDF_Worker.ReportProgress(percent, "Doc存檔...進行到" + (i + 1) + "/" + doc.Sections.Count + "個檔案");
                    }

                    i++;


                }
            }
        }



        private void ConvertToPDF_Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Util.EnableControls(this);

            if (Preference.ConvertToPDF)
            {
                MotherForm.SetStatusBarMessage("PDF轉換完成", 100);
                
            }
            else 
            {
                MotherForm.SetStatusBarMessage("存檔完成", 100);
                
            }
            
        }

        



        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region IStatusReporter 成員

        public void Feedback(string message, int percentage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, int>(Feedback), new object[] { message, percentage });
            }
            else
            {
                if (percentage < 0)
                    MotherForm.SetStatusBarMessage(message);
                else
                    MotherForm.SetStatusBarMessage(message, percentage);

                Application.DoEvents();
            }
        }

        #endregion

        private void lnkAbsence_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Dictionary<string, List<string>> result = Preference.AcceptAbsences.PeriodOptionsFromString();
            DialogResult dr = AbsenceTypeForm.PopupSettingForm(result, out result);

            if (dr == DialogResult.OK)
            {
                Preference.AcceptAbsences = result.PeriodOptionsToString();
                Preference.Save();
            }
        }

        private void lnkTemplate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportTemplate defaultTemplate = new ReportTemplate(Properties.Resources.高雄學生免試入學高中在校成績證明書, TemplateType.Word);
            TemplateSettingForm form = new TemplateSettingForm(Preference.Template, defaultTemplate);
            form.DefaultFileName = "學生免試入學在校成績證明單.doc";

            if (form.ShowDialog() == DialogResult.OK)
            {
                Preference.Template = (form.Template == defaultTemplate) ? null : form.Template;
                Preference.Save();
            }
        }

        private void RankScope_CheckedChanged(object sender, EventArgs e)
        {
            //intRankStart.Enabled = chkRankFilter.Checked;
            //intRankEnd.Enabled = chkRankFilter.Checked;
        }

        private void lnkSetStudType_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetStudAddWeight ssaw = new SetStudAddWeight();
            ssaw.Show();
        }

        private void lnkSetStudType_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetStudAddWeight ssaw = new SetStudAddWeight();
            ssaw.Show();
        }

        private void PrintForm_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.MinimumSize = this.Size;
        }
    }
}
