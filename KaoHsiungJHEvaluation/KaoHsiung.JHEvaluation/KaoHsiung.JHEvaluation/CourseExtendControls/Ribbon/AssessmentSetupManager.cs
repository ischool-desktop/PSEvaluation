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
using System.Threading;
using Framework;
using JHSchool.Data;
using KaoHsiung.JHEvaluation.EduAdminExtendControls.Ribbon.AssessmentSetupManagerControls;

namespace KaoHsiung.JHEvaluation.EduAdminExtendControls.Ribbon
{
    public partial class AssessmentSetupManager : FISCA.Presentation.Controls.BaseForm
    {
        //private const string DirtyMarkString = "<b><font color=\"#ED1C24\">(已修改)</font></b>";
        private ButtonItem _current_item, _previous_item;

        private BackgroundWorker _workder;
        private EnhancedErrorProvider _errors;
        private List<JHExamRecord> _exam;
        private ChangeListener Listener { get; set; }
        private List<JHAEIncludeRecord> OriginAEs { get; set; }

        public AssessmentSetupManager()
        {
            InitializeComponent();
            HideNavigationBar();

            JHAssessmentSetup.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(JHAssessmentSetup_AfterChanged);
            JHAssessmentSetup.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(JHAssessmentSetup_AfterChanged);
            JHAssessmentSetup.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(JHAssessmentSetup_AfterChanged);

            _errors = new EnhancedErrorProvider();
            _exam = new List<JHExamRecord>();
            OriginAEs = new List<JHAEIncludeRecord>();

            Listener = new ChangeListener();
            Listener.StatusChanged += new EventHandler<ChangeEventArgs>(Listener_StatusChanged);
            Listener.Add(new DataGridViewSource(dataview));
            Listener.Add(new TextBoxSource(txtOStartTime));
            Listener.Add(new TextBoxSource(txtOEndTime));
            Listener.Add(new TextBoxSource(txtTStartTime));
            Listener.Add(new TextBoxSource(txtTEndTime));
            Listener.Reset();
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
                updateItem.Text = record.Name;
                SelectAssessmentSetup(updateItem);
            }
            else if (record == null && updateItem != null) //Delete
            {
                updateItem.Click -= new EventHandler(AssessmentSetup_Click);
                updateItem.DoubleClick -= new EventHandler(AssessmentSetup_DoubleClick);
                ipList.Items.Remove(updateItem);
                ipList.Refresh();
                SelectAssessmentSetup(null);
                CurrentItem = null;
                Listener.Reset();
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
            Listener.SuspendListen();
            dataview.Rows.Clear();

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

                //填評量部分
                OriginAEs.Clear();
                foreach (JHAEIncludeRecord each in JHAEInclude.SelectByAssessmentSetupID(record.ID))
                {
                    OriginAEs.Add(each);
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dataview,
                        each.Exam.ID,
                        each.Weight,
                        each.UseScore,
                        each.UseEffort,
                        "否",
                        "否",
                        each.StartTime,
                        each.EndTime,
                        "是"
                        );
                    dataview.Rows.Add(row);
                }

                //填平時評量及文字評量輸入時間
                txtOStartTime.Text = txtOEndTime.Text = string.Empty;
                txtTStartTime.Text = txtTEndTime.Text = string.Empty;

                if (record.OrdinarilyStartTime.HasValue) txtOStartTime.Text = record.OrdinarilyStartTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
                if (record.OrdinarilyEndTime.HasValue) txtOEndTime.Text = record.OrdinarilyEndTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
                if (record.TextStartTime.HasValue) txtTStartTime.Text = record.TextStartTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
                if (record.TextEndTime.HasValue) txtTEndTime.Text = record.TextEndTime.Value.ToString("yyyy/MM/dd HH:mm:ss");

                FormatDateTime(txtOStartTime, PaddingMethod.First);
                FormatDateTime(txtOEndTime, PaddingMethod.Last);
                FormatDateTime(txtTStartTime, PaddingMethod.First);
                FormatDateTime(txtTEndTime, PaddingMethod.Last);

