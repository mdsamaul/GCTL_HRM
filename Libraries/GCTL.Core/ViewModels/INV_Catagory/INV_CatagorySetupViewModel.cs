using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.INV_Catagory
{
    public class INV_CatagorySetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string CatagoryID { get; set; }
        public string CatagoryName { get; set; }
        public string ShortName { get; set; }        
        public string CompanyCode { get; set; }
        public string ShowCreateDate { get; set; }
        public string ShowModifyDate { get; set; }

    }
}
