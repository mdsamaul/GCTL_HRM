using GCTL.Core.ViewModels.AdvanceLoanAdjustmentReport;

namespace GCTL.Service.AdvanceLoanAdjustmentReport
{
    public interface IAdvanceLoanAdjustmentReportServices
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<List<AdvanceLoanAdjustmentReportSetupViewModel>> GetAdvancePayReportAsync(HrmAdvancePayReportFilter filter);
        Task<List<DepartmentGroupedData>> GetAdvancePayReportGroupedAsync(HrmAdvancePayReportFilter filter);
        Task<AdvanceLoanFilterData> GetAdvancePayFiltersAsync(HrmAdvancePayReportFilter filter);

    }
}
