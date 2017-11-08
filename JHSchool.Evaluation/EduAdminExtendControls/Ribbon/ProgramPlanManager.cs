using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.Threading;
using System.Xml;
using FISCA.DSAUtil;
using JHSchool.Evaluation.Editor;
using Framework;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.ProgramPlanRelated;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon
{
    public partial class ProgramPlanManager : FISCA.Presentation.Controls.BaseForm
    {
        BackgroundWorker _loader;
        ButtonItem _selected_item;

        public ProgramPlanManager()
        {
            InitializeComponent();

            ProgramPlan.Instance.ItemLoaded += delegate
            {
                pictureBox1.Visible = false;
                btnAdd.Enabled = btnDelete.Enabled = true;
                LoadItems();
            };
            ProgramPlan.Instance.ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            {
                if (e.PrimaryKeys.Count > 0)
                    RefreshItemPanel(e.PrimaryKeys[0]);
            };
            graduationPlanEditor1.IsDirtyChanged += delegate
            {
                btnSave.Enabled = lblSaveWarning.Visible = graduationPlanEditor1.IsDirty;
            };

            pictureBox1.Visible = true;
            graduationPlanEditor1.Enabled = false;
            labelX1.Text = labelX2.Text = "";

            _loader = new BackgroundWorker();
            _loader.DoWork += delegate { ProgramPlan.Instance.SyncAllBackground(); };
            _loader.RunWorkerAsync();
        }

        private void RefreshItemPanel(string id)
        {
            ProgramPlanRecord record = ProgramPlan.Instance.Items[id];
            ButtonItem updateItem = null;

            foreach (ButtonItem item in itemPanel1.Items)
            {
                ProgramPlanRecord r = item.Tag as ProgramPlanRecord;
                if (r.ID == id)
                {
                    updateItem = item;
                    break;
                }
            }

            if(record != null && updateItem == null) //Insert
            {
                ButtonItem item = new ButtonItem();
                item.Text = record.Name;
                item.Tag = record;
                item.OptionGroup = "ProgramPlan";
                item.Click += new EventHandler(item_Click);
                itemPanel1.Items.Add(item);
                item.RaiseClick();
            }
            else if(record != null && updateItem != null) //Update
            {
                updateItem.Tag = record;
                updateItem.RaiseClick();
            }
            else if (record == null && updateItem != null) //Delete
            {
                updateItem.Click -= new EventHandler(item_Click);
                itemPanel1.Items.Remove(updateItem);
                graduationPlanEditor1.Enabled = false;
                graduationPlanEditor1.SetSource(null);
                btnSave.Enabled = lblSaveWarning.Visible = false;
                labelX1.Text = labelX2.Text = "";
            }

            itemPanel1.Refresh();
            itemPanel1.EnsureVisible(itemPanel1.Items[itemPanel1.Items.Count - 1]);
        }

        private void item_Click(object sender, EventArgs e)
        {
            ButtonItem item = sender as ButtonItem;
            graduationPlanEditor1.Enabled = true;
            labelX1.Text = labelX2.Text = item.Text;
            btnSave.Enabled = lblSaveWarning.Visible = false;
            _selected_item = item;
            graduationPlanEditor1.SetSource(ConvertToXml(item.Tag as ProgramPlanRecord));
            SetListView(item.Tag as ProgramPlanRecord);
        }

        private void LoadItems()
        {
            _selected_item = null;

            itemPanel1.SuspendLayout();
            itemPanel1.Items.Clear();

            List<ButtonItem> itemList = new List<ButtonItem>();
            foreach (var record in ProgramPlan.Instance.Items)
            {
                ButtonItem item = new ButtonItem();
                item.Text = record.Name;
                item.Tag = record;
                item.OptionGroup = "ProgramPlan";
                item.Click += new EventHandler(item_Click);
                itemList.Add(item);
            }

            itemList.Sort(ItemComparer);
            foreach (var item in itemList)
                itemPanel1.Items.Add(item);

            itemPanel1.ResumeLayout();
            itemPanel1.Refresh();
        }

        private int ItemComparer(ButtonItem a, ButtonItem b)
        {
            return a.Text.CompareTo(b.Text);
        }

        private void SetListView(ProgramPlanRecord record)
        {
            listViewEx1.SuspendLayout();
            listViewEx1.Items.Clear();
            listViewEx1.Groups.Clear();
            Dictionary<ClassRecord, int> classCount = new Dictionary<ClassRecord, int>();
            List<StudentRecord> noClassStudents = new List<StudentRecord>();
            foreach (StudentRecord stu in Student.Instance.Items)
            {
                if (stu.GetProgramPlanRecord() == record)
                {
                    if (stu.Class != null)
                    {
                        if (!classCount.ContainsKey(stu.Class))
                            classCount.Add(stu.Class, 0);
                        classCount[stu.Class]++;
                    }
                    else
                        noClassStudents.Add(stu);
                }
            }
            foreach (ClassRecord cla in classCount.Keys)
            {
                string groupKey;
                int a;
                if (int.TryParse(cla.GradeYear, out a))
                {
                    groupKey = cla.GradeYear + "　年級";
                }
                else
                    groupKey = cla.GradeYear;
                ListViewGroup group = listViewEx1.Groups[groupKey];
                if (group == null)
                    group = listViewEx1.Groups.Add(groupKey, groupKey);
                listViewEx1.Items.Add(new ListViewItem(cla.Name + "(" + classCount[cla] + ")　", 0, group));
            }
            if (noClassStudents.Count > 0)
            {
                ListViewGroup group = listViewEx1.Groups["未分班"];
                if (group == null)
                    group = listViewEx1.Groups.Add("未分班", "未分班");
                foreach (StudentRecord stu in noClassStudents)
                {
                    listViewEx1.Items.Add(new ListViewItem(stu.Name + "[" + stu.StudentNumber + "] 　", 1, group));
                }
            }
            listViewEx1.ResumeLayout();
        }

        private XmlElement ConvertToXml(ProgramPlanRecord record)
        {
            DSXmlHelper helper = new DSXmlHelper("GraduationPlan");

            foreach (var subject in record.Subjects)
            {
                XmlElement element = null;

                element = helper.AddElement("Subject");
                element.SetAttribute("GradeYear", subject.GradeYear);
                element.SetAttribute("Semester", subject.Semester);
                element.SetAttribute("Credit", subject.Credit);
                element.SetAttribute("Period", subject.Period);
                element.SetAttribute("Domain", subject.Domain);
                element.SetAttribute("FullName", subject.FullName);
                element.SetAttribute("Level", subject.Level);
                element.SetAttribute("CalcFlag", "" + subject.CalcFlag);
                element.SetAttribute("SubjectName", subject.SubjectName);

                element = helper.AddElement("Subject", "Grouping");
                element.SetAttribute("RowIndex", "" + subject.RowIndex);
            }

            return helper.BaseElement;

            //<Subject Credit="2" Domain="外國語文" FullName="ESL" Level="" NotIncludedInCalc="False" SubjectName="ESL">
            //    <Grouping RowIndex="1" />
            //</Subject>
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selected_item == null) return;

            if (MsgBox.Show("您確定要刪除 '" + _selected_item.Text + "' 嗎？", "刪除課程規劃表", MessageBoxButtons.YesNo) == DialogResult.No) return;

            ProgramPlanRecordEditor editor = (_selected_item.Tag as ProgramPlanRecord).GetEditor();
            editor.Remove = true;
            editor.Save();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            new GraduationPlanCreator().ShowDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_selected_item == null) return;
            if (graduationPlanEditor1.IsValidated == false)
            {
                MsgBox.Show("課程規劃表內容有錯誤，請先修正後再儲存。");
                return;
            }

            ProgramPlanRecordEditor editor = (_selected_item.Tag as ProgramPlanRecord).GetEditor();
            editor.Subjects = GetSubjectsFromXml(graduationPlanEditor1.GetSource());
            editor.Save();

            //_selected_item.Tag = ProgramPlan.Instance.Items[editor.ID];

            //graduationPlanEditor1.SetSource(ConvertToXml(_selected_item.Tag as ProgramPlanRecord));

            btnSave.Enabled = lblSaveWarning.Visible = false;
        }

        private List<ProgramSubject> GetSubjectsFromXml(XmlElement source)
        {
            List<ProgramSubject> list = new List<ProgramSubject>();

            foreach (XmlElement element in source.SelectNodes("Subject"))
                list.Add(new ProgramSubject(element));

            return list;
        }
    }
}
