using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.Core_Country;
using GCTL.Core.ViewModels.INV_Catagory;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.Core_Countrys
{
    public class Core_CountryService:AppService<CoreCountry>, ICore_CountryService
    {
        private readonly IRepository<CoreCountry> countryRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public Core_CountryService(
            IRepository<CoreCountry> countryRepo,
            IRepository<CoreAccessCode> accessCodeRepository
            ) : base(countryRepo)
        {
            this.countryRepo = countryRepo;
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

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Core Country" && x.TitleCheck);

        }

        public async Task<bool> SavePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Core Country" && x.CheckAdd);

        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Core Country" && x.CheckEdit);

        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Core Country" && x.CheckDelete);

        }

        #endregion

        public async Task<List<Core_CountrySetupViewModel>> GetAllAsync()
        {
            //await Task.Delay(100);
            try
            {
                return countryRepo.All().Select(c => new Core_CountrySetupViewModel
                {
                    CountryCode = c.CountryCode,
                    CountryID = c.CountryId,
                    CountryName = c.CountryName,
                    IOCCode = c.Ioccode,
                    ISOCode = c.Isocode
                }).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<Core_CountrySetupViewModel> GetByIdAsync(string id)
        {


            try
            {
                var entity = countryRepo.All().Where(x => x.CountryId == id).FirstOrDefault();
                if (entity == null) return null;

                return new Core_CountrySetupViewModel
                {
                    CountryCode = entity.CountryCode,
                    CountryID = entity.CountryId,
                    CountryName = entity.CountryName,
                    ISOCode = entity.Isocode,
                    IOCCode = entity.Ioccode,
                    //Ldate= entity.Ldate.HasValue? entity.Ldate.Value.ToString("dd/MM/yyyy"):"",
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

        public async Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(Core_CountrySetupViewModel model)
        {
            try
            {
                if (model.CountryCode == 0)
                {
                    var entity = new CoreCountry
                    {
                        CountryId = model.CountryID,
                        CountryName = model.CountryName,
                        Isocode = model.ISOCode,
                        Ioccode = model.IOCCode,
                        Luser = model.Luser,
                        Lip = model.Lip,
                        Ldate = model.Ldate,
                        Lmac = model.Lmac,
                    };

                    await countryRepo.AddAsync(entity);
                    return (true, CreateSuccess, entity);
                }

                var exData = await countryRepo.GetByIdAsync(model.CountryCode);
                if (exData != null)
                {
                    // Update existing data
                    exData.CountryId = model.CountryID;
                    exData.CountryName = model.CountryName;
                    exData.Ioccode = model.IOCCode;
                    exData.Isocode = model.ISOCode;
                    exData.ModifyDate = DateTime.Now;
                    await countryRepo.UpdateAsync(exData);
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
                    var entity = await countryRepo.GetByIdAsync(id);
                    if (entity == null)
                    {
                        continue;
                    }

                    await countryRepo.DeleteAsync(entity);
                }
                catch (Exception)
                {
                    return (true, DeleteFailed, null);
                }
            }

            return (true, DeleteSuccess, null);
        }

        public async Task<string> AutoCountryIdAsync()
        {
            try
            {
                var countryList = (await countryRepo.GetAllAsync()).ToList();

                int newCountryId;

                if (countryList != null && countryList.Count > 0)
                {
                    var lastCountryId = countryList
                        .OrderByDescending(x => x.CountryCode)
                        .Select(x => x.CountryId)
                        .FirstOrDefault();

                    // Try parse in case CountryId is string
                    int.TryParse(lastCountryId, out int lastId);
                    newCountryId = lastId + 1;
                }
                else
                {
                    newCountryId = 1;
                }

                return newCountryId.ToString("D3");
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string countryValue)
        {
            bool Exists = countryRepo.All().Any(x => x.CountryName == countryValue);
            return (Exists, DataExists, null);
        }
    }
}
