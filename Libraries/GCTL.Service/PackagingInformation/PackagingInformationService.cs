using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.ContactPersonInfo;
using GCTL.Core.ViewModels.PackagingInformation;
using GCTL.Core.ViewModels.SupplierCategory;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.PackagingInformation
{
    public class PackagingInformationService : AppService<RmgProdDefPackage>, IPackagingInformationService
    {
        #region Service & Repository
        private readonly IRepository<RmgProdDefPackage> packagingInformationrepository;
        private readonly IRepository<RmgProdDefUnitType> unitTyperepository;
        private readonly IRepository<InvDefPackageType> invTyperepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;

        private const string TableName = "RMG_Prod_Def_Package";
        private const string ColumnName = "PackageID";
        public PackagingInformationService(
            IRepository<RmgProdDefPackage> packagingInformationrepository,
            IRepository<RmgProdDefUnitType> unitTyperepository,
            IRepository<InvDefPackageType> invTyperepository,
            IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService

            )
            
    : base(packagingInformationrepository)
        {
            this.packagingInformationrepository = packagingInformationrepository;
            this.unitTyperepository = unitTyperepository;
            this.invTyperepository = invTyperepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        #endregion

        #region GetAllAsync

        public async Task<List<PackagingInformationSetupViewModel>> GetAllAsync()
        {
            var query = await (from per in packagingInformationrepository.All()
                               join hrm in unitTyperepository.All()
                               on per.UnitTypId equals hrm.UnitTypId into JobGroup
                               from hrm in JobGroup.DefaultIfEmpty()
                               join hr in invTyperepository.All()
                               on per.Type equals hr.PackageTypeId into PJobGroup
                               from hr in PJobGroup.DefaultIfEmpty()
                               select new PackagingInformationSetupViewModel
                               {
                                   Tc = per.Tc,
                                   PackageId = per.PackageId,
                                   PackageName = per.PackageName,
                                   UnitTypeName = hrm.UnitTypeName,
                                   PackageType = hr.PackageType,
                                   Volume = per.Volume,
                                   MaxCapacity = per.MaxCapacity,
                                   Remarks = per.Remarks,
                                   Ldate = per.Ldate,
                                   ModifyDate = per.ModifyDate,
                                   Luser = per.Luser,
                                   Lip = per.Lip,
                                   Lmac = per.Lmac
                               }).ToListAsync();

            return query;
        }

        #endregion

        #region GetByIdAsync

        public async Task<PackagingInformationSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await packagingInformationrepository.GetByIdAsync(code);
            if (entity == null) return null;

            PackagingInformationSetupViewModel entityVM = new PackagingInformationSetupViewModel
            {
                Tc = entity.Tc,
                PackageId = entity.PackageId,
                PackageName = entity.PackageName,
                Type = entity.Type,
                Volume = entity.Volume,
                MaxCapacity = entity.MaxCapacity,
                UnitTypId = entity.UnitTypId,
                Remarks = entity.Remarks,
                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Luser = entity.Luser,
                Lip = entity.Lip,
                Lmac = entity.Lmac,
            };

            return entityVM;
        }

        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(PackagingInformationSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            await packagingInformationrepository.BeginTransactionAsync();
            try
            {
                RmgProdDefPackage entity = new RmgProdDefPackage();
                entity.PackageId = strMaxNO;
                entity.PackageName = entityVM.PackageName;
                entity.Type = entityVM.Type ?? string.Empty;
                entity.Volume = entityVM.Volume ?? string.Empty;
                entity.MaxCapacity = entityVM.MaxCapacity ?? 0;
                entity.UnitTypId = entityVM.UnitTypId ?? string.Empty;
                entity.Remarks = entityVM.Remarks ?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await packagingInformationrepository.AddAsync(entity);
                await packagingInformationrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await packagingInformationrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(PackagingInformationSetupViewModel entityVM)
        {
            await packagingInformationrepository.BeginTransactionAsync();
            try
            {
                var entity = await packagingInformationrepository.GetByIdAsync(entityVM.PackageId);
                if (entity == null)
                {
                    await packagingInformationrepository.RollbackTransactionAsync();
                    return false;
                }
                entity.PackageId = entityVM.PackageId;
                entity.PackageName = entityVM.PackageName ?? string.Empty;
                entity.Type = entityVM.Type ?? string.Empty;
                entity.Volume = entityVM.Volume ?? string.Empty;
                entity.MaxCapacity = entityVM.MaxCapacity ?? 0;
                entity.UnitTypId = entityVM.UnitTypId ?? string.Empty;
                entity.Remarks = entityVM.Remarks ?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await packagingInformationrepository.UpdateAsync(entity);
                await packagingInformationrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await packagingInformationrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region SelectionAsync

        public async Task<IEnumerable<CommonSelectModel>> SelectionPackagingAsync()
        {

            var data = await packagingInformationrepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.PackageId,
                           Name = x.PackageName,
                       }).ToListAsync();
            return data;
        }

        #endregion

        #region DeleteTab

        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await packagingInformationrepository.All().Where(x => ids.Contains(x.PackageId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            packagingInformationrepository.Delete(entity);

            return true;
        }
        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await packagingInformationrepository.All().AnyAsync(x => x.PackageId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await packagingInformationrepository.All().AnyAsync(x => x.PackageName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode, string type)
        {
            return await packagingInformationrepository.All().AnyAsync(x => x.PackageId == name && x.PackageName != typeCode && x.Type != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Packaging Information" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Packaging Information" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Packaging Information" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Packaging Information" && x.CheckDelete);
        }
        #endregion
    }
}
