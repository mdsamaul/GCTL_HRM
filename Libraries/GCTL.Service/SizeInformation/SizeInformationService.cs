using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.SizeInformation;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.SizeInformation
{
    public class SizeInformationService : AppService<RmgProdDefSize>, ISizeInformationService
    {
        #region Service & Repository
        private readonly IRepository<RmgProdDefSize> sizeInformationrepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;

        private const string TableName = "RMG_Prod_Def_Size";
        private const string ColumnName = "SizeId";

        public SizeInformationService(
            IRepository<RmgProdDefSize> sizeInformationrepository,
            IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService

            )
    : base(sizeInformationrepository)
        {
            this.sizeInformationrepository = sizeInformationrepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        #endregion

        #region GetAllAsync

        public async Task<List<SizeInformationSetupViewModel>> GetAllAsync()
        {
            var entity = await sizeInformationrepository.GetAllAsync();
            return entity.Select(entityVM => new SizeInformationSetupViewModel
            {
                Tc = entityVM.Tc,
                SizeId = entityVM.SizeId,
                Size = entityVM.Size,
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

        public async Task<SizeInformationSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await sizeInformationrepository.GetByIdAsync(code);
            if (entity == null) return null;

            SizeInformationSetupViewModel entityVM = new SizeInformationSetupViewModel();
            entityVM.Tc = entity.Tc;
            entityVM.SizeId = entity.SizeId;
            entityVM.Size = entity.Size;
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

        public async Task<bool> SaveAsync(SizeInformationSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            await sizeInformationrepository.BeginTransactionAsync();
            try
            {
                RmgProdDefSize entity = new RmgProdDefSize();
                entity.SizeId = strMaxNO;
                entity.Size = entityVM.Size;
                entity.Detail = entityVM.Detail ?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await sizeInformationrepository.AddAsync(entity);
                await sizeInformationrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await sizeInformationrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(SizeInformationSetupViewModel entityVM)
        {
            await sizeInformationrepository.BeginTransactionAsync();
            try
            {
                var entity = await sizeInformationrepository.GetByIdAsync(entityVM.SizeId);
                if (entity == null)
                {
                    await sizeInformationrepository.RollbackTransactionAsync();
                    return false;
                }
                entity.SizeId = entityVM.SizeId;
                entity.Size = entityVM.Size;
                entity.Detail = entityVM.Detail ?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await sizeInformationrepository.UpdateAsync(entity);
                await sizeInformationrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await sizeInformationrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region SelectionAsync
        public async Task<IEnumerable<CommonSelectModel>> SelectionSizeInformationAsync()
        {

            var data = await sizeInformationrepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.SizeId,
                           Name = x.Size,
                       }).ToListAsync();
            return data;
        }

        #endregion

        #region DeleteTab
        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await sizeInformationrepository.All().Where(x => ids.Contains(x.SizeId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            sizeInformationrepository.Delete(entity);

            return true;
        }
        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await sizeInformationrepository.All().AnyAsync(x => x.SizeId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await sizeInformationrepository.All().AnyAsync(x => x.Size == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await sizeInformationrepository.All().AnyAsync(x => x.Size == name && x.SizeId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Size Information" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Size Information" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Size Information" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Size Information" && x.CheckDelete);
        }
        #endregion
    }
}
