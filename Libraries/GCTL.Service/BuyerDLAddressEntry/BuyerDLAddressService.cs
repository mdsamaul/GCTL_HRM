//using DocumentFormat.OpenXml.Wordprocessing;
//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.BuyerBrands;
//using GCTL.Core.ViewModels.BuyerDLAddress;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace GCTL.Service.BuyerDLAddressEntry
//{
//    public class BuyerDLAddressService : AppService<RmgProdDefDeliveryAddress>, IBuyerDLAddressService
//    {
//        #region Private Fields

//        private readonly IRepository<RmgProdDefBuyer> buyerRepo;
//        private readonly ICommonService comService;
//        private readonly IRepository<RmgProdDefDeliveryAddress> dlRepo;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        #endregion Private Fields

//        #region Public Constructors

//        public BuyerDLAddressService(ICommonService comService, IRepository<RmgProdDefDeliveryAddress> dlRepo, IRepository<RmgProdDefBuyer> buyerRepo, IRepository<CoreAccessCode> accessCodeRepository) : base(dlRepo)
//        {
//            this.comService = comService;
//            this.dlRepo = dlRepo;
//            this.buyerRepo = buyerRepo;
//            this.accessCodeRepository = accessCodeRepository;
//        }

//        #endregion Public Constructors

//        #region Public Methods

//        public async Task<(bool isSuccess, string message)> BulkDeleteAsync(List<decimal> tcs, bool useTransaction = true)
//        {

//            if (tcs == null || !tcs.Any())
//                return (false, "Validation Failed");

//            const int batchSize = 500;

//            if (useTransaction)
//                await dlRepo.BeginTransactionAsync();

//            try
//            {
//                for (int i = 0; i < tcs.Count; i += batchSize)
//                {
//                    var batch = tcs.Skip(i).Take(batchSize).ToList();
//                    var entries = await dlRepo.All()
//                        .Where(e => batch.Contains(e.Tc))
//                        .AsNoTracking()
//                        .ToListAsync();

//                    if (!entries.Any()) continue;

//                    await dlRepo.DeleteRangeAsync(entries);
//                }
//                if (useTransaction)
//                    await dlRepo.CommitTransactionAsync();

//                return (true, "Deleted Successfully");
//            }
//            catch (Exception ex)
//            {
//                if (useTransaction)
//                {
//                    await dlRepo.RollbackTransactionAsync();
//                    return (false, "Internal Server Error!");
//                }

//                throw;
//            }
//        }

//        public async Task<RMGProdDLAddressViewModel> GetByIdAsync(decimal id)
//        {
//            try
//            {
//                var dl = await dlRepo.GetByIdAsync(id);

//                var buyer = buyerRepo.All().Where(x => x.BuyerId == dl.BuyerId).FirstOrDefault();

//                if (dl == null)
//                {
//                    return null;
//                }

//                var record = new RMGProdDLAddressViewModel
//                {
//                    Tc = dl.Tc,
//                    BuyerId = dl.BuyerId,
//                    DeliveryAddressId = dl?.DeliveryAddressId,
//                    Name = dl?.Name,
//                    DeliveryAddress = dl?.DeliveryAddress,
//                    ContactPerson = dl?.ContactPerson,
//                    Designation = dl?.Designation,
//                    Phone = dl?.Phone,
//                    Email = dl?.Email,

//                    Ldate = dl.Ldate,
//                    ModifyDate = dl.ModifyDate,
//                };

//                return record;

//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        public async Task<(List<RMGProdDLAddressViewModel> Data, int totalRecord, int curentRecord)> GetPaginatedDataAsync(string searchValue, int page, int pageSize, string sortColumn, string sortDirection, string id, string buyerId)
//        {
//            var query = await (from b in dlRepo.All()
//                               join buy in buyerRepo.All() on b.BuyerId equals buy.BuyerId into bbuy
//                               from buy in bbuy.DefaultIfEmpty()   // left join

//                               select new RMGProdDLAddressViewModel
//                               {
//                                   Tc = b.Tc,
//                                   DeliveryAddressId = b.DeliveryAddressId,
//                                   Name = b.Name,
//                                   DeliveryAddress = b.DeliveryAddress,
//                                   BuyerId = b.BuyerId,
//                                   BuyerName = buy.BuyerName,
//                                   ContactPerson= b.ContactPerson,
//                                   Designation = b.Designation,
//                                   Phone = b.Phone,
//                                   Email = b.Email,
//                               })
//                              .ToListAsync();

//            var materializedQuery = query.Where(x => 
//                (id == null || x.DeliveryAddressId == id) && 
//                (buyerId == null || x.BuyerId == buyerId)
//            );

//            var totalRecord = query.Count();

//            IEnumerable<RMGProdDLAddressViewModel> filterQuery = materializedQuery;

//            if (!string.IsNullOrWhiteSpace(searchValue))
//            {
//                filterQuery = filterQuery.Where( d =>
//                    (d.DeliveryAddressId?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.Name?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.DeliveryAddress?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.ContactPerson?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.Designation?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.Phone?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.Email?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) 
//                );
//            }

