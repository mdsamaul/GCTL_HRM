using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.FebricTest;
using GCTL.Core.ViewModels.GarmentsTest;
using GCTL.Core.ViewModels.SizeInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.GarmentsTest
{
    public interface IGarmentsTestService
    {
        Task<List<GarmentsTestSetupViewModel>> GetAllAsync();
        Task<GarmentsTestSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(GarmentsTestSetupViewModel entityVM);
        Task<bool> UpdateAsync(GarmentsTestSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionGarmentsTestAsync();

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
