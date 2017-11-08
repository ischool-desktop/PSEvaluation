using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Report;
using System.ComponentModel;
using JHSchool.Data;
using Aspose.Words;
using FISCA.Presentation.Controls;
using FISCA.Presentation;
using JHEvaluation.StudentSemesterScoreReport.Writers;

namespace JHEvaluation.StudentSemesterScoreReport
{
    internal class ReportController
    {
        private BackgroundWorker _worker;
        private Document _doc;
        private Document _template;
        private ReportConfiguration Config { get; set; }
        private Options Options { get; set; }

        public ReportController()
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _doc = new Document();
            _doc.Sections.Clear();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MotherForm.SetStatusBarMessage(Global.ReportName + "產生中", e.ProgressPercentage);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MsgBox.Show("產生報表時發生錯誤。" + e.Error.Message);
                return;
            }

            
            //MotherForm.SetStatusBarMessage(Global.ReportName + "產生完成");
            //ReportSaver.SaveDocument(_doc, Global.ReportName);
            bool pdf = Config.GetBoolean("輸出成PDF格式", false);


            if (pdf)
            {
                MotherForm.SetStatusBarMessage("轉換成 PDF 格式中...");
                ReportSaver.SaveDocument(_doc, Global.ReportName, ReportSaver.OutputType.PDF);
                MotherForm.SetStatusBarMessage(Global.ReportName + "產生完成");
            }
            else
            {
                MotherForm.SetStatusBarMessage(Global.ReportName + "產生完成");
                ReportSaver.SaveDocument(_doc, Global.ReportName);
            }

        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            double total = Options.Students.Count;
            double count = 0;
            _worker.ReportProgress(0);

            #region 快取資料
            DataCache cache = new DataCache(Options);
            #endregion

            DocumentBuilder templateBuilder = new DocumentBuilder(_template);

            #region 取得學生服務學習時數

            // 取得學生服務學習時數
            Global._SRDict.Clear();
            List<string> sidList = (from data in Options.Students select data.ID).ToList();
            Global._SRDict = Utility.GetServiceLearningDetail(sidList, Options.SchoolYear, Options.Semester);
            
            #endregion

            #region 節權數顯示
            string pcDisplay = string.Empty;
            bool printPeriod = Config.GetBoolean("列印節數", true);
            bool printCredit = Config.GetBoolean("列印權數", false);
            if (printPeriod && printCredit)
                pcDisplay = "節/權數";
            else if (printPeriod)
                pcDisplay = "節數";
            else if (printCredit)
                pcDisplay = "權數";

            while (templateBuilder.MoveToMergeField("節權數"))
                templateBuilder.Write(pcDisplay);
            #endregion

            #region 產生
            BasicInfoWriter basicIndoWriter = new BasicInfoWriter();
            ScoreWriter scoreWriter = new ScoreWriter();
            MoralWriter moralWriter = new MoralWriter();
            AssnScoreWriter assnScoreWriter = new AssnScoreWriter();

            foreach (JHStudentRecord student in Options.Students)
            {
                count++;

                Document each = (Document)_template.Clone();

                //基本資料
                basicIndoWriter.Student = student;
                basicIndoWriter.HistoryItem = cache.GetSemesterHistoryItem(student.ID);
                basicIndoWriter.Write(each);

                //學期成績 & 學習領域文字
                scoreWriter.SemesterScoreRecord = cache.GetSemesterScore(student.ID);
                scoreWriter.Writer(each);

                //德行成績
                moralWriter.AutoSummaryRecord = cache.GetAutoSummary(student.ID);
                moralWriter.Writer(each);

                if (Global.Params["Mode"] == "KaoHsiung")
                {
                    //社團活動
                    assnScoreWriter.Score = cache.GetAssnScore(student.ID);
                    assnScoreWriter.Writer(each);
                }

                foreach (Section sec in each.Sections)
                    _doc.Sections.Add(_doc.ImportNode(sec, true));

                _worker.ReportProgress((int)(count * 100f / total));
            }

            OutsideFieldWriter ofWriter = new OutsideFieldWriter(Options);
            ofWriter.Write(_doc);
            #endregion
        }

        public void Generate(Options options)
        {
            Options = options;
            Config = new ReportConfiguration(Global.ReportName);

            #region 取得樣板
            if (Config.Template == null)
            {
                byte[] tplBytes = (Global.Params["Mode"] == "KaoHsiung") ? Properties.Resources.高雄學期成績證明單樣板 : Properties.Resources.新竹學期成績證明單樣板;
                Config.Template = new ReportTemplate(tplBytes, TemplateType.Word);
            }
            _template = Config.Template.ToDocument();
            #endregion

            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }
    }
}
