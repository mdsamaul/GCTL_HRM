using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRMTransportAssignEntry;
using GCTL.Core.ViewModels.HRMTransportExpenseEntry;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRMTransportExpenseEntryService
{
    public interface IHRMTransportExpenseEntryServicec
    {
        Task<List<TransportMasterDto>> GetAllAsync();
        Task<HRMTransportExpenseEntrySetupViewModel> GetByIdAsync(string id);
        Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(HRMTransportExpenseEntrySetupViewModel model, string companyCode, string employeeId);
        Task<(bool isSuccess, string message, object data)> DeleteAsync(List<decimal> ids, DeleteHistoryViewModel dmodel);
        Task<string> AutoIdAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string VehicleTypeValue);
        Task<(bool isSuccess, string message, object data)> TransportExpenseDetailsAsync(TransportExpenseDetailsTempDto model);
        Task<HRMTransportDetails> GetEmpDetailsIdAsync(string emp);
        Task<List<TransportExpenseDetailsTempDto>> TransportExpenseTempDetailsListAsny();
        Task<TransportExpenseDetailsTempDto> TransportExpenseTempDetailsByIdAsny(decimal id);
        Task<HRMTransportExpenseEntrySetupViewModel> TransportExpenseMasterDetailsByIdAsny(decimal id);
        Task<(bool isSuccess, string message, object data)> DeleteTransportExpenseAsync(decimal id);
        Task<List<TransportDetailsDto>> GetAllTransportDetailsAsync(string trnsId);
        Task<(bool isSuccess, object data)> ReloadDataBackTempToDetailsAsync();
    }
}
