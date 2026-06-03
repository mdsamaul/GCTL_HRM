using GCTL.Core.Data;
using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.SupplierCategory;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.SupplierCategory
{
    public class SupplierCategoryService : AppService<InvDefSupplierCategory>, ISupplierCategoryService
    {
        #region Service & Repository
        private readonly IRepository<InvDefSupplierCategory> supplierCategoryrepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;

        private const string TableName = "Inv_Def_SupplierCategory";
        private const string ColumnName = "SupplierCategoryID";
        public SupplierCategoryService(
            IRepository<InvDefSupplierCategory> supplierCategoryrepository,
            IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService

            ) 
            
    : base(supplierCategoryrepository)
        {
            this.supplierCategoryrepository = supplierCategoryrepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        #endregion

        #region GetAllAsync

        public async Task<List<SupplierCategorySetupViewModel>> GetAllAsync()
        {
            var entity = await supplierCategoryrepository.GetAllAsync();
            return entity.Select(entityVM => new SupplierCategorySetupViewModel
            {
                SupplierCategoryCode = entityVM.SupplierCategoryCode,
                SupplierCategoryId = entityVM.SupplierCategoryId,
                SupplierCategory = entityVM.SupplierCategory,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,

            }).ToList();
        }

        #endregion

        #region GetByIdAsync

        public async Task<SupplierCategorySetupViewModel> GetByIdAsync(string code)
        {
            var entity = await supplierCategoryrepository.GetByIdAsync(code);
            if (entity == null) return null;

            SupplierCategorySetupViewModel entityVM = new SupplierCategorySetupViewModel();
            entityVM.SupplierCategoryCode = entity.SupplierCategoryCode;
            entityVM.SupplierCategoryId = entity.SupplierCategoryId;
            entityVM.SupplierCategory = entity.SupplierCategory;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(SupplierCategorySetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            await supplierCategoryrepository.BeginTransactionAsync();
            try
            {
                InvDefSupplierCategory entity = new InvDefSupplierCategory();
                entity.SupplierCategoryId = strMaxNO;
                entity.SupplierCategory = entityVM.SupplierCategory;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await supplierCategoryrepository.AddAsync(entity);
                await supplierCategoryrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await supplierCategoryrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(SupplierCategorySetupViewModel entityVM)
        {
            await supplierCategoryrepository.BeginTransactionAsync();
            try
            {
                var entity = await supplierCategoryrepository.GetByIdAsync(entityVM.SupplierCategoryId);
                if (entity == null)
                {
                    await supplierCategoryrepository.RollbackTransactionAsync();
                    return false;
                }
                entity.SupplierCategoryId = entityVM.SupplierCategoryId;
                entity.SupplierCategory = entityVM.SupplierCategory;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await supplierCategoryrepository.UpdateAsync(entity);
                await supplierCategoryrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await supplierCategoryrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region SelectionAsync

        public async Task<IEnumerable<CommonSelectModel>> SelectionSupplierCategoryAsync()
        {

            var data = await supplierCategoryrepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.SupplierCategoryId,
                           Name = x.SupplierCategory,
                       }).ToListAsync();
            return data;
        }

        #endregion

        #region DeleteTab

        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await supplierCategoryrepository.All().Where(x => ids.Contains(x.SupplierCategoryId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            supplierCategoryrepository.Delete(entity);

            return true;
        }
        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await supplierCategoryrepository.All().AnyAsync(x => x.SupplierCategoryId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await supplierCategoryrepository.All().AnyAsync(x => x.SupplierCategory == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await supplierCategoryrepository.All().AnyAsync(x => x.SupplierCategory == name && x.SupplierCategoryId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Category" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Category" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Category" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Category" && x.CheckDelete);
        }
        #endregion

    }
}
