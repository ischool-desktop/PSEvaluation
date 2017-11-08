using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JHEvaluation.ScoreCalculation
{

    // 2016/6/6 穎驊製作，作為科目計算，如果有學生在同一學期修兩個相同名稱科目(Subject)，會跳出視窗提醒使用者。
    public partial class DuplicatedSubject : FISCA.Presentation.Controls.BaseForm
    {
        public DuplicatedSubject()
        {
            InitializeComponent();
        }

        public void SetList(List<List<JHSchool.Data.JHSCAttendRecord>> scAttendRec)
        {

            // 下面是直接幫dataGridViewX1填值
            //this.dataGridViewX1.Rows.Add("班級", "座號", "姓名1", "XXXXXXXX");
            //this.dataGridViewX1.Rows.Add("班級", "座號", "姓名2", "XXXXXXXX");
            //this.dataGridViewX1.Rows.Add("班級", "座號", "姓名3", "XXXXXXXX");
            //this.dataGridViewX1.Rows.Add("班級", "座號", "姓名4", "XXXXXXXX");
            //this.dataGridViewX1.Rows.Add("班級", "座號", "姓名5", "XXXXXXXX");
            //this.dataGridViewX1.Rows.Add("班級", "座號", "姓名6", "XXXXXXXX");
            //this.dataGridViewX1.Rows.Add("班級", "座號", "姓名7", "XXXXXXXX");
   
            foreach (List<JHSchool.Data.JHSCAttendRecord> JHSCARecordList in scAttendRec)
            {
                string msg = "";

                var row = this.dataGridViewX1.Rows[this.dataGridViewX1.Rows.Add()];

                row.Cells["col班級"].Value = JHSCARecordList[0].Student.Class.Name;
                row.Cells["col座號"].Value = JHSCARecordList[0].Student.SeatNo;
                row.Cells["col學生"].Value = JHSCARecordList[0].Student.Name;

                foreach (var item in JHSCARecordList)
                {
                    if (msg == "")
                    {
                        msg = item.Course.Name;
                    }
                    else
                    {
                        msg += "、";
                         msg += item.Course.Name;
                    }
                   


                    

                }
                row.Cells["col內容"].Value = msg+"修課科目重覆，請修正。" ;

            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
