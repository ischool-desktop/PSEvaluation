using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using SmartSchool.Feature.Course;
using System.Xml;
using DevComponents.DotNetBar;
//using SmartSchool.StudentRelated;
using FISCA.DSAUtil;
using Framework;
using JHSchool.Evaluation.Feature.Legacy;
//using DevComponents.DotNetBar;
//using SmartSchool.StudentRelated.Validate;
//using Framework.Presentation.DotNetBar.Validate;
//using SmartSchool.ExceptionHandler;
//using SmartSchool.ApplicationLog;
//using SmartSchool.AccessControl;
using FCode = Framework.Security.FeatureCodeAttribute;

namespace JHSchool.Evaluation.CourseExtendControls
{
    //[Framework.AccessControl.FeatureCode("Content0210")]
    [FCode("JHSchool.Course.Detail0030", "修課學生")]
    internal partial class SCAttendItem : JHSchool.Legacy.PalmerwormItem
    {
        private List<AttendInfo> _delete_list;
        private DSXmlHelper _current_response;

        List<string> _StudIDList;
        public SCAttendItem()
        {
            InitializeComponent();
            Title = "修課學生";
            _StudIDList = new List<string>();
            Course.Instance.ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            {
                if (SaveButtonVisible) return;

                if (e.PrimaryKeys.Contains(RunningID))
                    LoadContent(RunningID);
            };
        }

        //private void SCAttendInfo_Handler(object sender, SmartSchool.Broadcaster.EventArguments e)
        //{

        //    foreach (CourseInformation info in e.Items)
        //    {
        //        if (info.Identity.ToString() == RunningID)
        //        {
        //            LoadContent(RunningID);
        //            break;
        //        }
        //    }
        //}

