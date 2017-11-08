using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using FISCA.UDT;
using KaoHsiung.ReaderScoreImport.UDT;
using KaoHsiung.ReaderScoreImport.Mapper;

namespace KaoHsiung.ReaderScoreImport
{
    public partial class ExamCodeConfig : ConfigForm
    {
        //private List<JHExamRecord> _exams;
        private List<ExamCode> _list;

        public ExamCodeConfig()
        {
            InitializeComponent();
            //Init();
            Startup();
        }

        //private void Init()
        //{
        //    _exams = new List<JHExamRecord>();
        //}

        protected override void DoWork(object sender, DoWorkEventArgs e)
        {
            _list = _accessHelper.Select<ExamCode>();
            //if (_list.Count <= 0)
            //    _exams = JHExam.SelectAll();
        }

        protected override void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dgv.SuspendLayout();

            //foreach (var exam in _exams)
            //{
            //    DataGridViewRow row = new DataGridViewRow();
            //    row.CreateCells(dgv, exam.Name, "");

            //    dgv.Rows.Add(row);
            //    ValidateCodeFormat(row.Cells[chCode.Index],2);
            //}

            foreach (var record in _list)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, "" + record.ExamName, "" + record.Code);

                dgv.Rows.Add(row);
                ValidateCodeFormat(row.Cells[chCode.Index],2);
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

            List<ExamCode> newList = new List<ExamCode>();
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                ExamCode ec = new ExamCode();
                ec.ExamName = "" + row.Cells[chExamName.Index].Value;
                ec.Code = "" + row.Cells[chCode.Index].Value;
                newList.Add(ec);
            }
            _accessHelper.DeletedValues(_list.ToArray());
            _accessHelper.InsertValues(newList.ToArray());
            ExamCodeMapper.Instance.Reload();

            this.DialogResult = DialogResult.OK;
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (cell.OwningColumn == chCode)
            {
                ValidateCodeFormat(cell,2);
                ValidateDuplication(dgv, chCode.Index, "代碼不能重覆");
            }
            else
            {
                ValidateDuplication(dgv, chExamName.Index, "試別名稱不能重覆");
            }
        }

        protected override void Export()
        {
            ImportExport.Export(dgv, "匯出試別代碼設定");
        }

        protected override void Import()
        {
            ImportExport.Import(dgv, new List<string>() { "試別", "代碼" });

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                DataGridViewCell cell = row.Cells[chCode.Index];
                ValidateCodeFormat(cell,2);
            }
            ValidateDuplication(dgv, chCode.Index, "代碼不能重覆");
            ValidateDuplication(dgv, chExamName.Index, "試別名稱不能重覆");
        }
    }
}
