using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.EditCourseScoreControls
{
    class RowEntity
    {
        private string _studentID;

        public string StudentID
        {
            get { return _studentID; }
            set { _studentID = value; }
        }

        private CellCollection _collection;

        public RowEntity(string studentid)
        {
            _collection = new CellCollection();
            _studentID = studentid;
        }

        public void AddCell(string cellName,ICell cell)
        {
            _collection.Add(cellName, cell);
        }

        public Dictionary<string,ICell> Cells
        {
            get { return _collection.Items; }
        }
    }
}
