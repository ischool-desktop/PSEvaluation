using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Words;
using JHSchool.Data;

namespace JHEvaluation.StudentSemesterScoreReport.Writers
{
    internal class BasicInfoWriter
    {
        private List<string> _names;
        private Dictionary<string, string> _data;
        public JHStudentRecord Student { get; set; }
        public K12.Data.SemesterHistoryItem HistoryItem { get; set; }

        public BasicInfoWriter()
        {
            InitializeField();
        }

        private void InitializeField()
        {
            _data = new Dictionary<string, string>();
            _data.Add("姓名", "");
            _data.Add("學號", "");
            _data.Add("班級", "");
            _data.Add("座號", "");
            _data.Add("班導師", "");
            _data.Add("應到日數", "");
            _data.Add("服務學習時數", "");

            #region comment
            //_data.Add("性別", "");
            //_data.Add("身分證號", "");
            //_data.Add("出生日期", "");
            //_data.Add("國籍", "");
            //_data.Add("家長或監護人", "");
            //_data.Add("關係", "");
            //_data.Add("行動電話", "");
            //_data.Add("戶籍電話", "");
            //_data.Add("戶籍地址", "");
            //_data.Add("聯絡電話", "");
            //_data.Add("聯絡地址", "");
            //_data.Add("證書字號", "");
            //_data.Add("照片粘貼處", "");
            #endregion

            _names = new List<string>(_data.Keys);
        }

        private void ClearField()
        {
            foreach (string name in new List<string>(_data.Keys))
                _data[name] = "";
        }

        public void Write(Document doc)
        {
            if (Student == null) return;

            ClearField();

            //JHParentRecord parent = JHParent.SelectByStudent(_student);
            //JHPhoneRecord phone = JHPhone.SelectByStudent(_student);
            //JHAddressRecord address = JHAddress.SelectByStudent(_student);
            //string base64 = K12.Data.Photo.SelectGraduatePhoto(Student.ID);

            string teacherName = string.Empty;
            if (Student.Class != null && Student.Class.Teacher != null)
                teacherName = Student.Class.Teacher.Name;

            //班級座號，如果學期歷程中有記錄，就抓歷程中的班座，沒有記錄則用學生身上的。
            _data["姓名"] = Student.Name;
            _data["學號"] = Student.StudentNumber;
            _data["班級"] = (HistoryItem != null) ? HistoryItem.ClassName : ((Student.Class != null ? Student.Class.Name : ""));
            _data["座號"] = (HistoryItem != null) ? "" + HistoryItem.SeatNo : "" + Student.SeatNo;
            _data["班導師"] = teacherName;
            _data["應到日數"] = (HistoryItem != null) ? "" + HistoryItem.SchoolDayCount : "";

            _data["服務學習時數"] = "0";

            if (Global._SRDict.ContainsKey(Student.ID))
            {
                _data["服務學習時數"] = Global._SRDict[Student.ID].ToString();
            }

            #region comment
            //_data["性別"] = student.Gender;
            //_data["身分證號"] = student.IDNumber;
            //_data["出生日期"] = Common.CDate(student.Birthday.HasValue ? student.Birthday.Value.ToShortDateString() : "");
            //_data["國籍"] = student.Nationality;
            //_data["家長或監護人"] = (parent != null) ? parent.Custodian.Name : "";
            //_data["關係"] = (parent != null) ? parent.Custodian.Relationship : "";
            //_data["行動電話"] = (phone != null) ? phone.Cell : "";
            //_data["戶籍電話"] = (phone != null) ? phone.Permanent : "";
            //_data["戶籍地址"] = (address != null) ? address.Permanent.ToString() : "";
            //_data["聯絡電話"] = (phone != null) ? phone.Contact : "";
            //_data["聯絡地址"] = (address != null) ? address.Mailing.ToString() : "";
            //_data["證書字號"] = ""; //先放著…
            //_data["照片粘貼處"] = base64;
            #endregion

            doc.MailMerge.Execute(GetFieldName(), GetFieldValue());
        }

        private string[] GetFieldName()
        {
            return _names.ToArray();
        }

        private string[] GetFieldValue()
        {
            List<string> values = new List<string>();
            foreach (string name in _names)
                values.Add(_data[name]);
            return values.ToArray();
        }
    }
}
