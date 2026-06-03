using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.PFAssignEntry;
using GCTL.Core.ViewModels.RosterScheduleEntry;

namespace GCTL.Service.RosterScheduleEntry
{
    public interface IRosterScheduleEntryService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<RosterScheduleEntryFilterListDto> GetFilterDataAsync(RosterScheduleEntryFilterDto filter);
        Task<MasterBaseFilterResultDto> GetMasterBaseFiltersAsync(MasterBaseFilterDto filter);
        Task<List<PAYMonthResultDto>> getAllMonthService();
        Task<List<RosterShiftDto>> getAlllShiftService();
        Task<(bool isSuccess, string isMessage , object data )> CreateAndUpdateService(RosterScheduleEntrySetupViewModel FromModel);
        Task<List<RosterScheduleEntrySetupViewModel>> GetRosterScheduleGridService();
        Task<RosterScheduleEntrySetupViewModel> EditGetServices(string id);
        //Task<bool> BulkDeleteAsync(RosterScheduleEntrySetupViewModel FromModel,DeleteHistoryViewModel model);
        Task<(bool isSuccess, bool hasDependency, string message)> BulkDeleteAsync( RosterScheduleEntrySetupViewModel model, DeleteHistoryViewModel DModel);
        Task<byte[]> GenerateEmpRosterExcelDownload();
        Task<(bool isSuccess, string message, object data)> SaveRosterExcel(RosterScheduleEntrySetupViewModel model);
    }
}