//            var currentRecord = filterQuery.Count();

//            if (!string.IsNullOrWhiteSpace(sortColumn) && !string.IsNullOrWhiteSpace(sortDirection))
//            {
//                filterQuery = sortColumn.ToLower() switch
//                {
//                    "deliveryaddressid" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.DeliveryAddressId) : filterQuery.OrderByDescending(x => x.DeliveryAddressId),
//                    "name" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.Name) : filterQuery.OrderByDescending(x => x.Name),
//                    "deliveryaddress" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.DeliveryAddress) : filterQuery.OrderByDescending(x => x.DeliveryAddress),
//                    "contactperson" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.ContactPerson) : filterQuery.OrderByDescending(x => x.ContactPerson),
//                    "designation" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.Designation) : filterQuery.OrderByDescending(x => x.Designation),
//                    "phone" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.Phone) : filterQuery.OrderByDescending(x => x.Phone),
//                    "email" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.Email) : filterQuery.OrderByDescending(x => x.Email),
//                    _ => filterQuery.OrderBy(a => a.Tc)
//                };
//            }
//            else
//            {
//                filterQuery = filterQuery.OrderBy(a => a.Tc);
//            }

//            var data = pageSize < 0 ? filterQuery.ToList() : filterQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

//            return (data, totalRecord, currentRecord);
//        }

//        public async Task<bool> IsDuplicate(string dlAddress, string buyerId, string id = null)
//        {
//            if (string.IsNullOrWhiteSpace(dlAddress))
//                return false;

//            return await dlRepo.All()
//                .AnyAsync(x => x.DeliveryAddress.ToLower() == dlAddress.ToLower()
//                       && x.BuyerId == buyerId
//                       && (id == null || x.DeliveryAddressId != id));
//        }

//        public async Task<(bool isSuccess, string message, object data)> SaveAsync(RMGProdDLAddressViewModel model)
//        {
//            if (model == null)
//            {
//                return (false, "Internal server error!", null);
//            }

//            if (string.IsNullOrWhiteSpace(model.DeliveryAddress))
//            {
//                return (false, "Delivery Address is required!", null);
//            }
//            await dlRepo.BeginTransactionAsync();
//            string logoPath = string.Empty;
//            try
//            {
//                if (model.Tc == 0)
//                {
//                    if(await IsDuplicate(model.DeliveryAddress, model.BuyerId))
//                    {
//                        return (false, "Duplicate Delivery Address", null);
//                    }

//                    var newId = comService.GenerateNextCode("DeliveryAddressId", "RMG_Prod_Def_DeliveryAddress", 3);
//                    //need to edit
//                    RmgProdDefDeliveryAddress record = new RmgProdDefDeliveryAddress
//                    {
//                        DeliveryAddressId = newId,
//                        BuyerId = model.BuyerId,
//                        Name = model.Name ?? string.Empty,
//                        DeliveryAddress = model.DeliveryAddress,
//                        ContactPerson = model.ContactPerson ?? string.Empty,
//                        Designation = model.Designation ?? string.Empty,
//                        Phone = model.Phone ?? string.Empty,
//                        Email = model.Email ?? string.Empty,

//                        Ldate = model.Ldate,
//                        Luser = model.Luser,
//                        Lip = model.Lip,
//                        Lmac = model.Lmac,
//                    };
//                    await dlRepo.AddAsync(record);
//                    await dlRepo.CommitTransactionAsync();
//                    return (true, "Saved Successfully!", record);
//                }
//                else
//                {
//                    //need to edit
//                    var exData = await dlRepo.GetByIdAsync(model.Tc);
//                    if (exData == null)
//                        return (false, "Data does not exists!", null);

//                    if (await IsDuplicate(model.DeliveryAddress, model.BuyerId, exData.DeliveryAddressId))
//                        return (false, "Duplicate Delivery Address!", null);

//                    exData.Name = model.Name ?? string.Empty;
//                    exData.DeliveryAddress = model.DeliveryAddress;
//                    exData.ContactPerson = model.ContactPerson ?? string.Empty;
//                    exData.Designation = model.Designation ?? string.Empty;
//                    exData.Phone = model.Phone ?? string.Empty;
//                    exData.Email = model.Email ?? string.Empty;

//                    exData.ModifyDate = model.ModifyDate;
//                    exData.Luser = model.Luser;
//                    exData.Lip = model.Lip;
//                    exData.Lmac = model.Lmac;

//                    await dlRepo.UpdateAsync(exData);
//                    await dlRepo.CommitTransactionAsync();
//                    return (true, "Update Successfully", exData);
//                }
//            }
//            catch (Exception ex)
//            {
//                await dlRepo?.RollbackTransactionAsync();


//                return (false, "Internal Server Error!", null);
//            }
//        }


//        #region Permission all type
//        public async Task<bool> PagePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Buyer Info" && x.TitleCheck);
//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Buyer Info" && x.CheckAdd);
//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Buyer Info" && x.CheckEdit);
//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Buyer Info" && x.CheckDelete);
//        }
//        #endregion

//        #endregion Public Methods

//    }
//}
