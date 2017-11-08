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
using JHSchool.Behavior.BusinessLogic;
using System.IO;

namespace JHEvaluation.StudentScoreSummaryReport
{
    public partial class PrintForm : BaseForm, IStatusReporter
    {
        internal const string ConfigName = "StudentScoreSummaryReport";

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
            Preference = new ReportPreference(ConfigName, Prc.學生在校成績證明書);
            MasterWorker.DoWork += new DoWorkEventHandler(MasterWorker_DoWork);
            MasterWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(MasterWorker_RunWorkerCompleted);

            ConvertToPDF_Worker.DoWork += new DoWorkEventHandler(ConvertToPDF_Worker_DoWork);
            ConvertToPDF_Worker.WorkerReportsProgress = true;
            ConvertToPDF_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ConvertToPDF_Worker_RunWorkerCompleted);

            ConvertToPDF_Worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage(e.UserState.ToString(), e.ProgressPercentage);
            };

            rbDomainOnly.Checked = (Preference.ListMethod == ListMethod.DomainOnly);
            rbSubjectOnly.Checked = (Preference.ListMethod == ListMethod.SubjectOnly);
            chkRank.Checked = Preference.PrintRank;
            chkPercentage.Checked = Preference.PrintRankPercentage;
            txtGraduateDate.Text = Preference.GraduateDate;
            txtEntranceDate.Text = Preference.EntranceDate;
            chkRankFilter.Checked = Preference.FilterRankScope;
            intRankStart.Value = Preference.RankStart;
            intRankEnd.Value = Preference.RankEnd;
            rtnPDF.Checked = Preference.ConvertToPDF;

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

            intRankStart.Enabled = chkRankFilter.Checked;
            intRankEnd.Enabled = chkRankFilter.Checked;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (chkRankFilter.Checked)
            {
                if (intRankStart.Value > intRankEnd.Value)
                {
                    MsgBox.Show("請輸入正確的名次範圍。");
                    return;
                }
            }

            if (rbDomainOnly.Checked)
                Preference.ListMethod = ListMethod.DomainOnly;
            else
                Preference.ListMethod = ListMethod.SubjectOnly;

            Preference.PrintRank = chkRank.Checked;
            Preference.PrintRankPercentage = chkPercentage.Checked;
            Preference.GraduateDate = txtGraduateDate.Text;
            Preference.EntranceDate = txtEntranceDate.Text;
            Preference.FilterRankScope = chkRankFilter.Checked;
            Preference.RankStart = intRankStart.Value;
            Preference.RankEnd = intRankEnd.Value;
            Preference.ConvertToPDF = rtnPDF.Checked;

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
             PrintStudents = StudentIDs.ToReportStudent();
            List<ReportStudent> AllStudents = Util.GetAllStudents();

            AllStudents.ToSC().ReadSemesterScore(this);
            AllStudents.ToSC().ReadSemesterHistory(this);

            if (Preference.PrintRank || Preference.PrintRankPercentage || Preference.FilterRankScope)
            {
                #region 如果要排名。
                List<ReportStudent> RatingStudents = Util.GetRatingStudents(AllStudents);

                List<IScoreParser<ReportStudent>> parsers = new List<IScoreParser<ReportStudent>>();
                parsers.Add(new LearningDomainParser(Preference.PrintSemesters));
                parsers.Add(new SLearningDomainParser(1, 1));
                parsers.Add(new SLearningDomainParser(1, 2));
                parsers.Add(new SLearningDomainParser(2, 1));
                parsers.Add(new SLearningDomainParser(2, 2));
                parsers.Add(new SLearningDomainParser(3, 1));
                parsers.Add(new SLearningDomainParser(3, 2));
                parsers.Add(new SLearningDomainParser(7, 1));
                parsers.Add(new SLearningDomainParser(7, 2));
                parsers.Add(new SLearningDomainParser(8, 1));
                parsers.Add(new SLearningDomainParser(8, 2));
                parsers.Add(new SLearningDomainParser(9, 1));
                parsers.Add(new SLearningDomainParser(9, 2));

                List<RatingScope<ReportStudent>> scopes = RatingStudents.ToGradeYearScopes();

                foreach (RatingScope<ReportStudent> each in scopes)
                    foreach (IScoreParser<ReportStudent> parser in parsers)
                        each.Rank(parser, PlaceOptions.Unsequence);

                Dictionary<string, StudentScore> DicPrintStudents = PrintStudents.ToSC().ToDictionary();

                foreach (ReportStudent each in AllStudents)
                {
                    if (DicPrintStudents.ContainsKey(each.Id))
                        DicPrintStudents[each.Id] = each;
                }

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
                    }
                    PrintStudents = filteredStudent;
                }
                else
                    PrintStudents = new List<ReportStudent>(DicPrintStudents.Values.ToSS());
                #endregion
            }
            else
            {
                Dictionary<string, StudentScore> DicPrintStudents = PrintStudents.ToSC().ToDictionary();
                foreach (ReportStudent each in AllStudents)
                {
                    if (DicPrintStudents.ContainsKey(each.Id))
                        DicPrintStudents[each.Id] = each;
                }
                PrintStudents = new List<ReportStudent>(DicPrintStudents.Values.ToSS());
            }

            if (PrintStudents.Count <= 0)
            {
                Feedback("", -1);  //把 Status bar Reset...
                throw new ArgumentException("沒有任何學生資料可列印。");
            }

            List<StudentScore> CalcStudents = PrintStudents.ToSC();
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
                    if (Preference.ConvertToPDF)
                    {
                        MotherForm.SetStatusBarMessage("正在轉換PDF格式... 請耐心等候");
                    }

                    Util.DisableControls(this);
                    ConvertToPDF_Worker.RunWorkerAsync();
                    
                }


            }
            else 
            {
                MsgBox.Show(e.Error.Message);
            }
            

            if (Preference.ConvertToPDF)
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
                Util.Save(doc, "學生在校成績證明書", Preference.ConvertToPDF);
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
            ReportTemplate defaultTemplate = new ReportTemplate(Prc.學生在校成績證明書, TemplateType.Word);
            TemplateSettingForm form = new TemplateSettingForm(Preference.Template, defaultTemplate);
            form.DefaultFileName = "學生在校成績證明書樣版.doc";

            if (form.ShowDialog() == DialogResult.OK)
            {
                Preference.Template = (form.Template == defaultTemplate) ? null : form.Template;
                Preference.Save();
            }
        }

        private void RankScope_CheckedChanged(object sender, EventArgs e)
        {
            intRankStart.Enabled = chkRankFilter.Checked;
            intRankEnd.Enabled = chkRankFilter.Checked;
        }
    }
}
