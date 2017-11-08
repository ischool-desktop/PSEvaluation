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
using System.IO;

namespace JHEvaluation.StudentScoreSummaryReport
{
    public partial class PrintFormEnglish : BaseForm, IStatusReporter
    {
        internal const string ConfigName = "StudentScoreSummaryReportEnglish";

        private List<string> StudentIDs { get; set; }

        private ReportPreference Preference { get; set; }

        private BackgroundWorker MasterWorker = new BackgroundWorker();

        private bool PrintScore = false;

        List<ReportStudent> PrintStudents;

        public PrintFormEnglish(List<string> studentIds)
        {
            InitializeComponent();

            StudentIDs = studentIds;
            Preference = new ReportPreference(ConfigName, Prc.學生在校成績證明書_英文);
            MasterWorker.DoWork += new DoWorkEventHandler(MasterWorker_DoWork);
            MasterWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(MasterWorker_RunWorkerCompleted);

            rbDomainOnly.Checked = (Preference.ListMethod == ListMethod.DomainOnly);
            rbSubjectOnly.Checked = (Preference.ListMethod == ListMethod.SubjectOnly);
            chkRank.Checked = Preference.PrintRank;
            chkPercentage.Checked = Preference.PrintRankPercentage;
            txtGraduateDate.Text = Preference.GraduateDate;
            txtEntranceDate.Text = Preference.EntranceDate;
            rtnPDF.Checked = Preference.ConvertToPDF;
            chkSpiltPDF.Checked = Preference.SpiltPDF;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (rbDomainOnly.Checked)
                Preference.ListMethod = ListMethod.DomainOnly;
            else
                Preference.ListMethod = ListMethod.SubjectOnly;

            Preference.PrintRank = chkRank.Checked;
            Preference.PrintRankPercentage = chkPercentage.Checked;
            Preference.GraduateDate = txtGraduateDate.Text;
            Preference.EntranceDate = txtEntranceDate.Text;
            Preference.ConvertToPDF = rtnPDF.Checked;
            Preference.SpiltPDF = chkSpiltPDF.Checked;
            Preference.Save(); //儲存設定值。
            PrintScore = rbPrintScore.Checked;

            Util.DisableControls(this);
            MasterWorker.RunWorkerAsync();
        }

        private void MasterWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            StudentScore.SetClassMapping();
            PrintStudents = StudentIDs.ToReportStudent();

            PrintStudents.ToSC().ReadSemesterScore(this);
            PrintStudents.ToSC().ReadSemesterHistory(this);

            //if (Preference.PrintRank || Preference.PrintRankPercentage)
            //{
            //    #region 如果要排名。
            //    List<ReportStudent> RatingStudents = Util.GetAllStudents();
            //    //RatingStudents.ToSC().ReadSemesterScore(this);
            //    //RatingStudents.ToSC().ReadSemesterHistory(this);

            //    List<IScoreParser<ReportStudent>> parsers = new List<IScoreParser<ReportStudent>>();
            //    parsers.Add(new LearningDomainParser(new List<int>(new int[] { 1, 2, 3, 4, 5, 6 })));
            //    parsers.Add(new SLearningDomainParser(1, 1));
            //    parsers.Add(new SLearningDomainParser(1, 2));
            //    parsers.Add(new SLearningDomainParser(2, 1));
            //    parsers.Add(new SLearningDomainParser(2, 2));
            //    parsers.Add(new SLearningDomainParser(3, 1));
            //    parsers.Add(new SLearningDomainParser(3, 2));

            //    List<RatingScope<ReportStudent>> scopes = RatingStudents.ToGradeYearScopes();

            //    foreach (RatingScope<ReportStudent> each in scopes)
            //        foreach (IScoreParser<ReportStudent> parser in parsers)
            //            each.Rank(parser, PlaceOptions.Unsequence);

            //    Dictionary<string, StudentScore> DicPrintStudents = PrintStudents.ToSC().ToDictionary();

            //    foreach (ReportStudent each in RatingStudents)
            //    {
            //        if (DicPrintStudents.ContainsKey(each.Id))
            //            DicPrintStudents[each.Id] = each;
            //    }

