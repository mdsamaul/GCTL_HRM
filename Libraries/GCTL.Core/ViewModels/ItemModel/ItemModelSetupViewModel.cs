using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ItemModel
{
    public class ItemModelSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }             
        public string ModelID { get; set; }            
        public string? ModelName { get; set; }       
        public string? ShortName { get; set; }  
        public string CompanyCode { get; set; }        
        public string? BrandID { get; set; }
        public string? BrandName { get; set; }
        public string ShowCreateDate { get; set; }
        public string ShowModifyDate { get; set; }
    }
}
