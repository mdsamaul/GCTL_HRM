using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.CourseTitle;
using GCTL.Core.ViewModels.DeleteHistories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.CourseTitle
{
    public interface ICourseTitleService
    {
        Task<List<CourseTitleSetupViewModel>> GetAllAsync();
        Task<CourseTitleSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(CourseTitleSetupViewModel entityVM, string CompanyCode);
        Task<bool> UpdateAsync(CourseTitleSetupViewModel entityVM);
        Task<(bool succses, string messege)> DeleteTab(List<string> ids, DeleteHistoryViewModel model);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<IEnumerable<CommonSelectModel>> SelectionCourseTitleAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}
