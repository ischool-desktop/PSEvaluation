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

namespace KaoHsiung.TransferReport.ConfigForm
{
    public partial class PrintConfigForm : BaseForm
    {
        private string _name;

        public PrintConfigForm(string name)
        {
            InitializeComponent();

            _name = name;

            ConfigData cd = K12.Data.School.Configuration[_name];
            if (cd.Contains("節權設定") && !string.IsNullOrEmpty(cd["節權設定"]))
            {
                XmlElement xml = K12.Data.XmlHelper.LoadXml(cd["節權設定"]);
                chkPeriod.Checked = bool.Parse(xml.SelectSingleNode("Period").InnerText);
                chkCredit.Checked = bool.Parse(xml.SelectSingleNode("Credit").InnerText);
            }
            if (cd.Contains("領域科目設定") && !string.IsNullOrEmpty(cd["領域科目設定"]))
            {
                XmlElement xml = K12.Data.XmlHelper.LoadXml(cd["領域科目設定"]);
                XmlElement staticNode = (XmlElement)xml.SelectSingleNode("Static");
                if (staticNode != null)
                {
                    if (staticNode.InnerText == "Domain")
                        rbDomain.Checked = true;
                    else
                        rbSubject.Checked = true;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ConfigData cd = K12.Data.School.Configuration[_name];

            #region 產生節權數XML
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<PeriodCreditConfig/>");
            XmlElement period = doc.CreateElement("Period");
            period.InnerText = chkPeriod.Checked.ToString();
            XmlElement credit = doc.CreateElement("Credit");
            credit.InnerText = chkCredit.Checked.ToString();
            doc.DocumentElement.AppendChild(period);
            doc.DocumentElement.AppendChild(credit);
            cd["節權設定"] = doc.DocumentElement.OuterXml;
            #endregion

            #region 產生領域科目設定XML
            XmlDocument doc2 = new XmlDocument();
            doc2.LoadXml("<DomainSubjectSetup/>");
            XmlElement staticNode = doc2.CreateElement("Static");
            if (rbDomain.Checked == true)
                staticNode.InnerText = "Domain";
            else
                staticNode.InnerText = "Subject";
            doc2.DocumentElement.AppendChild(staticNode);
            cd["領域科目設定"] = doc2.DocumentElement.OuterXml;
            #endregion

            cd.Save();
            this.DialogResult = DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
