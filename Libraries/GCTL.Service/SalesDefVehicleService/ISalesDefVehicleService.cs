using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.SalesDefVehicle;
using GCTL.Core.ViewModels.SalesDefVehicleType;

namespace GCTL.Service.SalesDefVehicleService
{
    public interface ISalesDefVehicleService
    {
        Task<List<SalesDefVehicleSetupViewModel>> GetAllAsync();
        Task<SalesDefVehicleSetupViewModel> GetByIdAsync(string id);
        Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(SalesDefVehicleSetupViewModel model, string companyCode, string employeeId);
        Task<(bool isSuccess, string message, object data)> DeleteAsync(List<int> ids);
        Task<string> AutoTransportInfoIdAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string VehicleTypeValue);
    }
}
