using System;
using FISCA.Presentation;
using JHSchool.Data;
using FCode = Framework.Security.FeatureCodeAttribute;

namespace JHSchool.Evaluation.ClassExtendControls
{
    [FCode("JHSchool.Class.Detail0010", "班級基本資料")]
    public partial class ClassBaseInfoItem_Extend : DetailContentBase
    {
        private string _programPlanID;

        public ClassBaseInfoItem_Extend()
        {
            InitializeComponent();

            this.Group = "班級基本資料";
            _programPlanID = string.Empty;

            JHClass.AfterChange += new EventHandler<K12.Data.DataChangedEventArgs>(JHClass_AfterChange);
            JHProgramPlan.AfterChange += new EventHandler<K12.Data.DataChangedEventArgs>(JHProgramPlan_AfterChanged);

            //Class.Instance.ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            //{
            //    if (e.PrimaryKeys.Contains(PrimaryKey))
            //        OnPrimaryKeyChanged(EventArgs.Empty);
            //};

            //ProgramPlan.Instance.ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            //{
            //    if (e.PrimaryKeys.Count > 0)
            //    {
            //        cboProgramPlan.SuspendLayout();
            //        cboProgramPlan.Items.Clear();
            //        InitialProgramPlanList();
            //        OnPrimaryKeyChanged(EventArgs.Empty);
            //        cboProgramPlan.ResumeLayout();
            //    }
            //};

        }

        private void JHClass_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (e.PrimaryKeys.Contains(PrimaryKey))
                OnPrimaryKeyChanged(EventArgs.Empty);
        }

        private void JHProgramPlan_AfterChanged(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (e.PrimaryKeys.Count > 0)
            {
                cboProgramPlan.SuspendLayout();
                cboProgramPlan.Items.Clear();
                InitialProgramPlanList();
                OnPrimaryKeyChanged(EventArgs.Empty);
                cboProgramPlan.ResumeLayout();
            }
        }

        private void InitialProgramPlanList()
        {
            //if (!ProgramPlan.Instance.Loaded) ProgramPlan.Instance.SyncAllBackground();
            cboProgramPlan.Items.Add("<不指定>");

            foreach (var item in JHProgramPlan.SelectAll())
            {
                cboProgramPlan.Items.Add(item);
            }
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            SaveButtonVisible = CancelButtonVisible = false;
            JHClassRecord classRecord = JHClass.SelectByID(PrimaryKey);
            if (classRecord != null)
            {
                JHProgramPlanRecord programPlanRecord = GetProgramPlan(classRecord);
                if (programPlanRecord != null)
                {
                    _programPlanID = programPlanRecord.ID;
                    SetComboBoxSelection(programPlanRecord);
                }
                else
                {
                    _programPlanID = string.Empty;
                    SetComboBoxSelection(null);
                }
            }

            base.OnPrimaryKeyChanged(e);
        }

        /// <summary>
        /// 取得班級課程規劃表
        /// </summary>
        /// <param name="classRecord"></param>
        /// <returns></returns>
        private JHProgramPlanRecord GetProgramPlan(JHClassRecord classRecord)
        {
            if (string.IsNullOrEmpty(classRecord.RefProgramPlanID))
                return null;
            else
                return JHProgramPlan.SelectByID(classRecord.RefProgramPlanID);
        }

        private void SetComboBoxSelection(JHProgramPlanRecord programPlanRecord)
        {
            if (programPlanRecord == null)
            {
                cboProgramPlan.SelectedIndex = 0;
            }
            else
            {
                foreach (object item in cboProgramPlan.Items)
                {
                    if (item is JHProgramPlanRecord && (item as JHProgramPlanRecord).ID == programPlanRecord.ID)
                    {
                        cboProgramPlan.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        #region IDetailBulider 成員

        public DetailContent GetContent()
        {
            return this;
        }

        #endregion

        private void cboProgramPlan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProgramPlan.SelectedItem == null) return;

            string id = string.Empty;

            if (cboProgramPlan.SelectedItem is JHProgramPlanRecord)
            {
                JHProgramPlanRecord record = cboProgramPlan.SelectedItem as JHProgramPlanRecord;
                id = record.ID;
            }

            if (id != _programPlanID)
                SaveButtonVisible = CancelButtonVisible = true;
            else
                SaveButtonVisible = CancelButtonVisible = false;
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            if (cboProgramPlan.SelectedItem != null)
            {
                string id = string.Empty;

                if (cboProgramPlan.SelectedItem is JHProgramPlanRecord)
                {
                    JHProgramPlanRecord record = cboProgramPlan.SelectedItem as JHProgramPlanRecord;
                    id = record.ID;
                }

                if (id != _programPlanID)
                {
                    JHClassRecord cla = JHClass.SelectByID(PrimaryKey);
                    cla.RefProgramPlanID = id;
                    JHClass.Update(cla);
                    //ClassRecord classRecord = Class.Instance.Items[PrimaryKey];
                    //ClassRecordEditor editor = classRecord.GetEditor();
                    //editor.RefProgramPlanID = id;
                    //editor.Save();
                }
            }
            base.OnSaveButtonClick(e);
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            JHProgramPlanRecord programPlanRecord = JHProgramPlan.SelectByID(_programPlanID);

            if (programPlanRecord != null)
                SetComboBoxSelection(programPlanRecord);
            else
                SetComboBoxSelection(null);

            base.OnCancelButtonClick(e);
        }

        private void ClassBaseInfoItem_Extend_Load(object sender, EventArgs e)
        {
            InitialProgramPlanList();
        }
    }
}