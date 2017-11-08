using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.CalculationWizard
{
    class LackScoreExporter
    {
        private CourseCollection _courses;
        private IProgressUI _progress;

        public LackScoreExporter(CourseCollection courses, IProgressUI progress)
        {
            _courses = courses;
            _progress = progress;
        }

        public void Export()
        {
            //每一元素代表一種評量樣版。
            List<LackScoresCategory> lacks = CreateScoresCategory();

            Workbook book = new Workbook();
            book.Worksheets.Clear();

            OutputLackScoreToWorkbook(lacks, book);

            int newIndex = book.Worksheets.Add();
            Worksheet sheet = book.Worksheets[newIndex];
            sheet.Name = "無評量課程清單";
            sheet.Cells[0, 0].PutValue("課程名稱");
            int offset = 1;
            foreach (Course each in _courses.Values)
            {
                if (each.ExamTemplate == null)
                {
                    sheet.Cells[offset, 0].PutValue(each.CourseName);
                    offset++;
                }
            }

            _progress.ReportProgress("儲存檔案中…", 0);

            string fileName = GetFileName();
            book.Save(fileName);
            Process.Start(fileName);

            _progress.ReportProgress("儲存完成…", 0);
        }

        private void OutputLackScoreToWorkbook(List<LackScoresCategory> lacks, Workbook book)
        {
            int totalCount = 0;
            foreach (Course course in _courses.Values)
                totalCount += course.SCAttends.Count;

            int currentIndex = 0;
            foreach (LackScoresCategory each in lacks)
            {
                if (each.Tempalte == null) continue;

                int index = book.Worksheets.Add();
                Worksheet sheet = book.Worksheets[index];
                SheetOutputer outputer = new SheetOutputer(sheet, each.Tempalte);
                sheet.Name = string.Format("成績缺漏學生({0})", each.Tempalte.TemplateName);

                foreach (Course course in each.Courses.Values)
                {
                    foreach (SCAttend attend in course.SCAttends.Values)
                    {
                        if (attend.ContainsLack)
                        {
                            outputer.PutString("課程名稱", course.CourseName);
                            outputer.PutString("班級", attend.ClassName);
                            outputer.PutString("座號", attend.SeatNumber);
                            outputer.PutString("學號", attend.StudentNumber);
                            outputer.PutString("姓名", attend.StudentName);

                            foreach (TEInclude include in course.RefExams)
                            {
                                string score;
                                if (attend.SCETakes.ContainsKey(include.ExamId))
                                    score = attend.SCETakes[include.ExamId].Score;
                                else
                                    score = "無";

                                outputer.PutString(include.ExamId, score);
                            }

                            outputer.MoveNext();
                        }
                        currentIndex++;

                        if (currentIndex % 500 == 0)
                            _progress.ReportProgress("", (int)(((float)currentIndex / (float)totalCount) * 100f));
                    }
                }

                outputer.CalcLayout();
            }
        }

        private string GetFileName()
        {
            string path = Path.Combine(Application.StartupPath, "Reports");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileName = Path.Combine(path, "成績缺漏學生清單.xls");

            int i = 0;
            while (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch
                {
                    fileName = Path.Combine(path, string.Format("成績缺漏學生清單{0}.xls", ++i));
                }
            }

            return fileName;
        }

        private List<LackScoresCategory> CreateScoresCategory()
        {
            Dictionary<string, LackScoresCategory> dicLacks = new Dictionary<string, LackScoresCategory>();
            LackScoresCategory current;
            foreach (Course course in _courses.Values)
            {
                if (!dicLacks.ContainsKey(course.ExamTemplateId))
                {
                    current = new LackScoresCategory(course.ExamTemplate);
                    dicLacks.Add(course.ExamTemplateId, current);
                }
                else
                    current = dicLacks[course.ExamTemplateId];

                current.Courses.Add(course.Identity, course);
            }

            List<LackScoresCategory> lacks = new List<LackScoresCategory>();
            lacks.AddRange(dicLacks.Values);

            return lacks;
        }

        /// <summary>
        /// 缺漏成績有所屬的課程，而課程有所用的評量樣版，此類別代表某一種評量樣版的所有缺漏成績。
        /// </summary>
        class LackScoresCategory
        {
            private ExamTemplate _template;
            private CourseCollection _courses;

            public LackScoresCategory(ExamTemplate template)
            {
                _template = template;
                _courses = new CourseCollection();
            }

            public ExamTemplate Tempalte
            {
                get { return _template; }
            }

            public CourseCollection Courses
            {
                get { return _courses; }
            }
        }

        class SheetOutputer
        {
            private int _current_index;
            private Dictionary<string, int> _fields;
            private Worksheet _sheet;

            public SheetOutputer(Worksheet sheet, ExamTemplate template)
            {
                _fields = new Dictionary<string, int>();
                _sheet = sheet;
                _current_index = 1;

                int columnIndex = -1;
                _fields.Add("課程名稱", ++columnIndex);
                _fields.Add("班級", ++columnIndex);
                _fields.Add("座號", ++columnIndex);
                _fields.Add("學號", ++columnIndex);
                _fields.Add("姓名", ++columnIndex);

                foreach (KeyValuePair<string, int> each in _fields)
                    sheet.Cells[_current_index, each.Value].PutValue(each.Key);

                if (template != null)
                {
                    foreach (TEInclude include in template.TEIncludes.Values)
                        _fields.Add(include.ExamId, ++columnIndex);

                    foreach (TEInclude include in template.TEIncludes.Values)
                        sheet.Cells[_current_index, _fields[include.ExamId]].PutValue(include.ExamName);

                    sheet.Cells.Merge(0, 0, 1, _fields.Count);
                    sheet.Cells[0, 0].PutValue(string.Format("評量成績缺漏學生清單({0})", template.TemplateName));
                    sheet.Cells[0, 0].Style.HorizontalAlignment = TextAlignmentType.Center;
                }

                _current_index++;
            }

            public void PutString(string field, string value)
            {
                string score = value;
                _sheet.Cells[_current_index, _fields[field]].PutValue(score);
            }

            public void MoveNext()
            {
                _current_index++;
            }

            internal void CalcLayout()
            {
                _sheet.Cells.SetColumnWidth(0, 25);
            }
        }
    }
}
