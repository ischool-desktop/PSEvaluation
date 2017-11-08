using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Campus.Rating;
using FISCA.Presentation;
using JHEvaluation.ScoreCalculation;
using JHEvaluation.ScoreCalculation.ScoreStruct;
using K12.Data.Configuration;
using System.Reflection;

namespace JointAdmissionModule.StudentScoreSummaryReport_HsinChu
{
    public partial class PrintForm : FISCA.Presentation.Controls.BaseForm, IStatusReporter
    {
        BackgroundWorker _BGWorker;
        private List<string> StudentIDs { get; set; }
        internal ModuleMode CurrentMode { get; set; }

        public PrintForm(List<string> StudentIDs,ModuleMode CurrentMode):this(StudentIDs)
        {
            this.CurrentMode = CurrentMode;

            string Mode = CurrentMode.ToString();

            ConfigData cdUser = K12.Data.School.Configuration[Mode];
            string title = cdUser["ReportTitle"];

            if (!string.IsNullOrEmpty(cdUser["ReportTitle"]))
                txtReportTitle.Text = cdUser["ReportTitle"];
        }

        public PrintForm(List<string> studentIds)
        {
            InitializeComponent();
            StudentIDs = studentIds;
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
            if (e.Error!=null)
                SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);

            btnPrint.Enabled = btnExit.Enabled = true;
        }

        void _BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            StudentScore.SetClassMapping();

            // 取得學生類別List
            List<K12.Data.StudentTagRecord> StudTagRecList = K12.Data.StudentTag.SelectAll();

            // 過濾不排學生ID
            List<string> NonStudentIDList = DAL.DALTransfer.GetNonRankStudentIDFromUDTByStudentTag(StudTagRecList, JointAdmissionModule.DAL.DALTransfer.SchoolType.高中);
            foreach (string id in NonStudentIDList)
                if (StudentIDs.Contains(id))
                    StudentIDs.Remove(id);

            List<ReportStudent> PrintStudents = StudentIDs.ToReportStudent();

            List<ReportStudent> RatingStudents = Util.GetStudentsDef(NonStudentIDList);

            RatingStudents.ToSC().ReadSemesterScore(this);
            RatingStudents.ToSC().ReadSemesterHistory(this);

            #region 讀取三學期學期領域成績（LearnDomainScore），填入ReportStudent
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

