using GCTL.Core.ViewModels.EmployeeLoanInformationReport;

namespace GCTL.Service.EmployeeLoanInformationReport
{
    public interface IEmployeeLoanInformationReportServices
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<EmployeeLoanReportResponseVM> GetLoanDetailsAsync(LoanFilterVM filter);
    }
}
