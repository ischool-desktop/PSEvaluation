using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace Campus.Report
{
    public class ReportSaver
    {
        /// <summary>
        /// 儲存 Word 報表。
        /// </summary>
        /// <param name="document">要儲存的報表物件</param>
        /// <param name="filename">儲存的檔案名稱</param>
        public static void SaveDocument(Aspose.Words.Document document, string filename)
        {
            string path = CreatePath(filename, ".doc");

            try
            {
                document.Save(path);
                OpenFile(path, path);
            }
            catch (Exception ex)
            {
                //MsgBox.Show("儲存失敗" + ex.Message);
            }
        }

        /// <summary>
        /// 儲存 Word 報表。
        /// </summary>
        /// <param name="document">要儲存的報表物件</param>
        /// <param name="filename">儲存的檔案名稱</param>
        /// <param name="outputType">輸出檔案格式</param>
        public static void SaveDocument(Aspose.Words.Document document, string filename, OutputType outputType)
        {
            if (outputType == OutputType.Word)
                SaveDocument(document, filename);
            else if (outputType == OutputType.PDF)
            {
                string path = CreatePath(filename, ".pdf");

                FileInfo fi = new FileInfo(path);

                DirectoryInfo folder = new DirectoryInfo(Path.Combine(fi.DirectoryName, Path.GetRandomFileName()));
                if (!folder.Exists) folder.Create();

                FileInfo fileinfo = new FileInfo(Path.Combine(folder.FullName, fi.Name));

                string XmlFileName = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".xml";
                string PDFFileName = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".pdf";

                document.Save(XmlFileName, Aspose.Words.SaveFormat.AsposePdf);

                Aspose.Pdf.Pdf pdf = new Aspose.Pdf.Pdf();

                pdf.BindXML(XmlFileName, null);
                pdf.Save(PDFFileName);

                if (File.Exists(path))
                    File.Delete(Path.Combine(fi.DirectoryName, fi.Name));

                File.Move(PDFFileName, path);
                folder.Delete(true);

                OpenFile(path, fi.Name);
            }
        }

        /// <summary>
        /// 儲存 Excel 報表。
        /// </summary>
        /// <param name="workbook">要儲存的報表物件</param>
        /// <param name="filename">儲存的檔案名稱</param>
        public static void SaveWorkbook(Aspose.Cells.Workbook workbook, string filename)
        {
            string path = CreatePath(filename, ".xls");

            try
            {
                workbook.Save(path);
                //FISCA.LogAgent.ApplicationLog.Log("成績系統.報表", "列印" + ReportName, string.Format("產生{0}份{1}", Students.Count, ReportName));

                //MotherForm.SetStatusBarMessage(ReportName + "產生完成");

                //if (MsgBox.Show(ReportName + "產生完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //{
                //    System.Diagnostics.Process.Start(path);
                //}
                OpenFile(path, path);
            }
            catch (Exception ex)
            {
                //MsgBox.Show("儲存失敗" + ex.Message);
            }
        }

        private static string CreatePath(string filename, string ext)
        {
            string path = Path.Combine(Application.StartupPath, "Reports");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            string fullname = filename.EndsWith(ext) ? filename : filename + ext;
            path = Path.Combine(path, fullname);

            #region 如果檔案已經存在
            if (File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path));
                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }
            #endregion

            return path;
        }

        private static void OpenFile(string path, string filename)
        {
            if (MessageBox.Show(filename + "產生完成，是否立刻開啟？", "ischool", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    System.Diagnostics.Process.Start(path);
                }
                catch (Exception)
                {
                    MsgBox.Show("開啟檔案發生錯誤，您可能沒有相關的應用程式可以開啟此類型檔案。");
                }
            }
        }

        /// <summary>
        /// 輸出檔案格式
        /// </summary>
        public enum OutputType
        {
            Word, PDF
        }
    }
}
