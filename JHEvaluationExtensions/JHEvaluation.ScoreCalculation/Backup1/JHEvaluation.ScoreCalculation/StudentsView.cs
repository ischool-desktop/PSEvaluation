using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Presentation;
using Aspose.Cells;

namespace JHEvaluation.ScoreCalculation
{
    public partial class StudentsView<T> : BaseForm where T : IStudent
    {
        private List<T> Students { get; set; }

        public StudentsView(List<T> students, string message)
        {
            InitializeComponent();

            lblMsg.Text = message;
            Students = new List<T>(students);
            Students.Sort(delegate(T x, T y)
            {
                return x.OrderString.CompareTo(y.OrderString);
            });
        }

        private void StudentsView_Load(object sender, EventArgs e)
        {
            dgStudents.AutoGenerateColumns = false;
            dgStudents.DataSource = Students;
        }

        private void btnAddToTemp_Click(object sender, EventArgs e)
        {
            List<string> studentIds = new List<string>();
            foreach (T each in Students)
                studentIds.Add(each.Id);
            NLDPanels.Student.AddToTemp(studentIds);
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            Workbook book = new Workbook();
            book.Worksheets.Clear();
            Worksheet sheet = book.Worksheets[book.Worksheets.Add()];

            int columnIndex = 0;
            foreach (string header in new string[] { "學生系統編號", "班級", "座號", "姓名", "學號" })
                sheet.Cells[0, columnIndex++].PutValue(header);

            int rowIndex = 1;
            foreach (T each in Students)
            {
                columnIndex = 0;
                sheet.Cells[rowIndex, columnIndex++].PutValue(each.Id);
                sheet.Cells[rowIndex, columnIndex++].PutValue(each.ClassName);
                sheet.Cells[rowIndex, columnIndex++].PutValue(each.SeatNo);
                sheet.Cells[rowIndex, columnIndex++].PutValue(each.Name);
                sheet.Cells[rowIndex, columnIndex].PutValue(each.StudentNumber);
                rowIndex++;
            }

            Util.Save(book, "學生清單.xls");
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
        }
    }

    public interface IStudent
    {
        string Id { get; }

        string StudentNumber { get; }

        string ClassName { get; }

        string SeatNo { get; }

        string Name { get; }

        string OrderString { get; }
    }
}
