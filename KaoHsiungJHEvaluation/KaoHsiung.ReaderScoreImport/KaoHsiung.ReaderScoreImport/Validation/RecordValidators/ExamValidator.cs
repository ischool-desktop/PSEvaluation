using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaoHsiung.ReaderScoreImport.Model;
using JHSchool.Data;

namespace KaoHsiung.ReaderScoreImport.Validation.RecordValidators
{
    internal class ExamValidator : IRecordValidator<DataRecord>
    {
        private List<string> _examNameList;
        public ExamValidator(List<JHExamRecord> examList)
        {
            _examNameList = new List<string>();
            foreach (JHExamRecord exam in examList)
            {
                if (!_examNameList.Contains(exam.Name))
                    _examNameList.Add(exam.Name);
            }
        }

        #region IRecordValidator<DataRecord> 成員

        public string Validate(DataRecord record)
        {
            if (!_examNameList.Contains(record.Exam))
                return string.Format("試別「{0}」不存在系統中。", record.Exam);
            else
                return string.Empty;
        }

        #endregion
    }
}
