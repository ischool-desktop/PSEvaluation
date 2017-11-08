using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using HsinChu.StudentRecordReport.ConfigForms;
using JHSchool.Data;

namespace HsinChu.StudentRecordReport
{
    public partial class MainForm : BaseForm
    {
        private Options Options { get; set; }
        private EnterType _EnterType;

        internal static void Run(EnterType enterType)
        {
            new MainForm(enterType).ShowDialog();
        }

        public MainForm(EnterType enterType)
        {
            InitializeComponent();
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.Text = Global.ReportName;
            _EnterType = enterType;
            Options = new Options();

            // 學生
            if (_EnterType == EnterType.Student)
                Options.Students = JHSchool.Data.JHStudent.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource);
            else
            {
                // 班級
                List<JHStudentRecord> list = new List<JHStudentRecord>();
                List<JHStudentRecord> studList = new List<JHStudentRecord>();
                List<JHStudentRecord> studListN = new List<JHStudentRecord>();
                foreach (JHClassRecord cla in JHClass.SelectByIDs(K12.Presentation.NLDPanels.Class.SelectedSource))
                {
                    studList.Clear();
                    studListN.Clear();
                    // 過濾一般生
                    foreach (JHStudentRecord studRec in cla.Students.Where(x => x.Status == K12.Data.StudentRecord.StudentStatus.一般))
                    {
                        // 將是否有座號分開
                        if (studRec.SeatNo.HasValue)
                            studList.Add(studRec);
                        else
                            studListN.Add(studRec);
                    }
                    // 有座號依座號排序
                    if(studList.Count >0)
                        list.AddRange(studList.OrderBy(x=>x.SeatNo.Value));
                    // 沒有座號依學號排序
                    if(studListN.Count>0)
                        list.AddRange(studListN.OrderBy(x => x.StudentNumber));
                }
                Options.Students = list;            
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Report report = new Report(Options);
            report.Generate();

            this.DialogResult = DialogResult.OK;
        }

        private void lnTypeConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SelectTypeForm form = new SelectTypeForm(Global.ReportName);
            form.ShowDialog();
        }

        private void lnPrintConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PrintConfigForm form = new PrintConfigForm();
            form.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 取得日常生活表現名稱
            Global.DLBehaviorConfigNameDict = Global.GetDLBehaviorConfigNameDict();
        }       
    }
    /// <summary>
    /// 判斷學生或班級
    /// </summary>
    public enum EnterType
    { Student, Class }
}
