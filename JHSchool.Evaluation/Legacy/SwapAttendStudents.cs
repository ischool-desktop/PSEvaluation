using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using DevComponents.DotNetBar;
//using SmartSchool.Customization.Data;
using System.Xml;
using FISCA.DSAUtil;
using DevComponents.DotNetBar.Rendering;
using JHSchool;
using JHSchool.Evaluation.Legacy;
using Aspose.Cells;
using FISCA.Presentation;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Evaluation.Feature.Legacy;

namespace JHSchool.Evaluation.Legacy
{
    public partial class SwapAttendStudents : FISCA.Presentation.Controls.BaseForm
    {
        private SortedList<string, string> _CourseName = new SortedList<string, string>();
        private SortedList<string, Color> _CourseColor = new SortedList<string, Color>();
        Dictionary<string, List<AttInfo>> _StudentAttenCourses = new Dictionary<string, List<AttInfo>>();
        List<AttInfo> _AttendInfoList = new List<AttInfo>();
        //AccessHelper _AccessHelper = new AccessHelper();
        private Dictionary<DataGridViewRow, int> _RowIndex = new Dictionary<DataGridViewRow, int>();

        public SwapAttendStudents(int selectCourseCount)
        {
            InitializeComponent();
            this.pictureBox1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            List<string> list = new List<string>();

            //2018/8/15 穎驊註記， 舊寫法， 上限只能給七個課程互調，給了七種顏色，更正為下列依每一次選擇的課程數不同隨機產生顏色
            //Color[] colors = new Color[] { Color.Red, Color.Yellow, Color.Blue, Color.PowderBlue, Color.Orange, Color.Green, Color.Purple};

            List<Color> colorsList = new List<Color>();
            for (int runs = 0; runs < selectCourseCount; runs++)
            {
                Random r = new Random(runs); // 每一輪的種子都不同
                colorsList.Add(Color.FromArgb(r.Next(0,256), r.Next(0, 256), r.Next(0, 256)));
            }

            Color[] colors = colorsList.ToArray();

                        
            if (GlobalManager.Renderer is Office2007Renderer)
            {
                (GlobalManager.Renderer as Office2007Renderer).ColorTableChanged += delegate { this.dataGridViewX1.AlternatingRowsDefaultCellStyle.BackColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.MouseOver.TopBackground.End; };
                this.dataGridViewX1.AlternatingRowsDefaultCellStyle.BackColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.MouseOver.TopBackground.End;
            }
            int i = 0;
            foreach (CourseRecord cinfo in Course.Instance.SelectedList)
            {
                ButtonX item1 = new ButtonX();
                item1.FocusCuesEnabled = false;
                item1.Style = eDotNetBarStyle.Office2007;
                item1.ColorTable = eButtonColor.Flat;// eButtonColor.Office2007WithBackground;
                //item1.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
                //item1.ImagePaddingHorizontal = 8;
                item1.AutoSize = true;
                item1.Shape = new DevComponents.DotNetBar.RoundRectangleShapeDescriptor(15);
                item1.TextAlignment = eButtonTextAlignment.Left;
                item1.Size = new Size(110, 23);
                item1.Text = cinfo.Name;
                item1.Image = GetColorBallImage(colors[i]);
                item1.Tag = "" + cinfo.ID;
                item1.Click += new EventHandler(Swap);
                list.Add("" + cinfo.ID);
                _CourseName.Add("" + cinfo.ID, cinfo.Name);
                _CourseColor.Add("" + cinfo.ID, colors[i++]);
                this.flowLayoutPanel1.Controls.Add(item1);
            }
            BackgroundWorker bkw = new BackgroundWorker();
            bkw.DoWork += new DoWorkEventHandler(bkw_DoWork);
            bkw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bkw_RunWorkerCompleted);
            bkw.RunWorkerAsync(list.ToArray());
        }

