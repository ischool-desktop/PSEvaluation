using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using System.Xml;

namespace HsinChu.ClassExamScoreAvgComparison
{
    internal partial class ConfigForm : BaseForm
    {
        private Config _config;
        private Dictionary<CheckBox, RadioButton> _map;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConfigForm(Config config)
        {
            InitializeComponent();
            InitializeControlMapping();

            _config = config;
            LoadConfiguration();
        }

        /// <summary>
        /// 讀取設定值
        /// </summary>
        private void LoadConfiguration()
        {
            if (!_config.HasValue)
            {
                FillDefaultConfig();
            }
            else
            {
                foreach (CheckBox ctrl in gpItem.Controls)
                {
                    if (_config.PrintItems.Contains(ctrl.Text))
                        ctrl.Checked = true;
                }

                foreach (RadioButton ctrl in gpRank.Controls)
                {
                    if (_config.RankMethod == ctrl.Text)
                        ctrl.Checked = true;
                }
            }
        }

        /// <summary>
        /// 儲存設定值
        /// </summary>
        private void SaveConfiguration()
        {
            _config.PrintItems.Clear();
            foreach (CheckBox ctrl in gpItem.Controls)
            {
                if (ctrl.Checked)
                    _config.PrintItems.Add(ctrl.Text);
            }

            foreach (RadioButton ctrl in gpRank.Controls)
            {
                if (ctrl.Checked)
                    _config.RankMethod = ctrl.Text;
            }
            _config.Save();
        }

        /// <summary>
        /// 預設值
        /// </summary>
        private void FillDefaultConfig()
        {
            checkBox1.Checked = true;
            checkBox4.Checked = true;
            radioButton4.Checked = true;
        }

        /// <summary>
        /// 建立CheckBox與RadioButton的對應關係
        /// </summary>
        private void InitializeControlMapping()
        {
            _map = new Dictionary<CheckBox, RadioButton>();

            int index = 0;
            foreach (var ctrl in gpItem.Controls)
            {
                _map.Add(ctrl as CheckBox, gpRank.Controls[index] as RadioButton);
                index++;
            }
        }

        /// <summary>
        /// 驗證設定是否合理
        /// </summary>
        /// <returns></returns>
        private bool IsValid()
        {
            bool chkValid = false;
            foreach (CheckBox ctrl in gpItem.Controls)
                chkValid |= ctrl.Checked;

            bool rbValid = false;
            foreach (RadioButton ctrl in gpRank.Controls)
                rbValid |= (ctrl.Enabled && ctrl.Checked);

            return (chkValid && rbValid);
        }

        #region Events
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (_map.ContainsKey(chk))
                _map[chk].Enabled = chk.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValid())
            {
                MsgBox.Show("請選擇一種排名依據。");
                return;
            }

            SaveConfiguration();
            this.DialogResult = DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
