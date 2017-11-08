using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaoHsiung.ReaderScoreImport.Validation
{
    internal interface IRecordValidator<T>
    {
        string Validate(T record);
    }
}
