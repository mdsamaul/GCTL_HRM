using GCTL.Core.ViewModels.Companies;
using GCTL.Data.Models;
using GCTL.Core.ViewModels.Common;

namespace GCTL.Service.Companies
{
    public interface ICompanyService
    {
        List<CoreCompany> GetCompanies();
        CoreCompany GetCompany(string code);
        CompanyModel GetCompanyByCode(string code);
        bool DeleteCompany(string id);    
        CoreCompany SaveCompany(CoreCompany entity);
        bool IsCompanyExistByCode(string code);
        bool IsCompanyExist(string name);
        bool IsCompanyExist(string name, string companyCode);
        bool SavePermission(string accessCode);
        bool UpdatePermission(string accessCode);
        bool DeletePermission(string accessCode);
      Task <IEnumerable<CommonSelectModel>> GetCompanyDropDown();

    }
}