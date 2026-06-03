using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.ManualEarnLeaveEntry;
using GCTL.Core.ViewModels.PFAssignEntry;

namespace GCTL.Service.PFAssignEntry
{
    public interface IPFAssignEntryService
    {
        Task<bool> PagePermissionAsync(string accessCode);

        Task<bool> SavePermissionAsync(string accessCode);

        Task<bool> UpdatePermissionAsync(string accessCode);

        Task<bool> DeletePermissionAsync(string accessCode);
        Task<PFAssignEntryFilterListDto> GetFilterDataAsync(PFAssignEntryFilterDto filter);
        Task<(bool isSuccess, string message, object data)> CreateUpdatePFAssignService(PFAssignEntrySetupViewModel fromData);
        Task<byte[]> GeneratePfAssignExcelDownload();
        Task<(bool isSuccess, string message, object data)> SavePFAssignExcel(PFAssignEntrySetupViewModel fromData);
        Task<List<PFAssignEntrySetupViewModel>> GetPfAssignDataService();
        Task<bool> BulkDeleteAsync(List<decimal> ids, DeleteHistoryViewModel dmodel);
        Task<PFAssignEntrySetupViewModel> getAssignValueById(string id);
    }
}

