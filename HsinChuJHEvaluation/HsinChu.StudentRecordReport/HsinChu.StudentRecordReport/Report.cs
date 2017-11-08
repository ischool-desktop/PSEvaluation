using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Aspose.Words;
using Aspose.Words.Reporting;
using System.Windows.Forms;
using System.ComponentModel;
using Aspose.Words.Drawing;
using JHSchool.Data;
using FISCA.Presentation.Controls;
using FISCA.Presentation;
using K12.Data;
using Campus.Report;
using JHSchool.Behavior.BusinessLogic;
using JHSchool.Evaluation.Mapping;

namespace HsinChu.StudentRecordReport
{
    internal class Report
    {
        private BackgroundWorker _worker;
        private Document _doc;
        private Document _template;
        private DegreeMapper _degreeMapper;

        private Options Options { get; set; }
        private ReportConfiguration Config { get; set; }

        bool OneFileSave;
        Dictionary<string, Document> StudentDoc;

        public Report(Options _options)
        {
            Options = _options;
            Config = new ReportConfiguration(Global.ReportName);
            _degreeMapper = new DegreeMapper();

            InitializeTemplate();
            InitializeBackgroundWorker();
        }

        private void InitializeTemplate()
        {
            _doc = new Document();
            _doc.Sections.Clear();

            //_template = Config.Template.ToDocument();
            _template = new Document(new MemoryStream(Properties.Resources.國中學籍表));
        }

