using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.Threading;
using Aspose.Cells;
using System.IO;
using DevComponents.DotNetBar.Rendering;
using SmartSchool.API.PlugIn;
using Framework;
using SmartSchool.API.PlugIn.Export;

namespace JHSchool.Evaluation.ImportExport.Course
{
    public partial class ExportStudentV2 : FISCA.Presentation.Controls.BaseForm, SmartSchool.API.PlugIn.Export.ExportWizard
    {
        private string _Title;
        private IntelliSchool.DSA.ClientFramework.ControlCommunication.ListViewCheckAllManager _CheckAllManager = new IntelliSchool.DSA.ClientFramework.ControlCommunication.ListViewCheckAllManager();
        private ButtonX advButton;
        private DevComponents.DotNetBar.ControlContainerItem advContainer;
        private SmartSchool.API.PlugIn.Collections.FieldsCollection _ExportableFields;
        private SmartSchool.API.PlugIn.Collections.FieldsCollection _SelectedFields;
        private SmartSchool.API.PlugIn.Collections.OptionCollection _Options;
        private int _PackageLimint = 280;
        private LinkLabel helpButton;
        private PanelEx _OptionsContainer;
        private string Item { get; set; }
        private List<string> IDs { get; set; }

        public ExportStudentV2(string title, Image img, string item, List<string> ids)
            : this(title, img)
        {
            Item = item;
            IDs = ids;
        }

        public ExportStudentV2(string title, Image img)
        {
            InitializeComponent();
            _Title = this.Text = title;
            foreach (WizardPage page in wizard1.WizardPages)
            {
                page.PageTitle = _Title;
                if (img != null)
                {
                    Bitmap b = new Bitmap(48, 48);
                    using (Graphics g = Graphics.FromImage(b))
                        g.DrawImage(img, 0, 0, 48, 48);
                    page.PageHeaderImage = b;
                }
            }

            #region 加入進階跟HELP按鈕
            _OptionsContainer = new PanelEx();
            _OptionsContainer.Font = this.Font;
            _OptionsContainer.ColorSchemeStyle = eDotNetBarStyle.Office2007;
            _OptionsContainer.Size = new Size(100, 100);
            _OptionsContainer.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            _OptionsContainer.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            _OptionsContainer.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            _OptionsContainer.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            _OptionsContainer.Style.GradientAngle = 90;
            _Options = new SmartSchool.API.PlugIn.Collections.OptionCollection();
            _Options.ItemsChanged += new EventHandler(_Options_ItemsChanged);

            advContainer = new ControlContainerItem();
            advContainer.AllowItemResize = false;
            advContainer.GlobalItem = false;
            advContainer.MenuVisibility = eMenuVisibility.VisibleAlways;
            advContainer.Control = _OptionsContainer;

            ItemContainer itemContainer2 = new ItemContainer();
            itemContainer2.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
            itemContainer2.MinimumSize = new System.Drawing.Size(0, 0);
            itemContainer2.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            advContainer});

            advButton = new ButtonX();
            advButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            advButton.Text = "    進階";
            advButton.Top = this.wizard1.Controls[1].Controls[0].Top;
            advButton.Left = 5;
            advButton.Size = this.wizard1.Controls[1].Controls[0].Size;
            advButton.Visible = true;
            advButton.SubItems.Add(itemContainer2);
            advButton.PopupSide = ePopupSide.Top;
            advButton.SplitButton = true;
            advButton.Enabled = false;
            advButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            advButton.AutoExpandOnClick = true;
            advButton.SubItemsExpandWidth = 16;
            advButton.FadeEffect = false;
            advButton.FocusCuesEnabled = false;
            this.wizard1.Controls[1].Controls.Add(advButton);

            helpButton = new LinkLabel();
            helpButton.AutoSize = true;
            helpButton.BackColor = System.Drawing.Color.Transparent;
            helpButton.Location = new System.Drawing.Point(81, 10);
            helpButton.Size = new System.Drawing.Size(69, 17);
            helpButton.TabStop = true;
            helpButton.Text = "Help";
            //helpButton.Top = this.wizard1.Controls[1].Controls[0].Top + this.wizard1.Controls[1].Controls[0].Height - helpButton.Height;
            //helpButton.Left = 150;
            helpButton.Visible = false;
            helpButton.Click += delegate { if (HelpButtonClick != null)HelpButtonClick(this, new EventArgs()); };
            helpButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.wizard1.Controls[1].Controls.Add(helpButton);
            #endregion

