using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.Evaluation.Editor;
using FISCA.DSAUtil;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.ScoreCalcRuleRelated
{
    public partial class ScoreCalcRuleCreator : FISCA.Presentation.Controls.BaseForm
    {
        List<string> _names = new List<string>();

        public ScoreCalcRuleCreator()
        {
            InitializeComponent();

            foreach (var record in ScoreCalcRule.Instance.Items)
                _names.Add(record.Name);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(errorProvider.GetError(txtNewName)))
            {
                Save();
                this.DialogResult = DialogResult.OK;
            }
        }

        private void Save()
        {
            ScoreCalcRuleRecordEditor editor = new ScoreCalcRuleRecordEditor();
            editor.Name = txtNewName.Text;
            editor.Content = GetDefault();
            editor.Save();
        }

        private System.Xml.XmlElement GetDefault()
        {
            return Framework.XmlHelper.LoadXml(Properties.Resources.預設成績計算規則Content);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtNewName_TextChanged(object sender, EventArgs e)
        {
            errorProvider.SetError(txtNewName, "");

            if (string.IsNullOrEmpty(txtNewName.Text))
            {
                errorProvider.SetError(txtNewName, "名稱不可為空白");
                return;
            }

            if (_names.Contains(txtNewName.Text))
            {
                errorProvider.SetError(txtNewName, "名稱不可重覆");
                return;
            }
        }
    }
}
