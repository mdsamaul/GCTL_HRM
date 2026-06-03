using GCTL.Core.Data;
using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.SeasonInformation;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.SeasonInformation
{
    public class SeasonInformationService : AppService<RmgProdDefSeason>, ISeasonInformationService
    {
        #region Service & Repository
        private readonly IRepository<RmgProdDefSeason> seasonInformationrepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;

        private const string TableName = "RMG_Prod_Def_Season";
        private const string ColumnName = "SeasonId";
        public SeasonInformationService(
            IRepository<RmgProdDefSeason> seasonInformationrepository,
            IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService

            ) 
            
    : base(seasonInformationrepository)
        {
            this.seasonInformationrepository = seasonInformationrepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        #endregion

        #region GetAllAsync

        public async Task<List<SeasonInformationSetupViewModel>> GetAllAsync()
        {
            var entity = await seasonInformationrepository.GetAllAsync();
            return entity.Select(entityVM => new SeasonInformationSetupViewModel
            {
                Tc = entityVM.Tc,
                SeasonId = entityVM.SeasonId,
                Season = entityVM.Season,
                Detail = entityVM.Detail,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,

            }).ToList();
        }

        #endregion

        #region GetByIdAsync

        public async Task<SeasonInformationSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await seasonInformationrepository.GetByIdAsync(code);
            if (entity == null) return null;

            SeasonInformationSetupViewModel entityVM = new SeasonInformationSetupViewModel();
            entityVM.Tc = entity.Tc;
            entityVM.SeasonId = entity.SeasonId;
            entityVM.Season = entity.Season;
            entityVM.Detail = entity.Detail;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(SeasonInformationSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            await seasonInformationrepository.BeginTransactionAsync();
            try
            {
                RmgProdDefSeason entity = new RmgProdDefSeason();
                entity.SeasonId = strMaxNO;
                entity.Season = entityVM.Season;
                entity.Detail = entityVM.Detail ?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await seasonInformationrepository.AddAsync(entity);
                await seasonInformationrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await seasonInformationrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(SeasonInformationSetupViewModel entityVM)
        {
            await seasonInformationrepository.BeginTransactionAsync();
            try
            {
                var entity = await seasonInformationrepository.GetByIdAsync(entityVM.SeasonId);
                if (entity == null)
                {
                    await seasonInformationrepository.RollbackTransactionAsync();
                    return false;
                }
                entity.SeasonId = entityVM.SeasonId;
                entity.Season = entityVM.Season;
                entity.Detail = entityVM.Detail ?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await seasonInformationrepository.UpdateAsync(entity);
                await seasonInformationrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await seasonInformationrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region SelectionAsync
        public async Task<IEnumerable<CommonSelectModel>> SelectionSeasonInformationAsync()
        {

            var data = await seasonInformationrepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.SeasonId,
                           Name = x.Season,
                       }).ToListAsync();
            return data;
        }

        #endregion

        #region DeleteTab
        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await seasonInformationrepository.All().Where(x => ids.Contains(x.SeasonId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            seasonInformationrepository.Delete(entity);

            return true;
        }
        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await seasonInformationrepository.All().AnyAsync(x => x.SeasonId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await seasonInformationrepository.All().AnyAsync(x => x.Season == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await seasonInformationrepository.All().AnyAsync(x => x.Season == name && x.SeasonId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Season Information" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Season Information" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Season Information" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Season Information" && x.CheckDelete);
        }
        #endregion
    }
}
