using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.HRM_Def_Floor;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.HRM_Def_Floors
{
    public class HRM_Def_FloorService:AppService<HrmDefFloor>, IHRM_Def_FloorService
    {
        private readonly IRepository<HrmDefFloor> floorRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public HRM_Def_FloorService(
            IRepository<HrmDefFloor> floorRepo,
            IRepository<CoreAccessCode> accessCodeRepository
            ) : base(floorRepo)
        {
            this.floorRepo = floorRepo;
            this.accessCodeRepository = accessCodeRepository;
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

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "HRM Def Floor" && x.TitleCheck);

        }

        public async Task<bool> SavePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "HRM Def Floor" && x.CheckAdd);

        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "HRM Def Floor" && x.CheckEdit);

        }

        public async Task<bool> DeletePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "HRM Def Floor" && x.CheckDelete);

        }

        #endregion

        public async Task<List<HRM_Def_FloorSetupViewModel>> GetAllAsync()
        {
            //await Task.Delay(100);
            try
            {
                return floorRepo.All().Select(c => new HRM_Def_FloorSetupViewModel
                {
                    AutoId = c.AutoId,
                    FloorCode = c.FloorCode,
                    FloorName = c.FloorName,
                    ShortName = c.ShortName
                }).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<HRM_Def_FloorSetupViewModel> GetByIdAsync(string id)
        {


            try
            {
                var entity = floorRepo.All().Where(x => x.FloorCode == id).FirstOrDefault();
                if (entity == null) return null;

                return new HRM_Def_FloorSetupViewModel
                {
                    AutoId = entity.AutoId,
                    FloorCode = entity.FloorCode,
                    FloorName = entity.FloorName,
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

        public async Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(HRM_Def_FloorSetupViewModel model, string companyCode, string employeeId)
        {
            try
            {
                if (model.AutoId == 0)
                {
                    var entity = new HrmDefFloor
                    {
                        FloorCode = model.FloorCode,
                        FloorName = model.FloorName,
                        ShortName = model.ShortName,
                        CompanyCode = companyCode,
                        EmployeeId = employeeId,
                        Luser = model.Luser,
                        Lip = model.Lip,
                        Ldate = model.Ldate,
                        Lmac = model.Lmac,
                    };

                    await floorRepo.AddAsync(entity);
                    return (true, CreateSuccess, entity);
                }

                var exData = await floorRepo.GetByIdAsync(model.AutoId);
                if (exData != null)
                {
                    // Update existing data
                    exData.FloorCode = model.FloorCode;
                    exData.FloorName = model.FloorName;
                    exData.ShortName = model.ShortName;
                    exData.CompanyCode = companyCode;
                    exData.EmployeeId = employeeId;
                    exData.ModifyDate = DateTime.Now;
                    await floorRepo.UpdateAsync(exData);
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
                    var entity = await floorRepo.GetByIdAsync(decimal.Parse(id));
                    if (entity == null)
                    {
                        continue;
                    }

                    await floorRepo.DeleteAsync(entity);
                }
                catch (Exception)
                {
                    return (true, DeleteFailed, null);
                }
            }

            return (true, DeleteSuccess, null);
        }

        public async Task<string> AutoFloorIdAsync()
        {
            var FloorList = (await floorRepo.GetAllAsync()).ToList();

            int newFloorId;

            if (FloorList != null && FloorList.Count > 0)
            {
                var lastFloorId = FloorList
                    .OrderByDescending(x => x.AutoId)
                    .Select(x => x.FloorCode)
                    .FirstOrDefault();

                // Try parse in case FloorCode is string
                int.TryParse(lastFloorId, out int lastId);
                newFloorId = lastId + 1;
            }
            else
            {
                newFloorId = 1;
            }

            return newFloorId.ToString("D3");
        }

        public async Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string FloorValue)
        {
            bool Exists = floorRepo.All().Any(x => x.FloorName == FloorValue);
            return (Exists, DataExists, null);
        }
    }
}
