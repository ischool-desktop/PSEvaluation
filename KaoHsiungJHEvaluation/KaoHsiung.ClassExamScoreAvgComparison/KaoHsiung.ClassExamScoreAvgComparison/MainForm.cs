using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using KaoHsiung.ClassExamScoreAvgComparison.Model;
using KaoHsiung.JHEvaluation.Data;
using JHSchool.Evaluation.Ranking;
using K12.Data;
using FISCA.Data;

namespace KaoHsiung.ClassExamScoreAvgComparison
{
    public partial class MainForm : BaseForm
    {
        private Config _config;
        private List<JHClassRecord> _classes;
        private List<ClassExamScoreData> _data;
        private BackgroundWorker _worker;
        private List<JHExamRecord> _exams;
        //private Dictionary<string, List<string>> _ecMapping;
        private List<string> _courseList;

        private Dictionary<string, JHCourseRecord> _courseDict;

        private int _runningSchoolYear;
        private int _runningSemester;

        ReportPreference config = null;

        private List<string> notRankStudentIDList;

        public static void Run()
        {
            new MainForm().ShowDialog();
        }

        public MainForm()
        {
            InitializeComponent();
            InitializeSemester();
            this.Text = Global.ReportName;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;

            _config = new Config(Global.ReportName);

            config = new ReportPreference();

            _data = new List<ClassExamScoreData>();
            _courseDict = new Dictionary<string, JHCourseRecord>();
            _exams = new List<JHExamRecord>();
            //_ecMapping = new Dictionary<string, List<string>>();
            _courseList = new List<string>();

            cbExam.DisplayMember = "Name";
            //cbScore.Items.Add("定期");
            //cbScore.Items.Add("定期加平時");
            cbExam.Items.Add("");
            _exams = JHExam.SelectAll();
            foreach (var exam in _exams)
                cbExam.Items.Add(exam);
            cbExam.SelectedIndex = 0;


            //填上學生類別清單

            List<string> StudTagItemList = GetStudentTagList();
            // 取得學生類別清單
            cbxNotRankTag.Items.Clear();
            cbxNotRankTag.Items.Add("");
            foreach (string item in StudTagItemList)
            {
                cbxNotRankTag.Items.Add(item);            
            }

            cbxNotRankTag.Text = config.NotRankTag;


            notRankStudentIDList = GetNotRankStudentIDList(cbxNotRankTag.Text);


            _worker = new BackgroundWorker();
            _worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                JHExamRecord exam = e.Argument as JHExamRecord;

                #region 取得課程
                _courseDict.Clear();
                List<JHCourseRecord> courseList = JHCourse.SelectBySchoolYearAndSemester(_runningSchoolYear, _runningSemester);
                List<string> courseIDs = new List<string>();
                foreach (JHCourseRecord course in courseList)
                {
                    courseIDs.Add(course.ID);
                    _courseDict.Add(course.ID, course);
                }
                #endregion

                #region 取得評量成績
                //StudentID -> ClassExamScoreData
                Dictionary<string, ClassExamScoreData> scMapping = new Dictionary<string, ClassExamScoreData>();
                List<string> ids = new List<string>();
                _classes = JHClass.SelectByIDs(K12.Presentation.NLDPanels.Class.SelectedSource);

                // 排序
                if (_classes.Count > 1)
                    _classes = DAL.DALTransfer.ClassRecordSortByDisplayOrder(_classes);

                // TODO: 這邊要排序
                //List<K12.Data.ClassRecord> c = new List<K12.Data.ClassRecord>(_classes);
                //c.Sort();
                //_classes = new List<JHClassRecord>(c);
                //((List<K12.Data.ClassRecord>)_classes).Sort();

                _data.Clear();
                foreach (JHClassRecord cla in _classes)
                {
                    ClassExamScoreData classData = new ClassExamScoreData(cla, notRankStudentIDList);
                    foreach (JHStudentRecord stu in classData.Students)
                        scMapping.Add(stu.ID, classData);
                    _data.Add(classData);
                }

                _courseList.Clear();
                if (courseIDs.Count > 0)
                {
                    // TODO: JHSCETake 需要提供 SelectBy 課程IDs and 試別IDs 嗎？
                    foreach (JHSCETakeRecord sce in JHSCETake.SelectByCourseAndExam(courseIDs, exam.ID))
                    {
                        // TODO: 下面前兩個判斷應該可以拿掉
                        //if (!examIDs.Contains(sce.RefExamID)) continue; //試別無效
                        //if (!courseIDs.Contains(sce.RefCourseID)) continue; //課程無效
                        if (!scMapping.ContainsKey(sce.RefStudentID)) continue; //學生編號無效
                        if (string.IsNullOrEmpty(_courseDict[sce.RefCourseID].RefAssessmentSetupID)) continue; //課程無評量設定

                        if (!_courseList.Contains(sce.RefCourseID))
                            _courseList.Add(sce.RefCourseID);
                        //if (!_ecMapping[sce.RefExamID].Contains(sce.RefCourseID))
                        //    _ecMapping[sce.RefExamID].Add(sce.RefCourseID);

                        ClassExamScoreData classData = scMapping[sce.RefStudentID];
                        classData.AddScore(sce);
                    }
                }
                #endregion
            };
            _worker.RunWorkerCompleted += delegate
            {
                string running = _runningSchoolYear + "_" + _runningSemester;
                string current = (int)cboSchoolYear.SelectedItem + "_" + (int)cboSemester.SelectedItem;
                if (running != current)
                {
                    if (!_worker.IsBusy)
                    {
                        _runningSchoolYear = (int)cboSchoolYear.SelectedItem;
                        _runningSemester = (int)cboSemester.SelectedItem;
                        RunWorker();
                    }
                }
                else
                {
                    FillData();
                    ControlEnabled = true;
                }
            };
            RunWorker();
        }

