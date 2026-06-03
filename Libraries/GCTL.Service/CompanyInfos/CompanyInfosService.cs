using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.CompanyInfos;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.CompanyInfos
{
    public class CompanyInfosService : AppService<HrmDefCompanyInfo>, ICompanyInfosService
    {
        private readonly IRepository<HrmDefCompanyInfo> companyRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;
        private const string TableName = "HRM_Def_CompanyInfo";
        private const string ColumnName = "CompanyNameID";

        public CompanyInfosService(IRepository<HrmDefCompanyInfo> companyRepository, IRepository<CoreAccessCode> accessCodeRepository, ICommonService commonService) : base(companyRepository)
        {
            this.companyRepository = companyRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        public async Task<List<CompanyInfosSetupViewModel>> GetAllAsync()
        {
            var entity = await companyRepository.GetAllAsync();
            return entity.Select(entityVM => new CompanyInfosSetupViewModel
            {
                AutoId = entityVM.AutoId,
                CompanyNameId = entityVM.CompanyNameId,
                CompanyName = entityVM.CompanyName,
                ShortName = entityVM.ShortName,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,



            }).ToList();
        }

        public async Task<CompanyInfosSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await companyRepository.GetByIdAsync(code);
            if (entity == null) return null;

            CompanyInfosSetupViewModel entityVM = new CompanyInfosSetupViewModel();
            entityVM.AutoId = entity.AutoId;
            entityVM.CompanyNameId = entity.CompanyNameId;
            entityVM.CompanyName = entity.CompanyName;
            entityVM.ShortName = entity.ShortName;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        public async Task<IEnumerable<CommonSelectModel>> SelectionCompanyTypeAsync()
        {

            var data = await companyRepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.CompanyNameId,
                           Name = x.CompanyName
                       }).ToListAsync();
            return data;
        }


        public async Task<bool> SaveAsync(CompanyInfosSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 2);
            await companyRepository.BeginTransactionAsync();
            try
            {

                HrmDefCompanyInfo entity = new HrmDefCompanyInfo();
                entity.CompanyNameId= strMaxNO;
                entity.CompanyName = entityVM.CompanyName;
                entity.ShortName = entityVM.ShortName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await companyRepository.AddAsync(entity);
                await companyRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await companyRepository.RollbackTransactionAsync();

                return false;
            }
        }

        public async Task<bool> UpdateAsync(CompanyInfosSetupViewModel entityVM)
        {
            await companyRepository.BeginTransactionAsync();
            try
            {

                var entity = await companyRepository.GetByIdAsync(entityVM.CompanyNameId);
                if (entity == null)
                {
                    await companyRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.CompanyNameId = entityVM.CompanyNameId;
                entity.CompanyName = entityVM.CompanyName;
                entity.ShortName = entityVM.ShortName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await companyRepository.UpdateAsync(entity);
                await companyRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await companyRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await companyRepository.All().Where(x => ids.Contains(x.CompanyNameId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            companyRepository.Delete(entity);

            return true;
        }


        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await companyRepository.All().AnyAsync(x => x.CompanyNameId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await companyRepository.All().AnyAsync(x => x.CompanyName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await companyRepository.All().AnyAsync(x => x.CompanyName == name && x.CompanyNameId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Company Info" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Company Info" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Company Info" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Company Info" && x.CheckDelete);
        }
        #endregion
    }
}
