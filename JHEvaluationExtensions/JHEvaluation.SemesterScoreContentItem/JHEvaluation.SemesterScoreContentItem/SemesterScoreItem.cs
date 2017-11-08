using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using JHSchool.Evaluation.Editor;
using FCode = Framework.Security.FeatureCodeAttribute;
using JHSchool.Data;
using JHSchool;
using JHEvaluation.SemesterScoreContentItem.Forms;
using JHEvaluation.SemesterScoreContentItem.QuickAdd;
using System.Threading;

namespace JHEvaluation.SemesterScoreContentItem
{
    [FCode("JHSchool.Student.Detail0050", "學期成績")]
    public partial class SemesterScoreItem : DetailContent
    {
        private BackgroundWorker _worker;
        private List<JHSemesterScoreRecord> _recordList;
        private string _RunningID;
        private JHStudentRecord _student;

        private List<string> _domainList;

        internal static Framework.Security.FeatureAce UserPermission;

        public SemesterScoreItem()
        {
            InitializeComponent();
            InitializeQuickAddButton();

            _domainList = new List<string>();
            InitializeColumnHeader();

            UserPermission = Framework.User.Acl[FCode.GetCode(GetType())];

            btnAdd.Visible = UserPermission.Editable;
            btnModify.Visible = UserPermission.Editable;
            btnDelete.Visible = UserPermission.Editable;
            btnView.Visible = UserPermission.Viewable & !UserPermission.Editable;

            _worker = new BackgroundWorker();
            _worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                if (_student == null)
                    _student = JHStudent.SelectByID("" + e.Argument);
                else if (_student.ID != "" + e.Argument)
                    _student = JHStudent.SelectByID("" + e.Argument);

                e.Result = JHSemesterScore.SelectByStudentID("" + e.Argument);
            };
            _worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                if (_RunningID != PrimaryKey)
                {
                    _RunningID = PrimaryKey;
                    _worker.RunWorkerAsync(_RunningID);
                    return;
                }

                _recordList = e.Result as List<JHSemesterScoreRecord>;
                FillListView();
            };

