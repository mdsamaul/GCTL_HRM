using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.TaxChallanEntry
{
    public class EmployeeTaxChallanResultViewModel : BaseViewModel
    {
       
            public string? CompanyCode { get; set; }
            public string? EmployeeID { get; set; }
            public string? CompanyName { get; set; }
            public string? FullName { get; set; }
            public DateTime? JoiningDate { get; set; }
            public string? DesignationCode { get; set; }
            public string? DepartmentCode { get; set; }
            public string? DesignationName { get; set; }
            public string? BranchCode { get; set; }
            public string? BranchName { get; set; }
            public string? DepartmentName { get; set; }
            public decimal? GrossSalary { get; set; }
    }
   
  
}
