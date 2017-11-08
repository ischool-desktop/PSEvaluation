using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using HsinChu.TransferReport.ConfigForm;

namespace HsinChu.TransferReport
{
    public partial class TransferReportForm : BaseForm
    {
        private const string ReportName = "轉學成績證明書";
        private Config _config;

        public TransferReportForm()
        {
            InitializeComponent();
            _config = new Config(ReportName);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            _config.SetStudents(JHSchool.Data.JHStudent.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource));
            _config.Load();
            Report report = new Report(_config);
            report.GenerateReport();

            this.DialogResult = DialogResult.OK;
        }

        private void lnTypeConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SelectTypeForm form = new SelectTypeForm(ReportName);
            form.ShowDialog();
        }

        private void lnDomainSetup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //DomainSetupForm form = new DomainSetupForm("轉學成績證明書");
            //form.ShowDialog();

            PrintConfigForm form = new PrintConfigForm(ReportName);
            form.ShowDialog();
        }
    }
}
