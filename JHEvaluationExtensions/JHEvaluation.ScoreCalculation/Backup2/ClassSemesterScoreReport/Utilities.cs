using System.Collections.Generic;
using System.Text;
using DevComponents.DotNetBar.Controls;
using K12.Data;
using JHSchool.Data;
using FISCA.Presentation.Controls;
using System.Windows.Forms;
using System;
using Campus.Rating;
using Aspose.Cells;
using JHSchool.Evaluation;

namespace JHEvaluation.ClassSemesterScoreReport
{
    internal static class Utilities
    {
        public const string SubjectToken = "Subject";
        public const string DomainToken = "Domain";
        public const string SummaryToken = "Summary";

        /// <summary>
        /// Network Access.
        /// </summary>
        /// <param name="ctlExam"></param>
        public static void SetExamDefaultItems(ComboBoxEx ctlExam)
        {
            try
            {
                ctlExam.DisplayMember = "Name";
                ctlExam.ValueMember = "ID";

                List<JHExamRecord> exams = JHExam.SelectAll();
                exams.Sort(delegate(JHExamRecord x, JHExamRecord y)
                {
                    int xx = x.DisplayOrder.HasValue ? x.DisplayOrder.Value : int.MinValue;
                    int yy = y.DisplayOrder.HasValue ? y.DisplayOrder.Value : int.MinValue;

                    return xx.CompareTo(yy);
                });

                ctlExam.Items.AddRange(exams.ToArray());
            }
            catch (Exception)
            {
                MsgBox.Show("讀取評量資訊錯誤，請確定網路連線正常後再試一次。");
            }
        }

        public static void SetSemesterDefaultItems(ComboBoxEx ctlSchoolYear, ComboBoxEx ctlSemester)
        {
            try
            {
                ctlSchoolYear.Items.Clear();
                ctlSemester.Items.Clear();

                int schoolyear = int.Parse(School.DefaultSchoolYear);

                for (int i = -3; i < 3; i++)
                    ctlSchoolYear.Items.Add(schoolyear + i);

                ctlSemester.Items.Add("1");
                ctlSemester.Items.Add("2");

                ctlSchoolYear.Text = School.DefaultSchoolYear;
                ctlSemester.Text = School.DefaultSemester;
            }
            catch (Exception)
            {
                MsgBox.Show("讀取學期資訊錯誤，請確定網路連線正常後再試一次。");
            }
        }

        public static void DisableControls(Control topControl)
        {
            ChangeControlsStatus(topControl, false);
        }

        public static void EnableControls(Control topControl)
        {
            ChangeControlsStatus(topControl, true);
        }

        private static void ChangeControlsStatus(Control topControl, bool status)
        {
            foreach (Control each in topControl.Controls)
            {
                string tag = each.Tag + "";
                if (tag.ToUpper() == "StatusVarying".ToUpper())
                {
                    each.Enabled = status;
                }

                if (each.Controls.Count > 0)
                    ChangeControlsStatus(each, status);
            }
        }

        public static List<string> GetSelectedItems(this ListViewEx listView)
        {
            List<string> items = new List<string>();
            foreach (ListViewItem each in listView.Items)
            {
                if (each.Checked)
                    items.Add(each.Text);
            }

            return items;
        }

        public static Dictionary<string, ReportStudent> ToDictionary(this IEnumerable<ReportStudent> students)
        {
            Dictionary<string, ReportStudent> dicstuds = new Dictionary<string, ReportStudent>();
            foreach (ReportStudent each in students)
                dicstuds.Add(each.Id, each);
            return dicstuds;
        }

        public static List<ReportStudent> ToReportStudent(this IEnumerable<JHStudentRecord> srcStudents)
        {
            List<ReportStudent> students = new List<ReportStudent>();
            foreach (JHStudentRecord each in srcStudents)
            {
                if (each.Status == StudentRecord.StudentStatus.一般 ||
                    each.Status == StudentRecord.StudentStatus.輟學)
                    students.Add(new ReportStudent(each));
            }
            return students;
        }

        public static List<string> ToKeys(this IEnumerable<ReportStudent> students)
        {
            List<string> keys = new List<string>();
            foreach (ReportStudent each in students)
                keys.Add(each.Id);

            return keys;
        }

