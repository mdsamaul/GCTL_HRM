using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.FebricTest;
using GCTL.Core.ViewModels.SizeInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.FebricTest
{
    public interface IFebricTestService
    {
        Task<List<FebricTestSetupViewModel>> GetAllAsync();
        Task<FebricTestSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(FebricTestSetupViewModel entityVM);
        Task<bool> UpdateAsync(FebricTestSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionFebricTestAsync();

        Task<bool> DeleteTab(List<string> ids);

        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);


    }
}
