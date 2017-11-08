using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using System.IO;
using K12.Data;
using KaoHsiung.ReaderScoreImport.Model;
using KaoHsiung.ReaderScoreImport.Validation;
using JHSchool.Data;
using KaoHsiung.JHEvaluation.Data;
using KaoHsiung.ReaderScoreImport.Validation.RecordValidators;

namespace KaoHsiung.ReaderScoreImport
{
    public partial class ImportStartupForm : BaseForm
    {
        private BackgroundWorker _worker;
        private BackgroundWorker _upload;
        private BackgroundWorker _warn;

        private int SchoolYear { get; set; }
        private int Semester { get; set; }

        private DataValidator<RawData> _rawDataValidator;
        private DataValidator<DataRecord> _dataRecordValidator;

        private List<FileInfo> _files;
        private List<JHSCETakeRecord> _addScoreList;
        private List<JHSCETakeRecord> _deleteScoreList;

        /// <summary>
        /// 儲存畫面上學號長度
        /// </summary>
        K12.Data.Configuration.ConfigData cd;

        private string _StudentNumberLenght = "國中匯入讀卡學號長度";
        private string _StudentNumberLenghtName = "StudentNumberLenght";

        private EffortMapper _effortMapper;
        double counter = 0; //上傳成績時，算筆數用的。

        /// <summary>
        /// 載入學號長度值
        /// </summary>
        private void LoadConfigData()
        {
            int val = 7;
            cd = School.Configuration[_StudentNumberLenght];
            Global.StudentNumberLenght = intStudentNumberLenght.Value;
            if (int.TryParse(cd[_StudentNumberLenghtName], out val))
                intStudentNumberLenght.Value = val;
        }


        /// <summary>
        /// 儲存學號長度值
        /// </summary>
        private void SaveConfigData()
        {
            Global.StudentNumberLenght = intStudentNumberLenght.Value;
            cd[_StudentNumberLenghtName] = intStudentNumberLenght.Value.ToString();
            cd.Save();
        }

