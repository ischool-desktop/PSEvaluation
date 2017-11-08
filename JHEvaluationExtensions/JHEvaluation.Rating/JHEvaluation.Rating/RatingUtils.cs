using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevComponents.DotNetBar.Controls;
using JHSchool.Data;
using System.Windows.Forms;
using Aspose.Cells;
using Campus.Rating;
using Campus.Windows;
using K12.Data;

namespace JHEvaluation.Rating
{
    internal static class RatingUtils
    {
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

                List<JHExamRecord> exams = JHSchool.Data.JHExam.SelectAll();
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

        public static string GetExamId(ComboBoxEx ctl)
        {
            if (ctl.InvokeRequired)
            {
                return ctl.Invoke(new Func<ComboBoxEx, string>(GetExamId), ctl).ToString();
            }
            else
            {
                JHExamRecord jh = (ctl.SelectedItem as JHExamRecord);

                if (jh == null)
                    return "";
                else
                    return jh.ID;
            }
        }

        public static int RatingStudentComparison(RatingStudent x, RatingStudent y)
        {
            string xx = x.GradeYear.PadLeft(3, '0') + ":" + x.ClassOrderString + ":" + x.SeatNo.PadLeft(3, '0');
            string yy = y.GradeYear.PadLeft(3, '0') + ":" + y.ClassOrderString + ":" + y.SeatNo.PadLeft(3, '0');
            return xx.CompareTo(yy);
        }

        public static List<RatingStudent> ToRatingStudent(IEnumerable<string> studentIDs)
        {
            List<RatingStudent> students = new List<RatingStudent>();
            foreach (JHStudentRecord each in JHStudent.SelectByIDs(studentIDs))
                students.Add(new RatingStudent(each));
            return students;
        }

        public static Dictionary<string, RatingStudent> ToDictionary(this IEnumerable<RatingStudent> students)
        {
            Dictionary<string, RatingStudent> dicstuds = new Dictionary<string, RatingStudent>();
            foreach (RatingStudent each in students)
                dicstuds.Add(each.Id, each);
            return dicstuds;
        }

        public static List<string> ToKeys(this IEnumerable<RatingStudent> students)
        {
            List<string> keys = new List<string>();
            foreach (RatingStudent each in students)
                keys.Add(each.Id);

            return keys;
        }

        public static List<string> ToKeys(this IEnumerable<JHCourseRecord> courses)
        {
            List<string> keys = new List<string>();
            foreach (JHCourseRecord each in courses)
                keys.Add(each.ID);

            return keys;
        }

        public static List<ScoreParser> ToScoreParsers(this IEnumerable<ScoreItem> scoreItems, string token)
        {
            List<ScoreParser> parsers = new List<ScoreParser>();
            foreach (ScoreItem each in scoreItems)
                parsers.Add(new ScoreParser(each, token));
            return parsers;
        }

        public static List<MultiScoreParser> ToMultiScoreParsers(this IEnumerable<ScoreItem> scoreItems, List<string> tokens)
        {
            List<MultiScoreParser> parsers = new List<MultiScoreParser>();
            foreach (ScoreItem each in scoreItems)
                parsers.Add(new MultiScoreParser(each, tokens));
            return parsers;
        }

        public static CalculationScoreParser ToCalcScoreParser(this ItemWeightCollection subjects, string name, CalculationScoreParser.CalcMethod calcMethod, int round, string token)
        {
            return new CalculationScoreParser(name, subjects, calcMethod, round, token);
        }

        public static List<RatingScope<RatingStudent>> ToClassScopes(this IEnumerable<RatingStudent> students)
        {
            Dictionary<string, RatingScope<RatingStudent>> scopes = new Dictionary<string, RatingScope<RatingStudent>>();

            foreach (RatingStudent each in students)
            {
                string className = string.Empty;

                if (!string.IsNullOrEmpty(each.RefClassID))
                    className = JHClass.SelectByID(each.RefClassID).Name;

                if (!scopes.ContainsKey(className))
                    scopes.Add(className, new RatingScope<RatingStudent>(className));

                scopes[className].Add(each);
            }

            return new List<RatingScope<RatingStudent>>(scopes.Values).SortName();
        }