        protected override object OnBackgroundWorkerWorking()
        {
            try
            {
                List<AttendInfo> attends = new List<AttendInfo>();
                DSResponse rsp = QueryCourse.GetSCAttend(RunningID);
                _current_response = rsp.GetContent();

                foreach (XmlElement each in rsp.GetContent().GetElements("Student"))
                    attends.Add(new AttendInfo(each));

                return attends;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        protected override void OnBackgroundWorkerCompleted(object result)
        {
            if (!(result is Exception))
            {
                List<AttendInfo> attends = result as List<AttendInfo>;
                _delete_list = new List<AttendInfo>();

                lvStudents.Items.Clear();
                lvStudents.Items.AddRange(attends.ToArray());
                label2.Text = lvStudents.Items.Count.ToString();
                _valueManager.AddValue("IsDirty", "False");
                SaveButtonVisible = false;
            }
            else
                Enabled = false;
        }

        private bool RemoveSelected()
        {
            DSResponse rsp = null;
            try
            {
                rsp = JHSchool.Evaluation.Feature.Legacy.QueryCourse.GetSECTake(RunningID);
            }
            catch (Exception ex)
            {
                //CurrentUser user = CurrentUser.Instance;
                //BugReporter.ReportException(user.SystemName, user.SystemVersion, ex, false);
                MsgBox.Show("移除學生修課記錄失敗：" + ex.Message, Application.ProductName);
                return false;
            }

            List<AttendInfo> cannot = new List<AttendInfo>();
            List<AttendInfo> can = new List<AttendInfo>();
            int index = lvStudents.Items.Count - 1;
            for (; index >= 0; index--)
            {
                AttendInfo each = lvStudents.Items[index] as AttendInfo;
                if (each.Selected)
                {
                    if (rsp.GetContent().GetElement("Score/AttendID[.='" + each.Identity + "']") == null)
                        can.Add(each);
                    else
                        cannot.Add(each);
                }
            }

            if (cannot.Count > 0)
            {
                string names = string.Empty;
                foreach (AttendInfo each in cannot)
                    names += string.Format("{0,-12}  {1}", each.Class, each.StudentName + "\n");
                MsgBox.Show("下列學生已經有評量成績，請先刪除評量成績再移除學生修課記錄。\n\n" + names, Application.ProductName);
                return false;
            }
            else
            {
                _delete_list.AddRange(can);
                foreach (AttendInfo each in can)
                    each.Remove();
                return true;
            }
        }

        private void ChangeSelectedItemsRequiredBy(string value)
        {
            if (lvStudents.SelectedItems.Count > 0)
                OnValueChanged("IsDirty", "True");

            foreach (ListViewItem each in lvStudents.SelectedItems)
            {
                AttendInfo info = each as AttendInfo;
                info.RequiredBy = value;
            }
        }

        private void ChangeSelectedItemsRequired(string value)
        {
            if (lvStudents.SelectedItems.Count > 0)
                OnValueChanged("IsDirty", "True");

            foreach (ListViewItem each in lvStudents.SelectedItems)
            {
                AttendInfo info = each as AttendInfo;
                info.IsRequired = value;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvStudents.SelectedItems.Count > 0)
            {
                if (RemoveSelected())
                    OnValueChanged("IsDirty", "True");

                try
                {
                    lvStudents.Select();
                }
                catch { }
            }
            label2.Text = lvStudents.Items.Count.ToString();
        }

        private void lvStudents_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (lvStudents.FocusedItem == null) return;

            if (Control.ModifierKeys == Keys.Control && e.Item.Selected)
            {
                e.Item.Selected = false;
            }
        }

        private void lvStudents_DoubleClick(object sender, EventArgs e)
        {
            if (lvStudents.FocusedItem != null)
            {
                AttendInfo info = lvStudents.FocusedItem as AttendInfo;
                //Student.Instance.ShowDetail(info.RefStudentID.ToString());
                Student.Instance.PopupDetailPane(info.RefStudentID);
            }
        }

        private void btnAdd_PopupOpen(object sender, EventArgs e)
        {
            //依學生的「待處理」清單建立功能表項目。
            CreateStudentMenuItem();
            //同步功能表狀態，將已存在於 ListView 中的項目  Disable。
            SyncStudentMenuItemStatus();
        }

        private void CreateStudentMenuItem()
        {
            btnAdd.SubItems.Clear();

            if (Student.Instance.TemporaList.Count == 0)
            {
                LabelItem item = new LabelItem("No", "沒有任何學生在待處理");
                btnAdd.SubItems.Add(item);
                return;
            }

            foreach (StudentRecord each in Student.Instance.TemporaList)
            {
                if (each.Class != null)
                {
                    ButtonItem item = new ButtonItem(each.ID, each.Name + " (" + Class.Instance.Items[each.Class.ID].Name + ")");
                    item.Tooltip = "班級：" + Class.Instance.Items[each.Class.ID].Name + "\n座號：" + each.SeatNo + "\n學號：" + each.StudentNumber;
                    item.Tag = each;
                    item.Click += new EventHandler(AttendStudent_Click);
                    btnAdd.SubItems.Add(item);
                }
                else
                {
                    ButtonItem item = new ButtonItem(each.ID, each.Name + " (未分班級)");
                    item.Tooltip = "班級：(未分班級)\n座號：" + each.SeatNo + "\n學號：" + each.StudentNumber;
                    item.Tag = each;
                    item.Click += new EventHandler(AttendStudent_Click);
                    btnAdd.SubItems.Add(item);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CreateStudentMenuItem();
            SyncStudentMenuItemStatus();

            //#region 驗證資料
            //AbstractValidateStudent validate = new ValidateBasic(new ValidateGradeYear(), new ValidateGraduationPlan());
            //ErrorViewer viewer = new ErrorViewer();
            //viewer.Text = "學生資料錯誤無法加入修課";
            //bool pass = true;
            //foreach (object obj in btnAdd.SubItems)
            //{
            //    ButtonItem each = obj as ButtonItem;
            //    if (each != null && each.Enabled)
            //    {
            //        BriefStudentData student = each.Tag as BriefStudentData;
            //        if (student != null)
            //        {
            //            //驗證單筆學生資料
            //            pass &= validate.Validate(student, viewer);
            //        }
            //    }
            //}
            //if (!pass)
            //{
            //    viewer.Show();
            //    return;
            //}
            //#endregion

            _StudIDList.Clear();
            foreach (object obj in btnAdd.SubItems)
            {
                ButtonItem each = obj as ButtonItem;
                if (each != null && each.Enabled)
                {
                    StudentRecord student = each.Tag as StudentRecord;
                    if (student != null)
                    {
                        AddAddend(student);
                        _StudIDList.Add(student.ID);
                    }
                }
            }

            if (_StudIDList.Count > 0)
            {
                if (FISCA.Presentation.Controls.MsgBox.Show("已由學生待處理加入修課 共"+_StudIDList.Count+"位學生\n\n是否將其移出待處理?", "學生移除待處理", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    Student.Instance.RemoveFromTemporal(_StudIDList);
            }

            SyncStudentMenuItemStatus();
        }

        private void SyncStudentMenuItemStatus()
        {
            Dictionary<string, ButtonItem> _students = new Dictionary<string, ButtonItem>();
            foreach (object obj in btnAdd.SubItems)
            {
                ButtonItem each = obj as ButtonItem;
                if (each == null) continue;

                StudentRecord stu = each.Tag as StudentRecord;

                if (stu != null)
                {
                    //if (!stu.IsNormal)
                    //{
                    //    each.Enabled = false;
                    //    each.Tooltip = "需為在校生才能加入課程。";
                    //}
                    //else
                    //    _students.Add(stu.ID, each);
                }
            }
            foreach (ListViewItem each in lvStudents.Items)
            {
                AttendInfo info = each as AttendInfo;
                if (_students.ContainsKey(info.RefStudentID.ToString()))
                {
                    _students[info.RefStudentID.ToString()].Enabled = false;
                    _students[info.RefStudentID.ToString()].Tooltip = "此學生已在修課清單中";
                }
            }
        }

        private void AttendStudent_Click(object sender, EventArgs e)
        {
            StudentRecord student = (sender as ButtonItem).Tag as StudentRecord;
            //ErrorViewer viewer = new ErrorViewer();
            //viewer.Text = "學生資料錯誤無法加入修課";
            _StudIDList.Clear();
            if (student != null)
            {
                ////驗證單筆學生資料
                //AbstractValidateStudent validate = new ValidateBasic(new ValidateGradeYear(), new ValidateGraduationPlan());
                //if (!validate.Validate(student, viewer))
                //{
                //    viewer.Show();
                //}
                //else
                AddAddend(student);
                _StudIDList.Add(student.ID);
            }

            if (_StudIDList.Count > 0)
            {
                if (FISCA.Presentation.Controls.MsgBox.Show("己由學生待處理加入修課 \n\n是否將其移出待處理?", "學生移除待處理", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    Student.Instance.RemoveFromTemporal(_StudIDList);
            }
        }

        private void AddAddend(StudentRecord student)
        {
            // 檢查是否要加入
            bool CheckAdd = true;
            foreach (ListViewItem lvi in lvStudents.Items)
            {
                AttendInfo ai = lvi as AttendInfo;
                // 表示學生已經加入
                if (ai.RefStudentID == student.ID)
                {
                    CheckAdd = false;
                    break;
                }
            }

            if (CheckAdd)
            {
                AttendInfo newItem = new AttendInfo(student);
                CourseRecord info = Course.Instance.Items[RunningID];
                //GraduationPlanSubject subject = student.GraduationPlanInfo.GetSubjectInfo(
                //    info.Subject,
                //    info.SubjectLevel
                //    );

                //if (subject.Required == "必修")
                //    newItem.IsRequired = "必";
                //if (subject.Required == "選修")
                //    newItem.IsRequired = "選";
                //newItem.RequiredBy = subject.RequiredBy;

                
                lvStudents.Items.Add(newItem);
                newItem.EnsureVisible();
                OnValueChanged("IsDirty", "True");
                label2.Text = lvStudents.Items.Count.ToString();
            }
        }

        private void chkRequired_Click(object sender, EventArgs e)
        {
            ChangeSelectedItemsRequired("必");
        }

        private void chkUnRequired_Click(object sender, EventArgs e)
        {
            ChangeSelectedItemsRequired("選");
        }

        private void buttonItem4_Click(object sender, EventArgs e)
        {
            ChangeSelectedItemsRequired("");
        }

        private void buttonItem2_Click(object sender, EventArgs e)
        {
            ChangeSelectedItemsRequiredBy("校訂");
        }

        private void buttonItem3_Click(object sender, EventArgs e)
        {
            ChangeSelectedItemsRequiredBy("部訂");
        }

        private void buttonItem5_Click(object sender, EventArgs e)
        {
            ChangeSelectedItemsRequiredBy("");
        }

        public override void Save()
        {
            List<AttendInfo> insertList = new List<AttendInfo>();     //新增清單
            List<AttendInfo> updateList = new List<AttendInfo>();  //更新清單
            List<AttendInfo> deleteList = new List<AttendInfo>();    //刪除清單

            GetList(insertList, updateList, deleteList);

            try
            {
                List<string> synclist = new List<string>();
                DSXmlHelper helper = new DSXmlHelper("InsertSCAttend");
                foreach (AttendInfo each in insertList)
                {
                    XmlElement attend = helper.AddElement("Attend");
                    DSXmlHelper.AppendChild(attend, "<RefStudentID>" + each.RefStudentID.ToString() + "</RefStudentID>");
                    DSXmlHelper.AppendChild(attend, "<RefCourseID>" + RunningID + "</RefCourseID>");
                    DSXmlHelper.AppendChild(attend, "<IsRequired>" + each.IsRequired + "</IsRequired>");
                    DSXmlHelper.AppendChild(attend, "<RequiredBy>" + each.RequiredBy + "</RequiredBy>");
                    //DSXmlHelper.AppendChild(attend, "<GradeYear>" + each.GradeYear + "</GradeYear>");
                    helper.AddElement(".", attend);
                }
                if (insertList.Count > 0)
                {
                    DSXmlHelper insertResult = AddCourse.AttendCourse(helper).GetContent();
                    foreach (XmlElement each in insertResult.GetElements("NewID"))
                        synclist.Add(each.InnerText);

                }

                helper = new DSXmlHelper("UpdateSCAttend");
                foreach (AttendInfo each in updateList)
                {
                    XmlElement attend = helper.AddElement("Attend");
                    DSXmlHelper.AppendChild(attend, "<ID>" + each.Identity + "</ID>");
                    DSXmlHelper.AppendChild(attend, "<IsRequired>" + each.IsRequired + "</IsRequired>");
                    DSXmlHelper.AppendChild(attend, "<RequiredBy>" + each.RequiredBy + "</RequiredBy>");
                    synclist.Add(each.Identity.ToString());

                    helper.AddElement(".", attend);
                }
                if (updateList.Count > 0)
                    EditCourse.UpdateAttend(helper);

                helper = new DSXmlHelper("DeleteSCAttendRequest");
                foreach (AttendInfo each in deleteList)
                {
                    XmlElement attend = helper.AddElement("Attend");
                    DSXmlHelper.AppendChild(attend, "<ID>" + each.Identity + "</ID>");
                    synclist.Add(each.Identity.ToString());

                    helper.AddElement(".", attend);
                }
                if (deleteList.Count > 0)
                    EditCourse.DeleteAttend(helper);

                #region Log

                StringBuilder desc = new StringBuilder("");
                desc.AppendLine("課程名稱：" + Course.Instance.Items[RunningID].Name);
                if (insertList.Count > 0)
                    desc.AppendLine("加入修課學生：");
                foreach (AttendInfo info in insertList)
                    desc.AppendLine("- " + info.Class + " " + info.StudentNumber + " " + info.StudentName + " ");
                if (deleteList.Count > 0)
                    desc.AppendLine("移除課程修課學生：");
                foreach (AttendInfo info in deleteList)
                    desc.AppendLine("- " + info.Class + " " + info.StudentNumber + " " + info.StudentName + " ");
                if (updateList.Count > 0)
                    desc.AppendLine("課程修課學生修課資訊調整：");
                foreach (AttendInfo info in updateList)
                    desc.AppendLine("- " + info.Class + " " + info.StudentNumber + " " + info.StudentName + " ");

                //CurrentUser.Instance.AppLog.Write(EntityType.Course, "修改課程修課學生", RunningID, desc.ToString(), "課程", "");

                #endregion

                //SCAttend.Instance.SyncDataBackground(synclist);
                LoadContent(RunningID);
            }
            catch (Exception ex)
            {
                MsgBox.Show("儲存失敗：" + ex.Message);
            }
        }

        private void GetList(List<AttendInfo> insertList, List<AttendInfo> updateList, List<AttendInfo> deleteList)
        {
            foreach (ListViewItem each in lvStudents.Items)
            {
                AttendInfo info = each as AttendInfo;
                if (info.Identity == -1) //新增
                    insertList.Add(info);
                else if (info.IsDirty) //修改
                {
                    updateList.Add(info);
                }
            }

            foreach (AttendInfo each in _delete_list)
            {
                if (each.Identity != -1) //刪除
                {
                    deleteList.Add(each);
                }
            }
        }

        #region AttendInfo
        private class AttendInfo : ListViewItem
        {
            private int _identity;
            private string _name;
            private string _is_required;
            private string _class;
            private string _snum;
            private string _requiredby;
            private string _seatno;
            private string _ref_studentid;
            private bool _is_dirty;
            private string _gradeyear;

            public AttendInfo(XmlElement attend)
            {
                _identity = GetIntValue(attend, "@ID");
                _name = GetStringValue(attend, "Name");
                _is_required = GetStringValue(attend, "IsRequired");
                _requiredby = GetStringValue(attend, "RequiredBy");
                _class = GetStringValue(attend, "ClassName");
                _snum = GetStringValue(attend, "StudentNumber");
                _seatno = GetStringValue(attend, "SeatNumber");
                _ref_studentid = GetStringValue(attend, "RefStudentID");
                _gradeyear = GetStringValue(attend, "GradeYear");
                Text = Class;
                CreateSubItem();

                _is_dirty = false;
            }

            public AttendInfo(StudentRecord newStudent)
            {
                _identity = -1;
                _name = newStudent.Name;
                _is_required = "";
                _requiredby = "";
                if (newStudent.Class != null)
                {
                    _class = JHSchool.Class.Instance.Items[newStudent.Class.ID].Name;
                    _gradeyear = JHSchool.Class.Instance.Items[newStudent.Class.ID].GradeYear;
                }
                _snum = newStudent.StudentNumber;
                _seatno = newStudent.SeatNo;
                _ref_studentid = newStudent.ID;
                Text = Class;
                CreateSubItem();

                _is_dirty = false;

                foreach (ListViewSubItem each in SubItems)
                    each.ForeColor = Color.Blue;
            }

            private void CreateSubItem()
            {
                AddSubItem("SeatNumber", _seatno.ToString());
                AddSubItem("StudentNumber", _snum);
                AddSubItem("StudentName", _name);
                AddSubItem("IsRequired", _is_required);
                AddSubItem("RequiredBy", _requiredby);
            }

            private void AddSubItem(string name, string value)
            {
                ListViewSubItem sub = new ListViewSubItem();
                sub.Text = value;
                sub.Name = name;
                if ((name == "IsRequired" || name == "RequiredBy") && value == "")
                {
                    sub.Text = "未指定";
                    sub.ForeColor = Color.LightGray;
                }

                SubItems.Add(sub);
            }

            public int Identity
            {
                get { return _identity; }
            }

            public string StudentName
            {
                get { return _name; }
            }

            public string GradeYear
            {
                get { return _gradeyear; }
            }

            public string IsRequired
            {
                get { return _is_required; }
                set
                {
                    _is_dirty = (IsRequired != value);
                    _is_required = value;
                    if (value == "")
                        SubItems["IsRequired"].Text = "未指定";
                    else
                        SubItems["IsRequired"].Text = _is_required;

                    if (_is_dirty)
                    {
                        SubItems["IsRequired"].ForeColor = Color.Blue;
                    }
                }
            }

            public string RequiredBy
            {
                get { return _requiredby; }
                set
                {
                    _is_dirty = (_requiredby != value);
                    _requiredby = value;
                    if (value == "")
                        SubItems["RequiredBy"].Text = "未指定";
                    else
                        SubItems["RequiredBy"].Text = _requiredby;

                    if (_is_dirty)
                    {
                        SubItems["RequiredBy"].ForeColor = Color.Blue;
                    }
                }
            }

            public string Class
            {
                get { return _class; }
            }

            public string StudentNumber
            {
                get { return _snum; }
            }

            public string SeatNumber
            {
                get { return _seatno; }
            }

            public string RefStudentID
            {
                get { return _ref_studentid; }
            }

            public bool IsDirty
            {
                get { return _is_dirty; }
            }

            private int GetIntValue(XmlElement rawData, string xpath)
            {
                XmlNode value = rawData.SelectSingleNode(xpath);

                ThrowResultNullException(xpath, value);

                if (string.IsNullOrEmpty(value.InnerText))
                    return -1;

                int result;
                if (int.TryParse(value.InnerText, out result))
                    return result;
                else
                    throw new ArgumentException("欄位資料型態不正確，XPath 路徑：" + xpath);
            }

            private string GetStringValue(XmlElement rawData, string xpath)
            {
                XmlNode value = rawData.SelectSingleNode(xpath);

                ThrowResultNullException(xpath, value);

                return value.InnerText;
            }

            private static void ThrowResultNullException(string xpath, XmlNode value)
            {
                if (value == null)
                    throw new ArgumentException("欄位資料不存在，XPath 路徑：" + xpath);
            }

        }
        #endregion

        private void SCAttendInfo_DoubleClick(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift)
            {
                JHSchool.Legacy.XmlBox.ShowXml(_current_response.BaseElement);
            }
        }
    }
}
