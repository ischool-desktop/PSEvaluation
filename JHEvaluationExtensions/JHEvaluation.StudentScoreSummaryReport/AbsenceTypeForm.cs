using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data;

namespace JHEvaluation.StudentScoreSummaryReport
{
    public partial class AbsenceTypeForm : BaseForm
    {
        public AbsenceTypeForm()
        {
            InitializeComponent();
            ColumnIndexs = new Dictionary<string, int>();
            RowIndexs = new Dictionary<string, int>();
        }

        private void AbsenceTypeForm_Load(object sender, EventArgs e)
        {
            try
            {
                List<PeriodMappingInfo> periods = K12.Data.PeriodMapping.SelectAll();
                UniqueSet<string> pSet = new UniqueSet<string>();
                foreach (PeriodMappingInfo each in periods)
                    if (!pSet.Contains(each.Type)) pSet.Add(each.Type);

                List<AbsenceMappingInfo> absences = K12.Data.AbsenceMapping.SelectAll();
                UniqueSet<string> aSet = new UniqueSet<string>();
                foreach (AbsenceMappingInfo each in absences)
                    if (!aSet.Contains(each.Name)) aSet.Add(each.Name);

                dgAbsence.Columns.Clear();
                dgAbsence.Columns.Add(chCatalog);
                foreach (string eachAbsence in aSet.ToList())
                {
                    int index = dgAbsence.Columns.Add(chAbsenceTemplate.Clone() as DataGridViewColumn);
                    dgAbsence.Columns[index].HeaderText = eachAbsence;
                    ColumnIndexs.Add(eachAbsence, index);
                }

                foreach (string eachType in pSet.ToList())
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dgAbsence);
                    row.Cells[chCatalog.Index].Value = eachType;
                    dgAbsence.Rows.Add(row);
                    RowIndexs.Add(eachType, row.Index);
                }
                ApplySetting();
            }
            catch (Exception ex)
            {
                MsgBox.Show(string.Format("顯示資料失敗({0})。", ex.Message));
            }
        }

        private Dictionary<string, int> ColumnIndexs { get; set; }

        private Dictionary<string, int> RowIndexs { get; set; }

        /// <summary>
        /// 取得設定結果。
        /// </summary>
        public Dictionary<string, List<string>> SettingResult { get; private set; }

        public static DialogResult PopupSettingForm(Dictionary<string, List<string>> defaultSetting, out Dictionary<string, List<string>> result)
        {
            AbsenceTypeForm form = new AbsenceTypeForm();
            form.SettingResult = defaultSetting;
            DialogResult dr = form.ShowDialog();
            result = form.SettingResult;
            return dr;
        }

        /// <summary>
        /// 設定畫面的預設狀況。
        /// 例：一般:曠課,事假,病假;集合:曠課,事假,公假
        /// </summary>
        /// <param name="content"></param>
        public void ApplySetting()
        {
            ResetValues(false);

            foreach (KeyValuePair<string, List<string>> type in SettingResult)
            {
                if (!RowIndexs.ContainsKey(type.Key))
                {
                    lblLost.Visible = true;
                    continue;
                }

                int rowIndex = RowIndexs[type.Key];

                foreach (string absence in type.Value)
                {
                    if (!ColumnIndexs.ContainsKey(absence))
                    {
                        lblLost.Visible = true;
                        continue;
                    }

                    int columnIndex = ColumnIndexs[absence];

                    dgAbsence.Rows[rowIndex].Cells[columnIndex].Value = true;
                }
            }
        }

        private void ResetValues(bool value)
        {
            foreach (DataGridViewRow row in dgAbsence.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell is DataGridViewCheckBoxCell)
                        (cell as DataGridViewCheckBoxCell).Value = value;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SettingResult = new Dictionary<string, List<string>>();
                foreach (DataGridViewRow row in dgAbsence.Rows)
                {
                    string type = row.Cells[chCatalog.Index].Value + "";

                    SettingResult.Add(type, new List<string>());
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (!(cell is DataGridViewCheckBoxCell)) continue;

                        DataGridViewCheckBoxCell chkcell = cell as DataGridViewCheckBoxCell;

                        bool value;
                        if (bool.TryParse(chkcell.Value + "", out value))
                        {
                            if (value)
                                SettingResult[type].Add(dgAbsence.Columns[cell.ColumnIndex].HeaderText);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show(string.Format("儲存資料失敗({0})。", ex.Message));
                DialogResult = DialogResult.None;
            }
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ResetValues(chkSelectAll.Checked);
        }
    }
}
