using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using KaoHsiung.StudentRecordReport.Forms;
using JHSchool.Data;

namespace KaoHsiung.StudentRecordReport
{
    public partial class MainForm : BaseForm
    {
        private EnterType _EnterType;

        public MainForm(EnterType enterType)
        {
            InitializeComponent();
            this.Text = Global.ReportName;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            _EnterType = enterType;

            // 讀取畫面上承辦人員與註冊組長預設值
            K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration["學籍表_相關資料"];
            // 承辦人員
            txtTransName.Text = cd["TransName"];
            // 註冊組長
            txtRegName.Text = cd["RegName"];
        }

        internal static void Run(EnterType enterType)
        {
            new MainForm(enterType).ShowDialog();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Utility.MorItemDict = Utility.GetDLBehaviorConfigNameDict();
            // 回寫畫面預設值與使用全域值
            Global.TransferName = txtTransName.Text;
            Global.RegManagerName = txtRegName.Text;
            K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration["學籍表_相關資料"];
            cd["TransName"] = txtTransName.Text;
            cd["RegName"] = txtRegName.Text;
            cd.Save();


            Report report = new Report();
            List<JHStudentRecord> list = new List<JHStudentRecord>();
            List<JHStudentRecord> studList = new List<JHStudentRecord>();
            List<JHStudentRecord> studListN = new List<JHStudentRecord>();
            if (_EnterType == EnterType.Student)
                list=JHSchool.Data.JHStudent.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource);
            else
            {
                foreach (JHClassRecord cla in JHClass.SelectByIDs(K12.Presentation.NLDPanels.Class.SelectedSource))
                {
                    studList.Clear();
                    studListN.Clear();
                    // 取得一般生
                    foreach (JHStudentRecord studRec in cla.Students.Where(x => x.Status == K12.Data.StudentRecord.StudentStatus.一般))
                    { 
                        // 如果有座號依座號排序
                        if (studRec.SeatNo.HasValue)
                            studList.Add(studRec);
                        else  // 沒有座號依學號
                            studListN.Add(studRec);
                    }
                    // sort
                    if(studList.Count >0)
                        list.AddRange(studList.OrderBy(x => x.SeatNo.Value));
                    if(studListN.Count >0)
                        list.AddRange(studListN.OrderBy(x=>x.StudentNumber));                    
                }
            }

            list.Sort(SortStudentByClassSeatNo);
            report.SetStudents(list);
            report.GenerateReport();

            this.DialogResult = DialogResult.OK;
        }

        private int SortStudentByClassSeatNo(JHStudentRecord x, JHStudentRecord y)
        {
            JHClassRecord c1 = x.Class;
            JHClassRecord c2 = y.Class;
            if (c1.ID == c2.ID)
            {
                int seatNo1 = x.SeatNo.HasValue ? x.SeatNo.Value : int.MinValue;
                int seatNo2 = y.SeatNo.HasValue ? y.SeatNo.Value : int.MinValue;

                if (seatNo1 == seatNo2)
                    return x.StudentNumber.CompareTo(y.StudentNumber);
                else
                    return seatNo1.CompareTo(seatNo2);
            }
            else
            {
                if (c1 == null)
                    return -1;
                else if (c2 == null)
                    return 1;
                return c1.Name.CompareTo(c2.Name);
            }
        }

        private void lnPrintConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PrintConfigForm form = new PrintConfigForm();
            form.ShowDialog();
        }

        private void lnType_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SelectTypeForm form = new SelectTypeForm(Global.ReportName);
            form.ShowDialog();
        }
    }
    /// <summary>
    /// 判斷學生或班級
    /// </summary>
    public enum EnterType { Student,Class}
}
