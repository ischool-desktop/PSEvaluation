using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaoHsiung.ReaderScoreImport
{
    internal class DomainValidator : IColumnValidator
    {
        private List<string> _domains;

        public DomainValidator()
        {
            _domains = new List<string>() { "語文", "數學", "社會", "藝術與人文", "自然與生活科技", "健康與體育", "綜合活動", "彈性課程" };
        }

        #region IColumnValidator 成員

        public bool IsValid(string input)
        {
            if (_domains.Contains(input)) return true;
            else return false;
        }

        public string GetErrorMessage()
        {
            return "必須為七大領域。";
        }

        #endregion
    }
}
