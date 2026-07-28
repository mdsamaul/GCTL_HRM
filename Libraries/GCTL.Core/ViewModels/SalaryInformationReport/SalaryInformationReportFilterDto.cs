using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SalaryInformationReport
{
    public class SalaryInformationReportFilterDto
    {
        public string CompanyCode { get; set; }       
        public string BranchCode { get; set; }        
        public string DepartmentCode { get; set; }      
        public string EmployeeID { get; set; }          
        public string ModeOfPayment { get; set; }      
        public string EmploymentNature { get; set; }    

        public string GenerateType { get; set; }         
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string MonthName { get; set; }
        public int? YearName { get; set; }

        public DateTime? AsOnDate { get; set; }

        // Report options
        public string ExportFormat { get; set; } = "Excel";   
        public string MasterFileType { get; set; }
    }
}
