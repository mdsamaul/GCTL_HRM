using GCTL.Core.Data;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.INV_Catagory;
using GCTL.Core.ViewModels.SalesSupplier;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.SalesSupplierService
{
    public class SalesSupplierServices:AppService<SalesSupplier>,ISalesSupplierService
    {
        private readonly IRepository<SalesSupplier> salesSupplierRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<InvDefSupplierType> supTypeRepo;
        private readonly IDeleteHistoryService deleteHistoryService;
        private const string TableName = "Sales_Supplier";
        private const string ColumnName = "SupplierID";

        public SalesSupplierServices(
            IRepository<SalesSupplier> salesSupplierRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
            IRepository<InvDefSupplierType> supTypeRepo,
            IDeleteHistoryService deleteHistoryService
            ) : base(salesSupplierRepo)
        {
            this.salesSupplierRepo = salesSupplierRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.supTypeRepo = supTypeRepo;
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

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Supplier" && x.TitleCheck);

        }

        public async Task<bool> SavePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Supplier" && x.CheckAdd);

        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Supplier" && x.CheckEdit);

        }

        public async Task<bool> DeletePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Supplier" && x.CheckDelete);

        }

        #endregion

        public async Task<List<SalesSupplierSetupViewModel>> GetAllAsync()
        {
            try
            {
                return await Task.Run(() =>
                    salesSupplierRepo.All().Select(c => new SalesSupplierSetupViewModel
                    {
                        AutoId = c.AutoId,
                        SupplierID = c.SupplierId,
                        SupplierName = c.SupplierName,
                        SupplierAddress = c.SupplierAddress,
                        Phone = c.Phone,
                        Email = c.Email,
                        ContactPerson = c.ContactPerson,
                        ContactPhone = c.ContactPhone,
                        ContactEmail = c.ContactEmail,
                        CompanyCode = c.CompanyCode,
                        BinNo = c.BinNo,
                        VatRegNo = c.VatRegNo,
                        Tin = c.Tin,
                        OpeningBalance = c.OpeningBalance,
                        CountryId = c.CountryId,
                        DistrictsID = c.DistrictsId,
                        Place = c.Place,
                        FAX = c.Fax,
                        URL = c.Url,
                        ContatPerson1 = c.ContatPerson1,
                        Designation1 = c.Designation1,
                        Phone1 = c.Phone1,
                        Email1 = c.Email1,
                        ContatPerson2 = c.ContatPerson2,
                        Designation2 = c.Designation2,
                        Phone2 = c.Phone2,
                        Email2 = c.Email2,
                        SupplierTypeId = c.SupplierTypeId,
                        //SupplierType=supTypeRepo.All().Where(x=>x.SupplierTypeId== c.SupplierTypeId).Select(x=>x.SupplierType).FirstOrDefault(),
                        Remarks = c.Remarks,
                        CreditLimit = c.CreditLimit
                    }).ToList()
                );
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<SalesSupplierSetupViewModel> GetByIdAsync(string id)
        {
            try
            {
                var entity = salesSupplierRepo.All().FirstOrDefault(x => x.SupplierId == id);
                if (entity == null) return null;

                return new SalesSupplierSetupViewModel
                {
                    AutoId = entity.AutoId,
                    SupplierID = entity.SupplierId,
                    SupplierName = entity.SupplierName,
                    SupplierAddress = entity.SupplierAddress,
                    Phone = entity.Phone,
                    Email = entity.Email,
                    ContactPerson = entity.ContactPerson,
                    ContactPhone = entity.ContactPhone,
                    ContactEmail = entity.ContactEmail,
                    CompanyCode = entity.CompanyCode,
                    BinNo = entity.BinNo,
                    VatRegNo = entity.VatRegNo,
                    Tin = entity.Tin,
                    OpeningBalance = entity.OpeningBalance,
                    CountryId = entity.CountryId,
                    DistrictsID = entity.DistrictsId,
                    Place = entity.Place,
                    FAX = entity.Fax,
                    URL = entity.Url,
                    ContatPerson1 = entity.ContatPerson1,
                    Designation1 = entity.Designation1,
                    Phone1 = entity.Phone1,
                    Email1 = entity.Email1,
                    ContatPerson2 = entity.ContatPerson2,
                    Designation2 = entity.Designation2,
                    Phone2 = entity.Phone2,
                    Email2 = entity.Email2,
                    SupplierTypeId = entity.SupplierTypeId,
                    Remarks = entity.Remarks,
                    CreditLimit = entity.CreditLimit,
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


        public async Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(SalesSupplierSetupViewModel model)
        {
            try
            {
                if (model.AutoId == 0)
                {
                    var entity = new SalesSupplier
                    {
                        SupplierId = model.SupplierID,
                        SupplierName = model.SupplierName,
                        SupplierAddress = model.SupplierAddress,
                        Phone = model.Phone,
                        Email = model.Email,
                        ContactPerson = model.ContactPerson,
                        ContactPhone = model.ContactPhone,
                        ContactEmail = model.ContactEmail,
                        CompanyCode = model.CompanyCode ?? "001",
                        BinNo = model.BinNo,
                        VatRegNo = model.VatRegNo,
                        Tin = model.Tin,
                        OpeningBalance = model.OpeningBalance,
                        CountryId = model.CountryId,
                        DistrictsId = model.DistrictsID,
                        Place = model.Place,
                        Fax = model.FAX,
                        Url = model.URL,
                        ContatPerson1 = model.ContatPerson1,
                        Designation1 = model.Designation1,
                        Phone1 = model.Phone1,
                        Email1 = model.Email1,
                        ContatPerson2 = model.ContatPerson2,
                        Designation2 = model.Designation2,
                        Phone2 = model.Phone2,
                        Email2 = model.Email2,
                        SupplierTypeId = model.SupplierTypeId,
                        Remarks = model.Remarks,
                        CreditLimit = model.CreditLimit??0,
                        Luser = model.Luser,
                        Lip = model.Lip,
                        Ldate = model.Ldate ?? DateTime.Now,
                        Lmac = model.Lmac,
                        UserInfoEmployeeId = model.UserInfoEmployeeId,
                        
                    };

                    await salesSupplierRepo.AddAsync(entity);
                    return (true, CreateSuccess, entity);
                }

                var exData = await salesSupplierRepo.GetByIdAsync(model.AutoId);
                if (exData != null)
                {
                    exData.SupplierId = model.SupplierID;
                    exData.SupplierName = model.SupplierName;
                    exData.SupplierAddress = model.SupplierAddress;
                    exData.Phone = model.Phone;
                    exData.Email = model.Email;
                    exData.ContactPerson = model.ContactPerson;
                    exData.ContactPhone = model.ContactPhone;
                    exData.ContactEmail = model.ContactEmail;
                    exData.CompanyCode = model.CompanyCode ?? "001";
                    exData.BinNo = model.BinNo;
                    exData.VatRegNo = model.VatRegNo;
                    exData.Tin = model.Tin;
                    exData.OpeningBalance = model.OpeningBalance;
                    exData.CountryId = model.CountryId;
                    exData.DistrictsId = model.DistrictsID;
                    exData.Place = model.Place;
                    exData.Fax = model.FAX;
                    exData.Url = model.URL;
                    exData.ContatPerson1 = model.ContatPerson1;
                    exData.Designation1 = model.Designation1;
                    exData.Phone1 = model.Phone1;
                    exData.Email1 = model.Email1;
                    exData.ContatPerson2 = model.ContatPerson2;
                    exData.Designation2 = model.Designation2;
                    exData.Phone2 = model.Phone2;
                    exData.Email2 = model.Email2;
                    exData.SupplierTypeId = model.SupplierTypeId;
                    exData.Remarks = model.Remarks;
                    exData.CreditLimit = model.CreditLimit??0;
                    exData.ModifyDate = DateTime.Now;

                    await salesSupplierRepo.UpdateAsync(exData);
                    return (true, UpdateSuccess, exData);
                }

                return (false, UpdateFailed, null);
            }
            catch (Exception)
            {
                return (false, CreateFailed, null);
            }
        }


        //public async Task<(bool isSuccess, string message, object data)> DeleteAsync(List<string> ids)
        //{
        //    foreach (var id in ids)
        //    {

        //        try
        //        {
        //            var entity = await salesSupplierRepo.GetByIdAsync(decimal.Parse(id));
        //            if (entity == null)
        //            {
        //                continue;
        //            }

        //            await salesSupplierRepo.DeleteAsync(entity);
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


                var dCheckIds = await salesSupplierRepo.All().Where(x => ids.Contains(x.AutoId.ToString())).Select(s => s.SupplierId).ToListAsync();
                // Dependency check
                var alternateColumn = new List<string> { "SupplierID" };
                var dependencyCheck = await deleteHistoryService.CheckDependenciesAsync(
                salesSupplierRepo.GetTableName(),
                ColumnName,
                dCheckIds,
                alternateColumn
                );



                if (!dependencyCheck.CanDelete)
                {
                    return (false, true, dependencyCheck.Message);
                }



                await salesSupplierRepo.BeginTransactionAsync();



                // Fetch entities
                var entities = await salesSupplierRepo.All()
        .Where(x => ids.Contains(x.AutoId.ToString()))
        .ToListAsync();



                if (entities == null || entities.Count == 0)
                {
                    await salesSupplierRepo.RollbackTransactionAsync();
                    return (false, false, "No matching departments found to delete.");
                }



                // Perform delete
                salesSupplierRepo.Delete(entities);
                model.tableName = TableName;
                // Log deleted records
                await deleteHistoryService.LogDeletedRecordsAsync(
        entities, model
        );



                await salesSupplierRepo.CommitTransactionAsync();
                return (true, false, "Deleted successfully.");
            }
            catch (Exception ex)
            {
                await salesSupplierRepo.RollbackTransactionAsync();
                Console.WriteLine(ex);
                return (false, false, $"Delete failed: {ex.Message}");
            }
        }

        public async Task<string> AutoSalesSupplierIdAsync()
        {
            var salesSupplierList = (await salesSupplierRepo.GetAllAsync()).ToList();

            int nextSerialNumber = 1;
            string prefix = "SUP";
            string yearPart = DateTime.Now.ToString("yy"); 

            if (salesSupplierList != null && salesSupplierList.Count > 0)
            {
                var lastSupplierId = salesSupplierList
                    .OrderByDescending(x => x.AutoId)
                    .Select(x => x.SupplierId)
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(lastSupplierId) && lastSupplierId.Length >= 10)
                {
                    string lastYearPart = lastSupplierId.Substring(3, 2); 
                    string lastSerialPart = lastSupplierId.Substring(5);  

                    if (lastYearPart == yearPart && int.TryParse(lastSerialPart, out int lastSerial))
                    {
                        nextSerialNumber = lastSerial + 1;
                    }
                }
            }

            string formattedSupplierId = $"{prefix}{yearPart}{nextSerialNumber.ToString("D6")}";
            return formattedSupplierId;
        }


        public async Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string SalesSupplierValue)
        {
            bool Exists = salesSupplierRepo.All().Any(x => x.SupplierName == SalesSupplierValue);
            return (Exists, DataExists, null);
        }
    }
}
