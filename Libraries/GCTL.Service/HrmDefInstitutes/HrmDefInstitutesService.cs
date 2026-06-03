using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HRMDefExamTitles;
using GCTL.Core.ViewModels.HrmDefInstitutes;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefInstitutes
{
    public class HrmDefInstitutesService:AppService<HrmDefInstitute>, IHrmDefInstitutesService
    {
        private IRepository<HrmDefInstitute> hrmInstituteReposoitory;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;
        private const string TableName = "HRM_Def_Institute";
        private const string ColumnName = "InstituteCode";

        public HrmDefInstitutesService(IRepository<HrmDefInstitute> hrmInstituteReposoitory, IRepository<CoreAccessCode> accessCodeRepository, ICommonService commonService):base(hrmInstituteReposoitory)
        {
            this.hrmInstituteReposoitory = hrmInstituteReposoitory;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        public async Task<List<HrmDefInstitutesSetupViewModel>> GetAllAsync()
        {
            var entity = await hrmInstituteReposoitory.GetAllAsync();
            return entity.Select(entityVM => new HrmDefInstitutesSetupViewModel
            {
                AutoId = entityVM.AutoId,
                InstituteCode = entityVM.InstituteCode,
                InstituteName = entityVM.InstituteName,
                ShortName = entityVM.ShortName,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,



            }).ToList();
        }

        public async Task<HrmDefInstitutesSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await hrmInstituteReposoitory.GetByIdAsync(code);
            if (entity == null) return null;

            HrmDefInstitutesSetupViewModel entityVM = new HrmDefInstitutesSetupViewModel();
            entityVM.AutoId = entity.AutoId;
            entityVM.InstituteCode = entity.InstituteCode;
            entityVM.InstituteName = entity.InstituteName;
            entityVM.ShortName = entity.ShortName;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        public async Task<IEnumerable<CommonSelectModel>> SelectionHrmDefInstituteTypeAsync()
        {

            var data =await hrmInstituteReposoitory.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.InstituteCode,
                           Name = x.InstituteName
                       }).ToListAsync();
            return data;
        }


        public async Task<bool> SaveAsync(HrmDefInstitutesSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 2);
            await hrmInstituteReposoitory.BeginTransactionAsync();
            try
            {

                HrmDefInstitute entity = new HrmDefInstitute();
                entity.InstituteCode = strMaxNO;
                entity.InstituteName = entityVM.InstituteName;
                entity.ShortName = entityVM.ShortName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await hrmInstituteReposoitory.AddAsync(entity);
                await hrmInstituteReposoitory.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await hrmInstituteReposoitory.RollbackTransactionAsync();

                return false;
            }
        }

        public async Task<bool> UpdateAsync(HrmDefInstitutesSetupViewModel entityVM)
        {
            await hrmInstituteReposoitory.BeginTransactionAsync();
            try
            {

                var entity = await hrmInstituteReposoitory.GetByIdAsync(entityVM.InstituteCode);
                if (entity == null)
                {
                    await hrmInstituteReposoitory.RollbackTransactionAsync();
                    return false;
                }
                entity.InstituteCode = entityVM.InstituteCode;
                entity.InstituteName = entityVM.InstituteName;
                entity.ShortName = entityVM.ShortName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await hrmInstituteReposoitory.UpdateAsync(entity);
                await hrmInstituteReposoitory.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await hrmInstituteReposoitory.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> DeleteTab(List<string> ids)
        {
            await hrmInstituteReposoitory.BeginTransactionAsync(); 
            try
            {
                var entities = await hrmInstituteReposoitory .All() .Where(x => ids.Contains(x.InstituteCode)).ToListAsync();

                if (!entities.Any())
                {
                    return false;
                }

                hrmInstituteReposoitory.Delete(entities); 
                await hrmInstituteReposoitory.CommitTransactionAsync(); 
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during delete: {ex.Message}");
                await hrmInstituteReposoitory.RollbackTransactionAsync(); 
                return false;
            }
        }


       


        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await hrmInstituteReposoitory.All().AnyAsync(x => x.InstituteCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hrmInstituteReposoitory.All().AnyAsync(x => x.InstituteName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await hrmInstituteReposoitory.All().AnyAsync(x => x.InstituteName == name && x.InstituteCode != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Institute" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Institute" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Institute" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Institute" && x.CheckDelete);
        }
        #endregion
    }
}

