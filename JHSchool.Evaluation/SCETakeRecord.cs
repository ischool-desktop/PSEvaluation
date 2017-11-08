using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool.Evaluation
{
    public class SCETakeRecord
    {
        public string ID { get; private set; }
        public decimal? Score { get; private set; }
        public int? Effort { get; private set; }
        public string Text { get; private set; }
        public string RefSCAttendID { get; private set; }
        public string RefExamID { get; private set; }
        public string RefStudentID { get; private set; }
        public string RefCourseID { get; private set; }

        //public SCAttendRecord SCAttend { get { return JHSchool.Evaluation.SCAttend.Instance.Items[RefSCAttendID]; } }
        public ExamRecord Exam { get { return JHSchool.Evaluation.Exam.Instance.Items[RefExamID]; } }

        #region Service欄位參考
        //<Field OutputType="Attribute" Source="ID" Target="sce.id" />
        //<Field OutputConverter="AbsenceOutput" Source="Score" Target="sce.score" />
        //<Field Source="ExamID" Target="sce.ref_exam_id" />
        //<Field Source="AttendID" Target="sce.ref_sc_attend_id" />

        //<Field Source="RefStudentID" Target="sc.ref_student_id" />
        //<Field Source="RefCourseID" Target="sc.ref_course_id" />
        //<Field OutputConverter="RequiredOutput" Source="IsRequired" Target="sc.is_required" />
        //<Field OutputConverter="RequiredByOutput" Source="RequiredBy" Target="sc.required_by" />
        //<Field Source="AttendScore" Target="sc.score" />

        //<Field Source="ExamName" Target="exam.exam_name" />
        //<Field OutputType="Xml" Source="Extensions" Target="sce.extensions" />
        #endregion

        internal SCETakeRecord(XmlElement data)
        {
            DSXmlHelper helper = new DSXmlHelper(data);

            ID = helper.GetText("@ID");
            decimal d;
            Score = decimal.TryParse(helper.GetText("Score"), out d) ? (decimal?)d : null;
            RefSCAttendID = helper.GetText("AttendID");
            RefExamID = helper.GetText("ExamID");
            RefStudentID = helper.GetText("RefStudentID");
            RefCourseID = helper.GetText("RefCourseID");

            int i;
            Effort = int.TryParse(helper.GetText("Extension/Extension/Effort"), out i) ? (int?)i : null;
            Text = helper.GetText("Extension/Extension/Text");

            #region ResponseXml參考
            //<Score ID="2285">
            //    <Extensions />
            //    <RequiredBy>校訂</RequiredBy>
            //    <AttendScore />
            //    <IsRequired>選</IsRequired>
            //    <ExamName>第二次月考</ExamName>
            //    <AttendID>2967</AttendID>
            //    <RefStudentID>170626</RefStudentID>
            //    <RefCourseID>111</RefCourseID>
            //    <Score>80</Score>
            //    <ExamID>494</ExamID>
            //</Score>
            #endregion
        }
    }
}
