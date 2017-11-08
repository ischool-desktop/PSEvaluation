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
using Framework;
using JHSchool.Feature.Legacy;
using JHSchool.Evaluation.Feature.Legacy;
using JHSchool.Evaluation.Editor;
using JHSchool.Evaluation;
using HsinChu.JHEvaluation.CourseExtendControls.Ribbon.AssessmentSetupManagerControls;
using JHSchool.Data;
using HsinChu.JHEvaluation.Data;
using FISCA.Data;
using System.Xml.Linq;

namespace HsinChu.JHEvaluation.CourseExtendControls.Ribbon
{
    public partial class AssessmentSetupManager : FISCA.Presentation.Controls.BaseForm
    {
        //private const string DirtyMarkString = "<b><font color=\"#ED1C24\">(已修改)</font></b>";
        private ButtonItem _current_item, _previous_item;

        private BackgroundWorker _workder;
        private bool _has_deleted;
        private EnhancedErrorProvider _errors;
        private bool _CheckSaveAssessmentSetup = false;

        public AssessmentSetupManager()
        {
            InitializeComponent();
            HideNavigationBar();

            AssessmentSetup.Instance.ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            {
                if (e.PrimaryKeys.Count > 0)
                    RefreshItemPanel(e.PrimaryKeys[0]);
            };

            _errors = new EnhancedErrorProvider();
        }

