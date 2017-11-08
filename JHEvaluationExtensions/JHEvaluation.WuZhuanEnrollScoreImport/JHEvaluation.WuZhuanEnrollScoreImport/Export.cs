using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using JHSchool.Data;
using JHSchool.Evaluation.Calculation;
using K12.Data;
using Aspose.Cells;
using System.IO;
using JHSchool.Evaluation.Mapping;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.Deployment;

namespace JHEvaluation.WuZhuanRegisterScoreImport
{
    internal class Export
    {
        public static void Run()
        {
            new Export();
        }

        /// <summary>
        /// 五學期
        /// </summary>
        private const int FiveSemester = 5;

        /// <summary>
        /// 分數模式
        /// </summary>
        private bool ScoreMode { get; set; }

        private BackgroundWorker _worker;
        private List<string> AllStudentID { get { return K12.Presentation.NLDPanels.Student.SelectedSource; } }

        public Export()
        {
            InitializeWorker();

            if (Control.ModifierKeys == Keys.Shift) ScoreMode = true;
            else ScoreMode = false;

            if (Global.Params["Mode"].ToUpper() == "HsinChu".ToUpper())
            {
                SubjectCEForm form = new SubjectCEForm("五專集體報名成績匯入檔", "國", "英");
                if (form.ShowDialog() == DialogResult.Cancel)
                    return;
                else
                {
                    Global.ChineseSubject = form.ChineseSubject;
                    Global.EnglishSubject = form.EnglishSubject;
                }
            }

            _worker.RunWorkerAsync();
        }

        /// <summary>
        /// 初始化 BackgroundWorker
        /// </summary>
        private void InitializeWorker()
        {
            _worker = new BackgroundWorker();
            _worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _worker.WorkerReportsProgress = true;
        }

        /// <summary>
        /// 取得學生學期歷程
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, HistoryUtil> GetStudentHistories()
        {
            Dictionary<string, HistoryUtil> utils = new Dictionary<string, HistoryUtil>();
            foreach (JHSemesterHistoryRecord history in JHSemesterHistory.SelectByStudentIDs(AllStudentID))
            {
                if (!utils.ContainsKey(history.RefStudentID))
                    utils.Add(history.RefStudentID, new HistoryUtil(history));
            }
            return utils;
        }

        /// <summary>
        /// 取得學生學期成績
        /// </summary>
        /// <param name="utilCache"></param>
        /// <returns></returns>
        private Dictionary<string, StudentScores> GetStudentScores(Dictionary<string, HistoryUtil> utilCache)
        {
            Dictionary<string, StudentScores> scores = new Dictionary<string, StudentScores>();
            foreach (JHSemesterScoreRecord scoreRecord in JHSemesterScore.SelectByStudentIDs(AllStudentID))
            {
                if (!scores.ContainsKey(scoreRecord.RefStudentID))
                {
                    StudentScores studentScores = new StudentScores(utilCache[scoreRecord.RefStudentID]);
                    scores.Add(scoreRecord.RefStudentID, studentScores);
                }
                scores[scoreRecord.RefStudentID].Add(scoreRecord);
            }
            return scores;
        }

        /// <summary>
        /// 取得學生聯絡資訊
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, ContactInfo> GetStudentContactInfos()
        {
            Dictionary<string, ContactInfo> contactCache = new Dictionary<string, ContactInfo>();
            foreach (JHPhoneRecord phone in JHPhone.SelectByStudentIDs(AllStudentID))
            {
                if (!contactCache.ContainsKey(phone.RefStudentID))
                {
                    ContactInfo info = new ContactInfo();
                    info.PhoneNumber = phone.Contact;
                    contactCache.Add(phone.RefStudentID, info);
                }
            }
            foreach (JHAddressRecord addr in JHAddress.SelectByStudentIDs(AllStudentID))
            {
                if (!contactCache.ContainsKey(addr.RefStudentID))
                {
                    ContactInfo info = new ContactInfo();
                    info.ZipCode = addr.PermanentZipCode;
                    info.Address = addr.Permanent.County + addr.Permanent.Town + addr.Permanent.District + addr.Permanent.Detail;
                    contactCache.Add(addr.RefStudentID, info);
                }
                else
                {
                    ContactInfo info = contactCache[addr.RefStudentID];
                    info.ZipCode = addr.PermanentZipCode;
                    info.Address = addr.Permanent.County + addr.Permanent.Town + addr.Permanent.District + addr.Permanent.Detail;
                }
            }
            return contactCache;
        }

        #region Worker Events
        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            double total = AllStudentID.Count;
            double count = 0;

            //學期歷程
            Dictionary<string, HistoryUtil> utilCache = GetStudentHistories();
            //學期成績
            Dictionary<string, StudentScores> scoreCache = GetStudentScores(utilCache);
            //聯絡資訊
            Dictionary<string, ContactInfo> contactCache = GetStudentContactInfos();

            //等第對照表
            DegreeMapper degreeMapper = new DegreeMapper();

            Workbook book = new Workbook();
            book.Open(new MemoryStream(Properties.Resources.五專集體報名成績匯入檔));
            Worksheet ws = book.Worksheets[0];

