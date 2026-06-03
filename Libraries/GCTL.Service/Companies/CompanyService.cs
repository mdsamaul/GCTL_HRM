using GCTL.Core.Data;
using GCTL.Core.ViewModels.Companies;
using GCTL.Data.Models;
using GCTL.Core.ViewModels.Common;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.Companies
{
    public class CompanyService : AppService<CoreCompany>, ICompanyService
    {
        private readonly IRepository<CoreCompany> companyRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        public CompanyService(IRepository<CoreCompany> companyRepository,
            IRepository<CoreAccessCode> accessCodeRepository)
            : base(companyRepository)
        {
            this.companyRepository = companyRepository;
            this.accessCodeRepository = accessCodeRepository;
        }

        public List<CoreCompany> GetCompanies()
        {
            return GetAll();
        }

        public CoreCompany GetCompany(string id)
        {
            return companyRepository.GetById(id);
        }

        public CompanyModel GetCompanyByCode(string code)
        {
            return companyRepository.All().Where(x => x.CompanyCode == code)
                    .Select(company => new CompanyModel
                    {
                        CompanyName = company.CompanyName,
                        Address1 = company.Address1,
                        Email = company.Email,
                        Phone1 = company.Phone1,
                        HotLine = company.HotLine,
                        Url = company.Url,
                    }).FirstOrDefault();
        }

        public CoreCompany SaveCompany(CoreCompany entity)
        {
            if (IsCompanyExistByCode(entity.CompanyCode))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteCompany(string id)
        {
            var entity = GetCompany(id);
            if (entity != null)
            {
                companyRepository.Delete(entity);
                return true;
            }
            return false;
        }
        public async Task<IEnumerable<CommonSelectModel>> GetCompanyDropDown()
        {
            return  await companyRepository.All()
                 .Select(x => new CommonSelectModel
                 {
                     Code = x.CompanyCode,
                     Name = x.CompanyName
                 }).ToListAsync();
        }
        public bool IsCompanyExistByCode(string code)
        {
            return companyRepository.All().Any(x => x.CompanyCode == code);
        }

        public bool IsCompanyExist(string name)
        {
            return companyRepository.All().Any(x => x.CompanyName == name);
        }

        public bool IsCompanyExist(string name, string companyCode)
        {
            return companyRepository.All().Any(x => x.CompanyName == name && x.CompanyCode != companyCode);
        }
        public bool SavePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Company" && x.CheckAdd);
        }
        public bool UpdatePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Company" && x.CheckEdit);
        }
        public bool DeletePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Company" && x.CheckDelete);
        }
    }
}
