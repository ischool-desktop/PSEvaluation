using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using System.Threading;

namespace JHEvaluation.ScoreCalculation
{
    public partial class CalculationTest : BaseForm, IStatusReporter
    {
        public CalculationTest(string mode)
        {
            InitializeComponent();
            Text = mode;
        }

        List<StudentScore> students;
        private void button1_Click(object sender, EventArgs e)
        {
            StudentScore.SetClassMapping();

            JHClassRecord cls = JHClass.SelectByID("24");//正興 301 班。

            txtMsg.Text = string.Empty;
            txtMsg.AppendText("計算班級：" + cls.Name + "\n");

            ThreadPool.QueueUserWorkItem(new WaitCallback(ReadAttendScore), cls);
        }

        private void ReadAttendScore(object state)
        {
            JHClassRecord cls = state as JHClassRecord;

            List<string> ids = new List<string>();

            foreach (JHStudentRecord each in cls.Students)
                ids.Add(each.ID);

            students = ids.ToStudentScore();

            List<StudentScore> noValid;

            //string[] subjects = new string[] { "數學" };

            //helper.ReadAttendScore(students, 98, 1, new string[] { }, this);
            //List<StudentScore> noCourses = helper.CalcuateAttendScore(students, this); //回傳沒有修課的學生。
            //helper.SaveAttendScore(students, new string[] { }, this);
            //helper.ReadSemesterScore(students, 98, 1, this);
            List<StudentScore> noRule = students.ReadCalculationRule(this); //沒有計算規則的學生。
            //helper.ReadSemesterScore(students, this);

            //if (noRule.Count > 0)
            //{
            //    Feedback("有部份學生沒有設定成績計算規則。", 0);
            //    return;
            //}

            //helper.CalcuateSubjectSemesterScore(students, new string[] { });
            //helper.CalcuateDomainSemesterScore(students, new string[] { });
            //helper.SaveSemesterScore(students, this);

            students.ReadSemesterScore(this); //讀取所有學期的成績。
            students.ReadSemesterHistory(this); //讀取學期歷程。
            noValid = students.VaidSixSemesterHistory(); //驗證學期歷程是否完整。
            noValid = students.ValidSixSemesterDomainScore(); //驗證學期成績是否完整。
            students.ReadGraduateScore(this); //讀取畢業成績。
            students.CalculateGraduateScore(); //計算畢業成績。
            students.SaveGraduateScore(this);//儲存畢業成績。

            Feedback("成績計算完成。", 0);
        }

        #region IStatusReporter 成員

        public void Feedback(string message, int percentage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, int>(Feedback), new object[] { message, percentage });
            }
            else
            {
                txtMsg.AppendText((percentage == 0 ? message : string.Format("{0} ({1})", message, percentage)) + "\n");
                Application.DoEvents();
            }
        }

        #endregion
    }
}
