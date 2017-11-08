using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FISCA;
using FISCA.Presentation;
using Framework.Security;
using Framework;

namespace HsinChu.TransferReport
{
    /// <summary>
    /// 轉學成績證明書
    /// </summary>
    public static class Program
    {
        [MainMethod]
        public static void Main()
        {
            Catalog detail = RoleAclSource.Instance["學生"]["報表"];
            detail.Add(new ReportFeature("HsinChu.JHEvaluation.Student.Report0001", "轉學成績證明書"));

            MenuButton mb = MotherForm.RibbonBarItems["學生", "資料統計"]["報表"]["成績相關報表"]["轉學成績證明書"];
            mb.Enable = User.Acl["HsinChu.JHEvaluation.Student.Report0000"].Executable;
            mb.Click += delegate
            {
                TransferReportForm form = new TransferReportForm();
                form.ShowDialog();
            };
        }
    }
}
