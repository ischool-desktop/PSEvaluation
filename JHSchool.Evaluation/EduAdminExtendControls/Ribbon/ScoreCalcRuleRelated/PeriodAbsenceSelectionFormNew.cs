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
    public partial class PeriodAbsenceSelectionFormNew : FISCA.Presentation.Controls.BaseForm
    {
        private Dictionary<string, PeriodAbsencePanel> _panels;
        private EnhancedErrorProvider _error;

        private string _setting;
        private string _setting2;
        public string Setting
        {
            get { return _setting; }
        }

        public string Setting2
        {
            get { return _setting2; }
        }

        public PeriodAbsenceSelectionFormNew(string setting, string setting2)
        {
            InitializeComponent();
            _panels = new Dictionary<string, PeriodAbsencePanel>();
            _error = new EnhancedErrorProvider();
            _error.BlinkRate = 0;
            if (setting.StartsWith("(")) setting = setting.Substring(1, setting.Length - 1);
            if (setting.EndsWith(")")) setting = setting.Substring(0, setting.Length - 1);
            _setting = setting;
            _setting2 = setting2;
        }

        private void PeriodAbsenceSelectionForm_Load(object sender, EventArgs e)
        {
            List<JHPeriodMappingInfo> periodList = JHSchool.Data.JHPeriodMapping.SelectAll();
            List<JHAbsenceMappingInfo> absenceList = JHSchool.Data.JHAbsenceMapping.SelectAll();

            List<string> absences = new List<string>();
            foreach (JHAbsenceMappingInfo info in absenceList)
                absences.Add(info.Name);

            //核可假別(先處理)
            PeriodAbsencePanel panel2 = new PeriodAbsencePanel("核可假別", "1", _error);
            panel2.Size = new Size(flowPanel.Size.Width - 10, panel2.Size.Height);
            panel2.AddAbsence(absences.ToArray());
            panel2.onDataChanged += new EventHandler(DataChange);
            _panels.Add("核可假別", panel2);
            flowPanel.Controls.Add(panel2);

            //統計假別
            foreach (JHPeriodMappingInfo info in periodList)
            {
                if (!_panels.ContainsKey(info.Type))
                {
                    // TODO: 要改…
                    PeriodAbsencePanel panel = new PeriodAbsencePanel("統計假別:"+info.Type, "1", _error);
                    panel.Size = new Size(flowPanel.Size.Width - 10, panel.Size.Height);
                    panel.AddAbsence(absences.ToArray());
                    _panels.Add(info.Type, panel);
                    flowPanel.Controls.Add(panel);
                }
            }

            FillSetting();
        }

        private void DataChange(object sender, EventArgs e)
        {
            CheckBox box = sender as CheckBox;
            foreach (Control panel in flowPanel.Controls)
            {
                PeriodAbsencePanel p = panel as PeriodAbsencePanel;
                if (p == null) continue;
                foreach (Control group in p.Controls)
                {
                    DevComponents.DotNetBar.Controls.GroupPanel gp = group as DevComponents.DotNetBar.Controls.GroupPanel;
                    if (gp != null)
                    {
                        foreach (Control flp in gp.Controls)
                        {
                            FlowLayoutPanel fp = flp as FlowLayoutPanel;
                            if (fp != null)
                            {
                                foreach(Control ctrl in fp.Controls)
                                {
                                    CheckBox ch = ctrl as CheckBox;
                                    if(ch != null)
                                    {
                                        if (ch.Text == box.Text && ch.Tag+"" != "核可假別")
                                        {
                                            if (box.Checked)
                                            {
                                                ch.Checked = false;
                                                ch.Enabled = false;
                                            }
                                            else
                                            {
                                                ch.Enabled = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FillSetting()
        {
            string[] typelines = _setting.Split(';');
            string[] absences;
            string type;
            foreach (string typeline in typelines)
            {
                if (typeline.Split(':').Length != 2) continue;

                type = typeline.Split(':')[0];
                string weight = "1";
                if (type.EndsWith(")") && type.Contains("("))
                {
                    type = type.Substring(0, type.Length - 1);
                    weight = type.Split('(')[1];
                    type = type.Split('(')[0];
                }

                absences = typeline.Split(':')[1].Split(',');

                if (_panels.ContainsKey(type))
                {
                    _panels[type].SetAbsence(absences);
                    _panels[type].Weight = weight;
                }
            }

            absences = _setting2.Split(',');
            type = "核可假別";
            if (_panels.ContainsKey(type))
            {
                _panels[type].SetAbsence(absences);
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
            string result2 = string.Empty;
            foreach (string type in _panels.Keys)
            {
                string absence = _panels[type].GetAbsence();
                string weight = _panels[type].Weight;
                if (!string.IsNullOrEmpty(absence))
                    if (type != "核可假別")
                        result += type + "(" + weight + ")" + ":" + absence + ";";
                    else
                        result2 += absence;

            }
            if (result.EndsWith(";"))
                result = result.Substring(0, result.Length - 1);

            _setting = result;
            _setting2 = result2;
        }

    }
}
