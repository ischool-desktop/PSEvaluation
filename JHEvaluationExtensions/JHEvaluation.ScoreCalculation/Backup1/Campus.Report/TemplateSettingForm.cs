using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using Aspose.Words;
using Campus.Report;

namespace Campus.Report
{
    public partial class TemplateSettingForm : BaseForm
    {
        public ReportTemplate Template { get; set; }

        private ReportTemplate DefaultTemplate { get; set; }

        public string DefaultFileName
        {
            get { return tsControl.DefaultFileName; }
            set { tsControl.DefaultFileName = value; }
        }

        public TemplateSettingForm(ReportTemplate currentTemplate, ReportTemplate defaultTemplate)
        {
            InitializeComponent();

            Template = currentTemplate;
            DefaultTemplate = defaultTemplate;

            tsControl.UploadClicked += new EventHandler<TemplateEventArgs>(tsControl_UploadClicked);
            tsControl.DownloadClicked += new EventHandler<TemplateEventArgs>(tsControl_DownloadClicked);
            tsControl.ResetClicked += new EventHandler<TemplateEventArgs>(tsControl_ResetClicked);
            tsControl.SetCurrentTemplate(Template);
        }

        private void tsControl_UploadClicked(object sender, TemplateEventArgs e)
        {
            Template = e.Template;
        }

        private void tsControl_DownloadClicked(object sender, TemplateEventArgs e)
        {
            e.Template = Template;
        }

        private void tsControl_ResetClicked(object sender, TemplateEventArgs e)
        {
            e.Template = DefaultTemplate;
        }
    }
}
