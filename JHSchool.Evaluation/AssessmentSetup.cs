using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using JHSchool.Evaluation.Feature;
using JHSchool.Evaluation.Editor;
using FISCA.Presentation;
using JHSchool.Editor;

namespace JHSchool.Evaluation
{
    /// <summary>
    /// 負責「評量設定」的快取管理。
    /// </summary>
    public class AssessmentSetup : CacheManager<AssessmentSetupRecord>
    {
        #region 測試程式
        public static void TestProgram()
        {
            //List<AssessmentSetupRecordEditor> editors = new List<AssessmentSetupRecordEditor>();
            //List<AssessmentSetupRecord> records = QueryAssessmentSetup.GetAllAssessmentSetup();
            //records = QueryAssessmentSetup.GetAssessmentSetups("69");

            //AssessmentSetupRecordEditor record = AssessmentSetup.Instance.AddAssessmentSetup();
            //record.Name = "這是新的試別樣版。";
            //record.Description = "新試別樣版的說明。";
            //record.Remove = true;
            //editors.Add(record);

            //foreach (AssessmentSetupRecord each in records)
            //{
            //    AssessmentSetupRecordEditor rec = each.GetEditor();

            //    rec.Name += "1";
            //    rec.Description = "這是評量設定的說明資料。";
            //    rec.Remove = true;

            //    editors.Add(rec);
            //}

            //editors.SaveAll();

        }
        #endregion

        private static AssessmentSetup _Instance = null;
        public static AssessmentSetup Instance { get { if (_Instance == null)_Instance = new AssessmentSetup(); return _Instance; } }
        private AssessmentSetup()
        {
            this.ItemUpdated += delegate
            {
                _assessmentSetupField.Reload();
            };

            Course.Instance.ItemLoaded += delegate
            {
                _assessmentSetupField.Reload();
            };
        }

        protected override Dictionary<string, AssessmentSetupRecord> GetAllData()
        {
            Dictionary<string, AssessmentSetupRecord> records = new Dictionary<string, AssessmentSetupRecord>();

            foreach (AssessmentSetupRecord each in QueryAssessmentSetup.GetAllAssessmentSetup())
                records.Add(each.ID, each);

            return records;
        }

        protected override Dictionary<string, AssessmentSetupRecord> GetData(IEnumerable<string> primaryKeys)
        {
            Dictionary<string, AssessmentSetupRecord> records = new Dictionary<string, AssessmentSetupRecord>();

            foreach (AssessmentSetupRecord each in QueryAssessmentSetup.GetAssessmentSetups(primaryKeys))
                records.Add(each.ID, each);

            return records;
        }

        ListPaneField _assessmentSetupField = new ListPaneField("評分樣版");
        private bool _initialize = false;

        private RibbonBarButton assignButton;
        private MenuButton noAssignButton;

        /// <summary>
        /// 設定使用者介面
        /// </summary>
        public void SetupPresentation()
        {
            if (_initialize) return;

            _assessmentSetupField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                AssessmentSetupRecord record = Course.Instance.Items[e.Key].GetAssessmentSetup();
                if (record != null)
                    e.Value = record.Name;
                else
                    e.Value = "";
            };
            Course.Instance.AddListPaneField(_assessmentSetupField);

            K12.Data.Course.AfterChange += delegate(object s, K12.Data.DataChangedEventArgs arg)
            {
                Course.Instance.SyncDataBackground(arg.PrimaryKeys);
            };
            Course.Instance.ItemUpdated += delegate(object s, ItemUpdatedEventArgs arg)
            {
                _assessmentSetupField.Reload();
            };

            AddAssignAssessmentSetupButtons();

            _initialize = true;
        }

        private void AddAssignAssessmentSetupButtons()
        {
            //增加權限控管 by dylan(2010/11/25)
            Course.Instance.SelectedListChanged += delegate
            {
                assignButton.Enable = User.Acl["JHSchool.Course.Ribbon.AssignAssessmentSetup"].Executable && (Course.Instance.SelectedList.Count > 0);
            };

            assignButton = Course.Instance.RibbonBarItems["指定"]["評分樣版"];
            assignButton.Image = Properties.Resources.btnScores_Image;
            assignButton.Enable = User.Acl["JHSchool.Course.Ribbon.AssignAssessmentSetup"].Executable;
            assignButton.PopupOpen += new EventHandler<PopupOpenEventArgs>(AssignButton_PopupOpen);
            noAssignButton = assignButton["<無評分樣版>"];
            noAssignButton.Tag = "";
            noAssignButton.Click += new EventHandler(MenuButton_Click);
        }

        private void AssignButton_PopupOpen(object sender, PopupOpenEventArgs e)
        {
            if (Course.Instance.SelectedList.Count <= 0) return;

            foreach (var item in AssessmentSetup.Instance.Items)
            {
                MenuButton mb = e.VirtualButtons[item.Name];
                mb.Tag = item.ID;
                mb.Click += new EventHandler(MenuButton_Click);
            }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuButton mb = sender as MenuButton;
            ChangeAssessmentSetup("" + mb.Tag);
        }

        private static void ChangeAssessmentSetup(string id)
        {
            List<CourseRecordEditor> editors = new List<CourseRecordEditor>();
            foreach (var course in Course.Instance.SelectedList)
            {
                CourseRecordEditor editor = course.GetEditor();
                editor.RefAssessmentSetupID = id;
                if (string.IsNullOrEmpty(id))
                    editor.CalculationFlag = "2"; //無評量，則不列入成績
                editors.Add(editor);
            }
            if (editors.Count > 0)
                editors.SaveAllEditors();
        }
    }

    public static class AssessmentSetup_ExtendMethods
    {
        /// <summary>
        /// 取得課程的評分設定。
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public static AssessmentSetupRecord GetAssessmentSetup(this CourseRecord course)
        {
            if (course != null)
                return AssessmentSetup.Instance[course.RefAssessmentSetupID];
            else
                return null;
        }
    }
}
