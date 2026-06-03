using GCTL.Core.ViewModels.EditUserVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.UserEditEntry
{
    public interface IUserEditService
    {
        Task<(bool isSuccesss, string message, object data)> SaveAsync(EditUserSetupViewModel model);
        Task<(List<EditUserGridViewModel> Data, int totalRecord, int curentRecord)> GetPaginatedDataAsync(string searchValue, int page, int pageSize, string sortColumn, string sortDirection, string userName);
        Task<EditUserSetupViewModel> GetByIdAsync(int id);
        Task<EditUserSetupViewModel> GetByIdAsync(string userName);


        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
    }
}
