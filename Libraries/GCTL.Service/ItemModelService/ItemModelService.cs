using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.INV_Catagory;
using GCTL.Core.ViewModels.ItemModel;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.ItemModelService
{
    public class ItemModelService:AppService<HrmModel>,IItemModelService
    {
        private readonly IRepository<HrmModel> ItemModelRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<HrmBrand> brandRepo;

        public ItemModelService(
            IRepository<HrmModel> ItemModelRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
            IRepository<HrmBrand> brandRepo
            ) : base(ItemModelRepo)
        {
            this.ItemModelRepo = ItemModelRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.brandRepo = brandRepo;
        }

        private readonly string CreateSuccess = "Data saved successfully.";
        private readonly string CreateFailed = "Data insertion failed.";
        private readonly string UpdateSuccess = "Data updated successfully.";
        private readonly string UpdateFailed = "Data update failed.";
        private readonly string DeleteSuccess = "Data deleted successfully.";
        private readonly string DeleteFailed = "Data deletion failed.";
        private readonly string DataExists = "Data already exists.";

        #region Permission all type

        public async Task<bool> PagePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Item Model" && x.TitleCheck);

        }

        public async Task<bool> SavePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Item Model" && x.CheckAdd);

        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Item Model" && x.CheckEdit);

        }

        public async Task<bool> DeletePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Item Model" && x.CheckDelete);

        }

        #endregion

        public async Task<List<ItemModelSetupViewModel>> GetAllAsync()
        {
            //await Task.Delay(100);
            try
            {
                return ItemModelRepo.All().Select(c => new ItemModelSetupViewModel
                {
                    AutoId = c.AutoId,
                    ModelID = c.ModelId,
                    ModelName = c.ModelName,
                    ShortName = c.ShortName,
                    BrandID= c.BrandId,
                    BrandName= brandRepo.All().Where(x=>x.BrandId == c.BrandId).Select(x=>x.BrandName).FirstOrDefault(),
                }).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<ItemModelSetupViewModel> GetByIdAsync(string id)
        {
            try
            {
                var entity = ItemModelRepo.All().Where(x => x.ModelId == id).FirstOrDefault();
                if (entity == null) return null;

                return new ItemModelSetupViewModel
                {
                    AutoId = entity.AutoId,
                    ModelID = entity.ModelId,
                    ModelName = entity.ModelName,
                    ShortName = entity.ShortName,
                    BrandID = entity.BrandId,                  
                    ShowCreateDate = entity.Ldate.HasValue ? entity.Ldate.Value.ToString("dd/MM/yyyy") : "",
                    ShowModifyDate = entity.ModifyDate.HasValue ? entity.ModifyDate.Value.ToString("dd/MM/yyyy") : "",
                    ModifyDate = entity.ModifyDate
                };
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(ItemModelSetupViewModel model)
        {
            try
            {
                if (model.AutoId == 0)
                {
                    var entity = new HrmModel
                    {
                        ModelId = model.ModelID,
                        ModelName = model.ModelName,
                        BrandId= model.BrandID,
                        ShortName = model.ShortName,
                        CompanyCode = model.CompanyCode != null ? model.CompanyCode : "001",
                        Luser = model.Luser,
                        Lip = model.Lip,
                        Ldate = model.Ldate,
                        Lmac = model.Lmac,
                        UserInfoEmployeeId = model.UserInfoEmployeeId,
                    };

                    await ItemModelRepo.AddAsync(entity);
                    return (true, CreateSuccess, entity);
                }

                var exData = await ItemModelRepo.GetByIdAsync(model.AutoId);
                if (exData != null)
                {
                    // Update existing data
                    exData.ModelId = model.ModelID;
                    exData.ModelName = model.ModelName;
                    exData.ShortName = model.ShortName;
                    exData.BrandId=model.BrandID;
                    exData.CompanyCode = model.CompanyCode != null ? model.CompanyCode : "001";
                    exData.ModifyDate = model.ModifyDate;
                    await ItemModelRepo.UpdateAsync(exData);
                    return (true, UpdateSuccess, exData);
                }

                return (false, UpdateFailed, null);
            }
            catch (Exception)
            {
                return (false, CreateFailed, null);
            }
        }

        public async Task<(bool isSuccess, string message, object data)> DeleteAsync(List<string> ids)
        {
            foreach (var id in ids)
            {

                try
                {
                    var entity = await ItemModelRepo.GetByIdAsync(decimal.Parse(id));
                    if (entity == null)
                    {
                        continue;
                    }

                    await ItemModelRepo.DeleteAsync(entity);
                }
                catch (Exception)
                {
                    return (true, DeleteFailed, null);
                }
            }

            return (true, DeleteSuccess, null);
        }

        public async Task<string> AutoItemIdAsync()
        {
            var modelList = (await ItemModelRepo.GetAllAsync()).ToList();

            int newModelId;

            if (modelList != null && modelList.Count > 0)
            {
                var lastModelId = modelList
                    .OrderByDescending(x => x.AutoId)
                    .Select(x => x.ModelId)
                    .FirstOrDefault();

                // Try parse in case CatagoryId is string
                int.TryParse(lastModelId, out int lastId);
                newModelId = lastId + 1;
            }
            else
            {
                newModelId = 1;
            }

            return newModelId.ToString("D4");
        }

        public async Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string modelValue)
        {
            bool Exists = ItemModelRepo.All().Any(x => x.ModelName == modelValue);
            return (Exists, DataExists, null);
        }
    }
}
