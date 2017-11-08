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
using System.Xml;
using JHSchool.Behavior.BusinessLogic;

namespace HsinChu.TransferReport
{
    public class Report
    {
        private const string ReportName = "轉學成績證明書";
        private BackgroundWorker _worker;
        private Document _doc;
        private Document _template;

        private Config _config;
        private List<string> _domains;

        public Report(Config config)
        {
            //_domains = new List<string>(new string[] { "語文", "健康與體育", "社會", "藝術與人文", "數學", "自然與生活科技", "綜合活動", "彈性課程" });
            _domains = new List<string>();
            _domains.AddRange(JHSchool.Evaluation.Subject.Domains);
            _domains.Add("彈性課程");
            _config = config;

            InitializeTemplate();
            InitializeBackgroundWorker();
            //LoadConfiguration();
        }

        //private void LoadConfiguration()
        //{
        //    K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration[ReportName];
        //    #region Before
        //    //if (cd.Contains("領域科目設定") && !string.IsNullOrEmpty(cd["領域科目設定"]))
        //    //{
        //    //    XmlElement element = Framework.XmlHelper.LoadXml(cd["領域科目設定"]);
        //    //    foreach (string domain in _domains)
        //    //    {
        //    //        if (!_config.ContainsKey(domain))
        //    //            _config.Add(domain, new List<string>());

        //    //        XmlElement domainElement = (XmlElement)element.SelectSingleNode("Domain[@Name='" + domain + "']");
        //    //        if (domainElement != null)
        //    //        {
        //    //            foreach (XmlElement subjectElement in domainElement.SelectNodes("Subject"))
        //    //            {
        //    //                string subjectName = subjectElement.GetAttribute("Name");
        //    //                _config[domain].Add(subjectName);
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    #endregion
        //}

