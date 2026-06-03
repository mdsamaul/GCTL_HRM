using GCTL.Core.Data;
using GCTL.Core.ViewModels.InvDefSupplierType;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.InvDefSupplierTypes
{
    public class InvDefSupplierTypeService : AppService<InvDefSupplierType>, IInvDefSupplierTypeService
    {
        private readonly IRepository<InvDefSupplierType> supplierTypeRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public InvDefSupplierTypeService(
            IRepository<InvDefSupplierType> supplierTypeRepo,
            IRepository<CoreAccessCode> accessCodeRepository
            ) : base(supplierTypeRepo)
        {
            this.supplierTypeRepo = supplierTypeRepo;
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

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Inv Def Supplier Type" && x.TitleCheck);

        }

        public async Task<bool> SavePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Inv Def Supplier Type" && x.CheckAdd);

        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Inv Def Supplier Type" && x.CheckEdit);

        }

        public async Task<bool> DeletePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Inv Def Supplier Type" && x.CheckDelete);

        }

        #endregion

        public async Task<List<InvDefSupplierTypeSetupViewModel>> GetAllAsync()
        {
            //await Task.Delay(100);
            try
            {
                return supplierTypeRepo.All().Select(c => new InvDefSupplierTypeSetupViewModel
                {
                    //AutoId = c.AutoId,
                    SupplierTypeId = c.SupplierTypeId,
                    //SupplierType = c.SupplierType,
                    //ShortName = c.ShortName
                }).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<InvDefSupplierTypeSetupViewModel> GetByIdAsync(string id)
        {


            try
            {
                var entity = supplierTypeRepo.All().Where(x => x.SupplierTypeId == id).FirstOrDefault();
                if (entity == null) return null;

                return new InvDefSupplierTypeSetupViewModel
                {
                    //AutoId = entity.AutoId,/
                    SupplierTypeId = entity.SupplierTypeId,
                    //SupplierType = entity.SupplierType,
                    //ShortName = entity.ShortName,
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

        public async Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(InvDefSupplierTypeSetupViewModel model)
        {
            try
            {
                if (model.AutoId == 0)
                {
                    var entity = new InvDefSupplierType
                    {
                        SupplierTypeId = model.SupplierTypeId,
                        //SupplierType= model.SupplierType,
                        //ShortName = model.ShortName,
                        //CompanyCode = model.CompanyCode != null ? model.CompanyCode : "001",
                        Luser = model.Luser,
                        Lip = model.Lip,
                        Ldate = model.Ldate,
                        Lmac = model.Lmac,
                        //UserInfoEmployeeId = model.UserInfoEmployeeId,
                    };

                    await supplierTypeRepo.AddAsync(entity);
                    return (true, CreateSuccess, entity);
                }

                var exData = await supplierTypeRepo.GetByIdAsync(model.AutoId);
                if (exData != null)
                {
                    // Update existing data
                    exData.SupplierTypeId = model.SupplierTypeId;
                    //exData.SupplierType = model.SupplierType;
                    //exData.ShortName = model.ShortName;
                    //exData.CompanyCode = model.CompanyCode != null ? model.CompanyCode : "001";
                    exData.ModifyDate = DateTime.Now;
                    await supplierTypeRepo.UpdateAsync(exData);
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
                    var entity = await supplierTypeRepo.GetByIdAsync(decimal.Parse(id));
                    if (entity == null)
                    {
                        continue;
                    }

                    await supplierTypeRepo.DeleteAsync(entity);
                }
                catch (Exception)
                {
                    return (true, DeleteFailed, null);
                }
            }

            return (true, DeleteSuccess, null);
        }

        public async Task<string> AutoSuplierTypeIdAsync()
        {
            var SupplierTypeList = (await supplierTypeRepo.GetAllAsync()).ToList();

            int newSupplierTypeId;

            if (SupplierTypeList != null && SupplierTypeList.Count > 0)
            {
                var lastCatagoryId = SupplierTypeList
                    .OrderByDescending(x => x.SupplierTypeId)
                    .Select(x => x.SupplierTypeId)
                    .FirstOrDefault();

                // Try parse in case CatagoryId is string
                int.TryParse(lastCatagoryId, out int lastId);
                newSupplierTypeId = lastId + 1;
            }
            else
            {
                newSupplierTypeId = 1;
            }

            return newSupplierTypeId.ToString("D3");
        }

        public async Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string SupplierTypeValue)
        {
            //bool Exists = supplierTypeRepo.All().Any(x => x.SupplierTypeName == SupplierTypeValue);
            bool Exists = false;
            return (Exists, DataExists, null);
        }
    }
}