        public static List<RatingScope<RatingStudent>> ToGradeYearScopes(this IEnumerable<RatingStudent> students)
        {
            Dictionary<string, RatingScope<RatingStudent>> scopes = new Dictionary<string, RatingScope<RatingStudent>>();

            foreach (RatingStudent each in students)
            {
                string gradeYear = string.Empty;

                if (!string.IsNullOrEmpty(each.RefClassID))
                {
                    int? gy = JHClass.SelectByID(each.RefClassID).GradeYear;
                    if (gy.HasValue) gradeYear = gy.Value.ToString();
                }

                if (!scopes.ContainsKey(gradeYear))
                    scopes.Add(gradeYear, new RatingScope<RatingStudent>(gradeYear));

                scopes[gradeYear].Add(each);
            }

            return new List<RatingScope<RatingStudent>>(scopes.Values).SortName();
        }

        public static List<RatingScope<RatingStudent>> SortName(this List<RatingScope<RatingStudent>> scopes)
        {
            scopes.Sort(new Comparison<RatingScope<RatingStudent>>(delegate(RatingScope<RatingStudent> x, RatingScope<RatingStudent> y)
            {
                return x.Name.CompareTo(y.Name);
            }));

            return scopes;
        }

        public static List<string> ToNameList<T>(this List<T> parsers) where T : IScoreParser<RatingStudent>
        {
            List<string> items = new List<string>();
            foreach (T each in parsers)
                items.Add(each.Name);
            return items;
        }

        public static void Save(Workbook book, string fileName)
        {
            Campus.Report.ReportSaver.SaveWorkbook(book, fileName);

            //SaveFileDialog sdf = new SaveFileDialog();
            //sdf.FileName = fileName;
            //sdf.Filter = "Excel檔案(*.xls)|*.xls";

            //if (sdf.ShowDialog() == DialogResult.OK)
            //{
            //    try
            //    {
            //        book.Save(sdf.FileName, FileFormatType.Excel2003);
            //    }
            //    catch (Exception ex)
            //    {
            //        MsgBox.Show("儲存失敗。" + ex.Message);
            //        return;
            //    }

            //    try
            //    {
            //        //if (MsgBox.Show("排名完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //        //{
            //        System.Diagnostics.Process.Start(sdf.FileName);
            //        //}
            //    }
            //    catch (Exception ex)
            //    {
            //        MsgBox.Show("開啟失敗。" + ex.Message);
            //    }
            //}
        }

        /// <summary>
        /// 將 Token 依 ScoreType 標準化。
        /// </summary>
        public static string Regulation(this ScoreItem item, string token)
        {
            return string.Format("{0}:{1}", item.Type.ToString(), token);
        }

        /// <summary>
        /// 將 Token 依 ScoreType 標準化。
        /// </summary>
        public static string Regulation(this ScoreType type, string token)
        {
            return string.Format("{0}:{1}", type.ToString(), token);
        }
    }

    internal enum FilterMode
    {
        Place,
        Percentage,
        None,
        PlaceL,
        PercentageL
    }

    internal struct FilterParameter
    {
        public int Top { get; set; }
        public FilterMode Mode { get; set; }
        public int Last { get; set; }
    }

    internal class PlaceComparer : IComparer<RatingStudent>
    {
        private string RatingName { get; set; }

        public PlaceComparer(string ratingName)
        {
            RatingName = ratingName;
        }

        #region IComparer<RatingStudent> 成員

        public int Compare(RatingStudent x, RatingStudent y)
        {
            int xx = x.Places.Contains(RatingName) ? x.Places[RatingName].Level : int.MinValue;
            int yy = y.Places.Contains(RatingName) ? y.Places[RatingName].Level : int.MinValue;

            return xx.CompareTo(yy);
        }

        #endregion
    }
}
