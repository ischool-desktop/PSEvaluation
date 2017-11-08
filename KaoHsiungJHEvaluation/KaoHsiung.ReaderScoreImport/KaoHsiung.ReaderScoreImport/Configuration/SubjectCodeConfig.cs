using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using KaoHsiung.ReaderScoreImport.UDT;
using KaoHsiung.ReaderScoreImport.Mapper;

namespace KaoHsiung.ReaderScoreImport
{
    public partial class SubjectCodeConfig : ConfigForm
    {
        //private List<JHCourseRecord> _courses;
        private List<SubjectCode> _list;

        public SubjectCodeConfig()
        {
            InitializeComponent();
            //Init();
            Startup();
        }

        //private void Init()
        //{
        //    _courses = new List<JHCourseRecord>();
        //}

        protected override void DoWork(object sender, DoWorkEventArgs e)
        {
            _list = _accessHelper.Select<SubjectCode>();
            //if (_list.Count <= 0)
            //    _courses = JHCourse.SelectAll();
        }

        protected override void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dgv.SuspendLayout();

            //List<string> dsKey = new List<string>();
            //foreach (var course in _courses)
            //{
            //    if (string.IsNullOrEmpty(course.Domain)) continue;
            //    if (string.IsNullOrEmpty(course.Subject)) continue;

            //    string ds = course.Domain + "_ischool_" + course.Subject;
            //    if (!dsKey.Contains(ds))
            //    {
            //        dsKey.Add(ds);

            //        DataGridViewRow row = new DataGridViewRow();
            //        row.CreateCells(dgv, course.Domain, course.Subject, "");

            //        dgv.Rows.Add(row);
            //        ValidateCodeFormat(row.Cells[chCode.Index], 6);
            //    }
            //}

            foreach (var record in _list)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, record.Subject, record.Code);

                dgv.Rows.Add(row);
                ValidateCodeFormat(row.Cells[chCode.Index], 5);
            }

            dgv.ResumeLayout();
        }

        protected override void Save()
        {
            dgv.EndEdit();

            if (!IsValid(dgv))
            {
                MsgBox.Show("請先修正錯誤");
                return;
            }

            List<SubjectCode> newList = new List<SubjectCode>();
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                SubjectCode sc = new SubjectCode();
                //sc.Domain = "" + row.Cells[chDomain.Index].Value;
                sc.Subject = "" + row.Cells[chSubject.Index].Value;
                sc.Code = "" + row.Cells[chCode.Index].Value;
                newList.Add(sc);
            }
            _accessHelper.DeletedValues(_list.ToArray());
            _accessHelper.InsertValues(newList.ToArray());
            SubjectCodeMapper.Instance.Reload();

            this.DialogResult = DialogResult.OK;
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (cell.OwningColumn == chCode)
            {
                ValidateCodeFormat(cell,5);
                ValidateDuplication(dgv, chCode.Index, "代碼不能重覆");
            }
            else if (cell.OwningColumn == chSubject)
            {
                ValidateDuplication(dgv, chSubject.Index, "科目不能重覆");
            }
        }

        protected override void Export()
        {
            ImportExport.Export(dgv, "匯出科目代碼設定");
        }

        protected override void Import()
        {
            ImportExport.Import(dgv, new List<string>() { "科目", "代碼" });

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                DataGridViewCell cell = row.Cells[chCode.Index];
                ValidateCodeFormat(cell,5);
            }
            ValidateDuplication(dgv, chCode.Index, "代碼不能重覆");
            ValidateDuplication(dgv, chSubject.Index, "科目不能重覆");
        }
    }
}
