using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.MeritDetail
{
    public class MeritDetailData
    {
        public List<MeritDetailItem> Items { get; set; }

        public MeritDetailData()
        {
            Items = new List<MeritDetailItem>();
        }
    }
}