        private void RunWorker()
        {
            if (cbExam.SelectedItem != null && cbExam.SelectedItem is JHExamRecord)
            {
                ControlEnabled = false;
                _worker.RunWorkerAsync(cbExam.SelectedItem as JHExamRecord);
            }
            else
            {
                lvSubject.Items.Clear();
                lvDomain.Items.Clear();

                ControlEnabled = true;
                btnPrint.Enabled = false;
            }
        }

        private void InitializeSemester()
        {
            try
            {
                cboSchoolYear.SuspendLayout();
                cboSemester.SuspendLayout();
                int schoolYear = int.Parse(School.DefaultSchoolYear);
                int semester = int.Parse(School.DefaultSemester);
                for (int i = -2; i <= 2; i++)
                    cboSchoolYear.Items.Add(schoolYear + i);
                cboSemester.Items.Add(1);
                cboSemester.Items.Add(2);

                cboSchoolYear.SelectedIndex = 2;
                cboSemester.SelectedIndex = semester - 1;
                cboSchoolYear.ResumeLayout();
                cboSemester.ResumeLayout();

                _runningSchoolYear = (int)cboSchoolYear.SelectedItem;
                _runningSemester = (int)cboSemester.SelectedItem;

                cboSchoolYear.SelectedIndexChanged += new EventHandler(Semester_SelectedIndexChanged);
                cboSemester.SelectedIndexChanged += new EventHandler(Semester_SelectedIndexChanged);
            }
            catch (Exception ex)
            {

            }
        }