        public static List<string> ToKeys(this IEnumerable<JHClassRecord> classes)
        {
            List<string> keys = new List<string>();
            foreach (JHClassRecord each in classes)
                keys.Add(each.ID);

            return keys;
        }

        public static void FillItems(this ListViewEx listView, IEnumerable<string> items, string groupName)
        {
            listView.Groups.Clear();
            listView.Items.Clear();

            ListViewGroup group = new ListViewGroup(groupName);
            listView.Groups.Add(group);
            listView.SuspendLayout();
            foreach (string each in items)
            {
                ListViewItem item = new ListViewItem();
                item.Group = group;
                item.Text = each;
                listView.Items.Add(item);
            }
            listView.ResumeLayout();
            listView.Refresh();
        }

        public static List<RatingScope<ReportStudent>> SortName(this List<RatingScope<ReportStudent>> scopes)
        {
            scopes.Sort(new Comparison<RatingScope<ReportStudent>>(delegate(RatingScope<ReportStudent> x, RatingScope<ReportStudent> y)
            {
                return x.Name.CompareTo(y.Name);
            }));

            return scopes;
        }

        public static List<RatingScope<ReportStudent>> ToClassScopes(this IEnumerable<ReportStudent> students)
        {
            Dictionary<string, RatingScope<ReportStudent>> scopes = new Dictionary<string, RatingScope<ReportStudent>>();

            foreach (ReportStudent each in students)
            {
                string className = string.Empty;

                if (!string.IsNullOrEmpty(each.RefClassID))
                    className = JHClass.SelectByID(each.RefClassID).Name;

                if (!scopes.ContainsKey(className))
                    scopes.Add(className, new RatingScope<ReportStudent>(className, "班排名"));

                scopes[className].Add(each);
            }

            return new List<RatingScope<ReportStudent>>(scopes.Values).SortName();
        }

        public static List<RatingScope<ReportStudent>> ToGradeYearScopes(this IEnumerable<ReportStudent> students)
        {
            Dictionary<string, RatingScope<ReportStudent>> scopes = new Dictionary<string, RatingScope<ReportStudent>>();

            foreach (ReportStudent each in students)
            {
                string gradeYear = string.Empty;

                if (!string.IsNullOrEmpty(each.RefClassID))
                {
                    int? gy = JHClass.SelectByID(each.RefClassID).GradeYear;
                    if (gy.HasValue) gradeYear = gy.Value.ToString();
                }

                if (!scopes.ContainsKey(gradeYear))
                    scopes.Add(gradeYear, new RatingScope<ReportStudent>(gradeYear, "年排名"));

                scopes[gradeYear].Add(each);
            }

            return new List<RatingScope<ReportStudent>>(scopes.Values).SortName();
        }

        public static List<string> DomainNames
        {
            get
            {
                List<string> domains = new List<string>();

                foreach (string domain in JHSchool.Evaluation.Domain.SelectGeneral())
                    domains.Add(domain);
                foreach (string domain in JHSchool.Evaluation.Domain.SelectSpecial())
                    domains.Add(domain);

                //domains.Add("語文");
                //domains.Add("數學");
                //domains.Add("社會");
                //domains.Add("自然與生活科技");
                //domains.Add("藝術與人文");
                //domains.Add("健康與體育");
                //domains.Add("綜合活動");
                //domains.Add("學習領域");
                //domains.Add("課程學習");

                domains.Sort(Subject.CompareDomainOrdinal);

                return domains;
            }
        }

        public static void Save(Workbook book, string fileName)
        {
            SaveFileDialog sdf = new SaveFileDialog();
            sdf.FileName = fileName;
            sdf.Filter = "Excel檔案(*.xls)|*.xls";

            if (sdf.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    book.Save(sdf.FileName, FileFormatType.Excel2003);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存失敗。" + ex.Message);
                    return;
                }

                try
                {
                    //if (MsgBox.Show("排名完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    //{
                    System.Diagnostics.Process.Start(sdf.FileName);
                    //}
                }
                catch (Exception ex)
                {
                    MsgBox.Show("開啟失敗。" + ex.Message);
                }
            }
        }

        public static int JHClassRecordComparison(JHClassRecord x, JHClassRecord y)
        {
            string xx = x.DisplayOrder.PadLeft(3, '0') + ":" + x.Name;
            string yy = y.DisplayOrder.PadLeft(3, '0') + ":" + y.Name;
            return xx.CompareTo(yy);
        }
    }
}