        private void InitializeTemplate()
        {
            _doc = new Document();
            _doc.Sections.Clear();

            _template = new Document(new MemoryStream(Properties.Resources.新竹市轉學成績證明書));
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
            List<JHPeriodMappingInfo> periodList = JHPeriodMapping.SelectAll();
            List<JHAbsenceMappingInfo> absenceList = JHAbsenceMapping.SelectAll();

            double total = _config.Students.Count;
            double count = 0;

            List<string> student_ids = new List<string>();
            foreach (JHStudentRecord item in _config.Students)
                student_ids.Add(item.ID);

            #region 快取資料
            Dictionary<string, List<AutoSummaryRecord>> autoSummaryCache = new Dictionary<string, List<AutoSummaryRecord>>();
            foreach (AutoSummaryRecord record in AutoSummary.Select(student_ids, null))
            {
                if (!autoSummaryCache.ContainsKey(record.RefStudentID))
                    autoSummaryCache.Add(record.RefStudentID, new List<AutoSummaryRecord>());
                autoSummaryCache[record.RefStudentID].Add(record);
            }

            Dictionary<string, List<JHUpdateRecordRecord>> updateRecordCache = new Dictionary<string, List<JHUpdateRecordRecord>>();
            foreach (var record in JHUpdateRecord.SelectByStudentIDs(student_ids))
            {
                if (!updateRecordCache.ContainsKey(record.StudentID))
                    updateRecordCache.Add(record.StudentID, new List<JHUpdateRecordRecord>());
                updateRecordCache[record.StudentID].Add(record);
            }

            Dictionary<string, List<JHSemesterScoreRecord>> semesterScoreCache = new Dictionary<string, List<JHSemesterScoreRecord>>();
            foreach (var record in JHSemesterScore.SelectByStudentIDs(student_ids))
            {
                if (!semesterScoreCache.ContainsKey(record.RefStudentID))
                    semesterScoreCache.Add(record.RefStudentID, new List<JHSemesterScoreRecord>());
                semesterScoreCache[record.RefStudentID].Add(record);
            }

            Dictionary<string, JHSemesterHistoryRecord> semesterHistoryCache = new Dictionary<string, JHSemesterHistoryRecord>();
            foreach (var record in JHSemesterHistory.SelectByStudentIDs(student_ids))
            {
                if (!semesterHistoryCache.ContainsKey(record.RefStudentID))
                    semesterHistoryCache.Add(record.RefStudentID, record);
            }
            #endregion

            #region 取得所有科目
            Dictionary<string, SubjectScore> subjectDict = new Dictionary<string, SubjectScore>();
            foreach (JHSemesterScoreRecord record in JHSemesterScore.SelectByStudentIDs(student_ids))
            {
                foreach (SubjectScore subject in record.Subjects.Values)
                {
                    string key = Bind(subject.Domain, subject.Subject);
                    if (!subjectDict.ContainsKey(key))
                        subjectDict.Add(key, subject);
                }
            }

            List<SubjectScore> subjectList = new List<SubjectScore>(subjectDict.Values);
            subjectList.Sort(delegate(SubjectScore x, SubjectScore y)
            {
                List<string> list = new List<string>(new string[] { "國語文", "國文", "英文", "英語", "數學", "歷史", "地理", "公民", "理化", "生物" });
                int ix = list.IndexOf(x.Subject);
                int iy = list.IndexOf(y.Subject);

                if (ix >= 0 && iy >= 0)
                    return ix.CompareTo(iy);
                else if (ix >= 0)
                    return -1;
                else if (iy >= 0)
                    return 1;
                else
                    return x.Subject.CompareTo(y.Subject);
            });
            #endregion

            #region 判斷要列印的領域科目
            Dictionary<string, bool> domains = new Dictionary<string, bool>();
            Dictionary<string, List<string>> subjects = new Dictionary<string, List<string>>();

            if (_config.DomainSubjectSetup == "Domain")
            {
                foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                    domains.Add(domain, true);
                if (domains.ContainsKey("語文")) domains["語文"] = false;
                if (domains.ContainsKey("彈性課程")) domains["彈性課程"] = false;
                if (!domains.ContainsKey("")) domains.Add("", false);
            }
            else if (_config.DomainSubjectSetup == "Subject")
            {
                foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                    domains.Add(domain, false);
                if (!domains.ContainsKey("")) domains.Add("", false);
            }
            else
                throw new Exception("請重新儲存列印設定");

            foreach (var domain in JHSchool.Evaluation.Subject.Domains)
                subjects.Add(domain, new List<string>());
            if (!subjects.ContainsKey("")) subjects.Add("", new List<string>());

            foreach (SubjectScore ss in subjectList)
            {
                if (!subjects.ContainsKey(ss.Domain))
                    subjects.Add(ss.Domain, new List<string>());

                //很怪
                if (domains.ContainsKey(ss.Domain) && domains[ss.Domain] == true) continue;

                if (!subjects[ss.Domain].Contains(ss.Subject))
                    subjects[ss.Domain].Add(ss.Subject);
            }

            _config.SetPrintDomains(domains);
            _config.SetPrintSubjects(subjects);
            #endregion

            #region 依節權數設定，在畫面上顯示
            DocumentBuilder templateBuilder = new DocumentBuilder(_template);

            string pcDisplay = string.Empty;
            if (_config.PrintPeriod && _config.PrintCredit)
                pcDisplay = "節/權數";
            else if (_config.PrintPeriod)
                pcDisplay = "節數";
            else if (_config.PrintCredit)
                pcDisplay = "權數";


            while (templateBuilder.MoveToMergeField("節權數"))
                templateBuilder.Write(pcDisplay);
            #endregion

            #region 服務學習時數
            Config._SLRDict.Clear();
            Config._SLRDict = Utility.GetServiceLearningDetail(student_ids);
            #endregion

            #region 產生
            foreach (JHStudentRecord student in _config.Students)
            {
                count++;
                DocumentBuilder builder = null;

                #region 建立學期歷程對照
                List<SemesterHistoryItem> semesterHistoryList = null;
                if (semesterHistoryCache.ContainsKey(student.ID))
                    semesterHistoryList = semesterHistoryCache[student.ID].SemesterHistoryItems;
                else
                    semesterHistoryList = new List<SemesterHistoryItem>();

                SemesterMap map = new SemesterMap();
                map.SetData(semesterHistoryList);
                #endregion

                Document each = (Document)_template.Clone(true);

                builder = new DocumentBuilder(each);

                #region 基本資料
                StudentBasicInfo basic = new StudentBasicInfo();
                basic.SetStudent(student, semesterHistoryList);

                each.MailMerge.MergeField += delegate(object sender1, MergeFieldEventArgs e1)
                {
                    #region 處理照片
                    if (e1.FieldName == "照片粘貼處")
                    {
                        DocumentBuilder builder1 = new DocumentBuilder(e1.Document);
                        builder1.MoveToField(e1.Field, true);

                        byte[] photoBytes = null;
                        try
                        {
                            photoBytes = Convert.FromBase64String("" + e1.FieldValue);
                        }
                        catch (Exception ex)
                        {
                            builder1.Write("照片粘貼處");
                            e1.Field.Remove();
                            return;
                        }

                        if (photoBytes == null || photoBytes.Length == 0)
                        {
                            builder1.Write("照片粘貼處");
                            e1.Field.Remove();
                            return;
                        }

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

                        photoShape.Height = shapeHeight * 0.9;
                        photoShape.Width = shapeWidth * 0.9;

                        builder1.InsertNode(photoShape);
                    }
                    #endregion
                };

                List<string> fieldName = new List<string>();
                fieldName.AddRange(basic.GetFieldName());
                List<string> fieldValue = new List<string>();
                fieldValue.AddRange(basic.GetFieldValue());
                each.MailMerge.Execute(fieldName.ToArray(), fieldValue.ToArray());
                #endregion

                #region 異動資料
                List<JHUpdateRecordRecord> updateRecordList = null;
                if (updateRecordCache.ContainsKey(student.ID))
                    updateRecordList = updateRecordCache[student.ID];
                else
                    updateRecordList = new List<JHUpdateRecordRecord>();

                StudentUpdateRecordProcessor updateRecordProcessor = new StudentUpdateRecordProcessor(each);
                updateRecordProcessor.SetData(updateRecordList);
                #endregion

                #region 成績資料
                List<JHSemesterScoreRecord> semesterScoreList = null;
                if (semesterScoreCache.ContainsKey(student.ID))
                    semesterScoreList = semesterScoreCache[student.ID];
                else
                    semesterScoreList = new List<JHSemesterScoreRecord>();

                StudentSemesterScoreProcessor semesterScoreProcessor = new StudentSemesterScoreProcessor(builder, map, _config);
                semesterScoreProcessor.SetData(semesterScoreList);
                #endregion

                List<AutoSummaryRecord> autoSummaryList = null;
                if (autoSummaryCache.ContainsKey(student.ID))
                    autoSummaryList = autoSummaryCache[student.ID];
                else
                    autoSummaryList = new List<AutoSummaryRecord>();

                #region 獎懲資料
                StudentDisciplineProcessor disciplineProcessor = new StudentDisciplineProcessor(builder, map);
                disciplineProcessor.SetData(autoSummaryList);
                #endregion

                #region 缺曠資料
                StudentAttendanceProcessor attendanceProcessor = new StudentAttendanceProcessor(builder, map);
                attendanceProcessor.SetData(autoSummaryList);
                #endregion

                #region 日常行為
                StudentTextScoreProcessor textScoreProcessor = new StudentTextScoreProcessor(builder, map);
                textScoreProcessor.SetData(autoSummaryList);
                #endregion

                SemesterHistoryProcessor semesterHistoryProcessor = new SemesterHistoryProcessor(builder, map);

                foreach (Section sec in each.Sections)
                    _doc.Sections.Add(_doc.ImportNode(sec, true));

                //回報進度
                _worker.ReportProgress((int)(count * 100.0 / total));
            }

            List<string> globalFieldName = new List<string>();
            List<object> globalFieldValue = new List<object>();

            globalFieldName.Add("學校名稱");
            globalFieldValue.Add(School.ChineseName);

            globalFieldName.Add("列印日期");
            globalFieldValue.Add(Common.CDate(DateTime.Now.ToString("yyyy/MM/dd")) + " " + DateTime.Now.ToString("HH:mm:ss"));

            string chancellor, eduDirector, stuDirector;
            chancellor = eduDirector = stuDirector = string.Empty;

            XmlElement info = School.Configuration["學校資訊"].PreviousData;
            XmlElement chancellorElement = (XmlElement)info.SelectSingleNode("ChancellorChineseName");
            XmlElement eduDirectorElement = (XmlElement)info.SelectSingleNode("EduDirectorName");
            XmlElement stuDirectorElement = (XmlElement)info.SelectSingleNode("StuDirectorName");

            if (chancellorElement != null) chancellor = chancellorElement.InnerText;
            if (eduDirectorElement != null) eduDirector = eduDirectorElement.InnerText;
            if (stuDirectorElement != null) stuDirector = stuDirectorElement.InnerText;

            globalFieldName.Add("教務主任");
            globalFieldValue.Add(eduDirector);

            globalFieldName.Add("學務主任");
            globalFieldValue.Add(stuDirector);

            globalFieldName.Add("校長");
            globalFieldValue.Add(chancellor);

            _doc.MailMerge.Execute(globalFieldName.ToArray(), globalFieldValue.ToArray());
            #endregion
        }

        private string Bind(string x, string y)
        {
            return x + "_" + y;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MsgBox.Show("產生報表時發生錯誤。" + e.Error.Message);
                return;
            }

            #region 儲存
            if (_doc.Sections.Count > 0)
            {
                string path = Path.Combine(Application.StartupPath, "Reports");
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists) dir.Create();

                path = Path.Combine(path, ReportName + ".doc");

                if (File.Exists(path))
                {
                    int i = 1;
                    while (true)
                    {
                        string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                        if (!File.Exists(newPath))
                        {
                            path = newPath;
                            break;
                        }
                    }
                }

                try
                {
                    _doc.Save(path);
                    FISCA.LogAgent.ApplicationLog.Log("成績系統.報表", "列印" + ReportName, string.Format("產生{0}份{1}", _config.Students.Count, ReportName));

                    MotherForm.SetStatusBarMessage(ReportName + "產生完成");

                    if (MsgBox.Show(ReportName + "產生完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(path);
                    }
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存失敗");
                }
            }
            #endregion
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MotherForm.SetStatusBarMessage(ReportName + "產生中", e.ProgressPercentage);
        }

        public void GenerateReport()
        {
            if (_config.Students != null)
                _worker.RunWorkerAsync();
        }
    }
}