        private void InitializeBackgroundWorker()
        {
            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.WorkerReportsProgress = true;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            List<string> globalFieldName = new List<string>();
            List<object> globalFieldValue = new List<object>();

            globalFieldName.Add("學校名稱");
            globalFieldValue.Add(School.ChineseName);

            globalFieldName.Add("列印時間");
            globalFieldValue.Add(Global.CDate(DateTime.Now.ToString("yyyy/MM/dd")) + " " + DateTime.Now.ToString("HH:mm:ss"));

            ReportConfiguration _Dylanconfig = new ReportConfiguration(Global.OneFileSave);
            OneFileSave = _Dylanconfig.GetBoolean("單檔儲存", false);
            StudentDoc = new Dictionary<string, Document>();


            List<JHPeriodMappingInfo> periodList = JHPeriodMapping.SelectAll();
            List<JHAbsenceMappingInfo> absenceList = JHAbsenceMapping.SelectAll();

            double total = Options.Students.Count;
            double count = 0;

            List<string> student_ids = new List<string>();
            foreach (JHStudentRecord item in Options.Students)
                student_ids.Add(item.ID);

            DocumentBuilder templateBuilder = new DocumentBuilder(_template);

            #region 變更節權數顯示
            string pcDisplay = string.Empty;
            bool printPeriod = Config.GetBoolean("列印節數", false);
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

            #region 快取資料
            // 取得學生缺曠
            Dictionary<string, List<AutoSummaryRecord>> autoSummaryCache = new Dictionary<string, List<AutoSummaryRecord>>();
            foreach (AutoSummaryRecord record in AutoSummary.Select(student_ids, null))
            {
                if (!autoSummaryCache.ContainsKey(record.RefStudentID))
                    autoSummaryCache.Add(record.RefStudentID, new List<AutoSummaryRecord>());
                autoSummaryCache[record.RefStudentID].Add(record);
            }

            // 取得學生異動
            Dictionary<string, List<JHUpdateRecordRecord>> updateRecordCache = new Dictionary<string, List<JHUpdateRecordRecord>>();
            foreach (var record in JHUpdateRecord.SelectByStudentIDs(student_ids))
            {
                if (!updateRecordCache.ContainsKey(record.StudentID))
                    updateRecordCache.Add(record.StudentID, new List<JHUpdateRecordRecord>());
                updateRecordCache[record.StudentID].Add(record);
            }

            // 取得學生學期成績
            Dictionary<string, List<JHSemesterScoreRecord>> semesterScoreCache = new Dictionary<string, List<JHSemesterScoreRecord>>();
            foreach (var record in JHSemesterScore.SelectByStudentIDs(student_ids))
            {
                if (!semesterScoreCache.ContainsKey(record.RefStudentID))
                    semesterScoreCache.Add(record.RefStudentID, new List<JHSemesterScoreRecord>());
                semesterScoreCache[record.RefStudentID].Add(record);
            }

            // 取得學生學期歷程
            Dictionary<string, JHSemesterHistoryRecord> semesterHistoryCache = new Dictionary<string, JHSemesterHistoryRecord>();
            foreach (var record in JHSemesterHistory.SelectByStudentIDs(student_ids))
            {
                if (!semesterHistoryCache.ContainsKey(record.RefStudentID))
                    semesterHistoryCache.Add(record.RefStudentID, record);
            }

            // 取得學生畢業成績
            Dictionary<string, K12.Data.GradScoreRecord> StudGradScoreDic = new Dictionary<string, GradScoreRecord>();
            foreach (GradScoreRecord score in GradScore.SelectByIDs<GradScoreRecord>(student_ids))
            {
                StudGradScoreDic.Add(score.RefStudentID, score);
            }


            #endregion

            #region 判斷要列印的領域科目
            Dictionary<string, bool> domains = new Dictionary<string, bool>();
            string domainSubjectSetup = Config.GetString("領域科目設定", "Domain");
            if (domainSubjectSetup == "Domain")
            {
                foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                    domains.Add(domain, true);
                if (domains.ContainsKey("語文")) domains["語文"] = false;
                if (domains.ContainsKey("彈性課程")) domains["彈性課程"] = false;
                if (!domains.ContainsKey("")) domains.Add("", false);
            }
            else if (domainSubjectSetup == "Subject")
            {
                foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                    domains.Add(domain, false);
                if (!domains.ContainsKey("")) domains.Add("", false);
            }
            else
                throw new Exception("請重新儲存列印設定");


            //string domainSubjectSetup = Config.GetString("領域科目設定", "Domain");
            //if (domainSubjectSetup == "Domain")
            //{
            //    foreach (var domain in JHSchool.Evaluation.Subject.Domains)
            //        domains.Add(domain, DomainSubjectExpand.不展開);

            //    if (!domains.ContainsKey("")) domains.Add("", DomainSubjectExpand.展開);
            //}
            //else if (domainSubjectSetup == "Subject")
            //{
            //    foreach (var domain in JHSchool.Evaluation.Subject.Domains)
            //        domains.Add(domain, DomainSubjectExpand.展開);
            //    if (!domains.ContainsKey("")) domains.Add("", DomainSubjectExpand.展開);
            //}
            //else
            //    throw new Exception("請重新儲存一次列印設定");

            #endregion

            #region 服務學習時數
            Global._SLRDict.Clear();
            Global._SLRDict = Utility.GetServiceLearningDetail(student_ids);
            #endregion

            #region 產生
            foreach (JHStudentRecord student in Options.Students)
            {
                count++;
                DocumentBuilder builder = null;
                Document each = (Document)_template.Clone(true);

                #region 建立學期歷程對照
                List<SemesterHistoryItem> semesterHistoryList = null;
                if (semesterHistoryCache.ContainsKey(student.ID))
                    semesterHistoryList = semesterHistoryCache[student.ID].SemesterHistoryItems;
                else
                    semesterHistoryList = new List<SemesterHistoryItem>();

                SemesterMap map = new SemesterMap();
                map.SetData(semesterHistoryList);
                #endregion

                #region Part1
                builder = new DocumentBuilder(each);

                StudentBasicInfo basic = new StudentBasicInfo();
                basic.SetStudent(student, semesterHistoryList);

                StudentSemesterHistory history = new StudentSemesterHistory();
                history.SetData(semesterHistoryList);

                each.MailMerge.MergeField += delegate(object sender1, MergeFieldEventArgs e1)
                {
                    #region 處理照片
                    if (e1.FieldName == "照片")
                    {
                        byte[] photoBytes = null;
                        try
                        {
                            photoBytes = Convert.FromBase64String("" + e1.FieldValue);
                        }
                        catch (Exception ex)
                        {
                            e1.Field.Remove();
                            return;
                        }

                        if (photoBytes == null || photoBytes.Length == 0)
                        {
                            e1.Field.Remove();
                            return;
                        }

                        DocumentBuilder builder1 = new DocumentBuilder(e1.Document);
                        builder1.MoveToField(e1.Field, true);
                        e1.Field.Remove();

                        Shape photoShape = new Shape(e1.Document, ShapeType.Image);
                        photoShape.ImageData.SetImage(photoBytes);
                        photoShape.WrapType = WrapType.Inline;

                        #region AutoResize

                        double origHWRate = photoShape.ImageData.ImageSize.HeightPoints / photoShape.ImageData.ImageSize.WidthPoints;
                        double shapeHeight = (builder1.CurrentParagraph.ParentNode.ParentNode as Row).RowFormat.Height * 6;
                        double shapeWidth = (builder1.CurrentParagraph.ParentNode as Cell).CellFormat.Width;
                        if ((shapeHeight / shapeWidth) < origHWRate)
                            shapeWidth = shapeHeight / origHWRate;
                        else
                            shapeHeight = shapeWidth * origHWRate;

                        #endregion

                        photoShape.Height = shapeHeight;
                        photoShape.Width = shapeWidth;

                        builder1.InsertNode(photoShape);
                    }
                    #endregion
                };

                List<string> fieldName = new List<string>();
                fieldName.AddRange(basic.GetFieldName());
                fieldName.AddRange(history.GetFieldName());
                List<string> fieldValue = new List<string>();
                fieldValue.AddRange(basic.GetFieldValue());
                fieldValue.AddRange(history.GetFieldValue());

                each.MailMerge.Execute(fieldName.ToArray(), fieldValue.ToArray());

                builder.MoveToMergeField("異動");
                List<JHUpdateRecordRecord> updateRecordList = null;
                if (updateRecordCache.ContainsKey(student.ID))
                    updateRecordList = updateRecordCache[student.ID];
                else
                    updateRecordList = new List<JHUpdateRecordRecord>();
                StudentUpdateRecordProcessor updateRecordProcessor = new StudentUpdateRecordProcessor(builder);
                updateRecordProcessor.SetData(updateRecordList);

                builder.MoveToMergeField("成績");
                List<JHSemesterScoreRecord> semesterScoreList = null;
                if (semesterScoreCache.ContainsKey(student.ID))
                    semesterScoreList = semesterScoreCache[student.ID];
                else
                    semesterScoreList = new List<JHSemesterScoreRecord>();

                // 學生畢業成績
                K12.Data.GradScoreRecord studGradScore = new GradScoreRecord();
                if (StudGradScoreDic.ContainsKey(student.ID))
                    studGradScore = StudGradScoreDic[student.ID];

                StudentSemesterScoreProcessor semesterScoreProcessor = new StudentSemesterScoreProcessor(builder, map, domainSubjectSetup, domains, studGradScore);
                semesterScoreProcessor.DegreeMapper = _degreeMapper;
                semesterScoreProcessor.PrintPeriod = printPeriod;
                semesterScoreProcessor.PrintCredit = printCredit;
                semesterScoreProcessor.SetData(semesterScoreList);

                #endregion

                #region Part2
                //builder = new DocumentBuilder(each);

                builder.MoveToMergeField("領域文字描述");
                if (Config.GetBoolean("列印文字評語", true))
                {
                    StudentDomainTextProcessor domainTextProcessor = new StudentDomainTextProcessor(builder, map);
                    domainTextProcessor.SetData(semesterScoreList);
                }
                else
                {
                    Section deleteSection = builder.CurrentSection;
                    each.Sections.Remove(deleteSection);
                }

                #endregion

                #region Part3
                //Document part3 = (Document)_template_part3.Clone(true);
                //builder = new DocumentBuilder(each);

                builder.MoveToMergeField("日常行為");
                StudentMoralProcessor moralProcessor = new StudentMoralProcessor(builder, map, periodList, absenceList);

                List<AutoSummaryRecord> autoSummaryList = null;
                if (autoSummaryCache.ContainsKey(student.ID))
                    autoSummaryList = autoSummaryCache[student.ID];
                else
                    autoSummaryList = new List<AutoSummaryRecord>();

                moralProcessor.SetData(autoSummaryList);

                // 處理多出來 table row
                Cell cel = moralProcessor.GetCurrentCell();
                Table table = cel.ParentRow.ParentTable;
                for (int i = 47; i >= 20; i--)
                {
                    bool rm = true;
                    foreach (Aspose.Words.Cell cell in table.Rows[i].Cells)
                        if (cell.GetText() != "\a")
                        {
                            rm = false;
                            break;
                        }

                    if (rm)
                        table.Rows[i].Remove();
                }

                #endregion

                if (OneFileSave)
                {
                    each.MailMerge.Execute(globalFieldName.ToArray(), globalFieldValue.ToArray());

                    string fileName = "";
                    fileName = student.StudentNumber;

                    fileName += "_" + student.IDNumber;

                    if (!string.IsNullOrEmpty(student.RefClassID))
                        fileName += "_" + student.Class.Name;
                    else
                        fileName += "_";

                    fileName += "_" + (student.SeatNo.HasValue ? student.SeatNo.Value.ToString() : "");
                    fileName += "_" + student.Name;

                    if (!StudentDoc.ContainsKey(fileName))
                    {
                        StudentDoc.Add(fileName, each);
                    }

                }
                else
                {
                    foreach (Section sec in each.Sections)
                        _doc.Sections.Add(_doc.ImportNode(sec, true));
                }

                //回報進度
                _worker.ReportProgress((int)(count * 100.0 / total));
            }

            if (!OneFileSave)
            {
                _doc.MailMerge.Execute(globalFieldName.ToArray(), globalFieldValue.ToArray());
            }

            #endregion
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MsgBox.Show("產生報表時發生錯誤");
                return;
            }

            bool pdf = Config.GetBoolean("輸出成PDF格式", false);

            if (OneFileSave)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "請選擇儲存資料夾";
                fbd.ShowNewFolderButton = true;

                if (fbd.ShowDialog() == DialogResult.Cancel) return;


                if (pdf)
                {
                    MotherForm.SetStatusBarMessage("轉換成 PDF 格式中...");
                    foreach (string each in StudentDoc.Keys)
                    {
                        StudentDoc[each].Save(fbd.SelectedPath + "\\" + each, SaveFormat.AsposePdf);

                        //ReportSaver.SaveDocument(StudentDoc[each], each, ReportSaver.OutputType.PDF);
                    }
                }
                else
                {
                    foreach (string each in StudentDoc.Keys)
                    {
                        StudentDoc[each].Save(fbd.SelectedPath + "\\" + each + ".doc");
                    }
                }

                MotherForm.SetStatusBarMessage("產生完成");
                System.Diagnostics.Process.Start(fbd.SelectedPath);



            }
            else
            {
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
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MotherForm.SetStatusBarMessage("學籍表產生中", e.ProgressPercentage);
        }

        public void Generate()
        {
            if (Options.Students != null)
            {
                _worker.RunWorkerAsync();
            }
        }
    }
}
