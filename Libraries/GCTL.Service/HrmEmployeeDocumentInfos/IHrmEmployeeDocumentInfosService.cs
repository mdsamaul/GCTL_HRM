using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmEmployeeAdditionalInfos;
using GCTL.Core.ViewModels.HrmEmployeeDocumentInfos;
using GCTL.Core.ViewModels.HrmEmployeeFamilys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmEmployeeDocumentInfos
{
    public interface IHrmEmployeeDocumentInfosService
    {
        Task<List<HrmEmployeeDocumentInfosSetup>> GetAllAsync(string employeeId);
        Task<HrmEmployeeDocumentInfosSetup> GetByIdAsync(string code);
        //
        Task<List<HrmEmployeeDocumentInfosSetup>> GetEmployeeByCompanyCode(string companyCode);
        Task<List<HrmEmployeeDocumentInfosSetup>> GetComapnyByBranchCode(string companyCode);
        Task<HrmEmployeeDocumentInfosSetup> GetEmployeeNameDesDeptByCode(string employeeId);

        //
        Task<bool> SaveAsync(HrmEmployeeDocumentInfosSetup entityVM, string CompanyCode);
        Task<bool> UpdateAsync(HrmEmployeeDocumentInfosSetup entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string employeeCode, string typeCode, string name);
        IEnumerable<CommonSelectModel> SelectionHrmDefEmpDocumentTypeAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}