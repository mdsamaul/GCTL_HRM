using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRM_Def_Floor
{
    public class HRM_Def_FloorSetupViewModel :BaseViewModel
    {
        public decimal AutoId { get; set; }           
        public string FloorCode { get; set; }        
        public string? FloorName { get; set; }        
        public string? ShortName { get; set; }              
        public string CompanyCode { get; set; }      
        public string EmployeeID { get; set; }
        public string ShowCreateDate { get; set; }
        public string ShowModifyDate { get; set; }
    }
}
