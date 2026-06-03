using GCTL.Core.Data;
using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.PackageType;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.PackageType
{
    public class PackageTypeService : AppService<InvDefPackageType>, IPackageTypeService
    {
        #region Service & Repository
        private readonly IRepository<InvDefPackageType> packageTyperepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;

        private const string TableName = "Inv_Def_PackageType";
        private const string ColumnName = "PackageTypeID";
        public PackageTypeService(
            IRepository<InvDefPackageType> packageTyperepository,
            IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService

            ) 
            
    : base(packageTyperepository)
        {
            this.packageTyperepository = packageTyperepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        #endregion

        #region GetAllAsync

        public async Task<List<PackageTypeSetupViewModel>> GetAllAsync()
        {
            var entity = await packageTyperepository.GetAllAsync();
            return entity.Select(entityVM => new PackageTypeSetupViewModel
            {
                Tc = entityVM.Tc,
                PackageTypeId = entityVM.PackageTypeId,
                PackageType = entityVM.PackageType,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,

            }).ToList();
        }

        #endregion

        #region GetByIdAsync

        public async Task<PackageTypeSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await packageTyperepository.GetByIdAsync(code);
            if (entity == null) return null;

            PackageTypeSetupViewModel entityVM = new PackageTypeSetupViewModel();
            entityVM.Tc = entity.Tc;
            entityVM.PackageTypeId = entity.PackageTypeId;
            entityVM.PackageType = entity.PackageType;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(PackageTypeSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            await packageTyperepository.BeginTransactionAsync();
            try
            {
                InvDefPackageType entity = new InvDefPackageType();
                entity.PackageTypeId = strMaxNO;
                entity.PackageType = entityVM.PackageType;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await packageTyperepository.AddAsync(entity);
                await packageTyperepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await packageTyperepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(PackageTypeSetupViewModel entityVM)
        {
            await packageTyperepository.BeginTransactionAsync();
            try
            {
                var entity = await packageTyperepository.GetByIdAsync(entityVM.PackageTypeId);
                if (entity == null)
                {
                    await packageTyperepository.RollbackTransactionAsync();
                    return false;
                }
                entity.PackageTypeId = entityVM.PackageTypeId;
                entity.PackageType = entityVM.PackageType;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await packageTyperepository.UpdateAsync(entity);
                await packageTyperepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await packageTyperepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region SelectionAsync
        public async Task<IEnumerable<CommonSelectModel>> SelectionPackageTypeAsync()
        {

            var data = await packageTyperepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.PackageTypeId,
                           Name = x.PackageType,
                       }).ToListAsync();
            return data;
        }

        #endregion

        #region DeleteTab
        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await packageTyperepository.All().Where(x => ids.Contains(x.PackageTypeId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            packageTyperepository.Delete(entity);

            return true;
        }
        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await packageTyperepository.All().AnyAsync(x => x.PackageTypeId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await packageTyperepository.All().AnyAsync(x => x.PackageType == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await packageTyperepository.All().AnyAsync(x => x.PackageType == name && x.PackageTypeId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Package Type" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Package Type" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Package Type" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Package Type" && x.CheckDelete);
        }
        #endregion
    }
}
