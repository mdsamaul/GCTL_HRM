using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.SalesDefVehicleType;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.SalesDefVehicleTypeService
{
    public class SalesDefVehicleTypeServices : AppService<SalesDefVehicleType>, ISalesDefVehicleTypeService
    {
        private readonly IRepository<SalesDefVehicleType> transportRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public SalesDefVehicleTypeServices(
            IRepository<SalesDefVehicleType> transportRepo,
            IRepository<CoreAccessCode> accessCodeRepository
            ) : base(transportRepo)
        {
            this.transportRepo = transportRepo;
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

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Def Vehicle Type" && x.TitleCheck);

        }

        public async Task<bool> SavePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Def Vehicle Type" && x.CheckAdd);

        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Def Vehicle Type" && x.CheckEdit);

        }

        public async Task<bool> DeletePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Def Vehicle Type" && x.CheckDelete);

        }

        #endregion

        public async Task<List<SalesDefVehicleTypeSetupViewModel>> GetAllAsync()
        {
            //await Task.Delay(100);
            try
            {
                return transportRepo.All().Select(c => new SalesDefVehicleTypeSetupViewModel
                {
                    TC = c.Tc,
                    VehicleTypeID = c.VehicleTypeId,
                    VehicleType = c.VehicleType,
                    ShortName = c.ShortName
                }).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<SalesDefVehicleTypeSetupViewModel> GetByIdAsync(string id)
        {


            try
            {
                var entity = transportRepo.All().Where(x => x.VehicleTypeId == id).FirstOrDefault();
                if (entity == null) return null;

                return new SalesDefVehicleTypeSetupViewModel
                {
                    TC = entity.Tc  ,
                    VehicleTypeID = entity.VehicleTypeId,
                    VehicleType = entity.VehicleType,
                    ShortName = entity.ShortName,
                    ShowCreateDate = entity.Ldate.HasValue ? entity.Ldate.Value.ToString("dd/MM/yyyy") : "",
                    ShowModifyDate = entity.ModifyDate.HasValue ? entity.ModifyDate.Value.ToString("dd/MM/yyyy") : "",
                };
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(SalesDefVehicleTypeSetupViewModel model, string companyCode, string employeeId)
        {
            try
            {
                if (model.TC == 0)
                {
                    var entity = new SalesDefVehicleType
                    {
                        VehicleTypeId = model.VehicleTypeID,
                        VehicleType = model.VehicleType,
                        ShortName = model.ShortName,
                       
                        Luser = model.Luser,
                        Lip = model.Lip,
                        Ldate = model.Ldate,
                        Lmac = model.Lmac,
                    };

                    await transportRepo.AddAsync(entity);
                    return (true, CreateSuccess, entity);
                }

                var exData = await transportRepo.GetByIdAsync(model.TC);
                if (exData != null)
                {
                    // Update existing data
                    exData.VehicleTypeId = model.VehicleTypeID;
                    exData.VehicleType = model.VehicleType;
                    exData.ShortName = model.ShortName;
                   
                    exData.ModifyDate = DateTime.Now;
                    await transportRepo.UpdateAsync(exData);
                    return (true, UpdateSuccess, exData);
                }

                return (false, UpdateFailed, null);
            }
            catch (Exception)
            {
                return (false, CreateFailed, null);
            }
        }

        public async Task<(bool isSuccess, string message, object data)> DeleteAsync(List<int> ids)
        {
            foreach (var id in ids)
            {

                try
                {
                    var entity = await transportRepo.GetByIdAsync(id);
                    if (entity == null)
                    {
                        continue;
                    }

                    await transportRepo.DeleteAsync(entity);
                }
                catch (Exception)
                {
                    return (false, DeleteFailed, null);
                }
            }

            return (true, DeleteSuccess, null);
        }

        public async Task<string> AutoVehicleTypeIdAsync()
        {
            var VehicleTypeList = (await transportRepo.GetAllAsync()).ToList();

            int newVehicleTypeId;

            if (VehicleTypeList != null && VehicleTypeList.Count > 0)
            {
                var lastVehicleTypeId = VehicleTypeList
                    .OrderByDescending(x => x.Tc)
                    .Select(x => x.VehicleTypeId)
                    .FirstOrDefault();

                // Try parse in case VehicleTypeCode is string
                int.TryParse(lastVehicleTypeId, out int lastId);
                newVehicleTypeId = lastId + 1;
            }
            else
            {
                newVehicleTypeId = 1;
            }

            return newVehicleTypeId.ToString("D3");
        }

        public async Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string VehicleTypeValue)
        {
            bool Exists = transportRepo.All().Any(x => x.VehicleType == VehicleTypeValue);
            return (Exists, DataExists, null);
        }
    }
}
