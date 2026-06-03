using DocumentFormat.OpenXml.Office2010.PowerPoint;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRMTransportAssignEntry;
using GCTL.Core.ViewModels.HRMTransportExpenseEntry;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRMTransportExpenseEntryService
{
    public class HRMTransportExpenseEntryServicec :AppService<HrmTransportExpenseEntry> , IHRMTransportExpenseEntryServicec
    {
        private readonly IRepository<HrmTransportExpenseEntry> transportExpenseRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> offiEmpRepo;
        private readonly IRepository<HrmEmployee> empRepo;
        private readonly IRepository<HrmDefDepartment> depRepo;
        private readonly IRepository<HrmDefDesignation> desiRepo;
        private readonly IRepository<SalesDefVehicle> transportRepo;
        private readonly IRepository<SalesDefVehicleType> transportTypeRepo;
        private readonly IRepository<HrmTransportExpenseDetailsTemp> transportExpenseTempDetailsRepo;
        private readonly IRepository<SalesDefTransportExpenseHead> expenseRepo;
        private readonly IRepository<HrmTransportAssignEntry> transportEntryRepo;
        private readonly IRepository<HrmTransportExpenseDetails> transportExpenseDetailsRepo;
        private readonly IRepository<CoreCompany> comrepo;
        private readonly IDeleteHistoryService deleteHistoryService;

        public HRMTransportExpenseEntryServicec(
            IRepository<HrmTransportExpenseEntry> transportExpenseRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
             IRepository<HrmEmployeeOfficialInfo> offiEmpRepo,
            IRepository<HrmEmployee> empRepo,
            IRepository<HrmDefDepartment> depRepo,
            IRepository<HrmDefDesignation> desiRepo,
            IRepository<SalesDefVehicle> transportRepo,
           IRepository<SalesDefVehicleType> transportTypeRepo,
           IRepository<HrmTransportExpenseDetailsTemp> transportExpenseTempDetailsRepo,
           IRepository<SalesDefTransportExpenseHead> expenseRepo,
           IRepository<HrmTransportAssignEntry> transportEntryRepo,
           IRepository<HrmTransportExpenseDetails> transportExpenseDetailsRepo,
           IRepository<CoreCompany> comrepo,
           IDeleteHistoryService deleteHistoryService

            ) : base(transportExpenseRepo)
        {
            this.transportExpenseRepo = transportExpenseRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.offiEmpRepo = offiEmpRepo;
            this.empRepo = empRepo;
            this.depRepo = depRepo;
            this.desiRepo = desiRepo;
            this.transportRepo = transportRepo;
            this.transportTypeRepo = transportTypeRepo;
            this.transportExpenseTempDetailsRepo = transportExpenseTempDetailsRepo;
            this.expenseRepo = expenseRepo;
            this.transportEntryRepo = transportEntryRepo;
            this.transportExpenseDetailsRepo = transportExpenseDetailsRepo;
            this.comrepo = comrepo;
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

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "HRM Transport Assign" && x.TitleCheck);

        }

        public async Task<bool> SavePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "HRM Transport Assign" && x.CheckAdd);

        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "HRM Transport Assign" && x.CheckEdit);

        }

        public async Task<bool> DeletePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "HRM Transport Assign" && x.CheckDelete);

        }

        #endregion

        public async Task<List<TransportMasterDto>> GetAllAsync()
        {
            try
            {
                var result = await (
                    from tee in transportExpenseRepo.All()
                    join sdv in transportRepo.All() on tee.Tedid equals sdv.VehicleId into sdvJoin
                    from sdv in sdvJoin.DefaultIfEmpty()

                    join tae in transportEntryRepo.All() on sdv.VehicleId equals tae.TransportNoId into taeJoin
                    from tae in taeJoin.DefaultIfEmpty()

                    join emp in empRepo.All() on tae.EmployeeId equals emp.EmployeeId into empJoin
                    from emp in empJoin.DefaultIfEmpty()

                    select new TransportMasterDto
                    {
                        AutoId = tee.AutoId,
                        TEID = tee.Teid,
                        TEDate = tee.Tedate.HasValue ? tee.Tedate.Value.ToString("dd/MM/yyyy") : "",
                        VehicleNo = sdv != null ? sdv.VehicleNo : null,
                        FullName = emp != null ? emp.FirstName + " " + emp.LastName : null,
                        Telephone = emp != null ? emp.Telephone : null,
                        CompanyCode = comrepo.All()
                                            .Where(x => x.CompanyCode == tee.CompanyCode)
                                            .Select(x => x.CompanyCode)
                                            .FirstOrDefault(),
                        CompanyName = comrepo.All()
                                             .Where(x => x.CompanyCode == tee.CompanyCode)
                                             .Select(x => x.CompanyName)
                                             .FirstOrDefault(),
                    })
                    .Distinct()
                    .ToListAsync();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<HRMTransportExpenseEntrySetupViewModel> GetByIdAsync(string id)
        {
            try
            {
                var entity = transportExpenseRepo.All().Where(x => x.Teid == id).FirstOrDefault();
                if (entity == null) return null;

                var transportExpenseDetailsData = transportExpenseDetailsRepo.All().Where(x => x.Teid == id).ToList();
                //var transportExpenseTempDetailsData = transportExpenseTempDetailsRepo.All().Where(x => x.Teid == id).ToList();
                //if (transportExpenseTempDetailsData != null || transportExpenseTempDetailsData.Count > 0)
                //{
                //    await transportExpenseTempDetailsRepo.DeleteRangeAsync(transportExpenseTempDetailsData);
                //}
                 await ReloadDataBackTempToDetailsAsync();

                    if (transportExpenseDetailsData!= null || transportExpenseDetailsData.Count>0)
                {
                    foreach (var item in transportExpenseDetailsData)
                    {
                        var entityTemp = new HrmTransportExpenseDetailsTemp
                        {
                            Teid = item.Teid,
                            Tedid = item.Tedid,
                            ExpenseHeadId = item.ExpenseHeadId,
                            Amount = item.Amount,
                            Remarks = item.Remarks,
                            Luser = item.Luser,
                        };
                        await transportExpenseTempDetailsRepo.AddAsync(entityTemp);                        
                    }
                    await transportExpenseDetailsRepo.DeleteRangeAsync(transportExpenseDetailsData);
                }
                return new HRMTransportExpenseEntrySetupViewModel
                {
                    AutoId = entity.AutoId,
                    TEDID = entity.Tedid,
                    TEID = entity.Teid,
                    TEDate=entity.Tedate,  
                    ShowCreateDate = entity.Ldate.HasValue ? entity.Ldate.Value.ToString("dd/MM/yyyy") : "",
                    ShowModifyDate = entity.ModifyDate.HasValue ? entity.ModifyDate.Value.ToString("dd/MM/yyyy") : "",
                };
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(
       HRMTransportExpenseEntrySetupViewModel model,
       string companyCode,
       string employeeId)
        {
            try
            {
                if (model.AutoId == 0)
                {
                    //var exTransportExpenseTemp = new List<TransportExpenseDetailsTempDto>();
                    var exTransportExpenseTemp = transportExpenseTempDetailsRepo.All().ToList();

                    if (exTransportExpenseTemp.Count <= 0)
                    {
                        return (false , CreateFailed, null);
                    }

                    if (model.TEDID != null && model.TEDate != null)
                    {                      

                        foreach (var item in exTransportExpenseTemp)
                        {

                            var entityDetails = new HrmTransportExpenseDetails
                            {
                                Tedid=item.Tedid,
                                Teid=item.Teid,                                
                                Luser = item.Luser,
                                Amount= item.Amount,
                                Remarks = item.Remarks,
                                ExpenseHeadId=item.ExpenseHeadId
                            };
                            await transportExpenseDetailsRepo.AddAsync(entityDetails);
                            //await transportExpenseRepo.AddAsync(entity);
                        }
                        var entityMaster = new HrmTransportExpenseEntry
                        {
                            Teid = model.TEID,
                            Tedate = model.TEDate,
                            Tedid = model.TEDID,
                            Luser = model.Luser,
                            Ldate = model.Ldate,
                            Lip = model.Lip,
                            Lmac = model.Lmac,
                            CompanyCode = companyCode,
                            EntryUserEmployeeId = employeeId
                        };
                        await transportExpenseRepo.AddAsync(entityMaster);
                        
                        await transportExpenseTempDetailsRepo.DeleteRangeAsync(exTransportExpenseTemp);

                        return (true, CreateSuccess, null);
                    }

                    return (false, CreateFailed, null);
                }
                else // Update
                {
                    var exData = await transportExpenseRepo.GetByIdAsync(model.AutoId);
                    if (exData != null) //todo
                    {
                       await ReloadDataBackTempToDetailsAsync();
                        
                        exData.Teid = model.TEID;
                        exData.Tedid = model.TEDID;
                        exData.Ldate = model.Ldate;
                        exData.Tedate = model.TEDate;
                        exData.ModifyDate= DateTime.Now;

                        await transportExpenseRepo.UpdateAsync(exData);
                        return (true, UpdateSuccess, exData);
                    }

                    return (false, UpdateFailed, null);
                }
            }
            catch (Exception ex)
            {
                return (false, CreateFailed, null);
            }
        }


        public async Task<(bool isSuccess, string message, object data)> DeleteAsync(List<decimal> ids, DeleteHistoryViewModel dmodel)
        {
            if (ids == null || !ids.Any())
            {
                return (false, DeleteFailed, null);
            }

            await transportExpenseRepo.BeginTransactionAsync();
            try
            {
                var entitiesToDelete = await transportExpenseRepo.All()
                    .Where(x => ids.Contains(x.AutoId))
                    .ToListAsync();

                if (entitiesToDelete == null || entitiesToDelete.Count == 0)
                {
                    await transportExpenseRepo.RollbackTransactionAsync();
                    return (false, DeleteFailed, null);
                }

                var teids = entitiesToDelete.Select(x => x.Teid).ToList();

                var tempDetailsToDelete = await transportExpenseTempDetailsRepo.All()
                    .Where(x => teids.Contains(x.Teid))
                    .ToListAsync();

                if (tempDetailsToDelete.Any())
                {
                    await transportExpenseTempDetailsRepo.DeleteRangeAsync(tempDetailsToDelete);
                    dmodel.tableName = transportExpenseTempDetailsRepo.GetTableName();
                    await deleteHistoryService.LogDeletedRecordsAsync(tempDetailsToDelete, dmodel);
                }

                var masterDetailsToDelete = await transportExpenseDetailsRepo.All()
                    .Where(x => teids.Contains(x.Teid))
                    .ToListAsync();

                if (masterDetailsToDelete.Any())
                {
                    await transportExpenseDetailsRepo.DeleteRangeAsync(masterDetailsToDelete);
                    dmodel.tableName = transportExpenseDetailsRepo.GetTableName();
                    await deleteHistoryService.LogDeletedRecordsAsync(masterDetailsToDelete, dmodel);
                }

                await transportExpenseRepo.DeleteRangeAsync(entitiesToDelete);
                dmodel.tableName = transportExpenseRepo.GetTableName();
                await deleteHistoryService.LogDeletedRecordsAsync(entitiesToDelete, dmodel);

                await transportExpenseRepo.CommitTransactionAsync();
                return (true, DeleteSuccess, null);
            }
            catch (Exception)
            {
                await transportExpenseRepo.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<string> AutoIdAsync()
        {
            try
            {
                var transportExpenseEntries = (await transportExpenseRepo.GetAllAsync()).ToList();

                int newTransportExpenseEntryId;

                if (transportExpenseEntries != null && transportExpenseEntries.Count > 0)
                {
                    var lastTransportExpenseEntryId = transportExpenseEntries
                        .OrderByDescending(x => x.AutoId)
                        .Select(x => x.Teid)
                        .FirstOrDefault();

                    int lastNumber = 0;

                    if (!string.IsNullOrEmpty(lastTransportExpenseEntryId))
                    {
                        var parts = lastTransportExpenseEntryId.Split('_');
                        if (parts.Length == 3)
                        {
                            int.TryParse(parts[2], out lastNumber);
                        }
                    }

                    newTransportExpenseEntryId = lastNumber + 1;
                }
                else
                {
                    newTransportExpenseEntryId = 1;
                }

                string prefix = "TA";
                string year = DateTime.Now.Year.ToString();

                return $"{prefix}_{year}_{newTransportExpenseEntryId:D6}";
            }
            catch (Exception)
            {

                throw;
            }
            
        }


        public async Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string VehicleTypeValue)
        {
            bool Exists = transportExpenseRepo.All().Any(x => x.Teid == VehicleTypeValue);
            return (Exists, DataExists, null);
        }

        public async Task<HRMTransportDetails> GetEmpDetailsIdAsync(string emp)
        {
            try
            {
                var queary = from empOffi in offiEmpRepo.All()
                             join ep in empRepo.All() on empOffi.EmployeeId equals ep.EmployeeId into empJoin
                             from ep in empJoin.DefaultIfEmpty()
                             join dp in depRepo.All() on empOffi.DepartmentCode equals dp.DepartmentCode into dpJoin
                             from dp in dpJoin.DefaultIfEmpty()
                             join desi in desiRepo.All() on empOffi.DesignationCode equals desi.DesignationCode into desiJoin
                             from desi in desiJoin.DefaultIfEmpty()
                             where ep.EmployeeId == emp
                             select new HRMTransportDetails
                             {
                                 EmpId = ep.EmployeeId,
                                 EmpName = ep.FirstName + " " + ep.LastName,
                                 Department = dp.DepartmentName ?? "",
                                 Designation = desi.DesignationName ?? "",
                                 Phone = ep.Telephone ?? ""
                             };
                return await queary.FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<(bool isSuccess, string message, object data)> TransportExpenseDetailsAsync(TransportExpenseDetailsTempDto model)
        {
            try
            {
                if (model.AutoId == 0)
                {
                    if (!decimal.TryParse(model.Amount, out var amount) || amount <= 0)
                    {
                        return (false, "Invalid Amount", null);
                    }

                    if (!int.TryParse(model.ExpenseHeadId, out var expenseHeadId) || expenseHeadId <= 0)
                    {
                        return (false, "Invalid Expense Head", null);
                    }
                    var entity = new HrmTransportExpenseDetailsTemp
                    {
                        Teid=model.Teid,
                        Tedid=model.Tedid,
                        ExpenseHeadId = model.ExpenseHeadId,
                        Amount = model.Amount,
                        Remarks = model.Remarks,
                        Luser = model.Luser,
                    };
                   await transportExpenseTempDetailsRepo.AddAsync(entity);                   
                    return (true, CreateSuccess, entity);
                }
                else
                {
                    var detailsData = await transportExpenseTempDetailsRepo.GetByIdAsync(model.AutoId);
                    if (detailsData != null) { 
                        detailsData.Teid = model.Teid;
                        detailsData.Tedid = model.Tedid;
                        detailsData.Luser = model.Luser;
                        detailsData.Amount = model.Amount;
                        detailsData.Remarks = model.Remarks;
                        detailsData.ExpenseHeadId = model.ExpenseHeadId;

                        await transportExpenseTempDetailsRepo.UpdateAsync(detailsData);
                        return (true, UpdateSuccess, detailsData);
                    }

                    return (false, UpdateFailed, null);
                }
            }
            catch (Exception)
            {
              
                throw;
            }
        }

        public async Task<List<TransportExpenseDetailsTempDto>> TransportExpenseTempDetailsListAsny()
        {
            try
            {
                var data = transportExpenseTempDetailsRepo.All()
                    .Select(x => new TransportExpenseDetailsTempDto
                    {
                        AutoId = x.AutoId,
                        ExpenseHeadId = x.ExpenseHeadId,
                        ExpenseHead = expenseRepo.All().Where(c=> c.ExpenseHeadId == x.ExpenseHeadId).Select(s=> s.ExpenseHead).FirstOrDefault()??"", 
                        Amount = x.Amount,
                        Remarks = x.Remarks
                    }).ToList();

                return await Task.FromResult(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TransportExpenseDetailsTempDto> TransportExpenseTempDetailsByIdAsny(decimal id)
        {
            try
            {
                var exData = await transportExpenseTempDetailsRepo.GetByIdAsync(id);
                if (exData == null)
                {
                    return null;
                }
                var dto = new TransportExpenseDetailsTempDto
                {
                    AutoId = exData.AutoId,                   
                    ExpenseHead = await expenseRepo.All()
                        .Where(c => c.ExpenseHeadId == exData.ExpenseHeadId)
                        .Select(s => s.ExpenseHead)
                        .FirstOrDefaultAsync() ?? "",
                    ExpenseHeadId = exData.ExpenseHeadId,
                    Amount = exData.Amount,
                    Remarks = exData.Remarks,
                    Teid= exData.Teid,
                    Tedid=exData.Tedid
                };

                return dto;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<HRMTransportExpenseEntrySetupViewModel> TransportExpenseMasterDetailsByIdAsny(decimal id)
        {
            try
            {
                var exData = await transportExpenseRepo.GetByIdAsync(id);

                if (exData == null)
                {
                    return null;
                }

                var dto = new HRMTransportExpenseEntrySetupViewModel
                {
                    AutoId = exData.AutoId,
                    TEID = exData.Teid,
                    TEDID = exData.Tedid,
                    TEDate = exData.Tedate,
                    ShowCreateDate= exData.Ldate.HasValue? exData.Ldate.Value.ToString("dd/MM/yyyy"):"",
                    ShowModifyDate = exData.ModifyDate.HasValue? exData.ModifyDate.Value.ToString("dd/MM/yyyy") :"",
                };

                return dto;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<(bool isSuccess, string message, object data)> DeleteTransportExpenseAsync(decimal id)
        {
            try
            {
                var tempExpense = await transportExpenseTempDetailsRepo.GetByIdAsync(id);
                if (tempExpense == null) {
                      return (false, DeleteFailed, null);
                }
                await transportExpenseTempDetailsRepo.DeleteAsync(tempExpense);
                return (true, DeleteFailed, null);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<TransportDetailsDto>> GetAllTransportDetailsAsync(string trnsId)
        {
            try
            {
                // Solution 1: Split into two queries
                var transportData = await (
                    from sdv in transportRepo.All()
                    join sdvt in transportTypeRepo.All()
                        on sdv.VehicleTypeId equals sdvt.VehicleTypeId into vtGroup
                    from sdvt in vtGroup.DefaultIfEmpty()
                    join tae in transportEntryRepo.All()
                        on sdv.VehicleId equals tae.TransportNoId into taeGroup
                    from tae in taeGroup.DefaultIfEmpty()
                    join emp in empRepo.All()
                        on tae.EmployeeId equals emp.EmployeeId into empGroup
                    from emp in empGroup.DefaultIfEmpty()
                    join emp2 in empRepo.All()
                        on tae.TransportUser equals emp2.EmployeeId into emp2Group
                    from emp2 in emp2Group.DefaultIfEmpty()
                    where (string.IsNullOrEmpty(trnsId) || sdv.VehicleId == trnsId)
                    select new
                    {
                        VehicleId = sdv.VehicleId,
                        VehicleNo = sdv.VehicleNo,
                        VehicleTypeId = sdv.VehicleTypeId,
                        VehicleType = sdvt.VehicleType,
                        DriverName = emp.FirstName + " " + emp.LastName,
                        DriverPhone = emp.Telephone,
                        EmployeeId = emp.EmployeeId,
                        EmployeeName = emp.FirstName + " " + emp.LastName,
                        UserEmpId = emp2.EmployeeId,
                        UserFullName = emp2.FirstName + " " + emp2.LastName+" ("+emp2.EmployeeId+")" 
                    }
                ).ToListAsync();

                // Group in memory and create final result
                var result = transportData
                    .GroupBy(x => new
                    {
                        x.VehicleId,
                        x.VehicleNo,
                        x.VehicleTypeId,
                        x.VehicleType,
                        x.DriverName,
                        x.DriverPhone,
                        x.EmployeeId,
                    })
                    .Select(g => new TransportDetailsDto
                    {
                        TransportID = g.Key.VehicleId.ToString(),
                        TransportNo = g.Key.VehicleNo,
                        TransportTypeID = g.Key.VehicleTypeId.ToString(),
                        TransportType = g.Key.VehicleType,
                        DriverId = g.Key.EmployeeId,
                        DriverName = g.Key.DriverName,
                        DriverPhone = g.Key.DriverPhone,
                        TransportUsers = g
                            .Where(x => x.UserEmpId != null) 
                            .Select(x => new TransportUserDto
                            {
                                UserId = x.UserEmpId.ToString(),
                                UserName = x.UserFullName 
                            })
                            .Distinct()
                            .ToList()
                    })
                    .ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool isSuccess, object data)> ReloadDataBackTempToDetailsAsync()
        {
            try
            {
                var listOfTempdata = transportExpenseTempDetailsRepo.All().ToList();
                var listOfMaster = transportExpenseRepo.All().ToList();
                if (listOfTempdata != null && listOfTempdata.Count > 0)
                {
                    foreach (var item in listOfTempdata)
                    {
                        if (listOfMaster.Any(m => m.Teid == item.Teid))
                        {
                            var entityTemp = new HrmTransportExpenseDetails
                            {
                                Teid = item.Teid,
                                Tedid = item.Tedid,
                                ExpenseHeadId = item.ExpenseHeadId,
                                Amount = item.Amount,
                                Remarks = item.Remarks,
                                Luser = item.Luser,
                            };
                            await transportExpenseDetailsRepo.AddAsync(entityTemp);
                        }
                        
                    }
                    await transportExpenseTempDetailsRepo.DeleteRangeAsync(listOfTempdata);
                    return (true, listOfTempdata);
                }
                return (false, null);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
