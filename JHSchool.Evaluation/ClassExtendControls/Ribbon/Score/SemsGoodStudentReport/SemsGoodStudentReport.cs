using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.DSAUtil;
using System.Xml;
using JHSchool.Evaluation.ClassExtendControls.Ribbon.Score.SemsGoodStudentReport.Data;
using JHSchool.Evaluation.ClassExtendControls.Ribbon.Score.SemsGoodStudentReport.DAL;
using Aspose.Cells;
using System.IO;
using System.Diagnostics;
using DevComponents.DotNetBar;

namespace JHSchool.Evaluation.ClassExtendControls.Ribbon.Score.SemsGoodStudentReport
{
    public partial class SemsGoodStudentReport : FISCA.Presentation.Controls.BaseForm
    {
        public SemsGoodStudentReport()
        {
            InitializeComponent();

            int currentSchoolYear;
            if (!int.TryParse(School.DefaultSchoolYear, out currentSchoolYear))
                currentSchoolYear = 97;

            for (int i = currentSchoolYear; i > currentSchoolYear - 3; i--)
                cboSchoolYear.Items.Add(i);

            cboSchoolYear.Text = School.DefaultSchoolYear;

            cboSemester.Items.Add(1);
            cboSemester.Items.Add(2);
            cboSemester.Text = School.DefaultSemester;

            iiCount.Value = 3;
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboSchoolYear_TextChanged(object sender, EventArgs e)
        {
            int year;
            if (!int.TryParse(cboSchoolYear.Text, out year))
                errorProvider1.SetError(cboSchoolYear, "必須為數字");
            else
                errorProvider1.SetError(cboSchoolYear, string.Empty);
        }

        private void cboSemester_TextChanged(object sender, EventArgs e)
        {
            if (cboSemester.Text != "1" && cboSemester.Text != "2")
                errorProvider1.SetError(cboSemester, "必須為1或2");
            else
                errorProvider1.SetError(cboSemester, string.Empty);
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            ReportData report = ReportDAL.GetReportData(cboSchoolYear.Text, cboSemester.Text,Cbo_Exclude_Abnormal.Checked);

            Workbook book = new Workbook();
            book.Worksheets.Clear();
            int index = book.Worksheets.Add();
            Worksheet sheet = book.Worksheets[index];
            sheet.Name = "優異表現名單";

            Range titleRange = sheet.Cells.CreateRange("A1", "E1");
            titleRange.Merge();
            sheet.Cells["A1"].PutValue(cboSchoolYear.Text + "學年度 " + cboSemester.Text + "學期　學期優異表現名單");

            sheet.Cells["A2"].PutValue("班級");
            sheet.Cells["B2"].PutValue("座號");
            sheet.Cells["C2"].PutValue("姓名");
            sheet.Cells["D2"].PutValue("領域總平均");
            sheet.Cells["E2"].PutValue("名次");

            int rowIndex = 3;
            foreach (ClassData cd in report.Classes)
            {
                List<StudentData> studentDatas = cd.GetTopStudent(iiCount.Value);
                foreach (StudentData sd in studentDatas)
                {
                    sheet.Cells["A" + (rowIndex)].PutValue(cd.ClassRecord.Name);
                    sheet.Cells["B" + (rowIndex)].PutValue(sd.Student.SeatNo);
                    sheet.Cells["C" + (rowIndex)].PutValue(sd.Student.Name);
                    sheet.Cells["D" + (rowIndex)].PutValue(sd.Score.ToString());
                    sheet.Cells["E" + (rowIndex)].PutValue(sd.Rank);

                    rowIndex++;
                }
            }
            
            SaveFileDialog sf = new SaveFileDialog();
            sf.FileName = "學期優異表現名單";
            sf.Filter = "Excel 檔案格式|*.xls";
            
            DialogResult result = sf.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filename = sf.FileName;

                try
                {
                    book.Save(filename, FileFormatType.Excel97);
                    Process.Start(filename);
                }
                catch (Exception)
                {
                    MessageBoxEx.Show("存檔過程中發生錯誤, 請檢查檔案是否已存在並關閉該檔案再進行儲存。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }                
            }
        }
    }


}
