using System;
using System.Collections.Generic;
using DevComponents.Editors;
using JHSchool.Evaluation.Editor;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.ProgramPlanRelated
{
    public partial class GraduationPlanCreator : FISCA.Presentation.Controls.BaseForm
    {
        private ProgramPlanRecord _copy_record = null;

        public GraduationPlanCreator()
        {
            InitializeComponent();

            cboExistPlanList.SelectedIndex = 0;

            foreach (var record in ProgramPlan.Instance.Items)
            {
                ComboItem item = new ComboItem();
                item.Text = record.Name;
                item.Tag = record;
                cboExistPlanList.Items.Add(item);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNewName.Text))
            {
                ProgramPlanRecordEditor editor = ProgramPlan.Instance.AddProgramPlan();
                editor.Name = txtNewName.Text;
                if (_copy_record != null)
                {
                    List<ProgramSubject> list = new List<ProgramSubject>();
                    foreach (var subject in _copy_record.Subjects)
                        list.Add(subject.Clone() as ProgramSubject);
                    editor.Subjects = list;
                }
                editor.Save();
                this.Close();
            }
            else
                this.Close();
        }

        private void cboExistPlanList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboExistPlanList.SelectedItem == comboItem1)
                _copy_record = null;
            else
                _copy_record = (ProgramPlanRecord)((ComboItem)cboExistPlanList.SelectedItem).Tag;
        }

        private void txtNewName_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(txtNewName, "");
            btnSave.Enabled = true;
            if (string.IsNullOrEmpty(txtNewName.Text))
            {
                errorProvider1.SetError(txtNewName, "不可空白。");
                btnSave.Enabled = false;
                return;
            }
            foreach (var record in ProgramPlan.Instance.Items)
            {
                if (record.Name == txtNewName.Text)
                {
                    errorProvider1.SetError(txtNewName, "名稱不可重複。");
                    btnSave.Enabled = false;
                    return;
                }
            }
        }
    }
}