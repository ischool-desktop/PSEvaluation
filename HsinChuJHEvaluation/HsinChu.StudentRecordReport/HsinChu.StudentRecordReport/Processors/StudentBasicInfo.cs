using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;
using K12.Data;

namespace HsinChu.StudentRecordReport
{
    class StudentBasicInfo
    {
        private List<string> _names;
        private Dictionary<string, string> _data;
        private List<SemesterHistoryItem> _semesterHistoryList;
        public StudentBasicInfo()
        {
            InitializeField();
        }

        public void SetStudent(JHStudentRecord student, List<SemesterHistoryItem> semesterHistoryList)
        {
            ClearField();
            // 學期歷程
            _semesterHistoryList = semesterHistoryList;
            //ParentRecord parent = student.GetParent();
            //PhoneRecord phone = student.GetPhone();
            //AddressRecord address = student.GetAddress();

            JHParentRecord parent = JHParent.SelectByStudent(student);
            JHPhoneRecord phone = JHPhone.SelectByStudent(student);
            JHAddressRecord address = JHAddress.SelectByStudent(student);
            string base64 = K12.Data.Photo.SelectGraduatePhoto(student.ID);
            
            // 畢業資訊
            JHLeaveInfoRecord GradeInfo = JHLeaveIfno.SelectByStudentID(student.ID);

            string EnrollSchoolYear="";

            foreach(JHUpdateRecordRecord rec in JHUpdateRecord.SelectByStudentID(student.ID))
                if(rec.UpdateCode=="1")
                {
                    int sc;
                    if(rec.EnrollmentSchoolYear.Length==6)
                    {
                        int.TryParse(rec.EnrollmentSchoolYear.Substring(0,4),out sc);
                        EnrollSchoolYear = (sc - 1911) + rec.EnrollmentSchoolYear.Substring(4, 2);
                    }
                }

            _data["姓名"] = student.Name;
            _data["性別"] = student.Gender;
            _data["身分證號"] = student.IDNumber;
            _data["學號"] = student.StudentNumber;
            _data["出生日期"] = Global.CDate(student.Birthday.HasValue ? student.Birthday.Value.ToShortDateString() : "");
            _data["國籍"] = student.Nationality;
            _data["家長或監護人"] = (parent != null) ? parent.Custodian.Name : "";
            _data["關係"] = (parent != null) ? parent.Custodian.Relationship : "";
            _data["行動電話"] = (phone != null) ? phone.Cell : "";
            _data["戶籍電話"] = (phone != null) ? phone.Permanent : "";
            _data["戶籍地址"] = (address != null) ? address.Permanent.ToString() : "";
            _data["聯絡電話"] = (phone != null) ? phone.Contact : "";
            _data["聯絡地址"] = (address != null) ? address.Mailing.ToString() : "";
            _data["證書字號"] = GradeInfo.DiplomaNumber;
            _data["入學年月"] = EnrollSchoolYear;
            _data["照片"] = base64;

            // 處理服務學習時數
            if (Global._SLRDict.ContainsKey(student.ID))
            {
                foreach (SemesterHistoryItem shi in _semesterHistoryList)
                {
                    string key = shi.SchoolYear + "_" + shi.Semester;
                    if (Global._SLRDict[student.ID].ContainsKey(key))
                    {
                        string val = Global._SLRDict[student.ID][key];

                        if ((shi.GradeYear == 1 || shi.GradeYear == 7) && shi.Semester == 1)
                            _data["SLR1A"] = val;

                        if ((shi.GradeYear == 1 || shi.GradeYear == 7) && shi.Semester == 2)
                            _data["SLR1B"] = val;

                        if ((shi.GradeYear == 2 || shi.GradeYear == 8) && shi.Semester == 1)
                            _data["SLR2A"] = val;

                        if ((shi.GradeYear == 2 || shi.GradeYear == 8) && shi.Semester == 2)
                            _data["SLR2B"] = val;

                        if ((shi.GradeYear == 3 || shi.GradeYear == 9) && shi.Semester == 1)
                            _data["SLR3A"] = val;

                        if ((shi.GradeYear == 3 || shi.GradeYear == 9) && shi.Semester == 2)
                            _data["SLR3B"] = val;
                    }
                }
            }

        }

        private void InitializeField()
        {
            _data = new Dictionary<string, string>();
            _data.Add("姓名", "");
            _data.Add("性別", "");
            _data.Add("身分證號", "");
            _data.Add("學號", "");
            _data.Add("出生日期", "");
            _data.Add("國籍", "");
            _data.Add("家長或監護人", "");
            _data.Add("關係", "");
            _data.Add("行動電話", "");
            _data.Add("戶籍電話", "");
            _data.Add("戶籍地址", "");
            _data.Add("聯絡電話", "");
            _data.Add("聯絡地址", "");
            _data.Add("證書字號", "");
            _data.Add("照片", "");
            _data.Add("入學年月", "");
            // 各學年度學期服務學習時數
            _data.Add("SLR1A", "0");
            _data.Add("SLR1B", "0");
            _data.Add("SLR2A", "0");
            _data.Add("SLR2B", "0");
            _data.Add("SLR3A", "0");
            _data.Add("SLR3B", "0");         
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
