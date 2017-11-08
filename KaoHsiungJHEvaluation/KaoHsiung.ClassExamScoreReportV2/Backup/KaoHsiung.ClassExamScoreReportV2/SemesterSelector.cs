using System;
using System.Collections.Generic;
using System.Text;
using DevComponents.DotNetBar.Controls;
using System.Windows.Forms;

namespace KaoHsiung.ClassExamScoreReportV2
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
            return (int.TryParse(GetSchoolYear(), out sy));
        }


        private bool ValidateSemester()
        {
            int ss;
            return (int.TryParse(GetSemester(), out ss));
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
                    return int.Parse(GetSchoolYear());
                else
                    return 0;
            }
        }

        public int SelectedSemester
        {
            get
            {
                if (ValidateSemester())
                    return int.Parse(GetSemester());
                else
                    return 0;
            }
        }

        private string GetSchoolYear()
        {
            if (SchoolYear.InvokeRequired)
                return SchoolYear.Invoke(new Func<string>(GetSchoolYear)).ToString();
            else
                return SchoolYear.Text;
        }

        private string GetSemester()
        {
            if (Semester.InvokeRequired)
                return Semester.Invoke(new Func<string>(GetSemester)).ToString();
            else
                return Semester.Text;
        }

        public event EventHandler SemesterChanged;
    }
}
