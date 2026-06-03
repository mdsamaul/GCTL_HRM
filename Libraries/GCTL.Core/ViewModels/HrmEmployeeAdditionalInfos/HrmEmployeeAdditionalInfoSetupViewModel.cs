using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmEmployeeAdditionalInfos
{
    public class HrmEmployeeAdditionalInfoSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }

        public string DesignationName { get; set; }
        public string EmployeAddInfoId { get; set; }
       
        public string PassportNo { get; set; }
        public string PassportPlaceOfIssue { get; set; }
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PassportIssueDate { get; set; }
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PassportExpiryDate { get; set; }
        [Required]
        public string SalaryBankId { get; set; }
        public string SalaryBankName { get; set; }
        public string SalaryBranchId { get; set; }

        public string SalaryBranchName { get; set; }
        public string BranchAddres { get; set; }
        public string BankAcname { get; set; }
        public string BankAcNo { get; set; }
        public string AtmCardNo { get; set; }
        public string LicenseNo { get; set; }
        public string LicenseType { get; set; }
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LicenseIssueDate { get; set; }
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LicenseExpireDate { get; set; }
        public string SymbolOfVehicleClass { get; set; }
        public string LicensePlaceOfIssue { get; set; }
        public string WorkPermitNo { get; set; }
        public string WorkPermitType { get; set; }
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? WpEffectiveDate { get; set; }
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? WpExpireDate { get; set; }
       
        public string CompanyCode { get; set; }

        public string CompanyName { get; set; }
       
        public string PassportName { get; set; }
      
        public string BranchCode { get; set; }
        public string CoreBranchName { get; set; }

        [Required]
        public string BankIducbl { get; set; }
        public string BankNameUcbl { get; set; }
        public string BankBranchIducbl { get; set; }
        public string BankBranchNameUcbl { get; set; }
        public string BranchAddressUcbl { get; set; }
        public string BankAcNameUcbl { get; set; }
        public string BankAcNoUcbl { get; set; }

        public string BankAcNoSibl { get; set; }
        [Required]
        public string BankIdsibl { get; set; }
        public string BankNameSibl { get; set; }
        public string BankBranchIdsibl { get; set; }
        public string BankBranchNameSibl { get; set; }
        public string BranchAddressSibl { get; set; }
        public string BankAcNameSibl { get; set; }
    }
}
