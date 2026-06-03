using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.INV_Catagory;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.INV_Catagory
{
    public class INV_CatagoryService:AppService<InvCatagory>, IINV_CatagoryService
    {
        private readonly IRepository<InvCatagory> invCatRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public INV_CatagoryService(
            IRepository<InvCatagory> invCatRepo,
            IRepository<CoreAccessCode> accessCodeRepository
            ) :base(invCatRepo)
        {
            this.invCatRepo = invCatRepo;
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

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Catagory Info" && x.TitleCheck);

        }

        public async Task<bool> SavePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Catagory Info" && x.CheckAdd);

        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Catagory Info" && x.CheckEdit);

        }

        public async Task<bool> DeletePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Catagory Info" && x.CheckDelete);

        }

        #endregion

        public async Task<List<INV_CatagorySetupViewModel>> GetAllAsync()
        {
            //await Task.Delay(100);
            try
            {
                return invCatRepo.All().Select(c => new INV_CatagorySetupViewModel
                {
                    AutoId = c.AutoId,
                    CatagoryID = c.CatagoryId,
                    CatagoryName = c.CatagoryName,
                    ShortName = c.ShortName
                }).ToList();
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        public async Task<INV_CatagorySetupViewModel> GetByIdAsync(string id)        
        {
           
           
            try
            {
                var entity = invCatRepo.All().Where(x => x.CatagoryId == id).FirstOrDefault();
                if (entity == null) return null;

                return new INV_CatagorySetupViewModel
                {
                    AutoId = entity.AutoId,
                    CatagoryID = entity.CatagoryId,
                    CatagoryName = entity.CatagoryName,
                    ShortName = entity.ShortName,
                    ShowCreateDate= entity.Ldate.HasValue ? entity.Ldate.Value.ToString("dd/MM/yyyy"):"",
                   ShowModifyDate = entity.ModifyDate.HasValue? entity.ModifyDate.Value.ToString("dd/MM/yyyy") :"",
                    ModifyDate = entity.ModifyDate
                };
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(INV_CatagorySetupViewModel model)
        {
            try
            {
                if (model.AutoId == 0)
                {
                    var entity = new InvCatagory
                    {
                        CatagoryId = model.CatagoryID,
                        CatagoryName = model.CatagoryName,
                        ShortName = model.ShortName,
                        CompanyCode = model.CompanyCode != null ? model.CompanyCode : "001",
                        Luser = model.Luser,
                        Lip = model.Lip,
                        Ldate= model.Ldate,
                        Lmac= model.Lmac,
                        UserInfoEmployeeId= model.UserInfoEmployeeId,
                    };

                    await invCatRepo.AddAsync(entity);
                    return (true, CreateSuccess, entity);
                }

                var exData = await invCatRepo.GetByIdAsync(model.AutoId);
                if (exData != null)
                {
                    // Update existing data
                    exData.CatagoryId = model.CatagoryID;
                    exData.CatagoryName = model.CatagoryName;
                    exData.ShortName = model.ShortName;
                    exData.CompanyCode = model.CompanyCode!= null? model.CompanyCode:"001";
                    exData.ModifyDate = DateTime.Now;
                    await invCatRepo.UpdateAsync(exData);
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
                    var entity = await invCatRepo.GetByIdAsync(decimal.Parse(id));
                    if (entity == null)
                    {
                        continue;
                    }

                    await invCatRepo.DeleteAsync(entity);
                }
                catch (Exception)
                {               
                    return (true, DeleteFailed, null);
                }
            }

            return (true, DeleteSuccess, null);
        }

        public async Task<string> AutoCatagoryIdAsync()
        {
            var catagoryList = (await invCatRepo.GetAllAsync()).ToList();

            int newCatagoryId;

            if (catagoryList != null && catagoryList.Count > 0)
            {
                var lastCatagoryId = catagoryList
                    .OrderByDescending(x => x.AutoId)
                    .Select(x => x.CatagoryId)
                    .FirstOrDefault();

                // Try parse in case CatagoryId is string
                int.TryParse(lastCatagoryId, out int lastId);
                newCatagoryId = lastId + 1;
            }
            else
            {
                newCatagoryId = 1;
            }

            return newCatagoryId.ToString("D3"); 
        }

        public async Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string catagoryValue)
        {
            bool Exists =  invCatRepo.All().Any(x => x.CatagoryName == catagoryValue);
            return ( Exists,  DataExists, null);
        }
    }
}
