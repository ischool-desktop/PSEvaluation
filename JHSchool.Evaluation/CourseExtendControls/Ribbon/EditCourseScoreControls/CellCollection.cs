using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.EditCourseScoreControls
{
    class CellCollection
    {
        private Dictionary<string, ICell> _cells;

        public CellCollection()
        {
            _cells = new Dictionary<string, ICell>();
        }

        public void Add(string cellName, ICell cell)
        {
            _cells.Add(cellName, cell);
        }

        public string GetCellValue(string cellName)
        {
            if (_cells.ContainsKey(cellName))
                return _cells[cellName].GetValue();
            return null;
        }

        public Dictionary<string, ICell> Items
        {
            get { return _cells; }
        }
    }
}
