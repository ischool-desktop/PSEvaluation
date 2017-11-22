using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudentSemesterScoreNotification
{
    [TableName("StudentSemesterScoreNotification.AbsentSetting")]
    class AbsentSetting : ActiveRecord
    {
        [FISCA.UDT.Field(Field = "target")]
        public string Target { get; set; }

        [FISCA.UDT.Field(Field = "source")]
        public string Source { get; set; }
    }
}
