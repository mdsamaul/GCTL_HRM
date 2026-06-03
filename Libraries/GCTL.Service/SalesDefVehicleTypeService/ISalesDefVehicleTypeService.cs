using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.SalesDefVehicleType;

namespace GCTL.Service.SalesDefVehicleTypeService
{
    public interface ISalesDefVehicleTypeService
    {
        Task<List<SalesDefVehicleTypeSetupViewModel>> GetAllAsync();
        Task<SalesDefVehicleTypeSetupViewModel> GetByIdAsync(string id);
        Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(SalesDefVehicleTypeSetupViewModel model, string companyCode, string employeeId);
        Task<(bool isSuccess, string message, object data)> DeleteAsync(List<int> ids);
        Task<string> AutoVehicleTypeIdAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string VehicleTypeValue);
    }
}