                item.RaiseClick();
            }
            Listener.Reset();
            Listener.ResumeListen();
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
                }
                else
                    lblIsDirty.Visible = false;
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

        private bool IsDirty()
        {
            return lblIsDirty.Visible;
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
                return dt.Value.ToString(Consts.TimeFormat);
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
                        cell.ErrorText = "您必須輸入數字。";
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

        private void txtStartTime_Validated(object sender, EventArgs e)
        {
            FormatDateTime(sender as TextBoxX, PaddingMethod.First);
        }

        private void txtEndTime_Validated(object sender, EventArgs e)
        {
            FormatDateTime(sender as TextBoxX, PaddingMethod.Last);
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
            if (string.IsNullOrEmpty(textbox.Text)) return;

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

            //string errorMsg = "評量比重加總必須等於 100。";
            //foreach (DataGridViewRow each in dataview.Rows)
            //{
            //    if (each.ErrorText == errorMsg)
            //        each.ErrorText = string.Empty;
            //}

            //if (weight != 100)
            //    dataview.Rows[dataview.Rows.Count - 2].ErrorText = errorMsg;
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
                Listener.SuspendListen();

                JHAssessmentSetupRecord record = CurrentItem.Tag as JHAssessmentSetupRecord;

                //刪除原先的 AEInclude
                if (OriginAEs.Count > 0)
                {
                    JHAEInclude.Delete(OriginAEs);
                    OriginAEs.Clear();
                }

                //List<JHAEIncludeRecord> deleteList = JHAEInclude.SelectByAssessmentSetupID(record.ID);
                //if (deleteList.Count > 0)
                //    JHAEInclude.Delete(deleteList);

                //將畫面上新的 AEInclude 寫入
                List<JHAEIncludeRecord> insertList = new List<JHAEIncludeRecord>();
                foreach (DataGridViewRow each in dataview.Rows)
                {
                    if (each.IsNewRow) continue;

                    JHAEIncludeRecord aeNew = new JHAEIncludeRecord();
                    aeNew.RefAssessmentSetupID = record.ID;
                    aeNew.RefExamID = "" + each.Cells[ExamID.Index].Value;
                    aeNew.UseScore = GetYesNoString(each.Cells[UseScore.Index].FormattedValue, false);
                    aeNew.UseText = GetYesNoString(each.Cells[UseText.Index].FormattedValue, false);
                    aeNew.UseEffort = GetYesNoString(each.Cells[UseEffort.Index].FormattedValue, false);
                    int i;
                    aeNew.Weight = int.TryParse("" + each.Cells[Weight.Index].Value, out i) ? i : 0;
                    aeNew.StartTime = DateToSaveFormat("" + each.Cells[StartTime.Index].Value);
                    aeNew.EndTime = DateToSaveFormat("" + each.Cells[EndTime.Index].Value);

                    insertList.Add(aeNew);
                }
                if (insertList.Count > 0)
                    JHAEInclude.Insert(insertList);

                //繳交時間寫入
                record.OrdinarilyStartTime = GetDateTimeNullable(txtOStartTime.Text, PaddingMethod.First);
                record.OrdinarilyEndTime = GetDateTimeNullable(txtOEndTime.Text, PaddingMethod.Last);
                record.TextStartTime = GetDateTimeNullable(txtTStartTime.Text, PaddingMethod.First);
                record.TextEndTime = GetDateTimeNullable(txtTEndTime.Text, PaddingMethod.Last);
                JHAssessmentSetup.Update(record);

                lblIsDirty.Visible = false;
                Listener.Reset();
                Listener.ResumeListen();
                return true;
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
                return false;
            }
        }

        private DateTime? GetDateTimeNullable(string txt, PaddingMethod method)
        {
            if (string.IsNullOrEmpty(txt)) return null;
            return DateTimeHelper.ParseGregorian(txt, method);
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
                if (each.IsNewRow) continue;

                hasError |= (each.ErrorText != string.Empty);
                foreach (DataGridViewCell cell in each.Cells)
                    hasError |= (cell.ErrorText != string.Empty);
            }

            ValidTimeSpan(txtOStartTime, txtOEndTime);
            ValidTimeSpan(txtTStartTime, txtTEndTime);

            return hasError | _errors.HasError;
        }

        private void ValidTimeSpan(TextBoxX txtStart, TextBoxX txtEnd)
        {
            _errors.SetError(txtStart, "");
            _errors.SetError(txtEnd, "");

            string from = txtStart.Text;
            string to = txtEnd.Text;

            bool b_from = string.IsNullOrEmpty(from);
            bool b_to = string.IsNullOrEmpty(to);

            if (!b_from && !b_to) //都有填時間，檢查時間區間
            {
                DateTime? dt_from = DateTimeHelper.ParseGregorian(from, PaddingMethod.First);
                DateTime? dt_to = DateTimeHelper.ParseGregorian(to, PaddingMethod.Last);
                if (dt_from > dt_to) _errors.SetError(txtEnd, "結束時間必須在開始時間之後。");
                else _errors.SetError(txtEnd, "");
                return;
            }

            bool AND_NOT = !(b_from && b_to);
            bool OR = b_from || b_to;
            bool invalid = AND_NOT & OR;

            if (invalid)
            {
                if (b_from) _errors.SetError(txtStart, "請輸入開始時間。");
                if (b_to) _errors.SetError(txtEnd, "請輸入結束時間。");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentItem == null) return;

                string msg = "確定要刪除「" + (CurrentItem.Tag as JHAssessmentSetupRecord).Name + "」評量設定？\n";
                msg += "刪除後，使用此評量設定的「課程」將會自動變成未設定評量設定狀態。";

                DialogResult dr = MsgBox.Show(msg, Application.ProductName, MessageBoxButtons.YesNo);

                if (dr == DialogResult.Yes)
                {
                    JHAssessmentSetupRecord record = CurrentItem.Tag as JHAssessmentSetupRecord;
                    List<JHAEIncludeRecord> deleteList = JHAEInclude.SelectByAssessmentSetupID(record.ID);
                    if (deleteList.Count > 0)
                        JHAEInclude.Delete(deleteList);
                    JHAssessmentSetup.Delete(record);
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CanContinue()) return;

                AssessmentNameEditor editor = new AssessmentNameEditor();
                DialogResult dr = editor.ShowDialog();
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