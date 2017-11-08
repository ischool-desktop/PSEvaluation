using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Framework;
using JHSchool.Data;
using JHSchool;

namespace HsinChu.JHEvaluation.StudentExtendControls.CourseScoreItemRelated
{
    public partial class CreateForm : FISCA.Presentation.Controls.BaseForm
    {
        private JHStudentRecord _student;

        private JHCourseRecord _course;
        public JHCourseRecord Course
        {
            get { return _course; }
        }

        private Dictionary<SemesterInfo, List<JHCourseRecord>> _allcourses;

        public CreateForm(JHStudentRecord student)
        {
            InitializeComponent();
            InitializeCourses();

            _student = student;
        }

        private void InitializeCourses()
        {
            _allcourses = new Dictionary<SemesterInfo, List<JHSchool.Data.JHCourseRecord>>();
            List<int> schoolYearList = new List<int>();
            cboSchoolYear.Items.Clear();
            cboSemester.Items.Clear();

            SemesterInfo last = new SemesterInfo();
            foreach (JHCourseRecord course in JHCourse.SelectAll())
            {
                SemesterInfo info = new SemesterInfo();

                if (course.SchoolYear.HasValue)
                    info.SchoolYear = course.SchoolYear.Value;

                if (course.Semester.HasValue)
                    info.Semester = course.Semester.Value;

                //info.SchoolYear = (int)course.SchoolYear;
                //info.Semester = (int)course.Semester;
                

                if (!_allcourses.ContainsKey(info))
                    _allcourses.Add(info, new List<JHSchool.Data.JHCourseRecord>());
                _allcourses[info].Add(course);

                if (!schoolYearList.Contains(info.SchoolYear))
                    schoolYearList.Add(info.SchoolYear);

                if (info > last) last = info;
            }

            schoolYearList.Sort();
            foreach (int sy in schoolYearList)
                cboSchoolYear.Items.Add(sy);

            cboSemester.Items.Add(1);
            cboSemester.Items.Add(2);

            cboCourseName.DisplayMember = "Name";

            cboSchoolYear.SelectedItem = last.SchoolYear;
            cboSemester.SelectedItem = last.Semester;
        }

        private void cboSchoolYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSchoolYear.SelectedItem == null || cboSemester.SelectedItem == null) return;

            SemesterInfo info = new SemesterInfo();
            info.SchoolYear = Framework.Int.Parse("" + cboSchoolYear.SelectedItem);
            info.Semester = Framework.Int.Parse("" + cboSemester.SelectedItem);

            cboCourseName.Items.Clear();

            if (_allcourses.ContainsKey(info) && _allcourses[info].Count > 0)
            {
                _allcourses[info].Sort(delegate(JHCourseRecord x, JHCourseRecord y)
                {
                    return x.Name.CompareTo(y.Name);
                });

                cboCourseName.Items.AddRange(_allcourses[info].ToArray());
                cboCourseName.SelectedIndex = 0;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboCourseName.SelectedItem == null) return;

            _course = cboCourseName.SelectedItem as JHCourseRecord;

            foreach (JHSCAttendRecord record in JHSCAttend.SelectByStudentIDAndCourseID(new string[] { _student.ID }, new string[] { }))
            {
                if (record.RefCourseID == _course.ID)
                {
                    MsgBox.Show(string.Format("學生「{0}」已修習課程「{1}」", _student.Name, _course.Name));
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
