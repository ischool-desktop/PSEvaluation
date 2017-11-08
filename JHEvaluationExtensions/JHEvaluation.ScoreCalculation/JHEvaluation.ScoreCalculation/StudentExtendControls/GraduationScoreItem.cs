using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using Framework;
using JHSchool.Evaluation.Editor;

namespace JHSchool.Evaluation.StudentExtendControls
{
    [Framework.Security.FeatureCode("JHSchool.Student.Detail0051", "畢業成績")]
    public partial class GraduationScoreItem : DetailContent
    {
        private BackgroundWorker _worker;
        private ChangeListener _listener;
        private GradScoreRecord _record;
        private string _RunningID;

        public GraduationScoreItem()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            InitializeDomains();

            _listener = new ChangeListener();
            _listener.Add(new DataGridViewSource(dgv));

            _listener.StatusChanged += new EventHandler<ChangeEventArgs>(Listener_StatusChanged);
        }

        private void InitializeDomains()
        {
            foreach (string each in Domain.SelectGeneral())
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, each, string.Empty);
                dgv.Rows.Add(row);
            }

            DataGridViewRow elasticRow = new DataGridViewRow();
            elasticRow.CreateCells(dgv, "彈性課程", string.Empty);
            dgv.Rows.Add(elasticRow);

            foreach (string each in Domain.SelectSpecial())
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, each, string.Empty);
                dgv.Rows.Add(row);
            }

            DataGridViewRow learnDomainRow = new DataGridViewRow();
            learnDomainRow.CreateCells(dgv, "學習領域", string.Empty);
            learnDomainRow.Tag = "LearnDomain";
            dgv.Rows.Add(learnDomainRow);

            DataGridViewRow courseLearnRow = new DataGridViewRow();
            courseLearnRow.CreateCells(dgv, "課程學習", string.Empty);
            courseLearnRow.Tag = "CourseLearn";
            dgv.Rows.Add(courseLearnRow);
        }

        private void InitializeBackgroundWorker()
        {
            _worker = new BackgroundWorker();
            _worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                GradScore.Instance.SyncDataBackground(PrimaryKey);
                e.Result = GradScore.Instance.Items[PrimaryKey];
            };
            _worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                if (_RunningID != PrimaryKey)
                {
                    _RunningID = PrimaryKey;
                    _worker.RunWorkerAsync(_RunningID);
                    return;
                }

                if (e.Result != null)
                {
                    _record = e.Result as GradScoreRecord;
                    FillData();
                }
            };
        }

        private void Listener_StatusChanged(object sender, ChangeEventArgs e)
        {
            SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            dgv.EndEdit();
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            if (!_worker.IsBusy)
            {
                _RunningID = PrimaryKey;
                _worker.RunWorkerAsync();
            }
            base.OnPrimaryKeyChanged(e);
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            dgv.EndEdit();

            if (!IsValid())
            {
                MsgBox.Show("請先修正錯誤。");
                return;
            }

            GradScoreRecordEditor editor;
            if (_record != null)
                editor = _record.GetEditor();
            else
                editor = new GradScoreRecordEditor(Student.Instance.Items[PrimaryKey]);

            foreach (DataGridViewRow row in dgv.Rows)
            {
                string domain = "" + row.Cells[chDomain.Index].Value;
                string value = "" + row.Cells[chScore.Index].Value;

                if ("" + row.Tag == "LearnDomain")
                {
                    if (!string.IsNullOrEmpty(value)) editor.LearnDomainScore = decimal.Parse(value);
                    else editor.LearnDomainScore = null;
                }
                else if ("" + row.Tag == "CourseLearn")
                {
                    if (!string.IsNullOrEmpty(value)) editor.CourseLearnScore = decimal.Parse(value);
                    else editor.CourseLearnScore = null;
                }
                else
                {
                    if (!editor.Domains.ContainsKey(domain))
                        editor.Domains.Add(domain, new GradDomainScore(domain));

                    if (!string.IsNullOrEmpty(value)) editor.Domains[domain].Score = decimal.Parse(value);
                    else editor.Domains[domain].Score = null;
                }
            }

            //foreach (TextBox txt in _txtBoxes)
            //{
            //    string domain = "" + txt.Tag;
            //    if (!editor.Domains.ContainsKey(domain))
            //        editor.Domains.Add(domain, new GradDomainScore(domain));

            //    if (!string.IsNullOrEmpty(txt.Text))
            //        editor.Domains[domain].Score = decimal.Parse(txt.Text);
            //}

            //if (!string.IsNullOrEmpty(txtLearnDomain.Text))
            //    editor.LearnDomainScore = decimal.Parse(txtLearnDomain.Text);

            //if (!string.IsNullOrEmpty(txtCourseLearn.Text))
            //    editor.CourseLearnScore = decimal.Parse(txtCourseLearn.Text);

            editor.Save();
            SaveLog(editor);
            _listener.Reset();
            SaveButtonVisible = CancelButtonVisible = false;

            base.OnSaveButtonClick(e);
        }

        private void SaveLog(GradScoreRecordEditor editor)
        {
            FISCA.LogAgent.ApplicationLog.Log("成績系統.畢業成績", "修改畢業成績", "student", PrimaryKey, GetStudentInfo(JHSchool.Data.JHStudent.SelectByID(PrimaryKey)) + " 修改畢業成績");
        }

        private string GetStudentInfo(JHSchool.Data.JHStudentRecord student)
        {
            string info = string.Empty;
            StringBuilder builder = new StringBuilder("");
            if (student.Class != null)
            {
                builder.Append(student.Class.Name);
                if (!string.IsNullOrEmpty("" + student.SeatNo))
                    builder.Append("(" + student.SeatNo + "號)");
                builder.Append(" ");
            }
            if (!string.IsNullOrEmpty(student.StudentNumber))
                builder.Append(student.StudentNumber + " ");
            if (string.IsNullOrEmpty(builder.ToString()))
                builder.Append("學生：");
            builder.Append(student.Name);
            return builder.ToString();
        }

        private bool IsValid()
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                DataGridViewCell cell = row.Cells[chScore.Index];
                if (!string.IsNullOrEmpty(cell.ErrorText)) return false;
            }
            return true;
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            if (!_worker.IsBusy)
            {
                _RunningID = PrimaryKey;
                _worker.RunWorkerAsync();
            }
            CancelButtonVisible = false;
            SaveButtonVisible = false;
            base.OnCancelButtonClick(e);
        }

        private void FillData()
        {
            _listener.SuspendListen();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                DataGridViewCell cell = row.Cells[chScore.Index];
                cell.ErrorText = string.Empty;

                if ("" + row.Tag == "LearnDomain")
                    cell.Value = "" + _record.LearnDomainScore;
                else if ("" + row.Tag == "CourseLearn")
                    cell.Value = "" + _record.CourseLearnScore;
                else
                {
                    string domain = "" + row.Cells[chDomain.Index].Value;
                    if (_record.Domains.ContainsKey(domain))
                        cell.Value = "" + _record.Domains[domain].Score;
                    else
                        cell.Value = string.Empty;
                }
            }

            _listener.Reset();
            _listener.ResumeListen();
        }

        private void ValidCell(DataGridViewCell cell)
        {
            string value = "" + cell.Value;
            cell.ErrorText = string.Empty;
            if (string.IsNullOrEmpty(value)) return;

            decimal d;
            if (!decimal.TryParse(value, out d))
                cell.ErrorText = "分數必須為數字";
        }

        private void dgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgv.EndEdit();
            ValidCell(dgv.CurrentCell);
            dgv.BeginEdit(false);
        }
    }
}
