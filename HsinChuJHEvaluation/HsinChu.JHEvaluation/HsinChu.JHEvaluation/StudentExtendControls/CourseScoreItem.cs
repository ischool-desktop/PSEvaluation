using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using JHSchool;
using JHSchool.Data;
using FISCA.Presentation.Controls;
using FCode = Framework.Security.FeatureCodeAttribute;
using HsinChu.JHEvaluation.StudentExtendControls.CourseScoreItemRelated;
using HsinChu.JHEvaluation.Utility;

namespace HsinChu.JHEvaluation.StudentExtendControls
{
    [FCode("JHSchool.Student.Detail0052", "修課及評量成績")]
    public partial class CourseScoreItem : DetailContent
    {
        private BackgroundWorker _worker;
        private Dictionary<SemesterInfo, List<ListViewItem>> _items;
        private bool _reseting = false;
        private string _RunningID;
        internal static Framework.Security.FeatureAce UserPermission;

        public CourseScoreItem()
        {
            InitializeComponent();

            UserPermission = Framework.User.Acl[FCode.GetCode(GetType())];

            btnAdd.Visible = UserPermission.Editable;
            btnModify.Visible = UserPermission.Editable;
            btnDelete.Visible = UserPermission.Editable;

            InitializeBackgroundWorker();
            _items = new Dictionary<SemesterInfo, List<ListViewItem>>();
            _RunningID = string.Empty;
        }

        private void InitializeBackgroundWorker()
        {
            _worker = new BackgroundWorker();
            _worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                JHStudentRecord student = JHStudent.SelectByID(PrimaryKey);

                List<string> courseIDs = new List<string>();
                foreach (JHSCAttendRecord record in JHSCAttend.SelectByStudentIDAndCourseID(new string[] { student.ID }, new string[] { }))
                {
                    if (!courseIDs.Contains(record.RefCourseID))
                        courseIDs.Add(record.RefCourseID);
                }
                List<JHCourseRecord> courses = JHCourse.SelectByIDs(courseIDs);
                courses.Sort(delegate(JHCourseRecord x, JHCourseRecord y)
                {
                    return x.Name.CompareTo(y.Name);
                });

                e.Result = courses;
            };
            _worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                if (_RunningID != PrimaryKey)
                {
                    _RunningID = PrimaryKey;
                    _worker.RunWorkerAsync(_RunningID);
                    return;
                }