            int rowIndex = 2;

            foreach (JHStudentRecord stu in JHStudent.SelectByIDs(AllStudentID))
            {
                count++;

                int colIndex = 3; //前面三個欄位不用管，直接跳過。

                #region 基本資料
                ws.Cells[rowIndex, colIndex++].PutValue((stu.Class != null) ? stu.Class.Name : "");
                ws.Cells[rowIndex, colIndex++].PutValue(stu.StudentNumber);
                ws.Cells[rowIndex, colIndex++].PutValue(stu.Name);
                ws.Cells[rowIndex, colIndex++].PutValue(stu.IDNumber);
                ws.Cells[rowIndex, colIndex++].PutValue((stu.Gender == "男") ? "1" : "2");
                ws.Cells[rowIndex, colIndex++].PutValue(Global.GetFormatedBirthday(stu.Birthday));
                #endregion

                #region 聯絡資訊
                if (contactCache.ContainsKey(stu.ID))
                {
                    ContactInfo contact = contactCache[stu.ID];
                    ws.Cells[rowIndex, colIndex++].PutValue(contact.PhoneNumber);
                    ws.Cells[rowIndex, colIndex++].PutValue(contact.ZipCode);
                    ws.Cells[rowIndex, colIndex++].PutValue(contact.Address);
                }
                else
                    colIndex += 3; //沒有聯絡資訊直接跳過三個欄位
                #endregion

                #region 學期成績
                //有學期歷程及學期成績才會印
                if (utilCache.ContainsKey(stu.ID) && scoreCache.ContainsKey(stu.ID))
                {
                    #region 領域成績
                    int scoreStartColIndex = 12;
                    HistoryUtil util = utilCache[stu.ID];
                    StudentScores studentScore = scoreCache[stu.ID];

                    SemesterData indexSem = new SemesterData(0, 0, 2);
                    for (int i = 0; i < FiveSemester; i++)
                    {
                        colIndex = scoreStartColIndex + i * Global.GetDomains().Count;

                        indexSem = indexSem.NextSemester();
                        if (util.ExistByGradeYear(indexSem.GradeYear, indexSem.Semester) == false) continue;

                        foreach (decimal? score in studentScore.GetScoreList(util.SemesterData))
                        {
                            string value = score.HasValue ? degreeMapper.GetDegreeByScore(score.Value) : "";
                            if (ScoreMode) //如果是 ScoreMode，則印出分數
                                ws.Cells[rowIndex, colIndex++].PutValue("" + score);
                            else
                                ws.Cells[rowIndex, colIndex++].PutValue(value);

                        }
                    }
                    #endregion

                    #region 總平均
                    colIndex = scoreStartColIndex + FiveSemester * Global.GetDomains().Count;
                    decimal? averageScore = studentScore.GetAverage(util.AllSemesterData);
                    string averageValue = averageScore.HasValue ? degreeMapper.GetDegreeByScore(averageScore.Value) : "";
                    if (ScoreMode) //如果是 ScoreMode，則印出分數
                        ws.Cells[rowIndex, colIndex++].PutValue("" + averageScore);
                    else
                        ws.Cells[rowIndex, colIndex++].PutValue(averageValue);
                    #endregion
                }
                #endregion

                _worker.ReportProgress((int)(count * 100 / total));
                rowIndex++;
            }

            e.Result = book;
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("產生失敗");
                MsgBox.Show(e.Error.Message);
                return;
            }
            else
                FISCA.Presentation.MotherForm.SetStatusBarMessage("產生完成");