        public ImportStartupForm()
        {
            InitializeComponent();
            InitializeSemesters();

            _effortMapper = new EffortMapper();

            // 載入預設儲存值
            LoadConfigData();

            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {
                lblMessage.Text = "" + e.UserState;
            };
            _worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {                
                #region Worker DoWork
                _worker.ReportProgress(0, "檢查讀卡文字格式…");

                #region 檢查文字檔
                ValidateTextFiles vtf = new ValidateTextFiles(intStudentNumberLenght.Value);
                ValidateTextResult vtResult = vtf.CheckFormat(_files);
                if (vtResult.Error)
                {
                    e.Result = vtResult;
                    return;
                }
                #endregion

                //文字檔轉 RawData
                RawDataCollection rdCollection = new RawDataCollection();
                rdCollection.ConvertFromFiles(_files);

                //RawData 轉 DataRecord
                DataRecordCollection drCollection = new DataRecordCollection();
                drCollection.ConvertFromRawData(rdCollection);

                _rawDataValidator = new DataValidator<RawData>();
                _dataRecordValidator = new DataValidator<DataRecord>();

                #region 取得驗證需要的資料
                JHCourse.RemoveAll();
                _worker.ReportProgress(0, "取得學生資料…");
                List<JHStudentRecord> studentList = GetInSchoolStudents();
                
                List<string> s_ids = new List<string>();
                Dictionary<string, List<string>> studentNumberToStudentIDs = new Dictionary<string, List<string>>();
                foreach (JHStudentRecord student in studentList)
                {
                    string sn = SCValidatorCreator.GetStudentNumberFormat(student.StudentNumber);
                    if (!studentNumberToStudentIDs.ContainsKey(sn))
                        studentNumberToStudentIDs.Add(sn, new List<string>());
                    studentNumberToStudentIDs[sn].Add(student.ID);
                }
                foreach (var dr in drCollection)
                {
                    if (studentNumberToStudentIDs.ContainsKey(dr.StudentNumber))
                        s_ids.AddRange(studentNumberToStudentIDs[dr.StudentNumber]);
                }

                studentList.Clear();

                _worker.ReportProgress(0, "取得課程資料…");
                List<JHCourseRecord> courseList = JHCourse.SelectBySchoolYearAndSemester(SchoolYear, Semester);
                List<JHAEIncludeRecord> aeList = JHAEInclude.SelectAll();

                //List<JHSCAttendRecord> scaList = JHSCAttend.SelectAll();
                var c_ids = from course in courseList select course.ID;
                _worker.ReportProgress(0, "取得修課資料…");
                //List<JHSCAttendRecord> scaList2 = JHSCAttend.SelectByStudentIDAndCourseID(s_ids, c_ids.ToList<string>());
                List<JHSCAttendRecord> scaList = new List<JHSCAttendRecord>();
                FunctionSpliter<string, JHSCAttendRecord> spliter = new FunctionSpliter<string, JHSCAttendRecord>(300, 3);
                spliter.Function = delegate(List<string> part)
                {
                    return JHSCAttend.Select(part, c_ids.ToList<string>(), null, SchoolYear.ToString(), Semester.ToString());
                };
                scaList = spliter.Execute(s_ids);
                
                _worker.ReportProgress(0, "取得試別資料…");
                List<JHExamRecord> examList = JHExam.SelectAll();
                #endregion

                #region 註冊驗證
                _worker.ReportProgress(0, "載入驗證規則…");
                _rawDataValidator.Register(new SubjectCodeValidator());
                _rawDataValidator.Register(new ClassCodeValidator());
                _rawDataValidator.Register(new ExamCodeValidator());

                SCValidatorCreator scCreator = new SCValidatorCreator(JHStudent.SelectByIDs(s_ids), courseList, scaList);
                _dataRecordValidator.Register(scCreator.CreateStudentValidator());
                _dataRecordValidator.Register(new ExamValidator(examList));
                _dataRecordValidator.Register(scCreator.CreateSCAttendValidator());
                _dataRecordValidator.Register(new CourseExamValidator(scCreator.StudentCourseInfo, aeList, examList));
                #endregion

                #region 進行驗證
                _worker.ReportProgress(0, "進行驗證中…");
                List<string> msgList = new List<string>();

                foreach (RawData rawData in rdCollection)
                {
                    List<string> msgs = _rawDataValidator.Validate(rawData);
                    msgList.AddRange(msgs);
                }
                if (msgList.Count > 0)
                {
                    e.Result = msgList;
                    return;
                }

                foreach (DataRecord dataRecord in drCollection)
                {
                    List<string> msgs = _dataRecordValidator.Validate(dataRecord);
                    msgList.AddRange(msgs);
                }
                if (msgList.Count > 0)
                {
                    e.Result = msgList;
                    return;
                }
                #endregion

                #region 取得學生的評量成績
                _deleteScoreList.Clear();
                _addScoreList.Clear();

                //var student_ids = from student in scCreator.StudentNumberDictionary.Values select student.ID;
                //List<string> course_ids = scCreator.AttendCourseIDs;

                var scaIDs = from sca in scaList select sca.ID;

                Dictionary<string, JHSCETakeRecord> sceList = new Dictionary<string, JHSCETakeRecord>();
                FunctionSpliter<string, JHSCETakeRecord> spliterSCE = new FunctionSpliter<string, JHSCETakeRecord>(300, 3);
                spliterSCE.Function = delegate(List<string> part)
                {
                    return JHSCETake.Select(null, null, null, null, part);
                };
                foreach (JHSCETakeRecord sce in spliterSCE.Execute(scaIDs.ToList()))
                {
                    string key = GetCombineKey(sce.RefStudentID, sce.RefCourseID, sce.RefExamID);
                    if (!sceList.ContainsKey(key))
                        sceList.Add(key, sce);
                }

                Dictionary<string, JHExamRecord> examTable = new Dictionary<string, JHExamRecord>();
                Dictionary<string, JHSCAttendRecord> scaTable = new Dictionary<string, JHSCAttendRecord>();

                foreach (JHExamRecord exam in examList)
                    if (!examTable.ContainsKey(exam.Name))
                        examTable.Add(exam.Name, exam);

                foreach (JHSCAttendRecord sca in scaList)
                {
                    string key = GetCombineKey(sca.RefStudentID, sca.RefCourseID);
                    if (!scaTable.ContainsKey(key))
                        scaTable.Add(key, sca);
                }

                foreach (DataRecord dr in drCollection)
                {
                    JHStudentRecord student = student = scCreator.StudentNumberDictionary[dr.StudentNumber];
                    JHExamRecord exam = examTable[dr.Exam];
                    List<JHCourseRecord> courses = new List<JHCourseRecord>();
                    foreach (JHCourseRecord course in scCreator.StudentCourseInfo.GetCourses(dr.StudentNumber))
                    {
                        if (dr.Subjects.Contains(course.Subject))
                            courses.Add(course);
                    }

                    foreach (JHCourseRecord course in courses)
                    {
                        string key = GetCombineKey(student.ID, course.ID, exam.ID);

                        if (sceList.ContainsKey(key))
                            _deleteScoreList.Add(sceList[key]);

                        JHSCETakeRecord jh = new JHSCETakeRecord();
                        KH.JHSCETakeRecord sceNew = new KH.JHSCETakeRecord(jh);
                        sceNew.RefCourseID = course.ID;
                        sceNew.RefExamID = exam.ID;
                        sceNew.RefSCAttendID = scaTable[GetCombineKey(student.ID, course.ID)].ID;
                        sceNew.RefStudentID = student.ID;
                        sceNew.Score = dr.Score;
                        sceNew.Effort = _effortMapper.GetCodeByScore(dr.Score);
                        _addScoreList.Add(sceNew.AsJHSCETakeRecord());
                    }
                }
                #endregion

                e.Result = null;
                #endregion
            };
            _worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                #region Worker Completed
                if (e.Error == null && e.Result == null)
                {
                    if (!_upload.IsBusy)
                    {
                        //如果學生身上已有成績，則提醒使用者
                        if (_deleteScoreList.Count > 0)
                        {
                            _warn.RunWorkerAsync();
                        }
                        else
                        {
                            lblMessage.Text = "成績上傳中…";
                            FISCA.Presentation.MotherForm.SetStatusBarMessage("成績上傳中…", 0);
                            counter = 0;
                            _upload.RunWorkerAsync();
                        }
                    }
                }
                else
                {
                    ControlEnable = true;

                    if (e.Error != null)
                    {
                        MsgBox.Show("匯入失敗。" + e.Error.Message);
                        SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);

                    }
                    else if (e.Result != null && e.Result is ValidateTextResult)
                    {
                        ValidateTextResult result = e.Result as ValidateTextResult;
                        ValidationErrorViewer viewer = new ValidationErrorViewer();
                        viewer.SetTextFileError(result.LineIndexes, result.ErrorFormatLineIndexes, result.DuplicateLineIndexes);
                        viewer.ShowDialog();
                    }
                    else if (e.Result != null && e.Result is List<string>)
                    {
                        ValidationErrorViewer viewer = new ValidationErrorViewer();
                        viewer.SetErrorLines(e.Result as List<string>);
                        viewer.ShowDialog();
                    }
                }
                #endregion
            };

