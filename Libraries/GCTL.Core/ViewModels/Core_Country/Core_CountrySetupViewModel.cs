using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.Core_Country
{
    public class Core_CountrySetupViewModel:BaseViewModel
    {
        public int CountryCode { get; set; }           
        public string CountryID { get; set; }            
        public string? CountryName { get; set; }        
        public string? IOCCode { get; set; }             
        public string? ISOCode { get; set; }
        public string ShowCreateDate { get; set; }
        public string ShowModifyDate { get; set; }
    }
}
