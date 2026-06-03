using GCTL.Core.ViewModels.MonthlyTransportExpenseDetailsReport;

namespace GCTL.Service.MonthlyTransportExpenseDetailsReportService
{
    public interface IMonthlyTransportExpenseDetailsReportService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<MonthlyTransportExpenseDetailsReportFilterResultListDto> GetAllTransportExpenseStatementDropdownSelectReportAsync(MonthlyTransportExpenseDetailsReportFilterDataDto filter);

        Task<List<MonthlyTransportExpenseDetailsReportSetupDto>> GetAllTransportExpenseStatementResultReportAsync(MonthlyTransportExpenseDetailsReportFilterDataDto filter);

    }
}