            _upload = new BackgroundWorker();
            _upload.WorkerReportsProgress = true;
            _upload.ProgressChanged += new ProgressChangedEventHandler(_upload_ProgressChanged);
            //_upload.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            //{
            //    counter += double.Parse("" + e.ProgressPercentage);
            //    FISCA.Presentation.MotherForm.SetStatusBarMessage("成績上傳中…", (int)(counter * 100f / (double)_addScoreList.Count));
            //};
            _upload.DoWork += new DoWorkEventHandler(_upload_DoWork);

            //_upload.DoWork += delegate
            //{
                    

                //#region Upload DoWork
                //Framework.MultiThreadWorker<JHSCETakeRecord> multi = new Framework.MultiThreadWorker<JHSCETakeRecord>();
                //multi.MaxThreads = 3;
                //multi.PackageSize = 500;
                //multi.PackageWorker += delegate(object sender, Framework.PackageWorkEventArgs<JHSCETakeRecord> e)
                //{
                //    JHSCETake.Delete(e.List);
                //};
                //multi.Run(_deleteScoreList);

                //Framework.MultiThreadWorker<JHSCETakeRecord> multi2 = new Framework.MultiThreadWorker<JHSCETakeRecord>();
                //multi2.MaxThreads = 3;
                //multi2.PackageSize = 500;
                //multi2.PackageWorker += delegate(object sender, Framework.PackageWorkEventArgs<JHSCETakeRecord> e)
                //{
                //    JHSCETake.Insert(e.List);
                //    lock (_upload)
                //    {
                //        _upload.ReportProgress(e.List.Count);
                //    }
                //};
                //multi2.Run(_addScoreList);
                //#endregion
            //};


