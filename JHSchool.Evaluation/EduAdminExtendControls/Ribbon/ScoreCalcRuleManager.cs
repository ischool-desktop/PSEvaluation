using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.ScoreCalcRuleRelated;
using JHSchool.Evaluation.Editor;
using FISCA.DSAUtil;
using Framework;
using Framework.DataChangeManage;
using DevComponents.DotNetBar.Controls;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon
{
    public partial class ScoreCalcRuleManager : FISCA.Presentation.Controls.BaseForm
    {
        private BackgroundWorker _worker;
        private EnhancedErrorProvider _error;

        private ButtonItem SelectedItem { get; set; }
        private ChangeListener DataListener { get; set; }

        public ScoreCalcRuleManager()
        {
            InitializeComponent();
            _error = new EnhancedErrorProvider();

            #region Subscribe DataListener
            DataListener = new ChangeListener();

            #region Add Controls 成績計算規則
            DataListener.Add(new NumericUpDownSource(numericUpDown1));
            DataListener.Add(new NumericUpDownSource(numericUpDown2));
            DataListener.Add(new NumericUpDownSource(numericUpDown3));
            DataListener.Add(new NumericUpDownSource(numericUpDown4));
            DataListener.Add(new RadioButtonSource(
                radioButton1, radioButton2, radioButton3,
                radioButton4, radioButton5, radioButton6,
                radioButton7, radioButton8, radioButton9,
                radioButton10, radioButton11, radioButton12));
            #endregion

            #region Add Controls 畢業條件
            DataListener.Add(new CheckBoxSource(
                chkScore1,
                chkScore2,
                chkScore3,
                chkDaily1,
                chkDaily1b,
                chkDaily1c,
                chkDaily2,
                chkDaily2b,
                chkDaily2c,
                chkDaily3,
                chkDaily3b,
                chkDaily3c,
                chkDaily4,
                chkDaily4b,
                chkDaily4c
                ));
            DataListener.Add(new NumericUpDownSource(numDomain1));
            DataListener.Add(new NumericUpDownSource(numDomain2));
            DataListener.Add(new NumericUpDownSource(numDomain3));
            DataListener.Add(new ComboBoxSource(cboDegree1, ComboBoxSource.ListenAttribute.Text));
            DataListener.Add(new ComboBoxSource(cboDegree2, ComboBoxSource.ListenAttribute.Text));
            DataListener.Add(new ComboBoxSource(cboDegree3, ComboBoxSource.ListenAttribute.Text));
            DataListener.Add(new NumericUpDownSource(numPeriod1));
            DataListener.Add(new NumericUpDownSource(numPeriod1b));
            DataListener.Add(new NumericUpDownSource(numPeriod1c));
            DataListener.Add(new TextBoxSource(txtPeriod2));
            DataListener.Add(new TextBoxSource(txtPeriod2b));
            DataListener.Add(new TextBoxSource(txtPeriod2c));
            DataListener.Add(new NumericUpDownSource(numTimes3));
            DataListener.Add(new NumericUpDownSource(numTimes3b));
            DataListener.Add(new NumericUpDownSource(numTimes3c));
            DataListener.Add(new NumericUpDownSource(numMab1));
            DataListener.Add(new NumericUpDownSource(numMab2));
            DataListener.Add(new NumericUpDownSource(numMbc1));
            DataListener.Add(new NumericUpDownSource(numMbc2));
            DataListener.Add(new NumericUpDownSource(numDab1));
            DataListener.Add(new NumericUpDownSource(numDab2));
            DataListener.Add(new NumericUpDownSource(numDbc1));
            DataListener.Add(new NumericUpDownSource(numDbc2));
            DataListener.Add(new NumericUpDownSource(numMab1b));
            DataListener.Add(new NumericUpDownSource(numMab2b));
            DataListener.Add(new NumericUpDownSource(numMbc1b));
            DataListener.Add(new NumericUpDownSource(numMbc2b));
            DataListener.Add(new NumericUpDownSource(numDab1b));
            DataListener.Add(new NumericUpDownSource(numDab2b));
            DataListener.Add(new NumericUpDownSource(numDbc1b));
            DataListener.Add(new NumericUpDownSource(numDbc2b));
            DataListener.Add(new NumericUpDownSource(numMab1c));
            DataListener.Add(new NumericUpDownSource(numMab2c));
            DataListener.Add(new NumericUpDownSource(numMbc1c));
            DataListener.Add(new NumericUpDownSource(numMbc2c));
            DataListener.Add(new NumericUpDownSource(numDab1c));
            DataListener.Add(new NumericUpDownSource(numDab2c));
            DataListener.Add(new NumericUpDownSource(numDbc1c));
            DataListener.Add(new NumericUpDownSource(numDbc2c));
            DataListener.Add(new RadioButtonSource(rbCounterbalance1, rbUseDemeritOnly1));
            DataListener.Add(new RadioButtonSource(rbCounterbalance1b, rbUseDemeritOnly1b));
            DataListener.Add(new RadioButtonSource(rbCounterbalance1c, rbUseDemeritOnly1c));

            DataListener.Add(new TextBoxSource(txtSetAbsence1));
            DataListener.Add(new TextBoxSource(txtSetAbsence2));
            DataListener.Add(new TextBoxSource(txtSetAbsence1b));
            DataListener.Add(new TextBoxSource(txtSetAbsence1c));
            DataListener.Add(new TextBoxSource(txtSetAbsence2b));
            DataListener.Add(new TextBoxSource(txtSetAbsence2c));
            DataListener.Add(new TextBoxSource(txtSetAbsence2d));
            DataListener.Add(new NumericUpDownSource(numOfPeriod));

            DataListener.Add(new NumericUpDownSource(numTimes4));
            DataListener.Add(new ComboBoxSource(cboPerformanceDegree1, ComboBoxSource.ListenAttribute.Text));
            DataListener.Add(new NumericUpDownSource(numTimes4b));
            DataListener.Add(new ComboBoxSource(cboPerformanceDegree1b, ComboBoxSource.ListenAttribute.Text));
            DataListener.Add(new NumericUpDownSource(numTimes4c));
            DataListener.Add(new ComboBoxSource(cboPerformanceDegree1c, ComboBoxSource.ListenAttribute.Text));

            #endregion

            DataListener.StatusChanged += new EventHandler<ChangeEventArgs>(DataListener_StatusChanged);
            #endregion

            #region Subscribe ScoreCalcRule
            ScoreCalcRule.Instance.ItemLoaded += new EventHandler(ScoreCalcRule_ItemLoaded);
            ScoreCalcRule.Instance.ItemUpdated += new EventHandler<ItemUpdatedEventArgs>(ScoreCalcRule_ItemUpdated);
            #endregion

            InitializeScoreMappingList();
            InitializePerformanceDegree();
                
            picLoading.Visible = true;
            CardPanelEnabled = false;
            gpDaily3.Enabled = chkDaily3.Checked;
            gpDaily3b.Enabled = chkDaily3b.Checked;
            gpDaily3c.Enabled = chkDaily3c.Checked;
            LabelNameText = "";

            _worker = new BackgroundWorker();
            _worker.DoWork += delegate { ScoreCalcRule.Instance.SyncAllBackground(); };
            _worker.RunWorkerAsync();
        }

        private void InitializePerformanceDegree()
        {
            try
            {
                cboPerformanceDegree1.Items.Clear();
                cboPerformanceDegree1.DisplayMember = "Value";
                cboPerformanceDegree1.ValueMember = "Key";
                cboPerformanceDegree1b.Items.Clear();
                cboPerformanceDegree1b.DisplayMember = "Value";
                cboPerformanceDegree1b.ValueMember = "Key";
                cboPerformanceDegree1c.Items.Clear();
                cboPerformanceDegree1c.DisplayMember = "Value";
                cboPerformanceDegree1c.ValueMember = "Key";
                ConfigData cd = School.Configuration["DLBehaviorConfig"];
                XmlElement node = XmlHelper.LoadXml(cd["DailyBehavior"]);

                foreach (XmlElement item in node.SelectNodes("PerformanceDegree/Mapping"))
                {
                    KeyValuePair<string, string> pair = new KeyValuePair<string, string>(item.GetAttribute("Degree"), item.GetAttribute("Desc"));
                    cboPerformanceDegree1.Items.Add(pair);
                    cboPerformanceDegree1b.Items.Add(pair);
                    cboPerformanceDegree1c.Items.Add(pair);
                }

                if (cboPerformanceDegree1.Items.Count > 0)
                    cboPerformanceDegree1.SelectedIndex = 0;
                if (cboPerformanceDegree1b.Items.Count > 0)
                    cboPerformanceDegree1b.SelectedIndex = 0;
                if (cboPerformanceDegree1c.Items.Count > 0)
                    cboPerformanceDegree1c.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MsgBox.Show("取得日常表現程度對照表失敗。" + ex.Message);
            }
        }

        private void InitializeScoreMappingList()
        {
            try
            {
                JHSchool.Evaluation.Mapping.DegreeMapper mapper = new JHSchool.Evaluation.Mapping.DegreeMapper();

                cboDegree1.Items.Clear();
                cboDegree1.Items.AddRange(mapper.GetDegreeList().ToArray());

                cboDegree2.Items.Clear();
                cboDegree2.Items.AddRange(mapper.GetDegreeList().ToArray());
            }
            catch (Exception ex)
            {
                MsgBox.Show("取得等第對照表失敗。" + ex.Message);
            }
        }

        private void ScoreCalcRule_ItemLoaded(object sender, EventArgs e)
        {
            picLoading.Visible = false;
            btnAdd.Enabled = btnDelete.Enabled = true;
            FillItems();
        }

        private void ScoreCalcRule_ItemUpdated(object sender, ItemUpdatedEventArgs e)
        {
            if (e.PrimaryKeys.Count > 0)
            {
                RefreshItemPanel(e.PrimaryKeys[0]);
            }
        }

        private bool SaveButtonEnabled { set { btnSave1.Enabled = btnSave2.Enabled = value; } }
        private bool SaveWarningVisible { set { lblSaveWarning1.Visible = lblSaveWarning2.Visible = value; } }
        private bool CardPanelEnabled { set { cardPanelEx1.Enabled = cardPanelEx2.Enabled = value; } }
        private string LabelNameText
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                    lblName1.Visible = lblName2.Visible = false;
                else
                    lblName1.Visible = lblName2.Visible = true;

                lblName1.Text = lblName2.Text = value;
            }
        }

        private void DataListener_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (e.Status == ValueStatus.Dirty)
                SaveWarningVisible = SaveButtonEnabled = true;
            else
                SaveWarningVisible = SaveButtonEnabled = false;
        }

        private void RefreshItemPanel(string id)
        {
            ScoreCalcRuleRecord record = ScoreCalcRule.Instance.Items[id];
            ButtonItem updateItem = null;

            foreach (ButtonItem item in itemPanel.Items)
            {
                ScoreCalcRuleRecord r = item.Tag as ScoreCalcRuleRecord;
                if (r.ID == id)
                {
                    updateItem = item;
                    break;
                }
            }

            if (record != null && updateItem == null) //Insert
            {
                ButtonItem item = new ButtonItem();
                item.Text = record.Name;
                item.Tag = record;
                item.OptionGroup = "ScoreCalcRule";
                item.Click += new EventHandler(item_Click);
                itemPanel.Items.Add(item);
                item.RaiseClick();
            }
            else if (record != null && updateItem != null) //Update
            {
                updateItem.Tag = record;
                updateItem.RaiseClick();
            }
            else if (record == null && updateItem != null) //Delete
            {
                updateItem.Click -= new EventHandler(item_Click);
                itemPanel.Items.Remove(updateItem);
                SelectedItem = null;
                CardPanelEnabled = false;
                SaveButtonEnabled = false;
                SaveWarningVisible = false;
                LabelNameText = "";
            }

            itemPanel.Refresh();
            itemPanel.EnsureVisible(itemPanel.Items[itemPanel.Items.Count - 1]);
        }

        private void FillItems()
        {
            SelectedItem = null;

            itemPanel.SuspendLayout();
            itemPanel.Items.Clear();

            List<ButtonItem> itemList = new List<ButtonItem>();
            foreach (var record in ScoreCalcRule.Instance.Items)
            {
                ButtonItem item = new ButtonItem();
                item.Text = record.Name;
                item.Tag = record;
                item.OptionGroup = "ScoreCalcRule";
                item.Click += new EventHandler(item_Click);
                itemList.Add(item);
            }

            //itemList.Sort(ItemComparer);
            foreach (var item in itemList)
                itemPanel.Items.Add(item);

            itemPanel.ResumeLayout();
            itemPanel.Refresh();
        }

        private void item_Click(object sender, EventArgs e)
        {
            ButtonItem item = sender as ButtonItem;

            CardPanelEnabled = true;
            LabelNameText = item.Text;

            SelectedItem = item;
            DataListener.SuspendListen();
            SetContent((item.Tag as ScoreCalcRuleRecord).Content);
            DataListener.Reset();
            DataListener.ResumeListen();
        }

        private void SetContent(XmlElement xmlElement)
        {
            if (xmlElement == null)
            {
                SaveButtonEnabled = SaveWarningVisible = true;
                return;
            }
            else
            {
                SaveButtonEnabled = SaveWarningVisible = false;
            }

            DSXmlHelper helper = new DSXmlHelper(xmlElement);
            XmlElement element;

            #region 成績計算規則
            element = helper.GetElement("成績計算規則/各項成績計算位數/科目成績計算");
            numericUpDown1.Value = decimal.Parse(element.GetAttribute("位數"));
            SetRadioButtonSelection(element.GetAttribute("進位方式"), radioButton1, radioButton2, radioButton3);
            element = helper.GetElement("成績計算規則/各項成績計算位數/領域成績計算");
            numericUpDown2.Value = decimal.Parse(element.GetAttribute("位數"));
            SetRadioButtonSelection(element.GetAttribute("進位方式"), radioButton4, radioButton5, radioButton6);
            element = helper.GetElement("成績計算規則/各項成績計算位數/學習領域成績計算");
            numericUpDown3.Value = decimal.Parse(element.GetAttribute("位數"));
            SetRadioButtonSelection(element.GetAttribute("進位方式"), radioButton7, radioButton8, radioButton9);
            element = helper.GetElement("成績計算規則/各項成績計算位數/畢業成績計算");
            numericUpDown4.Value = decimal.Parse(element.GetAttribute("位數"));
            SetRadioButtonSelection(element.GetAttribute("進位方式"), radioButton10, radioButton11, radioButton12);
            //<各項成績計算位數>
            //    <科目成績計算 位數="2" 進位方式="四捨五入"/>
            //    <領域成績計算 位數="2" 進位方式="四捨五入"/>
            //    <學習領域成績計算 位數="2" 進位方式="四捨五入"/>
            //    <畢業成績計算 位數="2" 進位方式="四捨五入"/>
            //</各項成績計算位數>
            #endregion

            #region 畢業條件
            bool warning = false;

            element = helper.GetElement("畢業條件/學業成績畢業條件/條件[@Type='LearnDomainEach']");
            if (element != null)
            {
                chkScore1.Checked = bool.Parse(element.GetAttribute("Checked"));
                numDomain1.Value = decimal.Parse(element.GetAttribute("學習領域"));
                cboDegree1.Text = element.GetAttribute("等第");
            }
            else
                warning = true;

            element = helper.GetElement("畢業條件/學業成績畢業條件/條件[@Type='LearnDomainLast']");
            if (element != null)
            {
                chkScore2.Checked = bool.Parse(element.GetAttribute("Checked"));
                numDomain2.Value = decimal.Parse(element.GetAttribute("學習領域"));
                cboDegree2.Text = element.GetAttribute("等第");
            }
            else
                warning = true;

            chkScore3.Checked = false;
            element = helper.GetElement("畢業條件/學業成績畢業條件/條件[@Type='GraduateDomain']");
            if (element != null)
            {
                chkScore3.Checked = bool.Parse(element.GetAttribute("Checked"));
                numDomain3.Value = decimal.Parse(element.GetAttribute("學習領域"));
                cboDegree3.Text = element.GetAttribute("等第");
            }
            else
                warning = true;

            element = helper.GetElement("畢業條件/日常生活表現畢業條件/條件[@Type='AbsenceAmountEach']");
            if (element != null)
            {
                chkDaily1.Checked = bool.Parse(element.GetAttribute("Checked"));
                numPeriod1.Value = decimal.Parse(element.GetAttribute("節數"));
                lblSetAbsence1.Text = element.GetAttribute("假別");
                txtSetAbsence1.Text = lblSetAbsence1.Text;
            }
            else
                warning = true;

            element = helper.GetElement("畢業條件/日常生活表現畢業條件/條件[@Type='AbsenceAmountLast']");
            if (element != null)
            {
                chkDaily1b.Checked = bool.Parse(element.GetAttribute("Checked"));
                numPeriod1b.Value = decimal.Parse(element.GetAttribute("節數"));
                lblSetAbsence1b.Text = element.GetAttribute("假別");
                txtSetAbsence1b.Text = lblSetAbsence1b.Text;
            }
            else
                warning = true;

            //added by Cloud 2014.2.13
            element = helper.GetElement("畢業條件/日常生活表現畢業條件/條件[@Type='AbsenceAmountAll']");
            if (element != null)
            {
                chkDaily1c.Checked = bool.Parse(element.GetAttribute("Checked"));
                numPeriod1c.Value = decimal.Parse(element.GetAttribute("節數"));
                lblSetAbsence1c.Text = element.GetAttribute("假別");
                txtSetAbsence1c.Text = lblSetAbsence1c.Text;
            }
            else
                warning = true;

            numOfPeriod.Value = 7;
            element = helper.GetElement("畢業條件/日常生活表現畢業條件/條件[@Type='AbsenceAmountEachFraction']");
            if (element != null)
            {
                chkDaily2.Checked = bool.Parse(element.GetAttribute("Checked"));
                txtPeriod2.Text = element.GetAttribute("節數");
                lblSetAbsence2.Text = element.GetAttribute("假別");
                txtSetAbsence2.Text = lblSetAbsence2.Text;

                decimal d;
                if (decimal.TryParse(element.GetAttribute("每日節數"), out d))
                {
                    numOfPeriod.Value = d;
                }
                else
                {
                    warning = true;
                }
            }
            else
                warning = true;

            element = helper.GetElement("畢業條件/日常生活表現畢業條件/條件[@Type='AbsenceAmountLastFraction']");
            if (element != null)
            {
                chkDaily2b.Checked = bool.Parse(element.GetAttribute("Checked"));
                txtPeriod2b.Text = element.GetAttribute("節數");
                lblSetAbsence2b.Text = element.GetAttribute("假別");
                txtSetAbsence2b.Text = lblSetAbsence2b.Text;

                decimal d;
                if (decimal.TryParse(element.GetAttribute("每日節數"), out d))
                {
                    numOfPeriod.Value = d;
                }
                else
                {
                    warning = true;
                }
            }
            else
                warning = true;

            // 2013/12/5 新增
            element = helper.GetElement("畢業條件/日常生活表現畢業條件/條件[@Type='AbsenceAmountAllFraction']");
            if (element != null)
            {
                chkDaily2c.Checked = bool.Parse(element.GetAttribute("Checked"));
                txtPeriod2c.Text = element.GetAttribute("節數");
                lblSetAbsence2c.Text = element.GetAttribute("假別");
                txtSetAbsence2c.Text = lblSetAbsence2c.Text;
                lblSetAbsence2d.Text = element.GetAttribute("核可假別");
                txtSetAbsence2d.Text = lblSetAbsence2d.Text;

                decimal d;
                if (decimal.TryParse(element.GetAttribute("每日節數"), out d))
                {
                    numOfPeriod.Value = d;
                }
                else
                {
                    warning = true;
                }
            }
            else
                warning = true;

            element = helper.GetElement("畢業條件/日常生活表現畢業條件/條件[@Type='DemeritAmountEach']");
            if (element != null)
            {
                chkDaily3.Checked = bool.Parse(element.GetAttribute("Checked"));
                numTimes3.Value = decimal.Parse(element.GetAttribute("大過"));
                bool checkBalance = bool.Parse(element.GetAttribute("功過相抵")); ;
                rbCounterbalance1.Checked = checkBalance;
                rbUseDemeritOnly1.Checked = !checkBalance;
                SetMeritConversion1(element.GetAttribute("獎勵換算"));
                SetDemeritConversion1(element.GetAttribute("懲戒換算"));
            }
            else
                warning = true;

            element = helper.GetElement("畢業條件/日常生活表現畢業條件/條件[@Type='DemeritAmountLast']");
            if (element != null)
            {
                chkDaily3b.Checked = bool.Parse(element.GetAttribute("Checked"));
                numTimes3b.Value = decimal.Parse(element.GetAttribute("大過"));
                bool checkBalance = bool.Parse(element.GetAttribute("功過相抵")); ;
                rbCounterbalance1b.Checked = checkBalance;
                rbUseDemeritOnly1b.Checked = !checkBalance;
                SetMeritConversion1b(element.GetAttribute("獎勵換算"));
                SetDemeritConversion1b(element.GetAttribute("懲戒換算"));
            }
            else
                warning = true;

            // 2013/12/5 新增
            element = helper.GetElement("畢業條件/日常生活表現畢業條件/條件[@Type='DemeritAmountAll']");
            if (element != null)
            {
                chkDaily3c.Checked = bool.Parse(element.GetAttribute("Checked"));
                numTimes3c.Value = decimal.Parse(element.GetAttribute("大過"));
                bool checkBalance = bool.Parse(element.GetAttribute("功過相抵")); ;
                rbCounterbalance1c.Checked = checkBalance;
                rbUseDemeritOnly1c.Checked = !checkBalance;
                SetMeritConversion1c(element.GetAttribute("獎勵換算"));
                SetDemeritConversion1c(element.GetAttribute("懲戒換算"));
            }
            else
                warning = true;								

            element = helper.GetElement("畢業條件/日常生活表現畢業條件/條件[@Type='DailyBehavior']");
            if (element != null) //<條件 Checked="False" Type="DailyBehavior" 項目="4" 表現程度="2"/>
            {
                chkDaily4.Checked = bool.Parse(element.GetAttribute("Checked"));
                numTimes4.Value = decimal.Parse(element.GetAttribute("項目"));
                string degree = element.GetAttribute("表現程度");
                bool found = false;
                foreach (KeyValuePair<string, string> pair in cboPerformanceDegree1.Items)
                {
                    if (pair.Key == degree)
                    {
                        cboPerformanceDegree1.SelectedItem = pair;
                        found = true;
                        break;
                    }
                }
                if (!found)
                    warning = true;
            }
            else
                warning = true;

            element = helper.GetElement("畢業條件/日常生活表現畢業條件/條件[@Type='DailyBehaviorLast']");
            if (element != null) //<條件 Checked="False" Type="DailyBehavior" 項目="4" 表現程度="2"/>
            {
                chkDaily4b.Checked = bool.Parse(element.GetAttribute("Checked"));
                numTimes4b.Value = decimal.Parse(element.GetAttribute("項目"));
                string degree = element.GetAttribute("表現程度");
                bool found = false;
                foreach (KeyValuePair<string, string> pair in cboPerformanceDegree1b.Items)
                {
                    if (pair.Key == degree)
                    {
                        cboPerformanceDegree1b.SelectedItem = pair;
                        found = true;
                        break;
                    }
                }
                if (!found)
                    warning = true;
            }
            else
                warning = true;

            //added by Cloud 2014.2.13
            element = helper.GetElement("畢業條件/日常生活表現畢業條件/條件[@Type='DailyBehaviorAll']");
            if (element != null) //<條件 Checked="False" Type="DailyBehavior" 項目="4" 表現程度="2"/>
            {
                chkDaily4c.Checked = bool.Parse(element.GetAttribute("Checked"));
                numTimes4c.Value = decimal.Parse(element.GetAttribute("項目"));
                string degree = element.GetAttribute("表現程度");
                bool found = false;
                foreach (KeyValuePair<string, string> pair in cboPerformanceDegree1c.Items)
                {
                    if (pair.Key == degree)
                    {
                        cboPerformanceDegree1c.SelectedItem = pair;
                        found = true;
                        break;
                    }
                }
                if (!found)
                    warning = true;
            }
            else
                warning = true;
            
            if (warning)
            {
                //MsgBox.Show("畢業條件讀取發生錯誤，請重新儲存一次");
                SaveButtonEnabled = true;
                SaveWarningVisible = true;
            }
            #endregion
        }

        private XmlElement GetContent()
        {
            DSXmlHelper helper = new DSXmlHelper("ScoreCalcRule");
            XmlElement element;

            #region 成績計算規則
            helper.AddElement("成績計算規則");
            helper.AddElement("成績計算規則", "各項成績計算位數");

            element = helper.AddElement("成績計算規則/各項成績計算位數", "科目成績計算");
            element.SetAttribute("位數", "" + numericUpDown1.Value);
            element.SetAttribute("進位方式", GetRadioButtonResult(radioButton1, radioButton2, radioButton3));
            element = helper.AddElement("成績計算規則/各項成績計算位數", "領域成績計算");
            element.SetAttribute("位數", "" + numericUpDown2.Value);
            element.SetAttribute("進位方式", GetRadioButtonResult(radioButton4, radioButton5, radioButton6));
            element = helper.AddElement("成績計算規則/各項成績計算位數", "學習領域成績計算");
            element.SetAttribute("位數", "" + numericUpDown3.Value);
            element.SetAttribute("進位方式", GetRadioButtonResult(radioButton7, radioButton8, radioButton9));
            element = helper.AddElement("成績計算規則/各項成績計算位數", "畢業成績計算");
            element.SetAttribute("位數", "" + numericUpDown4.Value);
            element.SetAttribute("進位方式", GetRadioButtonResult(radioButton10, radioButton11, radioButton12));
            #endregion

            #region 畢業條件
            helper.AddElement("畢業條件");
            helper.AddElement("畢業條件", "學業成績畢業條件");
            helper.AddElement("畢業條件", "日常生活表現畢業條件");

            //LearnDomainEach
            element = helper.AddElement("畢業條件/學業成績畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkScore1.Checked);
            element.SetAttribute("Type", "LearnDomainEach");
            element.SetAttribute("學習領域", "" + numDomain1.Value);
            element.SetAttribute("等第", cboDegree1.Text);

            //LearnDomainLast
            element = helper.AddElement("畢業條件/學業成績畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkScore2.Checked);
            element.SetAttribute("Type", "LearnDomainLast");
            element.SetAttribute("學習領域", "" + numDomain2.Value);
            element.SetAttribute("等第", cboDegree2.Text);

            //GraduateDomain
            element = helper.AddElement("畢業條件/學業成績畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkScore3.Checked);
            element.SetAttribute("Type", "GraduateDomain");
            element.SetAttribute("學習領域", "" + numDomain3.Value);
            element.SetAttribute("等第", cboDegree3.Text);


            //AbsenceAmountEach
            element = helper.AddElement("畢業條件/日常生活表現畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkDaily1.Checked);
            element.SetAttribute("Type", "AbsenceAmountEach");
            element.SetAttribute("節數", "" + numPeriod1.Value);
            if (lblSetAbsence1.Text.StartsWith("(")) lblSetAbsence1.Text = lblSetAbsence1.Text.Substring(1, lblSetAbsence1.Text.Length - 1);
            if (lblSetAbsence1.Text.EndsWith(")")) lblSetAbsence1.Text = lblSetAbsence1.Text.Substring(0, lblSetAbsence1.Text.Length - 1);
            element.SetAttribute("假別", lblSetAbsence1.Text);

            //AbsenceAmountLast
            element = helper.AddElement("畢業條件/日常生活表現畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkDaily1b.Checked);
            element.SetAttribute("Type", "AbsenceAmountLast");
            element.SetAttribute("節數", "" + numPeriod1b.Value);
            if (lblSetAbsence1b.Text.StartsWith("(")) lblSetAbsence1b.Text = lblSetAbsence1b.Text.Substring(1, lblSetAbsence1b.Text.Length - 1);
            if (lblSetAbsence1b.Text.EndsWith(")")) lblSetAbsence1b.Text = lblSetAbsence1b.Text.Substring(0, lblSetAbsence1b.Text.Length - 1);
            element.SetAttribute("假別", lblSetAbsence1b.Text);

            //AbsenceAmountAll added by Cloud 2014.2.13
            element = helper.AddElement("畢業條件/日常生活表現畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkDaily1c.Checked);
            element.SetAttribute("Type", "AbsenceAmountAll");
            element.SetAttribute("節數", "" + numPeriod1c.Value);
            if (lblSetAbsence1c.Text.StartsWith("(")) lblSetAbsence1c.Text = lblSetAbsence1c.Text.Substring(1, lblSetAbsence1c.Text.Length - 1);
            if (lblSetAbsence1c.Text.EndsWith(")")) lblSetAbsence1c.Text = lblSetAbsence1c.Text.Substring(0, lblSetAbsence1c.Text.Length - 1);
            element.SetAttribute("假別", lblSetAbsence1c.Text);

            //AbsenceAmountEachFraction
            element = helper.AddElement("畢業條件/日常生活表現畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkDaily2.Checked);
            element.SetAttribute("Type", "AbsenceAmountEachFraction");
            element.SetAttribute("節數", txtPeriod2.Text);
            if (lblSetAbsence2.Text.StartsWith("(")) lblSetAbsence2.Text = lblSetAbsence2.Text.Substring(1, lblSetAbsence2.Text.Length - 1);
            if (lblSetAbsence2.Text.EndsWith(")")) lblSetAbsence2.Text = lblSetAbsence2.Text.Substring(0, lblSetAbsence2.Text.Length - 1);
            element.SetAttribute("假別", lblSetAbsence2.Text);
            element.SetAttribute("每日節數", numOfPeriod.Value + "");

            //AbsenceAmountLastFraction
            element = helper.AddElement("畢業條件/日常生活表現畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkDaily2b.Checked);
            element.SetAttribute("Type", "AbsenceAmountLastFraction");
            element.SetAttribute("節數", txtPeriod2b.Text);
            if (lblSetAbsence2b.Text.StartsWith("(")) lblSetAbsence2b.Text = lblSetAbsence2b.Text.Substring(1, lblSetAbsence2b.Text.Length - 1);
            if (lblSetAbsence2b.Text.EndsWith(")")) lblSetAbsence2b.Text = lblSetAbsence2b.Text.Substring(0, lblSetAbsence2b.Text.Length - 1);
            element.SetAttribute("假別", lblSetAbsence2b.Text);
            element.SetAttribute("每日節數", numOfPeriod.Value + "");

            //AbsenceAmountGraduateFraction (所有學期缺課節數合計未超過上課節數,2013/12/5,新增)
            element = helper.AddElement("畢業條件/日常生活表現畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkDaily2c.Checked);
            element.SetAttribute("Type", "AbsenceAmountAllFraction");
            element.SetAttribute("節數", txtPeriod2c.Text);
            if (lblSetAbsence2c.Text.StartsWith("(")) lblSetAbsence2c.Text = lblSetAbsence2c.Text.Substring(1, lblSetAbsence2c.Text.Length - 1);
            if (lblSetAbsence2c.Text.EndsWith(")")) lblSetAbsence2c.Text = lblSetAbsence2c.Text.Substring(0, lblSetAbsence2c.Text.Length - 1);
            element.SetAttribute("假別", lblSetAbsence2c.Text);

            if (lblSetAbsence2d.Text.StartsWith("(")) lblSetAbsence2d.Text = lblSetAbsence2d.Text.Substring(1, lblSetAbsence2d.Text.Length - 1);
            if (lblSetAbsence2d.Text.EndsWith(")")) lblSetAbsence2d.Text = lblSetAbsence2d.Text.Substring(0, lblSetAbsence2d.Text.Length - 1);
            element.SetAttribute("核可假別", lblSetAbsence2d.Text);
            element.SetAttribute("每日節數", numOfPeriod.Value + "");

            //DemeritAmountEach
            element = helper.AddElement("畢業條件/日常生活表現畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkDaily3.Checked);
            element.SetAttribute("Type", "DemeritAmountEach");
            element.SetAttribute("大過", "" + numTimes3.Value);
            element.SetAttribute("功過相抵", "" + rbCounterbalance1.Checked);
            element.SetAttribute("獎勵換算", GetMeritConversion1());
            element.SetAttribute("懲戒換算", GetDemeritConversion1());

            //DemeritAmountLast
            element = helper.AddElement("畢業條件/日常生活表現畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkDaily3b.Checked);
            element.SetAttribute("Type", "DemeritAmountLast");
            element.SetAttribute("大過", "" + numTimes3b.Value);
            element.SetAttribute("功過相抵", "" + rbCounterbalance1b.Checked);
            element.SetAttribute("獎勵換算", GetMeritConversion1b());
            element.SetAttribute("懲戒換算", GetDemeritConversion1b());

            //DemeritAmountAll (所有學期懲戒累計未超過,2013/12/5.新增)
            element = helper.AddElement("畢業條件/日常生活表現畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkDaily3c.Checked);
            element.SetAttribute("Type", "DemeritAmountAll");
            element.SetAttribute("大過", "" + numTimes3c.Value);
            element.SetAttribute("功過相抵", "" + rbCounterbalance1c.Checked);
            element.SetAttribute("獎勵換算", GetMeritConversion1c());
            element.SetAttribute("懲戒換算", GetDemeritConversion1c());

            //DailyBehavior
            //<條件 Checked="False" Type="DailyBehavior" 項目="4" 表現程度="2"/>
            element = helper.AddElement("畢業條件/日常生活表現畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkDaily4.Checked);
            element.SetAttribute("Type", "DailyBehavior");
            element.SetAttribute("項目", "" + numTimes4.Value);
            element.SetAttribute("表現程度", "" + ((KeyValuePair<string, string>)cboPerformanceDegree1.SelectedItem).Key);

            //DailyBehaviorLast
            element = helper.AddElement("畢業條件/日常生活表現畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkDaily4b.Checked);
            element.SetAttribute("Type", "DailyBehaviorLast");
            element.SetAttribute("項目", "" + numTimes4b.Value);
            element.SetAttribute("表現程度", "" + ((KeyValuePair<string, string>)cboPerformanceDegree1b.SelectedItem).Key);

            //DailyBehaviorAll
            element = helper.AddElement("畢業條件/日常生活表現畢業條件", "條件");
            element.SetAttribute("Checked", "" + chkDaily4c.Checked);
            element.SetAttribute("Type", "DailyBehaviorAll");
            element.SetAttribute("項目", "" + numTimes4c.Value);
            element.SetAttribute("表現程度", "" + ((KeyValuePair<string, string>)cboPerformanceDegree1c.SelectedItem).Key);

            #endregion

            return helper.BaseElement;
        }

        #region Get/Set

        private string GetDemeritConversion1()
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append(numDab1.Value + ":" + numDab2.Value + ",");
            builder.Append(numDbc1.Value + ":" + numDbc2.Value);
            return builder.ToString();
        }

        private string GetMeritConversion1()
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append(numMab1.Value + ":" + numMab2.Value + ",");
            builder.Append(numMbc1.Value + ":" + numMbc2.Value);
            return builder.ToString();
        }

        private string GetDemeritConversion1b()
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append(numDab1b.Value + ":" + numDab2b.Value + ",");
            builder.Append(numDbc1b.Value + ":" + numDbc2b.Value);
            return builder.ToString();
        }

        private string GetMeritConversion1b()
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append(numMab1b.Value + ":" + numMab2b.Value + ",");
            builder.Append(numMbc1b.Value + ":" + numMbc2b.Value);
            return builder.ToString();
        }

        private string GetDemeritConversion1c()
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append(numDab1c.Value + ":" + numDab2c.Value + ",");
            builder.Append(numDbc1c.Value + ":" + numDbc2c.Value);
            return builder.ToString();
        }

        private string GetMeritConversion1c()
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append(numMab1c.Value + ":" + numMab2c.Value + ",");
            builder.Append(numMbc1c.Value + ":" + numMbc2c.Value);
            return builder.ToString();
        }

        private void SetDemeritConversion1(string p)
        {
            string atob = p.Split(',')[0];
            string btoc = p.Split(',')[1];
            numDab1.Value = decimal.Parse(atob.Split(':')[0]);
            numDab2.Value = decimal.Parse(atob.Split(':')[1]);
            numDbc1.Value = decimal.Parse(btoc.Split(':')[0]);
            numDbc2.Value = decimal.Parse(btoc.Split(':')[1]);
        }

        private void SetMeritConversion1(string p)
        {
            string atob = p.Split(',')[0];
            string btoc = p.Split(',')[1];
            numMab1.Value = decimal.Parse(atob.Split(':')[0]);
            numMab2.Value = decimal.Parse(atob.Split(':')[1]);
            numMbc1.Value = decimal.Parse(btoc.Split(':')[0]);
            numMbc2.Value = decimal.Parse(btoc.Split(':')[1]);
        }

        private void SetDemeritConversion1b(string p)
        {
            string atob = p.Split(',')[0];
            string btoc = p.Split(',')[1];
            numDab1b.Value = decimal.Parse(atob.Split(':')[0]);
            numDab2b.Value = decimal.Parse(atob.Split(':')[1]);
            numDbc1b.Value = decimal.Parse(btoc.Split(':')[0]);
            numDbc2b.Value = decimal.Parse(btoc.Split(':')[1]);
        }

        private void SetMeritConversion1b(string p)
        {
            string atob = p.Split(',')[0];
            string btoc = p.Split(',')[1];
            numMab1b.Value = decimal.Parse(atob.Split(':')[0]);
            numMab2b.Value = decimal.Parse(atob.Split(':')[1]);
            numMbc1b.Value = decimal.Parse(btoc.Split(':')[0]);
            numMbc2b.Value = decimal.Parse(btoc.Split(':')[1]);
        }

        private void SetDemeritConversion1c(string p)
        {
            string atob = p.Split(',')[0];
            string btoc = p.Split(',')[1];
            numDab1c.Value = decimal.Parse(atob.Split(':')[0]);
            numDab2c.Value = decimal.Parse(atob.Split(':')[1]);
            numDbc1c.Value = decimal.Parse(btoc.Split(':')[0]);
            numDbc2c.Value = decimal.Parse(btoc.Split(':')[1]);
        }

        private void SetMeritConversion1c(string p)
        {
            string atob = p.Split(',')[0];
            string btoc = p.Split(',')[1];
            numMab1c.Value = decimal.Parse(atob.Split(':')[0]);
            numMab2c.Value = decimal.Parse(atob.Split(':')[1]);
            numMbc1c.Value = decimal.Parse(btoc.Split(':')[0]);
            numMbc2c.Value = decimal.Parse(btoc.Split(':')[1]);
        }

        private string GetRadioButtonResult(params RadioButton[] buttons)
        {
            foreach (var button in buttons)
            {
                if (button.Checked == true)
                    return button.Text;
            }
            return "";
        }

        private void SetRadioButtonSelection(string method, params RadioButton[] buttons)
        {
            foreach (var button in buttons)
            {
                if (button.Text == method)
                {
                    button.Checked = true;
                    return;
                }
            }
        }

        #endregion

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ScoreCalcRuleCreator creator = new ScoreCalcRuleCreator();
            if (creator.ShowDialog() == DialogResult.OK)
            {
                
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (SelectedItem == null) return;

            if (MsgBox.Show("您確定要刪除「" + SelectedItem.Text + "」嗎？", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ScoreCalcRuleRecord record = SelectedItem.Tag as ScoreCalcRuleRecord;
                ScoreCalcRuleRecordEditor editor = record.GetEditor();
                editor.Remove = true;
                editor.Save();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SelectedItem == null) return;

            if (!_error.HasError)
            {
                ScoreCalcRuleRecord record = SelectedItem.Tag as ScoreCalcRuleRecord;
                ScoreCalcRuleRecordEditor editor = record.GetEditor();
                editor.Content = GetContent();
                editor.Save();
            }
        }

        private void ComboBoxEx_TextChanged(object sender, EventArgs e)
        {
            ComboBoxEx cbo = sender as ComboBoxEx;
            if (cbo.FindStringExact(cbo.Text) < 0)
                _error.SetError(cbo, "不可以指定清單以外的值");
            else
                _error.SetError(cbo, "");
        }

        private void ScoreCalcRuleManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            ScoreCalcRule.Instance.ItemLoaded -= new EventHandler(ScoreCalcRule_ItemLoaded);
            ScoreCalcRule.Instance.ItemUpdated -= new EventHandler<ItemUpdatedEventArgs>(ScoreCalcRule_ItemUpdated);
        }

        private void chkDaily3_CheckedChanged(object sender, EventArgs e)
        {
            gpDaily3.Enabled = chkDaily3.Checked;
        }

        private void chkDaily3b_CheckedChanged(object sender, EventArgs e)
        {
            gpDaily3b.Enabled = chkDaily3b.Checked;
        }

        private void numPeriod1_ValueChanged(object sender, EventArgs e)
        {
            lblPeriod1.Text = "(<" + numPeriod1.Value + ")";
        }

        private void numPeriod1b_ValueChanged(object sender, EventArgs e)
        {
            lblPeriod1b.Text = "(<" + numPeriod1b.Value + ")";
        }

        private void numPeriod1c_ValueChanged(object sender, EventArgs e)
        {
            lblPeriod1c.Text = "(<" + numPeriod1c.Value + ")";
        }

        private void numTimes3_ValueChanged(object sender, EventArgs e)
        {
            lblTimes3.Text = "(<" + numTimes3.Value + ")";
        }

        private void numTimes3b_ValueChanged(object sender, EventArgs e)
        {
            lblTimes3b.Text = "(<" + numTimes3b.Value + ")";
        }

        private void btnSetAbsence1_Click(object sender, EventArgs e)
        {
            PeriodAbsenceSelectionForm form = new PeriodAbsenceSelectionForm(lblSetAbsence1.Text);
            if (form.ShowDialog() == DialogResult.OK)
            {
                lblSetAbsence1.Text = form.Setting;
                txtSetAbsence1.Text = form.Setting;
            }
        }

        private void btnSetAbsence1b_Click(object sender, EventArgs e)
        {
            PeriodAbsenceSelectionForm form = new PeriodAbsenceSelectionForm(lblSetAbsence1b.Text);
            if (form.ShowDialog() == DialogResult.OK)
            {
                lblSetAbsence1b.Text = form.Setting;
                txtSetAbsence1b.Text = form.Setting;
            }
        }

        //added by Cloud by 2014.2.13
        private void btnSetAbsence1c_Click(object sender, EventArgs e)
        {
            PeriodAbsenceSelectionForm form = new PeriodAbsenceSelectionForm(lblSetAbsence1c.Text);
            if (form.ShowDialog() == DialogResult.OK)
            {
                lblSetAbsence1c.Text = form.Setting;
                txtSetAbsence1c.Text = form.Setting;
            }
        }

        private void btnSetAbsence2_Click(object sender, EventArgs e)
        {
            PeriodAbsenceSelectionForm form = new PeriodAbsenceSelectionForm(lblSetAbsence2.Text);
            if (form.ShowDialog() == DialogResult.OK)
            {
                lblSetAbsence2.Text = form.Setting;
                txtSetAbsence2.Text = form.Setting;
            }
        }

        private void btnSetAbsence2b_Click(object sender, EventArgs e)
        {
            PeriodAbsenceSelectionForm form = new PeriodAbsenceSelectionForm(lblSetAbsence2b.Text);
            if (form.ShowDialog() == DialogResult.OK)
            {
                lblSetAbsence2b.Text = form.Setting;
                txtSetAbsence2b.Text = form.Setting;
            }
        }

        private void TextBoxPeriod_TextChanged(object sender, EventArgs e)
        {
            TextBoxX textBox = sender as TextBoxX;

            string fraction = textBox.Text;
            _error.SetError(textBox, "");
            string error_msg = "必須為分數或百分比";

            if (fraction.Contains("/"))
            {
                string part1 = fraction.Split('/')[0];
                string part2 = fraction.Split('/')[1];
                int i;
                if (!int.TryParse(part1, out i) || !int.TryParse(part2, out i))
                    _error.SetError(textBox, error_msg);
            }
            else if (fraction.EndsWith("%"))
            {
                string percent = fraction.Substring(0, fraction.Length - 1);
                int i;
                if (!int.TryParse(percent, out i))
                    _error.SetError(textBox, error_msg);
                else
                {
                    if (i > 100 || i < 1)
                        _error.SetError(textBox, "必須為1到100之間的數字");
                }
            }
            else
                _error.SetError(textBox, error_msg);
        }

        private void btnSetAbsence2c_Click(object sender, EventArgs e)
        {
            PeriodAbsenceSelectionFormNew form = new PeriodAbsenceSelectionFormNew(lblSetAbsence2c.Text, txtSetAbsence2d.Text);
            if (form.ShowDialog() == DialogResult.OK)
            {
                lblSetAbsence2c.Text = form.Setting;
                txtSetAbsence2c.Text = form.Setting;
                lblSetAbsence2d.Text = form.Setting2;
                txtSetAbsence2d.Text = form.Setting2;
            }
        }

        private void chkDaily3c_CheckedChanged(object sender, EventArgs e)
        {
            gpDaily3c.Enabled = chkDaily3c.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Talker talker = new Talker();
            talker.ShowDialog();
        }
    }
}
