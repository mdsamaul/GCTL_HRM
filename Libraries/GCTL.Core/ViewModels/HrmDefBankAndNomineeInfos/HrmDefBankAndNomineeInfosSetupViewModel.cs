using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmDefBankAndNomineeInfos
{
    public class HrmDefBankAndNomineeInfosSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string BankAndNomineeId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        public string BankId { get; set; }
        public string BankBranchId { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNo { get; set; }
        public string AtmcardNo { get; set; }
        public string NomineeName { get; set; }
        public string Relation { get; set; }
        public string PresentAddress { get; set; }
        public string ParmanentAddress { get; set; }
        [Required]
        public string CompanyCode { get; set; }
        public string EmployeeId2 { get; set; }
        public string ComapanyName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeName { get; set; }
        public string BranchAddress { get; set; }
        public string NomineePhotoUrl { get; set; }
        public string NomineeSignatureUrl { get; set; }
        public bool IsClearPhoto { get; set; }
        public bool IsClearSignature { get; set; }
        public IFormFile NomineePhoto { get; set; }
        public IFormFile NomineeSignature { get; set; }
        public string BankName { get; set; }
        public string BankBranchName { get; set; }
        public string CoreBranchName { get; set; }
      public string RelationName { get; set; }

    }
}
