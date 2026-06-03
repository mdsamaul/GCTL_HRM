using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmDefExamGroupInfos
{
    public class HrmDefExamGroupInfosSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string ShortName { get; set; }
        public string CompanyCode { get; set; }
 
    }
}
