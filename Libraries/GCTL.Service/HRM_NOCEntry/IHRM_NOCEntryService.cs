using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRLettersReportViewModel;
using GCTL.Core.ViewModels.HRM_NOCEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRM_NOCEntry
{
    public interface IHRM_NOCEntryService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        // Existing
        Task<FullEmployeeDetailsGetByIdNocViewModel> GetByEmployeeCodeAsync(string employeeCode);

        // New NOC CRUD
        Task<string> GenerateNewNocIdAsync();
        Task<HRM_NOCEntrySetupViewModel> GetNocByAutoIdAsync(long autoId);
        Task<NocOperationResult> SaveNocAsync(HRM_NOCEntrySetupViewModel model, string companyCode);
        Task<NocOperationResult> UpdateNocAsync(HRM_NOCEntrySetupViewModel model);
        Task<NocOperationResult> DeleteNocAsync(List<decimal> autoIds, DeleteHistoryViewModel dModel);
        Task<List<NocListItemDto>> GetListAsync(string nocType);

    }
}
