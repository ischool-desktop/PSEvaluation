using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using Aspose.Words;
using System.IO;

namespace KaoHsingReExamScoreReport
{
     [FISCA.UDT.TableName("KaoHsingReExamScoreReport_ReDomainForStudentForm_config")]
    public class Configure:ActiveRecord
    {
        /// <summary>
        /// 列印範本
        /// </summary>
        [FISCA.UDT.Field]
        private string TemplateStream { get; set; }
        public Document Template { get; set; }


        /// <summary>
        /// 在儲存前，把資料填入儲存欄位中
        /// </summary>
        public void Encode()
        {
            MemoryStream stream = new MemoryStream();
            this.Template.Save(stream, Aspose.Words.SaveFormat.Doc);
            this.TemplateStream = Convert.ToBase64String(stream.ToArray());
        }
        /// <summary>
        /// 在資料取出後，把資料從儲存欄位轉換至資料欄位
        /// </summary>
        public void Decode()
        {
            if(!string.IsNullOrEmpty(this.TemplateStream))
                this.Template = new Document(new MemoryStream(Convert.FromBase64String(this.TemplateStream)));
        }
    }
}
