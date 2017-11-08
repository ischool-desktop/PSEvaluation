using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data.Configuration;
using System.Xml;
using Campus.Report;
using System.IO;

namespace HsinChu.StudentRecordReport.ConfigForms
{
    public partial class PrintConfigForm : BaseForm
    {
        private ReportConfiguration _config;

        private ReportConfiguration _Dylanconfig;

        public PrintConfigForm()
        {
            InitializeComponent();
            _config = new ReportConfiguration(Global.ReportName);

            _Dylanconfig = new ReportConfiguration(Global.OneFileSave);

            //SetupDefaultTemplate();
            LoadConfig();
        }

        //private void SetupDefaultTemplate()
        //{
        //    //如果設定中沒有範本，使用預設範本。
        //    if (_config.Template == null)
        //    {
        //        ReportTemplate template = new ReportTemplate(Properties.Resources.國中學籍表, TemplateType.Word);
        //        _config.Template = template;
        //    }
        //}

        private void LoadConfig()
        {
            string print = _config.GetString("領域科目設定", string.Empty);
            if (print == "Domain")
                rbDomain.Checked = true;
            else if (print == "Subject")
                rbSubject.Checked = true;

            chkPeriod.Checked = _config.GetBoolean("列印節數", false);
            chkCredit.Checked = _config.GetBoolean("列印權數", false);

            chkText.Checked = _config.GetBoolean("列印文字評語", true);

            checkBoxX1.Checked = _Dylanconfig.GetBoolean("單檔儲存", false);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _config.SetString("領域科目設定", (rbDomain.Checked) ? "Domain" : "Subject");
            _config.SetBoolean("列印節數", chkPeriod.Checked);
            _config.SetBoolean("列印權數", chkCredit.Checked);
            _config.SetBoolean("列印文字評語", chkText.Checked);
            
            _config.Save();

            _Dylanconfig.SetBoolean("單檔儲存", checkBoxX1.Checked);
            _Dylanconfig.Save();

            this.DialogResult = DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
