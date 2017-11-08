using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JHEvaluation.SemesterScoreContentItem.QuickAdd
{
    internal class QuickAddRow : DataGridViewRow
    {
        private List<QuickAddRow> Childs { get; set; }
        private QuickAddRow Parent { get; set; }

        public QuickAddRow()
        {
            
        }
    }
}