                ResetListView();
                PrefillItems(e.Result as List<JHCourseRecord>);
            };
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            if (!_worker.IsBusy)
            {
                _RunningID = PrimaryKey;
                _worker.RunWorkerAsync();
            }
            base.OnPrimaryKeyChanged(e);
        }

        private void ResetListView()
        {
            _reseting = true;

            listView.SuspendLayout();
            listView.Items.Clear();
            listView.ResumeLayout();

            cboSchoolYear.SelectedItem = null;
            cboSemester.SelectedItem = null;

            lblCount.Text = string.Empty;

            _reseting = false;
        }

        private void PrefillItems(List<JHCourseRecord> courses)
        {
            List<int> schoolYearList = new List<int>();
            cboSchoolYear.Items.Clear();
            cboSemester.Items.Clear();

            _items.Clear();

            SemesterInfo last = new SemesterInfo();
            foreach (JHCourseRecord course in courses)
            {
                SemesterInfo sems = new SemesterInfo();
                if (course.SchoolYear.HasValue)
                    sems.SchoolYear = course.SchoolYear.Value;
                else
                    sems.SchoolYear = 0;

                if (course.Semester.HasValue)
                    sems.Semester = course.Semester.Value;
                else
                    sems.Semester = 0;

                ListViewItem item = new ListViewItem(new string[] { "" + sems.SchoolYear, "" + sems.Semester, course.Name, course.Subject });
                item.Tag = course;

                if (!_items.ContainsKey(sems))
                    _items.Add(sems, new List<ListViewItem>());
                _items[sems].Add(item);

                if (!schoolYearList.Contains(sems.SchoolYear))
                    schoolYearList.Add(sems.SchoolYear);

                if (sems > last) last = sems;
            }

            schoolYearList.Sort();
            cboSchoolYear.SuspendLayout();
            cboSemester.SuspendLayout();

            foreach (int sy in schoolYearList)
                cboSchoolYear.Items.Add(sy);
            cboSemester.Items.Add(1);
            cboSemester.Items.Add(2);

            cboSchoolYear.SelectedItem = last.SchoolYear;
            cboSemester.SelectedItem = last.Semester;

            cboSchoolYear.ResumeLayout();
            cboSemester.ResumeLayout();
        }

        private void Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_reseting) return;
            if (_worker.IsBusy) return;

            listView.Items.Clear();
            lblCount.Text = "";
            listView.SuspendLayout();

            if (cboSchoolYear.SelectedItem != null && cboSemester.SelectedItem != null)
            {
                int schoolYear = 0;
                int semester = 0;
                int.TryParse("" + cboSchoolYear.SelectedItem, out schoolYear);
                int.TryParse("" + cboSemester.SelectedItem, out semester);
                SemesterInfo sems = new SemesterInfo();
                sems.SchoolYear = schoolYear;
                sems.Semester = semester;

                if (_items.ContainsKey(sems))
                {
                    foreach (ListViewItem item in _items[sems])
                        listView.Items.Add(item);
                    lblCount.Text = "" + _items[sems].Count;
                }
            }
            listView.ResumeLayout();
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            OpenScoreInputForm();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            OpenScoreInputForm();
        }

        private void OpenScoreInputForm()
        {
            if (listView.SelectedItems.Count <= 0) return;

            ListViewItem item = listView.SelectedItems[0];
            JHCourseRecord course = item.Tag as JHCourseRecord;
            ScoreInputForm form = new ScoreInputForm(JHStudent.SelectByID(PrimaryKey), course);
            form.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count <= 0) return;

            ListViewItem item = listView.SelectedItems[0];
            JHCourseRecord course = item.Tag as JHCourseRecord;

            if (MsgBox.Show(string.Format("您確定要刪除課程「{0}」的修課記錄及相關評量成績嗎？", course.Name), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                JHStudentRecord student = JHStudent.SelectByID(PrimaryKey);

                List<JHSCETakeRecord> sces = JHSCETake.SelectByStudentAndCourse(student.ID, course.ID);
                if (sces.Count > 0)
                {
                    JHSCETake.Delete(sces);
                    FISCA.LogAgent.LogSaver logSaver = FISCA.LogAgent.ApplicationLog.CreateLogSaverInstance();
                    string studentInfo = StudentInfoConvertor.GetInfoWithClass(student);
                    foreach (var sce in sces)
                    {
                        string desc = studentInfo + " 刪除評量成績：" + sce.Course.Name + " " + sce.Exam.Name;
                        logSaver.AddBatch("成績系統.修課及評量成績", "刪除評量成績", "student", PrimaryKey, desc);
                    }
                    logSaver.LogBatch();
                }

                List<JHSCAttendRecord> scattends = JHSCAttend.SelectByStudentIDAndCourseID(new string[] { student.ID }, new string[] { course.ID });
                if (scattends.Count > 0)
                {
                    JHSCAttend.Delete(scattends);
                    StringBuilder builder = new StringBuilder("");
                    builder.Append(StudentInfoConvertor.GetInfoWithClass(student));
                    builder.Append(" 刪除修課：" + course.Name);
                    FISCA.LogAgent.ApplicationLog.Log("成績系統.修課及評量成績", "刪除修課", "student", PrimaryKey, builder.ToString());
                }

                listView.Items.Remove(item);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            JHStudentRecord student = JHStudent.SelectByID(PrimaryKey);
            CreateForm form = new CreateForm(student);
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (form.Course == null) return;

                try
                {
                    JHSCAttendRecord scattend = new JHSCAttendRecord();
                    scattend.RefCourseID = form.Course.ID;
                    scattend.RefStudentID = PrimaryKey;
                    JHSCAttend.Insert(scattend);
                    StringBuilder builder = new StringBuilder("");
                    builder.Append(StudentInfoConvertor.GetInfoWithClass(student));
                    builder.Append(" 加入修課：" + form.Course.Name);
                    FISCA.LogAgent.ApplicationLog.Log("成績系統.修課及評量成績", "新增修課", "student", PrimaryKey, builder.ToString());
                }
                catch (Exception ex)
                {
                    MsgBox.Show("新增修課記錄失敗。" + ex.Message);
                }

                ScoreInputForm inputform = new ScoreInputForm(student, form.Course);
                inputform.ShowDialog();

                if (!_worker.IsBusy)
                {
                    _RunningID = PrimaryKey;
                    _worker.RunWorkerAsync();
                }
            }
        }
    }
}