        private void Swap(object sender, EventArgs e)
        {
            ButtonX item1 = (ButtonX)sender;
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                ((AttInfo)row.Tag).CourseID = "" + item1.Tag;
                ((DataGridViewColorBallTextCell)row.Cells[8]).Color = _CourseColor["" + item1.Tag];
                ((DataGridViewColorBallTextCell)row.Cells[8]).Value = _CourseName["" + item1.Tag];
            }
            CountStudents();
        }

        private void CountStudents()
        {
            Dictionary<string, List<string>> courseStudents = new Dictionary<string, List<string>>();
            foreach (AttInfo attendInfo in _AttendInfoList)
            {
                if (!courseStudents.ContainsKey(attendInfo.CourseID))
                    courseStudents.Add(attendInfo.CourseID, new List<string>());
                courseStudents[attendInfo.CourseID].Add(attendInfo.StudentID);
            }
            foreach (Control var in flowLayoutPanel1.Controls)
            {
                string courseID = "" + var.Tag;
                if (!courseStudents.ContainsKey(courseID))
                {
                    var.Text = _CourseName[courseID] + "(0人)";
                }
                else
                {
                    int totle = courseStudents[courseID].Count;
                    int b = 0, g = 0;

                    foreach (StudentRecord studentRec in Student.Instance.GetStudents(courseStudents[courseID].ToArray()))
                    {
                        if (studentRec.Gender == "男") b++;
                        if (studentRec.Gender == "女") g++;
                    }
                    var.Text = _CourseName[courseID] + "(" + (b > 0 ? " " + b + "男" : "") + (g > 0 ? " " + g + "女" : "") + (totle - b - g > 0 ? " " + (totle - b - g) + "未知性別" : "") + " 共" + totle + "人" + " )";
                }
            }
        }

        private void bkw_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] list = (string[])e.Argument;

            DSResponse rsp = QueryCourse.GetSCAttend(list);
            foreach (XmlElement each in rsp.GetContent().GetElements("Student"))
            {
                string attendID = each.GetAttribute("ID");
                string studentID = each.SelectSingleNode("RefStudentID").InnerText;
                string courseID = each.SelectSingleNode("RefCourseID").InnerText;

                AttInfo attInfo = new AttInfo(attendID, courseID, studentID);
                if (!_StudentAttenCourses.ContainsKey(studentID))
                    _StudentAttenCourses.Add(studentID, new List<AttInfo>());
                _StudentAttenCourses[studentID].Add(attInfo);

                _AttendInfoList.Add(attInfo);
            }
        }

        private void bkw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
            List<StudentRecord> students = Student.Instance.GetStudents(new List<string>(_StudentAttenCourses.Keys).ToArray());
            foreach (StudentRecord studentRec in students)
            {
                int newRowIndex = dataGridViewX1.Rows.Add(studentRec.StudentNumber, studentRec.Name, studentRec.Gender, "", studentRec.Class == null ? "" : studentRec.Class.Name, studentRec.SeatNo,"",
                     _StudentAttenCourses[studentRec.ID].Count == 1 ? _CourseName[_StudentAttenCourses[studentRec.ID][0].CourseID] : "多重修課",
                     _StudentAttenCourses[studentRec.ID].Count == 1 ? _CourseName[_StudentAttenCourses[studentRec.ID][0].CourseID] : "多重修課");
                if (_StudentAttenCourses[studentRec.ID].Count == 1)
                {
                    dataGridViewX1.Rows[newRowIndex].Tag = _StudentAttenCourses[studentRec.ID][0];
                    ((DataGridViewColorBallTextCell)dataGridViewX1.Rows[newRowIndex].Cells[7]).Color = _CourseColor[_StudentAttenCourses[studentRec.ID][0].CourseID];
                    ((DataGridViewColorBallTextCell)dataGridViewX1.Rows[newRowIndex].Cells[8]).Color = _CourseColor[_StudentAttenCourses[studentRec.ID][0].CourseID];
                }
                else
                {
                    dataGridViewX1.Rows[newRowIndex].ReadOnly = true;
                    foreach (DataGridViewCell cell in dataGridViewX1.Rows[newRowIndex].Cells)
                    {
                        cell.Style.BackColor = Color.Gray;
                    }
                }
            }
            dataGridViewX1_SelectionChanged(null, null);
            dataGridViewX1_Sorted(null, null);
            CountStudents();
        }

        public Image GetColorBallImage(Color color)
        {
            Bitmap bmp = new Bitmap(16, 16);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            int w = 14,
                    x = 1,
                    y = 1;
            Color[] myColors = { color, Color.White, color, color };
            float[] myPositions = { 0.0f, 0.05f, 0.6f, 1.0f };
            ColorBlend myBlend = new ColorBlend();
            myBlend.Colors = myColors;
            myBlend.Positions = myPositions;
            using (LinearGradientBrush brush = new LinearGradientBrush(new Point(x, y), new Point(w, w), Color.White, color))
            {
                brush.InterpolationColors = myBlend;
                brush.GammaCorrection = true;
                graphics.FillRectangle(brush, x, y, w, w);
            }
            graphics.DrawRectangle(new Pen(Color.Black), x, y, w, w);
            return bmp;
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SwapAttendStudents_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Location = new Point((this.Width - pictureBox1.Width) / 2, (this.Height - pictureBox1.Height) / 2);
        }

        private void dataGridViewX1_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                if (row.Tag == null)
                { row.Selected = false; return; }
            }
            labelX1.Text = "選取" + dataGridViewX1.SelectedRows.Count + "人";
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            List<string> AttendList = new List<string>();
            this.UseWaitCursor = true;
            int changedCount = 0;
            DSXmlHelper helper = new DSXmlHelper("UpdateSCAttend");
            foreach (AttInfo each in _AttendInfoList)
            {
                if (each.HasChanged)
                {
                    changedCount++;
                    XmlElement attend = helper.AddElement("Attend");
                    DSXmlHelper.AppendChild(attend, "<ID>" + each.AttendID + "</ID>");
                    DSXmlHelper.AppendChild(attend, "<RefCourseID>" + each.CourseID + "</RefCourseID>");
                    helper.AddElement(".", attend);
                    AttendList.Add(each.AttendID);
                }
            }
            if (changedCount > 0)
                EditCourse.UpdateAttend(helper);
            //更新課程List資料
            //SCAttend.Instance.SyncDataBackground(AttendList);
            FISCA.Presentation.MotherForm.SetStatusBarMessage("調整修課學生完成。");


            //Customization.PlugIn.Global.SetStatusBarMessage("調整修課學生完成。");
            this.UseWaitCursor = false;
            this.Close();
        }

        private void dataGridViewX1_Sorted(object sender, EventArgs e)
        {
            _RowIndex.Clear();
            foreach (DataGridViewRow row in this.dataGridViewX1.Rows)
            {
                _RowIndex.Add(row, row.Index);
            }
        }

        private void dataGridViewX1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column == colSeatNo)//座號以數字排序
                e.SortResult = (int.Parse(e.CellValue1.ToString() == "" ? "0" : e.CellValue1.ToString())).CompareTo(int.Parse(e.CellValue2.ToString() == "" ? "0" : e.CellValue2.ToString()));
            else
                e.SortResult = ("" + e.CellValue1).CompareTo("" + e.CellValue2);
            if (e.SortResult == 0)
                e.SortResult = (_RowIndex[dataGridViewX1.Rows[e.RowIndex1]]).CompareTo(_RowIndex[dataGridViewX1.Rows[e.RowIndex2]]);
            e.Handled = true;
        }

        //匯入 學生排序資料
        private void ImportSeqBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            //od.Filter = "Excel檔案(*.xlsx)|*.xlsx|Excel檔案(*.xls)|*.xls"; //Aspose.Cells_201402 寫法 日後換參考

            od.Filter = "Excel檔案(*.xls)|*.xls";

            if (od.ShowDialog() != DialogResult.OK) return;

            //Workbook book = new Workbook(od.FileName); //Aspose.Cells_201402 寫法 日後換參考
            Workbook book = new Workbook();

            try
            {
                book.Open(od.FileName);
            }
            catch (Exception ex)
            {
                MsgBox.Show("開啟失敗。" + ex.Message);

                return;
            }


            Worksheet ws = book.Worksheets[0];

            int index = 0;
            Dictionary<string, int> colmap = new Dictionary<string, int>();
            Dictionary<int, int> map = new Dictionary<int, int>();

            // 驗證欄位
            List<string> fields = new List<string>();

            fields.Add("學號");
            fields.Add("分班排序");

            Dictionary<string, IColumnValidator> validators = new Dictionary<string, IColumnValidator>();

            #region 檢查標題是否正確
            for (int i = 0; i <= ws.Cells.MaxDataColumn; i++)
            {
                Cell cell = ws.Cells[index, i];
                string value = "" + cell.Value;

                int dgvColIndex;
                if (fields.Contains(value) && TryGetColumnIndex(dataGridViewX1, value, out dgvColIndex))
                {
                    colmap.Add(value, i);
                    map.Add(i, dgvColIndex);
                    fields.Remove(value);
                }
            }

            if (fields.Count > 0)
            {
                StringBuilder builder = new StringBuilder("");
                builder.AppendLine("匯入資料有誤。");
                builder.Append("缺少欄位：");
                foreach (var f in fields)
                    builder.Append(f + "、");
                string msg = builder.ToString();
                if (msg.EndsWith("、")) msg = msg.Substring(0, msg.Length - 1);
                MessageBox.Show(msg);
                return;
            }
            #endregion

            #region 檢查欄位是否有效
            bool valid = true;
            foreach (string header in validators.Keys)
            {
                if (!colmap.ContainsKey(header)) continue;

                int colIndex = colmap[header];
                IColumnValidator validator = validators[header];
                for (int i = 1; i <= ws.Cells.MaxDataRow; i++)
                {
                    Cell cell = ws.Cells[i, colIndex];
                    if (!validator.IsValid("" + cell.Value))
                    {                        
                        valid &= false;
                    }
                }
            }
            if (!valid)
            {
                MsgBox.Show("資料有誤。");
                return;
            }
            #endregion

            #region 填入 DataGridView
            if (MsgBox.Show("匯入動作會將會新增排序資料，請問是否要繼續？", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            dataGridViewX1.SuspendLayout();
            //dataGridViewX1.Rows.Clear();

            Dictionary<string, string> stuNumberSeqDict = new Dictionary<string, string>();

            for (int i = 1; i <= ws.Cells.MaxDataRow; i++)
            {
                // 舊寫法，全面置換 UI 介面
                //DataGridViewRow row = new DataGridViewRow();
                //row.CreateCells(dataGridViewX1);
                //foreach (int colIndex in map.Keys)
                //{
                //    Cell cell = ws.Cells[i, colIndex];
                //    row.Cells[map[colIndex]].Value = "" + cell.Value;
                //}
                //dataGridViewX1.Rows.Add(row);
                if (!stuNumberSeqDict.ContainsKey("" + ws.Cells[i, 0].Value))
                {
                    stuNumberSeqDict.Add("" + ws.Cells[i, colmap["學號"]].Value, "" + ws.Cells[i, colmap["分班排序"]].Value);
                }                                
            }

            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                row.Cells[6].Value = stuNumberSeqDict.ContainsKey("" + row.Cells[0].Value) ? stuNumberSeqDict["" + row.Cells[0].Value] : "";
            }

            dataGridViewX1.ResumeLayout();
            #endregion

            MsgBox.Show("匯入完成，可以點擊排序後重新指定分班。");

        }


        // 匯出畫面 dgv
        private void ExportResultBtn_Click(object sender, EventArgs e)
        {
            Workbook book = new Workbook();
            book.Worksheets.Clear();
            Worksheet ws = book.Worksheets[book.Worksheets.Add()];
            ws.Name = "課程重新分班檢查";

            int index = 0;
            Dictionary<string, int> map = new Dictionary<string, int>();

            #region 建立標題
            for (int i = 0; i < dataGridViewX1.Columns.Count; i++)
            {
                DataGridViewColumn col = dataGridViewX1.Columns[i];
                ws.Cells[index, i].PutValue(col.HeaderText);
                map.Add(col.HeaderText, i);
            }
            index++;
            #endregion

            #region 填入內容
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    int column = map[cell.OwningColumn.HeaderText];
                    ws.Cells[index, column].PutValue("" + cell.Value);
                }
                index++;
            }
            #endregion
            
            ws.Cells.DeleteColumn(3); // 刪除科別欄位 ，國小、國中不需此欄位

            SaveFileDialog sd = new SaveFileDialog();
            sd.FileName = "課程重新分班檢查";
            //sd.Filter = "Excel檔案(*.xlsx)|*.xlsx";
            sd.Filter = " Excel檔案(*.xls) | *.xls";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                DialogResult result = new DialogResult();

                try
                {
                    //book.Save(sd.FileName, SaveFormat.Xlsx); //Aspose.Cells_201402 寫法 日後換參考

                    book.Save(sd.FileName, FileFormatType.Excel2003);

                    result = MsgBox.Show("檔案儲存完成，是否開啟檔案?", "是否開啟", MessageBoxButtons.YesNo);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存失敗。" + ex.Message);
                }

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(sd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MsgBox.Show("開啟檔案發生失敗:" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }



            }
        }

        private static bool TryGetColumnIndex(DataGridView dgv, string headerText, out int colIndex)
        {
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.HeaderText == headerText)
                {
                    colIndex = col.Index;
                    return true;
                }
            }
            colIndex = 0;
            return false;
        }

        internal interface IColumnValidator
        {
            bool IsValid(string input);
            string GetErrorMessage();
        }


    }
    class AttInfo
    {
        private string _AttendID, _StudentID, _CourseID, _OldCourseID;
        public bool HasChanged { get { return _CourseID != _OldCourseID; } }
        public string AttendID { get { return _AttendID; } }
        public string CourseID { get { return _CourseID; } set { _CourseID = value; } }
        public string StudentID { get { return _StudentID; } }
        public AttInfo(string attendID, string courseID, string studentID)
        {
            _AttendID = attendID;
            _StudentID = studentID;
            _CourseID = _OldCourseID = courseID;
        }
    }
    class DataGridViewColorBallTextCell : DataGridViewTextBoxCell
    {
        private Color _Color = Color.Transparent;
        public Color Color
        {
            get { return _Color; }
            set { _Color = value; }
        }
        protected override void Paint(System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (_Color != Color.Transparent)
            {
                SmoothingMode mode = graphics.SmoothingMode;//跟妳說喔，NotNetBar很機車喔，如果你把SmoothingMode改掉沒改回去，格線會亂亂劃喔。
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, null, null, errorText, cellStyle, advancedBorderStyle, paintParts);

                int w = cellBounds.Height - 9,
                    x = cellBounds.X + 3,
                    y = cellBounds.Y + 4;
                Color[] myColors = { _Color, Color.White, _Color, _Color };
                float[] myPositions = { 0.0f, 0.05f, 0.6f, 1.0f };
                ColorBlend myBlend = new ColorBlend();
                myBlend.Colors = myColors;
                myBlend.Positions = myPositions;
                using (LinearGradientBrush brush = new LinearGradientBrush(new Point(x, y), new Point(x + w, y + w), Color.White, _Color))
                {
                    brush.InterpolationColors = myBlend;
                    brush.GammaCorrection = true;
                    graphics.FillRectangle(brush, x, y, w, w);
                }
                graphics.DrawRectangle(new Pen(Color.Black), x, y, w, w);

                cellBounds = new System.Drawing.Rectangle(cellBounds.X + cellBounds.Height - 4, cellBounds.Y, cellBounds.Width - cellBounds.Height + 4, cellBounds.Height);
                graphics.SmoothingMode = mode;
            }
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }

        public override Type EditType
        {
            get
            {
                return null;
            }
        }
    }
    class DataGridViewColorBallTextColumn : DataGridViewColumn
    {
        public DataGridViewColorBallTextColumn()
        {
            this.CellTemplate = new DataGridViewColorBallTextCell();
        }
    }
}