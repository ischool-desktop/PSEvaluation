using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Aspose.Words;
using Aspose.Words.Reporting;
using System.Windows.Forms;
using Framework;
using System.ComponentModel;
using JHSchool.Data;
using FISCA.Presentation;
using K12.Data;
using System.Xml;
using Campus.Report;
using KaoHsiung.StudentRecordReport.Processor;
using JHSchool.Behavior.BusinessLogic;
using JHSchool.Evaluation.Mapping;

namespace KaoHsiung.StudentRecordReport
{
    public class Report
    {
        private BackgroundWorker _worker;
        private Document _doc;
        private Document _template;
        private ReportConfiguration Config { get; set; }
        private DegreeMapper _degreeMapper;

        bool OneFileSave;
        Dictionary<string, Document> StudentDoc;

        private List<JHStudentRecord> _students;

        public Report()
        {
            _degreeMapper = new JHSchool.Evaluation.Mapping.DegreeMapper();

            InitializeTemplate();
            InitializeBackgroundWorker();
        }

        private void InitializeTemplate()
        {
            _doc = new Document();
            _doc.Sections.Clear();
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
            globalFieldValue.Add(K12.Data.School.ChineseName);

            globalFieldName.Add("列印日期");
            globalFieldValue.Add(DateConvert.CDate(DateTime.Now.ToString("yyyy/MM/dd")));

            globalFieldName.Add("列印時間");
            globalFieldValue.Add(DateTime.Now.ToString("HH:mm:ss"));

            ReportConfiguration _Dylanconfig = new ReportConfiguration(Global.OneFileSave);
            OneFileSave = _Dylanconfig.GetBoolean("單檔儲存", false);
            StudentDoc = new Dictionary<string, Document>();

            double total = _students.Count;
            double count = 0;

            List<string> student_ids = new List<string>();
            foreach (JHStudentRecord item in _students)
                student_ids.Add(item.ID);

            #region 快取資料
            //取得異動資料
            Dictionary<string, List<JHUpdateRecordRecord>> updateRecordCache = new Dictionary<string, List<JHUpdateRecordRecord>>();
            foreach (var record in JHUpdateRecord.SelectByStudentIDs(student_ids))
            {
                if (!updateRecordCache.ContainsKey(record.StudentID))
                    updateRecordCache.Add(record.StudentID, new List<JHUpdateRecordRecord>());
                updateRecordCache[record.StudentID].Add(record);
            }

            //取得缺曠獎懲資料
            Dictionary<string, List<AutoSummaryRecord>> autoSummaryCache = new Dictionary<string, List<AutoSummaryRecord>>();
            foreach (AutoSummaryRecord record in AutoSummary.Select(student_ids, null))
            {
                if (!autoSummaryCache.ContainsKey(record.RefStudentID))
                    autoSummaryCache.Add(record.RefStudentID, new List<AutoSummaryRecord>());
                autoSummaryCache[record.RefStudentID].Add(record);
            }

            //取得學期歷程
            Dictionary<string, JHSemesterHistoryRecord> semesterHistoryCache = new Dictionary<string, JHSemesterHistoryRecord>();
            foreach (var record in JHSemesterHistory.SelectByStudentIDs(student_ids))
            {
                if (!semesterHistoryCache.ContainsKey(record.RefStudentID))
                    semesterHistoryCache.Add(record.RefStudentID, record);
            }

            //取得學期成績
            Dictionary<string, List<JHSemesterScoreRecord>> semesterScoreCache = new Dictionary<string, List<JHSemesterScoreRecord>>();
            foreach (var record in JHSemesterScore.SelectByStudentIDs(student_ids))
            {
                if (!semesterScoreCache.ContainsKey(record.RefStudentID))
                    semesterScoreCache.Add(record.RefStudentID, new List<JHSemesterScoreRecord>());
                semesterScoreCache[record.RefStudentID].Add(record);
            }

            // 取得畢業成績
            Dictionary<string, K12.Data.GradScoreRecord> StudGradScoreDic = new Dictionary<string, GradScoreRecord>();
            foreach (GradScoreRecord score in GradScore.SelectByIDs<GradScoreRecord>(student_ids))
                StudGradScoreDic.Add(score.RefStudentID, score);

            ////課程
            //JHCourse.RemoveAll();
            //Dictionary<string, JHCourseRecord> courseCache = new Dictionary<string, JHCourseRecord>();
            //foreach (JHCourseRecord record in JHCourse.SelectAll())
            //{
            //    if (!courseCache.ContainsKey(record.ID))
            //        courseCache.Add(record.ID, record);
            //}
            #endregion

            #region 判斷要列印的領域科目
            Dictionary<string, bool> domains = new Dictionary<string, bool>();
            //Dictionary<string, List<string>> subjects = new Dictionary<string, List<string>>();

            //List<JHCourseRecord> courseList = new List<JHCourseRecord>(courseCache.Values);

            //courseList.Sort(delegate(JHCourseRecord x, JHCourseRecord y)
            //{
            //    return JHSchool.Evaluation.Subject.CompareSubjectOrdinal(x.Subject, y.Subject);
            //});

            string domainSubjectSetup = Config.GetString("領域科目設定", "Domain");
            if (domainSubjectSetup == "Domain")
            {
                foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                    domains.Add(domain, DomainSubjectExpand.不展開);

                if (!domains.ContainsKey("")) domains.Add("", DomainSubjectExpand.展開);
            }
            else if (domainSubjectSetup == "Subject")
            {
                foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                    domains.Add(domain, DomainSubjectExpand.展開);
                if (!domains.ContainsKey("")) domains.Add("", DomainSubjectExpand.展開);
            }
            else
                throw new Exception("請重新儲存一次列印設定");

            //foreach (var domain in JHSchool.Evaluation.Subject.Domains)
            //    subjects.Add(domain, new List<string>());
            //if (!subjects.ContainsKey("")) subjects.Add("", new List<string>());

            //foreach (var course in courseList)
            //{
            //    if (!subjects.ContainsKey(course.Domain))
            //        subjects.Add(course.Domain, new List<string>());
            //    if (!subjects[course.Domain].Contains(course.Subject) &&
            //        domains.ContainsKey(course.Domain) &&
            //        domains[course.Domain] == false)
            //    {
            //        subjects[course.Domain].Add(course.Subject);
            //    }
            //}
            #endregion

            DocumentBuilder templateBuilder = new DocumentBuilder(_template);

            #region 節權數顯示
            string pcDisplay = string.Empty;
            bool printPeriod = Config.GetBoolean("列印節數", true);
            bool printCredit = Config.GetBoolean("列印權數", false);
            if (printPeriod && printCredit)
                pcDisplay = "節"+Environment.NewLine+"權"+Environment.NewLine+"數";
            else if (printPeriod)
                pcDisplay = "節" + Environment.NewLine + "數";
            else if (printCredit)
                pcDisplay = "權" + Environment.NewLine + "數";

            
            while (templateBuilder.MoveToMergeField("節權數"))
                templateBuilder.Write(pcDisplay);
            #endregion

            #region 文字評語是否列印
            bool printText = Config.GetBoolean("列印文字評語", true);
            if (printText == false)
            {
                templateBuilder.MoveToMergeField("學習領域評量");
                (templateBuilder.CurrentParagraph.ParentNode as Cell).ParentRow.ParentTable.Remove();
            }
            #endregion

            #region 服務學習時數
            Global._SLRDict.Clear();
            Global._SLRDict = Utility.GetServiceLearningDetail(student_ids);
            #endregion

            #region 產生
            foreach (JHStudentRecord student in _students)
            {
                count++;

                Document each = (Document)_template.Clone(true);
                DocumentBuilder builder = new DocumentBuilder(each);

                #region 建立學期歷程對照
                List<SemesterHistoryItem> semesterHistoryList = null;
                if (semesterHistoryCache.ContainsKey(student.ID))
                    semesterHistoryList = semesterHistoryCache[student.ID].SemesterHistoryItems;
                else
                    semesterHistoryList = new List<SemesterHistoryItem>();

                SemesterMap map = new SemesterMap();
                map.SetData(semesterHistoryList);
                #endregion

                #region 學生基本資料
                StudentBasicInfo basicInfo = new StudentBasicInfo(builder);
                basicInfo.SetStudent(student, semesterHistoryList);
                #endregion

                #region 異動資料
                List<JHUpdateRecordRecord> updateRecordList = null;
                if (updateRecordCache.ContainsKey(student.ID))
                    updateRecordList = updateRecordCache[student.ID];
                else
                    updateRecordList = new List<JHUpdateRecordRecord>();

                StudentUpdateRecordProcessor updateRecordProcessor = new StudentUpdateRecordProcessor(builder);
                updateRecordProcessor.SetData(updateRecordList);
                #endregion

                #region 日常表現
                List<AutoSummaryRecord> autoSummaryList = null;
                if (autoSummaryCache.ContainsKey(student.ID))
                    autoSummaryList = autoSummaryCache[student.ID];
                else
                    autoSummaryList = new List<AutoSummaryRecord>();

                StudentMoralProcessor moralProcessor = new StudentMoralProcessor(builder, map);
                moralProcessor.SetData(autoSummaryList);
                #endregion

                #region 學期歷程
                StudentHistoryProcessor history = new StudentHistoryProcessor(builder, map, (semesterHistoryCache.ContainsKey(student.ID) ? semesterHistoryCache[student.ID] : null));
                #endregion

                #region 學期成績
                List<JHSemesterScoreRecord> semesterScoreList = null;
                if (semesterScoreCache.ContainsKey(student.ID))
                    semesterScoreList = semesterScoreCache[student.ID];
                else
                    semesterScoreList = new List<JHSemesterScoreRecord>();

                // 畢業成績
                K12.Data.GradScoreRecord StudGradScore = new GradScoreRecord();
                if (StudGradScoreDic.ContainsKey(student.ID))
                    StudGradScore = StudGradScoreDic[student.ID];

                StudentSemesterScoreProcessor semesterScoreProcessor = new StudentSemesterScoreProcessor(builder, map, domainSubjectSetup, domains, StudGradScore);
                semesterScoreProcessor.DegreeMapper = _degreeMapper;
                semesterScoreProcessor.PrintPeriod = printPeriod;
                semesterScoreProcessor.PrintCredit = printCredit;
                semesterScoreProcessor.SetData(semesterScoreList);
                #endregion

                #region 學習領域評量
                if (printText == true)
                {
                    StudentTextProcessor text = new StudentTextProcessor(builder, map);
                    text.SetData(semesterScoreList);
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
                //throw e.Error;
                MsgBox.Show("產生報表時發生錯誤。" + e.Error.Message);
                return;
            }

            //MotherForm.SetStatusBarMessage(Global.ReportName + "產生完成");
            //ReportSaver.SaveDocument(_doc, Global.ReportName);
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
                        //StudentDoc[each].Save(fbd.SelectedPath + "\\" + each, SaveFormat.AsposePdf);

                        //ReportSaver.SaveDocument(StudentDoc[each], fPath, ReportSaver.OutputType.PDF);
                        //ReportSaver.SaveDocument(StudentDoc[each], each, ReportSaver.OutputType.PDF);                        

                        #region 處理產生 PDF

                        string fPath = fbd.SelectedPath + "\\" + each + ".pdf";

                        FileInfo fi = new FileInfo(fPath);

                        DirectoryInfo folder = new DirectoryInfo(Path.Combine(fi.DirectoryName, Path.GetRandomFileName()));
                        if (!folder.Exists) folder.Create();

                        FileInfo fileinfo = new FileInfo(Path.Combine(folder.FullName, fi.Name));

                        string XmlFileName = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".xml";
                        string PDFFileName = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".pdf";

                        StudentDoc[each].Save(XmlFileName, Aspose.Words.SaveFormat.AsposePdf);

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
            MotherForm.SetStatusBarMessage(Global.ReportName + "產生中", e.ProgressPercentage);
        }

        public void SetStudents(List<JHStudentRecord> students)
        {
            _students = students;
        }

        public void GenerateReport()
        {
            Config = new ReportConfiguration(Global.ReportName);

            #region 取得樣板
            //if (Config.Template == null)
            //    Config.Template = new ReportTemplate(Properties.Resources.高雄國中學籍表, TemplateType.Word);
            //_template = Config.Template.ToDocument();
            _template = new Document(new MemoryStream(Properties.Resources.高雄國中學籍表));
            #endregion

            if (_students != null)
            {
                StudentMoralProcessor.ResetCache();
                _worker.RunWorkerAsync();
            }
        }
    }
}
