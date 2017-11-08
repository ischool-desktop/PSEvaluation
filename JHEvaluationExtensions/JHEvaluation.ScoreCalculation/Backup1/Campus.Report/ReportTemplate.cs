using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using Aspose.Cells;
using System.IO;

namespace Campus.Report
{
    public class ReportTemplate
    {
        /// <summary>
        /// 報表樣板類型。
        /// </summary>
        public TemplateType Type { get; private set; }
        private Stream Stream { get; set; }

        private ReportTemplate()
        {
            Stream = new MemoryStream();
        }

        public ReportTemplate(Aspose.Words.Document document)
            : this()
        {
            Type = TemplateType.Word;
            document.Save(Stream, SaveFormat.Doc);
        }

        public ReportTemplate(Aspose.Cells.Workbook workbook)
            : this()
        {
            Type = TemplateType.Excel;
            workbook.Save(Stream, FileFormatType.Excel2003);
        }

        /// <summary>
        /// 指定檔案轉成 ReportTemplate
        /// </summary>
        /// <param name="fileInfo">FileInfo 物件</param>
        /// <param name="type">報表樣板類型</param>
        public ReportTemplate(FileInfo fileInfo, TemplateType type)
            : this()
        {
            Type = type;
            if (type == TemplateType.Excel)
            {
                Workbook book = new Workbook();
                book.Open(fileInfo.FullName, FileFormatType.Excel2003);
                book.Save(Stream, FileFormatType.Excel2003);
            }
            else if (type == TemplateType.Word)
            {
                Document doc = new Document(fileInfo.FullName, LoadFormat.Doc, string.Empty);
                doc.Save(Stream, SaveFormat.Doc);
            }
            else
                throw new Exception("無效的 TemplateType");
        }

        /// <summary>
        /// 傳入樣板 Base64 字串及樣板類型，建立 ReportTemplate。
        /// </summary>
        /// <param name="base64">Base64 字串</param>
        /// <param name="type">樣板類型</param>
        public ReportTemplate(string base64, TemplateType type)
            : this(Convert.FromBase64String(base64), type)
        {
            //Type = type;
            //Stream = new MemoryStream(Convert.FromBase64String(base64));
        }

        /// <summary>
        /// 傳入 byte[] 及樣板類型，建立 ReportTemplate。
        /// </summary>
        /// <example> This sample shows how to call the GetZero method.
        /// <code>
        /// new ReportTemplate(Properties.Resources.學期成績單, TemplateType.Word)
        /// </code>
        /// </example>
        /// <param name="bytes">Byte 陣列</param>
        /// <param name="type">樣板類型</param>
        public ReportTemplate(byte[] bytes, TemplateType type)
        {
            Type = type;
            Stream = new MemoryStream(bytes);
        }

        #region Methods
        /// <summary>
        /// 轉成 Base64 字串
        /// </summary>
        /// <returns></returns>
        public string ToBase64()
        {
            string base64 = string.Empty;
            try
            {
                Stream.Position = 0;
                byte[] buffer = new byte[Stream.Length];
                Stream.Read(buffer, 0, (int)Stream.Length);
                base64 = Convert.ToBase64String(buffer);
            }
            catch (Exception ex)
            {
            }
            return base64;
        }

        public byte[] ToBinary()
        {
            byte[] binary = null;
            try
            {
                Stream.Position = 0;
                byte[] buffer = new byte[Stream.Length];
                Stream.Read(buffer, 0, (int)Stream.Length);
                binary = buffer;
            }
            catch (Exception)
            {
            }
            return binary;
        }

        /// <summary>
        /// 轉成 Aspose.Words.Document
        /// </summary>
        /// <returns></returns>
        public Aspose.Words.Document ToDocument()
        {
            try
            {
                Stream.Position = 0;
                Document doc = new Document(Stream);
                return doc;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 轉成 Aspose.Cells.Workbook
        /// </summary>
        /// <returns></returns>
        public Aspose.Cells.Workbook ToWorkbook()
        {
            try
            {
                Stream.Position = 0;
                Workbook book = new Workbook();
                book.Open(Stream);
                return book;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
