using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaoHsiung.ReaderScoreImport.Model;
using KaoHsiung.ReaderScoreImport.Mapper;

namespace KaoHsiung.ReaderScoreImport.Validation.RecordValidators
{
    internal class ClassCodeValidator : IRecordValidator<RawData>
    {
        #region IRecordValidator<RawData> 成員
        public string Validate(RawData record)
        {
            if (ClassCodeMapper.Instance.CheckCodeExists(record.ClassCode))
                return string.Empty;
            else
                return string.Format("班級代碼「{0}」不存在。", record.ClassCode);
        }
        #endregion
    }
}
