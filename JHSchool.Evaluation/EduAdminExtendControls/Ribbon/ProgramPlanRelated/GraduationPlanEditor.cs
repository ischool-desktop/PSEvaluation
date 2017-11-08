using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DevComponents.DotNetBar.Rendering;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.ProgramPlanRelated
{
    public partial class GraduationPlanEditor : UserControl, IGraduationPlanEditor
    {
        private List<DataGridViewCell> _DirtyCells = new List<DataGridViewCell>();

        private bool _RawDeleted;

        private bool _IsDirty;

        private int _CreditStartIndex;
        private int _SubjectNameIndex;
        private int _StartLevelIndex;
        private int _CategoryIndex;
        private int _DomainIndex;
        private int _RequiredByIndex;
        private int _RequiredIndex;
        private int _EntryIndex;
        private int _NotIncludedInCreditIndex;
        private int _CalcFlagIndex;

        private Dictionary<DataGridViewCell, object> _defaultValues = new Dictionary<DataGridViewCell, object>();

        private Dictionary<DataGridViewRow, string> _RowSubject = new Dictionary<DataGridViewRow, string>();

        private int _SelectedRowIndex;

        #region 事件處理

        private void dataGridViewX1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewRow row = dgv.Rows[e.RowIndex];
            if (e.ColumnIndex == _SubjectNameIndex && _RowSubject.ContainsKey(row))
            {
                row.Cells[_SubjectNameIndex].Value = _RowSubject[row];
            }
            #region 把初值存起來做為之後判斷IsDirty用
            if (!_defaultValues.ContainsKey(row.Cells[e.ColumnIndex]))
                _defaultValues.Add(row.Cells[e.ColumnIndex], row.Cells[e.ColumnIndex].Value);
            #endregion
        }

        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgv.Rows[e.RowIndex];
            #region 做IsDirty檢查
            if (_defaultValues.ContainsKey(row.Cells[e.ColumnIndex]) &&
                    ("" + _defaultValues[row.Cells[e.ColumnIndex]]) != ("" + row.Cells[e.ColumnIndex].Value)//把值用都轉成字串來比對相等，用string+object相加來省去null.ToString()的麻煩
                    )
            {
                _DirtyCells.Add(row.Cells[e.ColumnIndex]);
            }
            else
            {
                if (_DirtyCells.Contains(row.Cells[e.ColumnIndex]))
                    _DirtyCells.Remove(row.Cells[e.ColumnIndex]);
            }
            if ((_DirtyCells.Count != 0) != _IsDirty)
            {
                _IsDirty |= (_DirtyCells.Count != 0);
                if (IsDirtyChanged != null)
                    IsDirtyChanged.Invoke(this, new EventArgs());
            }
            #endregion

            #region 判斷是否為科目名稱欄加入至集合中
            if (e.ColumnIndex == _SubjectNameIndex)
            {
                if (!string.IsNullOrEmpty("" + row.Cells[_SubjectNameIndex].Value))
                {
                    if (_RowSubject.ContainsKey(row))
                        _RowSubject[row] = row.Cells[_SubjectNameIndex].Value.ToString();
                    else
                        _RowSubject.Add(row, row.Cells[_SubjectNameIndex].Value.ToString());
                }
                else
                    if (_RowSubject.ContainsKey(row))
                        _RowSubject.Remove(row);
            }
            #endregion

            #region 檢查整個 Row 的欄位

            row.Cells[_SubjectNameIndex].ErrorText = "";
            if (!row.IsNewRow && string.IsNullOrEmpty("" + row.Cells[_SubjectNameIndex].Value))
                row.Cells[_SubjectNameIndex].ErrorText = "科目不能為空白";

            if (!row.IsNewRow && string.IsNullOrEmpty(row.Cells[_SubjectNameIndex].ErrorText))
            {
                bool has_error = true;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.ColumnIndex < _CreditStartIndex || cell.ColumnIndex >= _CreditStartIndex + 6) continue;
                    if (!string.IsNullOrEmpty("" + cell.Value))
                    {
                        has_error = false;
                        break;
                    }
                }
                if (has_error == true)
                    row.Cells[_SubjectNameIndex].ErrorText = "必需輸入節數/權數";
            }

            //if (!row.IsNewRow && string.IsNullOrEmpty("" + row.Cells[_DomainIndex].Value))
            //    row.Cells[_DomainIndex].ErrorText = "領域不能為空白";
            //else
            //    row.Cells[_DomainIndex].ErrorText = "";

            #endregion

            //編輯最後一列(新增資料那列前依列)
            //if (e.RowIndex == dataGridViewX1.Rows.Count - 2)
            //{
            //    foreach (int index in new int[] { _CategoryIndex, _RequiredByIndex, _RequiredIndex, _EntryIndex })
            //    {
            //        dataGridViewX1.Rows[e.RowIndex + 1].Cells[index].Value = dataGridViewX1.Rows[e.RowIndex].Cells[index].Value;
            //    }
            //}
        }

        private void dataGridViewX1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            cell.ErrorText = "";

            #region 數字欄位輸入格式檢查
            if (!string.IsNullOrEmpty("" + e.FormattedValue) && e.ColumnIndex >= _CreditStartIndex && e.ColumnIndex < _CreditStartIndex + 6)
            {
                string orig = ("" + e.FormattedValue).Trim();

                PeriodCredit pc = new PeriodCredit();
                if (!pc.Parse(orig))
                    cell.ErrorText = pc.Error;
                //if (orig.IndexOf("/") > 0)
                //{
                //    string period = "" + orig.Split('/')[0]; //節數
                //    string credit = "" + orig.Split('/')[1]; //權數

                //    if (string.IsNullOrEmpty(period) || string.IsNullOrEmpty(credit))
                //        cell.ErrorText = "必須輸入節數/權數";
                //    else if (!int.TryParse(period, out i) && !int.TryParse(credit, out i))
                //        cell.ErrorText = "節數/權數必須是數字";
                //    else if (!int.TryParse(period, out i))
                //        cell.ErrorText = "節數必須是數字";
                //    else if (!int.TryParse(credit, out i))
                //        cell.ErrorText = "權數必須是數字";
                //}
                //else
                //{
                //    if (!int.TryParse("" + e.FormattedValue, out i))
                //        cell.ErrorText = "必須輸入數字";
                //}
            }
            #endregion

            #region 檢查科目名稱頭尾不可有空白
            if (e.ColumnIndex == _SubjectNameIndex && ("" + e.FormattedValue != ("" + e.FormattedValue).Trim()))
                cell.ErrorText = "科目名稱頭尾不可有空白字元";
            #endregion

            dgv.UpdateCellErrorText(e.ColumnIndex, e.RowIndex);

            #region 學分數檢查 OLD
            //if ("" + dataGridViewX1.Rows[e.RowIndex].Cells[_SubjectNameIndex].FormattedValue != "")
            //{
            //    bool pass = false;
            //    #region 若是正輸入的欄位則用驗證值檢查否則用欄位上的值檢察
            //    for (int i = 0; i < 8; i++)
            //    {
            //        int x = 0;
            //        if (i + _CreditStartIndex == e.ColumnIndex && (int.TryParse(e.FormattedValue.ToString(), out x)))
            //        {
            //            pass = true;
            //            break;
            //        }
            //        else if (int.TryParse("" + dataGridViewX1.Rows[e.RowIndex].Cells[i + _CreditStartIndex].FormattedValue, out x))
            //        {
            //            pass = true;
            //            break;
            //        }
            //    }
            //    #endregion
            //    if (!pass)
            //    {
            //        dataGridViewX1.Rows[e.RowIndex].Cells[_SubjectNameIndex].ErrorText = "必須輸入學分數";
            //        dataGridViewX1.UpdateCellErrorText(_SubjectNameIndex, e.RowIndex);
            //    }
            //    else if (dataGridViewX1.Rows[e.RowIndex].Cells[_SubjectNameIndex].ErrorText == "必須輸入學分數")
            //    {
            //        dataGridViewX1.Rows[e.RowIndex].Cells[_SubjectNameIndex].ErrorText = "";
            //        dataGridViewX1.UpdateCellErrorText(_SubjectNameIndex, e.RowIndex);
            //    }
            //}
            #endregion
        }

        private void dataGridViewX1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0 && e.Button == MouseButtons.Right)
            {
                _SelectedRowIndex = e.RowIndex;
                foreach (DataGridViewRow var in dgv.SelectedRows)
                {
                    if (var.Index != _SelectedRowIndex)
                        var.Selected = false;
                }
                dgv.Rows[_SelectedRowIndex].Selected = true;
                contextMenuStrip1.Show(dgv, dgv.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
            }
        }

        private void dataGridViewX1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv.SelectedCells.Count == 1)
                dgv.BeginEdit(true);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            dgv.Rows.Insert(_SelectedRowIndex, new DataGridViewRow());
            _IsDirty = true;
            if (IsDirtyChanged != null)
                IsDirtyChanged.Invoke(this, new EventArgs());
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count - 1 > _SelectedRowIndex)
            {
                dgv.Rows.RemoveAt(_SelectedRowIndex);
            }
        }

        private void dataGridViewX1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //新的 row，預設列入成績計算
            if (dgv.Rows[e.RowIndex].IsNewRow)
                dgv.Rows[e.RowIndex].Cells[_CalcFlagIndex].Value = true;

            //for (int i = 0; i < e.RowCount; i++)
            //{
            //    //如果有前一筆資料則複製前筆資料
            //    if (i + e.RowIndex > 0)
            //    {
            //        foreach (int index in new int[] { _CategoryIndex, _RequiredByIndex, _RequiredIndex, _EntryIndex })
            //        {
            //            dgv.Rows[i + e.RowIndex].Cells[index].Value = dgv.Rows[i + e.RowIndex - 1].Cells[index].Value;
            //        }
            //    }
            //}
        }

        private void dataGridViewX1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            List<DataGridViewRow> deletedList = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in _RowSubject.Keys)
            {
                if (!dgv.Rows.Contains(row))
                    deletedList.Add(row);
            }
            if (deletedList.Count > 0)
            {
                foreach (DataGridViewRow row in deletedList)
                {
                    _RowSubject.Remove(row);
                }
                _RawDeleted = _IsDirty = true;
                if (IsDirtyChanged != null)
                    IsDirtyChanged.Invoke(this, new EventArgs());
            }
        }

        private void dataGridViewX1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgv.EndEdit();
        }

        private void dataGridViewX1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string message = "儲存格值：" + cell.Value + "。\n發生錯誤： " + e.Exception.Message + "。";
            if (cell.ErrorText != message)
            {
                cell.ErrorText = message;
                dgv.UpdateCellErrorText(e.ColumnIndex, e.RowIndex);
            }
        }

        private void dataGridViewX1_MouseHover(object sender, EventArgs e)
        {
            if (!dgv.ContainsFocus)
                dgv.Focus();
        }

        #endregion

        private List<int> ProcessLevels(DataGridViewRow row)
        {
            List<int> list = new List<int>();
            int countLevel = 0;
            //掃描開課學期
            for (int i = _CreditStartIndex; i < _CreditStartIndex + 8; i++)
            {
                if (row.Cells[i].Value != null)
                {
                    //壘計課程級別
                    countLevel++;
                }
            }
            int startLevel = 0;
            //如果有填數開始級別則以開始級別開始算
            if (row.Cells[_StartLevelIndex].Value != null)
                int.TryParse("" + row.Cells[_StartLevelIndex].Value, out startLevel);
            //計算科目級別
            if (countLevel > 1)
            {
                #region 自動計算科目級別
                //如果開課超過一學期且沒有填入開始級別，開始級別從1開始
                if (startLevel == 0)
                {
                    //沒填入開始級別則第一筆不加級別
                    list.Add(0);
                    startLevel = 1;
                    //填入開始級別第二筆開始會從2開始算級別
                    for (int i = 1; i < countLevel; i++)
                    {
                        list.Add(i + startLevel);
                    }
                }
                else
                {
                    //填入開始級別
                    for (int i = 0; i < countLevel; i++)
                    {
                        list.Add(i + startLevel);
                    }
                }
                #endregion
            }
            else
            {
                //有填入開始級別，但沒有開課或只開課一學期
                list.Add(startLevel);
            }
            return list;
        }

        private string GetNumber(int p)
        {
            string levelNumber;
            switch (p)
            {
                #region 對應levelNumber
                case 1:
                    levelNumber = "I";
                    break;
                case 2:
                    levelNumber = "II";
                    break;
                case 3:
                    levelNumber = "III";
                    break;
                case 4:
                    levelNumber = "IV";
                    break;
                case 5:
                    levelNumber = "V";
                    break;
                case 6:
                    levelNumber = "VI";
                    break;
                case 7:
                    levelNumber = "VII";
                    break;
                case 8:
                    levelNumber = "VIII";
                    break;
                case 9:
                    levelNumber = "IX";
                    break;
                case 10:
                    levelNumber = "X";
                    break;
                default:
                    levelNumber = "" + (p);
                    break;
                #endregion
            }
            return levelNumber;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public GraduationPlanEditor()
        {
            InitializeComponent();

            if (GlobalManager.Renderer is Office2007Renderer)
            {
                (GlobalManager.Renderer as Office2007Renderer).ColorTableChanged += delegate { this.dgv.AlternatingRowsDefaultCellStyle.BackColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.MouseOver.TopBackground.End; };
                this.dgv.AlternatingRowsDefaultCellStyle.BackColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.MouseOver.TopBackground.End;
            }
            
            chDomain.Items.Clear();
            chDomain.Items.Add(string.Empty);
            foreach (string domain in Subject.Domains)
                chDomain.Items.Add(domain);

            chSubject.Items.Clear();
            chSubject.Items.Add(string.Empty);
            foreach (string subject in Subject.Subjects)
                chSubject.Items.Add(subject);

            _CategoryIndex = Column1.Index;
            _DomainIndex = chDomain.Index;
            _RequiredByIndex = Column3.Index;
            _RequiredIndex = Column4.Index;
            _SubjectNameIndex = chSubject.Index;
            _EntryIndex = Column13.Index;
            _StartLevelIndex = Column6.Index;
            _CreditStartIndex = Column7.Index;
            _NotIncludedInCreditIndex = Column17.Index;
            _CalcFlagIndex = Column16.Index;

            dgv.CurrentCell = dgv.FirstDisplayedCell;
            if(dgv.CurrentCell !=null)
                dgv.BeginEdit(true);
        }

        #region IGraduationPlanEditor 成員

        public void SetSource(System.Xml.XmlElement source)
        {
            _defaultValues = new Dictionary<DataGridViewCell, object>();
            _RowSubject = new Dictionary<DataGridViewRow, string>();
            _DirtyCells = new List<DataGridViewCell>();
            dgv.Rows.Clear();
            _RawDeleted = _IsDirty = false;
            Dictionary<string, DataGridViewRow> rowDictionary = new Dictionary<string, DataGridViewRow>();
            if (source != null)
            {
                foreach (XmlNode node in source.SelectNodes("Subject"))
                {
                    DataGridViewRow row;
                    XmlElement element = (XmlElement)node;
                    XmlNode groupNode = element.SelectSingleNode("Grouping");
                    //檢查是否符合群組設定
                    if (groupNode != null && groupNode.SelectSingleNode("@RowIndex") != null)
                    {
                        #region 以第一筆資料為主填入各級別學年度學期學分數
                        XmlElement groupElement = (XmlElement)groupNode;
                        if (!rowDictionary.ContainsKey(groupElement.Attributes["RowIndex"].InnerText))
                        {
                            row = new DataGridViewRow();
                            row.CreateCells(dgv, "", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
                            dgv.Rows.Add(row);
                            //row.Cells[_CategoryIndex].Value = element.Attributes["Category"].InnerText;

                            row.Cells[_DomainIndex].Value = element.GetAttribute("Domain");
                            //row.Cells[_DomainIndex].Value = string.Empty;
                            //string domain = element.GetAttribute("Domain");
                            //if (_domainList.Contains(domain))
                            //    row.Cells[_DomainIndex].Value = domain;
                            //else
                            //{
                            //    _IsDirty = true;
                            //    if (IsDirtyChanged != null)
                            //        IsDirtyChanged.Invoke(this, new EventArgs());
                            //}

                            //row.Cells[_RequiredByIndex].Value = element.Attributes["RequiredBy"].InnerText;
                            //row.Cells[_RequiredIndex].Value = element.Attributes["Required"].InnerText;
                            row.Cells[_SubjectNameIndex].Value = element.Attributes["SubjectName"].InnerText;

                            row.Cells[_CalcFlagIndex].Value = bool.Parse(element.GetAttribute("CalcFlag"));
                            ////舊版沒有下面這兩個 Attributes
                            //if (element.HasAttribute("NotIncludedInCredit") && element.HasAttribute("NotIncludedInCalc") && element.Attributes["NotIncludedInCredit"].Value != "" && element.Attributes["NotIncludedInCalc"].Value != "")
                            //{
                            //    row.Cells[_NotIncludedInCreditIndex].Value = bool.Parse(element.Attributes["NotIncludedInCredit"].Value);
                            //    row.Cells[_NotIncludedInCalcIndex].Value = bool.Parse(element.Attributes["NotIncludedInCalc"].Value);
                            //}

                            //if (element.HasAttribute("Entry"))
                            //{
                            //    switch (element.GetAttribute("Entry"))
                            //    {
                            //        default:
                            //        case "學業":
                            //            row.Cells[_EntryIndex].Value = "學業";
                            //            break;
                            //        case "體育":
                            //            row.Cells[_EntryIndex].Value = "體育";
                            //            break;
                            //        case "國防通識":
                            //            row.Cells[_EntryIndex].Value = "國防通識(軍訓)";
                            //            break;
                            //        case "健康與護理":
                            //            row.Cells[_EntryIndex].Value = "健康與護理";
                            //            break;
                            //        case "實習科目":
                            //            row.Cells[_EntryIndex].Value = "實習科目";
                            //            break;
                            //    }
                            //}
                            //else
                            //{
                            //    row.Cells[_EntryIndex].Value = "學業";
                            //}

                            row.Cells[_StartLevelIndex].Value = element.Attributes["Level"].InnerText;
                            rowDictionary.Add(groupElement.Attributes["RowIndex"].InnerText, row);
                            //呼叫結束編輯處理函式
                            dataGridViewX1_CellEndEdit(this, new DataGridViewCellEventArgs(_SubjectNameIndex, row.Index));
                        }
                        else
                            row = rowDictionary[groupElement.Attributes["RowIndex"].InnerText];
                        //填入自動開課資料(年級學期學分數)
                        int gradeyear = 0;
                        int semester = 0;
                        int cellIndex = 0;
                        if (int.TryParse(element.Attributes["GradeYear"].InnerText, out gradeyear) && int.TryParse(element.Attributes["Semester"].InnerText, out semester))
                        {
                            string credit = element.GetAttribute("Credit");
                            string period = element.GetAttribute("Period");

                            cellIndex = (gradeyear - 1) * 2 + semester + _CreditStartIndex - 1;
                            DataGridViewCell cell = row.Cells[cellIndex];

                            PeriodCredit pc = new PeriodCredit();
                            if (pc.Parse(period + "/" + credit))
                            {
                                cell.Value = pc.ToString();
                            }
                            else
                            {
                                cell.Value = string.Empty;
                                cell.ErrorText = pc.Error;
                            }

                            //if (string.IsNullOrEmpty(period)) period = credit;

                            //if (credit.CompareTo(period) == 0) //節數與權數相等，則不需要用'/'
                            //    row.Cells[(gradeyear - 1) * 2 + semester + _CreditStartIndex - 1].Value = credit;
                            //else
                            //    row.Cells[(gradeyear - 1) * 2 + semester + _CreditStartIndex - 1].Value = period + "/" + credit;
                        }
                        //呼叫結束編輯處理函式
                        dataGridViewX1_CellEndEdit(this, new DataGridViewCellEventArgs(cellIndex, row.Index));
                        #endregion

                    }
                    else
                    {
                        #region 以該科別的開始級別做為開始級別
                        row = new DataGridViewRow();
                        row.CreateCells(dgv, "", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
                        dgv.Rows.Add(row);
                        //row.Cells[_CategoryIndex].Value = element.Attributes["Category"].InnerText;
                        row.Cells[_DomainIndex].Value = element.GetAttribute("Domain");
                        //row.Cells[_DomainIndex].Value = string.Empty;
                        //string domain = element.GetAttribute("Domain");
                        //if (_domainList.Contains(domain))
                        //    row.Cells[_DomainIndex].Value = domain;
                        //else
                        //{
                        //    _IsDirty = true;
                        //    if (IsDirtyChanged != null)
                        //        IsDirtyChanged.Invoke(this, new EventArgs());
                        //}

                        //row.Cells[_RequiredByIndex].Value = element.Attributes["RequiredBy"].InnerText;
                        //row.Cells[_RequiredIndex].Value = element.Attributes["Required"].InnerText;
                        row.Cells[_SubjectNameIndex].Value = element.Attributes["SubjectName"].InnerText;
                        //row.Cells[_NotIncludedInCreditIndex].Value = element.Attributes["NotIncludedInCredit"].InnerText == "True" ? true : false;
                        row.Cells[_CalcFlagIndex].Value = element.GetAttribute("CalcFlag") == "True" ? true : false;
                        //if (element.HasAttribute("Entry"))
                        //{
                        //    switch (element.GetAttribute("Entry"))
                        //    {
                        //        default:
                        //        case "學業":
                        //            row.Cells[_EntryIndex].Value = "學業";
                        //            break;
                        //        case "體育":
                        //            row.Cells[_EntryIndex].Value = "體育";
                        //            break;
                        //        case "國防通識":
                        //            row.Cells[_EntryIndex].Value = "國防通識(軍訓)";
                        //            break;
                        //        case "健康與護理":
                        //            row.Cells[_EntryIndex].Value = "健康與護理";
                        //            break;
                        //        case "實習科目":
                        //            row.Cells[_EntryIndex].Value = "實習科目";
                        //            break;
                        //    }
                        //}
                        //else
                        //    row.Cells[_EntryIndex].Value = "學業";

                        row.Cells[_StartLevelIndex].Value = element.Attributes["Level"].InnerText;
                        //填入自動開課資料(年級學期學分數)
                        int gradeyear = 0;
                        int semester = 0;
                        int cellIndex = 0;
                        if (int.TryParse(element.Attributes["GradeYear"].InnerText, out gradeyear) && int.TryParse(element.Attributes["Semester"].InnerText, out semester))
                        {
                            string period = element.GetAttribute("Period");
                            string credit = element.GetAttribute("Credit");
                            if (string.IsNullOrEmpty(period)) period = credit;

                            cellIndex = (gradeyear - 1) * 2 + semester + _CreditStartIndex - 1;
                            DataGridViewCell cell = row.Cells[cellIndex];

                            PeriodCredit pc = new PeriodCredit();
                            if (pc.Parse(period + "/" + credit))
                            {
                                cell.Value = pc.ToString();
                            }
                            else
                            {
                                cell.Value = string.Empty;
                                cell.ErrorText = pc.Error;
                            }

                            //if (credit.CompareTo(period) == 0) //節數與權數相等，則不需要用'/'
                            //    row.Cells[(gradeyear - 1) * 2 + semester + _CreditStartIndex - 1].Value = credit;
                            //else
                            //    row.Cells[(gradeyear - 1) * 2 + semester + _CreditStartIndex - 1].Value = period + "/" + credit;
                        }
                        #endregion
                        //呼叫結束編輯處理函式
                        dataGridViewX1_CellEndEdit(this, new DataGridViewCellEventArgs(_SubjectNameIndex, row.Index));

                    }
                }
            }
            if (this.IsValidated) { }
            dgv.CurrentCell = dgv.FirstDisplayedCell;
            if(dgv.CurrentCell !=null)
                dgv.BeginEdit(true);
            ValidateSameSubject();
        }

        public System.Xml.XmlElement GetSource()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<GraduationPlan/>");
            int rowIndex = 0;
            //掃每一列資料
            foreach (DataGridViewRow row in dgv.Rows)
            {
                //有表示科目欄有填寫
                if (_RowSubject.ContainsKey(row))
                {
                    rowIndex++;
                    //節數
                    List<decimal> Periods = new List<decimal>();
                    //權數
                    List<decimal> Credits = new List<decimal>();

                    //記錄每個級別所包含的學期
                    List<int> Semesters = new List<int>();
                    //記錄每個級別所包含的學年度
                    List<int> GradeYears = new List<int>();
                    //未加級別前的科目名稱
                    string subjectName = _RowSubject[row];
                    //int countLevel = 0;
                    //掃描開課學期
                    for (int i = _CreditStartIndex; i < _CreditStartIndex + 8; i++)
                    {
                        if (row.Cells[i].Value != null)
                        {
                            string orig = ("" + row.Cells[i].Value).Trim();
                            PeriodCredit pc = new PeriodCredit();
                            if (pc.Parse(orig))
                            {
                                Credits.Add(pc.Credit);
                                Periods.Add(pc.Period);
                            }

                            Semesters.Add((i - _CreditStartIndex + 2) % 2 + 1);
                            GradeYears.Add((i - _CreditStartIndex + 2) / 2);
                        }
                    }

                    //int startLevel = 0;
                    ////如果有填數開始級別則以開始級別開始算
                    //if (row.Cells[_StartLevelIndex].Value != null)
                    //    int.TryParse("" + row.Cells[_StartLevelIndex].Value, out startLevel);
                    //這個row中包含的數個科目級別
                    XmlElement parentElement;
                    //全組資料(回填用)
                    XmlElement grouping;

                    //這個row中包含的數個科目級別
                    parentElement = doc.CreateElement("Subject");
                    //全組資料(回填用)
                    grouping = doc.CreateElement("Grouping");
                    grouping.SetAttribute("RowIndex", rowIndex.ToString());
                    grouping.SetAttribute("startLevel", row.Cells[_StartLevelIndex].Value == null ? "" : row.Cells[_StartLevelIndex].Value.ToString());
                    parentElement.AppendChild(grouping);
                    //建立新Element加入至doc
                    parentElement.SetAttribute("SubjectName", subjectName);
                    parentElement.SetAttribute("Category", row.Cells[_CategoryIndex].Value == null ? "" : row.Cells[_CategoryIndex].Value.ToString());
                    parentElement.SetAttribute("Domain", row.Cells[_DomainIndex].Value == null ? "" : row.Cells[_DomainIndex].Value.ToString());
                    parentElement.SetAttribute("RequiredBy", row.Cells[_RequiredByIndex].Value == null ? "" : row.Cells[_RequiredByIndex].Value.ToString());
                    parentElement.SetAttribute("Required", row.Cells[_RequiredIndex].Value == null ? "" : row.Cells[_RequiredIndex].Value.ToString());

                    bool b;
                    bool.TryParse(row.Cells[_NotIncludedInCreditIndex].Value == null ? "false" : row.Cells[_NotIncludedInCreditIndex].Value.ToString(), out b);
                    parentElement.SetAttribute("NotIncludedInCredit", b.ToString());
                    bool.TryParse(row.Cells[_CalcFlagIndex].Value == null ? "false" : row.Cells[_CalcFlagIndex].Value.ToString(), out b);
                    parentElement.SetAttribute("CalcFlag", b.ToString());

                    #region 填入分項類別
                    //switch ("" + row.Cells[_EntryIndex].Value)
                    //{
                    //    default:
                    //    case "學業":
                    //        parentElement.SetAttribute("Entry", "學業");
                    //        break;
                    //    case "體育":
                    //        parentElement.SetAttribute("Entry", "體育");
                    //        break;
                    //    case "國防通識(軍訓)":
                    //        parentElement.SetAttribute("Entry", "國防通識");
                    //        break;
                    //    case "健康與護理":
                    //        parentElement.SetAttribute("Entry", "健康與護理");
                    //        break;
                    //    case "實習科目":
                    //        parentElement.SetAttribute("Entry", "實習科目");
                    //        break;
                    //}
                    #endregion


                    #region 計算科目級別
                    List<int> levelList = ProcessLevels(row);
                    if (levelList.Count == 0)
                        throw new Exception("輸入資料無法計算學分數");
                    if (levelList.Count == 1 && levelList[0] == 0)
                    {
                        XmlElement element = (XmlElement)parentElement.CloneNode(true);
                        element.SetAttribute("FullName", subjectName);
                        element.SetAttribute("Level", row.Cells[_StartLevelIndex].Value == null ? "" : row.Cells[_StartLevelIndex].Value.ToString());

                        element.SetAttribute("Credit", Credits[0].ToString());
                        element.SetAttribute("Period", Periods[0].ToString());
                        element.SetAttribute("GradeYear", GradeYears[0].ToString());
                        element.SetAttribute("Semester", Semesters[0].ToString());

                        doc.DocumentElement.AppendChild(element);
                    }
                    else
                    {
                        int index = 0;
                        //沒輸入開始級別，第一筆沒有級別，第二筆以後從2開始
                        XmlElement element = (XmlElement)parentElement.CloneNode(true);
                        if (levelList[0] == 0)
                        {
                            #region 單獨處理第一筆

                            element.SetAttribute("FullName", subjectName);
                            element.SetAttribute("Level", "");

                            element.SetAttribute("Credit", Credits[0].ToString());
                            element.SetAttribute("Period", Periods[0].ToString());
                            element.SetAttribute("GradeYear", GradeYears[0].ToString());
                            element.SetAttribute("Semester", Semesters[0].ToString());

                            doc.DocumentElement.AppendChild(element);
                            #endregion
                            //接下來從第二筆開始
                            index = 1;
                        }
                        for (int i = index; i < levelList.Count; i++)
                        {
                            element = (XmlElement)parentElement.CloneNode(true);

                            element.SetAttribute("FullName", subjectName + " " + GetNumber(levelList[i]));
                            element.SetAttribute("Level", (levelList[i]).ToString());

                            element.SetAttribute("Credit", Credits[i].ToString());
                            element.SetAttribute("Period", Periods[i].ToString());
                            element.SetAttribute("GradeYear", GradeYears[i].ToString());
                            element.SetAttribute("Semester", Semesters[i].ToString());

                            doc.DocumentElement.AppendChild(element);
                        }
                    }
                    #region OldWay
                    //if (countLevel == 0)
                    //    throw new Exception("輸入資料無法計算學分數");
                    //if (countLevel == 1 && startLevel == 0)
                    //{
                    //    XmlElement element = (XmlElement)parentElement.CloneNode(true);
                    //    element.SetAttribute("FullName", subjectName);
                    //    element.SetAttribute("Level", row.Cells[_StartLevelIndex].Value == null ? "" : row.Cells[_StartLevelIndex].Value.ToString());

                    //    element.SetAttribute("Credit", Credits[0].ToString());
                    //    element.SetAttribute("GradeYear", GradeYears[0].ToString());
                    //    element.SetAttribute("Semester", Semesters[0].ToString());

                    //    doc.DocumentElement.AppendChild(element);
                    //}
                    //else//countLevel>1
                    //{
                    //    //沒輸入開始級別，第一筆沒有級別，第二筆以後從2開始
                    //    if (startLevel == 0)
                    //    {
                    //        #region 單獨處理第一筆
                    //        XmlElement element = (XmlElement)parentElement.CloneNode(true);

                    //        element.SetAttribute("FullName", subjectName);
                    //        element.SetAttribute("Level", "");

                    //        element.SetAttribute("Credit", Credits[0].ToString());
                    //        element.SetAttribute("GradeYear", GradeYears[0].ToString());
                    //        element.SetAttribute("Semester", Semesters[0].ToString());

                    //        doc.DocumentElement.AppendChild(element); 
                    //        #endregion
                    //        //填入開始級別
                    //        startLevel = 1;
                    //        for (int i = 1; i < countLevel; i++)
                    //        {
                    //            #region 第二筆之後
                    //            element = (XmlElement)parentElement.CloneNode(true);

                    //            element.SetAttribute("FullName", subjectName + " " + GetNumber(i + startLevel));
                    //            element.SetAttribute("Level", (i + startLevel).ToString());

                    //            element.SetAttribute("Credit", Credits[i].ToString());
                    //            element.SetAttribute("GradeYear", GradeYears[i].ToString());
                    //            element.SetAttribute("Semester", Semesters[i].ToString());

                    //            doc.DocumentElement.AppendChild(element); 
                    //            #endregion
                    //        }
                    //    }
                    //    else
                    //    {
                    //        //填入開始級別
                    //        for (int i = 0; i < countLevel; i++)
                    //        {
                    //            XmlElement element = (XmlElement)parentElement.CloneNode(true);

                    //            element.SetAttribute("FullName", subjectName + " " + GetNumber(i + startLevel));
                    //            element.SetAttribute("Level", (i + startLevel).ToString());

                    //            element.SetAttribute("Credit", Credits[i].ToString());
                    //            element.SetAttribute("GradeYear", GradeYears[i].ToString());
                    //            element.SetAttribute("Semester", Semesters[i].ToString());

                    //            doc.DocumentElement.AppendChild(element);
                    //        }
                    //    }
                    //} 
                    #endregion
                    #endregion.
                }
            }
            return doc.DocumentElement;
        }

        public bool IsDirty
        {
            get { return _IsDirty | _RawDeleted; }
        }

        public event EventHandler IsDirtyChanged;

        public bool IsValidated
        {
            get
            {
                dgv.EndEdit();

                #region 如果 DataGridView 中有任何的 ErrorText，驗證就失敗
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.ErrorText != "")
                        return false;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.ErrorText != "")
                            return false;
                    }
                }
                #endregion

                if (!ValidateSameSubject())
                    return false;

                return true;
            }
        }

        #endregion

        /// <summary>
        /// 驗證是否有相同科目名稱的資料
        /// </summary>
        /// <returns></returns>
        private bool ValidateSameSubject()
        {
            bool pass = true;
            List<string> subjectList = new List<string>();
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                string key = "" + row.Cells[_SubjectNameIndex].Value;
                if (!subjectList.Contains(key))
                {
                    subjectList.Add(key);
                    row.Cells[_SubjectNameIndex].ErrorText = "";
                }
                else
                {
                    pass = false;
                    row.Cells[_SubjectNameIndex].ErrorText = "已經有相同科目名稱的資料存在";
                }
            }
            return pass;
        }

        private void dgv_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            //if (e.Row.IsNewRow)
            //    e.Row.Cells[_CalcFlagIndex].Value = true;
        }
    }
}
