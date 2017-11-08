using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using JointAdmissionModule.StudentScoreSummaryReport5;
using Campus.Rating;
using JHEvaluation.ScoreCalculation;
using JHEvaluation.ScoreCalculation.ScoreStruct;
using K12.Data.Configuration;
using System.Reflection;

namespace JointAdmissionModule.StudentScoreSummaryReport5
{
    public partial class PrintForm : FISCA.Presentation.Controls.BaseForm, IStatusReporter
    {
        BackgroundWorker _BGWorker;
        private List<string> StudentIDs { get; set; }

        public PrintForm(List<string> studentIds)
        {
            InitializeComponent();
            StudentIDs = studentIds;

            ConfigData cdUser = K12.Data.School.Configuration["五專"];
            string title = cdUser["ReportTitle"];

            if (!string.IsNullOrEmpty(cdUser["ReportTitle"]))
                txtReportTitle.Text = cdUser["ReportTitle"];

            this.MaximumSize = this.MinimumSize = this.Size;
            string DALMessage = "『";
            foreach (Assembly Assembly in AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.Equals("JointAdmissionModule")))
                DALMessage += Assembly.GetName().Version;
            DALMessage += "』";
            this.Text += DALMessage;

            _BGWorker = new BackgroundWorker();
            _BGWorker.DoWork += new DoWorkEventHandler(_BGWorker_DoWork);
            _BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWorker_RunWorkerCompleted);
        }

        void _BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnPrint.Enabled = btnExit.Enabled = true;
        }

        void _BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            StudentScore.SetClassMapping();

            // 取得學生類別List
            List<K12.Data.StudentTagRecord> StudTagRecList = K12.Data.StudentTag.SelectAll();

            // 過濾不排學生ID
            List<string> NonStudentIDList = DAL.DALTransfer.GetNonRankStudentIDFromUDTByStudentTag(StudTagRecList, JointAdmissionModule.DAL.DALTransfer.SchoolType.五專);
            foreach (string id in NonStudentIDList)
                if (StudentIDs.Contains(id))
                    StudentIDs.Remove(id);

            List<ReportStudent> PrintStudents = StudentIDs.ToReportStudent();


                List<ReportStudent> RatingStudents = Util.GetStudentsDef(NonStudentIDList);

                RatingStudents.ToSC().ReadSemesterScore(this);
                RatingStudents.ToSC().ReadSemesterHistory(this);

            // 讀取三學期學期領域成績，填入ReportStudent
                foreach (ReportStudent stud in RatingStudents)
                {
                    // 取得2,3年級學期歷程
                    List<SemesterData> semtList = (from xx in stud.SHistory where xx.GradeYear == 2 || xx.GradeYear == 8 || xx.GradeYear == 3 || xx.GradeYear == 9 select xx).ToList();

                    foreach (SemesterData sd in semtList)
                    {
                        foreach (SemesterScore ss in stud.SemestersScore)
                        {
                            // 二年級
                            if (sd.GradeYear == 2 || sd.GradeYear ==8)
                            {
                                if (sd.SchoolYear == ss.SchoolYear)
                                {
                                    if (ss.Semester == 1)
                                    {
                                        if (ss.LearnDomainScore.HasValue)
                                            stud.LearnDomainScore81 = ss.LearnDomainScore.Value;
                                        else
                                            stud.LearnDomainScore81 = 0;
                                    }

                                    if (ss.Semester == 2)
                                    {
                                        if (ss.LearnDomainScore.HasValue)
                                            stud.LearnDomainScore82 = ss.LearnDomainScore.Value;
                                        else
                                            stud.LearnDomainScore82 = 0;
                                    }
                                }
                            }

                            // 三上
                            if ((sd.GradeYear == 3 || sd.GradeYear == 9) && sd.Semester == 1)
                            {
                                if (sd.SchoolYear == ss.SchoolYear && ss.Semester == 1)
                                {
                                    if (ss.LearnDomainScore.HasValue)
                                        stud.LearnDomainScore91 = ss.LearnDomainScore.Value;
                                    else
                                        stud.LearnDomainScore91 = 0;
                                }
                            }
                        }
                    }
                }

                List<IScoreParser<ReportStudent>> parsers = new List<IScoreParser<ReportStudent>>();
                List<SLearningDomainParser> allsems = new List<SLearningDomainParser>();
                List<int> semInt = new List<int>();
                semInt.Add(3);
                semInt.Add(4);
                semInt.Add(5);
                parsers.Add(new LearningDomainParser(semInt));
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

                List<string> DomainNameList = DAL.DALTransfer.GetDomainNameList();
                //DomainNameList.Add("國語文");
                //DomainNameList.Add("英語");
                //DomainNameList.Add("數學");
                //DomainNameList.Add("社會");
                //DomainNameList.Add("自然與生活科技");
                //DomainNameList.Add("藝術與人文");
                //DomainNameList.Add("健康與體育");
                //DomainNameList.Add("綜合活動");
                // 放領域排名
                List<DomainParser> AllDomainSems = new List<DomainParser>();

            // 取得各領域三學期平均排名
                foreach (string name in DomainNameList)
                {
                    DomainParser dp = new DomainParser(name,100);                    
                    AllDomainSems.Add(dp);
                    //AllDomainSems.Add(new DomainParser(name));
                }
                foreach (SLearningDomainParser each in allsems)
                    parsers.Add(each);

                foreach(DomainParser each in AllDomainSems)
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

                }


                // 整理全部學生年排名
                DAL.DALTransfer.StudRankScoreDict.Clear();

                // 建立Key
                foreach (RatingScope<ReportStudent> scope in scopes)
                {
                    DAL.DALTransfer.StudRankScoreDict.Add(scope.Name, new Dictionary<string, List<JointAdmissionModule.DAL.StudRankScore>>());
                    DAL.DALTransfer.StudRankScoreDict[scope.Name].Add("學期總平均", new List<JointAdmissionModule.DAL.StudRankScore>());

                    // 領域平均
                    foreach (SLearningDomainParser semsIndex in allsems)
                        DAL.DALTransfer.StudRankScoreDict[scope.Name].Add(semsIndex.Name, new List<JointAdmissionModule.DAL.StudRankScore>());

                    // 各領域
                    foreach (DomainParser semsIdx in AllDomainSems)
                        DAL.DALTransfer.StudRankScoreDict[scope.Name].Add(semsIdx.Name, new List<JointAdmissionModule.DAL.StudRankScore>());
                }

                foreach (RatingScope<ReportStudent> scope in scopes)
                {
                    // 學習領域
                    foreach (ReportStudent stud in (from xx in scope where xx.Places.NS("年排名").Contains("學習領域") select xx).ToList())
                    {
                        DAL.StudRankScore srs = new JointAdmissionModule.DAL.StudRankScore();
                        srs.StudentID = stud.StudentID;
                        srs.Place = stud.Places.NS("年排名")["學習領域"];
                        DAL.DALTransfer.StudRankScoreDict[scope.Name]["學期總平均"].Add(srs);
                    }
                                        
                    // 領域平均
                    foreach (SLearningDomainParser semsIndex in allsems)
                    {
                        foreach (ReportStudent stud in (from xx in scope where xx.Places.NS("年排名").Contains(semsIndex.Name) select xx).ToList())
                        {
                            DAL.StudRankScore srs = new JointAdmissionModule.DAL.StudRankScore();
                            srs.StudentID = stud.StudentID;
                            srs.Place = stud.Places.NS("年排名")[semsIndex.Name];
                            DAL.DALTransfer.StudRankScoreDict[scope.Name][semsIndex.Name].Add(srs);
                        }
                    }

                    // 各領域
                    foreach (DomainParser semsIndex in AllDomainSems)
                    {
                        foreach (ReportStudent stud in (from xx in scope where xx.Places.NS("年排名").Contains(semsIndex.Name) select xx).ToList())
                        {
                            DAL.StudRankScore srs = new JointAdmissionModule.DAL.StudRankScore();
                            srs.StudentID = stud.StudentID;
                            srs.Place = stud.Places.NS("年排名")[semsIndex.Name];
                            DAL.DALTransfer.StudRankScoreDict[scope.Name][semsIndex.Name].Add(srs);
                        }
                    }
                }

                //foreach (RatingScope<ReportStudent> scope in scopes)
                //{
                //    if (!DAL.DALTransfer.StudRankScoreDict.ContainsKey(scope.Name))
                //        DAL.DALTransfer.StudRankScoreDict.Add(scope.Name, new Dictionary<string, List<DAL.StudRankScore>>());

                //    foreach (ReportStudent stud in scope)
                //    {
                //        if (stud.GradeYear != scope.Name) continue;

                //        // 學期總平均
                //        if (stud.Places.NS("年排名").Contains("學習領域"))
                //        {

                //            if (!DAL.DALTransfer.StudRankScoreDict.ContainsKey(stud.GradeYear))
                //                DAL.DALTransfer.StudRankScoreDict.Add(stud.GradeYear, new Dictionary<string, List<DAL.StudRankScore>>());

                //            DAL.StudRankScore srs = new JointAdmissionModule.DAL.StudRankScore();
                //            srs.StudentID = stud.StudentID;
                //            srs.Place = stud.Places.NS("年排名")["學習領域"];
                //            if (DAL.DALTransfer.StudRankScoreDict[stud.GradeYear].ContainsKey("學期總平均"))
                //            {
                //                DAL.DALTransfer.StudRankScoreDict[stud.GradeYear]["學期總平均"].Add(srs);
                //            }
                //            else
                //            {
                //                List<DAL.StudRankScore> srsList = new List<JointAdmissionModule.DAL.StudRankScore>();
                //                srsList.Add(srs);
                //                DAL.DALTransfer.StudRankScoreDict[stud.GradeYear].Add("學期總平均", srsList);
                //            }
                //        }


                //        // 學期平均
                //        foreach (SLearningDomainParser semsIndex in allsems)
                //        {
                //            if (stud.GradeYear != semsIndex.Grade.ToString()) continue;

                //            if (stud.Places.NS("年排名").Contains(semsIndex.Name))
                //            {
                //                if (!DAL.DALTransfer.StudRankScoreDict.ContainsKey(stud.GradeYear))
                //                    DAL.DALTransfer.StudRankScoreDict.Add(stud.GradeYear, new Dictionary<string, List<DAL.StudRankScore>>());

                //                DAL.StudRankScore srs = new JointAdmissionModule.DAL.StudRankScore();
                //                srs.StudentID = stud.StudentID;
                //                srs.Place = stud.Places.NS("年排名")[semsIndex.Name];
                //                if (DAL.DALTransfer.StudRankScoreDict[stud.GradeYear].ContainsKey(semsIndex.Name))
                //                {
                //                    DAL.DALTransfer.StudRankScoreDict[stud.GradeYear][semsIndex.Name].Add(srs);
                //                }
                //                else
                //                {
                //                    List<DAL.StudRankScore> srsList = new List<JointAdmissionModule.DAL.StudRankScore>();
                //                    srsList.Add(srs);
                //                    DAL.DALTransfer.StudRankScoreDict[stud.GradeYear].Add(semsIndex.Name, srsList);
                //                }
                //            }
                //        }

                //        // 各領域
                //        foreach (DomainParser semsIdx in AllDomainSems)
                //        {
                //            if (stud.GradeYear != semsIdx.Grade.ToString()) continue;

                //            if (stud.Places.NS("年排名").Contains(semsIdx.Name))
                //            {
                //                if (!DAL.DALTransfer.StudRankScoreDict.ContainsKey(stud.GradeYear))
                //                    DAL.DALTransfer.StudRankScoreDict.Add(stud.GradeYear, new Dictionary<string, List<DAL.StudRankScore>>());

                //                DAL.StudRankScore srs = new JointAdmissionModule.DAL.StudRankScore();
                //                srs.StudentID = stud.StudentID;
                //                srs.Place = stud.Places.NS("年排名")[semsIdx.Name];
                //                if (DAL.DALTransfer.StudRankScoreDict[stud.GradeYear].ContainsKey(semsIdx.Name))
                //                {
                //                    DAL.DALTransfer.StudRankScoreDict[stud.GradeYear][semsIdx.Name].Add(srs);
                //                }
                //                else
                //                {
                //                    List<DAL.StudRankScore> srsList = new List<JointAdmissionModule.DAL.StudRankScore>();
                //                    srsList.Add(srs);
                //                    DAL.DALTransfer.StudRankScoreDict[stud.GradeYear].Add(semsIdx.Name, srsList);
                //                }
                //            }
                //        }
                //    }
                //}

                //// 排序對照年排名資料 debug 用
                //for (int i = 1; i <= 9; i++)
                //{
                //    if (DAL.DALTransfer.StudRankScoreDict.ContainsKey(i.ToString()))
                //    {
                //        string idx = i.ToString();
                //        foreach (SLearningDomainParser semsIndex in allsems)
                //        {
                //            if (DAL.DALTransfer.StudRankScoreDict[idx].ContainsKey(semsIndex.Name))
                //            {
                //                var x = from xx in DAL.DALTransfer.StudRankScoreDict[idx][semsIndex.Name] orderby xx.Place.Score descending select xx;
                //                DAL.DALTransfer.StudRankScoreDict[idx][semsIndex.Name] = x.ToList();

                //            }

                //        }
                //    }
                //}


                // 排序對照年排名資料 debug 用
                Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook();
                Dictionary<string, ReportStudent> students = RatingStudents.ToDictionary(x => x.Id);
                int wstCot = 0;
                foreach (RatingScope<ReportStudent> scope in scopes)
                {
                    if (DAL.DALTransfer.StudRankScoreDict.ContainsKey(scope.Name))
                    {
                        string idx = scope.Name;
                        string AvggName="學期總平均";
                        if (DAL.DALTransfer.StudRankScoreDict[idx].ContainsKey(AvggName))
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

                            foreach (JointAdmissionModule.DAL.StudRankScore xx in DAL.DALTransfer.StudRankScoreDict[idx][AvggName].OrderByDescending(x => x.Place.Score))
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

                            if (DAL.DALTransfer.StudRankScoreDict[idx].ContainsKey(semsIndex.Name))
                            {
                                foreach (JointAdmissionModule.DAL.StudRankScore xx in DAL.DALTransfer.StudRankScoreDict[idx][semsIndex.Name].OrderByDescending(x => x.Place.Score))
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

                        foreach (DomainParser semsIndex in AllDomainSems)
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

                            if (DAL.DALTransfer.StudRankScoreDict[idx].ContainsKey(semsIndex.Name))
                            {
                                foreach (JointAdmissionModule.DAL.StudRankScore xx in DAL.DALTransfer.StudRankScoreDict[idx][semsIndex.Name].OrderByDescending(x => x.Place.Score))
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
                    wb.Save(Application.StartupPath + "\\五專成績debug.xls", Aspose.Cells.FileFormatType.Excel2003);

                    //System.Diagnostics.Process.Start(Application.StartupPath + "\\五專成績debug.xls");
                }
                catch (Exception ex)
                {

                }

                if (true)
                {
                    List<ReportStudent> filteredStudent = new List<ReportStudent>();
                    foreach (ReportStudent each in DicPrintStudents.Values.ToSS())
                    {
                        //看是否有「學習領域」的年排名。
                        if (each.Places.NS("年排名").Contains(LearningDomainParser.PlaceName))
                        {
                            Place place = each.Places.NS("年排名")[LearningDomainParser.PlaceName];
                                 filteredStudent.Add(each);
                        }
                        PrintStudents = filteredStudent;
                    }
                }
                else
                    PrintStudents = new List<ReportStudent>(DicPrintStudents.Values.ToSS());
     
                PrintStudents.ToSC().ReadSemesterScore(this);
                PrintStudents.ToSC().ReadSemesterHistory(this);
                
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


                PrintStudents.ReadUpdateRecordDate(this);

                e.Result = new Report(PrintStudents,txtReportTitle.Text);
                Feedback("列印完成", -1);
            }
        }

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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GetOutputString()
        {
            if (rdo1.Checked)
                return "北區";
            else if (rdo2.Checked)
                return "中區";
            else if (rdo3.Checked)
                return "南區";

            return string.Empty;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            ConfigData cdUser = K12.Data.School.Configuration["五專"];//個人的設定值
            cdUser["ReportTitle"] = txtReportTitle.Text;
            cdUser.SaveAsync();

            btnExit.Enabled = btnPrint.Enabled = false;
            DAL.DALTransfer._SchoolType = JointAdmissionModule.DAL.DALTransfer.SchoolType.五專;
            Report.checkOutputImportType = GetOutputString();
            Report.checkAddressType = cbxAddressType.Text;
            _BGWorker.RunWorkerAsync();
        }

        private void lnkSetStudSpceType_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetStudAddWeight ssaw = new SetStudAddWeight();
            ssaw.Show();
        }

        private void PrintForm_Load(object sender, EventArgs e)
        {
            rdo1.Checked = true;
            cbxAddressType.Items.Add("戶籍");
            cbxAddressType.Items.Add("聯絡");
            cbxAddressType.Text = "戶籍";
        }
    }
}
