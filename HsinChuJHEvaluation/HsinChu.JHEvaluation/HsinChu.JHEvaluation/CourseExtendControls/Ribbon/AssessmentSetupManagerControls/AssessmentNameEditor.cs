using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Framework;
using JHSchool.Evaluation.Editor;
using System.Threading;
using JHSchool.Evaluation;

namespace HsinChu.JHEvaluation.CourseExtendControls.Ribbon.AssessmentSetupManagerControls
{
    internal partial class AssessmentNameEditor : FISCA.Presentation.Controls.BaseForm
    {
        private AssessmentSetupRecord _record;
        private string _orig_name;

        public AssessmentNameEditor()
        {
            InitializeComponent();

            _record = null;
            _orig_name = string.Empty;
            Text = "新增評分樣版";

            foreach (var item in AssessmentSetup.Instance.Items)
                cboExistTemplates.Items.Add(item);
            //foreach (Template tpl in templates)
            //    cboExistTemplates.Items.Add(tpl);
            cboExistTemplates.SelectedIndex = 0;

            
        }

        public AssessmentNameEditor(AssessmentSetupRecord record)
        {
            InitializeComponent();

            _record = record;
            _orig_name = record.Name;
            txtTemplateName.Text = record.Name;
            txtTemplateName.SelectAll();

            Text = "重新命名評分樣版";
            lblCopyExist.Enabled = lblCopyExist.Visible = false;
            cboExistTemplates.Enabled = cboExistTemplates.Visible = false;
            Size = new Size(373, 104);
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTemplateName.Text.Trim()))
            {
                MsgBox.Show("您必須輸入名稱。");
                DialogResult = DialogResult.None;
                return;
            }

            if (ContainsTemplateName(txtTemplateName.Text.Trim()) && txtTemplateName.Text.Trim() != _orig_name)
            {
                MsgBox.Show("名稱重覆，請選擇其他名稱。");
                DialogResult = DialogResult.None;
                txtTemplateName.SelectAll();
                return;
            }

            if (txtTemplateName.Text.Trim() != _orig_name)
            {
                AssessmentSetupRecordEditor editor;
                if (_record == null) //Insert
                    editor = AssessmentSetup.Instance.AddAssessmentSetup();
                else
                    editor = _record.GetEditor();
                editor.Name = txtTemplateName.Text.Trim();

                AssessmentSetup.Instance.ItemUpdated += new EventHandler<ItemUpdatedEventArgs>(Instance_ItemUpdated);
                editor.Save();

                DialogResult = DialogResult.OK;
            }
        }

        private void Instance_ItemUpdated(object sender, ItemUpdatedEventArgs e)
        {
            if (e.PrimaryKeys.Count > 0)
            {
                if (cboExistTemplates.Enabled == true && cboExistTemplates.SelectedItem is AssessmentSetupRecord)
                {
                    AssessmentSetupRecord source = cboExistTemplates.SelectedItem as AssessmentSetupRecord;
                    bool executeRequired = false;

                    List<AEIncludeRecordEditor> aeincludeEditors = new List<AEIncludeRecordEditor>();
                    foreach (var each in source.GetAEIncludes())
                    {
                        AEIncludeRecordEditor aeincludeEditor = AEInclude.Instance.AddAEInclude();
                        aeincludeEditor.RefAssessmentSetupID = e.PrimaryKeys[0];
                        aeincludeEditor.RefExamID = each.RefExamID;
                        aeincludeEditor.UseScore = each.UseScore;
                        aeincludeEditor.UseText = each.UseText;
                        aeincludeEditor.UseEffort = each.UseEffort;
                        aeincludeEditor.Weight = each.Weight;
                        aeincludeEditor.StartTime = each.StartTime;
                        aeincludeEditor.EndTime = each.EndTime;

                        aeincludeEditors.Add(aeincludeEditor);
                        executeRequired = true;
                    }
                    if (executeRequired)
                        aeincludeEditors.SaveAllEditors();
                }
            }
            AssessmentSetup.Instance.ItemUpdated -= new EventHandler<ItemUpdatedEventArgs>(Instance_ItemUpdated);
        }

        private bool ContainsTemplateName(string p)
        {
            foreach (var item in AssessmentSetup.Instance.Items)
            {
                if (item.Name == p)
                    return true;
            }
            return false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}