using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ManualEntryApproval
{
    public class ManualApprovalRequest:BaseViewModel
    {
        public List<string> CheckedApprovalList { get; set; }
        public string Remark { get; set; } = "";
    }
}
