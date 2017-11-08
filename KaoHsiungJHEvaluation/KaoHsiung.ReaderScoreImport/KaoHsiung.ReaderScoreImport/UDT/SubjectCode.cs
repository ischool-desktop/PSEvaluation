using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace KaoHsiung.ReaderScoreImport.UDT
{
    [TableName("ReaderScoreImport.SubjectCode")]
    public class SubjectCode : ActiveRecord
    {
        [Field(Field = "Subject", Indexed = true)]
        public string Subject { get; set; }

        [Field(Field = "Code", Indexed = false)]
        public string Code { get; set; }
    }
}
