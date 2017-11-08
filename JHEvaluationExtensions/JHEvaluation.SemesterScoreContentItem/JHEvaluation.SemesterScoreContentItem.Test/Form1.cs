using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.Data;
using JHEvaluation.SemesterScoreContentItem.Forms;

namespace JHEvaluation.SemesterScoreContentItem.Test
{
    public partial class Form1 : Form
    {
        private JHStudentRecord Student { get; set; }
        public Form1()
        {
            InitializeComponent();

            FISCA.Authentication.DSAServices.SetLicense("SmartSchoolLicense.key");
            FISCA.Authentication.DSAServices.Login("adrnin", "1234");

            Student = JHStudent.SelectByID("147285");
            listBox1.DisplayMember = "SchoolYear";
            foreach (var record in JHSemesterScore.SelectByStudentID(Student.ID))
            {
                listBox1.Items.Add(record);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            JHSemesterScoreRecord record = listBox1.SelectedItem as JHSemesterScoreRecord;
            SemesterScoreEditor editor = new SemesterScoreEditor(Student, record);
            editor.ShowDialog();
        }
    }
}