        private void RefreshItemPanel(string id)
        {
            AssessmentSetupRecord record = AssessmentSetup.Instance.Items[id];
            ButtonItem updateItem = null;

            foreach (ButtonItem item in ipList.Items)
            {
                AssessmentSetupRecord r = item.Tag as AssessmentSetupRecord;
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
                ResetDirty();
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
            Exam.Instance.SyncAllBackground();
            AssessmentSetup.Instance.SyncAllBackground();
            AEInclude.Instance.SyncAllBackground();
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
            foreach (var item in Exam.Instance.Items)
            {
                KeyValuePair<string, string> exam = new KeyValuePair<string, string>(item.ID, item.Name);
                ExamID.Items.Add(exam);
            }
            ExamID.DisplayMember = "Value";
            ExamID.ValueMember = "Key";
        }

        private void FillAssessmentSetupToItemPanel()
        {
            foreach (var record in AssessmentSetup.Instance.Items)
            {
                AddToList(record);
            }
        }

        private void AddToList(AssessmentSetupRecord record)
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

        private void SaveAssessmentSetupToDB(string ID)
        {
            XElement elm = new XElement("Extension");
            // 定期比例
            elm.SetElementValue("ScorePercentage", ipt01.Value);
            // 平時比例
           
            string query = "update exam_template set extension='" + elm.ToString() + "' where id=" + ID;
            UpdateHelper uh = new UpdateHelper();
            uh.Execute(query);
        }

        private void SelectAssessmentSetup(ButtonItem item)
        {
            dataview.Rows.Clear();
            
            //cboScoreSource.Enabled = (item != null);
            //txtStartTime.Enabled = (item != null);
            //txtEndTime.Enabled = (item != null);
            ipt01.Tag = null;
            if (item == null)
            {
                
                peTemplateName1.Text = string.Empty;
                dataview.AllowUserToAddRows = false;
                //panel1.Enabled = false;
                ipt01.Enabled = false;
                return;
            }
            else
            {
                
                AssessmentSetupRecord record = (item.Tag as AssessmentSetupRecord);
                peTemplateName1.Text = record.Name;
                dataview.AllowUserToAddRows = true;
                //panel1.Enabled = true;
                              

                // 需要儲存樣版比例
                if (_CheckSaveAssessmentSetup)
                {
                    SaveAssessmentSetupToDB(record.ID);
                    _CheckSaveAssessmentSetup = false;
                }

                ipt01.Value = 50;
                ipt01.Enabled =true;

                // 取得比例
                QueryHelper qh = new QueryHelper();
                string selQuery = "select id,extension from exam_template where id=" + record.ID;
                DataTable dt = qh.Select(selQuery);
                string xmlStr = "<root>" + dt.Rows[0]["extension"].ToString() + "</root>";
                XElement elmRoot = XElement.Parse(xmlStr);

                if (elmRoot != null)
                {
                    if (elmRoot.Element("Extension") != null)
                    {
                        if (elmRoot.Element("Extension").Element("ScorePercentage") != null)
                            ipt01.Value = int.Parse(elmRoot.Element("Extension").Element("ScorePercentage").Value);
                        
                    }
                }               

                foreach (var ae in JHAEInclude.SelectByAssessmentSetupID(record.ID))
                {
                    HC.JHAEIncludeRecord each = new HC.JHAEIncludeRecord(ae);
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dataview, each.RefExamID, each.Weight, each.UseScore, each.UseAssignmentScore, each.UseText, "", each.StartTime, each.EndTime);
                    dataview.Rows.Add(row);
                }

                item.RaiseClick();
                ResetDirty();
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
            if (IsDirty() || lblIsDirty.Visible)
            {
                DialogResult dr = MsgBox.Show("您未儲存目前資料，是否要儲存？", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (dr == DialogResult.Cancel)
                {
                    CurrentItem.RaiseClick();
                    return false;
                }
                else if (dr == DialogResult.Yes)
                {
                    AssessmentSetupRecord rec = CurrentItem.Tag as AssessmentSetupRecord;
                    if (rec != null)
                        SaveAssessmentSetupToDB(rec.ID);
                    
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

            AssessmentNameEditor editor = new AssessmentNameEditor(CurrentItem.Tag as AssessmentSetupRecord);
            DialogResult dr = editor.ShowDialog();

            //if (dr == DialogResult.OK)
            //{
            //    try
            //    {
            //        EditTemplate.Rename(CurrentItem.Identity, editor.TemplateName);

            //        CurrentItem.TemplateName = editor.TemplateName;
            //        //CourseEntity.Instance.InvokeForeignTableChanged();
            //    }
            //    catch (Exception ex)
            //    {
            //        //CurrentUser.ReportError(ex);
            //        MsgBox.Show(ex.Message);
            //    }
            //}
        }

        private void BeforeLoadAssessmentSetup()
        {
            Loading = true;
            ipList.Items.Clear();
            //panel1.Enabled = false;
            btnAddNew.Enabled = false;
            btnSave.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void AfterLoadAssessmentSetup()
        {
            ipList.RecalcLayout();
            //panel1.Enabled = true;
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

        //private Template CreateTemplateObject(XmlElement templateData)
        //{
        //    Template item;
        //    string id = templateData.GetAttribute("ID");
        //    string name = templateData.SelectSingleNode("TemplateName").InnerText;

        //    item = new Template(id, name);
        //    item.AllowUpload = templateData.SelectSingleNode("AllowUpload").InnerText;
        //    item.StartTime = DateToDisplayFormat(templateData.SelectSingleNode("StartTime").InnerText);
        //    item.EndTime = DateToDisplayFormat(templateData.SelectSingleNode("EndTime").InnerText);

        //    item.OptionGroup = "TemplateItems";
        //    item.Click += new EventHandler(Template_Click);
        //    item.DoubleClick += new EventHandler(Template_DoubleClick);
        //    return item;
        //}

        //private _Exam CreateExamObject(XmlElement each)
        //{
        //    _Exam item = new _Exam();
        //    item.Identity = each.SelectSingleNode("@ID").InnerText;
        //    item.TemplateID = each.SelectSingleNode("ExamTemplateID").InnerText;
        //    item.ExamID = each.SelectSingleNode("RefExamID").InnerText;
        //    item.OpenTeacherAccess = each.SelectSingleNode("OpenTeacherAccess").InnerText;
        //    item.StartTime = DateToDisplayFormat(each.SelectSingleNode("StartTime").InnerText);
        //    item.EndTime = DateToDisplayFormat(each.SelectSingleNode("EndTime").InnerText);
        //    item.UseScore = each.SelectSingleNode("UseScore").InnerText;
        //    item.UseText = each.SelectSingleNode("UseText").InnerText;
        //    item.Weight = each.SelectSingleNode("Weight").InnerText;
        //    item.InputRequired = each.SelectSingleNode("InputRequired").InnerText;

        //    return item;
        //}

        private void ResetDirty()
        {
            foreach (DataGridViewRow row in dataview.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Tag = cell.Value;
            }
            //cboScoreSource.Tag = cboScoreSource.SelectedIndex;
            //txtStartTime.Tag = txtStartTime.Text;
            //txtEndTime.Tag = txtEndTime.Text;

            _has_deleted = false;
            _errors.Clear();

            if (CurrentItem == null)
                peTemplateName1.Text = string.Empty;
            else
                peTemplateName1.Text = CurrentItem.Text;

            ipt01.Tag = ipt01.Value;         
        }

        private bool IsDirty()
        {
            bool dirty = false;
            foreach (DataGridViewRow row in dataview.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                    dirty = dirty || (cell.Tag + string.Empty != cell.Value + string.Empty);
            }

            //dirty |= ((cboScoreSource.Tag == null ? "-1" : cboScoreSource.Tag) + string.Empty != cboScoreSource.SelectedIndex + string.Empty);
            //dirty |= (txtStartTime.Tag + string.Empty != txtStartTime.Text + string.Empty);
            //dirty |= (txtEndTime.Tag + string.Empty != txtEndTime.Text + string.Empty);

            return dirty || _has_deleted;
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
                    {
                        //cell.ErrorText = "您必須輸入小於(等於) 100 的數字。";
                        cell.ErrorText = "您必須輸入數值。";
                    }
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

            ShowDirtyStatus();
        }

        private void ShowDirtyStatus()
        {
            //if (IsDirty())
            //    peTemplateName1.Text = (CurrentItem.Tag as AssessmentSetupRecord).Name + DirtyMarkString;
            //else
            //    peTemplateName1.Text = (CurrentItem.Tag as AssessmentSetupRecord).Name;

            lblIsDirty.Visible = IsDirty();
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

        private void dataview_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            _has_deleted = true;
        }

        private DataGridViewCell GetCell(int row, int column)
        {
            return dataview.Rows[row].Cells[column];
        }

        private string GetColumnName(DataGridViewCell cell)
        {
            return dataview.Columns[cell.ColumnIndex].Name;
        }

        //private void txtStartTime_Validating(object sender, CancelEventArgs e)
        //{
        //    ValidTextTime(txtStartTime, PaddingMethod.First);
        //}

        //private void txtEndTime_Validating(object sender, CancelEventArgs e)
        //{
        //    ValidTextTime(txtEndTime, PaddingMethod.Last);
        //}

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

        //private void txtStartTime_Validated(object sender, EventArgs e)
        //{
        //    if (txtStartTime.Text != string.Empty)
        //        FormatDateTime(txtStartTime, PaddingMethod.First);

        //    CurrentItem.StartTime = txtStartTime.Text;

        //    ShowDirtyStatus();
        //}

        //private void txtEndTime_Validated(object sender, EventArgs e)
        //{
        //    if (txtEndTime.Text != string.Empty)
        //        FormatDateTime(txtEndTime, PaddingMethod.Last);

        //    CurrentItem.EndTime = txtEndTime.Text;

        //    ShowDirtyStatus();
        //}

        //private void cboScoreSource_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (CurrentItem == null) return;

        //    if (cboScoreSource.SelectedIndex == 0)
        //    {
        //        CurrentItem.AllowUpload = "是";
        //        txtStartTime.Enabled = true;
        //        txtEndTime.Enabled = true;
        //    }
        //    else
        //    {
        //        CurrentItem.AllowUpload = "否";
        //        txtStartTime.Enabled = false;
        //        txtEndTime.Enabled = false;
        //    }

        //    ShowDirtyStatus();
        //}

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
                _CheckSaveAssessmentSetup = true;
                //ReloadTempalte(CurrentItem);
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
                AssessmentSetupRecord record = CurrentItem.Tag as AssessmentSetupRecord;

                List<JHAEIncludeRecord> list = JHAEInclude.SelectByAssessmentSetupID(record.ID);
                int w = JHAEInclude.Delete(list);

                List<AEIncludeRecordEditor> editors = new List<AEIncludeRecordEditor>();
                //foreach (var item in record.GetAEIncludes())
                //{
                //    AEIncludeRecordEditor editor = item.GetEditor();
                //    editor.Remove = true;
                //    editors.Add(editor);
                //}
                //if (editors.Count > 0)
                //    editors.SaveAll();

                //editors = new List<AEIncludeRecordEditor>();
                list = new List<JHAEIncludeRecord>();
                foreach (DataGridViewRow each in dataview.Rows)
                {
                    if (each.IsNewRow) continue;

                    HC.JHAEIncludeRecord hc = new HC.JHAEIncludeRecord(new JHAEIncludeRecord());
                    hc.RefAssessmentSetupID = record.ID;
                    hc.RefExamID = "" + each.Cells[ExamID.Index].Value;
                    hc.UseScore = GetYesNoString(each.Cells[UseScore.Index].FormattedValue, false);
                    hc.UseText = GetYesNoString(each.Cells[UseText.Index].FormattedValue, false);
                    hc.UseAssignmentScore = GetYesNoString(each.Cells[UseAssignmentScore.Index].FormattedValue, false);
                    int i;
                    hc.Weight = int.TryParse("" + each.Cells[Weight.Index].Value, out i) ? i : 0;
                    hc.StartTime = DateToSaveFormat("" + each.Cells[StartTime.Index].Value);
                    hc.EndTime = DateToSaveFormat("" + each.Cells[EndTime.Index].Value);

                    list.Add(hc.AsJHAEIncludeRecord());
                }
                if (list.Count > 0)
                    JHAEInclude.Insert(list);

                ResetDirty();

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

        private void ipt01_ValueChanged(object sender, EventArgs e)
        {
            // 最多100
            lblpt02.Text = (100 - ipt01.Value) + " %";
            
            lblIsDirty.Visible = false;
            if (ipt01.Tag != null)
                if (ipt01.Value.ToString() != ipt01.Tag.ToString())
                    lblIsDirty.Visible = true;
            
          
        }
             
    }
}