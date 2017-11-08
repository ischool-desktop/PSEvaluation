using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Aspose.Words;
using System.IO;
using FISCA.Presentation.Controls;
using Aspose.Cells;

namespace Campus.Report
{
    public partial class TemplateSettingControl : UserControl
    {
        public TemplateSettingControl()
        {
            InitializeComponent();
        }

        public void SetCurrentTemplate(ReportTemplate currentTemplate)
        {
            if (currentTemplate.Type == TemplateType.Word)
                Strategy = new WordStrategy(this);
            else if (currentTemplate.Type == TemplateType.Excel)
                Strategy = new ExcelStrtegy(this);
            else
                throw new ArgumentException("不支援此種格式。");

            Template = currentTemplate;
            Strategy.ShowTemplateProperty(Template);
        }

        private TempalteStrategy Strategy { get; set; }

        public ReportTemplate Template { get; set; }

        public string DefaultFileName { get; set; }

        private void lnkUpload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog of = Strategy.GetOpenDialog();
            of.FileName = DefaultFileName;

            if (of.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileStream fs = new FileStream(of.FileName, FileMode.Open);
                    byte[] data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    fs.Close();

                    ReportTemplate rt = new ReportTemplate(data, Template.Type);
                    rt = Strategy.WriteTemplateProperty(rt);
                    TemplateEventArgs arg = new TemplateEventArgs(rt);

                    if (UploadClicked != null)
                    {
                        UploadClicked(this, arg);
                        try { Strategy.ShowTemplateProperty(arg.Template); } //反正無關緊要。
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    MsgBox.Show("上傳樣版失敗：" + ex.Message);
                }
            }
        }

        private void lnkDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveFileDialog of = Strategy.GetSaveDialog();
            of.FileName = DefaultFileName;

            TemplateEventArgs arg = new TemplateEventArgs(null);
            if (DownloadClicked != null) DownloadClicked(this, arg);