                                    //取得核心學習領域成績
                                    if (CurrentMode == ModuleMode.Tainan)
                                        stud.CoreLearnDomainScore81 = ss.GetCoreDomainLearningScore();
                                }

                                if (ss.Semester == 2)
                                {
                                    if (ss.LearnDomainScore.HasValue)
                                        stud.LearnDomainScore82 = ss.LearnDomainScore.Value;
                                    else
                                        stud.LearnDomainScore82 = 0;

                                    //取得核心學習領域成績
                                    if (CurrentMode == ModuleMode.Tainan)
                                        stud.CoreLearnDomainScore82 = ss.GetCoreDomainLearningScore();
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

                                //取得核心學習領域成績
                                if (CurrentMode == ModuleMode.Tainan)
                                    stud.CoreLearnDomainScore91 = ss.GetCoreDomainLearningScore();
                            }
                        }
                    }
                }
            }
            #endregion

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

                //加入核心學習領域解析
                if (CurrentMode == ModuleMode.Tainan)
                    parsers.Add(new CoreLearningDomainParser());

                foreach (SLearningDomainParser each in allsems)
                    parsers.Add(each);

                #region 3學期8科目
                List<string> DomainNameList = DAL.DALTransfer.GetDomainNameList();
                // 放領域排名
                List<DomainParser> AllDomainSems = new List<DomainParser>();

                //取得各領域三學期平均排名
                foreach (string name in DomainNameList)
                {
                    DomainParser dp = new DomainParser(name,100);                    
                    AllDomainSems.Add(dp);
                }

                foreach(DomainParser each in AllDomainSems)
                    parsers.Add(each);
                #endregion

                //將學生依年級分類
                List<RatingScope<ReportStudent>> scopes = RatingStudents.ToGradeYearScopes();
            
                #region 針對每個年級的學生進行排名
                foreach (RatingScope<ReportStudent> each in scopes)
                    foreach (IScoreParser<ReportStudent> parser in parsers)
                        each.Rank(parser, PlaceOptions.Unsequence);

                Dictionary<string, StudentScore> DicPrintStudents = PrintStudents.ToSC().ToDictionary();

                foreach (ReportStudent each in RatingStudents)
                {
                    if (DicPrintStudents.ContainsKey(each.Id))
                        DicPrintStudents[each.Id] = each;
                }
                #endregion

                // 整理全部學生年排名
                DAL.DALTransfer.StudRankScoreDict.Clear();

                // 建立Key
                foreach (RatingScope<ReportStudent> scope in scopes)
                {
                    DAL.DALTransfer.StudRankScoreDict.Add(scope.Name, new Dictionary<string, List<JointAdmissionModule.DAL.StudRankScore>>());
                    //學期總平均
                    DAL.DALTransfer.StudRankScoreDict[scope.Name].Add("學期總平均", new List<JointAdmissionModule.DAL.StudRankScore>());
                    //核心學期總平均
                    DAL.DALTransfer.StudRankScoreDict[scope.Name].Add("核心學期總平均", new List<DAL.StudRankScore>());

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

                    //核心學習領域
                    foreach (ReportStudent stud in (from xx in scope where xx.Places.NS("年排名").Contains("核心學習領域") select xx).ToList())
                    {
                        DAL.StudRankScore srs = new JointAdmissionModule.DAL.StudRankScore();
                        srs.StudentID = stud.StudentID;
                        srs.Place = stud.Places.NS("年排名")["核心學習領域"];
                        DAL.DALTransfer.StudRankScoreDict[scope.Name]["核心學期總平均"].Add(srs);
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

                        string CoreAvggName = "核心學期總平均";
                        if (DAL.DALTransfer.StudRankScoreDict[idx].ContainsKey(CoreAvggName))
                        {
                            int row = 1;

                            wb.Worksheets.Add();
                            wb.Worksheets[wstCot].Name = idx + CoreAvggName;
                            wb.Worksheets[wstCot].Cells[0, 0].PutValue("成績");
                            wb.Worksheets[wstCot].Cells[0, 1].PutValue("年排名");
                            wb.Worksheets[wstCot].Cells[0, 2].PutValue("分類");
                            wb.Worksheets[wstCot].Cells[0, 3].PutValue("年級");
                            wb.Worksheets[wstCot].Cells[0, 4].PutValue("SID");
                            wb.Worksheets[wstCot].Cells[0, 5].PutValue("StudCount");

                            foreach (JointAdmissionModule.DAL.StudRankScore xx in DAL.DALTransfer.StudRankScoreDict[idx][CoreAvggName].OrderByDescending(x => x.Place.Score))
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
                    wb.Save(Application.StartupPath + "\\竹苗成績debug.xls", Aspose.Cells.FileFormatType.Excel2003);

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

                e.Result = new Report(PrintStudents,txtReportTitle.Text,CurrentMode);
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

        private void btnPrint_Click(object sender, EventArgs e)       
        {
            ConfigData cdUser = K12.Data.School.Configuration[CurrentMode.ToString()];//個人的設定值

            cdUser["ReportTitle"] = txtReportTitle.Text;
            cdUser["chkOutputImportFile"] = chkOutputImportFile.Checked.ToString();
            cdUser["chkWst3aa"] = chkWst3aa.Checked.ToString();
            cdUser["chkWst3ab"] = chkWst3ab.Checked.ToString();
            cdUser.SaveAsync();

            btnExit.Enabled = btnPrint.Enabled = false;
            DAL.DALTransfer._SchoolType = JointAdmissionModule.DAL.DALTransfer.SchoolType.高中;

            Report.checkOutputImportFile = chkOutputImportFile.Checked;
            Report.checkWst3aa = chkWst3aa.Checked;
            Report.checkWst3ab = chkWst3ab.Checked;
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

            ConfigData cdUser = K12.Data.School.Configuration[CurrentMode.ToString()];//個人的設定值

            if (CurrentMode == ModuleMode.HsinChu)
            {
                chkWst3aa.Checked = true;
                chkWst3ab.Checked = true;
                chkWst3aa.Visible = true;
                chkWst3ab.Visible = true;
                chkOutputImportFile.Visible = false;
                this.Size = new System.Drawing.Size(432, 167);

                
            }
            else
            {
                chkOutputImportFile.Checked = true;
                chkWst3aa.Visible = false;
                chkWst3ab.Visible = false;
                chkOutputImportFile.Visible = true;
                this.Size = new System.Drawing.Size(432, 151);
            }

            bool b1, b2, b3;
            if (bool.TryParse(cdUser["chkOutputImportFile"],out b1))
                chkOutputImportFile.Checked = b1;

            if (bool.TryParse(cdUser["chkWst3aa"], out b2))
                chkWst3aa.Checked = b2;

            if (bool.TryParse(cdUser["chkWst3ab"], out b3))
                chkWst3ab.Checked = b3;

            chkOutputImportFile.Checked = true;
            cbxAddressType.Items.Add("戶籍");
            cbxAddressType.Items.Add("聯絡");
            cbxAddressType.Text = "戶籍";
        }
    }
}