using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Framework;
using FISCA.Presentation;
using DevComponents.DotNetBar;
using JHSchool.Editor;
using FCode = Framework.Security.FeatureCodeAttribute;
using JHSchool.Data;

namespace JHSchool.Evaluation.CourseExtendControls
{
    [FCode("JHSchool.Course.Detail0020", "成績計算")]
    internal partial class ScoreCalcSetupItem : DetailContentBase
    {
        private ComboBoxItem NoScoreItem = new ComboBoxItem();

        private EnhancedErrorProvider Errors = new EnhancedErrorProvider();

        private EventHandler<K12.Data.DataChangedEventArgs> courseChangedEventHandler;

        public ScoreCalcSetupItem()
        {
            InitializeComponent();

            Group = "成績計算";
            NoScoreItem.Text = "<無評量>";

            courseChangedEventHandler = new EventHandler<K12.Data.DataChangedEventArgs>(JHCourse_AfterChange);
            JHCourse.AfterChange += courseChangedEventHandler;

            JHAssessmentSetup.AfterDelete += delegate { ReloadAssessmentSetupData(); };
            JHAssessmentSetup.AfterInsert += delegate { ReloadAssessmentSetupData(); };
            JHAssessmentSetup.AfterUpdate += delegate { ReloadAssessmentSetupData(); };

            Course.Instance.ItemUpdated += new EventHandler<ItemUpdatedEventArgs>(Instance_ItemUpdated);
        }

        private void Instance_ItemUpdated(object sender, ItemUpdatedEventArgs e)
        {
            foreach( string key in e.PrimaryKeys)
                if (key == PrimaryKey)
                {
                    OnPrimaryKeyChanged(EventArgs.Empty);
                    break;
                }
        }

        private void ScoreCalcSetupItem_Load(object sender, EventArgs e)
        {
            ReloadAssessmentSetupData();
        }

        private void JHCourse_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
                this.Invoke(courseChangedEventHandler, new object[] { sender, e });
            else
            {
                if (e.PrimaryKeys.Contains(PrimaryKey))
                    OnPrimaryKeyChanged(EventArgs.Empty);
            }
        }

        private void ReloadAssessmentSetupData()
        {
            cboAssessmentSetup.Items.Clear();
            cboAssessmentSetup.Items.Add(NoScoreItem);

            foreach (JHAssessmentSetupRecord each in JHAssessmentSetup.SelectAll())
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Text = each.Name;
                item.Tag = each;
                cboAssessmentSetup.Items.Add(item);
            }
        }

        private void DisplayRecord(JHCourseRecord record)
        {
            Enabled = (record != null);
            if (!Enabled) return;

            Errors.Clear();
            if (record != null)
            {
                if (record.CalculationFlag == "1")
                    rdoCalcTrue.Checked = true;
                else if (record.CalculationFlag == "2")
                    rdoCalcFalse.Checked = true;
            }
            JHAssessmentSetupRecord ass = JHAssessmentSetup.SelectByID(record.RefAssessmentSetupID);

            if (ass != null)
                cboAssessmentSetup.Text = ass.Name;
            else
                cboAssessmentSetup.SelectedItem = NoScoreItem;

        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            JHCourse.RemoveByIDs(new string[] { PrimaryKey });
            DisplayRecord(JHCourse.SelectByID(PrimaryKey));
            SaveButtonVisible = false;
            CancelButtonVisible = false;
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            if (!IsValid()) return;

            JHCourseRecord record = JHCourse.SelectByID(PrimaryKey);
            //CourseRecordEditor editor = record.GetEditor();

            if (rdoCalcTrue.Checked)
                record.CalculationFlag = "1";
            else
                record.CalculationFlag = "2";

            record.RefAssessmentSetupID = string.Empty;

            if (cboAssessmentSetup.SelectedItem != null)
            {
                ComboBoxItem item = (cboAssessmentSetup.SelectedItem as ComboBoxItem);
                if (item.Tag != null && (item.Tag + "") != string.Empty)
                    record.RefAssessmentSetupID = (item.Tag as JHAssessmentSetupRecord).ID;
            }

            JHCourse.Update(record);
        }

        private bool IsValid()
        {
            Errors.Clear();
            bool hasError = false;

            if (rdoCalcTrue.Checked)
            {
                ComboBoxItem item = cboAssessmentSetup.SelectedItem as ComboBoxItem;
                if (item == null || (item.Tag + "") == string.Empty)
                {
                    Errors.SetError(cboAssessmentSetup, "如果要列入學期成績，您必需指定一種「評量設定」。");
                    hasError = true;
                }
            }

            return !hasError;
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            OnPrimaryKeyChanged(EventArgs.Empty);
        }

        private void cboAssessmentSetup_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveButtonVisible = true;
            CancelButtonVisible = true;
        }

        private void rdoCalcFalse_CheckedChanged(object sender, EventArgs e)
        {
            SaveButtonVisible = true;
            CancelButtonVisible = true;
        }

        private void rdoCalcTrue_CheckedChanged(object sender, EventArgs e)
        {
            SaveButtonVisible = true;
            CancelButtonVisible = true;
        }
    }
}