            #region 設定Wizard會跟著Style跑
            //this.wizard1.FooterStyle.ApplyStyle(( GlobalManager.Renderer as Office2007Renderer ).ColorTable.GetClass(ElementStyleClassKeys.RibbonFileMenuBottomContainerKey));
            this.wizard1.HeaderStyle.ApplyStyle((GlobalManager.Renderer as Office2007Renderer).ColorTable.GetClass(ElementStyleClassKeys.RibbonFileMenuBottomContainerKey));
            this.wizard1.FooterStyle.BackColorGradientAngle = -90;
            this.wizard1.FooterStyle.BackColorGradientType = eGradientType.Linear;
            this.wizard1.FooterStyle.BackColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TopBackground.Start;
            this.wizard1.FooterStyle.BackColor2 = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TopBackground.End;
            this.wizard1.BackColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TopBackground.Start;
            this.wizard1.BackgroundImage = null;
            for (int i = 0; i < 6; i++)
            {
                (this.wizard1.Controls[1].Controls[i] as ButtonX).ColorTable = eButtonColor.OrangeWithBackground;
            }
            (this.wizard1.Controls[0].Controls[1] as System.Windows.Forms.Label).ForeColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.MouseOver.TitleText;
            (this.wizard1.Controls[0].Controls[2] as System.Windows.Forms.Label).ForeColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TitleText;
            #endregion


            this.checkBox1.ForeColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.CheckBoxItem.Default.Text;
            listViewEx1.ForeColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.CheckBoxItem.Default.Text;

            _CheckAllManager.TargetComboBox = this.checkBox1;
            _CheckAllManager.TargetListView = this.listViewEx1;

            advButton.PopupOpen += delegate { if (ControlPanelOpen != null)ControlPanelOpen(this, new EventArgs()); };
            advButton.PopupClose += delegate { if (ControlPanelClose != null)ControlPanelClose(this, new EventArgs()); };

