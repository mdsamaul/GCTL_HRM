using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefInstitutes;
using GCTL.Core.ViewModels.HrmDefOccupations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefOccupations
{
    public interface IHrmDefOccupationsService
    {
        Task<List<HrmDefOccupationsSetupViewModel>> GetAllAsync();
        Task<HrmDefOccupationsSetupViewModel> GetByIdAsync(string code);
        Task<bool>SaveAsync(HrmDefOccupationsSetupViewModel entityVM);
        Task<bool>UpdateAsync(HrmDefOccupationsSetupViewModel entityVM);
        Task<bool> DeleteAsyncTab(List<string> ids);
        Task<bool> IsexistByCodeAsnyc(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string code);
        Task<IEnumerable<CommonSelectModel>> SelectOccupationAsnyc();
        Task<bool>PagePermissionAsync(string accessCode);
        Task<bool> SavePaermissonAsync(string accessCode);
        Task<bool> UpdatePermisson(string accessCode);
        Task<bool> DeletePermissonAsync(string accesCode);
     
       
    }
}
