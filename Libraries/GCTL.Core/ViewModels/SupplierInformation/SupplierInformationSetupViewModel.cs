using GCTL.Core.ViewModels.SupplierBankAccountTemp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SupplierInformation
{
    public class SupplierInformationSetupViewModel : BaseViewModel
    {
        public decimal Tc { get; set; } = 0;
        public string SupplierId { get; set; }
        [Required(ErrorMessage = "Supplier Name is required")]
        public string SupplierName { get; set; }
        public string SupplierTitle { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierCategoryId { get; set; }
        public string SupplierCategory { get; set; }
        public string SupplierTypeId { get; set; }
        public string SupplierTypeName { get; set; }
        public string Address { get; set; }
        public string LocalOfficeAddress { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string SupplierOriginId { get; set; }
        public string SupplierOrigin { get; set; }
        public string CountryId { get; set; }
        public string CountryName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        [RegularExpression(@"^(?:\+8801\d{9}|01\d{9})$", ErrorMessage = "Invalid Phone")]
        [MaxLength(14)]
        public string Phone { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed")]
        public string Fax { get; set; }
        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }
        public string Url { get; set; }
        public string SupplierTin { get; set; }
        public string SupplierBankId { get; set; }
        public string SupplierBankName { get; set; }
        public string SupplierBankBranchId { get; set; }
        public string SupplierBankBranchName { get; set; }
        public string IalbankId { get; set; }
        public string IalbankBranchId { get; set; }
        public string ContatPerson1 { get; set; }
        public string ContatPersonName { get; set; }
        public string DesignationId { get; set; }
        public string Phone1 { get; set; }
        public string Email1 { get; set; }
        public string ContatPerson2 { get; set; }
        public string DesignationId2 { get; set; }
        public string Phone2 { get; set; }
        public string Email2 { get; set; }
        public string ContatPerson3 { get; set; }
        public string DesignationId3 { get; set; }
        public string Phone3 { get; set; }
        public string Email3 { get; set; }
        public decimal? OpeningBalance { get; set; }
        public string Optype { get; set; }
        public string SalesPersonId { get; set; }
        public string SalesPersonName { get; set; }
        public string SalesPerson { get; set; }
        public string Remarks { get; set; }
        public string Active { get; set; }
        public string ExportLicenceNo { get; set; }
        public string BranchAddress { get; set; }
        public string AccountNo { get; set; }
        public string SwiftCode { get; set; }
        public string Bin { get; set; }
        public string VatregNo { get; set; }
        public List<SupplierBankAccountTempSetupViewModel> bangkInfo { get; set; } = new List<SupplierBankAccountTempSetupViewModel>();
    }
}
