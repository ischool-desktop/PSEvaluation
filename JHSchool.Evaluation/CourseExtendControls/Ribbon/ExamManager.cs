using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DevComponents.DotNetBar;
using FISCA.DSAUtil;
using JHSchool.Evaluation.Feature.Legacy;
using Framework;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon
{
    public partial class ExamManager : FISCA.Presentation.Controls.BaseForm
    {
        private Dictionary<string, ExamInformation> _exam_list;

        public ExamManager()
        {
            InitializeComponent();

            try
            {
                _exam_list = new Dictionary<string, ExamInformation>();
                XmlElement list = QueryExam.GetAbstractList();
                foreach (XmlElement node in list.SelectNodes("Exam"))
                {
                    ExamInformation exam = new ExamInformation(node);

                    int index = dataGridView.Rows.Add();
                    DataGridViewRow row = dataGridView.Rows[index];
                    row.Cells[0].Value = exam.ExamName;
                    row.Cells[1].Value = exam.DisplayOrder;
                    row.Tag = exam;
                    _exam_list.Add(exam.Identity, exam);
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show("載入評量資訊錯誤。");
                btnSave.Enabled = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateAllCell())
            {
                MsgBox.Show("資料有錯誤，請修改後再儲存。", Application.ProductName);
                return;
            }

            List<ExamInformation> newList = new List<ExamInformation>();
            List<string> deleteList = new List<string>();
            Dictionary<string, ExamInformation> updateList = new Dictionary<string, ExamInformation>();

            //處理「新增」、「更新」。
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow) continue; //如果是 NewRow 不處理。

                ExamInformation exam = row.Tag as ExamInformation; //這裡取得的是舊資料。

                if (exam == null)//用 exam 判斷這筆資料是否為新的。
                {
                    ExamInformation newExam = new ExamInformation(row);
                    newList.Add(newExam);
                }
                else
                {
                    ExamInformation updateExam = new ExamInformation(row);
                    updateList.Add(updateExam.Identity, updateExam); //其他的都是更新。
                }
            }

            //處理「刪除」。
            foreach (string each in _exam_list.Keys)
            {
                bool deleteRequired = true;
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Tag == null) continue;

                    ExamInformation deleteExam = row.Tag as ExamInformation;

                    if (deleteExam != null && each == deleteExam.Identity)
                        deleteRequired = false;
                }

                if (deleteRequired)
                    deleteList.Add(each);
            }

            if (deleteList.Count > 0)
            {
                try
                {
                    EditExam.Delete(deleteList);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("刪除試別失敗:" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (updateList.Count > 0)
            {
                try
                {
                    DSXmlHelper helper = new DSXmlHelper("UpdateRequest");
                    foreach (ExamInformation each in updateList.Values)
                    {
                        helper.AddElement("Exam");
                        helper.AddElement("Exam", "ExamName", each.ExamName);
                        helper.AddElement("Exam", "DisplayOrder", each.DisplayOrder);
                        helper.AddElement("Exam", "Condition");
                        helper.AddElement("Exam/Condition", "ID", each.Identity);
                    }

                    EditExam.Update(helper);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("修改試別失敗:" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (newList.Count > 0)
            {
                try
                {
                    DSXmlHelper helper = new DSXmlHelper("InsertRequest");
                    foreach (ExamInformation each in newList)
                    {
                        helper.AddElement("Exam");
                        helper.AddElement("Exam", "ExamName", each.ExamName);
                        helper.AddElement("Exam", "DisplayOrder", each.DisplayOrder);
                    }

                    EditExam.Insert(helper);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("新增試別失敗:" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView.Rows[e.RowIndex].IsNewRow) return;

            ValidateCell(dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex]);
        }

        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridView.Rows[e.RowIndex].IsNewRow) return;

            ValidateCell(dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex]);
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            ExamInformation exam = e.Row.Tag as ExamInformation;

            try
            {
                if (exam != null)
                {
                    int templateCount = QueryExam.GetExamTemplateUseCount(exam.Identity);
                    int textCount = QueryExam.GetTextScoreUseCount(exam.Identity);
                    int numberCount = QueryExam.GetNumberScoreUseCount(exam.Identity);

                    if (templateCount > 0 || textCount > 0 || numberCount > 0)
                    {
                        //string msg = "已有其他資料使用【{3}】，無法刪除。\n\n評量樣版使用數：{0}\n數字評量使用數：{1}\n文字評量使用數：{2}";
                        string msg = "已有其他資料使用【{0}】，無法刪除。";
                        //msg = string.Format(msg, templateCount, numberCount, textCount, exam.ExamName);
                        msg = string.Format(msg,  exam.ExamName);
                        MsgBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                //CurrentUser current = CurrentUser.Instance;
                //BugReporter.ReportException(current.SystemName, current.SystemVersion, ex, false);
                MsgBox.Show(ex.Message, Application.ProductName);
            }
        }

        private bool ValidateAllCell()
        {
            bool validResult = true;
            foreach (DataGridViewRow each in dataGridView.Rows)
            {
                if (each.IsNewRow) continue;

                foreach (DataGridViewCell each2 in each.Cells)
                    validResult &= ValidateCell(each2);
            }

            if (validResult)
            {
                //檢查試別名稱是否重覆。
                List<string> examdump = new List<string>();
                foreach (DataGridViewRow each in dataGridView.Rows)
                {
                    if (each.IsNewRow) continue;

                    string value = each.Cells[0].Value + string.Empty;

                    if (value == string.Empty)
                        continue;

                    if (examdump.Contains(value))
                    {
                        each.Cells[0].ErrorText = "評量名稱不可重覆。";
                        validResult = false;
                        break;
                    }
                    else
                        examdump.Add(value);
                }
            }

            return validResult;
        }

        private bool ValidateCell(DataGridViewCell cell)
        {
            try
            {
                string value = cell.Value + string.Empty;
                cell.ErrorText = "";

                if (cell.ColumnIndex == 0)
                {
                    if (value == string.Empty)
                    {
                        cell.ErrorText = "此欄位必須有資料。";
                        return false;
                    }
                    else
                        return true;
                }
                else
                {
                    if (value == string.Empty)
                        return true;

                    int result;
                    bool validResult;
                    validResult = int.TryParse(value, out result);

                    if (validResult)
                    {
                        if (result >= -32768 && result <= 32767)
                            return true;
                        else
                        {
                            cell.ErrorText = "數字必須在 -32768 ~ 32767 之間。";
                            return false;
                        }
                    }
                    else
                    {
                        cell.ErrorText = "此欄位資料必須是整數。";
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                //CurrentUser user = CurrentUser.Instance;
                //BugReporter.ReportException(user.SystemName, user.SystemVersion, ex, false);

                MsgBox.Show(ex.Message);
                return false;
            }
        }

        private class ExamInformation
        {
            public ExamInformation(XmlElement examData)
            {
                Identity = examData.SelectSingleNode("@ID").InnerText;
                ExamName = examData.SelectSingleNode("ExamName").InnerText;
                DisplayOrder = examData.SelectSingleNode("DisplayOrder").InnerText;
            }

            /// <summary>
            /// 此建構式用於更新時。
            /// </summary>
            public ExamInformation(DataGridViewRow row)
            {
                ExamInformation exam = row.Tag as ExamInformation;

                if (exam != null)
                    Identity = exam.Identity;
                else
                    Identity = "-1";

                ExamName = row.Cells[0].Value + string.Empty;
                DisplayOrder = row.Cells[1].Value + string.Empty;
            }

            public ExamInformation(string examName, string displayOrder)
            {
                Identity = "-1";
                ExamName = examName;
                DisplayOrder = displayOrder;
            }

            public string Identity;

            public string ExamName;

            public string DisplayOrder;
        }
    }
}