//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Text;
//using System.Windows.Forms;
//using Framework;

//namespace JHSchool.Evaluation.CourseExtendControls.Ribbon
//{
//    [Obsolete("這是之前的課程成績輸入")]
//    public partial class EditCourseScore : FISCA.Presentation.Controls.BaseForm
//    {
//        private CourseRecord _course;

//        public EditCourseScore(CourseRecord course)
//        {
//            Font = Framework.DotNetBar.FontStyles.General;

//            InitializeComponent();

//            score.Font = Font;

//            _course = course;
//            score.LoadContent(_course.ID);

//            lblCourseName.Text = course.Name;
//            TeacherRecord record = course.GetFirstTeacher();
//            if (record != null)
//                lblCourseName.Text = course.Name + "(" + record.Name + ")";

//            Text = lblCourseName.Text;
//        }

//        private void btnSave_Click(object sender, EventArgs e)
//        {
//            if (!score.IsValid)
//            {
//                MsgBox.Show("輸入成績中有部份資料不符合規範，請修正後再行儲存");
//                return;
//            }

//            try
//            {
//                score.Save();
//                Hide();
//            }
//            catch (Exception ex)
//            {
//                //CurrentUser user = CurrentUser.Instance;
//                //BugReporter.ReportException(user.SystemName, user.SystemVersion, ex, false);

//                MsgBox.Show(ex.Message);
//            }
//        }

//        private CourseRecord Course
//        {
//            get { return _course; }
//        }

//        private static Dictionary<string, EditCourseScore> _editors;

//        static EditCourseScore()
//        {
//            _editors = new Dictionary<string, EditCourseScore>();
//        }

//        public static void DisplayCourseScore(CourseRecord course)
//        {
//            if (_editors.ContainsKey(course.ID))
//                _editors[course.ID].ShowDialog();
//            else
//            {
//                EditCourseScore editor = new EditCourseScore(course);
//                editor.FormClosed += new FormClosedEventHandler(Editor_FormClosed);
//                _editors.Add(course.ID, editor);
//                editor.ShowDialog();
//            }
//        }

//        private static void Editor_FormClosed(object sender, FormClosedEventArgs e)
//        {
//            EditCourseScore editor = sender as EditCourseScore;
//            editor.FormClosed -= new FormClosedEventHandler(Editor_FormClosed);

//            _editors.Remove(editor.Course.ID);
//        }

//        private void btnExit_Click(object sender, EventArgs e)
//        {
//            Close();
//        }
//    }
//}