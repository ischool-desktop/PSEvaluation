using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.EditCourseScoreControls
{
    interface IStandard
    {
        string StandardName { get;}
        void Judge(DataGridViewCell handlingCell);
    }

    internal class NormalStandard : IStandard
    {
        private string _name;

        public NormalStandard()
        {
            _name = "Normal";
        }

        public string StandardName
        {
            get { return _name; }
        }

        public void Judge(DataGridViewCell handlingCell)
        {
            string value = handlingCell.Value == null ? "" : handlingCell.Value.ToString();
            DataGridViewCellStyle style = handlingCell.DataGridView.Rows[handlingCell.RowIndex].Cells[0].Style;
            //handlingCell.DataGridView.Rows[handlingCell.RowIndex].Cells[0].Style;
            Color defaultBackColor = style.BackColor;
            Color defaultForeColor = style.ForeColor;
            handlingCell.Style.BackColor = defaultBackColor;
            handlingCell.Style.ForeColor = defaultForeColor;

            if (value == "") return;
            decimal d;
            if (decimal.TryParse(value, out d))
            {
                handlingCell.Style.BackColor = defaultBackColor;               
                if (d < 60)
                    handlingCell.Style.ForeColor = Color.Red;
                else
                    handlingCell.Style.ForeColor = Color.Black;                                
            }
            else if(value == "¯Ê")
            {
                handlingCell.Style.BackColor = Color.LightGray;
                handlingCell.Style.ForeColor = Color.Brown;
            }
            else
            {
                handlingCell.Style.BackColor = Color.Red;
                handlingCell.Style.ForeColor = Color.White;
            }
        }
    }
}
