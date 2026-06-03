using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration;
using GCTL.Core.ViewModels.ManualEarnLeaveEntry;

namespace GCTL.Service.ManualEarnLeaveEntry
{
    public interface IManualEarnLeaveEntryService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<ManualEarnLeaveEntryEmployeeFilterListDto> GetFilterDataAsync(ManualEarnLeaveEntryEmployeeFilterDto filter);
        Task<(bool isSuccess, string message, object data)>SaveUpdateEarnLeaveServices(ManualEarnLeaveEntryEmployeeCreateDto FromData);
        Task<List<ManualEarnLeaveEntryEmployeeCreateDto>> GetEarnLeaveEmployeeService();
        Task<byte[]> GenerateEmpEarnLeaveExcelDownload();
        Task<(bool isSuccess, string message, object data)> SaveEarnLeaveExcel(ManualEarnLeaveEntryEmployeeCreateDto model);
        Task<bool> BulkDeleteAsync(List<decimal> ids, DeleteHistoryViewModel dmodel);
        Task<ManualEarnLeaveEntryEmployeeCreateDto> getEarnLeaveEmployeeById(string id);
    }
}
