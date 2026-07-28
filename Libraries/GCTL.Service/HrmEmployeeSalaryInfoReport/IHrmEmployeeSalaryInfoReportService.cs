using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.EachGcFilterRequest;
using GCTL.Core.ViewModels.HrmEmployeeSalaryInfoEntry;

namespace GCTL.Service.HrmEmployeeSalaryInfoReport
{
    public interface IHrmEmployeeSalaryInfoReportServices
    {
        Task<ReportFilterListViewModel> GetFilterDataAsync(ReportFilterViewModel filter);
        Task<List<ReportFilterResultViewModel>> GetDataAsync(ReportFilterViewModel filter);
        Task<byte[]> GeneratePdfReport(List<ReportFilterResultViewModel> data, BaseViewModel model);
        Task<byte[]> GenerateExcelReport(List<ReportFilterResultViewModel> data);

        //Task<PagedResultDto<GcItemDto>> GetEmployeesAsync(ReportFilterViewModel req);
        //Task<PagedResultDto<GcItemDto>> GetEmployeesAsync(EmployeeFilterViewModel req);
        //Task<ReportDropdownPageResult> GetDropdownPageAsync2(ReportDropdownPageRequest request);
        Task<bool> PagePermissionAsync(string accessCode);
    }
}
