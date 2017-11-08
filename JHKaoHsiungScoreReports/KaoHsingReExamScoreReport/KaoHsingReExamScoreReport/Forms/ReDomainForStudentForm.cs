using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using KaoHsingReExamScoreReport.DAO;
using Aspose.Cells;
using System.IO;
using Aspose.Words;
using KaoHsingReExamScoreReport.DAO;
using JHSchool.Data;

namespace KaoHsingReExamScoreReport.Forms
{
    public partial class ReDomainForStudentForm : BaseForm
    {
        BackgroundWorker _bgWorker;
        int _SchoolYear = 0;
        int _Semester = 0;
        List<StudentData> _StudentDataList;
        List<string> _SelectStudentIDList;
        decimal _passScore = 60;
        Configure _Configure;

        public ReDomainForStudentForm(List<string> StudentIDList)
        {
            InitializeComponent();
            _bgWorker = new BackgroundWorker();
            _StudentDataList = new List<DAO.StudentData>();
            _SelectStudentIDList = StudentIDList;
            _bgWorker.DoWork += _bgWorker_DoWork;
            _bgWorker.RunWorkerCompleted += _bgWorker_RunWorkerCompleted;
            _bgWorker.WorkerReportsProgress = true;
            _bgWorker.ProgressChanged += _bgWorker_ProgressChanged;
        }

        void _bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("領域補考通知單產生中 ...", e.ProgressPercentage);
        }

        void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnPrint.Enabled = true;

            Document doc = (Document)e.Result;

            if (doc != null)
            {
                string reportNameW = "高雄領域補考通知單";
                string pathW = Path.Combine(System.Windows.Forms.Application.StartupPath + "\\Reports", "");
                if (!Directory.Exists(pathW))
                    Directory.CreateDirectory(pathW);
                pathW = Path.Combine(pathW, reportNameW + ".doc");

                if (File.Exists(pathW))
                {
                    int i = 1;
                    while (true)
                    {
                        string newPathW = Path.GetDirectoryName(pathW) + "\\" + Path.GetFileNameWithoutExtension(pathW) + (i++) + Path.GetExtension(pathW);
                        if (!File.Exists(newPathW))
                        {
                            pathW = newPathW;
                            break;
                        }
                    }
                }

                try
                {
                    doc.Save(pathW, Aspose.Words.SaveFormat.Doc);
                    System.Diagnostics.Process.Start(pathW);

                }
                catch (Exception exow)
                {

                }
                doc = null;

                GC.Collect();
            }