            _upload.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_upload_RunWorkerCompleted);

            _warn = new BackgroundWorker();
            _warn.WorkerReportsProgress = true;
            _warn.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                _warn.ReportProgress(0, "產生警告訊息...");

                Dictionary<string, string> examDict = new Dictionary<string, string>();
                foreach (JHExamRecord exam in JHExam.SelectAll())
                {
                    if (!examDict.ContainsKey(exam.ID))
                        examDict.Add(exam.ID, exam.Name);
                }

                WarningForm form = new WarningForm();
                int count = 0;
                foreach (JHSCETakeRecord sce in _deleteScoreList)
                {
                    // 當成績資料是空值跳過
                    if (sce.Score.HasValue == false && sce.Effort.HasValue == false && string.IsNullOrEmpty(sce.Text))
                        continue;
                    
                        count++;

                    JHStudentRecord student = JHStudent.SelectByID(sce.RefStudentID);
                    JHCourseRecord course = JHCourse.SelectByID(sce.RefCourseID);
                    string exam = (examDict.ContainsKey(sce.RefExamID) ? examDict[sce.RefExamID] : "<未知的試別>");

                    string s = "";
                    if (student.Class != null) s += student.Class.Name;
                    if (!string.IsNullOrEmpty("" + student.SeatNo)) s += " " + student.SeatNo + "號";
                    if (!string.IsNullOrEmpty(student.StudentNumber)) s += " (" + student.StudentNumber + ")";
                    s += " " + student.Name;

                    form.Add(student.ID, s, string.Format("學生在「{0}」課程「{1}」中已有成績。", course.Name, exam));
                    _warn.ReportProgress((int)(count * 100 / _deleteScoreList.Count), "產生警告訊息...");
                }

                e.Result = form;
            };
            _warn.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                WarningForm form = e.Result as WarningForm;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    lblMessage.Text = "成績上傳中…";
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("成績上傳中…", 0);
                    counter = 0;
                    _upload.RunWorkerAsync();
                }
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                }
            };
            _warn.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("" + e.UserState, e.ProgressPercentage);
            };

            _files = new List<FileInfo>();
            _addScoreList = new List<JHSCETakeRecord>();
            _deleteScoreList = new List<JHSCETakeRecord>();
        }

        void _upload_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("成績上傳中…", (int)(counter * 100f / (double)_addScoreList.Count));
        }

        // 上傳成績完成
        void _upload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                string msg = "";
                if (e.Result != null)
                    msg = e.Result.ToString();

                if (e.Cancelled)
                {
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("匯入失敗");
                    MsgBox.Show("匯入失敗" + msg);
                }
                else
                {
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("匯入完成");
                    MsgBox.Show("匯入完成,共匯入" + msg + "筆.");
                }
                ControlEnable = true;
            }
            catch (Exception ex)
            { 
                
            }
        }


        // 上傳
        void _upload_DoWork(object sender, DoWorkEventArgs e)
        {
                // 傳送與回傳筆數
                int SendCount = 0, RspCount = 0;
                // 刪除舊資料
                SendCount = _deleteScoreList.Count;

                // 取得 del id
                List<string> delIDList = _deleteScoreList.Select(x => x.ID).ToList();

                // 執行
                try
                {
                    JHSCETake.Delete(_deleteScoreList);
                }
                catch (Exception ex)
                {
                    e.Result = ex.Message;
                    e.Cancel = true;
                }
            //    RspCount = JHSCETake.SelectByIDs(delIDList).Count;

            //// 刪除未完成
            //    if (RspCount > 0)
            //        e.Cancel = true;

                try
                {

                    //新增資料，分筆上傳
                    Dictionary<int, List<JHSCETakeRecord>> batchDict = new Dictionary<int, List<JHSCETakeRecord>>();
                    int bn = 150;
                    int n1 = (int)(_addScoreList.Count / bn);

                    if ((_addScoreList.Count % bn) != 0)
                        n1++;

                    for (int i = 0; i <= n1; i++)
                        batchDict.Add(i, new List<JHSCETakeRecord>());


                    if (_addScoreList.Count > 0)
                    {
                        int idx = 0, count = 1;
                        // 分批
                        foreach (JHSCETakeRecord rec in _addScoreList)
                        {
                            // 100 分一批
                            if ((count % bn) == 0)
                                idx++;

                            batchDict[idx].Add(rec);
                            count++;
                        }
                    }


                    // 上傳資料
                    foreach (KeyValuePair<int, List<JHSCETakeRecord>> data in batchDict)
                    {
                        SendCount = 0; RspCount = 0;
                        if (data.Value.Count > 0)
                        {
                            SendCount = data.Value.Count;
                            try
                            {
                                JHSCETake.Insert(data.Value);
                            }
                            catch (Exception ex)
                            {
                                e.Cancel = true;
                                e.Result = ex.Message;
                            }

                            counter += SendCount;

                        }
                    }
                    e.Result = _addScoreList.Count;
                }
                catch (Exception ex)
                {
                    e.Result = ex.Message;
                    e.Cancel = true;
                
                }
        }
        
        private List<JHStudentRecord> GetInSchoolStudents()
        {
            List<JHStudentRecord> list = new List<JHStudentRecord>();
            foreach (JHStudentRecord student in JHStudent.SelectAll())
            {
                if (student.Status == StudentRecord.StudentStatus.一般 ||
                    student.Status == StudentRecord.StudentStatus.輟學)
                    list.Add(student);
            }
            return list;
        }

        private string GetCombineKey(string s1, string s2, string s3)
        {
            return s1 + "_" + s2 + "_" + s3;
        }

        private string GetCombineKey(string s1, string s2)
        {
            return s1 + "_" + s2;
        }

        private void InitializeSemesters()
        {
            try
            {
                for (int i = -2; i <= 2; i++)
                {
                    cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) + i);
                }
                cboSemester.Items.Add(1);
                cboSemester.Items.Add(2);

                cboSchoolYear.SelectedIndex = 2;
                cboSemester.SelectedIndex = int.Parse(School.DefaultSemester) - 1;
            }
            catch { }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Filter = "純文字文件(*.txt)|*.txt";
            ofd.Multiselect = true;
            ofd.Title = "開啟檔案";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _files.Clear();
                StringBuilder builder = new StringBuilder("");
                foreach (var file in ofd.FileNames)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    _files.Add(fileInfo);
                    builder.Append(fileInfo.Name + ", ");
                }
                string fileString = builder.ToString();
                if (fileString.EndsWith(", ")) fileString = fileString.Substring(0, fileString.Length - 2);
                txtFiles.Text = fileString;
            }
        }

        private bool ControlEnable
        {
            set
            {
                foreach (Control ctrl in this.Controls)
                    ctrl.Enabled = value;

                pic.Enabled = lblMessage.Enabled = !value;
                pic.Visible = lblMessage.Visible = !value;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (cboSchoolYear.SelectedItem == null) return;
            if (cboSemester.SelectedItem == null) return;
            if (_files.Count <= 0) return;

            ControlEnable = false;

            // 儲存設定值
            SaveConfigData();

            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }

        private void cboSchoolYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSchoolYear.SelectedItem != null)
                SchoolYear = (int)cboSchoolYear.SelectedItem;
        }

        private void cboSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSemester.SelectedItem != null)
                Semester = (int)cboSemester.SelectedItem;
        }

        private void ImportStartupForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("");
        }
    }
}
