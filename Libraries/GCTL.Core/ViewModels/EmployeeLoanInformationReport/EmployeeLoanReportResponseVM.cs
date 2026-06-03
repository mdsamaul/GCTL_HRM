using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EmployeeLoanInformationReport
{
    public class EmployeeLoanReportResponseVM
    {
        public List<EmployeeLoanInformationReportVM> LoanReports { get; set; }
        public List<CompanyBasicInfoVM> Companies { get; set; }
        public List<EmployeeBasicInfoVM> Employees { get; set; }
        public List<LoanBasicInfoVm> LoanIDs { get; set; }
        public List<LoanTypeInfoVm> LoanTypes { get; set; }
    }

    public class CompanyBasicInfoVM
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
    }

    public class LoanBasicInfoVm
    {
        public string LoanIDs { get; set; }
        public string LoanDate { get; set; }
        public string LoanType { get; set; }
        public decimal LoanAmount { get; set; }
        public string InstStartEndDate { get; set; }
        public string NoOfInstallment { get; set; }
    }
    public class LoanTypeInfoVm
    {
        public string LoanTypeId { get; set; }
        public string LoanType { get; set; }
    }

}
