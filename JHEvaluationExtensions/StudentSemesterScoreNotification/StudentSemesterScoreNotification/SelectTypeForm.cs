using Campus.Report;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using JHSchool.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace StudentSemesterScoreNotification
{
    public partial class SelectTypeForm : BaseForm
    {
        private BackgroundWorker _BGWAbsenceAndPeriodList;

        private AccessHelper _A;
        private List<AbsentSetting> _setting;
        private List<string> _sourceList;

        public bool CheckColumnCount { get; set; }

        public SelectTypeForm()
        {
            InitializeComponent();

            for (int i = 1; i <= Global.SupportAbsentCount; i++)
                colTarget.Items.Add("列印假別" + i);

            _setting = new List<AbsentSetting>();
            _sourceList = new List<string>();
            _A = new AccessHelper();

            _BGWAbsenceAndPeriodList = new BackgroundWorker();
            _BGWAbsenceAndPeriodList.DoWork += new DoWorkEventHandler(_BGWAbsenceAndPeriodList_DoWork);
            _BGWAbsenceAndPeriodList.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWAbsenceAndPeriodList_RunWorkerCompleted);
            _BGWAbsenceAndPeriodList.RunWorkerAsync();
        }

        private void _BGWAbsenceAndPeriodList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dgv.Rows.Clear();

            foreach (string s in _sourceList)
                colSource.Items.Add(s);

            //設定排序
            _setting.Sort(delegate(AbsentSetting x, AbsentSetting y)
            {
                string xx = x.Target.PadLeft(20, '0');
                xx += x.Source.PadLeft(20, '0');
                string yy = y.Target.PadLeft(20, '0');
                yy += y.Source.PadLeft(20, '0');
                return xx.CompareTo(yy);
            });

            //讀回設定
            foreach (AbsentSetting abs in _setting)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, abs.Target, abs.Source);

                dgv.Rows.Add(row);
            }
        }

        private void _BGWAbsenceAndPeriodList_DoWork(object sender, DoWorkEventArgs e)
        {
            _sourceList.Clear();
            _setting.Clear();

            List<JHPeriodMappingInfo> typeList = JHPeriodMapping.SelectAll();
            List<JHAbsenceMappingInfo> absenceList = JHAbsenceMapping.SelectAll();

            foreach (string p in typeList.Select(x => x.Type).Distinct())
                foreach (string a in absenceList.Select(x => x.Name).Distinct())
                {
                    _sourceList.Add(Global.GetKey(p,a));
                }

            _setting = _A.Select<AbsentSetting>();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            List<string> checkList = new List<string>();
            List<AbsentSetting> list = new List<AbsentSetting>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string target = row.Cells[colTarget.Index].Value + "";
                string source = row.Cells[colSource.Index].Value + "";

                if (!string.IsNullOrWhiteSpace(target) && !string.IsNullOrWhiteSpace(source))
                {
                    string key = Global.GetKey(target, source);

                    //相同的資料不重複加入
                    if (!checkList.Contains(key))
                    {
                        checkList.Add(key);

                        AbsentSetting abs = new AbsentSetting();

                        abs.Target = target;
                        abs.Source = source;

                        list.Add(abs);
                    }
                }
            }

            _A.DeletedValues(_setting);
            _A.SaveAll(list);

            this.Close();
        }
    }
}
