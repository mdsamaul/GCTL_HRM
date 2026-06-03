using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EmployeeContactInfos
{
    public class EmployeeContactInfosSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string EmployeeId { get; set; }
        public string EmpContactId { get; set; }
        public string EmployeeName { get; set; }

        public string ParmanentAddress { get; set; }
        public string ParmanentAddressBangla { get; set; }
        public string ParmanentPostOffice { get; set; }
        public string ParmanentThana { get; set; }
        public string ParmanentPostCode { get; set; }
        public string ParmanentDistrict { get; set; }
        [Display(Name = "Phone ")]
        [RegularExpression(@"^(?:(?:\+|00)88|01)?\d{11}$", ErrorMessage = "Invalid phone.")]
        [MaxLength(14)]
        public string ParmanentPhone { get; set; }
       

        public string PresentAddress { get; set; }
        public string PresentAddressBangla { get; set; }
        public string PresentPostOffice { get; set; }
        public string PresentThana { get; set; }
        public string PresentPostCode { get; set; }
        public string PresentDistrict { get; set; }
       
        [RegularExpression(@"^(?:(?:\+|00)88|01)?\d{11}$", ErrorMessage = "Invalid Phone")]
        [MaxLength(14)]
        public string PresentMobile { get; set; }
        [RegularExpression(@"^(?:(?:\+|00)88|01)?\d{11}$", ErrorMessage = "Invalid Mobile")]
        [MaxLength(14)]
        public string PresentPhone { get; set; }
        public string PresentFax { get; set; }
        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string PresentEmail { get; set; }

        public string EmContactName1 { get; set; }
        public string EmContactRelation1 { get; set; }
        public string EmContactAddress1 { get; set; }
        [RegularExpression(@"^(?:(?:\+|00)88|01)?\d{11}$", ErrorMessage = "Invalid Phone")]
        [MaxLength(14)]
        public string EmContactPhone1 { get; set; }
        [RegularExpression(@"^(?:(?:\+|00)88|01)?\d{11}$", ErrorMessage = "Invalid Mobile")]
        [MaxLength(14)]
        public string EmContactMobile1 { get; set; }
        public string EmContactFax1 { get; set; }
        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string EmContactEmail { get; set; }
        public string Relation {  get; set; }

        public string EmContactName2 { get; set; }
        public string EmContactRelation2 { get; set; }
        public string EmContactAddress2 { get; set; }
        [RegularExpression(@"^(?:(?:\+|00)88|01)?\d{11}$", ErrorMessage = "Invalid Phone")]
        [MaxLength(14)]
        public string EmContactPhone2 { get; set; }
        [RegularExpression(@"^(?:(?:\+|00)88|01)?\d{11}$", ErrorMessage = "Invalid Mobile")]
        [MaxLength(14)]
        public string EmContactMobile2 { get; set; }
        public string EmContactFax2 { get; set; }
        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string EmContactEmai2 { get; set; }

        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }

        public string CompanyCode { get; set; }
        
      //  public string UserInfoEmployeeId { get; set; }
        
        public string BranchCode { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string CoreBranchName { get; set; }
        public string DistrictName { get; set; }
    }
}