            FISCA.InteractionService.SubscribeEvent("CalculationHelper.SaveSemesterScore", (sender, args) => {
                AfterSaveSemesterScore();
            });
        }

        private void InitializeQuickAddButton()
        {
            //if (Global.Params["Mode"] == "KaoHsiung")
            {
                DevComponents.DotNetBar.ButtonItem btnQuick = new DevComponents.DotNetBar.ButtonItem();
                btnQuick.Text = "快速新增";

                btnQuick.Click += delegate
                {
                    JHSchool.SF.Evaluation.QuickInputSemesterScoreForm.ShowDialog(_student.ID);
                };
                //btnQuick.Click += delegate
                //{
                //    new KaoHsiungQuickAdd().ShowDialog();
                //};
                btnAdd.SubItems.Add(btnQuick);
            }
        }

        private void InitializeColumnHeader()
        {
            // 高雄也需要加入一個由國語文+英語計算出語文領域
            List<string> DomainList = new List<string>();
            DomainList.Add("語文");
            foreach (string str in JHSchool.Evaluation.Subject.Domains)
                if (!DomainList.Contains(str))
                    DomainList.Add(str);
                
            
            //一般領域
            foreach (string each in DomainList)
            {
                ColumnHeader ch = new ColumnHeader();
                ch.Name = "ch" + each;
                ch.Text = each;
                ch.Width = GetColumnHeaderWidth(each);
                listView.Columns.Add(ch);
                _domainList.Add(each);
            }

            //學習領域總成績
            ColumnHeader chScore = new ColumnHeader();
            chScore.Name = "ch" + "學習領域總成績";
            chScore.Text = "學習領域總成績";
            chScore.Width = GetColumnHeaderWidth("學習領域總成績");
            listView.Columns.Add(chScore);
        }

        private int GetColumnHeaderWidth(string text)
        {
            return (text.Length - 1) * 13 + 31; //神奇的欄位寬度計算…
        }

        private void FillListView()
        {
            listView.Items.Clear();
            if (_recordList == null) return;

            picLoading.Visible = false;

            _recordList.Sort(delegate(JHSemesterScoreRecord a, JHSemesterScoreRecord b)
            {
                if (a.SchoolYear == b.SchoolYear)
                    return a.Semester.CompareTo(b.Semester);
                return a.SchoolYear.CompareTo(b.SchoolYear);
            });

            //記錄哪些領域沒有成績
            Dictionary<string, bool> no_score_domains = new Dictionary<string, bool>();
            foreach (string domain in _domainList)
                no_score_domains.Add(domain, false);

            foreach (var record in _recordList)
            {
                ListViewItem item = new ListViewItem();
                item.Tag = record;

                listView.Items.Add(item);

                item.Text = "" + record.SchoolYear;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "" + record.Semester));

                foreach (string domain in _domainList)
                {
                    ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(item, string.Empty);
                    if (record.Domains.ContainsKey(domain) && !string.IsNullOrEmpty("" + record.Domains[domain].Score))
                    {
                        subItem.Text = "" + record.Domains[domain].Score;
                        no_score_domains[domain] = true;
                    }
                    item.SubItems.Add(subItem);
                }

                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "" + record.LearnDomainScore));
            }

            //將沒有成績的領域從畫面中拿掉
            foreach (string domain in no_score_domains.Keys)
            {
                string ch = "ch" + domain;
                int width = 0;

                if (no_score_domains[domain]) width = GetColumnHeaderWidth(domain);

                if (listView.Columns.ContainsKey("ch" + domain))
                    listView.Columns["ch" + domain].Width = width;
            }
        }

        #region IDetailBulider 成員

        public DetailContent GetContent()
        {
            return new SemesterScoreItem();
        }

        #endregion

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            LoadSemesterScores();
            base.OnPrimaryKeyChanged(e);
        }

        private void LoadSemesterScores()
        {
            btnModify.Enabled = btnDelete.Enabled = false;

            if (!_worker.IsBusy)
            {
                _recordList = null;
                _RunningID = PrimaryKey;
                _worker.RunWorkerAsync(_RunningID);
                picLoading.Visible = true;
            }
        }

        //修改
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count <= 0) return;

            SemesterScoreEditor form = new SemesterScoreEditor(JHStudent.SelectByID(PrimaryKey), listView.SelectedItems[0].Tag as JHSemesterScoreRecord);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadSemesterScores();
            }

            listView.Focus();
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            //btnModify_Click(sender, e);
            if (listView.SelectedItems.Count <= 0) return;

            SemesterScoreEditor form = new SemesterScoreEditor(JHStudent.SelectByID(PrimaryKey), listView.SelectedItems[0].Tag as JHSemesterScoreRecord, false);
            form.ShowDialog();
        }

        //刪除
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count <= 0) return;
            ListViewItem item = listView.SelectedItems[0];

            string info = string.Format("{0}學年度 第{1}學期", item.SubItems[0].Text, item.SubItems[1].Text);
            if (Framework.MsgBox.Show("您確定要刪除「" + item.SubItems[0].Text + "學年度 第" + item.SubItems[1].Text + "學期」的學期成績嗎？", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                JHSemesterScoreRecord record = item.Tag as JHSemesterScoreRecord;
                JHSemesterScore.Delete(record);
                FISCA.LogAgent.ApplicationLog.Log("成績系統.學期成績", "刪除學期成績", "student", PrimaryKey, string.Format("{0}，刪除「{1}」的學期成績", StudentInfoConvertor.GetInfoWithClass(_student), info));

                listView.Items.Remove(listView.SelectedItems[0]);
                listView.Refresh();
            }

            listView.Focus();
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnModify.Enabled = btnDelete.Enabled = (listView.SelectedItems.Count > 0);
        }

        //一般新增
        private void btnNornalAdd_Click(object sender, EventArgs e)
        {
            SemesterScoreEditor form = new SemesterScoreEditor(JHStudent.SelectByID(PrimaryKey));
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadSemesterScores();
            }

            listView.Focus();
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            if (UserPermission.Editable)
                btnModify_Click(null, null);
        }

        private void AfterSaveSemesterScore()
        {
            if (InvokeRequired)
                Invoke(new Action(AfterSaveSemesterScore));
            else
            {
                this.OnPrimaryKeyChanged(EventArgs.Empty);
            }
        }
    }
}
