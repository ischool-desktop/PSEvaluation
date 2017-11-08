using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent
{
    public interface IStep
    {
        bool Valid();
        string ErrorMessage { get; set; }
        void OnChangeStep();
    }
}
