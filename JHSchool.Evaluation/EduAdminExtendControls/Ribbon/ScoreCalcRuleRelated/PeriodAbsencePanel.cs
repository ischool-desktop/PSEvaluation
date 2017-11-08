using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Framework;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.ScoreCalcRuleRelated
{
    public partial class PeriodAbsencePanel : UserControl
    {
        public EventHandler onDataChanged;
        private string _type;
        private EnhancedErrorProvider _error;
        public string Weight { get { return textBoxX1.Text; } set { textBoxX1.Text = value; } }

        public PeriodAbsencePanel(string type, string p, EnhancedErrorProvider error)
        {
            InitializeComponent();

            _error = error;
            _type = type;
            textBoxX1.Text = p;

            if (_type == "核可假別")
            {
                textBoxX1.Visible = false;
                labelX1.Visible = false;
            }
                
            groupPanel1.Text = type;
            flowPanel.Controls.Clear();
        }

        public void AddAbsence(params string[] absences)
        {
            flowPanel.SuspendLayout();
            foreach (string absence in absences)
            {
                CheckBox chk = new CheckBox();
                chk.Text = absence;
                chk.AutoSize = true;
                if (_type == "核可假別")
                {
                    chk.Tag = "核可假別";
                    chk.CheckedChanged += delegate
                    {
                        if (onDataChanged != null)
                            onDataChanged(chk, new EventArgs());
                    };
                }
                flowPanel.Controls.Add(chk);
            }
            flowPanel.ResumeLayout();
        }

        public void SetAbsence(params string[] absences)
        {
            List<string> list = new List<string>(absences);

            foreach (Control ctrl in flowPanel.Controls)
            {
                CheckBox chk = ctrl as CheckBox;
                if (list.Contains(chk.Text)) chk.Checked = true;
            }
        }

        public string GetAbsence()
        {
            string result = string.Empty;

            foreach (Control ctrl in flowPanel.Controls)
            {
                CheckBox chk = ctrl as CheckBox;
                if (chk.Checked == true)
                {
                    result += chk.Text + ",";
                }
            }

            if (result.EndsWith(","))
                result = result.Substring(0, result.Length - 1);
            return result;
        }

        private void textBoxX1_TextChanged(object sender, EventArgs e)
        {
            _error.SetError(textBoxX1, "");

            decimal d;
            if (!decimal.TryParse(textBoxX1.Text, out d))
            {
                _error.SetError(textBoxX1, "必須為數字");
            }
        }

        private void groupPanel1_SizeChanged(object sender, EventArgs e)
        {
            labelX1.Location = new Point(groupPanel1.Size.Width - 100, 0);
            textBoxX1.Location = new Point(groupPanel1.Size.Width - 60, 0);
        }
    }
}
