using GCTL.Core.ViewModels.TransportExpenseStatementReport;

namespace GCTL.Service.TransportExpenseStatementReportService
{
    public interface ITransportExpenseStatementReportServices
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<TransportExpenseStatementReportFilterResultListDto> GetAllTransportExpenseStatementDropdownSelectReportAsync(TransportExpenseStatementReportFilterDataDto filter);
        Task<List<TransportExpenseStatementReportSetupViewModel>> GetAllTransportExpenseStatementResultReportAsync(TransportExpenseStatementReportFilterDataDto filter); Task<List<TransportExpenseStatementReportSetupViewModel>> GetAllTransportExpenseStatementResultReportExcelAsync(TransportExpenseStatementReportFilterDataDto filter);
    }
}
