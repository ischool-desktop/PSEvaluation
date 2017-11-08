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
using FISCA.UDT;
using KaoHsiung.ReaderScoreImport.UDT;
using KaoHsiung.ReaderScoreImport.Mapper;

namespace KaoHsiung.ReaderScoreImport
{
    public partial class ClassCodeConfig : ConfigForm
    {
        //private List<JHClassRecord> _classes;
        private List<ClassCode> _list;

        public ClassCodeConfig()
        {
            InitializeComponent();
            //Init();
            Startup();
        }

        //private void Init()
        //{
        //    _classes = new List<JHClassRecord>();
        //}

        protected override void DoWork(object sender, DoWorkEventArgs e)
        {
            _list = _accessHelper.Select<ClassCode>();
            //if (_list.Count <= 0)
            //    _classes = JHClass.SelectAll();
        }

        protected override void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dgv.SuspendLayout();

            //foreach (var cla in _classes)
            //{
            //    DataGridViewRow row = new DataGridViewRow();
            //    row.CreateCells(dgv, cla.Name, "");

            //    dgv.Rows.Add(row);
            //    ValidateCodeFormat(row.Cells[chCode.Index],3);
            //}

            foreach (var record in _list)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, record.ClassName, record.Code);

                dgv.Rows.Add(row);
                ValidateCodeFormat(row.Cells[chCode.Index],3);
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

            List<ClassCode> newList = new List<ClassCode>();
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                ClassCode cc = new ClassCode();
                cc.ClassName = "" + row.Cells[chClassName.Index].Value;
                cc.Code = "" + row.Cells[chCode.Index].Value;
                newList.Add(cc);
            }
            _accessHelper.DeletedValues(_list.ToArray());
            _accessHelper.InsertValues(newList.ToArray());
            ClassCodeMapper.Instance.Reload();

            this.DialogResult = DialogResult.OK;
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (cell.OwningColumn == chCode)
            {
                ValidateCodeFormat(cell,3);
                ValidateDuplication(dgv, chCode.Index, "代碼不能重覆");
            }
            else
            {
                ValidateDuplication(dgv, chClassName.Index, "班級名稱不能重覆");
            }
        }

        protected override void Export()
        {
            ImportExport.Export(dgv, "匯出班級代碼設定");
        }

        protected override void Import()
        {
            ImportExport.Import(dgv, new List<string>() { "班級", "代碼" });

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                DataGridViewCell cell = row.Cells[chCode.Index];
                ValidateCodeFormat(cell,3);
            }
            ValidateDuplication(dgv, chCode.Index, "代碼不能重覆");
            ValidateDuplication(dgv, chClassName.Index, "班級名稱不能重覆");
        }
    }
}
