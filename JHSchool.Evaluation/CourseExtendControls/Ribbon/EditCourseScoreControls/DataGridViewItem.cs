using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using FISCA.DSAUtil;
using JHSchool.Legacy;
using JHSchool.Evaluation.Feature.Legacy;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.EditCourseScoreControls
{
    internal partial class DataGridViewItem : PalmerwormItem
    {
        private DataGridViewHelper _helper;
        private Dictionary<string, bool> _showItems;

        public DataGridViewItem()
        {
            InitializeComponent();
            Title = "編輯成績";
        }

        public bool IsValid
        {
            get { return _helper.IsValid(); }
        }

        public override void Save()
        {
            // 產生新增成績 Request
            int insertStudentCount = 0;
            int updateStudentCount = 0;
            int deleteStudentCount = 0;
            int usCount = 0;
            
            DSXmlHelper insertHelper = new DSXmlHelper("Request");
            DSXmlHelper updateHelper = new DSXmlHelper("Request");
            DSXmlHelper deleteHelper = new DSXmlHelper("Request");
            DSXmlHelper usHelper = new DSXmlHelper("Request");

            insertHelper.AddElement("ScoreSheetList");
            updateHelper.AddElement("ScoreSheetList");
            deleteHelper.AddElement("ScoreSheet");

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string attendid = row.Tag.ToString();

                foreach (DataGridViewCell cell in row.Cells)
                {
                    IExamCell ic = cell.Tag as IExamCell;
                    if (ic == null) continue;
                    ColumnSetting setting = dataGridView1.Columns[cell.ColumnIndex].Tag as ColumnSetting;
                    string examid = setting.Key;
                    if (ic is ScoreExamCell && ic.IsDirty)
                    {
                        usCount++;
                        usHelper.AddElement("Attend");
                        usHelper.AddElement("Attend", "Score", ic.GetValue());
                        usHelper.AddElement("Attend", "ID", attendid);
                    }
                    else if (string.IsNullOrEmpty(ic.Key) && ic.IsDirty)
                    {
                        insertStudentCount++;
                        insertHelper.AddElement("ScoreSheetList", "ScoreSheet");
                        insertHelper.AddElement("ScoreSheetList/ScoreSheet", "ExamID", examid);
                        insertHelper.AddElement("ScoreSheetList/ScoreSheet", "AttendID", attendid);
                        insertHelper.AddElement("ScoreSheetList/ScoreSheet", "Score", ic.GetValue());
                    }
                    else if (!string.IsNullOrEmpty(ic.Key) && ic.IsDirty && !string.IsNullOrEmpty(ic.GetValue()))
                    {
                        updateStudentCount++;
                        updateHelper.AddElement("ScoreSheetList", "ScoreSheet");
                        updateHelper.AddElement("ScoreSheetList/ScoreSheet", "Score", ic.GetValue());
                        updateHelper.AddElement("ScoreSheetList/ScoreSheet", "ID", ic.Key);
                    }
                    else if (!string.IsNullOrEmpty(ic.Key) && ic.IsDirty && string.IsNullOrEmpty(ic.GetValue()))
                    {
                        deleteStudentCount++;
                        deleteHelper.AddElement("ScoreSheet", "ID", ic.Key);
                    }
                }
            }

            if (insertStudentCount > 0)
                EditCourse.InsertSCEScore(new DSRequest(insertHelper));
            if (updateStudentCount > 0)
                EditCourse.UpdateSCEScore(new DSRequest(updateHelper));
            if (deleteStudentCount > 0)
                EditCourse.DeleteSCEScore(new DSRequest(deleteHelper));
            if (usCount > 0)
                EditCourse.UpdateAttend(usHelper);

            SaveButtonVisible = false;
            LoadContent(RunningID);
        }

        protected override object OnBackgroundWorkerWorking()
        {
            return new SmartSchoolDataProvider(RunningID);
        }

        protected override void OnBackgroundWorkerCompleted(object result)
        {
            //因為這是第二個執行緒的 Callback，很可能使用者操作很快，所以在沒有執行這此時，畫面就被關了，不檢查可能會造成錯誤！
            if (IsDisposed) return;

            _valueManager.AddValue("IsDirty", false.ToString());
            IDataProvider provider = result as IDataProvider;
            _helper = new DataGridViewHelper(dataGridView1, provider);
            _helper.DirtyChanged += new EventHandler<DirtyChangedEventArgs>(_helper_DirtyChanged);
            _helper.Fill();

            if (_showItems == null)
            {                
                _showItems = new Dictionary<string, bool>();
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    _showItems.Add(column.Name, true);
                }                
            }
            // 設定顯示項目
            btnShowItems.SubItems.Clear();
            btnShowItems.AutoExpandOnClick = true;
            List<string> displayItem = new List<string>();
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                bool visible = _showItems[column.Name];
                CheckBoxItem item = new CheckBoxItem(column.Name, column.Name);
                btnShowItems.SubItems.Add(item);
                item.AutoCollapseOnClick = false;
                item.Checked = visible;
                item.CheckedChanged += new CheckBoxChangeEventHandler(item_CheckedChanged);
                if (visible) displayItem.Add(column.Name);
            }
            _helper.ResetDisplayColumn(displayItem);
        }

        void _helper_DirtyChanged(object sender, DirtyChangedEventArgs e)
        {
            OnValueChanged("IsDirty", e.Dirty.ToString());
        }

        void item_CheckedChanged(object sender, CheckBoxChangeEventArgs e)
        {
            List<string> displays = new List<string>();
            foreach (BaseItem item in btnShowItems.SubItems)
            {
                CheckBoxItem citem = (CheckBoxItem)item;
                if (citem.Checked)
                    displays.Add(citem.Text);
                _showItems[item.Text] = citem.Checked;
            }
            _helper.ResetDisplayColumn(displays);
        }

        public override void Undo()
        {
            _helper.ResetValue();
            _valueManager.AddValue("IsDirty", false.ToString());
            SaveButtonVisible = false;
        }
    }
}
