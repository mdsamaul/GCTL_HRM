using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.SalesDefVehicle;
using GCTL.Core.ViewModels.SalesDefVehicleType;
using GCTL.Data.Models;
using GCTL.Service.SalesDefVehicleTypeService;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.SalesDefVehicleService
{
    public class SalesDefVehicleService : AppService<SalesDefVehicle>, ISalesDefVehicleService
    {
        private readonly IRepository<SalesDefVehicle> transportInfoRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<CoreCompany> comRepo;
        private readonly IRepository<SalesDefVehicleType> transportTypeRepo;

        public SalesDefVehicleService(
            IRepository<SalesDefVehicle> transportInfoRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
            IRepository<CoreCompany> comRepo,
            IRepository<SalesDefVehicleType> transportTypeRepo
            ) : base(transportInfoRepo)
        {
            this.transportInfoRepo = transportInfoRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.comRepo = comRepo;
            this.transportTypeRepo = transportTypeRepo;
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

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Def Vehicle" && x.TitleCheck);

        }

        public async Task<bool> SavePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Def Vehicle" && x.CheckAdd);

        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Def Vehicle" && x.CheckEdit);

        }

        public async Task<bool> DeletePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Def Vehicle" && x.CheckDelete);

        }

        #endregion

        public async Task<List<SalesDefVehicleSetupViewModel>> GetAllAsync()
        {
            //await Task.Delay(100);
            try
            {
                return transportInfoRepo.All().Select(c => new SalesDefVehicleSetupViewModel
                {
                    TC = c.Tc,
                    VehicleID = c.VehicleId,
                    VehicleTypeID = c.VehicleTypeId,
                    VehicleTypeName = transportTypeRepo.All()
                        .Where(x => x.VehicleTypeId == c.VehicleTypeId)
                        .Select(x => x.VehicleType)
                        .FirstOrDefault(),
                    VehicleNo = c.VehicleNo,
                    CompanyCode = c.CompanyCode,
                    TransportCapacity = c.TransportCapacity,
                    CompanyName = comRepo.All()
                        .Where(x => x.CompanyCode == c.CompanyCode)
                        .Select(x => x.CompanyName)
                        .FirstOrDefault(),
                }).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<SalesDefVehicleSetupViewModel> GetByIdAsync(string id)
        {


            try
            {
                var entity = transportInfoRepo.All().Where(x => x.VehicleId == id).FirstOrDefault();
                if (entity == null) return null;

                return new SalesDefVehicleSetupViewModel
                {
                    TC = entity.Tc,
                    VehicleTypeID = entity.VehicleTypeId,
                    VehicleID = entity.VehicleId,
                    VehicleNo = entity.VehicleNo,
                    TransportCapacity= entity.TransportCapacity,
                    CompanyCode = entity.CompanyCode,
                    ShowCreateDate = entity.Ldate.HasValue ? entity.Ldate.Value.ToString("dd/MM/yyyy") : "",
                    ShowModifyDate = entity.ModifyDate.HasValue ? entity.ModifyDate.Value.ToString("dd/MM/yyyy") : "",
                };
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(SalesDefVehicleSetupViewModel model, string companyCode, string employeeId)
        {
            try
            {
                if (model.TC == 0)
                {
                    var entity = new SalesDefVehicle
                    {
                        VehicleTypeId = model.VehicleTypeID,
                        VehicleNo = model.VehicleNo,
                        VehicleId = model.VehicleID,
                        TransportCapacity = model.TransportCapacity,
                        CompanyCode = companyCode,
                        Luser = model.Luser,
                        Lip = model.Lip,
                        Ldate = model.Ldate,
                        Lmac = model.Lmac,
                    };

                    await transportInfoRepo.AddAsync(entity);
                    return (true, CreateSuccess, entity);
                }

                var exData = await transportInfoRepo.GetByIdAsync(model.TC);
                if (exData != null)
                {
                    // Update existing data
                    exData.VehicleTypeId = model.VehicleTypeID;
                    exData.VehicleNo = model.VehicleNo;
                    exData.VehicleId = model.VehicleID;
                    exData.TransportCapacity = model.TransportCapacity;
                    exData.CompanyCode = companyCode;
                    exData.ModifyDate = DateTime.Now;
                    await transportInfoRepo.UpdateAsync(exData);
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
                    var entity = await transportInfoRepo.GetByIdAsync(id);
                    if (entity == null)
                    {
                        continue;
                    }

                    await transportInfoRepo.DeleteAsync(entity);
                }
                catch (Exception)
                {
                    return (false, DeleteFailed, null);
                }
            }

            return (true, DeleteSuccess, null);
        }

        public async Task<string> AutoTransportInfoIdAsync()
        {
            var TransportInfoList = (await transportInfoRepo.GetAllAsync()).ToList();

            int newTransportInfoId;

            if (TransportInfoList != null && TransportInfoList.Count > 0)
            {
                var lastTransportInfoId = TransportInfoList
                    .OrderByDescending(x => x.Tc)
                    .Select(x => x.VehicleId)
                    .FirstOrDefault();

                // Try parse in case TransportInfoCode is string
                int.TryParse(lastTransportInfoId, out int lastId);
                newTransportInfoId = lastId + 1;
            }
            else
            {
                newTransportInfoId = 1;
            }

            return newTransportInfoId.ToString("D3");
        }

        public async Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string TransportInfoValue)
        {
            bool Exists = transportInfoRepo.All().Any(x => x.VehicleNo == TransportInfoValue);
            return (Exists, DataExists, null);
        }
    }
}
