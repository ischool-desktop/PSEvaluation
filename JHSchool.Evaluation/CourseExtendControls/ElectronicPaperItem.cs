using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
//using SmartSchool.AccessControl;
using FISCA.DSAUtil;
using System.Xml;
using System.IO;
using Framework;
using FCode = Framework.Security.FeatureCodeAttribute;

namespace JHSchool.Evaluation.CourseExtendControls
{
    //[Framework.AccessControl.FeatureCode("Content0215")]
    [FCode("JHSchool.Course.Detail0040", "電子報表")]
    public partial class ElectronicPaperItem : JHSchool.Legacy.PalmerwormItemBase
    {
        private BackgroundWorker _loader;
        private string _CurrentID;
        private string _RunningID;

        private string ViewerType { get { return "Course"; } }

        public ElectronicPaperItem()
        {
            InitializeComponent();
            _loader = new BackgroundWorker();
            _loader.DoWork += new DoWorkEventHandler(_loader_DoWork);
            _loader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_loader_RunWorkerCompleted);
            SaveButtonVisible = false;
            CancelButtonVisible = false;
        }

        private void _loader_DoWork(object sender, DoWorkEventArgs e)
        {
            string running_id = e.Argument as string;
            try
            {
                e.Result = JHSchool.Feature.Legacy.QueryElectronicPaper.GetPaperItemByViewer(ViewerType, running_id).GetContent();
            }
            catch (Exception ex)
            {
                //CurrentUser.ReportError(ex);
                e.Result = new DSXmlHelper("BOOM");
            }
        }

        private void _loader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.IsDisposed)
                return;
            if (_RunningID != _CurrentID)
            {
                LoadContent(_CurrentID);
                return;
            }
            this.WaitingPicVisible = false;

            dgEPaper.SuspendLayout();

            DSXmlHelper helper = e.Result as DSXmlHelper;
            foreach (XmlElement paper in helper.GetElements("PaperItem"))
            {
                DSXmlHelper paperHelper = new DSXmlHelper(paper);

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgEPaper,
                    paperHelper.GetText("@ID"),
                    paperHelper.GetText("Format"),
                    paperHelper.GetText("PaperName"),
                    DateTimeFormat(paperHelper.GetText("Timestamp")));
                dgEPaper.Rows.Add(row);
                //row.Tag = paperHelper.GetText("Content");
            }

            dgEPaper.ResumeLayout();
        }

        private string DateTimeFormat(string datetime)
        {
            DateTime tryValue;
            if (DateTime.TryParse(datetime, out tryValue))
                return tryValue.ToString("yyyy/MM/dd HH:mm:ss");
            return "";
        }

        public override void LoadContent(string id)
        {
            _CurrentID = id;

            dgEPaper.Rows.Clear();
            if (!_loader.IsBusy)
            {
                _RunningID = _CurrentID;
                _loader.RunWorkerAsync(_RunningID);
                WaitingPicVisible = true;
            }
        }

        public override void Save()
        {
        }

        public override void Undo()
        {
        }

        public override object Clone()
        {
            return new ElectronicPaperItem();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgEPaper.SelectedRows.Count <= 0) return;
            DataGridViewRow row = dgEPaper.SelectedRows[0];

            if (MsgBox.Show("您確定要刪除「" + row.Cells[colPaperName.Index].Value + "」嗎？", "", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            string id = "" + row.Cells[colID.Index].Value;
            try
            {
                JHSchool.Feature.Legacy.EditElectronicPaper.DeletePaperItem(id);
            }
            catch (Exception ex)
            {
                //CurrentUser.ReportError(ex);
                MsgBox.Show("刪除電子報表發生錯誤。");
            }

            LoadContent(_CurrentID);
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (dgEPaper.SelectedRows.Count <= 0) return;
            DataGridViewRow row = dgEPaper.SelectedRows[0];

            string base64string = "";
            try
            {
                DSXmlHelper helper = JHSchool.Feature.Legacy.QueryElectronicPaper.GetPaperItemContentById("" + row.Cells[colID.Index].Value).GetContent();
                base64string = helper.GetText("PaperItem/Content");
            }
            catch (Exception ex)
            {
                //CurrentUser.ReportError(ex);
                MsgBox.Show(ex.Message);
                return;
            }

            if (string.IsNullOrEmpty(base64string)) return;

            saveFileDialog1.FileName = "" + row.Cells[colPaperName.Index].Value;
            saveFileDialog1.Filter = GetFilter("" + row.Cells[colFormat.Index].Value);
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                //base64 轉 stream
                MemoryStream stream = new MemoryStream(Convert.FromBase64String(base64string));
                SaveFile(stream, saveFileDialog1.FileName, "" + row.Cells[colFormat.Index].Value);
            }
            catch (Exception ex)
            {
                //CurrentUser.ReportError(ex);
                MsgBox.Show("儲存檔案發生錯誤。");
                return;
            }

            System.Diagnostics.Process.Start(saveFileDialog1.FileName);
        }

        private void SaveFile(MemoryStream stream, string filename, string format)
        {
            if (format == "xls")
            {
                Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook();
                wb.Open(stream);
                wb.Save(filename, Aspose.Cells.FileFormatType.Excel2003);
            }
            else if (format == "xlsx")
            {
                Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook();
                wb.Open(stream);
                wb.Save(filename, Aspose.Cells.FileFormatType.Excel2007Xlsx);
            }
            else if (format == "doc")
            {
                Aspose.Words.Document doc = new Aspose.Words.Document(stream);
                doc.Save(filename, Aspose.Words.SaveFormat.Doc);
            }
            else if (format == "docx")
            {
                Aspose.Words.Document doc = new Aspose.Words.Document(stream);
                doc.Save(filename, Aspose.Words.SaveFormat.Docx);
            }
        }

        private string GetFilter(string format)
        {
            string filter = "";
            if (format == "doc")
                filter += "Word 2003 (*.doc)|*.doc";
            else if (format == "docx")
                filter += "Word 2007 (*.docx)|*.docx";
            else if (format == "xls")
                filter += "Excel 2003 (*.xls)|*.xls";
            else if (format == "xlsx")
                filter += "Excel 2007 (*.xlsx)|*.xlsx";
            filter += "|所有檔案 (*.*)|*.*";
            return filter;
        }
    }
}
