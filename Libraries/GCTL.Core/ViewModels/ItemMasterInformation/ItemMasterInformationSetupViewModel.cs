using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.Brand;

namespace GCTL.Core.ViewModels.ItemMasterInformation
{
    public class ItemMasterInformationSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string UnitID { get; set; }
        public decimal? PurchaseCost { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
        public string CatagoryId { get; set; }
        public string CatagoryName { get; set; }
        public string BrandId { get; set; }
        public string BrandName { get; set; }
        public string ShowModifyDate { get; set; }
        public string ShowCreateDate { get; set; }
        public List<BrandSetupViewModel> BrandList {get; set;}
    }
}
