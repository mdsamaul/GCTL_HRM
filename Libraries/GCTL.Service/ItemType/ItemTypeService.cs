using GCTL.Core.Data;
using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.ItemType;
using GCTL.Core.ViewModels.StyleInformation;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.ItemType
{
    public class ItemTypeService : AppService<InvDefItemType>, IItemTypeService
    {
        #region Service & Repository
        private readonly IRepository<InvDefItemType> itemTyperepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;
        private const string TableName = "Inv_Def_ItemType";
        private const string ColumnName = "ItemTypeID";

        public ItemTypeService(
            IRepository<InvDefItemType> itemTyperepository,
            IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService

            )
            
    : base(itemTyperepository)
        {
            this.itemTyperepository = itemTyperepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        #endregion

        #region GetAllAsync

        public async Task<List<ItemTypeSetupViewModel>> GetAllAsync()
        {
            var entity = await itemTyperepository.GetAllAsync();
            return entity.Select(entityVM => new ItemTypeSetupViewModel
            {
                AutoId = entityVM.AutoId,
                ItemTypeId = entityVM.ItemTypeId,
                ItemName = entityVM.ItemName,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,

            }).ToList();
        }

        #endregion

        #region GetByIdAsync

        public async Task<ItemTypeSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await itemTyperepository.GetByIdAsync(code);
            if (entity == null) return null;

            ItemTypeSetupViewModel entityVM = new ItemTypeSetupViewModel();
            entityVM.AutoId = entity.AutoId;
            entityVM.ItemTypeId = entity.ItemTypeId;
            entityVM.ItemName = entity.ItemName;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(ItemTypeSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            await itemTyperepository.BeginTransactionAsync();
            try
            {
                InvDefItemType entity = new InvDefItemType();
                entity.ItemTypeId = strMaxNO;
                entity.ItemName = entityVM.ItemName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await itemTyperepository.AddAsync(entity);
                await itemTyperepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await itemTyperepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(ItemTypeSetupViewModel entityVM)
        {
            await itemTyperepository.BeginTransactionAsync();
            try
            {
                var entity = await itemTyperepository.GetByIdAsync(entityVM.ItemTypeId);
                if (entity == null)
                {
                    await itemTyperepository.RollbackTransactionAsync();
                    return false;
                }
                entity.ItemTypeId = entityVM.ItemTypeId;
                entity.ItemName = entityVM.ItemName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await itemTyperepository.UpdateAsync(entity);
                await itemTyperepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await itemTyperepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region SelectionAsync
        public async Task<IEnumerable<CommonSelectModel>> SelectionItemTypeAsync()
        {

            var data = await itemTyperepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.ItemTypeId,
                           Name = x.ItemName,
                       }).ToListAsync();
            return data;
        }

        #endregion

        #region DeleteTab
        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await itemTyperepository.All().Where(x => ids.Contains(x.ItemTypeId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            itemTyperepository.Delete(entity);

            return true;
        }
        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await itemTyperepository.All().AnyAsync(x => x.ItemTypeId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await itemTyperepository.All().AnyAsync(x => x.ItemTypeId == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await itemTyperepository.All().AnyAsync(x => x.ItemName == name && x.ItemTypeId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Item Type" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Item Type" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Item Type" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Item Type" && x.CheckDelete);
        }
        #endregion

    }
}
