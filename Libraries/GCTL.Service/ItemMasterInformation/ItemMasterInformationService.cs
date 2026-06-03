using GCTL.Core.Data;
using GCTL.Core.ViewModels.Brand;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.ItemMasterInformation;
using GCTL.Core.ViewModels.ItemModel;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.ItemMasterInformation
{
    public class ItemMasterInformationService : AppService<HrmItemMasterInformation>, IItemMasterInformationService
    {
        private readonly IRepository<HrmItemMasterInformation> itemRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<HrmBrand> ProductRepo;
        private readonly IRepository<InvCatagory> cataRepo;
        private readonly IRepository<RmgProdDefUnitType> unitRepo;
        private readonly IRepository<HrmBrand> branchRepo;
        private readonly IDeleteHistoryService deleteHistoryService;
        private const string TableName = "HRM_ItemMasterInformation";
        private const string ColumnName = "ProductCode";
        public ItemMasterInformationService(
            IRepository<HrmItemMasterInformation> itemRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
            IRepository<HrmBrand> ProductRepo,
            IRepository<InvCatagory> cataRepo,
            IRepository<RmgProdDefUnitType> unitRepo,
            IRepository<HrmBrand> branchRepo,
            IDeleteHistoryService deleteHistoryService
            ) : base(itemRepo)
        {
            this.itemRepo = itemRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.ProductRepo = ProductRepo;
            this.cataRepo = cataRepo;
            this.unitRepo = unitRepo;
            this.branchRepo = branchRepo;
            this.deleteHistoryService = deleteHistoryService;
        }



        private readonly string CreateSuccess = "Data saved successfully.";
        private readonly string CreateFailed = "Data insertion failed.";
        private readonly string UpdateSuccess = "Data updated successfully.";
        private readonly string UpdateFailed = "Data update failed.";
        private readonly string DeleteSuccess = "Data deleted successfully.";
        private readonly string DeleteFailed = "Data deletion failed.";
        private readonly string DataExists = "Data already exists.";

      
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


        public async Task<List<ItemMasterInformationSetupViewModel>> GetAllAsync()
        {
            //await Task.Delay(100);
            try
            {
                return itemRepo.All().Select(c => new ItemMasterInformationSetupViewModel
                {
                    AutoId = c.AutoId,
                    ProductCode = c.ProductCode,
                    ProductName = c.ProductName,
                   BrandName = ProductRepo.All().Where(x=>x.BrandId == c.BrandId).Select(x=>x.BrandName).FirstOrDefault(),
                    CatagoryName = cataRepo.All().Where(x=>x.CatagoryId == c.CatagoryId).Select(x=>x.CatagoryName).FirstOrDefault(),
                    UnitID = unitRepo.All().Where(x=>x.UnitTypId == c.UnitId).Select(x=>x.UnitTypeName).FirstOrDefault(),
                    PurchaseCost= c.PurchaseCost,
                    Description=c.Description??""
                }).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<ItemMasterInformationSetupViewModel> GetByIdAsync(string id)
        {
            try
            {
                var entity = itemRepo.All().Where(x => x.ProductCode == id).FirstOrDefault();
                if (entity == null) return null;

                return new ItemMasterInformationSetupViewModel
                {
                    AutoId = entity.AutoId,
                    ProductCode = entity.ProductCode,
                    ProductName = entity.ProductName,
                    Description = entity.Description,
                    BrandName = branchRepo.All().Where(x => x.BrandId == entity.BrandId).Select(x => x.BrandName).FirstOrDefault(),
                    BrandId = entity.BrandId,
                    CatagoryId = entity.CatagoryId,
                    CatagoryName = cataRepo.All().Where(x => x.CatagoryId == entity.CatagoryId).Select(x => x.CatagoryName).FirstOrDefault(),
                    UnitID = entity.UnitId,
                    PurchaseCost = entity.PurchaseCost,
                    CompanyCode = entity.CompanyCode != null ? entity.CompanyCode : "001",  
                    ShowCreateDate = entity.Ldate.HasValue ? entity.Ldate.Value.ToString("dd/MM/yyyy") : "",
                    ShowModifyDate = entity.ModifyDate.HasValue ? entity.ModifyDate.Value.ToString("dd/MM/yyyy") : "",                    
                };
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(ItemMasterInformationSetupViewModel model)
        {
            try
            {
                
                // ================= INSERT =================
                if (model.AutoId == 0)
                {
                    var isDuplicate = await itemRepo.All()
                    .AnyAsync(x => x.ProductCode == model.ProductCode || x.ProductName == model.ProductName);

                    if (isDuplicate)
                    {
                        return (false, "Product Code or Product Name already exists", null);
                    }

                    var entity = new HrmItemMasterInformation
                    {
                        ProductCode = model.ProductCode,
                        ProductName = model.ProductName,
                        Description = model.Description,
                        BrandId = model.BrandName,
                        CatagoryId = model.CatagoryName,
                        UnitId = model.UnitID,
                        PurchaseCost = model.PurchaseCost,
                        CompanyCode = model.CompanyCode ?? "001",
                        Luser = model.Luser,
                        Lip = model.Lip,
                        Ldate = DateTime.Now,
                        Lmac = model.Lmac,
                        UserInfoEmployeeId = model.UserInfoEmployeeId,
                    };

                    await itemRepo.AddAsync(entity);

                    return (true, CreateSuccess, entity);
                }

                // ================= UPDATE =================
                var exData = await itemRepo.GetByIdAsync(model.AutoId);

                if (exData != null)
                {
                    exData.ProductCode = model.ProductCode;
                    exData.ProductName = model.ProductName;
                    exData.Description = model.Description;
                    exData.BrandId = model.BrandName;
                    exData.CatagoryId = model.CatagoryName;
                    exData.UnitId = model.UnitID;
                    exData.PurchaseCost = model.PurchaseCost;
                    exData.CompanyCode = model.CompanyCode ?? "001";
                    exData.ModifyDate = DateTime.Now;

                    await itemRepo.UpdateAsync(exData);

                    return (true, UpdateSuccess, exData);
                }

                return (false, UpdateFailed, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        //public async Task<(bool isSuccess, string message, object data)> DeleteAsync(List<string> ids)
        //{
        //    foreach (var id in ids)
        //    {

        //        try
        //        {
        //            var entity = await itemRepo.GetByIdAsync(decimal.Parse(id));
        //            if (entity == null)
        //            {
        //                continue;
        //            }

        //            await itemRepo.DeleteAsync(entity);
        //        }
        //        catch (Exception)
        //        {
        //            return (true, DeleteFailed, null);
        //        }
        //    }

        //    return (true, DeleteSuccess, null);
        //}

        public async Task<(bool success, bool refSuccess, string message)> DeleteAsync(List<string> ids, DeleteHistoryViewModel model)
        {



            try
            {
                if (ids.Count == 0)
                {
                    return (false, false, "No valid department codes found.");
                }


                var dCheckIds = await itemRepo.All().Where(x => ids.Contains(x.AutoId.ToString())).Select(s => s.ProductCode).ToListAsync();
                // Dependency check
                var alternateColumn = new List<string> { "ProductCode" };
                var dependencyCheck = await deleteHistoryService.CheckDependenciesAsync(
                itemRepo.GetTableName(),
                ColumnName,
                dCheckIds,
                alternateColumn
                );



                if (!dependencyCheck.CanDelete)
                {
                    return (false, true, dependencyCheck.Message);
                }



                await itemRepo.BeginTransactionAsync();



                // Fetch entities
                var entities = await itemRepo.All()
        .Where(x => ids.Contains(x.AutoId.ToString()))
        .ToListAsync();



                if (entities == null || entities.Count == 0)
                {
                    await itemRepo.RollbackTransactionAsync();
                    return (false, false, "No matching departments found to delete.");
                }



                // Perform delete
                itemRepo.Delete(entities);
                model.tableName = TableName;
                // Log deleted records
                await deleteHistoryService.LogDeletedRecordsAsync(
        entities, model
        );



                await itemRepo.CommitTransactionAsync();
                return (true, false, "Deleted successfully.");
            }
            catch (Exception ex)
            {
                await itemRepo.RollbackTransactionAsync();
                Console.WriteLine(ex);
                return (false, false, $"Delete failed: {ex.Message}");
            }
        }

        public async Task<string> AutoProductIdAsync()
        {
            var ProductList = (await itemRepo.GetAllAsync()).ToList();

            int newProductId;

            if (ProductList != null && ProductList.Count > 0)
            {
                var lastProductId = ProductList
                    .OrderByDescending(x => x.AutoId)
                    .Select(x => x.ProductCode)
                    .FirstOrDefault();

                // Try parse in case ProductId is string
                int.TryParse(lastProductId, out int lastId);
                newProductId = lastId + 1;
            }
            else
            {
                newProductId = 1;
            }

            return newProductId.ToString("D6");
        }

        public async Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string ProductValue)
        {
            bool Exists = itemRepo.All().Any(x => x.ProductName == ProductValue);
            return (Exists, DataExists, null);
        }

    }
}