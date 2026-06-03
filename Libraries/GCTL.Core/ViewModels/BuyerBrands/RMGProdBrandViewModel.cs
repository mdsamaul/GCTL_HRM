using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.BuyerBrands
{
    public class RMGProdBrandViewModel : BaseViewModel
    {
        public decimal Tc { get; set; }
        [DisplayName("Brand ID")]
        public string BrandId { get; set; }
        [DisplayName("Buyer ID")]
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
        [DisplayName("Brand Name")]
        public string Name { get; set; }
        public string LogoMonogram { get; set; }
        [DisplayName("Detail")]
        public string Detail { get; set; }

        public IFormFile logoPhoto { get; set; }
        public string Photo { get; set; }
        public string PhotoType { get; set; }

        public string CompanyCode { get; set; }
    }
}
