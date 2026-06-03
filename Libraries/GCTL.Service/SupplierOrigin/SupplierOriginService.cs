using GCTL.Core.Data;
using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.SupplierOrigin;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.SupplierOrigin
{
    public class SupplierOriginService : AppService<InvDefSupplierOrigin>, ISupplierOriginService
    {
        #region Service & Repository
        public readonly IRepository<InvDefSupplierOrigin> supplierOriginrepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;

        private const string TableName = "Inv_Def_SupplierOrigin";
        private const string ColumnName = "SupplierOriginID";
        public SupplierOriginService(
            IRepository<InvDefSupplierOrigin> supplierOriginrepository,
             IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService

            ) 
            
    : base(supplierOriginrepository)
        {
            this.supplierOriginrepository = supplierOriginrepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        #endregion

        #region GetAllAsync

        public async Task<List<SupplierOriginSetupViewModel>> GetAllAsync()
        {
            var entity = await supplierOriginrepository.GetAllAsync();
            return entity.Select(entityVM => new SupplierOriginSetupViewModel
            {
                Tc = entityVM.Tc,
                SupplierOriginId = entityVM.SupplierOriginId,
                SupplierOrigin = entityVM.SupplierOrigin,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,

            }).ToList();
        }

        #endregion

        #region GetByIdAsync

        public async Task<SupplierOriginSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await supplierOriginrepository.GetByIdAsync(code);
            if (entity == null) return null;

            return new SupplierOriginSetupViewModel
            {
                Tc = entity.Tc,
                SupplierOriginId = entity.SupplierOriginId,
                SupplierOrigin = entity.SupplierOrigin,
                Luser = entity.Luser,
                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Lip = entity.Lip,
                Lmac = entity.Lmac
            };
        }

        //public async Task<SupplierOriginSetupViewModel> GetByIdAsync(string code)
        //{
        //    var entity = await supplierOriginrepository.GetByIdAsync(code);
        //    if (entity == null) return null;

        //    SupplierOriginSetupViewModel entityVM = new SupplierOriginSetupViewModel();
        //    entityVM.Tc = entity.Tc;
        //    entityVM.SupplierOriginId = entity.SupplierOriginId;
        //    entityVM.SupplierOrigin = entity.SupplierOrigin;
        //    entityVM.Luser = entity.Luser;
        //    entityVM.Ldate = entity.Ldate;
        //    entityVM.ModifyDate = entity.ModifyDate;
        //    entityVM.Lip = entity.Lip;
        //    entityVM.Lmac = entity.Lmac;

        //    return entityVM;
        //}

        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(SupplierOriginSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            await supplierOriginrepository.BeginTransactionAsync();
            try
            {
                InvDefSupplierOrigin entity = new InvDefSupplierOrigin();
                entity.SupplierOriginId = strMaxNO;
                entity.SupplierOrigin = entityVM.SupplierOrigin;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await supplierOriginrepository.AddAsync(entity);
                await supplierOriginrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await supplierOriginrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(SupplierOriginSetupViewModel entityVM)
        {
            await supplierOriginrepository.BeginTransactionAsync();
            try
            {
                var entity = await supplierOriginrepository.GetByIdAsync(entityVM.SupplierOriginId);
                if (entity == null)
                {
                    await supplierOriginrepository.RollbackTransactionAsync();
                    return false;
                }
                entity.SupplierOriginId = entityVM.SupplierOriginId;
                entity.SupplierOrigin = entityVM.SupplierOrigin;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await supplierOriginrepository.UpdateAsync(entity);
                await supplierOriginrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await supplierOriginrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region SelectionAsync
        public async Task<IEnumerable<CommonSelectModel>> SelectionSupplierOriginAsync()
        {
            var data = await supplierOriginrepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.SupplierOriginId,
                           Name = x.SupplierOrigin,
                       }).ToListAsync();
            return data;
        }

        #endregion

        #region DeleteTab
        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await supplierOriginrepository.All().Where(x => ids.Contains(x.SupplierOriginId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            supplierOriginrepository.Delete(entity);

            return true;
        }
        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await supplierOriginrepository.All().AnyAsync(x => x.SupplierOriginId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await supplierOriginrepository.All().AnyAsync(x => x.SupplierOrigin == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await supplierOriginrepository.All().AnyAsync(x => x.SupplierOrigin == name && x.SupplierOriginId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Origin" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Origin" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Origin" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Origin" && x.CheckDelete);
        }
        #endregion

    }
}
