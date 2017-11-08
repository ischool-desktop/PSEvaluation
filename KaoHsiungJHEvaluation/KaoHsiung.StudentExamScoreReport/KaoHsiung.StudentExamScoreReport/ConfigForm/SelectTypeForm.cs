using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using K12.Data.Configuration;

namespace KaoHsiung.StudentExamScoreReport.ConfigForm
{
    public partial class SelectTypeForm : BaseForm
    {
        private string _preferenceElementName;
        private BackgroundWorker _BGWAbsenceAndPeriodList;

        private List<string> typeList = new List<string>();
        private List<string> absenceList = new List<string>();

        public bool CheckColumnCount { get; set; }

        public SelectTypeForm(string name)
        {
            InitializeComponent();

            _preferenceElementName = name;
            CheckColumnCount = false;

            _BGWAbsenceAndPeriodList = new BackgroundWorker();
            _BGWAbsenceAndPeriodList.DoWork += new DoWorkEventHandler(_BGWAbsenceAndPeriodList_DoWork);
            _BGWAbsenceAndPeriodList.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWAbsenceAndPeriodList_RunWorkerCompleted);
            _BGWAbsenceAndPeriodList.RunWorkerAsync();
        }

        private void _BGWAbsenceAndPeriodList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox.Visible = false;

            DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
            colName.HeaderText = "�`������";
            colName.MinimumWidth = 70;
            colName.Name = "colName";
            colName.ReadOnly = true;
            colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colName.Width = 70;
            this.dgv.Columns.Add(colName);

            foreach (string absence in absenceList)
            {
                System.Windows.Forms.DataGridViewCheckBoxColumn newCol = new DataGridViewCheckBoxColumn();
                newCol.HeaderText = absence;
                newCol.Width = 55;
                newCol.ReadOnly = false;
                newCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
                newCol.Tag = absence;
                newCol.ValueType = typeof(bool);
                this.dgv.Columns.Add(newCol);
            }
            foreach (string type in typeList)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, type);
                row.Tag = type;
                dgv.Rows.Add(row);
            }

            #region Ū���C�L�]�w Preference
            ConfigData cd = K12.Data.School.Configuration[_preferenceElementName];
            if (cd.Contains("���O�]�w"))
            {
                XmlElement config = Framework.XmlHelper.LoadXml(cd["���O�]�w"]);
                #region �w���]�w�ɫh�N�]�w�ɤ��e��^�e���W
                foreach (XmlElement type in config.SelectNodes("Type"))
                {
                    string typeName = type.GetAttribute("Text");
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (typeName == ("" + row.Tag))
                        {
                            foreach (XmlElement absence in type.SelectNodes("Absence"))
                            {
                                string absenceName = absence.GetAttribute("Text");
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    if (cell.OwningColumn is DataGridViewCheckBoxColumn && ("" + cell.OwningColumn.Tag) == absenceName)
                                    {
                                        cell.Value = true;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
                #endregion
            }
            #endregion
        }

        private void _BGWAbsenceAndPeriodList_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (JHPeriodMappingInfo info in JHPeriodMapping.SelectAll())
            {
                if (!typeList.Contains(info.Type))
                    typeList.Add(info.Type);
            }

            foreach (JHAbsenceMappingInfo info in JHAbsenceMapping.SelectAll())
            {
                if (!absenceList.Contains(info.Name))
                    absenceList.Add(info.Name);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (CheckColumnCount)
            {
                if (!CheckColumnNumber())
                    return;
            }

            #region ��s�C�L�]�w Preference
            ConfigData cd = K12.Data.School.Configuration[_preferenceElementName];

            XmlElement config = new XmlDocument().CreateElement("���O�]�w");

            foreach (DataGridViewRow row in dgv.Rows)
            {
                bool needToAppend = false;
                XmlElement type = config.OwnerDocument.CreateElement("Type");
                type.SetAttribute("Text", "" + row.Tag);
                foreach (DataGridViewCell cell in row.Cells)
                {
                    XmlElement absence = config.OwnerDocument.CreateElement("Absence");
                    absence.SetAttribute("Text", "" + cell.OwningColumn.Tag);
                    if (cell.Value is bool && ((bool)cell.Value))
                    {
                        needToAppend = true;
                        type.AppendChild(absence);
                    }
                }
                if (needToAppend)
                    config.AppendChild(type);
            }

            cd["���O�]�w"] = config.OuterXml;
            cd.Save();
            #endregion

            this.DialogResult = DialogResult.OK;
        }

        private bool CheckColumnNumber()
        {
            int limit = 253;
            int columnNumber = 0;
            int block = 9;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value is bool && ((bool)cell.Value))
                        columnNumber++;
                }
            }

            if (columnNumber * block > limit)
            {
                MsgBox.Show("�z�ҿ�ܪ����O�W�X Excel ���̤j���A�д�ֳ������O");
                return false;
            }
            else
                return true;
        }
    }
}