using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaoHsiung.ReaderScoreImport.Model;
using JHSchool.Data;

namespace KaoHsiung.ReaderScoreImport.Validation.RecordValidators
{
    internal class StudentValidator : IRecordValidator<DataRecord>
    {
        private StudentNumberDictionary _studentDict;
        public StudentValidator(StudentNumberDictionary studentDict)
        {
            _studentDict = studentDict;
        }

        #region IRecordValidator<DataRecord> 成員

        public string Validate(DataRecord record)
        {
            if (!_studentDict.ContainsKey(record.StudentNumber))
                return string.Format("學號「{0}」不存在系統中。", record.StudentNumber);
            else if (_studentDict[record.StudentNumber].Status != K12.Data.StudentRecord.StudentStatus.一般)
                return string.Format("學生「{0}({1})」非一般生。", _studentDict[record.StudentNumber].Name, record.StudentNumber);
            else
                return string.Empty;
        }

        #endregion
    }
}
