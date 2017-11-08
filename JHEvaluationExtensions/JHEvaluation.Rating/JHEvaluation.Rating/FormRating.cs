using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace JHEvaluation.Rating
{
    public partial class FormRating : BaseForm
    {
        internal List<RatingStudent> Students { get; private set; }

        private BackgroundWorker ScoreWorker;

        public FormRating()
        {
            InitializeComponent();
            Students = new List<RatingStudent>();

            ScoreWorker = new BackgroundWorker();
            ScoreWorker.DoWork += delegate
            {
                PrepareDataBackground();
            };
            ScoreWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                PrepareDataComplete(e.Error);
            };

        }

        /// <summary>
        /// 將需要排名的學生指定到表單中。
        /// </summary>
        public void SetRatingStudents(IEnumerable<string> studentIds)
        {
            Students = RatingUtils.ToRatingStudent(studentIds);
            DisplaySelectedStudentCount();
        }

        protected virtual void DisplaySelectedStudentCount()
        {
            Text = "你沒有蓋掉啦! (DisplaySelectedStudentCount)";
        }

        protected void PrepareData()
        {
            ScoreWorker.RunWorkerAsync();
        }

        protected virtual void PrepareDataBackground()
        {
            throw new NotImplementedException();
        }

        protected virtual void PrepareDataComplete(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
