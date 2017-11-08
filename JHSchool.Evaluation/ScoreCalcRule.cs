using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using JHSchool.Evaluation.Feature;
using FISCA.Presentation;
using System.ComponentModel;
using JHSchool.Editor;
using JHSchool.Data;

namespace JHSchool.Evaluation
{
    public class ScoreCalcRule : CacheManager<ScoreCalcRuleRecord>
    {
        private static ScoreCalcRule _Instance = null;
        public static ScoreCalcRule Instance { get { if (_Instance == null)_Instance = new ScoreCalcRule(); return _Instance; } }
        private ScoreCalcRule()
        {
            #region 註冊事件

            this.ItemUpdated += delegate
            {
                _classScoreCalsRuleField.Reload();
                _studentScoreCalsRuleField.Reload();
            };

            Class.Instance.ItemLoaded += delegate
            {
                _classScoreCalsRuleField.Reload();
                _studentScoreCalsRuleField.Reload();
            };
            Class.Instance.ItemUpdated += delegate
            {
                _classScoreCalsRuleField.Reload();
                _studentScoreCalsRuleField.Reload();
            };
            Student.Instance.ItemLoaded += delegate { _studentScoreCalsRuleField.Reload(); };
            Student.Instance.ItemUpdated += delegate { _studentScoreCalsRuleField.Reload(); };

            #endregion
        }

        private FISCA.LogAgent.LogSaver _logSaver = FISCA.LogAgent.ApplicationLog.CreateLogSaverInstance();

        private ListPaneField _classScoreCalsRuleField = new ListPaneField("計算規則");
        private ListPaneField _studentScoreCalsRuleField = new ListPaneField("計算規則");
        private bool _initialize = false;

        private RibbonBarButton classButton;
        private RibbonBarButton studentButton;
        private MenuButton classNoAssignButton;
        private MenuButton studentNoAssignButton;

        private string entity = string.Empty;

        protected override Dictionary<string, ScoreCalcRuleRecord> GetAllData()
        {
            Dictionary<string, ScoreCalcRuleRecord> records = new Dictionary<string, ScoreCalcRuleRecord>();

            foreach (ScoreCalcRuleRecord each in QueryScoreCalcRule.GetAllScoreCalcRules())
                records.Add(each.ID, each);

            return records;
        }

        protected override Dictionary<string, ScoreCalcRuleRecord> GetData(IEnumerable<string> primaryKeys)
        {
            Dictionary<string, ScoreCalcRuleRecord> records = new Dictionary<string, ScoreCalcRuleRecord>();

            foreach (ScoreCalcRuleRecord each in QueryScoreCalcRule.GetScoreCalcRules(primaryKeys))
                records.Add(each.ID, each);

            return records;
        }

        public void SetupPresentation()
        {
            if (_initialize) return;

            _classScoreCalsRuleField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                ScoreCalcRuleRecord record = Class.Instance.Items[e.Key].GetScoreCalcRuleRecord();
                if (record != null)
                    e.Value = record.Name;
                else
                    e.Value = "";
            };
            Class.Instance.AddListPaneField(_classScoreCalsRuleField);

            _studentScoreCalsRuleField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                StudentRecord stu = Student.Instance.Items[e.Key];
                ScoreCalcRuleRecord record = stu.GetScoreCalcRuleRecord();
                if (record != null)
                    e.Value = string.IsNullOrEmpty(stu.OverrideScoreCalcRuleID) ? record.Name : "(指定)" + record.Name;
                else
                    e.Value = "";
            };
            Student.Instance.AddListPaneField(_studentScoreCalsRuleField);

            AddAssignScoreCalcRuleButtons();

