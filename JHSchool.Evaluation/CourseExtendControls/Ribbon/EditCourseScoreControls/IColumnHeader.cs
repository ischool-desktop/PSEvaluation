using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.EditCourseScoreControls
{
    class ColumnHeader
    {
        private List<ColumnSetting> _columns;

        public List<ColumnSetting> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public ColumnHeader()
        {
            _columns = new List<ColumnSetting>();
        }
    }

    internal class ColumnSetting
    {
        private string _columnName;

        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }

        private int _columnWidth;

        public int ColumnWidth
        {
            get { return _columnWidth; }
            set { _columnWidth = value; }
        }

        private string _key;

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public ColumnSetting(string key, string columnName, int columnWidth)
        {
            _key = key;
            _columnName = columnName;
            _columnWidth = columnWidth;
            _comparer = new StringComparer();
        }

        public ColumnSetting(string key, string columnName, int columnWidth,IDataGridViewComparer comparer)
        {
            _key = key;
            _columnName = columnName;
            _columnWidth = columnWidth;
            _comparer = comparer;
        }

        private IDataGridViewComparer _comparer;

        public IDataGridViewComparer Comparer
        {
            get { return _comparer; }
            set { _comparer = value; }
        }
    }

    interface IDataGridViewComparer : IComparer
    {
        void Initialize(bool asc, int columnIndex);
    }

    class DecimalComparer : IDataGridViewComparer
    {
        private int _columnIndex;
        private int _asc;

        public DecimalComparer()
        {
        }

        #region IComparer 成員

        public void Initialize(bool asc, int columnIndex)
        {
            _columnIndex = columnIndex;
            if (asc) _asc = 1;
            else _asc = -1;
        }

        public int Compare(object x, object y)
        {
            DataGridViewRow xr = x as DataGridViewRow;
            DataGridViewRow yr = y as DataGridViewRow;

            object xo = xr.Cells[_columnIndex].Value;
            object yo = yr.Cells[_columnIndex].Value;

            string xs = xo == null ? "" : xo.ToString();
            string ys = yo == null ? "" : yo.ToString();
            
            decimal xi, yi;
            if (!decimal.TryParse(xs, out xi))
                xi = decimal.MinValue;
            if (!decimal.TryParse(ys, out yi))
                yi = decimal.MinValue;

            return xi.CompareTo(yi) * _asc;
        }

        #endregion
    }

    class StringComparer : IDataGridViewComparer
    {
        private int _columnIndex;
        private int _asc;

        public StringComparer()
        {
        }

        #region IComparer 成員

        public void Initialize(bool asc, int columnIndex)
        {
            _columnIndex = columnIndex;
            if (asc) _asc = 1;
            else _asc = -1;
        }

        public int Compare(object x, object y)
        {
            DataGridViewRow xr = x as DataGridViewRow;
            DataGridViewRow yr = y as DataGridViewRow;

            object xo = xr.Cells[_columnIndex].Value;
            object yo = yr.Cells[_columnIndex].Value;

            string xs = xo == null ? "" : xo.ToString();
            string ys = yo == null ? "" : yo.ToString();

            return xs.CompareTo(ys) * _asc;
        }

        #endregion
    }
}
