using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using JHSchool.Evaluation.Feature;
using JHSchool.Evaluation.Editor;
using FISCA.Presentation;
using JHSchool.Editor;
using JHSchool.Data;

namespace JHSchool.Evaluation
{
    public class ProgramPlan : CacheManager<ProgramPlanRecord>
    {
        private static ProgramPlan _Instance = null;
        public static ProgramPlan Instance
        {
            get { if (_Instance == null) _Instance = new ProgramPlan(); return _Instance; }
        }

        private ProgramPlan()
        {
            #region 註冊事件

            this.ItemUpdated += delegate
            {
                _classProgramPlanField.Reload();
                _studentProgramPlanField.Reload();
            };

            Class.Instance.ItemLoaded += delegate
            {
                _classProgramPlanField.Reload();
                _studentProgramPlanField.Reload();
            };
            Class.Instance.ItemUpdated += delegate
            {
                _classProgramPlanField.Reload();
                _studentProgramPlanField.Reload();
            };
            Student.Instance.ItemLoaded += delegate { _studentProgramPlanField.Reload(); };
            Student.Instance.ItemUpdated += delegate { _studentProgramPlanField.Reload(); };

            #endregion
        }

        private FISCA.LogAgent.LogSaver _logSaver = FISCA.LogAgent.ApplicationLog.CreateLogSaverInstance();

        ListPaneField _classProgramPlanField = new ListPaneField("課程規劃");
        ListPaneField _studentProgramPlanField = new ListPaneField("課程規劃");
        private bool _initialize = false;

        private RibbonBarButton classButton;
        private RibbonBarButton studentButton;
        private MenuButton classNoAssignButton;
        private MenuButton studentNoAssignButton;

        private Dictionary<string, JHProgramPlanRecord> programPlanCache;

        private string entity = string.Empty;

        /// <summary>
        /// 設定使用者介面
        /// </summary>
        public void SetupPresentation()
        {
            if (_initialize) return;

            programPlanCache = new Dictionary<string, JHProgramPlanRecord>();

            _classProgramPlanField.PreloadVariableBackground += delegate
            {
                foreach (JHProgramPlanRecord record in JHProgramPlan.SelectAll())
                {
                    if (!programPlanCache.ContainsKey(record.ID))
                        programPlanCache.Add(record.ID, record);
                }
            };

            _classProgramPlanField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                JHProgramPlanRecord record = GetProgramPlan(JHClass.SelectByID(e.Key));

                if (record != null)
                    e.Value = record.Name;
                else
                    e.Value = "";
            };
            K12.Presentation.NLDPanels.Class.AddListPaneField(_classProgramPlanField);

            _studentProgramPlanField.PreloadVariableBackground += delegate
            {
                foreach (JHProgramPlanRecord record in JHProgramPlan.SelectAll())
                {
                    if (!programPlanCache.ContainsKey(record.ID))
                        programPlanCache.Add(record.ID, record);
                }
            };

            _studentProgramPlanField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                JHStudentRecord stu = JHStudent.SelectByID(e.Key);
                JHProgramPlanRecord record = GetProgramPlan(stu);
                if (record != null)
                    e.Value = string.IsNullOrEmpty(stu.OverrideProgramPlanID) ? record.Name : "(指定)" + record.Name;
                else
                    e.Value = "";
            };
            K12.Presentation.NLDPanels.Student.AddListPaneField(_studentProgramPlanField);

            JHClass.AfterUpdate += delegate
            {
                _classProgramPlanField.Reload();
            };

            JHStudent.AfterUpdate += delegate
            {
                _studentProgramPlanField.Reload();
            };

            AddAssignProgramPlanButtons();