            if (of.ShowDialog() == DialogResult.OK && arg.Template != null)
            {
                try
                {
                    byte[] data = arg.Template.ToBinary();
                    FileStream fs = new FileStream(of.FileName, FileMode.Create);
                    fs.Write(data, 0, data.Length);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存失敗。" + ex.Message);
                    return;
                }

                try
                {
                    System.Diagnostics.Process.Start(of.FileName);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("開啟失敗。" + ex.Message);
                }
            }
        }

        private void lnkReset_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TemplateEventArgs arg = new TemplateEventArgs(null);

            if (ResetClicked != null)
                ResetClicked(this, arg);

            if (UploadClicked != null)
            {
                UploadClicked(this, new TemplateEventArgs(arg.Template));
                Strategy.ShowTemplateProperty(arg.Template);
            }
        }

        /// <summary>
        /// 當使用者按下「上載樣版」時。
        /// </summary>
        public event EventHandler<TemplateEventArgs> UploadClicked;

        /// <summary>
        /// 當使用者按下「下載目前樣版」時。
        /// </summary>
        public event EventHandler<TemplateEventArgs> DownloadClicked;

        /// <summary>
        /// 當使用者按下「重設樣版」時。
        /// </summary>
        public event EventHandler<TemplateEventArgs> ResetClicked;

        private string _author;
        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                lblLastSavedBy.Text = string.Format("修改人：{0}", value);
            }
        }

        private DateTime _last_save_time;
        public DateTime LastSaveTime
        {
            get { return _last_save_time; }
            set
            {
                _last_save_time = value;

                if (_last_save_time == DateTime.MinValue)
                    lblLastSaveTime.Text = "修改時間：";
                else
                    lblLastSaveTime.Text = string.Format("修改時間：{0}", value.ToString("yyyy/MM/dd HH:mm"));
            }
        }

        private int _template_space;
        public int TemplateSpace
        {
            get { return _template_space; }
            set
            {
                _template_space = value;

                int kb = (value >> 10);

                lblSpace.Text = string.Format("樣版容量：{0}KB", kb);
            }
        }

        public DateTime ParseToDateTime(object utcValue)
        {
            DateTime dt;

            if (DateTime.TryParse(utcValue + "", out dt))
                return dt.ToLocalTime();
            else
                return DateTime.MinValue;
        }
    }

    public class TemplateEventArgs : EventArgs
    {
        public TemplateEventArgs(ReportTemplate template)
        {
            Template = template;
        }

        public ReportTemplate Template { get; set; }
    }

    internal class TempalteStrategy
    {
        public TempalteStrategy(TemplateSettingControl control)
        {
            SettingControl = control;
        }

        protected TemplateSettingControl SettingControl { get; set; }

        public virtual void ShowTemplateProperty(ReportTemplate report)
        {
            throw new NotImplementedException();
        }

        public virtual ReportTemplate WriteTemplateProperty(ReportTemplate report)
        {
            return report;
        }

        public virtual OpenFileDialog GetOpenDialog()
        {
            throw new NotImplementedException();
        }

        public virtual SaveFileDialog GetSaveDialog()
        {
            throw new NotImplementedException();
        }
    }

    internal class WordStrategy : TempalteStrategy
    {
        public WordStrategy(TemplateSettingControl control)
            : base(control)
        {
        }

        public override OpenFileDialog GetOpenDialog()
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "選擇報表樣版";
            of.Filter = "Word 檔案(*.doc)|*.doc";
            return of;
        }

        public override SaveFileDialog GetSaveDialog()
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Title = "選擇儲存位置";
            sf.Filter = "Word 檔案(*.doc)|*.doc";
            return sf;
        }

        public override ReportTemplate WriteTemplateProperty(ReportTemplate report)
        {
            Document doc = report.ToDocument();
            doc.BuiltInDocumentProperties.Author = FISCA.Authentication.DSAServices.UserAccount;

            return new ReportTemplate(doc);
        }

        public override void ShowTemplateProperty(ReportTemplate report)
        {
            Document doc = report.ToDocument();
            if (doc == null) return;

            if (doc.BuiltInDocumentProperties.Contains("Author"))
                SettingControl.Author = doc.BuiltInDocumentProperties["Author"].Value + "";
            else
                SettingControl.Author = string.Empty;

            if (doc.BuiltInDocumentProperties.Contains("LastSavedTime"))
                SettingControl.LastSaveTime = SettingControl.ParseToDateTime(doc.BuiltInDocumentProperties["LastSavedTime"].Value);
            else
                SettingControl.LastSaveTime = DateTime.MinValue;

            SettingControl.TemplateSpace = Encoding.UTF8.GetByteCount(report.ToBase64());
        }
    }

    internal class ExcelStrtegy : TempalteStrategy
    {
        public ExcelStrtegy(TemplateSettingControl control)
            : base(control)
        {
        }

        public override OpenFileDialog GetOpenDialog()
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "選擇報表樣版";
            of.Filter = "Excel 檔案(*.xls)|*.xls";
            return of;
        }

        public override SaveFileDialog GetSaveDialog()
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Title = "選擇儲存位置";
            sf.Filter = "Excel 檔案(*.xls)|*.xls";
            return sf;
        }

        public override ReportTemplate WriteTemplateProperty(ReportTemplate report)
        {
            Workbook book = report.ToWorkbook();
            book.Worksheets.BuiltInDocumentProperties.Author = FISCA.Authentication.DSAServices.UserAccount;

            return new ReportTemplate(book);
        }

        public override void ShowTemplateProperty(ReportTemplate report)
        {
            Workbook book = report.ToWorkbook();
            if (book == null) return;

            if (book.Worksheets.BuiltInDocumentProperties.Contains("Author"))
                SettingControl.Author = book.Worksheets.BuiltInDocumentProperties["Author"].Value + "";
            else
                SettingControl.Author = string.Empty;

            if (book.Worksheets.BuiltInDocumentProperties.Contains("LastSavedTime"))
                SettingControl.LastSaveTime = SettingControl.ParseToDateTime(book.Worksheets.BuiltInDocumentProperties["LastSavedTime"].Value);
            else
                SettingControl.LastSaveTime = DateTime.MinValue;

            SettingControl.TemplateSpace = Encoding.UTF8.GetByteCount(report.ToBase64());
        }
    }
}