            _ExportableFields = new SmartSchool.API.PlugIn.Collections.FieldsCollection();
            _SelectedFields = new SmartSchool.API.PlugIn.Collections.FieldsCollection();
            _ExportableFields.ItemsChanged += delegate
            {
                List<string> uncheckItems = new List<string>();
                foreach (ListViewItem item in listViewEx1.Items)
                {
                    if (item != null && item.Checked == false)
                        uncheckItems.Add(item.Text);
                }
                listViewEx1.Items.Clear();

                List<string> newFields = null;
                if (Item == "社團")
                    newFields = new List<string>(new string[] { "學年度", "學期", "社團名稱" });
                else
                    newFields = new List<string>(new string[] { "課程系統編號", "學年度", "學期", "課程名稱" });
                //newFields.AddRange(_Process.ExportableFields);
                foreach (string field in _ExportableFields)
                {
                    if (!newFields.Contains(field))
                        newFields.Add(field);
                }
                List<ListViewItem> items = new List<ListViewItem>();
                foreach (string var in newFields)
                {
                    ListViewItem item = new ListViewItem(var);
                    item.Checked = !uncheckItems.Contains(var);
                    items.Add(item);
                }
                listViewEx1.Items.AddRange(items.ToArray());
                listViewEx1_ItemChecked(null, null);
            };
        }
        Dictionary<VirtualCheckBox, DevComponents.DotNetBar.Controls.CheckBoxX> _CheckBoxs = new Dictionary<VirtualCheckBox, DevComponents.DotNetBar.Controls.CheckBoxX>();
        Dictionary<VirtualRadioButton, System.Windows.Forms.RadioButton> _RadioButtons = new Dictionary<VirtualRadioButton, System.Windows.Forms.RadioButton>();
        List<VirtualCheckItem> _AllItems = new List<VirtualCheckItem>();
        private void _Options_ItemsChanged(object sender, EventArgs e)
        {
            _CheckBoxs.Clear();
            _RadioButtons.Clear();
            int width = 0;
            int Y = 1;
            int speace = 1;
            int visibleCount = 0;
            foreach (Control control in _OptionsContainer.Controls)
            {
                control.Dispose();
            }
            _OptionsContainer.Controls.Clear();
            foreach (VirtualCheckItem item in _Options)
            {
                if (!_AllItems.Contains(item))
                {
                    _AllItems.Add(item);
                    item.VisibleChanged += new EventHandler(item_VisibleChanged);
                }
                if (item.Visible)
                {
                    visibleCount++;
                    if (item is VirtualCheckBox)
                    {
                        #region 加入CheckBox
                        DevComponents.DotNetBar.Controls.CheckBoxX checkbox = new DevComponents.DotNetBar.Controls.CheckBoxX();
                        checkbox.TabIndex = 0;
                        checkbox.TabStop = true;
                        checkbox.AutoSize = true;
                        checkbox.Text = item.Text;
                        checkbox.Checked = item.Checked;
                        checkbox.Enabled = item.Enabled;
                        checkbox.Tag = item;
                        checkbox.CheckedChanged += new EventHandler(checkbox_CheckedChanged);
                        item.CheckedChanged += new EventHandler(syncCheckBox);
                        item.TextChanged += new EventHandler(syncCheckBox);
                        item.EnabledChanged += new EventHandler(syncCheckBox);
                        checkbox.Location = new Point(9, Y);
                        _OptionsContainer.Controls.Add(checkbox);//要先加入Panel後抓Size才準
                        Y += checkbox.Height + speace;
                        if (checkbox.PreferredSize.Width + 25 > width)
                            width = checkbox.PreferredSize.Width + 25;
                        _CheckBoxs.Add(item as VirtualCheckBox, checkbox);
                        #endregion
                    }
                    if (item is VirtualRadioButton)
                    {
                        #region 加入RadioButton
                        System.Windows.Forms.RadioButton radioButton = new System.Windows.Forms.RadioButton();
                        radioButton.TabIndex = 0;
                        radioButton.TabStop = true;
                        radioButton.AutoSize = true;
                        radioButton.Text = item.Text;
                        radioButton.Checked = item.Checked;
                        radioButton.Enabled = item.Enabled; ;
                        radioButton.Tag = item;
                        radioButton.CheckedChanged += new EventHandler(radioButton_CheckedChanged);
                        item.CheckedChanged += new EventHandler(syncRadioButton);
                        item.TextChanged += new EventHandler(syncRadioButton);
                        item.EnabledChanged += new EventHandler(syncRadioButton);
                        radioButton.Location = new Point(12, Y);
                        _OptionsContainer.Controls.Add(radioButton);
                        //radioButton.Invalidate();
                        //radioButton.PerformLayout();
                        Y += radioButton.Height + speace;
                        if (radioButton.PreferredSize.Width + 25 > width)
                            width = radioButton.PreferredSize.Width + 25;
                        _RadioButtons.Add(item as VirtualRadioButton, radioButton);
                        #endregion
                    }
                }
            }
            _OptionsContainer.Size = new Size(width, Y);
            advButton.Enabled = visibleCount > 0;
            advContainer.RecalcSize();
            SetForeColor(_OptionsContainer);
        }

        private void checkbox_CheckedChanged(object sender, EventArgs e)
        {
            DevComponents.DotNetBar.Controls.CheckBoxX checkbox = (DevComponents.DotNetBar.Controls.CheckBoxX)sender;
            VirtualCheckItem item = (VirtualCheckItem)checkbox.Tag;
            if (item.Checked != checkbox.Checked)
                item.Checked = checkbox.Checked;
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.RadioButton radioButton = (System.Windows.Forms.RadioButton)sender;
            VirtualCheckItem item = (VirtualCheckItem)radioButton.Tag;
            if (item.Checked != radioButton.Checked)
                item.Checked = radioButton.Checked;
        }

        private void item_VisibleChanged(object sender, EventArgs e)
        {
            _Options_ItemsChanged(null, null);
        }

        private void syncCheckBox(object sender, EventArgs e)
        {
            VirtualCheckBox item = (VirtualCheckBox)sender;
            if (!_CheckBoxs.ContainsKey(item)) return;
            DevComponents.DotNetBar.Controls.CheckBoxX checkbox = _CheckBoxs[item];
            checkbox.Text = item.Text;
            checkbox.Enabled = item.Enabled;
            if (item.Checked != checkbox.Checked)
                checkbox.Checked = item.Checked;
            if (checkbox.PreferredSize.Width + 25 > _OptionsContainer.Width)
                _OptionsContainer.Width = checkbox.PreferredSize.Width + 25;
        }

        private void syncRadioButton(object sender, EventArgs e)
        {
            VirtualRadioButton item = (VirtualRadioButton)sender;
            if (!_RadioButtons.ContainsKey(item)) return;
            System.Windows.Forms.RadioButton radioButton = _RadioButtons[item];
            radioButton.Text = item.Text;
            radioButton.Enabled = item.Enabled;
            if (item.Checked != radioButton.Checked)
                radioButton.Checked = item.Checked;
            if (radioButton.PreferredSize.Width + 25 > _OptionsContainer.Width)
                _OptionsContainer.Width = radioButton.PreferredSize.Width + 25;
        }

        private void SetForeColor(Control parent)
        {
            foreach (Control var in parent.Controls)
            {
                if (var is System.Windows.Forms.RadioButton)
                    var.ForeColor = ((Office2007Renderer)GlobalManager.Renderer).ColorTable.CheckBoxItem.Default.Text;
                SetForeColor(var);
            }
        }

        private void listViewEx1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            SelectedFields.Clear();
            foreach (ListViewItem item in listViewEx1.Items)
            {
                if (item != null && item.Checked && _ExportableFields.Contains(item.Text))
                    SelectedFields.Add(item.Text);
            }
        }

        private void wizard1_CancelButtonClick(object sender, CancelEventArgs e)
        {
            this.Close();
        }

        #region ExportWizard 成員

        //public Control ControlPanel
        //{
        //    get
        //    {
        //        return advContainer.Control;
        //    }
        //    set
        //    {
        //        advContainer.Control = value;
        //        advButton .Enabled= (advContainer.Control != null);
        //    }
        //}
        public SmartSchool.API.PlugIn.Collections.OptionCollection Options
        {
            get { return _Options; }
        }

        public event EventHandler ControlPanelClose;

        public event EventHandler ControlPanelOpen;

        public SmartSchool.API.PlugIn.Collections.FieldsCollection ExportableFields
        {
            get { return _ExportableFields; }
        }

        public event EventHandler HelpButtonClick;

        public bool HelpButtonVisible
        {
            get
            {
                return helpButton.Visible;
            }
            set
            {
                helpButton.Visible = value;
            }
        }

        public int PackageLimit
        {
            get
            {
                return _PackageLimint;
            }
            set
            {
                _PackageLimint = value;
            }
        }

        public SmartSchool.API.PlugIn.Collections.FieldsCollection SelectedFields
        {
            get
            {
                return _SelectedFields;
            }
        }

        public event EventHandler<SmartSchool.API.PlugIn.Export.ExportPackageEventArgs> ExportPackage;

        #endregion


        private void wizard1_FinishButtonClick(object sender, CancelEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "另存新檔";
            saveFileDialog1.FileName = "" + _Title + ".xls";
            saveFileDialog1.Filter = "Excel (*.xls)|*.xls|所有檔案 (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<string> idlist = new List<string>();
                #region 取得選取課程編號
                List<Data.JHCourseRecord> selectedCourses = null;
                if (IDs != null)
                    selectedCourses = Data.JHCourse.SelectByIDs(IDs);
                else
                    selectedCourses = Data.JHCourse.SelectByIDs(JHSchool.Course.Instance.SelectedKeys);
                foreach (Data.JHCourseRecord course in selectedCourses)
                {
                    if (!idlist.Contains(course.ID))
                    {
                        idlist.Add(course.ID);
                    }
                }
                #endregion

                List<string> courseFieldList = new List<string>();
                List<string> exportFieldList = new List<string>(_SelectedFields);
                #region 取得選取欄位
                for (int index = 0; index < listViewEx1.Items.Count; index++)
                {
                    if (listViewEx1.Items[index] != null && listViewEx1.Items[index].Checked)
                    {
                        courseFieldList.Add(listViewEx1.Items[index].Text.Trim());
                    }
                }
                #endregion

                List<List<string>> splitList = new List<List<string>>();
                //把全部課程以_PackageLimint人分一包
                #region 把全部課程以_PackageLimint人分一包
                int count = 0;
                List<string> package = new List<string>();
                foreach (string id in idlist)
                {
                    if (count == 0)
                    {
                        count = (splitList.Count + 1) * 50;
                        count = count > _PackageLimint ? _PackageLimint : count;
                        package = new List<string>(_PackageLimint);
                        splitList.Add(package);
                    }
                    package.Add(id);
                    count--;
                }
                #endregion
                //兩條獨立讀取
                Dictionary<List<string>, ManualResetEvent> Loader1 = new Dictionary<List<string>, ManualResetEvent>();
                Dictionary<List<string>, ManualResetEvent> Loader2 = new Dictionary<List<string>, ManualResetEvent>();
                //已讀取資料
                Dictionary<ManualResetEvent, List<RowData>> Filler = new Dictionary<ManualResetEvent, List<RowData>>();
                int i = 0;
                foreach (List<string> p in splitList)
                {
                    ManualResetEvent handleEvent = new ManualResetEvent(false);
                    if ((i & 1) == 0)
                        Loader1.Add(p, handleEvent);
                    else
                        Loader2.Add(p, handleEvent);
                    Filler.Add(handleEvent, new List<RowData>());
                    i++;
                }

                //在背景執行取得資料
                BackgroundWorker bkwDataLoader = new BackgroundWorker();
                bkwDataLoader.DoWork += new DoWorkEventHandler(bkwDataLoader_DoWork);
                bkwDataLoader.RunWorkerAsync(new object[] { Loader1, Filler, exportFieldList });
                bkwDataLoader = new BackgroundWorker();
                bkwDataLoader.DoWork += new DoWorkEventHandler(bkwDataLoader_DoWork);
                bkwDataLoader.RunWorkerAsync(new object[] { Loader2, Filler, exportFieldList });
                //在背景計算不及格名單
                BackgroundWorker bkwNotPassComputer = new BackgroundWorker();
                bkwNotPassComputer.WorkerReportsProgress = true;
                bkwNotPassComputer.DoWork += new DoWorkEventHandler(bkwNotPassComputer_DoWork);
                bkwNotPassComputer.ProgressChanged += new ProgressChangedEventHandler(bkwNotPassComputer_ProgressChanged);
                bkwNotPassComputer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bkwNotPassComputer_RunWorkerCompleted);
                bkwNotPassComputer.RunWorkerAsync(new object[] { saveFileDialog1.FileName, courseFieldList, exportFieldList, Filler });
                this.Close();
            }
        }

        void bkwNotPassComputer_DoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = (string)((object[])e.Argument)[0];
            List<string> courseFieldList = (List<string>)((object[])e.Argument)[1];
            List<string> exportFieldList = (List<string>)((object[])e.Argument)[2];
            Dictionary<ManualResetEvent, List<RowData>> Filler = (Dictionary<ManualResetEvent, List<RowData>>)((object[])e.Argument)[3];
            double totleProgress = 0.0;
            double packageProgress = 100.0 / Filler.Count;
            Workbook report = new Workbook();
            report.Worksheets[0].Name = _Title;
            ((BackgroundWorker)sender).ReportProgress(1, _Title + " 資料整理中...");
            int RowIndex = 0;
            int i = 0;

            foreach (string field in exportFieldList)
            {
                if (courseFieldList.Contains(field))
                    courseFieldList.Remove(field);
            }
            //填表頭
            for (; i < courseFieldList.Count; i++)
            {
                report.Worksheets[0].Cells[0, i].PutValue(courseFieldList[i]);
            }
            for (int j = 0; j < exportFieldList.Count; j++)
            {
                report.Worksheets[0].Cells[0, i + j].PutValue(exportFieldList[j]);
            }
            RowIndex = 1;
            foreach (ManualResetEvent eve in Filler.Keys)
            {
                eve.WaitOne();
                if (RowIndex <= 65535)
                {
                    double miniProgress = Filler[eve].Count == 0 ? 1 : packageProgress / Filler[eve].Count;
                    double miniTotle = 0;
                    foreach (RowData row in Filler[eve])
                    {
                        Data.JHCourseRecord course = null;
                        if (row.ID != "")
                            course = Data.JHCourse.SelectByID(row.ID);

                        if (course != null)
                        {
                            if (RowIndex <= 65535)
                            {
                                i = 0;
                                for (; i < courseFieldList.Count; i++)
                                {
                                    switch (courseFieldList[i])
                                    {
                                        case "課程系統編號": report.Worksheets[0].Cells[RowIndex, i].PutValue(course.ID); break;
                                        case "學年度": report.Worksheets[0].Cells[RowIndex, i].PutValue("" + course.SchoolYear); break;
                                        case "學期": report.Worksheets[0].Cells[RowIndex, i].PutValue("" + course.Semester); break;
                                        case "課程名稱": report.Worksheets[0].Cells[RowIndex, i].PutValue(course.Name); break;
                                        case "社團名稱": report.Worksheets[0].Cells[RowIndex, i].PutValue(course.Name); break;
                                        default:
                                            break;
                                    }
                                }
                                for (int j = 0; j < exportFieldList.Count; j++)
                                {
                                    report.Worksheets[0].Cells[RowIndex, i + j].PutValue(row.ContainsKey(exportFieldList[j]) ? row[exportFieldList[j]] : "");
                                }
                            }
                            RowIndex++;
                        }
                        miniTotle += miniProgress;
                        ((BackgroundWorker)sender).ReportProgress((int)(totleProgress + miniTotle), _Title + " 處理中...");
                    }
                }
                totleProgress += packageProgress;
                ((BackgroundWorker)sender).ReportProgress((int)(totleProgress), _Title + " 處理中...");
            }
            for (int k = 0; k < courseFieldList.Count + exportFieldList.Count; k++)
            {
                report.Worksheets[0].AutoFitColumn(k, 0, 150);
            }
            report.Worksheets[0].FreezePanes(1, 0, 1, courseFieldList.Count + exportFieldList.Count);
            e.Result = new object[] { report, fileName, RowIndex > 65535 };
        }

        void bkwNotPassComputer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SmartSchool.Customization.PlugIn.Global.SetStatusBarMessage("" + e.UserState, e.ProgressPercentage);
        }

        void bkwNotPassComputer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SmartSchool.Customization.PlugIn.Global.SetStatusBarMessage(_Title + " 檔案儲存中。", 100);
            if (e.Error == null)
            {
                Workbook report = (Workbook)((object[])e.Result)[0];
                bool overLimit = (bool)((object[])e.Result)[2];
                //儲存 Excel
                #region 儲存 Excel
                string path = (string)((object[])e.Result)[1];

                if (File.Exists(path))
                {
                    bool needCount = true;
                    try
                    {
                        File.Delete(path);
                        needCount = false;
                    }
                    catch { }
                    int i = 1;
                    while (needCount)
                    {
                        string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                        if (!File.Exists(newPath))
                        {
                            path = newPath;
                            break;
                        }
                        else
                        {
                            try
                            {
                                File.Delete(newPath);
                                path = newPath;
                                break;
                            }
                            catch { }
                        }
                    }
                }
                try
                {
                    File.Create(path).Close();
                }
                catch
                {
                    SaveFileDialog sd = new SaveFileDialog();
                    sd.Title = "另存新檔";
                    sd.FileName = Path.GetFileNameWithoutExtension(path) + ".xls";
                    sd.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                    if (sd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            File.Create(sd.FileName);
                            path = sd.FileName;
                        }
                        catch
                        {
                            MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                report.Save(path, FileFormatType.Excel2003);
                #endregion
                SmartSchool.Customization.PlugIn.Global.SetStatusBarMessage(_Title + "完成。");
                if (overLimit)
                    MsgBox.Show("匯出資料已經超過Excel的極限(65536筆)。\n超出的資料無法被匯出。\n\n請減少選取學生人數。");
                System.Diagnostics.Process.Start(path);
            }
            else
                SmartSchool.Customization.PlugIn.Global.SetStatusBarMessage(_Title + "發生未預期錯誤。");
        }

        void bkwDataLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<List<string>, ManualResetEvent> handle = (Dictionary<List<string>, ManualResetEvent>)((object[])e.Argument)[0];
            Dictionary<ManualResetEvent, List<RowData>> Filler = (Dictionary<ManualResetEvent, List<RowData>>)((object[])e.Argument)[1];
            List<string> exportFieldList = (List<string>)((object[])e.Argument)[2];
            foreach (List<string> splitList in handle.Keys)
            {
                try
                {
                    if (ExportPackage != null)
                    {
                        ExportPackageEventArgs args = new ExportPackageEventArgs();
                        foreach (string var in splitList)
                        {
                            args.List.Add(var);
                        }
                        foreach (string var in exportFieldList)
                        {
                            if (_ExportableFields.Contains(var))
                                args.ExportFields.Add(var);
                        }
                        ExportPackage.Invoke(this, args);
                        Filler[handle[splitList]].AddRange(args.Items);
                    }
                }
                catch (Exception ex)
                {
                    BugReporter.ReportException(ex, false);
                }
                finally
                {
                    handle[splitList].Set();
                }
            }
        }
    }
}