            Workbook book = e.Result as Workbook;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel(*.xls)|*.xls";
            sfd.FileName = "五專集體報名成績匯入檔";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    book.Save(sfd.FileName, FileFormatType.Excel2003);
                }
                catch (Exception ex)
                {
                    MsgBox.Show(ex.Message);
                }

                if (MsgBox.Show("是否立即開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                    catch (Exception ex2)
                    {
                        MsgBox.Show(ex2.Message);
                    }
                }
            }
        }

        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("產生五專集體報名成績匯入檔...", e.ProgressPercentage);
        }
        #endregion

        /// <summary>
        /// 學生各學期成績
        /// </summary>
        private class StudentScores
        {
            private Dictionary<SemesterData, JHSemesterScoreRecord> _scores;
            private HistoryUtil _historyUtil;

            public StudentScores(HistoryUtil historyUtil)
            {
                _scores = new Dictionary<SemesterData, JHSemesterScoreRecord>();
                _historyUtil = historyUtil;
            }

            /// <summary>
            /// 加入學期成績
            /// </summary>
            /// <param name="record"></param>
            public void Add(JHSemesterScoreRecord record)
            {
                if (_historyUtil.ExistBySchoolYear(record.SchoolYear, record.Semester) == false) return;

                if (!_scores.ContainsKey(_historyUtil.SemesterData))
                    _scores.Add(_historyUtil.SemesterData, record);
            }

            /// <summary>
            /// 取得各領域成績
            /// </summary>
            /// <param name="sem"></param>
            /// <returns></returns>
            public List<decimal?> GetScoreList(SemesterData sem)
            {
                if (_scores.ContainsKey(sem))
                {
                    List<decimal?> scoreList = new List<decimal?>();
                    foreach (string domain in Global.GetDomains())
                    {
                        decimal? score = null;
                        if (_scores[sem].Domains.ContainsKey(domain))
                        {
                            DomainScore domainScore = _scores[sem].Domains[domain];
                            score = domainScore.Score;
                        }
                        scoreList.Add(score);
                    }
                    return scoreList;
                }
                else
                    return new List<decimal?>();
            }

            /// <summary>
            /// 取得五學期領域成績平均
            /// </summary>
            /// <param name="sems"></param>
            /// <returns></returns>
            public decimal? GetAverage(SemesterDataCollection sems)
            {
                decimal sum, count;

                Dictionary<SemesterData, decimal?> semScores = new Dictionary<SemesterData, decimal?>();
                foreach (SemesterData sem in sems)
                {
                    sum = count = decimal.Zero;

                    if (!_scores.ContainsKey(sem)) continue;
                    foreach (string domain in Global.GetDomains())
                    {
                        if (!_scores[sem].Domains.ContainsKey(domain)) continue;

                        DomainScore domainScore = _scores[sem].Domains[domain];
                        if (!domainScore.Score.HasValue) continue;
                        if (!domainScore.Credit.HasValue) continue;
                        sum += domainScore.Score.Value * domainScore.Credit.Value;
                        count += domainScore.Credit.Value;
                    }
                    if (count > 0)
                        semScores.Add(sem, sum / count);
                    else
                        semScores.Add(sem, null);
                }

                sum = count = decimal.Zero;
                foreach (decimal? score in semScores.Values)
                {
                    if (!score.HasValue) continue;
                    sum += score.Value;
                    count++;
                }

                if (count > 0)
                    return sum / count;
                else
                    return null;
            }
        }

        /// <summary>
        /// 學生學期歷程
        /// </summary>
        private class HistoryUtil
        {
            private SemesterDataCollection _sems;

            public HistoryUtil(JHSemesterHistoryRecord historyRecord)
            {
                _sems = new SemesterDataCollection();

                foreach (SemesterHistoryItem item in historyRecord.SemesterHistoryItems)
                    _sems.Add(new SemesterData(item.GradeYear, item.SchoolYear, item.Semester));

                _sems = _sems.GetGradeYearSemester();

                SemesterData last = SemesterData.Empty;
                bool delete = false;
                foreach (SemesterData sem in _sems)
                {
                    if (sem.GradeYear == 3 && sem.Semester == 2)
                    {
                        last = sem;
                        delete = true;
                    }
                }
                if (delete)
                    _sems.Remove(last);
            }

            /// <summary>
            /// 取得所有學期資訊
            /// </summary>
            public SemesterDataCollection AllSemesterData { get { return _sems; } }

            /// <summary>
            /// 取得單一學期資訊，需先呼叫 ExistBy* function
            /// </summary>
            public SemesterData SemesterData { get; private set; }

            /// <summary>
            /// 檢驗學期資訊是否存在
            /// </summary>
            /// <param name="gradeYear">年級</param>
            /// <param name="semester">學期</param>
            /// <returns></returns>
            public bool ExistByGradeYear(int gradeYear, int semester)
            {
                foreach (var sem in _sems)
                {
                    if (sem.GradeYear == gradeYear && sem.Semester == semester)
                    {
                        this.SemesterData = sem;
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// 檢驗學期資訊是否存在
            /// </summary>
            /// <param name="schoolYear">學年度</param>
            /// <param name="semester">學期</param>
            /// <returns></returns>
            public bool ExistBySchoolYear(int schoolYear, int semester)
            {
                foreach (var sem in _sems)
                {
                    if (sem.SchoolYear == schoolYear && sem.Semester == semester)
                    {
                        this.SemesterData = sem;
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 學生聯絡資訊
        /// </summary>
        private class ContactInfo
        {
            public string PhoneNumber { get; set; }
            public string ZipCode { get; set; }
            public string Address { get; set; }
        }
    }

    internal class Global
    {
        public static string ChineseSubject { get; set; }
        public static string EnglishSubject { get; set; }

        /// <summary>
        /// 先這樣吧
        /// </summary>
        public static List<string> GetDomains()
        {
            if (string.IsNullOrEmpty(ChineseSubject)) ChineseSubject = "國語文";
            if (string.IsNullOrEmpty(EnglishSubject)) EnglishSubject = "英語";

            return new List<string>(new string[] {
                ChineseSubject,
                EnglishSubject,
                "數學",
                "自然與生活科技",
                "社會",
                "健康與體育",
                "藝術與人文",
                "綜合活動"
            });
        }

        /// <summary>
        /// 取得格式化後的生日(yyMMdd, yy為民國年)
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public static string GetFormatedBirthday(DateTime? birthday)
        {
            if (birthday.HasValue)
            {
                DateTime value = birthday.Value;
                return "" + (value.Year - 1911) + value.ToString("MMdd");
            }
            else
                return string.Empty;
        }

        internal static DeployParameters Params { get; set; }
    }
}
