using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Framework;

namespace KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizard.Forms
{
    internal partial class CalculateProgress : FISCA.Presentation.Controls.BaseForm, IProgressUI
    {
        private BackgroundWorker _worker;
        private CourseCollection _courses;
        private bool _clear_data = false;

        public CalculateProgress(CourseCollection courses)
        {
            InitializeComponent();
            InitializeBackgroundWorker();

            _clear_data = false;
            _courses = courses;
        }

        public bool ClearData
        {
            get { return _clear_data; }
            set { _clear_data = value; }
        }

        private void CalculateProgress_Load(object sender, EventArgs e)
        {
            if (ClearData)
                Text = "清除課程成績資料";
            else
                Text = "儲存課程成績資料";

            int count = 0;
            foreach (Course course in _courses.Values)
            {
                count += course.SCAttends.Count;
            }
            Progress.Minimum = 0;
            Progress.Maximum = count;

            _worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            CourseScoreUpdater updater = new CourseScoreUpdater(_courses, this, ClearData);
            updater.UpdateToServer();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress.Value = e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
                DialogResult = DialogResult.OK;
            else
            {
                MsgBox.Show("儲存資料失敗，" + e.Error.Message);
                DialogResult = DialogResult.Cancel;
            }
        }

        #region IProgressUI 成員

        public void ReportProgress(string message, int progress)
        {
            _worker.ReportProgress(progress, message);
        }

        public void Cancel()
        {
        }

        public bool Cancellation
        {
            get { return false; }
        }

        #endregion

        private void InitializeBackgroundWorker()
        {
            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.WorkerReportsProgress = true;
        }
    }
}