            FISCA.Presentation.MotherForm.SetStatusBarMessage("領域補考通知單產生完成.");
        }

        void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _bgWorker.ReportProgress(1);
            // 取得所選學生資料
            _StudentDataList = QueryData.GetStudentDataListByStudentIDs(_SelectStudentIDList);

            // 取得領域成績並判斷
            _StudentDataList = Utility.CalcStudDomainScorePass(_StudentDataList, _SchoolYear, _Semester, _passScore);

            _bgWorker.ReportProgress(30);


            // 取得收件人資料，取聯絡、戶籍　
            // 取得父母監護人資訊
            Dictionary<string, JHParentRecord> ParentRecordDict = new Dictionary<string, JHParentRecord>();
            foreach (JHParentRecord rec in JHParent.SelectByStudentIDs(_SelectStudentIDList))
            {
                if (!ParentRecordDict.ContainsKey(rec.RefStudentID))
                    ParentRecordDict.Add(rec.RefStudentID, rec);
            }

            // 取得地址相關資訊
            Dictionary<string, JHAddressRecord> AddressRecordDict = new Dictionary<string, JHAddressRecord>();
            foreach (JHAddressRecord rec in JHAddress.SelectByStudentIDs(_SelectStudentIDList))
            {
                if (!AddressRecordDict.ContainsKey(rec.RefStudentID))
                    AddressRecordDict.Add(rec.RefStudentID, rec);
            }


            Document DocTemp = null;
            // 讀取範本,如果沒有使用預設樣版
            if (_Configure.Template == null)
            {
                _Configure.Template = new Document(new MemoryStream(Properties.Resources.領域補考通知單範本));
                _Configure.Encode();
                _Configure.Save();
            }
                
                DocTemp = _Configure.Template.Clone();

            List<string> ColumnList = new List<string>();
            ColumnList.Add("學年度");
            ColumnList.Add("學期");
            ColumnList.Add("班級");
            ColumnList.Add("座號");
            ColumnList.Add("姓名");
            ColumnList.Add("學號");
            ColumnList.Add("學校名稱");
            ColumnList.Add("學校電話");
            ColumnList.Add("學校地址");
            ColumnList.Add("收件人地址");
            ColumnList.Add("收件人姓名");
            ColumnList.Add("補考數");
            ColumnList.Add("監護人姓名");
            ColumnList.Add("父親姓名");
            ColumnList.Add("母親姓名");
            ColumnList.Add("戶籍地址");
            ColumnList.Add("聯絡地址");
            ColumnList.Add("其他地址");



            for (int i = 1; i <= 10; i++)
            {
                ColumnList.Add("領域名稱" + i);
                ColumnList.Add("原始成績" + i);                
            }

            
            List<Document> docList = new List<Document>();
            DataTable dt = new DataTable();

            // 填入欄位
            foreach (string name in ColumnList)
                dt.Columns.Add(name);

            _bgWorker.ReportProgress(70);

            foreach (StudentData sd in _StudentDataList)
            {

                // 填入資料
                DataRow row = dt.NewRow();
                row["學年度"] = _SchoolYear;
                row["學期"] = _Semester;
                row["班級"] = sd.ClassName;
                row["座號"] = sd.SeatNo;
                row["姓名"] = sd.Name;
                row["學號"] = sd.StudentNumber;
                row["學校名稱"] = K12.Data.School.ChineseName;
                row["學校電話"] = K12.Data.School.Telephone;
                row["學校地址"] = K12.Data.School.Address;

                int ReDomainCount = sd.GetReDomainCount();
                row["補考數"] = ReDomainCount;

                int rDomCOunt = 1;
                if(sd.StudSemesterScoreRecord !=null)
                foreach (K12.Data.DomainScore ds in sd.StudSemesterScoreRecord.Domains.Values)
                {
                    if (ds.Score.HasValue)
                    {
                        if (ds.Domain != "國語文" && ds.Domain != "英語") // 2017/1/9 穎驊 註解， 高雄不再分別使用 國語文 、英語 領域，而是兩領域合一算一"語文"領域成績 來看有沒有及格。 
                        {
                        if (ds.Score.Value < _passScore)
                        {
                            row["領域名稱" + rDomCOunt] = ds.Domain;
                            row["原始成績" + rDomCOunt] = "";

                            if (ds.ScoreOrigin.HasValue)
                                row["原始成績" + rDomCOunt] = ds.ScoreOrigin.Value;

                            rDomCOunt++;
                        }                                                
                        }
                        
                    }
                }

                if (ParentRecordDict.ContainsKey(sd.StudentID))
                {
                    row["監護人姓名"] = ParentRecordDict[sd.StudentID].CustodianName;
                    row["父親姓名"] = ParentRecordDict[sd.StudentID].FatherName;
                    row["母親姓名"] = ParentRecordDict[sd.StudentID].MotherName;

                    if (!string.IsNullOrEmpty(ParentRecordDict[sd.StudentID].CustodianName))
                        row["收件人姓名"] = ParentRecordDict[sd.StudentID].CustodianName;
                    else if (!string.IsNullOrEmpty(ParentRecordDict[sd.StudentID].FatherName))
                        row["收件人姓名"] = ParentRecordDict[sd.StudentID].FatherName;
                    else
                        row["收件人姓名"] = ParentRecordDict[sd.StudentID].MotherName;

                }
                if (AddressRecordDict.ContainsKey(sd.StudentID))
                {
                    row["戶籍地址"] = AddressRecordDict[sd.StudentID].PermanentAddress;
                    row["聯絡地址"] = AddressRecordDict[sd.StudentID].MailingAddress;
                    row["其他地址"] = AddressRecordDict[sd.StudentID].Address1Address;

                    if (!string.IsNullOrEmpty(AddressRecordDict[sd.StudentID].MailingAddress))
                        row["收件人地址"] = AddressRecordDict[sd.StudentID].MailingAddress;
                    else if (!string.IsNullOrEmpty(AddressRecordDict[sd.StudentID].PermanentAddress))
                        row["收件人地址"] = AddressRecordDict[sd.StudentID].PermanentAddress;
                    else
                        row["收件人地址"] = AddressRecordDict[sd.StudentID].Address1Address;
                }

                // 需要補考再出現
                if(ReDomainCount>0)
                    dt.Rows.Add(row);
            }
                //#region debug
                //dt.TableName = "debug";
                //dt.WriteXml(Application.StartupPath+"\\dttable.xml");
                //StreamWriter sw = new StreamWriter(Application.StartupPath + "\\tempField.txt", false);
                //foreach (string str in DocTemp.MailMerge.GetFieldNames())
                //    sw.WriteLine(str);
                //sw.Flush();
                //sw.Close();

                //#endregion


                if (DocTemp != null)
                {
                    // 處理固定欄位對應
                    DocTemp.MailMerge.Execute(dt);
                    DocTemp.MailMerge.RemoveEmptyParagraphs = true;
                    DocTemp.MailMerge.DeleteFields();                    
                }
            
            _bgWorker.ReportProgress(90);
            e.Result = DocTemp;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            _SchoolYear = iptSchoolYear.Value;
            _Semester = iptSemester.Value;
            _bgWorker.RunWorkerAsync();
        }

        private void ReDomainForStudentForm_Load(object sender, EventArgs e)
        {
            this.MinimumSize = this.MaximumSize = this.Size;

            // 預設學年度、學期
            iptSchoolYear.Value = int.Parse(K12.Data.School.DefaultSchoolYear);
            iptSemester.Value = int.Parse(K12.Data.School.DefaultSemester);

            // 讀取範本
            _Configure = GetUDTConfig();
            _Configure.Decode();
        }

        private void lnkDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkDownload.Enabled = false;

            #region 下載範本
            // 當沒有設定檔
            if (_Configure == null) return;

            string reportName = "高雄領域補考通知單範本";

            string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".doc");

            if (File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            try
            {
                // 檢查目前範本是否 Null，當Null使用預設
                if (_Configure.Template == null)
                    _Configure.Template = new Document(new MemoryStream(Properties.Resources.領域補考通知單範本));
                
                _Configure.Template.Save(path, Aspose.Words.SaveFormat.Doc);                
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".doc";
                sd.Filter = "Word檔案 (*.doc)|*.doc|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        _Configure.Template.Save(sd.FileName, Aspose.Words.SaveFormat.Doc);
                    }
                    catch
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            #endregion
            lnkDownload.Enabled = true;
        }

        private void lnkUpload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkUpload.Enabled = false;

            #region 上傳範本
            if (_Configure == null) return;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "上傳範本";
            dialog.Filter = "Word檔案 (*.doc)|*.doc|所有檔案 (*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    _Configure.Template = new Aspose.Words.Document(dialog.FileName);

                    // 檢查範本是否有合併欄位
                    int FieldsCount = _Configure.Template.MailMerge.GetFieldNames().Count();

                    if (FieldsCount > 0)
                    {
                        _Configure.Encode();
                        _Configure.Save();
                        MsgBox.Show("上傳範本完成.");
                    }
                    else
                    {
                        if (MsgBox.Show("上傳範本內沒有合併欄位，當按「是」將更新為預設範本?", "沒有合併欄位", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                        {
                            _Configure.Template = new Document(new MemoryStream(Properties.Resources.領域補考通知單範本));
                            _Configure.Encode();
                            _Configure.Save();
                            MsgBox.Show("已將範本更新為預設範本.");
                        }
                        else
                            MsgBox.Show("上傳範本內沒有合併欄位無法上傳.");
                    
                    }

                }
                catch
                {
                    MsgBox.Show("範本開啟失敗");
                }
            }
            #endregion

            lnkUpload.Enabled = true;

        }

        private void lnkMField_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkMField.Enabled = false;
            #region 合併欄位總表
            string reportName = "高雄領域補考通知單合併欄位總表";

            string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".doc");

            if (File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            try
            {
                Document doc = new Document(new MemoryStream(Properties.Resources.高雄領域補考通知單合併欄位總表));
                doc.Save(path);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".doc";
                sd.Filter = "Word檔案 (*.doc)|*.doc|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        Document doc = new Document(new MemoryStream(Properties.Resources.高雄領域補考通知單合併欄位總表));
                        doc.Save(sd.FileName);
                    }
                    catch
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            #endregion

            lnkMField.Enabled = true;
        }

        /// <summary>
        /// 取得設定值
        /// </summary>
        /// <returns></returns>
        private Configure GetUDTConfig()
        {
            Configure conf;
            FISCA.UDT.AccessHelper accHelper = new FISCA.UDT.AccessHelper();
            List<Configure> confList = accHelper.Select<Configure>();
            if (confList.Count == 1)
                conf = confList[0];
            else
            {
                conf = new Configure();
                conf.Template = new Document(new MemoryStream(Properties.Resources.領域補考通知單範本));
            }
            return conf;
        }

        private void lnDownDefalut_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnDownDefalut.Enabled = false;

            #region 下載預設範本
            Document DefDoc = null;
            // 下載預設範本
            string reportName = "高雄領域補考通知單預設範本";

            string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".doc");

            if (File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            try
            {
                DefDoc = new Document(new MemoryStream(Properties.Resources.領域補考通知單範本));
                DefDoc.Save(path, Aspose.Words.SaveFormat.Doc);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".doc";
                sd.Filter = "Word檔案 (*.doc)|*.doc|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        DefDoc.Save(sd.FileName, Aspose.Words.SaveFormat.Doc);
                    }
                    catch
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            #endregion

            lnDownDefalut.Enabled = true;
        }

    }
}
