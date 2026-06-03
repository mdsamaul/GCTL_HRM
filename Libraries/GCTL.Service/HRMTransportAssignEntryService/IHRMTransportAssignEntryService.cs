using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRMTransportAssignEntry;
using GCTL.Core.ViewModels.SalesDefVehicleType;

namespace GCTL.Service.HRMTransportAssignEntryService
{
    public interface IHRMTransportAssignEntryService
    {
        Task<List<HRMTransportAssignEntrySetupViewModel>> GetAllAsync();
        Task<HRMTransportAssignEntrySetupViewModel> GetByIdAsync(string id);
        Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(HRMTransportAssignEntrySetupViewModel model, string companyCode, string employeeId);
        Task<(bool isSuccess, string message, object data)> DeleteAsync(List<decimal> ids, DeleteHistoryViewModel dmodel);
        Task<string> AutoIdAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string VehicleTypeValue);
        Task<HRMTransportDetails> GetEmpDetailsIdAsync(string emp);
    }
}
