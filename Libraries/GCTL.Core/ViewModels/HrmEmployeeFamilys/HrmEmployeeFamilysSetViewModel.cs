using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmEmployeeFamilys
{
    public class HrmEmployeeFamilysSetViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string EmpFamilyId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string RelationshipId { get; set; }
        public string RelationShipName { get; set; }
        [Required]
        public string Name { get; set; }
        public string OccupationId { get; set; }
        public string OccupationName { get; set; }
        public string BloodGroupId { get; set; }
        public string BloodGroupName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string DateOfBirth { get; set; } 
        public string AddressDetails { get; set; }
        public string Phone { get; set; }
        [EmailAddress(ErrorMessage ="Invalid Email")]
       // [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        public string CompanyCode { get; set; }
        public string ComapanyName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
    }
}