            //    PrintStudents = new List<ReportStudent>(DicPrintStudents.Values.ToSS());
            //    #endregion
            //}
            //else
            //{
            //    PrintStudents.ToSC().ReadSemesterScore(this);
            //    PrintStudents.ToSC().ReadSemesterHistory(this);
            //}

            List<StudentScore> CalcStudents = PrintStudents.ToSC();
            CalcStudents.ReadCalculationRule(this); //讀取成績計算規則。

            #region 讀取缺曠獎懲。
            //List<JHMoralScoreRecord> jhmorals = JHSchool.Data.JHMoralScore.SelectByStudentIDs(StudentIDs);
            //Dictionary<string, ReportStudent> DicStudents = PrintStudents.ToDictionary();
            //foreach (JHMoralScoreRecord each in jhmorals)
            //{
            //    if (!DicStudents.ContainsKey(each.RefStudentID)) continue;

            //    SemesterData semester = new SemesterData(0, each.SchoolYear, each.Semester);
            //    ReportStudent student = DicStudents[each.RefStudentID];

            //    if (!student.Summaries.ContainsKey(semester))
            //        student.Summaries.Add(semester, each.Summary);
            //}
            #endregion

            PrintStudents.ReadUpdateRecordDate(this);
            PrintStudents.ReadGraduatePhoto(this); //讀取照片。

            if(Preference.SpiltPDF)
                e.Result = new ReportEnglish(PrintStudents, Preference).PrintDict(PrintScore);
            else
                e.Result = new ReportEnglish(PrintStudents, Preference).Print(PrintScore);

            Feedback("列印完成", -1);        
        }

        private void MasterWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Util.EnableControls(this);

            if(Preference.SpiltPDF)
            {
                Dictionary<string,Document> dataDict =e.Result as Dictionary<string,Document>;

                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "請選擇儲存資料夾";
                fbd.ShowNewFolderButton = true;

                if (fbd.ShowDialog() == DialogResult.Cancel) return;
                if(Preference.ConvertToPDF)
                {
                    MotherForm.SetStatusBarMessage("轉換成 PDF 格式中...");
                    foreach (string each in dataDict.Keys)
                    {

                        #region 處理產生 PDF

                        string fPath = fbd.SelectedPath + "\\" + each + ".pdf";

                        FileInfo fi = new FileInfo(fPath);

                        DirectoryInfo folder = new DirectoryInfo(Path.Combine(fi.DirectoryName, Path.GetRandomFileName()));
                        if (!folder.Exists) folder.Create();

                        FileInfo fileinfo = new FileInfo(Path.Combine(folder.FullName, fi.Name));

                        string XmlFileName = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".xml";
                        string PDFFileName = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".pdf";

                        dataDict[each].Save(XmlFileName, Aspose.Words.SaveFormat.AsposePdf);

                        Aspose.Pdf.Pdf pdf1 = new Aspose.Pdf.Pdf();

                        pdf1.BindXML(XmlFileName, null);
                        pdf1.Save(PDFFileName);

                        if (File.Exists(fPath))
                            File.Delete(Path.Combine(fi.DirectoryName, fi.Name));

                        File.Move(PDFFileName, fPath);
                        folder.Delete(true);
                        #endregion
                    }
                }
                else
                {
                    MotherForm.SetStatusBarMessage("儲存中...");
                    foreach (string each in dataDict.Keys)
                    {
                        #region 處理產生 Doc

                        string fPath = fbd.SelectedPath + "\\" + each + ".doc";
                        dataDict[each].Save(fPath, Aspose.Words.SaveFormat.Doc);
                        #endregion
                    }

                }

                MotherForm.SetStatusBarMessage("產生完成");
                System.Diagnostics.Process.Start(fbd.SelectedPath);
            }
            else
            {
                Document doc = e.Result as Document;
                Util.Save(doc, "學生在校成績證明書_英文", Preference.ConvertToPDF);
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
            ReportTemplate defaultTemplate = new ReportTemplate(Prc.學生在校成績證明書_英文, TemplateType.Word);
            TemplateSettingForm form = new TemplateSettingForm(Preference.Template, defaultTemplate);
            form.DefaultFileName = "學生在校成績證明書樣版_英文.doc";

            if (form.ShowDialog() == DialogResult.OK)
            {
                Preference.Template = (form.Template == defaultTemplate) ? null : form.Template;
                Preference.Save();
            }
        }
    }
}
