using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using System.Windows.Forms;
using Framework;
using JHSchool.Evaluation.Ranking;
using JHSchool.Data;

namespace JHSchool.Evaluation.Ranking
{
    internal abstract class AbstractReport
    {
        protected Dictionary<string, JHStudentRecord> _studentDict;
        protected List<RankData> _rankData;

        public RankType RankType { get; set; }
        public string ReportName { get; set; }

        public AbstractReport()
        {
            RankType = RankType.Class;
            ReportName = string.Empty;
        }

        public abstract void Export();

        protected void Save(Workbook book)
        {
            SaveFileDialog sdf = new SaveFileDialog();
            sdf.FileName = ReportName;
            sdf.Filter = "Excel檔案(*.xls)|*.xls";

            if (sdf.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    book.Save(sdf.FileName, FileFormatType.Excel2003);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存失敗。" + ex.Message);
                    return;
                }

                try
                {
                    if (MsgBox.Show("排名完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sdf.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MsgBox.Show("開啟失敗。" + ex.Message);
                }
            }
        }

        public int SortStudents(JHStudentRecord x, JHStudentRecord y)
        {
            JHClassRecord c1 = x.Class;
            JHClassRecord c2 = y.Class;
            if (c1 != null && c2 != null && c1 == c2)
            {
                int seatNo1 = int.MinValue, seatNo2 = int.MinValue;
                int.TryParse("" + x.SeatNo, out seatNo1);
                int.TryParse("" + y.SeatNo, out seatNo2);
                if (seatNo1 == seatNo2)
                    return x.StudentNumber.CompareTo(y.StudentNumber);
                else
                    return seatNo1.CompareTo(seatNo2);
            }
            else
            {
                if (c1 == null)
                    return int.MinValue;
                else if (c2 == null)
                    return int.MaxValue;
                return c1.CompareTo(c2);
            }
        }
    }
}
