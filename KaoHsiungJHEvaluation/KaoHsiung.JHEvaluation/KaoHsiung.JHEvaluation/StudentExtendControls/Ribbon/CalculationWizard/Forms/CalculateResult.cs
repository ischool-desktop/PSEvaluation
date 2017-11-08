using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Framework;

namespace KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizard.Forms
{
    internal partial class CalculateResult : FISCA.Presentation.Controls.BaseForm
    {
        private CourseCollection _courses;

        public CalculateResult(CourseCollection courses)
        {
            InitializeComponent();

            _courses = courses;
            foreach (Course each in courses.Values)
            {
                ListViewItem item = new ListViewItem(each.CourseName);
                item.Tag = each;
                CourseList.Items.Add(item);
            }
        }

        private void CourseList_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem each in CourseList.Items)
                each.BackColor = Color.White;

            if (CourseList.FocusedItem != null)
            {
                CourseList.FocusedItem.BackColor = Color.LightBlue;

                Course course = CourseList.FocusedItem.Tag as Course;
                if (course != null)
                {
                    StudentList.Items.Clear();
                    List<ListViewItem> items = new List<ListViewItem>();
                    foreach (SCAttend each in course.SCAttends.Values)
                    {
                        ListViewItem item = new ListViewItem(each.ClassName);
                        item.SubItems.Add(each.StudentNumber);
                        item.SubItems.Add(each.StudentName);
                        item.SubItems.Add(each.PreviousScore == string.Empty ? "無" : each.PreviousScore);
                        System.Windows.Forms.ListViewItem.ListViewSubItem sub = item.SubItems.Add(each.Score);
                        sub.ForeColor = Color.Blue;
                        item.ToolTipText = GetToolTip(each);

                        items.Add(item);
                    }
                    StudentList.Items.AddRange(items.ToArray());
                }
            }
        }

        private string GetToolTip(SCAttend each)
        {
            StringBuilder result = new StringBuilder();
            bool result_contain = false;
            foreach (TEInclude include in each.Course.RefExams)
            {
                if (each.SCETakes.ContainsKey(include.ExamId))
                    result.AppendLine(include.ExamName + "(" + include.Weight + "%)" + "：" + each.SCETakes[include.ExamId].Score);
                else
                    result.AppendLine(include.ExamName + "(" + include.Weight + "%)" + "：無");

                result_contain = true;
            }

            if (result_contain)
                return result.ToString();
            else
                return "未設評量樣版。";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int count = 0;
            foreach (Course each in _courses.Values)
            {
                count += each.SCAttends.Count;
            }

            string message = string.Format("您確定要儲存計算結果？儲存時可能需要較長的時間。(成績筆數：{0})", count);
            DialogResult dr = MsgBox.Show(message, Application.ProductName, MessageBoxButtons.YesNo);

            if (dr == DialogResult.Yes)
            {
                CalculateProgress progress = new CalculateProgress(_courses);
                DialogResult dr1 = progress.ShowDialog();

                if (dr1 == DialogResult.OK)
                    MsgBox.Show(string.Format("儲存完成。", count), Application.ProductName);

                DialogResult = dr1;
            }
            else
                DialogResult = DialogResult.None;
        }

        private void btnClearData_Click(object sender, EventArgs e)
        {
            CalculateProgress progress = new CalculateProgress(_courses);
            progress.ClearData = true;
            progress.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //string message = "計算結果並未儲存，您確定要離開？";
            //DialogResult dr = MsgBox.Show(message, Application.ProductName, MessageBoxButtons.YesNo);
            //if (dr == DialogResult.No)
            //    DialogResult = DialogResult.None;
        }

        private void CalculateResult_DoubleClick(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift)
                btnClearData.Visible = true;
        }
    }
}