using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DevComponents.DotNetBar;
using System.Collections;
using DevComponents.DotNetBar.Controls;
using System.Globalization;
using FISCA.DSAUtil;
using System.Threading;
using JHSchool.Evaluation.CourseExtendControls.Ribbon.AssessmentSetupManagerControls;
using Framework;
using JHSchool.Feature.Legacy;
using JHSchool.Evaluation.Feature.Legacy;
using JHSchool.Evaluation.Editor;
using JHSchool.Data;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon
{
    public partial class AssessmentSetupManager : FISCA.Presentation.Controls.BaseForm
    {
        //private const string DirtyMarkString = "<b><font color=\"#ED1C24\">(已修改)</font></b>";
        private ButtonItem _current_item, _previous_item;

        private BackgroundWorker _workder;
        //private bool _has_deleted;
        private EnhancedErrorProvider _errors;
        private List<JHExamRecord> _exam;
        //private List<JHAssessmentSetupRecord> _as;
        private ChangeListener _listener;

        public AssessmentSetupManager()
        {
            InitializeComponent();
            HideNavigationBar();

            //AssessmentSetup.Instance.ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            //{
            //    if (e.PrimaryKeys.Count > 0)
            //        RefreshItemPanel(e.PrimaryKeys[0]);
            //};
            JHAssessmentSetup.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(JHAssessmentSetup_AfterChanged);
            JHAssessmentSetup.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(JHAssessmentSetup_AfterChanged);
            JHAssessmentSetup.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(JHAssessmentSetup_AfterChanged);

            _errors = new EnhancedErrorProvider();
            _exam = new List<JHExamRecord>();
            //_as = new List<JHAssessmentSetupRecord>();
            _listener = new ChangeListener();
            _listener.StatusChanged += new EventHandler<ChangeEventArgs>(Listener_StatusChanged);
            _listener.Add(new DataGridViewSource(dataview));
            _listener.Add(new TextBoxSource(txtOStartTime));
            _listener.Add(new TextBoxSource(txtOEndTime));
            _listener.Add(new TextBoxSource(txtTStartTime));
            _listener.Add(new TextBoxSource(txtTEndTime));
        }

        private void Listener_StatusChanged(object sender, ChangeEventArgs e)
        {
            lblIsDirty.Visible = (e.Status == ValueStatus.Dirty);
        }

        private void JHAssessmentSetup_AfterChanged(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (e.PrimaryKeys.Count > 0)
                RefreshItemPanel(e.PrimaryKeys[0]);
        }

        private void RefreshItemPanel(string id)
        {
            JHAssessmentSetupRecord record = JHAssessmentSetup.SelectByID(id);
            ButtonItem updateItem = null;

            foreach (ButtonItem item in ipList.Items)
            {
                JHAssessmentSetupRecord r = item.Tag as JHAssessmentSetupRecord;
                if (r.ID == id)
                {
                    updateItem = item;
                    break;
                }
            }

            if (record != null && updateItem == null) //Insert
            {
                AddToList(record);
                ipList.Refresh();
                ipList.EnsureVisible(ipList.Items[ipList.Items.Count - 1]);
            }
            else if (record != null && updateItem != null) //Update
            {
                updateItem.Tag = record;
            }
            else if (record == null && updateItem != null) //Delete
            {
                updateItem.Click -= new EventHandler(AssessmentSetup_Click);
                updateItem.DoubleClick -= new EventHandler(AssessmentSetup_DoubleClick);
                ipList.Items.Remove(updateItem);
                ipList.Refresh();
                SelectAssessmentSetup(null);
                CurrentItem = null;
                _listener.Reset();
            }
        }

        private void HideNavigationBar()
        {
            npLeft.NavigationBar.Visible = false;
        }

        private void DisableFunctions()
        {
            npLeft.Enabled = false;
        }

        private void AssessmentSetupManager_Load(object sender, EventArgs e)
        {
            LoadAssessmentSetups();
            SelectAssessmentSetup(null);//不選擇任何 Tempalte
        }

        /// <summary>
        /// 非同步處理，使用時要小心。
        /// </summary>
        private void LoadAssessmentSetups()
        {
            _workder = new BackgroundWorker();
            _workder.DoWork += new DoWorkEventHandler(Workder_DoWork);
            _workder.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Workder_RunWorkerCompleted);

            BeforeLoadAssessmentSetup();
            _workder.RunWorkerAsync();
        }

        private void Workder_DoWork(object sender, DoWorkEventArgs e)
        {
            _exam.Clear();
            foreach (JHExamRecord exam in JHExam.SelectAll())
                _exam.Add(exam);

            //_as.Clear();
            //foreach (JHAssessmentSetupRecord assessmentSetup in JHAssessmentSetup.SelectAll())
            //    _as.Add(assessmentSetup);
        }

        private void Workder_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    //CurrentUser.ReportError(e.Error);
                    DisableFunctions();
                    AfterLoadAssessmentSetup();
                    MsgBox.Show("下載評量設定資料錯誤。", Application.ProductName);
                    return;
                }

                FillExamRowSource();//將資料填入 Exam 欄位的 RowSource。

                FillAssessmentSetupToItemPanel(); //顯示 Tempalte 資料物件到畫面上。

                AfterLoadAssessmentSetup();
            }
            catch (Exception ex)
            {
                //CurrentUser.ReportError(ex);
                MsgBox.Show(ex.Message);
            }
        }

        private void FillExamRowSource()
        {
            ExamID.Items.Clear();
            ExamID.Items.Add(new KeyValuePair<string, string>("-1", string.Empty));
            foreach (JHExamRecord exam in _exam)
            {
                KeyValuePair<string, string> kv = new KeyValuePair<string, string>(exam.ID, exam.Name);
                ExamID.Items.Add(kv);
            }
            ExamID.DisplayMember = "Value";
            ExamID.ValueMember = "Key";
        }

        private void FillAssessmentSetupToItemPanel()
        {
            foreach (JHAssessmentSetupRecord record in JHAssessmentSetup.SelectAll())
                AddToList(record);
        }

        private void AddToList(JHAssessmentSetupRecord record)
        {
            ButtonItem item = new ButtonItem();
            item.Text = record.Name;
            item.Tag = record;
            item.OptionGroup = "AssessmentSetup";
            item.Click += new EventHandler(AssessmentSetup_Click);
            item.DoubleClick += new EventHandler(AssessmentSetup_DoubleClick);
            ipList.Items.Add(item);
            ipList.RecalcLayout();
        }

        private ButtonItem CurrentItem
        {
            get { return _current_item; }
            set { _current_item = value; }
        }

        private ButtonItem PreviousItem
        {
            get { return _previous_item; }
            set { _previous_item = value; }
        }

        private void SelectAssessmentSetup(ButtonItem item)
        {
            dataview.Rows.Clear();

            //cboScoreSource.Enabled = (item != null);
            //txtStartTime.Enabled = (item != null);
            //txtEndTime.Enabled = (item != null);

            if (item == null)
            {
                peTemplateName1.Text = string.Empty;
                dataview.AllowUserToAddRows = false;
                panel1.Enabled = false;

                return;
            }
            else
            {
                JHAssessmentSetupRecord record = (item.Tag as JHAssessmentSetupRecord);
                peTemplateName1.Text = record.Name;
                dataview.AllowUserToAddRows = true;
                panel1.Enabled = true;

                foreach (JHAEIncludeRecord each in JHAEInclude.SelectByAssessmentSetupID(record.ID))
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dataview, each.Exam.ID, each.Weight, each.UseScore, each.UseEffort, "", "", each.StartTime, each.EndTime);
                    dataview.Rows.Add(row);
                }

                item.RaiseClick();
                _listener.Reset();
            }
        }

        private void AssessmentSetup_Click(object sender, EventArgs e)
        {
            if (CurrentItem == sender) return;

            if (!CanContinue()) return;

            PreviousItem = CurrentItem;
            CurrentItem = sender as ButtonItem;
            SelectAssessmentSetup(CurrentItem);
        }

        private bool CanContinue()
        {
            if (IsDirty())
            {
                DialogResult dr = MsgBox.Show("您未儲存目前資料，是否要儲存？", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (dr == DialogResult.Cancel)
                {
                    CurrentItem.RaiseClick();
                    return false;
                }
                else if (dr == DialogResult.Yes)
                {
                    if (!SaveTemplate())
                    {
                        CurrentItem.RaiseClick();
                        return false;
                    }
                    //ReloadTempalte(CurrentItem);
                }
            }

            return true;
        }

        private void AssessmentSetup_DoubleClick(object sender, EventArgs e)
        {
            if (!CanContinue()) return;

            AssessmentNameEditor editor = new AssessmentNameEditor(CurrentItem.Tag as JHAssessmentSetupRecord);
            DialogResult dr = editor.ShowDialog();
        }

        private void BeforeLoadAssessmentSetup()
        {
            Loading = true;
            ipList.Items.Clear();
            panel1.Enabled = false;
            btnAddNew.Enabled = false;
            btnSave.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void AfterLoadAssessmentSetup()
        {
            ipList.RecalcLayout();
            panel1.Enabled = true;
            btnAddNew.Enabled = true;
            btnSave.Enabled = true;
            btnDelete.Enabled = true;
            Loading = false;
        }

        private bool Loading
        {
            get { return loading.Visible; }
            set { loading.Visible = value; }
        }

        private void ResetDirty()
        {
            //foreach (DataGridViewRow row in dataview.Rows)
            //{
            //    foreach (DataGridViewCell cell in row.Cells)
            //        cell.Tag = cell.Value;
            //}
            //cboScoreSource.Tag = cboScoreSource.SelectedIndex;
            //txtStartTime.Tag = txtStartTime.Text;
            //txtEndTime.Tag = txtEndTime.Text;

            //_has_deleted = false;
            //_errors.Clear();

            //if (CurrentItem == null)
            //    peTemplateName1.Text = string.Empty;
            //else
            //    peTemplateName1.Text = CurrentItem.Text;

        }

        private bool IsDirty()
        {
            return lblIsDirty.Visible;

            //bool dirty = false;
            //foreach (DataGridViewRow row in dataview.Rows)
            //{
            //    foreach (DataGridViewCell cell in row.Cells)
            //        dirty = dirty || (cell.Tag + string.Empty != cell.Value + string.Empty);
            //}

            //dirty |= ((cboScoreSource.Tag == null ? "-1" : cboScoreSource.Tag) + string.Empty != cboScoreSource.SelectedIndex + string.Empty);
            //dirty |= (txtStartTime.Tag + string.Empty != txtStartTime.Text + string.Empty);
            //dirty |= (txtEndTime.Tag + string.Empty != txtEndTime.Text + string.Empty);

            //return dirty || _has_deleted;
        }

        private void dataview_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            dataview.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "資料錯誤！";
            e.Cancel = true;
        }

        private static string DateToDisplayFormat(string source)
        {
            if (source == null || source == string.Empty) return string.Empty;

            DateTime? dt = DateTimeHelper.ParseGregorian(source, PaddingMethod.First);
            if (dt.HasValue)
                return dt.Value.ToString(Consts.TimeFormat);
            else
                return string.Empty;
        }

        private static string DateToSaveFormat(string source)
        {
            if (source == null || source == string.Empty) return string.Empty;

            DateTime? dt = DateTimeHelper.ParseGregorian(source, PaddingMethod.First);
            if (dt.HasValue)
                return dt.Value.ToString("yyyy/MM/dd HH:mm");
            else
                return string.Empty;
        }

        private void dataview_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataview.Rows[e.RowIndex].IsNewRow) return;

            DataGridViewCell cell = GetCell(e.RowIndex, e.ColumnIndex);

            string columnName = GetColumnName(cell);
            string validValue = e.FormattedValue + string.Empty;

            if (columnName == "Weight")
            {
                float weight;

                if (validValue == string.Empty)
                    cell.ErrorText = string.Empty;
                else
                {
                    if (!float.TryParse(validValue + string.Empty, out weight))
                        cell.ErrorText = "您必須輸入小於(等於) 100 的數字。";
                    else
                        cell.ErrorText = string.Empty;
                }
            }
            else if (columnName == "StartTime" || columnName == "EndTime")
            {
                if (validValue == string.Empty)
                    cell.ErrorText = string.Empty;
                else
                {
                    DateTime? dt = DateTimeHelper.ParseGregorian(validValue, PaddingMethod.First);
                    if (!dt.HasValue)
                        cell.ErrorText = "您必須要輸入合法的日期格式。";
                    else
                        cell.ErrorText = string.Empty;
                }
            }
        }

        private void dataview_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dataview.Rows[e.RowIndex].IsNewRow) return;

            DataGridViewCell cell = GetCell(e.RowIndex, e.ColumnIndex);

            if (cell.ErrorText == string.Empty)//表示驗證沒有通過。
            {
                string columnName = GetColumnName(cell);

                if (columnName == "StartTime" || columnName == "EndTime")
                {
                    if ((cell.Value + string.Empty) != string.Empty)
                    {
                        PaddingMethod method = (columnName == "StartTime" ? PaddingMethod.First : PaddingMethod.Last);

                        DateTime? dt = DateTimeHelper.ParseGregorian(cell.Value + string.Empty, method);
                        cell.Value = dt.Value.ToString(Consts.TimeFormat);
                    }
                }
            }
        }

        private void dataview_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewRow row = dataview.Rows[e.RowIndex];

            if (row.IsNewRow) return;

            DataGridViewCell startTime = row.Cells["StartTime"];
            DataGridViewCell endTime = row.Cells["EndTime"];

            if (startTime.ErrorText != string.Empty || endTime.ErrorText != string.Empty)
                return;

            if (startTime.Value != null && endTime.Value != null)
            {
                DateTime? objStart = DateTimeHelper.ParseGregorian(startTime.Value + string.Empty, PaddingMethod.First);
                DateTime? objEnd = DateTimeHelper.ParseGregorian(endTime.Value + string.Empty, PaddingMethod.Last);

                if ((objStart.HasValue && objEnd.HasValue) && objStart.Value > objEnd.Value)
                    row.ErrorText = "「開始時間」必須在「結束時間」之前。";
                else
                    row.ErrorText = string.Empty;
            }

            string exam = row.Cells["ExamID"].Value + string.Empty;
            if (exam == string.Empty)
                row.Cells["ExamID"].ErrorText = "必須選擇一個評量。";
            else
                row.Cells["ExamID"].ErrorText = string.Empty;
        }

        private DataGridViewCell GetCell(int row, int column)
        {
            return dataview.Rows[row].Cells[column];
        }

        private string GetColumnName(DataGridViewCell cell)
        {
            return dataview.Columns[cell.ColumnIndex].Name;
        }

        private void txtStartTime_Validating(object sender, CancelEventArgs e)
        {
            ValidTextTime(sender as TextBoxX, PaddingMethod.First);
        }

        private void txtEndTime_Validating(object sender, CancelEventArgs e)
        {
            ValidTextTime(sender as TextBoxX, PaddingMethod.Last);
        }

        private void ValidTextTime(TextBoxX textbox, PaddingMethod method)
        {
            if (textbox.Text == string.Empty)
                _errors.SetError(textbox, "");
            else
            {
                DateTime? objStart = DateTimeHelper.ParseGregorian(textbox.Text, method);
                if (!objStart.HasValue)
                    _errors.SetError(textbox, "您必須輸入合法的日期格式。");
                else
                    _errors.SetError(textbox, "");
            }
        }

        private void FormatDateTime(TextBoxX textbox, PaddingMethod method)
        {
            if (_errors.GetError(textbox) == string.Empty)
            {
                DateTime? dt = DateTimeHelper.ParseGregorian(textbox.Text, method);
                textbox.Text = dt.Value.ToString(Consts.TimeFormat);
            }
        }

        private void ValidateGrid()
        {
            List<string> examDuplicate = new List<string>();
            float weight = 0;
            foreach (DataGridViewRow each in dataview.Rows)
            {
                if (each.IsNewRow) continue;

                string examValue = string.Empty + each.Cells["ExamID"].Value;
                if (examValue != string.Empty)
                {
                    if (examDuplicate.Contains(examValue))
                        each.ErrorText = "評量名稱重複。";
                    else
                    {
                        examDuplicate.Add(examValue);
                        each.ErrorText = string.Empty;
                    }
                }

                string weightValue = each.Cells["Weight"].Value + string.Empty;
                bool weightNoError = each.Cells["Weight"].ErrorText == string.Empty;
                if (weightValue != string.Empty && weightNoError)
                    weight += float.Parse(weightValue);
            }

            string errorMsg = "評量比重加總必須等於 100。";
            foreach (DataGridViewRow each in dataview.Rows)
            {
                if (each.ErrorText == errorMsg)
                    each.ErrorText = string.Empty;
            }

            if (weight != 100)
                dataview.Rows[dataview.Rows.Count - 2].ErrorText = errorMsg;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (CurrentItem == null) return;

            dataview.EndEdit();

            if (SaveTemplate())
            {
                SelectAssessmentSetup(CurrentItem);
            }
        }

        private bool SaveTemplate()
        {
            if (HasErrors())
            {
                MsgBox.Show("請修正資料後再儲存。", Application.ProductName);
                return false;
            }

            try
            {
                //string startTime = DateToSaveFormat(CurrentItem.StartTime);
                //string endTime = DateToSaveFormat(CurrentItem.EndTime);

                //if (cboScoreSource.SelectedIndex == 0)
                //{
                //    //if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
                //    //{
                //    //    MsgBox.Show("由教師提供課程成績必須指定上傳時間。", Application.ProductName);
                //    //    return false;
                //    //}
                //    //由教師提供
                //    EditTemplate.UpdateTemplate(CurrentItem.Identity, CurrentItem.TemplateName, CurrentItem.AllowUpload, startTime, endTime);
                //}
                //else //由學校計算
                //{
                //    foreach (DataGridViewRow each in dataview.Rows)
                //    {
                //        if (each.IsNewRow) continue;

                //        if ((bool)each.Cells["InputRequired"].FormattedValue)
                //        {
                //            MsgBox.Show("成績由「學校計算」時，所有「評量」必需設定成「強制繳交成績」。");
                //            return false;
                //        }
                //    }

                //    EditTemplate.UpdateTemplate(CurrentItem.Identity, CurrentItem.TemplateName, CurrentItem.AllowUpload, string.Empty, string.Empty);
                //}

                AssessmentSetupRecord record = CurrentItem.Tag as AssessmentSetupRecord;

                List<AEIncludeRecordEditor> editors = new List<AEIncludeRecordEditor>();
                foreach (var item in record.GetAEIncludes())
                {
                    AEIncludeRecordEditor editor = item.GetEditor();
                    editor.Remove = true;
                    editors.Add(editor);
                }
                if (editors.Count > 0)
                    editors.SaveAllEditors();

                editors = new List<AEIncludeRecordEditor>();
                foreach (DataGridViewRow each in dataview.Rows)
                {
                    if (each.IsNewRow) continue;

                    AEIncludeRecordEditor editor = AEInclude.Instance.AddAEInclude();
                    editor.RefAssessmentSetupID = record.ID;
                    editor.RefExamID = "" + each.Cells[ExamID.Index].Value;
                    editor.UseScore = GetYesNoString(each.Cells[UseScore.Index].FormattedValue, false);
                    editor.UseText = GetYesNoString(each.Cells[UseText.Index].FormattedValue, false);
                    editor.UseEffort = GetYesNoString(each.Cells[UseEffort.Index].FormattedValue, false);
                    int i;
                    editor.Weight = int.TryParse("" + each.Cells[Weight.Index].Value, out i) ? i : 0;
                    editor.StartTime = DateToSaveFormat("" + each.Cells[StartTime.Index].Value);
                    editor.EndTime = DateToSaveFormat("" + each.Cells[EndTime.Index].Value);

                    editors.Add(editor);
                }
                if (editors.Count > 0)
                    editors.SaveAllEditors();

                _listener.Reset();

                return true;
            }
            catch (Exception ex)
            {
                //CurrentUser.ReportError(ex);
                MsgBox.Show(ex.Message);
                return false;
            }
        }

        private bool GetYesNoString(object input, bool defaultValue)
        {
            if (input == null)
                return defaultValue;

            bool b;
            if (bool.TryParse("" + input, out b))
                return b;
            else
                return defaultValue;
        }

        private bool HasErrors()
        {
            bool hasError = false;

            foreach (DataGridViewRow each in dataview.Rows)
            {
                hasError |= (each.ErrorText != string.Empty);
                foreach (DataGridViewCell cell in each.Cells)
                    hasError |= (cell.ErrorText != string.Empty);
            }

            return hasError || _errors.HasError;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentItem == null) return;

                string msg = "確定要刪除「" + (CurrentItem.Tag as AssessmentSetupRecord).Name + "」評量設定？\n";
                msg += "刪除後，使用此評量設定的「課程」將會自動變成未設定評量設定狀態。";

                DialogResult dr = MsgBox.Show(msg, Application.ProductName, MessageBoxButtons.YesNo);

                if (dr == DialogResult.Yes)
                {
                    AssessmentSetupRecord record = CurrentItem.Tag as AssessmentSetupRecord;
                    List<AEIncludeRecordEditor> editors = new List<AEIncludeRecordEditor>();
                    foreach (var item in record.GetAEIncludes())
                    {
                        AEIncludeRecordEditor editor = item.GetEditor();
                        editor.Remove = true;
                        editors.Add(editor);
                    }
                    if (editors.Count > 0)
                        editors.SaveAllEditors();

                    AssessmentSetupRecordEditor ASEditor = record.GetEditor();
                    ASEditor.Remove = true;
                    ASEditor.Save();
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
                //CurrentUser.ReportError(ex);
            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CanContinue()) return;

                AssessmentNameEditor editor = new AssessmentNameEditor();
                DialogResult dr = editor.ShowDialog();
                //string newid = string.Empty;

                //if (dr == DialogResult.OK)
                //{
                //    newid = EditTemplate.Insert(editor.TemplateName);
                //    //CourseEntity.Instance.InvokeForeignTableChanged();


                //    Template item = new Template(newid, editor.TemplateName);
                //    item.OptionGroup = "TemplateItems";
                //    item.Click += new EventHandler(Template_Click);
                //    item.DoubleClick += new EventHandler(Template_DoubleClick);

                //    if (editor.ExistTemplate != null)
                //        CopyTemplate(editor.ExistTemplate, item);

                //    AddTemplateToList(item);
                //    _templates.Add(item.Identity, item);
                //    CurrentItem = item;
                //    ReloadTempalte(item);
                //    SelectTemplate(item);
                //}
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
                //CurrentUser.ReportError(ex);
            }
        }

        private void dataview_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //新增ROW時，開放繳交預設為 "是"
            if (string.IsNullOrEmpty(dataview.Rows[e.RowIndex].Cells[ExamID.Name].FormattedValue.ToString()))
            {
                dataview.Rows[e.RowIndex].Cells[OpenTeacherAccess.Name].Value = "是";
                dataview.Rows[e.RowIndex].Cells[OpenTeacherAccess.Name].Tag = "是";
            }
        }

        private void dataview_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            ValidateGrid();
        }
    }
}