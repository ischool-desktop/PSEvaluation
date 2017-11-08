using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using DevComponents.DotNetBar;
using System.Xml;
using Framework;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.EditCourseScoreControls
{
    class DataGridViewHelper
    {
        private DataGridView _dgv;
        private IDataProvider _provider;
        private List<string> _displayColumns;

        public DataGridViewHelper(DataGridView dataGridView, IDataProvider dataProvider)
        {
            List<string> columns = new List<string>();
            foreach (ColumnSetting setting in dataProvider.ColumnHeader.Columns)
                columns.Add(setting.ColumnName);
            Initialize(dataGridView, dataProvider, columns);
        }

        public DataGridViewHelper(DataGridView dataGridView, IDataProvider dataProvider, List<string> displayColumns)
        {
            Initialize(dataGridView, dataProvider, displayColumns);
        }

        private void Initialize(DataGridView dataGridView, IDataProvider dataProvider, List<string> displayColumns)
        {
            if (dataGridView == null)
                throw new Exception("DataGridView 不可是 NULL");
            if (dataProvider == null)
                throw new Exception("IDataProvider 不可是 NULL");

            _dgv = dataGridView;
            _dgv.AllowUserToAddRows = false;
            _provider = dataProvider;
            _displayColumns = displayColumns;

            _dgv.CellEndEdit += new DataGridViewCellEventHandler(_dgv_CellEndEdit);
            _dgv.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(_dgv_ColumnHeaderMouseClick);
            _dgv.KeyUp += new KeyEventHandler(_dgv_KeyUp);
            _dgv.SelectionChanged += new EventHandler(_dgv_SelectionChanged);
            _dgv.CellEnter += new DataGridViewCellEventHandler(_dgv_CellEnter);
        }

        private DataGridViewCell _selectedCell;
        void _dgv_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            _selectedCell = _dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
        }

        private CellSquare _square;
        void _dgv_SelectionChanged(object sender, EventArgs e)
        {
            _square = new CellSquare();
            int startRowIndex = int.MaxValue;
            int startColumnIndex = int.MaxValue;
            int rowLength = 0, columnLength = 0;
            
            foreach (DataGridViewCell cell in _dgv.SelectedCells)
            {
                if (cell.ColumnIndex <= startColumnIndex && cell.RowIndex <= startRowIndex)
                {
                    startRowIndex = cell.RowIndex;
                    startColumnIndex = cell.ColumnIndex;
                }
            }
            _square.StartRowIndex = startRowIndex;
            _square.StartColumnIndex = startColumnIndex;

            for (int i = startRowIndex; i < _dgv.Rows.Count; i++)
            {
                if (_dgv.Rows[i].Cells[startColumnIndex].Selected)
                    rowLength++;
            }
            _square.RowLength = rowLength;

            for (int i = startColumnIndex; i < _dgv.Columns.Count; i++)
            {
                if (_dgv.Rows[startRowIndex].Cells[i].Selected)
                    columnLength++;
            }
            _square.ColumnLength = columnLength;

            for (int i = startRowIndex; i < startRowIndex + rowLength - 1; i++)
            {
                for (int j = startColumnIndex; j < startColumnIndex + columnLength - 1; j++)
                {
                    if (!_dgv.Rows[i].Cells[j].Selected)
                    {
                        _square.MutiSquareSelected = true;
                        break;
                    }
                }
            }
        }

        void _dgv_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewCell cell in _dgv.SelectedCells)
                {
                    IExamCell ic = cell.Tag as IExamCell;
                    if (ic == null) continue;
                    cell.Value = "";
                    ic.SetValue("");
                    CellEdited(cell, ic);
                }
            }
            //else if (e.KeyCode == Keys.Control | e.KeyCode == Keys.V)
            //{
            //    string value = Clipboard.GetText();
            //    string[] rowValues = value.Split('\n');
            //    XmlDocument doc = new XmlDocument();
            //    XmlElement root = doc.CreateElement("RootElement");
            //    foreach (string rowValue in rowValues)
            //    {
            //        XmlElement rowElement = doc.CreateElement("");
            //    }               
            //}
        }

        private bool _asc;
        void _dgv_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            _asc = !_asc;
            DataGridViewColumn column = _dgv.Columns[e.ColumnIndex];
            ColumnSetting setting = column.Tag as ColumnSetting;

            if (setting != null)
            {
                IDataGridViewComparer comparer = setting.Comparer;
                comparer.Initialize(_asc, e.ColumnIndex);
                _dgv.Sort(comparer);
                return;
            }   
        }

        void _dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = _dgv.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];
            IExamCell icell = cell.Tag as IExamCell;
            CellEdited(cell, icell);
        }

        private void CellEdited(DataGridViewCell cell , IExamCell icell)
        {
            DataGridViewCellStyle style = cell.DataGridView.Rows[cell.RowIndex].Cells[0].Style;          
            Color defaultBackColor = style.BackColor;
            Color defaultForeColor = style.ForeColor;

            cell.Style.BackColor = defaultBackColor;
            cell.Style.ForeColor = defaultForeColor;
            cell.ToolTipText = "";

            string value = cell.Value == null ? "" : cell.Value.ToString().Trim();
            cell.Value = value;

            if (icell != null)
            {
                icell.SetValue(value);
                if (icell.IsValid())
                {
                    icell.Standard.Judge(cell);
                    if (icell.IsDirty)
                    {
                        cell.Style.BackColor = Color.Yellow;
                        cell.ToolTipText = "此欄位值已變更，其原值為『" + icell.DefaultValue + "』";
                    }
                }
                else
                {
                    cell.Style.BackColor = Color.Red;
                    cell.Style.ForeColor = defaultForeColor;
                    cell.ToolTipText = "錯誤!!此欄位必須為數字";
                }
            }
            
            if (DirtyChanged != null)
                DirtyChanged.Invoke(this, new DirtyChangedEventArgs(IsDirty()));
        }

        public void Fill()
        {
            _dgv.Columns.Clear();
            _dgv.Rows.Clear();

            foreach (ColumnSetting setting in _provider.ColumnHeader.Columns)
            {
                int columnIndex = _dgv.Columns.Add(setting.ColumnName, setting.ColumnName);
                DataGridViewColumn col = _dgv.Columns[columnIndex];
                col.Width = setting.ColumnWidth;
                col.Tag = setting;           
            }

            try
            {
                foreach (RowEntity row in _provider.Rows.Entities)
                {
                    int rowIndex = _dgv.Rows.Add();
                    DataGridViewRow currentRow = _dgv.Rows[rowIndex];
                    currentRow.Tag = row.StudentID;

                    foreach (DataGridViewColumn column in _dgv.Columns)
                    {
                        ICell valueCell = row.Cells[column.Name];
                        string value = valueCell.GetValue();
                        DataGridViewCell currentCell = currentRow.Cells[column.Name];
                        currentCell.Value = value;
                        currentCell.Tag = valueCell;

                        if (valueCell is StudentCell)
                        {
                            column.Frozen = true;
                            column.ReadOnly = true;
                        }
                        else
                        {
                            IExamCell examCell = valueCell as IExamCell;
                            if (!string.IsNullOrEmpty(value))
                                examCell.Standard.Judge(currentCell);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MsgBox.Show("資料設定錯誤:" + exception.Message);
            }

            ResetDisplayColumn(_displayColumns);
        }

        public void ResetDisplayColumn(List<string> displayColumns)
        {
            _displayColumns = displayColumns;
            ResetDisplayColumn();
        }

        public void ResetDisplayColumn()
        {
            foreach (DataGridViewColumn column in _dgv.Columns)
                column.Visible = _displayColumns.Contains(column.Name);            
        }

        public void ResetValue()
        {
            foreach (DataGridViewRow row in _dgv.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.BackColor = Color.White;
                    cell.Style.ForeColor = Color.Black;
                    cell.ToolTipText = "";

                    IExamCell ic = cell.Tag as IExamCell;
                    if (ic != null)
                    {
                        ic.Reset();
                        cell.Value = ic.GetValue();
                        ic.Standard.Judge(cell);
                    }
                }
            }
        }

        public bool IsValid()
        {
            foreach (DataGridViewRow row in _dgv.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    IExamCell ic = cell.Tag as IExamCell;
                    if (ic == null) continue;
                    if (!ic.IsValid()) return false;
                }
            }
            return true;
        }

        public bool IsDirty()
        {
            foreach (DataGridViewRow row in _dgv.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    IExamCell ic = cell.Tag as IExamCell;
                    if (ic == null) continue;
                    if (ic.IsDirty) return true;
                }
            }
            return false;
        }

        #region 提供事件(Event)
        /// <summary>
        /// 資料變更時觸發
        /// </summary>
        public event EventHandler<DirtyChangedEventArgs> DirtyChanged;
        #endregion
    }

    public class DirtyChangedEventArgs : EventArgs
    {
        private bool _dirty;

        public bool Dirty
        {
            get { return _dirty; }
            set { _dirty = value; }
        }

        public DirtyChangedEventArgs(bool dirty)
        {
            _dirty = dirty;
        }

    }

    public class CellSquare
    {
        private int _startRowIndex;

        public int StartRowIndex
        {
            get { return _startRowIndex; }
            set { _startRowIndex = value; }
        }
        private int _startColumnIndex;

        public int StartColumnIndex
        {
            get { return _startColumnIndex; }
            set { _startColumnIndex = value; }
        }
        private int _rowLength;

        public int RowLength
        {
            get { return _rowLength; }
            set { _rowLength = value; }
        }
        private int _columnLength;

        public int ColumnLength
        {
            get { return _columnLength; }
            set { _columnLength = value; }
        }

        private bool _mutiSquareSelected;

        public bool MutiSquareSelected
        {
            get { return _mutiSquareSelected; }
            set { _mutiSquareSelected = value; }
        }
    }
}
