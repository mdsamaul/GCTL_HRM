using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRM_Size
{
    public class HRM_SizeSetupViewModel :BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string SizeID { get; set; }
        public string SizeName { get; set; }
        public string ShortName { get; set; }        
        public string CompanyCode { get; set; }
        public string ShowCreateDate { get; set; }
        public string ShowModifyDate { get; set; }
    }
}
