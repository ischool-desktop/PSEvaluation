using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Framework;
using JHSchool.Data;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.ScoreCalcRuleRelated
{
    public partial class PeriodAbsenceSelectionForm : FISCA.Presentation.Controls.BaseForm
    {
        private Dictionary<string, PeriodAbsencePanel> _panels;
        private EnhancedErrorProvider _error;

        private string _setting;
        public string Setting
        {
            get { return _setting; }
        }

        public PeriodAbsenceSelectionForm(string setting)
        {
            InitializeComponent();
            _panels = new Dictionary<string, PeriodAbsencePanel>();
            _error = new EnhancedErrorProvider();
            _error.BlinkRate = 0;
            if (setting.StartsWith("(")) setting = setting.Substring(1, setting.Length - 1);
            if (setting.EndsWith(")")) setting = setting.Substring(0, setting.Length - 1);
            _setting = setting;
        }

        private void PeriodAbsenceSelectionForm_Load(object sender, EventArgs e)
        {
            List<JHPeriodMappingInfo> periodList = JHSchool.Data.JHPeriodMapping.SelectAll();
            List<JHAbsenceMappingInfo> absenceList = JHSchool.Data.JHAbsenceMapping.SelectAll();

            List<string> absences = new List<string>();
            foreach (JHAbsenceMappingInfo info in absenceList)
                absences.Add(info.Name);

            foreach (JHPeriodMappingInfo info in periodList)
            {
                if (!_panels.ContainsKey(info.Type))
                {
                    // TODO: 要改…
                    PeriodAbsencePanel panel = new PeriodAbsencePanel(info.Type, "1", _error);
                    panel.Size = new Size(flowPanel.Size.Width - 10, panel.Size.Height);
                    panel.AddAbsence(absences.ToArray());
                    _panels.Add(info.Type, panel);
                    flowPanel.Controls.Add(panel);
                }
            }

            FillSetting();
        }

        private void FillSetting()
        {
            string[] typelines = _setting.Split(';');
            foreach (string typeline in typelines)
            {
                if (typeline.Split(':').Length != 2) continue;

                string type = typeline.Split(':')[0];
                string weight = "1";
                if (type.EndsWith(")") && type.Contains("("))
                {
                    type = type.Substring(0, type.Length - 1);
                    weight = type.Split('(')[1];
                    type = type.Split('(')[0];
                }

                string[] absences = typeline.Split(':')[1].Split(',');

                if (_panels.ContainsKey(type))
                {
                    _panels[type].SetAbsence(absences);
                    _panels[type].Weight = weight;
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_error.HasError)
            {
                MsgBox.Show("請先修正錯誤。");
                return;
            }

            this.DialogResult = DialogResult.OK;

            string result = string.Empty;
            foreach (string type in _panels.Keys)
            {
                string absence = _panels[type].GetAbsence();
                string weight = _panels[type].Weight;
                if (!string.IsNullOrEmpty(absence))
                    result += type + "(" + weight + ")" + ":" + absence + ";";
            }
            if (result.EndsWith(";"))
                result = result.Substring(0, result.Length - 1);

            _setting = result;
        }
    }
}