            _initialize = true;
        }

        private JHProgramPlanRecord GetProgramPlan(JHStudentRecord stu)
        {
            string id = string.Empty;
            if (stu != null)
                if (string.IsNullOrEmpty(stu.OverrideProgramPlanID))
                {
                    if (stu.Class != null)
                        return GetProgramPlan(stu.Class);
                }
                else
                    id = stu.OverrideProgramPlanID;

            if (programPlanCache.ContainsKey(id)) return programPlanCache[id];
            else return null;
        }

        private JHProgramPlanRecord GetProgramPlan(JHClassRecord classRec)
        {
            if (classRec != null && !string.IsNullOrEmpty(classRec.RefProgramPlanID))
            {
                if (programPlanCache.ContainsKey(classRec.RefProgramPlanID)) return programPlanCache[classRec.RefProgramPlanID];
                else return null;
            }
            else
                return null;
        }

        protected override Dictionary<string, ProgramPlanRecord> GetAllData()
        {
            Dictionary<string, ProgramPlanRecord> records = new Dictionary<string, ProgramPlanRecord>();

            foreach (ProgramPlanRecord each in QueryProgramPlan.GetAllProgramPlans())
                records.Add(each.ID, each);

            return records;
        }

        protected override Dictionary<string, ProgramPlanRecord> GetData(IEnumerable<string> primaryKeys)
        {
            Dictionary<string, ProgramPlanRecord> records = new Dictionary<string, ProgramPlanRecord>();

            foreach (ProgramPlanRecord each in QueryProgramPlan.GetProgramPlans(primaryKeys))
                records.Add(each.ID, each);

            return records;
        }

        private void AddAssignProgramPlanButtons()
        {
            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate { classButton.Enable = (K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0 && User.Acl["JHSchool.Class.Ribbon0055"].Executable); };
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate { studentButton.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0 && User.Acl["JHSchool.Student.Ribbon0055"].Executable); };

            classButton = K12.Presentation.NLDPanels.Class.RibbonBarItems["指定"]["課程規劃"];
            classButton.Image = JHSchool.Evaluation.ClassExtendControls.Ribbon.Resources.btnProgramPlan_Image;
            classButton.Tag = "Class";
            classButton.Enable = false;
            classButton.PopupOpen += new EventHandler<PopupOpenEventArgs>(Button_PopupOpen);
            classNoAssignButton = classButton["不指定"];
            classNoAssignButton.Tag = "";
            classNoAssignButton.Click += new EventHandler(ChangeProgramPlanID);

            studentButton = K12.Presentation.NLDPanels.Student.RibbonBarItems["指定"]["課程規劃"];
            studentButton.Image = JHSchool.Evaluation.ClassExtendControls.Ribbon.Resources.btnProgramPlan_Image;
            studentButton.Tag = "Student";
            studentButton.Enable = false;
            studentButton.PopupOpen += new EventHandler<PopupOpenEventArgs>(Button_PopupOpen);
            studentNoAssignButton = studentButton["不指定"];
            studentNoAssignButton.Tag = "";
            studentNoAssignButton.Click += new EventHandler(ChangeProgramPlanID);
        }

        private void Button_PopupOpen(object sender, PopupOpenEventArgs e)
        {
            RibbonBarButton button = sender as RibbonBarButton;
            entity = "" + button.Tag;
            if (entity == "Class" && K12.Presentation.NLDPanels.Class.SelectedSource.Count <= 0) return;
            if (entity == "Student" && K12.Presentation.NLDPanels.Student.SelectedSource.Count <= 0) return;

            foreach (var item in JHProgramPlan.SelectAll())
            {
                MenuButton mb = e.VirtualButtons[item.Name];
                mb.Tag = item.ID;
                mb.Click += new EventHandler(ChangeProgramPlanID);
            }
        }

        private void ChangeProgramPlanID(object sender, EventArgs e)
        {
            MenuButton mb = sender as MenuButton;
            if (entity == "Class")
                ChangeClassesProgramPlanID("" + mb.Tag);
            else if (entity == "Student")
                ChangeStudentsProgramPlanID("" + mb.Tag);
        }

        private void ChangeClassesProgramPlanID(string id)
        {
            _logSaver.ClearBatch();

            List<JHClassRecord> classList = new List<JHClassRecord>();
            foreach (var cla in JHClass.SelectByIDs(K12.Presentation.NLDPanels.Class.SelectedSource))
            {
                cla.RefProgramPlanID = id;
                classList.Add(cla);

                string desc = string.Empty;
                if (string.IsNullOrEmpty(id))
                    desc = string.Format("班級「{0}」不指定課程規劃", cla.Name);
                else
                    desc = string.Format("班級「{0}」指定課程規劃為：{1}", cla.Name, ProgramPlan.Instance.Items[id].Name);
                _logSaver.AddBatch("成績系統.課程規劃", "班級指定課程規劃", desc);
            }
            if (classList.Count > 0)
            {
                JHClass.Update(classList);
                _logSaver.LogBatch();
            }
        }

        private void ChangeStudentsProgramPlanID(string id)
        {
            _logSaver.ClearBatch();

            List<JHStudentRecord> studentList = new List<JHStudentRecord>();
            foreach (var stu in JHStudent.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource))
            {
                stu.OverrideProgramPlanID = id;
                studentList.Add(stu);

                string s = string.Empty;
                if (stu.Class != null)
                {
                    s += stu.Class.Name;
                    if ("" + stu.SeatNo != "")
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
                    desc = string.Format("學生「{0}」不指定課程規劃", s);
                else
                    desc = string.Format("學生「{0}」指定課程規劃為：{1}", s, ProgramPlan.Instance.Items[id].Name);
                _logSaver.AddBatch("成績系統.課程規劃", "學生指定課程規劃", desc);
            }
            if (studentList.Count > 0)
            {
                JHStudent.Update(studentList);
                _logSaver.LogBatch();
            }
        }

        public static void Test()
        {
            ProgramPlan.Instance.SyncAllBackground();

            if (Student.Instance.SelectedList.Count <= 0) return;

            Student.Instance.SelectedList.FillProgramPlanRecord();
            ProgramPlanRecord record = Student.Instance.SelectedList[0].GetProgramPlanRecord();

            ProgramPlanRecordEditor editor = record.GetEditor();
            //editor.Name = editor.Name + "假的";
            //editor.Save();
        }
    }

    public static class ProgramPlan_ExtendMethods
    {
        #region Student Extend Methods
        public static ProgramPlanRecord GetProgramPlanRecord(this StudentRecord studentRec)
        {
            string id = "";
            if (studentRec != null)
                if (string.IsNullOrEmpty(studentRec.OverrideProgramPlanID))
                {
                    if (studentRec.Class != null)
                        return studentRec.Class.GetProgramPlanRecord();
                }
                else
                    id = studentRec.OverrideProgramPlanID;

            return ProgramPlan.Instance.Items[id];
        }

        public static void FillProgramPlanRecord(this IEnumerable<StudentRecord> studentRecs)
        {
            List<string> primaryKeys = new List<string>();
            foreach (var item in studentRecs)
            {
                if (string.IsNullOrEmpty(item.OverrideProgramPlanID))
                {
                    if (item.Class != null && !string.IsNullOrEmpty(item.Class.RefProgramPlanID))
                        primaryKeys.Add(item.Class.RefProgramPlanID);
                }
                else
                    primaryKeys.Add(item.OverrideProgramPlanID);
            }
            ProgramPlan.Instance.SyncDataBackground(primaryKeys);
        }
        #endregion

        #region Class Extend Methods
        public static ProgramPlanRecord GetProgramPlanRecord(this ClassRecord classRec)
        {
            if (classRec != null && !string.IsNullOrEmpty(classRec.RefProgramPlanID))
                return ProgramPlan.Instance.Items[classRec.RefProgramPlanID];
            else
                return null;
        }

        public static void FillProgramPlanRecord(this IEnumerable<ClassRecord> classRecs)
        {
            List<string> primaryKeys = new List<string>();
            foreach (var item in classRecs)
            {
                primaryKeys.Add(item.RefProgramPlanID);
            }
            ProgramPlan.Instance.SyncDataBackground(primaryKeys);
        }
        #endregion
    }
}
