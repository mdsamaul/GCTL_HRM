using GCTL.Core.Data;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRM_Size;
using GCTL.Core.ViewModels.INV_Catagory;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRM_Size
{
    public class HRM_SizeService:AppService<HrmSize>, IHRM_SizeService
    {
        private readonly IRepository<HrmSize> hrmSizeRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IDeleteHistoryService deleteHistoryService;
        private const string TableName = "HRM_Size";
        private const string ColumnName = "SizeID";
        public HRM_SizeService(
            IRepository<HrmSize> hrmSizeRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
            IDeleteHistoryService deleteHistoryService
            ) : base(hrmSizeRepo)
        {
            this.hrmSizeRepo = hrmSizeRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.deleteHistoryService = deleteHistoryService;
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

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Size" && x.TitleCheck);

        }

        public async Task<bool> SavePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Size" && x.CheckAdd);

        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Size" && x.CheckEdit);

        }

        public async Task<bool> DeletePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Size" && x.CheckDelete);

        }

        #endregion

        public async Task<List<HRM_SizeSetupViewModel>> GetAllAsync()
        {
            //await Task.Delay(100);
            try
            {
                return hrmSizeRepo.All().Select(c => new HRM_SizeSetupViewModel
                {
                    AutoId = c.AutoId,
                    SizeID = c.SizeId,
                    SizeName = c.SizeName,
                    ShortName = c.ShortName
                }).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<HRM_SizeSetupViewModel> GetByIdAsync(string id)
        {


            try
            {
                var entity = hrmSizeRepo.All().Where(x => x.SizeId == id).FirstOrDefault();
                if (entity == null) return null;

                return new HRM_SizeSetupViewModel
                {
                    AutoId = entity.AutoId,
                    SizeName = entity.SizeName,
                    SizeID = entity.SizeId,
                    ShortName = entity.ShortName,
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

        public async Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(HRM_SizeSetupViewModel model)
        {
            try
            {
                if (model.AutoId == 0)
                {
                    var entity = new HrmSize
                    {
                        SizeId = model.SizeID,
                        SizeName = model.SizeName??"",
                        ShortName = model.ShortName ?? "",
                        CompanyCode = model.CompanyCode != null ? model.CompanyCode : "001",
                        Luser = model.Luser,
                        Lip = model.Lip,
                        Ldate = model.Ldate,
                        Lmac = model.Lmac,
                        UserInfoEmployeeId = model.UserInfoEmployeeId,
                    };

                    await hrmSizeRepo.AddAsync(entity);
                    return (true, CreateSuccess, entity);
                }

                var exData = await hrmSizeRepo.GetByIdAsync(model.AutoId);
                if (exData != null)
                {
                    // Update existing data
                    exData.SizeId = model.SizeID;
                    exData.SizeName = model.SizeName;
                    exData.ShortName = model.ShortName;
                    exData.CompanyCode = model.CompanyCode != null ? model.CompanyCode : "001";
                    exData.ModifyDate = DateTime.Now;
                    await hrmSizeRepo.UpdateAsync(exData);
                    return (true, UpdateSuccess, exData);
                }

                return (false, UpdateFailed, null);
            }
            catch (Exception)
            {
                return (false, CreateFailed, null);
            }
        }

        


        public async Task<(bool success, bool refSuccess, string message)> DeleteAsync(List<string> ids, DeleteHistoryViewModel model)
        {

            try
            {
                if (ids.Count == 0)
                {
                    return (false, false, "No valid department codes found.");
                }


                var dCheckIds = await hrmSizeRepo.All().Where(x => ids.Contains(x.AutoId.ToString())).Select(s => s.SizeId).ToListAsync();
                // Dependency check
                var alternateColumn = new List<string> { "SizeID" };
                var dependencyCheck = await deleteHistoryService.CheckDependenciesAsync(
                hrmSizeRepo.GetTableName(),
                ColumnName,
                dCheckIds,
                alternateColumn
                );



                if (!dependencyCheck.CanDelete)
                {
                    return (false, true, dependencyCheck.Message);
                }



                await hrmSizeRepo.BeginTransactionAsync();



                // Fetch entities
                var entities = await hrmSizeRepo.All()
        .Where(x => ids.Contains(x.AutoId.ToString()))
        .ToListAsync();



                if (entities == null || entities.Count == 0)
                {
                    await hrmSizeRepo.RollbackTransactionAsync();
                    return (false, false, "No matching departments found to delete.");
                }



                // Perform delete
                hrmSizeRepo.Delete(entities);
                model.tableName = TableName;
                // Log deleted records
                await deleteHistoryService.LogDeletedRecordsAsync(
        entities, model
        );



                await hrmSizeRepo.CommitTransactionAsync();
                return (true, false, "Deleted successfully.");
            }
            catch (Exception ex)
            {
                await hrmSizeRepo.RollbackTransactionAsync();
                Console.WriteLine(ex);
                return (false, false, $"Delete failed: {ex.Message}");
            }
        }


        public async Task<string> AutoSizeyIdAsync()
        {
            var SizeList = (await hrmSizeRepo.GetAllAsync()).ToList();

            int newSizeId;

            if (SizeList != null && SizeList.Count > 0)
            {
                var lastCatagoryId = SizeList
                    .OrderByDescending(x => x.AutoId)
                    .Select(x => x.SizeId)
                    .FirstOrDefault();

                // Try parse in case CatagoryId is string
                int.TryParse(lastCatagoryId, out int lastId);
                newSizeId = lastId + 1;
            }
            else
            {
                newSizeId = 1;
            }

            return newSizeId.ToString("D3");
        }

        public async Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string SizeValue)
        {
            bool Exists = hrmSizeRepo.All().Any(x => x.SizeName == SizeValue);
            return (Exists, DataExists, null);
        }
    }
}