        private void Semester_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_worker.IsBusy)
            {
                _runningSchoolYear = (int)cboSchoolYear.SelectedItem;
                _runningSemester = (int)cboSemester.SelectedItem;
                RunWorker();
            }
        }

        private bool ControlEnabled
        {
            set
            {
                cboSchoolYear.Enabled = value;
                cboSemester.Enabled = value;
                cbExam.Enabled = value;
                btnPrint.Enabled = value;
                pictureBox1.Visible = !value;
                pictureBox2.Visible = !value;
                cbxNotRankTag.Enabled = value;
            }
        }

        private void lnConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _config.Load();
            new ConfigForm(_config).ShowDialog();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            #region 進行驗證
            if (!ValidItem())
            {
                MsgBox.Show("請選擇要列印的" + gpSubject.Text + "或" + gpDomain.Text);
                return;
            }
            #endregion

            Report._UserSelectCount = 0;

            Global.UserSelectSchoolYear = cboSchoolYear.Text;
            Global.UserSelectSemester = cboSemester.Text;

            #region 取得使用者選取的課程(科目)編號
            List<string> courseIDs = new List<string>();
            foreach (ListViewItem item in lvSubject.Items)
            {
                if (item.Checked)
                {
                    Report._UserSelectCount++;
                    List<string> list = item.Tag as List<string>;
                    courseIDs.AddRange(list);
                }
            }
            #endregion

            #region 取得使用者選取的領域
            List<string> domains = new List<string>();
            foreach (ListViewItem item in lvDomain.Items)
            {
                if (item.Checked)
                {
                    Report._UserSelectCount++;
                    domains.Add(item.Text);
                }
            }
            #endregion

            btnPrint.Enabled = false;
            pictureBox1.Visible = true;
            pictureBox2.Visible = true;

            #region 轉換成 CourseScore、計算分數、排名
            JHExamRecord exam = cbExam.SelectedItem as JHExamRecord;
            //ScoreType type = GetScoreType();

            List<string> asIDs = new List<string>();
            Dictionary<string, string> asMapping = new Dictionary<string, string>();
            Dictionary<string, JHAEIncludeRecord> aeDict = new Dictionary<string, JHAEIncludeRecord>();
            foreach (string courseID in courseIDs)
            {
                string asID = _courseDict[courseID].RefAssessmentSetupID;
                asIDs.Add(asID);
                asMapping.Add(courseID, asID);
            }

            foreach (JHAEIncludeRecord record in JHAEInclude.SelectByAssessmentSetupIDs(asIDs))
            {
                if (record.RefExamID != exam.ID) continue;
                aeDict.Add(record.RefAssessmentSetupID, record);
            }

            //ComputeScore computer = new ComputeScore(_courseDict);
            //List<ComputeMethod> methods = GetMethods(_config);
            //RankMethod rankMethod = GetRankMethod(_config);

            Rank rank = new Rank();
            // TODO: 不確定排名是否接續
            rank.Sequence = false;

            foreach (var ced in _data)
            {
                //轉成 CourseScore
                ced.ConvertToCourseScores(courseIDs, exam.ID);
                //RankData rd = new RankData();

                foreach (string studentID in ced.Rows.Keys)
                {
                    StudentRow row = ced.Rows[studentID];

                    //計算單一評量成績
                    foreach (CourseScore courseScore in row.CourseScoreList)
                    {
                        string asID = asMapping[courseScore.CourseID];
                        if (aeDict.ContainsKey(asID))
                            courseScore.CalculateScore(new KH.JHAEIncludeRecord(aeDict[asID]));
                    }
                }

                //排序班級課程ID
                ced.SortCourseIDs(courseIDs);
            }
            #endregion



            #region 儲存不排名學生設定

            config.NotRankTag = cbxNotRankTag.Text;

            config.Save(); 

            #endregion


            #region 產生報表
            //Report report = new Report(_data, _courseDict, exam, methods);
            Report report = new Report(_data, _courseDict, exam, domains);
            report.GenerateCompleted += new EventHandler(report_GenerateCompleted);
            report.GenerateError += new EventHandler(report_GenerateError);
            report.Generate();
            #endregion
        }

        private void report_GenerateError(object sender, EventArgs e)
        {
            btnPrint.Enabled = true;
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
        }

        private void report_GenerateCompleted(object sender, EventArgs e)
        {
            btnPrint.Enabled = true;
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
        }

        private RankMethod GetRankMethod(Config _config)
        {
            if (_config.RankMethod == "加權平均") return RankMethod.加權平均;
            else if (_config.RankMethod == "加權總分") return RankMethod.加權總分;
            else if (_config.RankMethod == "合計總分") return RankMethod.合計總分;
            else if (_config.RankMethod == "算術平均") return RankMethod.算術平均;
            else
            {
                throw new Exception("無效的排名依據");
            }
        }

        private List<ComputeMethod> GetMethods(Config _config)
        {
            List<ComputeMethod> methods = new List<ComputeMethod>();
            foreach (string item in _config.PrintItems)
            {
                if (item == "加權平均") methods.Add(ComputeMethod.加權平均);
                else if (item == "加權總分") methods.Add(ComputeMethod.加權總分);
                else if (item == "合計總分") methods.Add(ComputeMethod.合計總分);
                else if (item == "算術平均") methods.Add(ComputeMethod.算術平均);
            }
            return methods;
        }

        /// <summary>
        /// 取得使用者選擇的分數類型
        /// </summary>
        /// <returns></returns>
        //private ScoreType GetScoreType()
        //{
        //    string type = "" + cbScore.SelectedItem;
        //    if (type == "定期") return ScoreType.定期;
        //    else if (type == "定期加平時") return ScoreType.定期加平時;
        //    else
        //    {
        //        throw new Exception("無效的分數類型");
        //    }
        //}

        /// <summary>
        /// 驗證，至少要選一個項目
        /// </summary>
        /// <returns></returns>
        private bool ValidItem()
        {
            foreach (ListViewItem item in lvSubject.Items)
            {
                if (item.Checked)
                    return true;
            }
            foreach (ListViewItem item in lvDomain.Items)
            {
                if (item.Checked)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 驗證設定
        /// </summary>
        /// <returns></returns>
        private bool ValidConfig()
        {
            return _config.HasValue;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FillData()
        {
            lvSubject.Items.Clear();
            lvDomain.Items.Clear();
            lvSubject.SuspendLayout();
            lvDomain.SuspendLayout();

            Dictionary<string, ListViewItem> subjectItems = new Dictionary<string, ListViewItem>();
            Dictionary<string, ListViewItem> domainItems = new Dictionary<string, ListViewItem>();

            //string id = (cb.SelectedItem as JHExamRecord).ID;
            foreach (string courseID in _courseList)
            {
                JHCourseRecord course = _courseDict[courseID];

                ListViewItem subject_item;
                if (!subjectItems.ContainsKey(course.Subject))
                {
                    subject_item = new ListViewItem(course.Subject);
                    List<string> ids = new List<string>();
                    subject_item.Tag = ids;
                    subjectItems.Add(course.Subject, subject_item);
                }
                subject_item = subjectItems[course.Subject];
                (subject_item.Tag as List<string>).Add(course.ID);

                if (!domainItems.ContainsKey(course.Domain))
                {
                    ListViewItem domain_item = new ListViewItem(course.Domain);
                    domainItems.Add(course.Domain, domain_item);
                }
            }
            List<ListViewItem> itemList = new List<ListViewItem>(subjectItems.Values);
            itemList.Sort(ItemSort);
            foreach (ListViewItem item in itemList)
            {
                (item.Tag as List<string>).Sort();
                lvSubject.Items.Add(item);
            }
            List<ListViewItem> itemList2 = new List<ListViewItem>(domainItems.Values);
            itemList2.Sort(ItemSort);
            foreach (ListViewItem item in itemList2)
                lvDomain.Items.Add(item);
            lvSubject.ResumeLayout();
            lvDomain.ResumeLayout();
        }

        private void cbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            RunWorker();
        }

        private int ItemSort(ListViewItem x, ListViewItem y)
        {
            return JHSchool.Evaluation.Subject.CompareSubjectOrdinal(x.Text, y.Text);

        }

        /// <summary>
        /// 取得學生清單，[]表示Prefix
        /// </summary>
        /// <returns></returns>
        public static List<string> GetStudentTagList()
        {
            List<string> retVal = new List<string>();
            QueryHelper qh = new QueryHelper();
            string query = "select  distinct tag.prefix,tag.name from tag where category='Student' order by tag.prefix,tag.name;";
            DataTable dt = qh.Select(query);
            foreach (DataRow dr in dt.Rows)
            {
                string prefix = "", name = "";
                if (dr["prefix"] != null)
                    prefix = dr["prefix"].ToString();

                name = dr["name"].ToString();

                if (string.IsNullOrEmpty(prefix))
                {
                    if (!retVal.Contains(name))
                        retVal.Add(name);
                }
                else
                {
                    prefix = "[" + prefix + "]";
                    if (!retVal.Contains(prefix))
                        retVal.Add(prefix);
                }
            }

            retVal.Sort();

            return retVal;
        }

        //取得不排名類別學生 ID List
        public static List<string> GetNotRankStudentIDList(string StudTag)
        {
            List<string> retVal = new List<string>();

            Dictionary<string, List<string>> mapDict = new Dictionary<string, List<string>>();
            QueryHelper qh = new QueryHelper();
            string query = @"select tag.prefix,tag.name,ref_student_id as sid from tag inner join tag_student on tag.id=tag_student.ref_tag_id  where category='Student' order by tag.prefix,tag.name";
            DataTable dt = qh.Select(query);
            foreach (DataRow dr in dt.Rows)
            {
                string prefix = "", name = "";
                if (dr["prefix"] != null)
                    prefix = dr["prefix"].ToString();

                name = dr["name"].ToString();
                string sid = dr["sid"].ToString();

                if (string.IsNullOrEmpty(prefix))
                {
                    if (!mapDict.ContainsKey(name))
                        mapDict.Add(name, new List<string>());

                    mapDict[name].Add(sid);
                }
                else
                {
                    prefix = "[" + prefix + "]";
                    if (!mapDict.ContainsKey(prefix))
                        mapDict.Add(prefix, new List<string>());

                    mapDict[prefix].Add(sid);
                }
            }

            if (mapDict.ContainsKey(StudTag))
                retVal = mapDict[StudTag];

            return retVal;
        }

        private void cbxNotRankTag_SelectedIndexChanged(object sender, EventArgs e)
        {
            notRankStudentIDList = GetNotRankStudentIDList(cbxNotRankTag.Text);
            RunWorker();
        }

    }
}
