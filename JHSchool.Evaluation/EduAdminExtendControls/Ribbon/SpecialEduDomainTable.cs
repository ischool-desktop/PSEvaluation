using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using K12.Data;
using K12.Data.Configuration;
using FISCA.Presentation.Controls;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon
{
    public partial class SpecialEduDomainTable : BaseForm
    {
        private string TitleName = "特殊教育領域設定";
        public SpecialEduDomainTable()
        {
            InitializeComponent();
            this.Text = TitleName;
        }

        private void SpecialEduDomainTable_Load(object sender, EventArgs e)
        {
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            dgv.SuspendLayout();
            dgv.Rows.Clear();

            K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration[TitleName];
            if (cd.Contains("xml") && !string.IsNullOrEmpty(cd["xml"]))
            {
                XmlElement element = cd.GetXml("xml", XmlHelper.LoadXml("<SpecialEduDomainList/>"));
                foreach (XmlElement domainElement in element.SelectNodes("Domain"))
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dgv, domainElement.InnerText);
                    dgv.Rows.Add(row);
                }
            }

            dgv.ResumeLayout();
        }

        private void SaveConfiguration()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement("SpecialEduDomainList");

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                string domain = "" + row.Cells[chDomain.Index].Value;
                if (!string.IsNullOrEmpty(domain))
                {
                    XmlElement domainElement = doc.CreateElement("Domain");
                    domainElement.InnerText = domain;
                    element.AppendChild(domainElement);
                }
            }

            ConfigData cd = K12.Data.School.Configuration[TitleName];
            cd.SetXml("xml", element);
            cd.Save();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid() == false)
            {
                return;
            }
            SaveConfiguration();
            this.DialogResult = DialogResult.OK;
        }

        private bool IsValid()
        {
            List<string> list = new List<string>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                row.Cells[chDomain.Index].ErrorText = string.Empty;

                string domain = "" + row.Cells[chDomain.Index].Value;
                if (list.Contains(domain))
                {
                    row.Cells[chDomain.Index].ErrorText = "領域名稱不能重覆";
                    return false;
                }
                list.Add(domain);
            }

            return true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
