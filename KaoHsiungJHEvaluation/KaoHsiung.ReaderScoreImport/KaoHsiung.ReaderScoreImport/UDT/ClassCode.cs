using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace KaoHsiung.ReaderScoreImport.UDT
{
    [TableName("ReaderScoreImport.ClassCode")]
    public class ClassCode : ActiveRecord
    {
        [Field(Field = "ClassName", Indexed = true)]
        public string ClassName { get; set; }

        [Field(Field = "Code", Indexed = false)]
        public string Code { get; set; }
    }
}
