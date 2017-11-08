using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;

namespace KaoHsiung.TransferReport
{
    class StudentBasicInfo
    {
        private List<string> _names;
        private Dictionary<string, string> _data;

        public StudentBasicInfo()
        {
            InitializeField();
        }

        public void SetStudent(JHStudentRecord student)
        {
            ClearField();

            JHParentRecord parent = JHParent.SelectByStudent(student);
            JHPhoneRecord phone = JHPhone.SelectByStudent(student);
            JHAddressRecord address = JHAddress.SelectByStudent(student);
            string base64 = K12.Data.Photo.SelectGraduatePhoto(student.ID);

            _data["姓名"] = student.Name;
            _data["性別"] = student.Gender;
            _data["身分證號"] = student.IDNumber;
            _data["學號"] = student.StudentNumber;
            _data["班級"] = (student.Class != null ? student.Class.Name : "");
            _data["座號"] = "" + student.SeatNo;
            //_data["出生日期"] = Common.CDate(student.Birthday.HasValue ? student.Birthday.Value.ToShortDateString() : "");
            //_data["國籍"] = student.Nationality;
            //_data["家長或監護人"] = (parent != null) ? parent.Custodian.Name : "";
            //_data["關係"] = (parent != null) ? parent.Custodian.Relationship : "";
            //_data["行動電話"] = (phone != null) ? phone.Cell : "";
            //_data["戶籍電話"] = (phone != null) ? phone.Permanent : "";
            _data["戶籍地址"] = (address != null) ? address.Permanent.ToString() : "";
            //_data["聯絡電話"] = (phone != null) ? phone.Contact : "";
            //_data["聯絡地址"] = (address != null) ? address.Mailing.ToString() : "";
            //_data["證書字號"] = ""; //先放著…
            _data["照片粘貼處"] = base64;
        }

        private void InitializeField()
        {
            _data = new Dictionary<string, string>();
            _data.Add("姓名", "");
            _data.Add("性別", "");
            _data.Add("身分證號", "");
            _data.Add("學號", "");
            _data.Add("班級", "");
            _data.Add("座號", "");
            //_data.Add("出生日期", "");
            //_data.Add("國籍", "");
            //_data.Add("家長或監護人", "");
            //_data.Add("關係", "");
            //_data.Add("行動電話", "");
            //_data.Add("戶籍電話", "");
            _data.Add("戶籍地址", "");
            //_data.Add("聯絡電話", "");
            //_data.Add("聯絡地址", "");
            //_data.Add("證書字號", "");
            _data.Add("照片粘貼處", "");

            _names = new List<string>(_data.Keys);
        }

        private void ClearField()
        {
            foreach (string name in new List<string>(_data.Keys))
                _data[name] = "";
        }

        public string[] GetFieldName()
        {
            return _names.ToArray();
        }

        public string[] GetFieldValue()
        {
            List<string> values = new List<string>();
            foreach (string name in _names)
                values.Add(_data[name]);
            return values.ToArray();
        }
    }
}