            _initialize = true;
        }

        public void AddAssignScoreCalcRuleButtons()
        {
            Class.Instance.SelectedListChanged += delegate { classButton.Enable = (Class.Instance.SelectedList.Count > 0 && User.Acl["JHSchool.Class.Ribbon0056"].Executable); };

            Student.Instance.SelectedListChanged += delegate { studentButton.Enable = (Student.Instance.SelectedList.Count > 0 && User.Acl["JHSchool.Student.Ribbon0056"].Executable); };

            classButton = Class.Instance.RibbonBarItems["指定"]["計算規則"];
            //classButton.Image = JHSchool.Evaluation.ClassExtendControls.Ribbon.Resources.btnProgramPlan_Image;
            classButton.Image = Properties.Resources.course_plan;
            classButton.Tag = "Class";
            classButton.Enable = false;
            classButton.PopupOpen += new EventHandler<PopupOpenEventArgs>(Button_PopupOpen);
            classNoAssignButton = classButton["不指定"];
            classNoAssignButton.Tag = "";
            classNoAssignButton.Click += new EventHandler(ChangeScoreCalcRuleID);

            studentButton = Student.Instance.RibbonBarItems["指定"]["計算規則"];
            //studentButton.Image = JHSchool.Evaluation.ClassExtendControls.Ribbon.Resources.btnProgramPlan_Image;
            studentButton.Image = Properties.Resources.course_plan;
            studentButton.Tag = "Student";
            studentButton.Enable = false;
            studentButton.PopupOpen += new EventHandler<PopupOpenEventArgs>(Button_PopupOpen);
            studentNoAssignButton = studentButton["不指定"];
            studentNoAssignButton.Tag = "";
            studentNoAssignButton.Click += new EventHandler(ChangeScoreCalcRuleID);
        }

        private void Button_PopupOpen(object sender, PopupOpenEventArgs e)
        {
            RibbonBarButton button = sender as RibbonBarButton;
            entity = "" + button.Tag;
            if (entity == "Class" && Class.Instance.SelectedList.Count <= 0) return;
            if (entity == "Student" && Student.Instance.SelectedList.Count <= 0) return;

            foreach (var item in ScoreCalcRule.Instance.Items)
            {
                MenuButton mb = e.VirtualButtons[item.Name];
                mb.Tag = item.ID;
                mb.Click += new EventHandler(ChangeScoreCalcRuleID);
            }
        }

        private void ChangeScoreCalcRuleID(object sender, EventArgs e)
        {
            MenuButton mb = sender as MenuButton;
            if (entity == "Class")
                ChangeClassesScoreCalcRuleID("" + mb.Tag);
            else if (entity == "Student")
                ChangeStudentsScoreCalcRuleID("" + mb.Tag);
        }

        private void ChangeClassesScoreCalcRuleID(string id)
        {
            _logSaver.ClearBatch();
            List<ClassRecordEditor> classRecordEditors = new List<ClassRecordEditor>();
            foreach (var cla in Class.Instance.SelectedList)
            {
                ClassRecordEditor editor = cla.GetEditor();
                editor.RefScoreCalcRuleID = id;
                classRecordEditors.Add(editor);

                string desc = string.Empty;
                if (string.IsNullOrEmpty(id))
                    desc = string.Format("班級「{0}」不指定計算規則", cla.Name);
                else
                    desc = string.Format("班級「{0}」指定計算規則為：{1}", cla.Name, ScoreCalcRule.Instance.Items[id].Name);
                _logSaver.AddBatch("成績系統.計算規則", "班級指定計算規則", desc);
            }
            if (classRecordEditors.Count > 0)
            {
                classRecordEditors.SaveAllEditors();
                _logSaver.LogBatch();

                // 同步班級JHDAL
                List<string> classIDs = (from data in Class.Instance.SelectedList select data.ID).ToList();
                JHClass.RemoveByIDs(classIDs);                
                JHClass.SelectByIDs(classIDs);                
            }
        }

        private void ChangeStudentsScoreCalcRuleID(string id)
        {
            _logSaver.ClearBatch();
            List<StudentRecordEditor> studentRecordEditors = new List<StudentRecordEditor>();
            foreach (var stu in Student.Instance.SelectedList)
            {
                StudentRecordEditor editor = stu.GetEditor();
                editor.OverrideScoreCalcRuleID = id;
                studentRecordEditors.Add(editor);

                string s = string.Empty;
                if (stu.Class != null)
                {
                    s += stu.Class.Name;
                    if (stu.SeatNo != "")
                        s += "(" + stu.SeatNo + "號)";
                    s += " ";
                }
                if (stu.StudentNumber != "")
                    s += stu.StudentNumber + " ";
                if (s == "")
                    s += "學生：";
                s += stu.Name;

                string desc = string.Empty;
                if (string.IsNullOrEmpty(id))
                    desc = string.Format("學生「{0}」不指定計算規則", s);
                else
                    desc = string.Format("學生「{0}」指定計算規則為：{1}", s, ScoreCalcRule.Instance.Items[id].Name);
                _logSaver.AddBatch("成績系統.計算規則", "學生指定計算規則", desc);
            }
            if (studentRecordEditors.Count > 0)
            {
                studentRecordEditors.SaveAllEditors();
                _logSaver.LogBatch();

                // 同步 JHDAL
                List<string> StudentIDs = (from data in Student.Instance.SelectedList select data.ID).ToList();
                JHStudent.RemoveByIDs(StudentIDs);
                JHStudent.SelectByIDs(StudentIDs);                
            }
        }
    }

    public static class ScoreCalcRule_ExtendMethods
    {
        #region Student Extend Methods
        public static ScoreCalcRuleRecord GetScoreCalcRuleRecord(this StudentRecord studentRec)
        {
            string id = "";
            if (studentRec != null)
                if (string.IsNullOrEmpty(studentRec.OverrideScoreCalcRuleID))
                {
                    if (studentRec.Class != null)
                        return studentRec.Class.GetScoreCalcRuleRecord();
                }
                else
                    id = studentRec.OverrideScoreCalcRuleID;

            return ScoreCalcRule.Instance.Items[id];
        }
        #endregion

        #region Class Extend Methods
        public static ScoreCalcRuleRecord GetScoreCalcRuleRecord(this ClassRecord classRec)
        {
            if (classRec != null && !string.IsNullOrEmpty(classRec.RefScoreCalcRuleID))
                return ScoreCalcRule.Instance.Items[classRec.RefScoreCalcRuleID];
            else
                return null;
        }
        #endregion
    }
}
