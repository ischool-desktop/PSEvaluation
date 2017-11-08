using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Framework;
using JHSchool.Evaluation.Calculation;
using JHSchool.Evaluation.Calculation.GraduationConditions;
using Aspose.Cells;
using System.IO;
using JHSchool.Data;
using Campus.Report;
using FISCA.Presentation;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReportControls
{
    public partial class GraduationPredictReportForm : FISCA.Presentation.Controls.BaseForm
    {
        private List<StudentRecord> _errorList;
        private Dictionary<string, bool> _passList;
        private EvaluationResult _result;
        private List<StudentRecord> _students;
        private BackgroundWorker _ExportWorker;
        private Aspose.Words.Document _doc;
        private Aspose.Words.Document _template;
        private ReportConfiguration _rc;
        private string ReportName = "";
        // 是否產生學生清單
        bool _UserSelExportStudentList = false;
        Workbook _wbStudentList;
        private MultiThreadBackgroundWorker<StudentRecord> _historyWorker, _inspectWorker;
        private const string _NewLine = "\r\n"; // 小郭, 2013/12/25

        public GraduationPredictReportForm(List<StudentRecord> students)
        {
            InitializeComponent();
            //ReportName = "未達畢業標準通知單";
            //_rc = new ReportConfiguration(ReportName);

            _students = students;
            _errorList = new List<StudentRecord>();
            _passList = new Dictionary<string, bool>();
            _result = new EvaluationResult();
            _doc = new Aspose.Words.Document();

            //if (_rc.Template == null)
            //    _rc.Template = new ReportTemplate(Properties.Resources.未達畢業標準通知單樣板, TemplateType.Word);

            //_template = _rc.Template.ToDocument();

            InitializeWorkers();
        }

        private void InitializeWorkers()
        {
            _historyWorker = new MultiThreadBackgroundWorker<StudentRecord>();
            _historyWorker.Loading = MultiThreadLoading.Light;
            _historyWorker.PackageSize = _students.Count; //暫解
            _historyWorker.AutoReportsProgress = true;
            _historyWorker.DoWork += new EventHandler<PackageDoWorkEventArgs<StudentRecord>>(HistoryWorker_DoWork);
            _historyWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(HistoryWorker_RunWorkerCompleted);
            _historyWorker.ProgressChanged += new ProgressChangedEventHandler(HistoryWorker_ProgressChanged);

            _inspectWorker = new MultiThreadBackgroundWorker<StudentRecord>();
            _inspectWorker.Loading = MultiThreadLoading.Light;
            _inspectWorker.PackageSize = _students.Count; //暫解
            _inspectWorker.AutoReportsProgress = true;
            _inspectWorker.DoWork += new EventHandler<PackageDoWorkEventArgs<StudentRecord>>(InspectWorker_DoWork);
            _inspectWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(InspectWorker_RunWorkerCompleted);
            _inspectWorker.ProgressChanged += new ProgressChangedEventHandler(InspectWorker_ProgressChanged);

            _ExportWorker = new BackgroundWorker();
            _ExportWorker.DoWork += new DoWorkEventHandler(_ExportWorker_DoWork);
            _ExportWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_ExportWorker_RunWorkerCompleted);
        }

        #region HistoryWorker Event
        private void HistoryWorker_DoWork(object sender, PackageDoWorkEventArgs<StudentRecord> e)
        {
            try
            {
                List<StudentRecord> error_list = e.Argument as List<StudentRecord>;
                error_list.AddRange(Graduation.Instance.CheckSemesterHistories(e.Items));
            }
            catch (Exception ex)
            {
                SmartSchool.ErrorReporting.ReportingService.ReportException(ex);
                MsgBox.Show("檢查學期歷程時發生錯誤。" + ex.Message);
            }
        }

        private void HistoryWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //progressBar.Value = 100;

            if (e.Error != null)
            {
                SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);
                MsgBox.Show("檢查學期歷程時發生錯誤。" + e.Error.Message);
                return;
            }

            if (_errorList.Count > 0)
            {
                btnExit.Enabled = true;

                JHSchool.Evaluation.Calculation_for_JHEvaluation.ScoreCalculation.ErrorViewer viewer = new JHSchool.Evaluation.Calculation_for_JHEvaluation.ScoreCalculation.ErrorViewer();
                viewer.SetHeader("學生");
                foreach (StudentRecord student in _errorList)
                    viewer.SetMessage(student, new List<string>(new string[] { "學期歷程不完整" }));
                viewer.ShowDialog();
                return;
            }
            else
            {
                // 加入這段主要在處理當學期還沒有產生學期歷程，資料可以判斷
                UIConfig._StudentSHistoryRecDict.Clear();
                Dictionary<string, int> studGradeYearDict = new Dictionary<string, int>();

                // 取得學生ID
                List<string> studIDs = (from data in _students select data.ID).Distinct().ToList();

                foreach (JHStudentRecord stud in JHStudent.SelectByIDs(studIDs))
                {
                    if (stud.Class != null)
                    {
                        if (stud.Class.GradeYear.HasValue)
                            if (!studGradeYearDict.ContainsKey(stud.ID))
                                studGradeYearDict.Add(stud.ID, stud.Class.GradeYear.Value);
                    }
                }
                bool checkInsShi = false;
                // 取得學生學期歷程，並加入學生學習歷程Cache
                foreach (JHSemesterHistoryRecord rec in JHSemesterHistory.SelectByStudentIDs(studIDs))
                {
                    checkInsShi = true;
                    K12.Data.SemesterHistoryItem shi = new K12.Data.SemesterHistoryItem();
                    shi.SchoolYear = UIConfig._UserSetSHSchoolYear;
                    shi.Semester = UIConfig._UserSetSHSemester;
                    if (studGradeYearDict.ContainsKey(rec.RefStudentID))
                        shi.GradeYear = studGradeYearDict[rec.RefStudentID];

                    foreach (K12.Data.SemesterHistoryItem shiItem in rec.SemesterHistoryItems)
                        if (shiItem.SchoolYear == shi.SchoolYear && shiItem.Semester == shi.Semester)
                            checkInsShi = false;
                    if (checkInsShi)
                        rec.SemesterHistoryItems.Add(shi);

                    if (!UIConfig._StudentSHistoryRecDict.ContainsKey(rec.RefStudentID))
                        UIConfig._StudentSHistoryRecDict.Add(rec.RefStudentID, rec);
                }

                //lblProgress.Text = "畢業資格審查中…";
                FISCA.LogAgent.ApplicationLog.Log("成績系統.報表", "列印畢業預警報表", "產生畢業預警報表");

                _inspectWorker.RunWorkerAsync(_students, new object[] { _passList, _result });
            }
        }

        private void HistoryWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progressBar.Value = e.ProgressPercentage;
        }
        #endregion

        #region InspectWorker Event
        private void InspectWorker_DoWork(object sender, PackageDoWorkEventArgs<StudentRecord> e)
        {
            try
            {
                object[] objs = e.Argument as object[];
                Dictionary<string, bool> passList = objs[0] as Dictionary<string, bool>;
                EvaluationResult result = objs[1] as EvaluationResult;

                StudentDomainResult.Clear();    // 小郭, 2013/12/30
                Dictionary<string, bool> list = Graduation.Instance.Evaluate(e.Items);
                foreach (string id in list.Keys)
                {
                    if (!passList.ContainsKey(id))
                        passList.Add(id, list[id]);
                }

                if (Graduation.Instance.Result.Count > 0)
                {
                    MergeResult(list, Graduation.Instance.Result, result);
                }
            }
            catch (Exception ex)
            {
                SmartSchool.ErrorReporting.ReportingService.ReportException(ex);
                MsgBox.Show("發生錯誤。" + ex.Message);
            }
        }

        private void InspectWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //progressBar.Value = 100;
            if (e.Error != null)
            {
                //btnExit.Enabled = true;
                SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);
                MsgBox.Show("審查時發生錯誤。" + e.Error.Message);
                return;
            }

            //lblProgress.Text = "審查完成";

            if (_passList.Count > 0)
            {
                //lblProgress.Text = "寫入類別資訊…";
                PrintOut();

                // 檢查是否需要產生通知單
                if (checkExportDoc.Checked)
                {
                    _ExportWorker.RunWorkerAsync();
                }

                btnPrint.Enabled = true;
            }


        }

        // 未達畢業標準通知單
        void _ExportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MsgBox.Show("產生報表時發生錯誤。" + e.Error.Message);
                return;
            }

            MotherForm.SetStatusBarMessage(ReportName + "產生完成");
            ReportSaver.SaveDocument(_doc, ReportName);

            // 檢查是否產生學生清單
            if (_UserSelExportStudentList)
            {
                if (_wbStudentList != null)
                {
                    SaveFileDialog sd = new SaveFileDialog();
                    sd.FileName = "學生清單(未達畢業標準學生)";
                    sd.Filter = "Excel檔案(*.xls)|*.xls";
                    if (sd.ShowDialog() != DialogResult.OK) return;

                    try
                    {
                        _wbStudentList.Save(sd.FileName, FileFormatType.Excel2003);

                        if (MsgBox.Show("報表產生完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(sd.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        SmartSchool.ErrorReporting.ReportingService.ReportException(ex);
                        MsgBox.Show("儲存失敗。" + ex.Message);
                    }
                }
            }
        }

        void _ExportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ExportDoc();
        }

        /// <summary>
        /// 日期轉換(2011/1/1)->民國年(100年1月1日)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string ConvertDate1(DateTime dt)
        {
            string retVal = string.Empty;
            retVal = (dt.Year - 1911) + "年" + dt.Month + "月" + dt.Day + "日";
            return retVal;
        }

        /// <summary>
        /// 未達畢業標準通知單
        /// </summary>
        private void ExportDoc()
        {
            if (_students.Count == 0) return;
            _doc.Sections.Clear();

            if (_rc.Template == null)
                _rc.Template = new ReportTemplate(JHEvaluation.ScoreCalculation.Properties.Resources.未達畢業標準通知單樣板, TemplateType.Word);

            _template = _rc.Template.ToDocument();

            string UserSelAddresseeAddress = _rc.GetString(PrintConfigForm.setupAddresseeAddress, "聯絡地址");
            string UserSelAddresseeName = _rc.GetString(PrintConfigForm.setupAddresseeName, "監護人");
            _UserSelExportStudentList = _rc.GetBoolean(PrintConfigForm.setupExportStudentList, false);

            string UserSeldtDate = "";
            DateTime dt;
            if (DateTime.TryParse(_rc.GetString(PrintConfigForm.setupdtDocDate, ""), out dt))
                UserSeldtDate = ConvertDate1(dt);
            else
                UserSeldtDate = ConvertDate1(DateTime.Now);

            List<StudentGraduationPredictData> StudentGraduationPredictDataList = new List<StudentGraduationPredictData>();
            // 取得學生ID，製作 Dict 用
            List<string> StudIDList = (from data in _students select data.ID).ToList();

            // Student Address,Key:StudentID
            Dictionary<string, JHAddressRecord> AddressDict = new Dictionary<string, JHAddressRecord>();
            // Student Parent,Key:StudentID
            Dictionary<string, JHParentRecord> ParentDict = new Dictionary<string, JHParentRecord>();

            // 地址
            foreach (JHAddressRecord rec in JHAddress.SelectByStudentIDs(StudIDList))
                if (!AddressDict.ContainsKey(rec.RefStudentID))
                    AddressDict.Add(rec.RefStudentID, rec);

            // 父母監護人
            foreach (JHParentRecord rec in JHParent.SelectByStudentIDs(StudIDList))
                if (!ParentDict.ContainsKey(rec.RefStudentID))
                    ParentDict.Add(rec.RefStudentID, rec);




            // 資料轉換 ..
            foreach (StudentRecord StudRec in _students)
            {
                if (!_result.ContainsKey(StudRec.ID)) continue;

                //處理年級為0的資料
                List<ResultDetail> zeroGrades = new List<ResultDetail>();

                StudentGraduationPredictData sgpd = new StudentGraduationPredictData();

                if (StudRec.Class != null)
                    sgpd.ClassName = StudRec.Class.Name;

                sgpd.Name = StudRec.Name;

                sgpd.SchoolAddress = K12.Data.School.Address;
                sgpd.SchoolName = K12.Data.School.ChineseName;
                sgpd.SchoolPhone = K12.Data.School.Telephone;

                sgpd.SeatNo = StudRec.SeatNo;
                sgpd.StudentNumber = StudRec.StudentNumber;

                // 文字
                if (_result.ContainsKey(StudRec.ID))
                {
                    int GrYear;
                    foreach (ResultDetail rd in _result[StudRec.ID])
                    {
                        if (int.TryParse(rd.GradeYear, out GrYear))
                        {
                            //if (GrYear == 0) continue;
                            //後續處理
                            if (GrYear == 0)
                            {
                                zeroGrades.Add(rd);
                            }

                            // 組訊息
                            string Detail = "";
                            if (rd.Details.Count > 0)
                                Detail = string.Join(",", rd.Details.ToArray());

                            // 一年級
                            if (GrYear == 1 || GrYear == 7)
                            {
                                if (rd.Semester.Trim() == "1")
                                    sgpd.Text11 = Detail;

                                if (rd.Semester.Trim() == "2")
                                    sgpd.Text12 = Detail;
                            }

                            // 二年級
                            if (GrYear == 2 || GrYear == 8)
                            {
                                if (rd.Semester.Trim() == "1")
                                    sgpd.Text21 = Detail;

                                if (rd.Semester.Trim() == "2")
                                    sgpd.Text22 = Detail;
                            }

                            // 三年級
                            if (GrYear == 3 || GrYear == 9)
                            {
                                if (rd.Semester.Trim() == "1")
                                    sgpd.Text31 = Detail;

                                if (rd.Semester.Trim() == "2")
                                    sgpd.Text32 = Detail;
                            }

                        }
                    }
                }

                // 地址
                if (AddressDict.ContainsKey(StudRec.ID))
                {
                    if (UserSelAddresseeAddress == "聯絡地址")
                        sgpd.AddresseeAddress = AddressDict[StudRec.ID].MailingAddress;

                    if (UserSelAddresseeAddress == "戶籍地址")
                        sgpd.AddresseeAddress = AddressDict[StudRec.ID].PermanentAddress;
                }

                // 父母監護人
                if (ParentDict.ContainsKey(StudRec.ID))
                {
                    if (UserSelAddresseeName == "父親")
                        sgpd.AddresseeName = ParentDict[StudRec.ID].FatherName;

                    if (UserSelAddresseeName == "母親")
                        sgpd.AddresseeName = ParentDict[StudRec.ID].MotherName;

                    if (UserSelAddresseeName == "監護人")
                        sgpd.AddresseeName = ParentDict[StudRec.ID].CustodianName;
                }

                sgpd.DocDate = UserSeldtDate;

                // 說明文字
                StringBuilder sbText = new StringBuilder();
                List<string> textList = new List<string>();
                List<string> textSubList = new List<string>();
                foreach (ResultDetail rd in zeroGrades)
                    sbText.Append(string.Join(",", rd.Details.ToArray()));


                sbText.AppendLine("");
                textList.Clear();

                // 領域
                if (StudentDomainResult._DomainResult.ContainsKey(StudRec.ID))
                {
                    foreach (string dName in StudentDomainResult._DomainResult[StudRec.ID].Keys)
                    {
                        textList.Add(dName + ":" + StudentDomainResult._DomainResult[StudRec.ID][dName].domainScore);
                    }
                }

                if (TempData.tmpStudDomainScoreDict.ContainsKey(StudRec.ID))
                {
                    foreach (string key in TempData.tmpStudDomainScoreDict[StudRec.ID].Keys)
                    {
                        // 小數下第2位四捨五入
                        decimal sc = Math.Round(TempData.tmpStudDomainScoreDict[StudRec.ID][key] / TempData.tmpStudDomainCreditDict[StudRec.ID][key], 2, MidpointRounding.AwayFromZero);
                        textList.Add(key + ":" + sc);
                    }
                }

                if (textList.Count > 0)
                {
                    sbText.Append("各領域加權總平均：");
                    sbText.AppendLine(string.Join(",", textList.ToArray()));
                }

                // 缺曠
                if (TempData.tmpStudentAbsenceAmountAllDict.ContainsKey(StudRec.ID))
                {
                    textList.Clear();

                    foreach (string key in TempData.tmpStudentAbsenceAmountAllDict[StudRec.ID].Keys)
                    {
                        textSubList.Clear();
                        foreach (string ss in TempData.tmpStudentAbsenceAmountAllDict[StudRec.ID][key].Keys)
                            textSubList.Add(ss + ":" + TempData.tmpStudentAbsenceAmountAllDict[StudRec.ID][key][ss]);

                        if (textSubList.Count > 0)
                            textList.Add(key + ":" + string.Join(",", textSubList.ToArray()));
                    }

                    if (textList.Count > 0)
                        sbText.Append("各學期缺曠統計：");

                    sbText.AppendLine(string.Join(",", textList.ToArray()));
                }

                // 獎懲
                if (TempData.tmpStudentDemeritAmountAllDict.ContainsKey(StudRec.ID))
                {
                    textList.Clear();
                    foreach (string key in TempData.tmpStudentDemeritAmountAllDict[StudRec.ID].Keys)
                    {
                        textSubList.Clear();
                        foreach (string ss in TempData.tmpStudentDemeritAmountAllDict[StudRec.ID][key].Keys)
                        {
                            if (TempData.tmpStudentDemeritAmountAllDict[StudRec.ID][key][ss] > 0)
                                textSubList.Add(ss + ":" + TempData.tmpStudentDemeritAmountAllDict[StudRec.ID][key][ss]);
                        }

                        if (textSubList.Count > 0)
                            textList.Add(key + ":" + string.Join(",", textSubList.ToArray()));
                    }
                    if (textList.Count > 0)
                        sbText.Append("各學期獎懲統計：");
                    sbText.Append(string.Join(",", textList.ToArray()));
                }

                sgpd.Text = sbText.ToString();

                StudentGraduationPredictDataList.Add(sgpd);
            }

            // 產生Word 套印
            Dictionary<string, object> FieldData = new Dictionary<string, object>();

            // 班座排序
            StudentGraduationPredictDataList = (from data in StudentGraduationPredictDataList orderby data.ClassName, data.SeatNo.PadLeft(3, '0') ascending select data).ToList();

            foreach (StudentGraduationPredictData sgpd in StudentGraduationPredictDataList)
            {
                FieldData.Clear();
                FieldData.Add("學校名稱", sgpd.SchoolName);
                FieldData.Add("學校電話", sgpd.SchoolPhone);
                FieldData.Add("學校地址", sgpd.SchoolAddress);
                FieldData.Add("收件人地址", sgpd.AddresseeAddress);
                FieldData.Add("收件人姓名", sgpd.AddresseeName);
                FieldData.Add("班級", sgpd.ClassName);
                FieldData.Add("座號", sgpd.SeatNo);
                FieldData.Add("姓名", sgpd.Name);
                FieldData.Add("學號", sgpd.StudentNumber);
                FieldData.Add("一上文字", sgpd.Text11);
                FieldData.Add("一下文字", sgpd.Text12);
                FieldData.Add("二上文字", sgpd.Text21);
                FieldData.Add("二下文字", sgpd.Text22);
                FieldData.Add("三上文字", sgpd.Text31);
                FieldData.Add("三下文字", sgpd.Text32);
                FieldData.Add("發文日期", sgpd.DocDate);
                FieldData.Add("所有說明", sgpd.Text);

                Aspose.Words.Document each = (Aspose.Words.Document)_template.Clone(true);
                Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(each);
                // 合併
                if (FieldData.Count > 0)
                    builder.Document.MailMerge.Execute(FieldData.Keys.ToArray(), FieldData.Values.ToArray());

                foreach (Aspose.Words.Section sec in each.Sections)
                    _doc.Sections.Add(_doc.ImportNode(sec, true));

            }

            // 產生學生清單
            if (_UserSelExportStudentList)
            {
                _wbStudentList = new Workbook();
                _wbStudentList.Worksheets[0].Cells[0, 0].PutValue("班級");
                _wbStudentList.Worksheets[0].Cells[0, 1].PutValue("座號");
                _wbStudentList.Worksheets[0].Cells[0, 2].PutValue("學號");
                _wbStudentList.Worksheets[0].Cells[0, 3].PutValue("學生姓名");
                _wbStudentList.Worksheets[0].Cells[0, 4].PutValue("收件人姓名");
                _wbStudentList.Worksheets[0].Cells[0, 5].PutValue("地址");
                //班級	座號	學號	學生姓名	收件人姓名	地址

                int rowIdx = 1;
                foreach (StudentGraduationPredictData sgpd in StudentGraduationPredictDataList)
                {
                    _wbStudentList.Worksheets[0].Cells[rowIdx, 0].PutValue(sgpd.ClassName);
                    _wbStudentList.Worksheets[0].Cells[rowIdx, 1].PutValue(sgpd.SeatNo);
                    _wbStudentList.Worksheets[0].Cells[rowIdx, 2].PutValue(sgpd.StudentNumber);
                    _wbStudentList.Worksheets[0].Cells[rowIdx, 3].PutValue(sgpd.Name);
                    _wbStudentList.Worksheets[0].Cells[rowIdx, 4].PutValue(sgpd.AddresseeName);
                    _wbStudentList.Worksheets[0].Cells[rowIdx, 5].PutValue(sgpd.AddresseeAddress);
                    rowIdx++;
                }

                _wbStudentList.Worksheets[0].AutoFitColumns();
            }
        }


        /// <summary>
        /// 未達畢業標準名冊
        /// </summary>
        private void PrintOut()
        {
            if (_students.Count <= 0) return;

            SaveFileDialog sd = new SaveFileDialog();
            sd.FileName = "未達畢業標準學生名冊";
            sd.Filter = "Excel檔案(*.xls)|*.xls";
            if (sd.ShowDialog() != DialogResult.OK) return;

            Workbook template = new Workbook();
            template.Open(new MemoryStream(JHEvaluation.ScoreCalculation.Properties.Resources.未達畢業標準學生名冊template));
            Worksheet tempsheet = template.Worksheets[0];
            Worksheet tempsheet1 = template.Worksheets[1];

            Workbook book = new Workbook();
            book.Open(new MemoryStream(JHEvaluation.ScoreCalculation.Properties.Resources.未達畢業標準學生名冊template));
            //Worksheet sheet = book.Worksheets[0];
            //Worksheet sheet1 = book.Worksheets[1];
            //sheet.Name = "未達畢業標準學生";
            //sheet1.Name = "未達畢業標準學生-依畢業總平均";

            Range temprow = tempsheet.Cells.CreateRange(3, 1, false);

            //sheet.Cells[0, 0].PutValue(School.DefaultSchoolYear + "學年度 " + School.ChineseName);

            //Range temprow1 = tempsheet1.Cells.CreateRange(3, 1, false);
            Range temprow2 = tempsheet1.Cells.CreateRange(3, 1, false);

            //sheet1.Cells[0, 0].PutValue(School.DefaultSchoolYear + "學年度 " + School.ChineseName);

            book.Worksheets[0].Cells[0, 0].PutValue(School.DefaultSchoolYear + "學年度 " + School.ChineseName);
            book.Worksheets[1].Cells[0, 0].PutValue(School.DefaultSchoolYear + "學年度 " + School.ChineseName);

            //int rowIndex = 3, rowIndex1 = 3; ;
            int rowIndex = 3;
            int sheet2_Index = 3;

            List<StudentRecord> sorted = new List<StudentRecord>();
            foreach (StudentRecord stu in _students)
                sorted.Add(stu);
            sorted.Sort();

            foreach (StudentRecord stu in sorted)
            {
                if (!_result.ContainsKey(stu.ID)) continue;

                //正常年級的清單
                List<ResultDetail> regularGrades = new List<ResultDetail>();
                //年級為0的清單
                List<ResultDetail> zeroGrades = new List<ResultDetail>();

                foreach (ResultDetail rd in _result[stu.ID])
                {
                    int gradeYear = int.Parse(rd.GradeYear);

                    //分類result
                    if (gradeYear == 0)
                    {
                        zeroGrades.Add(rd);
                    }
                    else
                    {
                        regularGrades.Add(rd);
                    }
                }

                if (regularGrades.Count > 0)
                {
                    Worksheet sheet = book.Worksheets[0];
                    sheet.Cells.CreateRange(rowIndex, 1, false).Copy(temprow);
                    sheet.Cells.CreateRange(rowIndex, 1, false).CopyStyle(temprow);

                    //學生資訊
                    if (stu.Class != null) sheet.Cells[rowIndex, 0].PutValue(stu.Class.Name);
                    sheet.Cells[rowIndex, 1].PutValue(stu.SeatNo);
                    sheet.Cells[rowIndex, 2].PutValue(stu.StudentNumber);
                    sheet.Cells[rowIndex, 3].PutValue(stu.Name);

                    foreach (ResultDetail rd in regularGrades)
                    {
                        int gradeYear = int.Parse(rd.GradeYear);

                        if (gradeYear > 6) gradeYear -= 6;

                        int index = (gradeYear - 1) * 2 + int.Parse(rd.Semester);

                        string details = string.Empty;
                        foreach (string detail in rd.Details)
                            details += detail + ",";
                        if (details.EndsWith(",")) details = details.Substring(0, details.Length - 1);
                        sheet.Cells[rowIndex, index + 3].PutValue(details);
                    }

                    rowIndex++;
                }

                if (zeroGrades.Count > 0)
                {
                    //處理年級為0的result
                    foreach (ResultDetail rd in zeroGrades)
                    {
                        //學期也為0代表是所有學期
                        if (rd.Semester == "0")
                        {
                            Worksheet sheet = book.Worksheets[1];
                            sheet.Cells.CreateRange(sheet2_Index, 1, false).Copy(temprow2);
                            sheet.Cells.CreateRange(sheet2_Index, 1, false).CopyStyle(temprow2);

                            if (stu.Class != null) sheet.Cells[sheet2_Index, 0].PutValue(stu.Class.Name);
                            sheet.Cells[sheet2_Index, 1].PutValue(stu.SeatNo);
                            sheet.Cells[sheet2_Index, 2].PutValue(stu.StudentNumber);
                            sheet.Cells[sheet2_Index, 3].PutValue(stu.Name);

                            string val = "";
                            foreach (string str in rd.Details)
                            {
                                val += str + ",";
                            }
                            if (val.EndsWith(",")) val = val.Substring(0, val.Length - 1);
                            sheet.Cells[sheet2_Index, 4].PutValue(val);

                        }
                    }

                    sheet2_Index++;
                }
            }

            #region 小郭版
            /*
            #region 處理畢業成績類類
            foreach (StudentRecord stu in sorted)
            {
                if (!_result.ContainsKey(stu.ID)) continue;

                sheet1.Cells.CreateRange(rowIndex1, 1, false).Copy(temprow1);
                sheet1.Cells.CreateRange(rowIndex1, 1, false).CopyStyle(temprow1);

                if (stu.Class != null) sheet1.Cells[rowIndex1, 0].PutValue(stu.Class.Name);
                sheet1.Cells[rowIndex1, 1].PutValue(stu.SeatNo);
                sheet1.Cells[rowIndex1, 2].PutValue(stu.StudentNumber);
                sheet1.Cells[rowIndex1, 3].PutValue(stu.Name);


                foreach (ResultDetail rd in _result[stu.ID])
                {
                    int gradeYear = int.Parse(rd.GradeYear);

                    if (gradeYear == 0)
                    {
                        string details = string.Empty;
                        foreach (string detail in rd.Details)
                            details += detail + "," + _NewLine;   // 小郭, 2013/12/25
                        if (details.EndsWith("," + _NewLine)) details = details.Substring(0, details.Length - ("," + _NewLine).Length); // 小郭, 2013/12/25

                        sheet1.Cells[rowIndex1, 4].PutValue(details);
                    }
                }

                rowIndex1++;
            }
            #endregion

            #region 處理學期類
            foreach (StudentRecord stu in sorted)
            {
                if (!_result.ContainsKey(stu.ID)) continue;

                sheet.Cells.CreateRange(rowIndex, 1, false).Copy(temprow);
                sheet.Cells.CreateRange(rowIndex, 1, false).CopyStyle(temprow);

                if (stu.Class != null) sheet.Cells[rowIndex, 0].PutValue(stu.Class.Name);
                sheet.Cells[rowIndex, 1].PutValue(stu.SeatNo);
                sheet.Cells[rowIndex, 2].PutValue(stu.StudentNumber);
                sheet.Cells[rowIndex, 3].PutValue(stu.Name);


                foreach (ResultDetail rd in _result[stu.ID])
                {
                    int gradeYear = int.Parse(rd.GradeYear);

                    if (gradeYear == 0) continue;

                    if (gradeYear > 6) gradeYear -= 6;

                    int index = (gradeYear - 1) * 2 + int.Parse(rd.Semester);

                    string details = string.Empty;
                    foreach (string detail in rd.Details)
                        details += detail + "," + _NewLine;   // 小郭, 2013/12/25
                    if (details.EndsWith("," + _NewLine)) details = details.Substring(0, details.Length - ("," + _NewLine).Length); // 小郭, 2013/12/25
                    sheet.Cells[rowIndex, index + 3].PutValue(details);
                }

                if (stu.Class != null)
                {
                    int i;
                    if (int.TryParse(stu.Class.GradeYear, out i))
                    {
                        int last = (i - 1) * 2 + 2;
                        for (int j = 6; j > last; j--)
                            sheet.Cells[rowIndex, j + 3].PutValue("- - - - -");
                    }
                }

                rowIndex++;
            }
            #endregion
            */
            #endregion

            // 產出"學習領域畢業總平均明細", 小郭, 2013/12/30
            if (Config.rpt_isCheckGraduateDomain == true)
            {
                OutStudentDomainData(book, sorted);
                ExportStudentDemeritAmountAllData(book, sorted);
                ExportStudentAbsenceAmountAllData(book, sorted);
            }
            try
            {

                // 判斷工作表是否顯示
                if (Config.rpt_isCheckGraduateDomain == false)
                    book.Worksheets.RemoveAt("未達畢業標準學生-依畢業總平均");

                if (Config.rpt_isCheckSemesterDomain == false)
                    book.Worksheets.RemoveAt("未達畢業標準學生");


                book.Save(sd.FileName, FileFormatType.Excel2003);

                if (MsgBox.Show("報表產生完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(sd.FileName);
                }
            }
            catch (Exception ex)
            {
                SmartSchool.ErrorReporting.ReportingService.ReportException(ex);
                MsgBox.Show("儲存失敗。" + ex.Message);
            }
        }

        private void InspectWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progressBar.Value = e.ProgressPercentage;
        }
        #endregion

        private void btnPrint_Click(object sender, EventArgs e)
        {
            _result.Clear();

            //取得通知單樣板
            ReportName = "未達畢業標準通知單";
            _rc = new ReportConfiguration(ReportName);

            if (_rc.Template == null)
                _rc.Template = new ReportTemplate(JHEvaluation.ScoreCalculation.Properties.Resources.未達畢業標準通知單樣板, TemplateType.Word);

            _template = _rc.Template.ToDocument();

            // 檢查學業勾選
            Config.rpt_isCheckGraduateDomain = Config.rpt_isCheckSemesterDomain = false;
            /* 小郭, 2013/12/25
            if (chCondition1.Checked == true || chCondition2.Checked == true)
                Config.rpt_isCheckSemesterDomain = true;

            if (chConditionGr1.Checked)
                Config.rpt_isCheckGraduateDomain = true;
            */
            // 取得使用者選擇學年度學期
            UIConfig._UserSetSHSchoolYear = iptSchoolYear.Value;
            UIConfig._UserSetSHSemester = iptSemester.Value;

            if (UIConfig._UserSetSHSchoolYear == 0 || UIConfig._UserSetSHSemester == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("日前學年度或學期輸入錯誤.");
                return;
            }


            List<string> conditions = new List<string>();

            // 檢查畫面上畢業成績使用者勾選
            foreach (Control ctrl in gpScore.Controls)
            {
                if (ctrl is System.Windows.Forms.CheckBox)
                {
                    System.Windows.Forms.CheckBox chk = ctrl as System.Windows.Forms.CheckBox;
                    if (chk.Checked && !string.IsNullOrEmpty("" + chk.Tag))
                    {
                        if (!conditions.Contains("" + chk.Tag))
                            conditions.Add("" + chk.Tag);
                    }
                }
            }

            // 檢查畫面上日常生活表現使用者勾選
            foreach (Control ctrl in gpDaily.Controls)
            {
                if (ctrl is System.Windows.Forms.CheckBox)
                {
                    System.Windows.Forms.CheckBox chk = ctrl as System.Windows.Forms.CheckBox;
                    if (chk.Checked && !string.IsNullOrEmpty("" + chk.Tag))
                    {
                        if (!conditions.Contains("" + chk.Tag))
                            conditions.Add("" + chk.Tag);
                    }
                }
            }

            // 檢查使用者是否有勾選，至少有勾一項才執行。
            if (conditions.Count > 0)
            {

                Config.rpt_isCheckSemesterDomain = IsShowSemesterDomainSheet(); // 小郭, 2013/12/25
                Config.rpt_isCheckGraduateDomain = IsShowGraduateDomainSheet(); // 小郭, 2013/12/25

                IEvaluateFactory factory = new ConditionalEvaluateFactory(conditions);
                Graduation.Instance.SetFactory(factory);
                Graduation.Instance.Refresh();

                if (!_historyWorker.IsBusy)
                {
                    btnPrint.Enabled = false;
                    _historyWorker.RunWorkerAsync(_students, _errorList);
                }
            }
            else
            {
                FISCA.Presentation.Controls.MsgBox.Show("請勾選檢查條件。");
                return;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MergeResult(Dictionary<string, bool> passList, EvaluationResult sourceResult, EvaluationResult targetResult)
        {
            foreach (string student_id in sourceResult.Keys)
            {
                if (passList.ContainsKey(student_id) && passList[student_id]) continue;
                targetResult.MergeResults(student_id, sourceResult[student_id]);
            }
        }

        private void GraduationPredictReportForm_Load(object sender, EventArgs e)
        {

            int schoolYear = int.Parse(School.DefaultSchoolYear);
            int semester = int.Parse(School.DefaultSemester);

            iptSchoolYear.Value = schoolYear;
            iptSemester.Value = semester;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Object each in gpScore.Controls)
            {
                if (each is System.Windows.Forms.CheckBox)
                {
                    System.Windows.Forms.CheckBox cb = each as System.Windows.Forms.CheckBox;
                    if (cb.Text.Contains("領域"))
                    {
                        cb.Checked = checkBox1.Checked;
                    }
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Object each in gpDaily.Controls)
            {
                if (each is System.Windows.Forms.CheckBox)
                {
                    System.Windows.Forms.CheckBox cb = each as System.Windows.Forms.CheckBox;
                    if (cb.Text.Contains("各學期"))
                    {
                        cb.Checked = checkBox2.Checked;
                    }
                }
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Object each in gpDaily.Controls)
            {
                if (each is System.Windows.Forms.CheckBox)
                {
                    System.Windows.Forms.CheckBox cb = each as System.Windows.Forms.CheckBox;
                    if (cb.Text.Contains("第六學期"))
                    {
                        cb.Checked = checkBox3.Checked;
                    }
                }
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Object each in gpDaily.Controls)
            {
                if (each is System.Windows.Forms.CheckBox)
                {
                    System.Windows.Forms.CheckBox cb = each as System.Windows.Forms.CheckBox;
                    if (cb.Text.Contains("所有學期"))
                    {
                        cb.Checked = checkBox4.Checked;
                    }
                }
            }
        }

        private void ExportDoctSetup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 樣板與列印設定
            PrintConfigForm pcf = new PrintConfigForm();
            pcf.ShowDialog();
        }

        private void chConditionGr1_CheckedChanged(object sender, EventArgs e)
        {

        }

        #region 判斷工作表是否出現
        /// <summary>
        /// 是否顯示"未達畢業標準學生-依畢業總平均"的工作表, 小郭, 2013/12/25
        /// </summary>
        private bool IsShowGraduateDomainSheet()
        {
            // 假如有勾選"學習領域畢業總平均成績符合規範。"
            if (chConditionGr1.Checked && (!string.IsNullOrEmpty("" + chConditionGr1.Tag)))
                return true;

            if (chCondition4c.Checked && (!string.IsNullOrEmpty("" + chCondition4c.Tag)))
                return true;

            if (chCondition5c.Checked && (!string.IsNullOrEmpty("" + chCondition5c.Tag)))
                return true;

            return false;
        }

        /// <summary>
        /// 是否顯示"未達畢業標準學生"的工作表, 小郭, 2013/12/25
        /// </summary>
        private bool IsShowSemesterDomainSheet()
        {
            // 假如有勾選"各學期領域成績均符合規範。"
            if (chCondition1.Checked && (!string.IsNullOrEmpty("" + chCondition1.Tag)))
                return true;
            // 假如有勾選"第六學期各領域成績符合規範。"
            if (chCondition2.Checked && (!string.IsNullOrEmpty("" + chCondition2.Tag)))
                return true;

            // 只要"日常生活表現"其中一項有勾選就顯示
            foreach (Control ctrl in gpDaily.Controls)
            {
                if (ctrl is System.Windows.Forms.CheckBox)
                {
                    System.Windows.Forms.CheckBox chk = ctrl as System.Windows.Forms.CheckBox;
                    if (chk.Checked && !string.IsNullOrEmpty("" + chk.Tag))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion

        /// <summary>
        /// 產生"學習領域畢業總平均明細"工作表, 小郭, 2013/12/30
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="studList"></param>
        private void OutStudentDomainData(Workbook wb, List<StudentRecord> studList)
        {
            Worksheet sheet = wb.Worksheets[wb.Worksheets.Add()];
            sheet.Name = "學習領域畢業總平均明細";
            int rowIndex = 0;
            int columnIndex = 0;

            Cells cells = sheet.Cells;

            #region 設定Style
            Style styleNormal = wb.Styles[wb.Styles.Add()];
            //Setting the line style of the top border
            styleNormal.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            //Setting the color of the top border
            styleNormal.Borders[BorderType.TopBorder].Color = Color.Black;
            //Setting the line style of the bottom border
            styleNormal.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            //Setting the color of the bottom border
            styleNormal.Borders[BorderType.BottomBorder].Color = Color.Black;
            //Setting the line style of the left border
            styleNormal.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            //Setting the color of the left border
            styleNormal.Borders[BorderType.LeftBorder].Color = Color.Black;
            //Setting the line style of the right border
            styleNormal.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            //Setting the color of the right border
            styleNormal.Borders[BorderType.RightBorder].Color = Color.Black;

            Style styleRed = wb.Styles[wb.Styles.Add()];
            styleRed.Copy(styleNormal);
            styleRed.Font.Color = Color.Red;
            #endregion 設定Style

            #region 輸出標題
            SetStudentDomainCell(cells, rowIndex, columnIndex++, "班級", styleNormal);
            SetStudentDomainCell(cells, rowIndex, columnIndex++, "座號", styleNormal);
            SetStudentDomainCell(cells, rowIndex, columnIndex++, "學號", styleNormal);
            SetStudentDomainCell(cells, rowIndex, columnIndex++, "姓名", styleNormal);
            // 領域名稱
            foreach (string domainName in StudentDomainResult._DomainNameList)
            {
                SetStudentDomainCell(cells, rowIndex, columnIndex++, domainName, styleNormal);
            }
            SetStudentDomainCell(cells, rowIndex, columnIndex++, "不及格數", styleNormal);

            // 當有國語文、英語
            if (TempData.tmpStudDomainCreditDict.Count > 0)
            {
                SetStudentDomainCell(cells, rowIndex, columnIndex++, "語文(國語文)", styleNormal);
                SetStudentDomainCell(cells, rowIndex, columnIndex++, "語文(英語)", styleNormal);
            }

            #endregion 輸出標題

            #region 輸出平均明細
            foreach (StudentRecord student in studList)
            {
                rowIndex++;
                columnIndex = 0;
                string className = student.Class != null ? student.Class.Name : "";
                SetStudentDomainCell(cells, rowIndex, columnIndex++, className, styleNormal);      // 班級
                SetStudentDomainCell(cells, rowIndex, columnIndex++, student.SeatNo, styleNormal);          // 座號
                SetStudentDomainCell(cells, rowIndex, columnIndex++, student.StudentNumber, styleNormal);   // 學號
                SetStudentDomainCell(cells, rowIndex, columnIndex++, student.Name, styleNormal);            // 姓名
                #region 輸出有資料的學生
                if (StudentDomainResult._DomainResult.ContainsKey(student.ID))
                {
                    int failCnt = 0;
                    foreach (string domainName in StudentDomainResult._DomainNameList)
                    {
                        if (StudentDomainResult._DomainResult[student.ID].ContainsKey(domainName))
                        {
                            if (StudentDomainResult._DomainResult[student.ID][domainName].isPass)
                            {
                                SetStudentDomainCell(cells, rowIndex, columnIndex++, StudentDomainResult._DomainResult[student.ID][domainName].domainScore, styleNormal);
                            }
                            else
                            {
                                SetStudentDomainCell(cells, rowIndex, columnIndex++, StudentDomainResult._DomainResult[student.ID][domainName].domainScore, styleRed);
                                failCnt++;
                            }
                        }
                        else
                            SetStudentDomainCell(cells, rowIndex, columnIndex++, "", styleNormal);
                    }
                    SetStudentDomainCell(cells, rowIndex, columnIndex++, "" + failCnt, styleNormal);

                    if (TempData.tmpStudDomainCreditDict.Count > 0)
                    {
                        if (TempData.tmpStudDomainCreditDict.ContainsKey(student.ID))
                        {
                            // 國語文
                            if (TempData.tmpStudDomainScoreDict[student.ID].ContainsKey("國語文"))
                            {
                                decimal sc = TempData.tmpStudDomainScoreDict[student.ID]["國語文"] / TempData.tmpStudDomainCreditDict[student.ID]["國語文"];
                                if (sc >= 60)
                                    SetStudentDomainCell(cells, rowIndex, columnIndex++, sc, styleNormal);
                                else
                                    SetStudentDomainCell(cells, rowIndex, columnIndex++, sc, styleRed);
                            }
                            else
                                SetStudentDomainCell(cells, rowIndex, columnIndex++, "", styleNormal);

                            // 英語
                            if (TempData.tmpStudDomainScoreDict[student.ID].ContainsKey("英語"))
                            {
                                decimal sc = 0;
                                if (TempData.tmpStudDomainScoreDict[student.ID].ContainsKey("英語"))
                                    sc = TempData.tmpStudDomainScoreDict[student.ID]["英語"] / TempData.tmpStudDomainCreditDict[student.ID]["英語"];

                                if (sc >= 60)
                                    SetStudentDomainCell(cells, rowIndex, columnIndex++, sc, styleNormal);
                                else
                                    SetStudentDomainCell(cells, rowIndex, columnIndex++, sc, styleRed);
                            }
                            else
                                SetStudentDomainCell(cells, rowIndex, columnIndex++, "", styleNormal);
                        }
                    }
                }
                #endregion 輸出有資料的學生
                else
                {
                    foreach (string domainName in StudentDomainResult._DomainNameList)
                    {
                        SetStudentDomainCell(cells, rowIndex, columnIndex++, "", styleNormal);
                    }
                    SetStudentDomainCell(cells, rowIndex, columnIndex++, "", styleNormal);

                    if (TempData.tmpStudDomainCreditDict.Count > 0)
                    {
                        SetStudentDomainCell(cells, rowIndex, columnIndex++, "", styleNormal);
                        SetStudentDomainCell(cells, rowIndex, columnIndex++, "", styleNormal);
                    }
                }
            }
            #endregion 輸出平均明細
        }

        /// <summary>
        /// 設定每一個欄位的內容
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="value"></param>
        /// <param name="style"></param>
        private void SetStudentDomainCell(Cells cells, int rowIndex, int columnIndex, string value, Style style)
        {
            cells[rowIndex, columnIndex].PutValue(value);
            cells[rowIndex, columnIndex].Style = style;
        }
        private void SetStudentDomainCell(Cells cells, int rowIndex, int columnIndex, decimal value, Style style)
        {
            cells[rowIndex, columnIndex].PutValue(String.Format("{0:0.##}", value));    // 小數點以下, 最多兩位
            cells[rowIndex, columnIndex].Style = style;
        }

        /// <summary>
        /// 產生學生獎懲明細工作表
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="studList"></param>
        private void ExportStudentDemeritAmountAllData(Workbook wb, List<StudentRecord> studList)
        {
            Worksheet sheet = wb.Worksheets[wb.Worksheets.Add()];
            sheet.Name = "各學期獎懲統計";
            int rowIndex = 0;
            int columnIndex = 0;

            Cells cells = sheet.Cells;

            #region 設定Style
            Style styleNormal = wb.Styles[wb.Styles.Add()];
            //Setting the line style of the top border
            styleNormal.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            //Setting the color of the top border
            styleNormal.Borders[BorderType.TopBorder].Color = Color.Black;
            //Setting the line style of the bottom border
            styleNormal.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            //Setting the color of the bottom border
            styleNormal.Borders[BorderType.BottomBorder].Color = Color.Black;
            //Setting the line style of the left border
            styleNormal.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            //Setting the color of the left border
            styleNormal.Borders[BorderType.LeftBorder].Color = Color.Black;
            //Setting the line style of the right border
            styleNormal.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            //Setting the color of the right border
            styleNormal.Borders[BorderType.RightBorder].Color = Color.Black;

            Style styleRed = wb.Styles[wb.Styles.Add()];
            styleRed.Copy(styleNormal);
            styleRed.Font.Color = Color.Red;
            #endregion 設定Style

            #region 輸出標題
            SetStudentDomainCell(cells, rowIndex, columnIndex++, "班級", styleNormal);
            SetStudentDomainCell(cells, rowIndex, columnIndex++, "座號", styleNormal);
            SetStudentDomainCell(cells, rowIndex, columnIndex++, "學號", styleNormal);
            SetStudentDomainCell(cells, rowIndex, columnIndex++, "姓名", styleNormal);
            List<string> scList = new List<string>();
            Dictionary<string, int> colDict = new Dictionary<string, int>();
            int colIdx = columnIndex;
            foreach (string sid in TempData.tmpStudentDemeritAmountAllDict.Keys)
            {
                // 學年度學期
                foreach (string key in TempData.tmpStudentDemeritAmountAllDict[sid].Keys)
                {
                    if (!scList.Contains(key))
                        scList.Add(key);
                }
            }

            scList.Sort();

            foreach (string key in scList)
            {
                colDict.Add(key, colIdx);
                colIdx++;
            }


            foreach (string key in colDict.Keys)
                SetStudentDomainCell(cells, rowIndex, colDict[key], key, styleNormal);

            #endregion

            #region 輸出明細
            foreach (StudentRecord student in studList)
            {
                rowIndex++;
                columnIndex = 0;
                string className = student.Class != null ? student.Class.Name : "";
                SetStudentDomainCell(cells, rowIndex, columnIndex++, className, styleNormal);      // 班級
                SetStudentDomainCell(cells, rowIndex, columnIndex++, student.SeatNo, styleNormal);          // 座號
                SetStudentDomainCell(cells, rowIndex, columnIndex++, student.StudentNumber, styleNormal);   // 學號
                SetStudentDomainCell(cells, rowIndex, columnIndex++, student.Name, styleNormal);            // 姓名

                #region 輸出有資料的學生
                if (TempData.tmpStudentDemeritAmountAllDict.ContainsKey(student.ID))
                {
                    List<string> detail = new List<string>();
                    foreach (string sc in TempData.tmpStudentDemeritAmountAllDict[student.ID].Keys)
                    {
                        int cot = 0;
                        // 獎懲明細
                        detail.Clear();
                        if (colDict.ContainsKey(sc))
                            cot = colDict[sc];

                        foreach (string ssc in TempData.tmpStudentDemeritAmountAllDict[student.ID][sc].Keys)
                            if (TempData.tmpStudentDemeritAmountAllDict[student.ID][sc][ssc] > 0)
                                detail.Add(ssc + ":" + TempData.tmpStudentDemeritAmountAllDict[student.ID][sc][ssc]);

                        string value = string.Join(",", detail.ToArray());
                        SetStudentDomainCell(cells, rowIndex, cot, value, styleNormal);
                    }


                }
                #endregion
            }
            #endregion
            sheet.AutoFitColumns();
        }

        private void ExportStudentAbsenceAmountAllData(Workbook wb, List<StudentRecord> studList)
        {
            Worksheet sheet = wb.Worksheets[wb.Worksheets.Add()];
            sheet.Name = "各學期缺曠統計";
            int rowIndex = 0;
            int columnIndex = 0;

            Cells cells = sheet.Cells;

            #region 設定Style
            Style styleNormal = wb.Styles[wb.Styles.Add()];
            //Setting the line style of the top border
            styleNormal.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            //Setting the color of the top border
            styleNormal.Borders[BorderType.TopBorder].Color = Color.Black;
            //Setting the line style of the bottom border
            styleNormal.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            //Setting the color of the bottom border
            styleNormal.Borders[BorderType.BottomBorder].Color = Color.Black;
            //Setting the line style of the left border
            styleNormal.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            //Setting the color of the left border
            styleNormal.Borders[BorderType.LeftBorder].Color = Color.Black;
            //Setting the line style of the right border
            styleNormal.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            //Setting the color of the right border
            styleNormal.Borders[BorderType.RightBorder].Color = Color.Black;

            Style styleRed = wb.Styles[wb.Styles.Add()];
            styleRed.Copy(styleNormal);
            styleRed.Font.Color = Color.Red;
            #endregion 設定Style

            #region 輸出標題
            SetStudentDomainCell(cells, rowIndex, columnIndex++, "班級", styleNormal);
            SetStudentDomainCell(cells, rowIndex, columnIndex++, "座號", styleNormal);
            SetStudentDomainCell(cells, rowIndex, columnIndex++, "學號", styleNormal);
            SetStudentDomainCell(cells, rowIndex, columnIndex++, "姓名", styleNormal);

            List<string> scList = new List<string>();
            Dictionary<string, int> colDict = new Dictionary<string, int>();
            int colIdx = columnIndex;
            foreach (string sid in TempData.tmpStudentAbsenceAmountAllDict.Keys)
            {
                // 學年度學期
                foreach (string key in TempData.tmpStudentAbsenceAmountAllDict[sid].Keys)
                {
                    if (!scList.Contains(key))
                        scList.Add(key);
                }
            }

            scList.Sort();

            foreach (string key in scList)
            {
                colDict.Add(key, colIdx);
                colIdx++;
            }


            foreach (string key in colDict.Keys)
                SetStudentDomainCell(cells, rowIndex, colDict[key], key, styleNormal);

            #endregion

            #region 輸出明細
            foreach (StudentRecord student in studList)
            {
                rowIndex++;
                columnIndex = 0;
                string className = student.Class != null ? student.Class.Name : "";
                SetStudentDomainCell(cells, rowIndex, columnIndex++, className, styleNormal);      // 班級
                SetStudentDomainCell(cells, rowIndex, columnIndex++, student.SeatNo, styleNormal);          // 座號
                SetStudentDomainCell(cells, rowIndex, columnIndex++, student.StudentNumber, styleNormal);   // 學號
                SetStudentDomainCell(cells, rowIndex, columnIndex++, student.Name, styleNormal);            // 姓名

                foreach (string key in colDict.Keys)
                    SetStudentDomainCell(cells, rowIndex, colDict[key], "", styleNormal);

                List<string> detail = new List<string>();
                #region 輸出有資料的學生
                if (TempData.tmpStudentAbsenceAmountAllDict.ContainsKey(student.ID))
                {
                    foreach (string key in TempData.tmpStudentAbsenceAmountAllDict[student.ID].Keys)
                    {
                        int col = 0;
                        if (colDict.ContainsKey(key))
                            col = colDict[key];

                        // 缺曠明細
                        detail.Clear();
                        foreach (string str in TempData.tmpStudentAbsenceAmountAllDict[student.ID][key].Keys)
                        {
                            string ss = str + ":" + TempData.tmpStudentAbsenceAmountAllDict[student.ID][key][str];
                            detail.Add(ss);
                        }

                        string value = string.Join(",", detail.ToArray());
                        SetStudentDomainCell(cells, rowIndex, col, value, styleNormal);
                    }
                }
                #endregion
            }
            #endregion
            sheet.AutoFitColumns();
        }
    }
}
