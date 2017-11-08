using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using JHSchool.Data;
using FISCA.Presentation.Controls;

namespace JHEvaluation.AssignmentScore
{
    public partial class MainFormScoreInput : FISCA.Presentation.Controls.BaseForm 
    {
        // 努力程度對照
        private EffortMap _EffortMap;
        // 課程成績
        private Dictionary<string,JHSCAttendRecord> _SCAttendListDic;

        // 比對 log 用
        private Dictionary<string, string> _StudOldScoreDic;
        private Dictionary<string, string> _StudOldEffortDic;


        // 評量設定
        private JHAssessmentSetupRecord _AssessmentSetupRec;

        // 修課學生成績
        private List<StudentSCAtendScore> _StudentSCAtendScoreList;

        List<string> CIDs;
        // 課程紀錄
        private JHCourseRecord _CourseRec;

        public MainFormScoreInput(JHCourseRecord courseRec)
        {
            InitializeComponent();

            _EffortMap = new EffortMap();            
            _CourseRec = courseRec;
            CIDs = new List<string>();
            _StudOldScoreDic = new Dictionary<string, string>();
            _StudOldEffortDic = new Dictionary<string, string>();
            CIDs.Add(courseRec.ID);
            _SCAttendListDic = new Dictionary<string, JHSCAttendRecord>();
            lblCourseName.Text = courseRec.Name;
            _StudentSCAtendScoreList = new List<StudentSCAtendScore>();

            this.MinimumSize = this.MaximumSize = this.Size;

            // 取得課程評量
            _AssessmentSetupRec = JHAssessmentSetup.SelectByID(courseRec.RefAssessmentSetupID);
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 複製SCAttendListDic 給 log 用
        private void CopySCAttendListDic()
        {
            _StudOldScoreDic.Clear();
            _StudOldEffortDic.Clear();

            foreach (KeyValuePair<string, JHSCAttendRecord> data in _SCAttendListDic)
            {
                if (data.Value.OrdinarilyScore.HasValue)
                    _StudOldScoreDic.Add(data.Key, data.Value.OrdinarilyScore.Value.ToString());
                else
                    _StudOldScoreDic.Add(data.Key, string.Empty);

                if (data.Value.OrdinarilyEffort.HasValue)
                    _StudOldEffortDic.Add(data.Key, data.Value.OrdinarilyEffort.Value.ToString());
                else
                    _StudOldEffortDic.Add(data.Key, string.Empty);
            }
        }


        /// <summary>
        /// 當輸入欄位結束編輯時驗證
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != colInputScore.Index && e.ColumnIndex != colInputEffort.Index) return;
            if (e.RowIndex < 0) return;

            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (cell.OwningColumn == colInputScore)
            {
                #region 驗證分數評量 & 低於60分變紅色
                cell.Style.ForeColor = Color.Black;

                if (!string.IsNullOrEmpty("" + cell.Value))
                {
                    decimal d;
                    if (!decimal.TryParse("" + cell.Value, out d))
                        cell.ErrorText = "分數必須是數字";
                    else
                    {
                        cell.ErrorText = "";
                        if (d < 60)
                            cell.Style.ForeColor = Color.Red;
                        if (d > 100 || d < 0)
                            cell.Style.ForeColor = Color.Green;

                        // 取得努力程度代碼
                        dgv.Rows[cell.RowIndex].Cells[colInputEffort.Index].Value = _EffortMap.GetCodeByScore(d);
                    }
                }
                else
                    cell.ErrorText = "";
                #endregion
            }
            else if (cell.OwningColumn == colInputEffort)
            {
                #region 驗證努力程度
                if (!string.IsNullOrEmpty("" + cell.Value))
                {
                    int i;
                    if (!int.TryParse("" + cell.Value, out i))
                        cell.ErrorText = "努力程度必須是整數數字";
                }
                else
                    cell.ErrorText = "";
                #endregion
            }

            if (dgv.Rows.Count > 0)
                lblSave.Visible = true;
            else
                lblCourseName.Visible = false;
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != colInputScore.Index && e.ColumnIndex != colInputEffort.Index) return;
            if (e.RowIndex < 0) return;

