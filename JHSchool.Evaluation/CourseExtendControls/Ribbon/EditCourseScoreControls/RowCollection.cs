using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.EditCourseScoreControls
{
    class RowCollection
    {
        //private List<KeyValuePair<string, RowEntity>> _rows;
        //private Dictionary<string, RowEntity> _rows;
        private List<string> _rowIndex;
        private List<RowEntity> _rows;

        public RowCollection()
        {
            _rowIndex = new List<string>();
            _rows = new List<RowEntity>();
        }

        public void Add(string studentid, RowEntity entity)
        {
            _rowIndex.Add(studentid);
            _rows.Add(entity);
        }

        public ICell FindCell(string studentid, string columnName)
        {
            int index = -1;
            for (int i = 0; i < _rowIndex.Count; i++)
            {
                if (_rowIndex[i] == studentid)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
                return null;

            foreach (string s in _rows[index].Cells.Keys)
            {
                if (s == columnName)
                    return _rows[index].Cells[s];
            }
            return null;
        }

        public RowEntity[] Entities
        {
            get
            {
                return _rows.ToArray();
            }
        }
    }
}
