using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SalesSupplier
{
    public class SalesSupplierSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string SupplierID { get; set; } = string.Empty;
        public string? SupplierName { get; set; }
        public string? SupplierAddress { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }       
        public string CompanyCode { get; set; }
        public string? BinNo { get; set; }
        public string? VatRegNo { get; set; }
        public string? Tin { get; set; }
        public decimal? OpeningBalance { get; set; }
        public string? CountryId { get; set; }
        public string? DistrictsID { get; set; }
        public string? Place { get; set; }
        public string? FAX { get; set; }
        public string? URL { get; set; }
        public string? ContatPerson1 { get; set; }
        public string? Designation1 { get; set; }
        public string? Phone1 { get; set; }
        public string? Email1 { get; set; }
        public string? ContatPerson2 { get; set; }
        public string? Designation2 { get; set; }
        public string? Phone2 { get; set; }
        public string? Email2 { get; set; }
        public string? SupplierTypeId { get; set; }
        public string? SupplierType { get; set; }
        public string? Remarks { get; set; }
        public decimal? CreditLimit { get; set; }
        public string ShowCreateDate { get; set; }
        public string ShowModifyDate { get; set; }
    }
}
