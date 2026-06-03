
//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.BuyerDepartment;
//using GCTL.Core.ViewModels.Common;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace GCTL.Service.BuyerDepartmentEntry
//{
//    public class BuyerDepEntryService : AppService<InvDefBuyerDepartment>, IBuyerDepEntryService
//    {
//        #region Private Fields

//        private readonly ICommonService comService;
//        private readonly IRepository<InvDefBuyerDepartment> depRepo;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;

//        #endregion Private Fields

//        #region Public Constructors

//        public BuyerDepEntryService(ICommonService comService, IRepository<InvDefBuyerDepartment> depRepo, IRepository<CoreAccessCode> accessCodeRepository) : base(depRepo)
//        {
//            this.comService = comService;
//            this.depRepo = depRepo;
//            this.accessCodeRepository = accessCodeRepository;
//        }

//        #endregion Public Constructors

//        #region Public Methods

//        public async Task<(bool isSuccess, string message)> BulkDeleteAsync(List<int> tcs)
//        {

//            if (tcs == null || !tcs.Any())
//                return (false, "Validation Failed");

//            const int batchSize = 500;

//            await depRepo.BeginTransactionAsync();
//            try
//            {
//                for (int i = 0; i < tcs.Count; i += batchSize)
//                {
//                    var batch = tcs.Skip(i).Take(batchSize).ToList();
//                    var entries = await depRepo.All()
//                        .Where(e => batch.Contains(e.Tc))
//                        .AsNoTracking()
//                        .ToListAsync();

//                    if (!entries.Any()) continue;

//                    await depRepo.DeleteRangeAsync(entries);
//                }
//                await depRepo.CommitTransactionAsync();
//                return (true, "Deleted Successfully");
//            }
//            catch (Exception ex)
//            {
//                await depRepo.RollbackTransactionAsync();
//                return (false, "Internal Server Error!");
//            }
//        }

//        public async Task<InvBuyerDepartmentViewModel> GetByIdAsync(int id)
//        {
//            try
//            {
//                var dep = await depRepo.GetByIdAsync(id);

//                if (dep == null)
//                {
//                    return null;
//                }

//                var record = new InvBuyerDepartmentViewModel
//                {
//                    Tc = dep.Tc,
//                    BuyerDepartmentId = dep.BuyerDepartmentId,
//                    DepartmentName = dep.DepartmentName,
//                    ShortName = dep.ShortName,

//                    Ldate = dep.Ldate,
//                    ModifyDate = dep.ModifyDate,
//                };

//                return record;

//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        public async Task<(List<InvBuyerDepartmentViewModel> Data, int totalRecord, int curentRecord)> GetPaginatedDataAsync(string searchValue, int page, int pageSize, string sortColumn, string sortDirection)
//        {
//            var query = await (from b in depRepo.All()

//                               select new InvBuyerDepartmentViewModel
//                               {
//                                   Tc = b.Tc,
//                                   BuyerDepartmentId= b.BuyerDepartmentId,
//                                   DepartmentName= b.DepartmentName,
//                                   ShortName= b.ShortName,
//                               })
//                              .ToListAsync();

//            var totalRecord = query.Count();

//            var materializedQuery = query;


//            IEnumerable<InvBuyerDepartmentViewModel> filterQuery = materializedQuery;

//            if (!string.IsNullOrWhiteSpace(searchValue))
//            {
//                filterQuery = filterQuery.Where(d =>
//                    (d.BuyerDepartmentId?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.DepartmentName?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.ShortName?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) 
//                );
//            }

//            var currentRecord = filterQuery.Count();

//            if (!string.IsNullOrWhiteSpace(sortColumn) && !string.IsNullOrWhiteSpace(sortDirection))
//            {
//                filterQuery = sortColumn.ToLower() switch
//                {
//                    "buyerdepartmentid" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.BuyerDepartmentId) : filterQuery.OrderByDescending(x => x.BuyerDepartmentId),
//                    "departmentname" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.DepartmentName) : filterQuery.OrderByDescending(x => x.DepartmentName),
//                    "shortname" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.ShortName) : filterQuery.OrderByDescending(x => x.ShortName),
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

//        public async Task<bool> IsDuplicate(string name, string id = null)
//        {
//            if (string.IsNullOrWhiteSpace(name))
//                return false;

//            return await depRepo.All()
//                .AnyAsync(x => x.DepartmentName.ToLower() == name.ToLower()
//                               && (id == null || x.BuyerDepartmentId != id));
//        }

//        public async Task<(bool isSuccess, string message, object data)> SaveAsync(InvBuyerDepartmentViewModel model)
//        {
//            if (model == null || string.IsNullOrWhiteSpace(model.DepartmentName))
//            {
//                return (false, "Internal server error!", null);
//            }

//            await depRepo.BeginTransactionAsync();

//            try
//            {
//                if (model.Tc == 0)
//                {
//                    if(await IsDuplicate(model.DepartmentName))
//                    {
//                        return (false, "Duplicate Data!", null);
//                    }

//                    var newId = comService.GenerateNextCode("BuyerDepartmentId", "Inv_Def_BuyerDepartment", 3);

//                    InvDefBuyerDepartment record = new InvDefBuyerDepartment
//                    {
//                        BuyerDepartmentId = newId,
//                        DepartmentName = model.DepartmentName,
//                        ShortName = model.ShortName,

//                        Ldate = model.Ldate,
//                        Luser = model.Luser,
//                        Lip = model.Lip,
//                        Lmac = model.Lmac,
//                    };

//                    await depRepo.AddAsync(record);
//                    await depRepo.CommitTransactionAsync();
//                    return (true, "Saved Successfully!", record);
//                }
//                else
//                {
//                    var exData = await depRepo.GetByIdAsync(model.Tc);
//                    if (await IsDuplicate(model.DepartmentName, exData.BuyerDepartmentId))
//                    {
//                        return (false, "Duplicate Data!", null);
//                    }

//                    if (exData == null)
//                        return (false, "Data does not exists!", null);

//                    exData.DepartmentName = model.DepartmentName;
//                    exData.ShortName = model.ShortName;

//                    exData.ModifyDate = model.ModifyDate;
//                    exData.Luser = model.Luser;
//                    exData.Lip = model.Lip;
//                    exData.Lmac = model.Lmac;

//                    await depRepo.UpdateAsync(exData);
//                    await depRepo.CommitTransactionAsync();
//                    return (true, "Update Successfully", exData);
//                }
//            }
//            catch (Exception ex)
//            {
//                await depRepo?.RollbackTransactionAsync();

//                return (false, "Internal Server Error!", null);
//            }
//        }

//        public async Task<IEnumerable<CommonSelectModel>> SelectionBuyerDepAsync()
//        {

//            var data = await depRepo.All()
//                       .Select(x => new CommonSelectModel
//                       {
//                           Code = x.BuyerDepartmentId,
//                           Name = x.DepartmentName,
//                       }).ToListAsync();
//            return data;
//        }


//        #region Permission all type
//        public async Task<bool> PagePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Buyer Department" && x.TitleCheck);
//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Buyer Department" && x.CheckAdd);
//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Buyer Department" && x.CheckEdit);
//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Buyer Department" && x.CheckDelete);
//        }
//        #endregion

//        #endregion Public Methods
//    }
//}
