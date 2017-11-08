using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord.DAL;
using JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord.Data;

namespace JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord.Forms
{
    public partial class StudentActivityRecordItem : DetailContentBase
    {
        public StudentActivityRecordItem()
        {
            InitializeComponent();
            this.Group = "個人活動表現紀錄";

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                ActivityRecordDAL.PrepareMetadata();
            };
            worker.RunWorkerAsync();
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {            
            Fill();
        }

        private void Fill()
        {
            if (string.IsNullOrEmpty(this.PrimaryKey)) return;
            
            List<ActivityRecordItem> list = ActivityRecordDAL.GetActivityRecordHistory(this.PrimaryKey);

            list.Sort(new ActivityRecordItemComparer());

            dgView.Rows.Clear();
            foreach (ActivityRecordItem item in list)
            {
                int rowIndex = dgView.Rows.Add();
                DataGridViewRow row = dgView.Rows[rowIndex];

                row.Cells[colItem.Name].Value = item.Item;
                row.Cells[this.colSchoolYear.Name].Value = item.SchoolYear;
                row.Cells[this.colSemester.Name].Value = item.Semester;
                row.Cells[this.colType.Name].Value = item.Type;
            }
        }
    }

    class ActivityRecordItemComparer : IComparer<ActivityRecordItem>
    {
        #region IComparer<ActivityRecordItem> 成員

        public int Compare(ActivityRecordItem x, ActivityRecordItem y)
        {
            int xy, yy;

            if (!int.TryParse(x.SchoolYear, out xy))
                xy = 0;

            if (!int.TryParse(y.SchoolYear, out yy))
                yy = 0;

            if (xy == yy)
                return x.Semester.CompareTo(y.Semester);
            else
                return xy.CompareTo(yy);
        }

        #endregion
    }

}