            dgv.BeginEdit(true);
        }

        // 畫面載入時
        private void MainFormScoreInput_Load(object sender, EventArgs e)
        {
            
            
            // 載入成績
            LoadSCAttendScoreToDGV();

            // 複製一份分數給log 用
            CopySCAttendListDic();

            // 載入分數顏色
            LoadDvScoreColor();


        }

        // 座號排序
        private static int SortStudSeatNo(StudentSCAtendScore x, StudentSCAtendScore y)
        {
            /* 小郭, 2013/12/27, 排序方法改成跟"成績輸入"的一樣
            int intX, intY;
            if (x.SeatNo == null)
                intX = 999;
            else
                intX = x.SeatNo;
            if (y.SeatNo == null)
                intY = 999;
            else
                intY = y.SeatNo;

            return intX.CompareTo(intY);
            */

            if (x.ClassId != null && y.ClassId != null)
            {
                if (x.ClassId == y.ClassId)
                {
                    if (x.SeatNo != null && y.SeatNo != null)
                    {
                        if (x.SeatNo == y.SeatNo)
                            return x.StudentNumber.CompareTo(y.StudentNumber);
                        return x.SeatNo.CompareTo(y.SeatNo);
                    }
                    else if (x.SeatNo != null)
                        return -1;
                    else if (y.SeatNo != null)
                        return 1;
                    else
                        return x.StudentNumber.CompareTo(y.StudentNumber);
                }
                else
                    return x.ClassName.CompareTo(y.ClassName);
            }
            else if (x.ClassId != null && y.ClassId == null)
                return -1;
            else if (x.ClassId == null && y.ClassId != null)
                return 1;
            else
                return x.StudentNumber.CompareTo(y.StudentNumber);
        }

        // 載入修課學生與成績到畫面
        private void LoadSCAttendScoreToDGV()
        {
          
            
            // 取得修課成績
            foreach (JHSCAttendRecord rec in JHSCAttend.SelectByCourseIDs(CIDs))
            {
                if (!_SCAttendListDic.ContainsKey(rec.RefStudentID))
                    _SCAttendListDic.Add(rec.RefStudentID, rec);
            }

            // 組成學生課程成績
            foreach (JHStudentRecord studRec in JHStudent.SelectByIDs(_SCAttendListDic.Keys))
            {
                // 過濾非一般生
                if (studRec.Status != K12.Data.StudentRecord.StudentStatus.一般)
                    continue;

                StudentSCAtendScore sscas = new StudentSCAtendScore();
                sscas.StudentID = studRec.ID;
                if (studRec.Class != null)
                {
                    sscas.ClassName = studRec.Class.Name;
                    // 小郭, 2012/12/27
                    sscas.ClassId = studRec.Class.ID;
                }
                if (studRec.SeatNo.HasValue)
                    sscas.SeatNo = studRec.SeatNo.Value;
                sscas.Name = studRec.Name;
                sscas.StudentNumber =studRec.StudentNumber ;
                if (_SCAttendListDic.ContainsKey(studRec.ID))
                    sscas.SCAtendRec = _SCAttendListDic[studRec.ID];
                _StudentSCAtendScoreList.Add(sscas);
            }

            // 排序
            _StudentSCAtendScoreList.Sort(new Comparison<StudentSCAtendScore>(SortStudSeatNo));

            // 載入值
            dgv.SuspendLayout();
            dgv.Rows.Clear();

            int dgRowIdx = 0;
            foreach (StudentSCAtendScore sscas in _StudentSCAtendScoreList)
            {
                dgv.Rows.Add();
                dgv.Rows[dgRowIdx].Cells[colClassName.Index].Value = sscas.ClassName;
                dgv.Rows[dgRowIdx].Cells[colSeatNo.Index].Value = sscas.SeatNo;
                dgv.Rows[dgRowIdx].Cells[colName.Index].Value = sscas.Name;
                dgv.Rows[dgRowIdx].Cells[colStudentNumber.Index].Value = sscas.StudentNumber;
                dgv.Rows[dgRowIdx].Cells[colInputScore.Index].Value = sscas.GetOrdinarilyScore();
                dgv.Rows[dgRowIdx].Cells[colInputEffort.Index].Value = sscas.GetOrdinarilyEffort();
                dgv.Rows[dgRowIdx].Tag = sscas.StudentID;
                dgRowIdx++;
            }
            dgv.ResumeLayout();
        
        }


        /// <summary>
        /// 載入分數顏色
        /// </summary>
        private void LoadDvScoreColor()
        {
            // 處理初始分數變色
            foreach (DataGridViewRow dgv1 in dgv.Rows)
                foreach (DataGridViewCell cell in dgv1.Cells)
                {
                    cell.ErrorText = "";

                    if (cell.OwningColumn == colInputScore)
                    {
                        cell.Style.ForeColor = Color.Black;
                        if (!string.IsNullOrEmpty("" + cell.Value))
                        {
                            decimal d;
                            if (!decimal.TryParse("" + cell.Value, out d))
                                cell.ErrorText = "分數必須為數字";
                            else
                            {
                                if (d < 60)
                                    cell.Style.ForeColor = Color.Red;
                                if (d > 100 || d < 0)
                                    cell.Style.ForeColor = Color.Green;
                            }
                        }
                    }
                    else if (cell.OwningColumn == colInputEffort)
                    {
                        if (!string.IsNullOrEmpty("" + cell.Value))
                        {
                            int i;
                            if (!int.TryParse("" + cell.Value, out i))
                                cell.ErrorText = "努力程度必須為整數";
                        }
                    }
                }
        }

        /// <summary>
        /// 回寫成績到 StudentSCAtendScore
        /// </summary>
        private void WriteDGVToRec()
        {
            foreach (DataGridViewRow dgvr in dgv.Rows)
            {
                if (dgvr.IsNewRow)
                    continue;

                string strID = (string)dgvr.Tag;
                if (_SCAttendListDic.ContainsKey(strID))
                { 
                    decimal score;
                    int Eff;

                    if(dgvr.Cells[colInputScore.Index].Value==null )
                        _SCAttendListDic[strID].OrdinarilyScore = null;
                    else
                    {
                    if (decimal.TryParse(dgvr.Cells[colInputScore.Index].Value.ToString(), out score))
                        _SCAttendListDic[strID].OrdinarilyScore = score;
                    }

                    if (dgvr.Cells[colInputEffort.Index].Value == null)
                        _SCAttendListDic[strID].OrdinarilyEffort = null;
                    else
                    {
                    if (int.TryParse(dgvr.Cells[colInputEffort.Index].Value.ToString(), out Eff))                    
                        _SCAttendListDic[strID].OrdinarilyEffort = Eff;
                    }
                
                }            
            }        
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            dgv.EndEdit();

            try
            {

                //回寫資料到 record
                WriteDGVToRec();

                List<JHSCAttendRecord> UpdateSCAtendList = new List<JHSCAttendRecord>();
                UpdateSCAtendList.AddRange(_SCAttendListDic.Values);

                if (UpdateSCAtendList.Count < 1)
                    return;

                bool checkLoadSave = false;
                // 檢查超過 0~100

                foreach (JHSCAttendRecord rec in UpdateSCAtendList)
                {
                    if (rec.OrdinarilyScore.HasValue)
                        if (rec.OrdinarilyScore.Value < 0 || rec.OrdinarilyScore.Value > 100)
                            checkLoadSave = true;
                }

                if (checkLoadSave)
                {
                    CheckSaveForm csf = new CheckSaveForm();
                    csf.ShowDialog();
                    if (csf.DialogResult == DialogResult.Cancel)
                        return;
                }
                
                StringBuilder sb = new StringBuilder();
                
                // 記 Log;
                foreach (StudentSCAtendScore rec in _StudentSCAtendScoreList)
                {
                    string str = "";

                    if(_StudOldScoreDic.ContainsKey(rec.StudentID ))
                        if (_StudOldScoreDic[rec.StudentID] != rec.GetOrdinarilyScore())
                            str += "平時評量由「" + _StudOldScoreDic[rec.StudentID] + "」改成「" + rec.GetOrdinarilyScore() + "」";
                    if(_StudOldEffortDic.ContainsKey(rec.StudentID))
                        if (_StudOldEffortDic[rec.StudentID] != rec.GetOrdinarilyEffort ())
                            str += "努力程度由「" + _StudOldEffortDic[rec.StudentID] + "」改成「" + rec.GetOrdinarilyEffort() + "」";

                        if(str!="")
                            sb.Append("班級："+rec.ClassName +",座號："+rec.SeatNo+",姓名："+rec.Name +" "+str);                   
                }               

                JHSCAttend.Update(UpdateSCAtendList);
                lblSave.Visible = false;
                
                FISCA.LogAgent.ApplicationLog.Log("課程平時評量輸入", "","course", _CourseRec.ID, sb.ToString());               

                CopySCAttendListDic();
                MsgBox.Show("儲存成功。");

            }
            catch (Exception ex)
            {
                MsgBox.Show("儲存失敗。\n" + ex.Message);
            }

            // 載入分數顏色
            LoadDvScoreColor();
        }

        private void MainFormScoreInput_Shown(object sender, EventArgs e)
        {
            // 檢查平時評量輸入時間點
            if (_AssessmentSetupRec != null)
                if (_AssessmentSetupRec.OrdinarilyStartTime.HasValue && _AssessmentSetupRec.OrdinarilyEndTime.HasValue)
                    if (DateTime.Now >= _AssessmentSetupRec.OrdinarilyStartTime.Value && DateTime.Now <= _AssessmentSetupRec.OrdinarilyEndTime)
                        FISCA.Presentation.Controls.MsgBox.Show("在開放輸入平時評量時間範圍內。");

        }
    }
}
