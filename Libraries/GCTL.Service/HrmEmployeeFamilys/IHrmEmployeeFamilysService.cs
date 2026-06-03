using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmEmployeeAdditionalInfos;
using GCTL.Core.ViewModels.HrmEmployeeEducations;
using GCTL.Core.ViewModels.HrmEmployeeFamilys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmEmployeeFamilys
{
    public interface IHrmEmployeeFamilysService
    {
        Task<List<HrmEmployeeFamilysSetViewModel>> GetAllAsync(string employeeId);
        Task<HrmEmployeeFamilysSetViewModel> GetByIdAsync(string code);

        Task<List<HrmEmployeeFamilysSetViewModel>> GetEmployeeByCompanyCode(string companyCode);

        Task<HrmEmployeeFamilysSetViewModel> GetEmployeeNameDesDeptByCode(string employeeId);
        //
        Task<bool> SaveAsync(HrmEmployeeFamilysSetViewModel entityVM, string CompanyCode);
        Task<bool> UpdateAsync(HrmEmployeeFamilysSetViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string employeeCode, string typeCode, string name);
        IEnumerable<CommonSelectModel> SelectionHrmDefEmpFamilyTypeAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}