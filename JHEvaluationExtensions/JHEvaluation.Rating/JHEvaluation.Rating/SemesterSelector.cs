using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevComponents.DotNetBar.Controls;
using System.Windows.Forms;

namespace JHEvaluation.Rating
{
    internal class SemesterSelector
    {
        private ComboBoxEx SchoolYear { get; set; }

        private ComboBoxEx Semester { get; set; }

        private ErrorProvider error = new ErrorProvider();

        public SemesterSelector(ComboBoxEx schoolyear, ComboBoxEx semester)
        {
            SchoolYear = schoolyear;
            Semester = semester;

            SchoolYear.TextChanged += delegate
            {
                if (ValidateSchoolYear())
                    SemesterChanged(this, EventArgs.Empty);
            };

            Semester.TextChanged += delegate
            {
                if (ValidateSemester())
                    SemesterChanged(this, EventArgs.Empty);
            };
        }

        private bool ValidateSchoolYear()
        {
            int sy;
            return (int.TryParse(SchoolYear.Text, out sy));
        }

        private bool ValidateSemester()
        {
            int ss;
            return (int.TryParse(Semester.Text, out ss));
        }

        public bool ValidateControlContent()
        {
            error.SetError(SchoolYear, string.Empty);
            error.SetError(Semester, string.Empty);

            if (!ValidateSchoolYear())
            {
                error.SetError(SchoolYear, "學年度只允許數字。");
                return false;
            }

            if (!ValidateSemester())
            {
                error.SetError(Semester, "學期只允許「1」或「2」。");
                return false;
            }

            return true;
        }

        public int SelectedSchoolYear
        {
            get
            {
                if (ValidateSchoolYear())
                    return int.Parse(SchoolYear.Text);
                else
                    return 0;
            }
        }

        public int SelectedSemester
        {
            get
            {
                if (ValidateSemester())
                    return int.Parse(Semester.Text);
                else
                    return 0;
            }
        }

        public event EventHandler SemesterChanged;
    }
}
