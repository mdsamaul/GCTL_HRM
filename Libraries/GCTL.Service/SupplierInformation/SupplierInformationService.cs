//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.Common;
//using GCTL.Core.ViewModels.SupplierInformation;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace GCTL.Service.SupplierInformation
//{
//    public class SupplierInformationService : AppService<RmgDefSupplier>, ISupplierInformationService
//    {
//        #region Service & repository  
//        private readonly IRepository<RmgDefSupplier> supplierInformationRepository;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        private readonly ICommonService commonService;
//        private readonly IRepository<InvDefSupplierOrigin> supplierInformationOriginRepository;
//        private readonly IRepository<InvDefSupplierType> supplierInformationTypeRepository;
//        private readonly IRepository<InvDefSupplierCategory> supplierInformationCategoryRepository;
//        private readonly IRepository<InvDefCompanyInfo> supplierInformationCoreCompanyRepository;
//        private readonly IRepository<CaDefCountry> caDefCountryRepository;
//        private readonly IRepository<SalesContactPerson> salesContactPersonRepository;
//        private readonly IRepository<SalesDefBankInfo> salesDefBankInfoRepository;
//        private readonly IRepository<SalesDefBankBranchInfo> salesDefBankBranchInfoRepository;
//        private readonly IRepository<CoreBankAccountInformation> salesDefBankAccountInformationRepository;
//        private readonly IRepository<InvDefSalesPerson> invDefSalesPersonRepository;
//        private readonly IRepository<SalesSupplierBankAccountTemp> salesSupplierBankAccountTempRepo;
//        private readonly IRepository<SalesSupplierBankAccount> salesSupplierBankAccountRepo;
//        string strMaxNO = string.Empty;

//        private const string TableName = "RMG_Def_Supplier";
//        private const string ColumnName = "SupplierId";



//        public SupplierInformationService(
//            IRepository<RmgDefSupplier> supplierInformationRepository,
//            IRepository<CoreAccessCode> accessCodeRepository,
//            ICommonService commonService,
//            IRepository<InvDefSupplierOrigin> supplierInformationOriginRepository, 
//            IRepository<InvDefSupplierType> supplierInformationTypeRepository,
//            IRepository<InvDefSupplierCategory> supplierInformationCategoryRepository,
//            IRepository<InvDefCompanyInfo> supplierInformationCoreCompanyRepository,
//            IRepository<CaDefCountry> caDefCountryRepository,
//            IRepository<SalesContactPerson> salesContactPersonRepository,
//            IRepository<SalesDefBankInfo> salesDefBankInfoRepository,
//            IRepository<SalesDefBankBranchInfo> salesDefBankBranchInfoRepository,
//            IRepository<CoreBankAccountInformation> salesDefBankAccountInformationRepository,
//            IRepository<InvDefSalesPerson> invDefSalesPersonRepository,
//            IRepository<SalesSupplierBankAccountTemp> salesSupplierBankAccountTempRepo,
//            IRepository<SalesSupplierBankAccount> salesSupplierBankAccountRepo

//            ) 

//    : base(supplierInformationRepository)
//        {
//            this.supplierInformationRepository = supplierInformationRepository;
//            this.accessCodeRepository = accessCodeRepository;
//            this.commonService = commonService;
//            this.supplierInformationOriginRepository = supplierInformationOriginRepository;
//            this.supplierInformationTypeRepository = supplierInformationTypeRepository;
//            this.supplierInformationCategoryRepository = supplierInformationCategoryRepository;
//            this.supplierInformationCoreCompanyRepository = supplierInformationCoreCompanyRepository;
//            this.caDefCountryRepository = caDefCountryRepository;
//            this.salesContactPersonRepository = salesContactPersonRepository;
//            this.salesDefBankInfoRepository = salesDefBankInfoRepository;
//            this.salesDefBankBranchInfoRepository = salesDefBankBranchInfoRepository;
//            this.salesDefBankAccountInformationRepository = salesDefBankAccountInformationRepository;
//            this.invDefSalesPersonRepository = invDefSalesPersonRepository;
//            this.salesSupplierBankAccountTempRepo = salesSupplierBankAccountTempRepo;
//            this.salesSupplierBankAccountRepo = salesSupplierBankAccountRepo;
//        }

//        #endregion

//        #region GetAllAsync

//        public async Task<List<SupplierInformationSetupViewModel>> GetAllAsync()
//        {
//            var data = await (
//                from sup in supplierInformationRepository.All().AsNoTracking()

//                join cat in supplierInformationCategoryRepository.All().AsNoTracking()
//                on sup.SupplierCategoryId equals cat.SupplierCategoryId into catJoin
//                from cat in catJoin.DefaultIfEmpty()

//                join typ in caDefCountryRepository.All().AsNoTracking()
//                on sup.CountryId equals typ.CountryId into typJoin
//                from typ in typJoin.DefaultIfEmpty()

//                join org in supplierInformationOriginRepository.All().AsNoTracking()
//                on sup.SupplierOriginId equals org.SupplierOriginId into orgJoin
//                from org in orgJoin.DefaultIfEmpty()

//                join sal in invDefSalesPersonRepository.All().AsNoTracking()
//                on sup.SalesPersonId equals sal.SalesPersonId into supJoin
//                from sal in supJoin.DefaultIfEmpty()

//                select new SupplierInformationSetupViewModel
//                {
//                    Tc = sup.Tc,
//                    SupplierId = sup.SupplierId,
//                    SupplierName = sup.SupplierName,
//                    SupplierCategory = cat.SupplierCategory,
//                    SupplierOrigin = org.SupplierOrigin,
//                    CountryName = typ.CountryName,
//                    SalesPersonName = sal.SalesPerson,
//                    Luser = sup.Luser,
//                    Ldate = sup.Ldate
//                }
//            ).ToListAsync();

//            return data;
//        }

//        #endregion

//        #region GetAllAsync BankInfo

//        public async Task<List<SalesSupplierBankAccountTempDto>> GetTableBankAccountInfoDataAsync()
//        {
//            try
//            {
//                var entities = await salesSupplierBankAccountTempRepo.All()
//                    .Select(bankAccount => new SalesSupplierBankAccountTempDto
//                    {
//                        AutoId = bankAccount.AutoId,
//                        Sbaid = bankAccount.Sbaid,
//                        SupplierId = bankAccount.SupplierId,
//                        BankId = bankAccount.BankId,
//                        BankBranchId = bankAccount.BankBranchId,
//                        AccountName = bankAccount.AccountName,
//                        Luser = bankAccount.Luser,
//                        BankName = salesDefBankInfoRepository.All().Where(x=> x.BankId== bankAccount.BankId).Select(s=> s.BankName).FirstOrDefault(),
//                        BankBranchName = salesDefBankBranchInfoRepository.All().Where(x => x.BankBranchId == bankAccount.BankBranchId).Select(s => s.BankBranchName).FirstOrDefault()
//                    }).OrderBy(x=>x.Sbaid).ToListAsync();

//                return entities;
//            }
//            catch (Exception)
//            {
//                // Optionally log the exception
//                return new List<SalesSupplierBankAccountTempDto>();
//            }
//        }

//        #endregion

//        #region GetByIdAsync

//        public async Task<SupplierInformationSetupViewModel> GetByIdAsync(string code)
//        {
//            var data = await (
//                from sup in supplierInformationRepository.All().AsNoTracking()

//                    // Filter by Supplier Code
//                where sup.SupplierId == code

//                // Join with Supplier Category
//                join cat in supplierInformationCategoryRepository.All().AsNoTracking()
//                on sup.SupplierCategoryId equals cat.SupplierCategoryId into catJoin
//                from cat in catJoin.DefaultIfEmpty()

//                    // Join with Supplier Type
//                join typ in supplierInformationTypeRepository.All().AsNoTracking()
//                on sup.SupplierTypeId equals typ.SupplierTypeId into typJoin
//                from typ in typJoin.DefaultIfEmpty()

//                    // Join with Supplier Origin
//                join org in supplierInformationOriginRepository.All().AsNoTracking()
//                on sup.SupplierOriginId equals org.SupplierOriginId into orgJoin
//                from org in orgJoin.DefaultIfEmpty()

//                    // Join with Company
//                join com in supplierInformationCoreCompanyRepository.All().AsNoTracking()
//                on sup.CompanyId equals com.CompanyCode into comJoin
//                from com in comJoin.DefaultIfEmpty()

//                    // Join with Country
//                join ctry in caDefCountryRepository.All().AsNoTracking()
//                on sup.CountryId equals ctry.CountryId into countryJoin
//                from ctry in countryJoin.DefaultIfEmpty()

//                    // Join with Sales Contact Person
//                join sp in salesContactPersonRepository.All().AsNoTracking()
//                on sup.ContatPerson1 equals sp.Cpid into spJoin
//                from sp in spJoin.DefaultIfEmpty()

//                    // Join with Bank Info
//                join bank in salesDefBankInfoRepository.All().AsNoTracking()
//                on sup.SupplierBankId equals bank.BankId into bankJoin
//                from bank in bankJoin.DefaultIfEmpty()

//                    // Join with Bank Branch
//                join branch in salesDefBankBranchInfoRepository.All().AsNoTracking()
//                on sup.SupplierBankBranchId equals branch.BankBranchId into branchJoin
//                from branch in branchJoin.DefaultIfEmpty()

//                    // Join with Bank Account Information
//                join acc in salesDefBankAccountInformationRepository.All().AsNoTracking()
//                on sup.AccountNo equals acc.AccountNo into accJoin
//                from acc in accJoin.DefaultIfEmpty()

//                select new SupplierInformationSetupViewModel
//                {
//                    // Supplier main info
//                    Tc = sup.Tc,
//                    SupplierId = sup.SupplierId,
//                    SupplierCode = sup.SupplierCode,
//                    SupplierName = sup.SupplierName,
//                    Address = sup.Address,
//                    LocalOfficeAddress = sup.LocalOfficeAddress,
//                    City = sup.City,
//                    State = sup.State,
//                    Phone = sup.Phone,
//                    Fax = sup.Fax,
//                    Email = sup.Email,
//                    Url = sup.Url,
//                    Bin = sup.Bin,
//                    VatregNo = sup.VatregNo,
//                    SupplierTin = sup.SupplierTin,
//                    OpeningBalance = sup.OpeningBalance,
//                    Optype = sup.Optype,
//                    Remarks = sup.Remarks,
//                    Active = sup.Active,
//                    ZipCode = sup.ZipCode,
//                    ExportLicenceNo = sup.ExportLicenceNo,
//                    ContatPerson1 = sup.ContatPerson1,

//                    // Category, Type, Origin
//                    SupplierCategoryId = sup.SupplierCategoryId,
//                    SupplierCategory = cat.SupplierCategory,
//                    SupplierTypeId = sup.SupplierTypeId,
//                    SupplierTypeName = typ.SupplierTypeName,
//                    SupplierOriginId = sup.SupplierOriginId,
//                    SupplierOrigin = org.SupplierOrigin,

//                    // Company and Country
//                    CompanyId = sup.CompanyId,
//                    CompanyName = com.CompanyName,
//                    CountryId = ctry.CountryId,
//                    CountryName = ctry.CountryName,

//                    // Bank Information
//                    SupplierBankId = sup.SupplierBankId,
//                    SupplierBankName = bank.BankName,
//                    SupplierBankBranchId = sup.SupplierBankBranchId,
//                    SupplierBankBranchName = branch.BankBranchName,
//                    AccountNo = sup.AccountNo,                   

//                    // Sales Contact Person
//                    SalesPersonId = sup.SalesPersonId,
//                    ContatPersonName = sp.ContactPersonName,
//                    SalesPersonName = sup.SalesPersonId,

//                    // Audit Information
//                    Luser = sup.Luser,
//                    Ldate = sup.Ldate,
//                    ModifyDate = sup.ModifyDate
//                }
//            ).FirstOrDefaultAsync();

//            var tempBankAccounts = await salesSupplierBankAccountTempRepo.All().ToListAsync();

//             if (tempBankAccounts.Any())
//             {
//                // Get last Sbaid in main table
//                var lastSbaidMainStr = await salesSupplierBankAccountRepo.All().OrderByDescending(x => x.Sbaid).Select(c => c.Sbaid).FirstOrDefaultAsync();

//                int lastSbaMainId = 0;
//                if (!string.IsNullOrEmpty(lastSbaidMainStr)) int.TryParse(lastSbaidMainStr, out lastSbaMainId);

//                var mainAccounts = tempBankAccounts.Select(bank =>
//                {
//                    lastSbaMainId++;
//                    return new SalesSupplierBankAccount
//                    {
//                        SupplierId = bank.SupplierId,
//                        BankId = bank.BankId,
//                        BankBranchId = bank.BankBranchId,
//                        AccountName = bank.AccountName,
//                        Luser = bank.Luser,
//                        Sbaid = lastSbaMainId.ToString("D3")
//                    };
//                }).ToList();

//                await salesSupplierBankAccountRepo.AddRangeAsync(mainAccounts);
//                await salesSupplierBankAccountTempRepo.DeleteRangeAsync(tempBankAccounts);
//             }

//            // 2️⃣ Move Main -> Temp (if needed)
//            var mainBankAccounts = await salesSupplierBankAccountRepo.All().Where(x => x.SupplierId == code).ToListAsync();

//            if (mainBankAccounts.Any())
//            {
//                // Get last Sbaid in temp table
//                var lastSbaidTempStr = await salesSupplierBankAccountTempRepo.All().OrderByDescending(x => x.Sbaid).Select(c => c.Sbaid).FirstOrDefaultAsync();

//                int lastSbaidTemp = 0;
//                if (!string.IsNullOrEmpty(lastSbaidTempStr))int.TryParse(lastSbaidTempStr, out lastSbaidTemp);

//                var tempAccounts = mainBankAccounts.Select(bank =>
//                {
//                    lastSbaidTemp++;
//                    return new SalesSupplierBankAccountTemp
//                    {
//                        SupplierId = bank.SupplierId,
//                        BankId = bank.BankId,
//                        BankBranchId = bank.BankBranchId,
//                        AccountName = bank.AccountName,
//                        Luser = bank.Luser,
//                        Sbaid = lastSbaidTemp.ToString("D3")
//                    };
//                }).ToList();

//                await salesSupplierBankAccountTempRepo.AddRangeAsync(tempAccounts);
//                await salesSupplierBankAccountRepo.DeleteRangeAsync(mainBankAccounts);
//            }
//            return data;
//        }

//        #endregion

//        #region SaveAsync

//        public async Task<bool> SaveAsync(SupplierInformationSetupViewModel entityVM)
//        {
//            await supplierInformationRepository.BeginTransactionAsync();

//            try
//            {
//                // Correct duplicate check: compare entityVM.Email with x.Email
//                bool isExist = await supplierInformationRepository.All()
//                    .AnyAsync(x => x.SupplierName == entityVM.SupplierName
//                                && x.Phone == entityVM.Phone
//                                && x.Email == entityVM.Email);

//                if (isExist)
//                {
//                    await supplierInformationRepository.RollbackTransactionAsync();
//                    return false;
//                }

//                // 1️⃣ Process temporary bank accounts
//                var bankInfoListTemp = await salesSupplierBankAccountTempRepo.All()
//                    .Where(x => x.SupplierId == entityVM.SupplierId)
//                    .ToListAsync();

//                var lastSbaidStr = await salesSupplierBankAccountRepo.All()
//                    .OrderByDescending(x => x.Sbaid)
//                    .Select(c => c.Sbaid)
//                    .FirstOrDefaultAsync();

//                int lastSbaid = 0;
//                if (!string.IsNullOrEmpty(lastSbaidStr))
//                {
//                    int.TryParse(lastSbaidStr, out lastSbaid);
//                }

//                var newBankAccounts = new List<SalesSupplierBankAccount>();
//                foreach (var bank in bankInfoListTemp)
//                {
//                    lastSbaid++;
//                    newBankAccounts.Add(new SalesSupplierBankAccount
//                    {
//                        SupplierId = bank.SupplierId,
//                        BankId = bank.BankId,
//                        BankBranchId = bank.BankBranchId,
//                        AccountName = bank.AccountName,
//                        Luser = bank.Luser,
//                        Sbaid = lastSbaid.ToString("D3")
//                    });
//                }

//                if (newBankAccounts.Any())
//                {
//                    await salesSupplierBankAccountRepo.AddRangeAsync(newBankAccounts);
//                    await salesSupplierBankAccountTempRepo.DeleteRangeAsync(bankInfoListTemp);
//                }

//                // 2️⃣ Add main supplier entity
//                var newEntity = new RmgDefSupplier
//                {
//                    Tc = entityVM.Tc,
//                    SupplierId = entityVM.SupplierId,
//                    SupplierTitle = entityVM.SupplierTitle,
//                    SupplierName = entityVM.SupplierName,
//                    SupplierCode = entityVM.SupplierCode ?? string.Empty,
//                    SupplierCategoryId = entityVM.SupplierCategoryId ?? string.Empty,
//                    SupplierTypeId = entityVM.SupplierTypeId ?? string.Empty,
//                    SupplierOriginId = entityVM.SupplierOriginId ?? string.Empty,
//                    CompanyId = entityVM.CompanyId ?? string.Empty,
//                    Address = entityVM.Address ?? string.Empty,
//                    LocalOfficeAddress = entityVM.LocalOfficeAddress ?? string.Empty,
//                    CountryId = entityVM.CountryId ?? string.Empty,
//                    City = entityVM.City ?? string.Empty,
//                    State = entityVM.State ?? string.Empty,
//                    ZipCode = entityVM.ZipCode ?? string.Empty,
//                    Phone = entityVM.Phone ?? string.Empty,
//                    Fax = entityVM.Fax ?? string.Empty,
//                    Email = entityVM.Email ?? string.Empty,
//                    Url = entityVM.Url ?? string.Empty,
//                    Bin = entityVM.Bin ?? string.Empty,
//                    VatregNo = entityVM.VatregNo ?? string.Empty,
//                    SupplierTin = entityVM.SupplierTin ?? string.Empty,
//                    ExportLicenceNo = entityVM.ExportLicenceNo ?? string.Empty,
//                    ContatPerson1 = entityVM.ContatPerson1 ?? string.Empty,
//                    SupplierBankId = entityVM?.SupplierBankId ?? string.Empty,
//                    SupplierBankBranchId = entityVM?.SupplierBankBranchId ?? string.Empty,
//                    AccountNo = entityVM?.AccountNo ?? string.Empty,
//                    OpeningBalance = entityVM.OpeningBalance ?? 0,
//                    Optype = entityVM.Optype ?? string.Empty,
//                    SalesPersonId = entityVM?.SalesPersonId ?? string.Empty,
//                    Remarks = entityVM?.Remarks ?? string.Empty,
//                    Active = entityVM?.Active ?? string.Empty,
//                    Luser = entityVM.Luser ?? string.Empty,
//                    Ldate = DateTime.Now,
//                    Lip = entityVM.Lip ?? string.Empty,
//                    Lmac = entityVM.Lmac ?? string.Empty,
//                };

//                await supplierInformationRepository.AddAsync(newEntity);
//                await supplierInformationRepository.CommitTransactionAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error message: {ex.Message}");
//                await supplierInformationRepository.RollbackTransactionAsync();
//                return false;
//            }
//        }


//        #endregion

//        #region UpdateAsync

//        public async Task<bool> UpdateAsync(SupplierInformationSetupViewModel entityVM)
//        {
//            await supplierInformationRepository.BeginTransactionAsync();

//            try
//            {
//                // Make sure ID is received
//                if (string.IsNullOrWhiteSpace(entityVM.SupplierId))
//                {
//                    await supplierInformationRepository.RollbackTransactionAsync();
//                    return false;
//                }

//                var entity = await supplierInformationRepository.GetByIdAsync(entityVM.SupplierId);

//                if (entity == null)
//                {
//                    await supplierInformationRepository.RollbackTransactionAsync();
//                    return false;
//                }

//                bool isExist = supplierInformationRepository.GetAll().Any(x => x.SupplierName == entityVM.SupplierName && x.Phone == entityVM.Phone && x.Email == entityVM.Email && x.Tc != entityVM.Tc);
//                if (isExist)
//                {
//                    return false;
//                }
//                // Update fields


//                // 1️⃣ Process temporary bank accounts
//                var bankInfoListTemp = await salesSupplierBankAccountTempRepo.All()
//                    .Where(x => x.SupplierId == entityVM.SupplierId)
//                    .ToListAsync();

//                var lastSbaidStr = await salesSupplierBankAccountRepo.All()
//                    .OrderByDescending(x => x.Sbaid)
//                    .Select(c => c.Sbaid)
//                    .FirstOrDefaultAsync();

//                int lastSbaid = 0;
//                if (!string.IsNullOrEmpty(lastSbaidStr))
//                {
//                    int.TryParse(lastSbaidStr, out lastSbaid);
//                }

//                var newBankAccounts = new List<SalesSupplierBankAccount>();
//                foreach (var bank in bankInfoListTemp)
//                {
//                    lastSbaid++;
//                    newBankAccounts.Add(new SalesSupplierBankAccount
//                    {
//                        SupplierId = bank.SupplierId,
//                        BankId = bank.BankId,
//                        BankBranchId = bank.BankBranchId,
//                        AccountName = bank.AccountName,
//                        Luser = bank.Luser,
//                        Sbaid = lastSbaid.ToString("D3")
//                    });
//                }

//                if (newBankAccounts.Any())
//                {
//                    await salesSupplierBankAccountRepo.AddRangeAsync(newBankAccounts);
//                    await salesSupplierBankAccountTempRepo.DeleteRangeAsync(bankInfoListTemp);
//                }

//                entity.SupplierId = entityVM.SupplierId ?? string.Empty;
//                entity.SupplierName = entityVM.SupplierName ?? string.Empty;
//                entity.SupplierCode = entityVM.SupplierCode ?? string.Empty;
//                entity.SupplierCategoryId = entityVM.SupplierCategoryId ?? string.Empty;
//                entity.SupplierCategoryId = entityVM.SupplierCategoryId ?? string.Empty;
//                entity.SupplierOriginId = entityVM.SupplierOriginId ?? string.Empty;
//                entity.CompanyId = entityVM.CompanyId ?? string.Empty;
//                entity.Address = entityVM.Address ?? string.Empty;
//                entity.LocalOfficeAddress = entityVM.LocalOfficeAddress ?? string.Empty;
//                entity.CountryId = entityVM.CountryId ?? string.Empty;
//                entity.City = entityVM.City ?? string.Empty;
//                entity.State = entityVM.State ?? string.Empty;
//                entity.ZipCode = entityVM.ZipCode ?? string.Empty;
//                entity.Phone = entityVM.Phone ?? string.Empty;
//                entity.Fax = entityVM.Fax ?? string.Empty;
//                entity.Email = entityVM.Email ?? string.Empty;
//                entity.Url = entityVM.Url ?? string.Empty;
//                entity.Bin = entityVM.Bin ?? string.Empty;
//                entity.VatregNo = entityVM.VatregNo ?? string.Empty;
//                entity.SupplierTin = entityVM.SupplierTin ?? string.Empty;
//                entity.ExportLicenceNo = entityVM.ExportLicenceNo ?? string.Empty;
//                entity.ContatPerson1 = entityVM.ContatPerson1 ?? string.Empty;
//                entity.SupplierBankId = entityVM.SupplierBankId ?? string.Empty;
//                entity.SupplierBankBranchId = entityVM.SupplierBankBranchName ?? string.Empty;
//                entity.AccountNo = entityVM.AccountNo ?? string.Empty;
//                entity.OpeningBalance = entityVM.OpeningBalance ?? 0;
//                entity.Optype = entityVM.Optype ?? string.Empty;
//                entity.SalesPersonId = entityVM.SalesPersonId ?? string.Empty;
//                entity.Remarks = entityVM.Remarks;
//                entity.Active = entityVM.Active ?? string.Empty;
//                entity.Luser = entityVM.Luser ?? string.Empty;
//                entity.Lip = entityVM.Lip ?? string.Empty;
//                entity.Lmac = entityVM.Lmac ?? string.Empty;
//                entity.ModifyDate = DateTime.Now;

//                await supplierInformationRepository.UpdateAsync(entity);
//                await supplierInformationRepository.CommitTransactionAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error occurred: {ex.Message}");
//                await supplierInformationRepository.RollbackTransactionAsync();
//                return false;
//            }
//        }

//        #endregion

//        #region SelectionTypeAsync

//        public IEnumerable<CommonSelectModel> SelectionSupplierInformationTypeAsync()
//        {

//            var data = supplierInformationRepository.All()
//                       .Select(x => new CommonSelectModel
//                       {
//                           Code = x.SupplierId,
//                           Name = x.SupplierName,
//                       });
//            return data;
//        }

//        #endregion

//        #region DeleteTab
//        public async Task<bool> DeleteTab(List<string> ids)
//        {
//            var entity = await supplierInformationRepository.All().Where(x => ids.Contains(x.SupplierId)).ToListAsync();

//            if (!entity.Any())
//            {
//                return false;
//            }

//            supplierInformationRepository.Delete(entity);

//            return true;
//        }

//        #endregion

//        #region Duplicate Check 

//        public async Task<bool> IsExistByCodeAsync(string code)
//        {
//            return await supplierInformationRepository.All().AnyAsync(x => x.SupplierName == code);
//        }

//        public async Task<bool> IsExistAsync(string name)
//        {
//            return await supplierInformationRepository.All().AnyAsync(x => x.Phone == name);
//        }

//        public async Task<bool> IsExistAsync(string employeeCode, string phone, string email)
//        {
//            var result = supplierInformationRepository.All().FirstOrDefault(e => e.SupplierName == employeeCode);

//            return await supplierInformationRepository.All().AnyAsync(x => x.SupplierName == employeeCode && x.Phone == phone && x.Email == email);
//        }

//        #endregion

//        #region Permission all type

//        public async Task<bool> PagePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Information" && x.TitleCheck);
//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Information" && x.CheckAdd);
//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Information" && x.CheckEdit);
//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Information" && x.CheckDelete);
//        }

//        #endregion

//        #region Next Id Substring

//        public Task<string> Autoid()
//        {
//            var appList = supplierInformationRepository.All().OrderByDescending(x => x.Tc).FirstOrDefault();

//            int newId = 1;
//            if (appList != null)
//            {
//                string lastId = appList.SupplierId;
//                string numericPart = lastId.Substring(3);
//                if (int.TryParse(numericPart, out int parsedId))
//                {
//                    newId = parsedId + 1;
//                }
//            }

//            string formattedId = $"SUP{newId:D6}";

//            return Task.FromResult(formattedId);
//        }

//        #endregion

//        #region Bank AccountInfo SaveEditAsync

//        public async Task<bool> BankAccountInfoSaveEditAsync(SalesSupplierBankAccountTempDto entityVM)
//        {
//            await salesSupplierBankAccountTempRepo.BeginTransactionAsync();

//            try
//            {
//                if (entityVM.Sbaid == "0")
//                {
//                    bool isExist = await salesSupplierBankAccountTempRepo.All().AnyAsync(x => x.BankId == entityVM.BankId&& x.BankBranchId == entityVM.BankBranchId && x.SupplierId == entityVM.SupplierId);

//                    if (!isExist)
//                    {
//                        var newSbaid = await salesSupplierBankAccountTempRepo.All().OrderByDescending(x => x.Sbaid).Select(c => c.Sbaid).FirstOrDefaultAsync();

//                        var newEntity = new SalesSupplierBankAccountTemp
//                        {
//                            SupplierId = entityVM.SupplierId,
//                            BankId = entityVM.BankId,
//                            BankBranchId = entityVM.BankBranchId,
//                            AccountName = entityVM.AccountName,
//                            Sbaid = (Convert.ToInt32(newSbaid) + 1).ToString("D3"),
//                            Luser = entityVM.Luser ?? string.Empty,
//                        };

//                        await salesSupplierBankAccountTempRepo.AddAsync(newEntity);
//                        await salesSupplierBankAccountTempRepo.CommitTransactionAsync();
//                        return true;
//                    }
//                    else
//                    {
//                        await salesSupplierBankAccountTempRepo.RollbackTransactionAsync();
//                        return false;
//                    }
//                }
//                else
//                {
//                    var exData = await salesSupplierBankAccountTempRepo.GetByIdAsync(entityVM.Sbaid);
//                    if (exData == null)
//                    {
//                        return false;
//                    }
//                    exData.BankId = entityVM.BankId;
//                    exData.BankBranchId = entityVM.BankBranchId;
//                    exData.AccountName = entityVM.AccountName;
//                    exData.Luser = entityVM.Luser;

//                    await salesSupplierBankAccountTempRepo.UpdateAsync(exData);
//                    await salesSupplierBankAccountTempRepo.CommitTransactionAsync();
//                    return true;
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error message: {ex.Message}");
//                await salesSupplierBankAccountTempRepo.RollbackTransactionAsync();
//                return false;
//            }
//        }

//        #endregion

//        #region Bank AccountInfo Delete

//        public async Task<bool> BankAccountInfoDeleteAsync(string sbaid)
//        {
//            try
//            {
//                var entity =await salesSupplierBankAccountTempRepo.GetByIdAsync(sbaid);
//                if (entity != null)
//                {
//                    salesSupplierBankAccountTempRepo.Delete(entity);
//                    return true;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        #endregion

//        #region Bank AccountInfo Clear

//        public async Task<bool> BankAccountInfoClearTableTempAsync()
//        {
//            await salesSupplierBankAccountRepo.BeginTransactionAsync();

//            try
//            {
//                var tempBankAccounts = await salesSupplierBankAccountTempRepo.All().ToListAsync();
//                if (!tempBankAccounts.Any())
//                    return false;

//                var supplierIds = await supplierInformationRepository.All().Select(s => s.SupplierId).ToListAsync();

//                var lastSbaidMainStr = await salesSupplierBankAccountRepo.All().OrderByDescending(x => x.Sbaid).Select(c => c.Sbaid).FirstOrDefaultAsync();

//                int lastSbaMainId = 0;
//                if (!string.IsNullOrEmpty(lastSbaidMainStr))
//                    int.TryParse(lastSbaidMainStr, out lastSbaMainId);

//                var mainAccounts = tempBankAccounts.Where(bank => supplierIds.Contains(bank.SupplierId))
//                    .Select(bank =>
//                    {
//                        lastSbaMainId++;
//                        return new SalesSupplierBankAccount
//                        {
//                            SupplierId = bank.SupplierId,
//                            BankId = bank.BankId,
//                            BankBranchId = bank.BankBranchId,
//                            AccountName = bank.AccountName,
//                            Luser = bank.Luser,
//                            Sbaid = lastSbaMainId.ToString("D3")
//                        };
//                    }).ToList();

//                if (mainAccounts.Any())
//                await salesSupplierBankAccountRepo.AddRangeAsync(mainAccounts);

//                await salesSupplierBankAccountTempRepo.DeleteRangeAsync(tempBankAccounts);

//                await salesSupplierBankAccountRepo.CommitTransactionAsync();
//                return true;
//            }
//            catch (Exception)
//            {
//                await salesSupplierBankAccountRepo.RollbackTransactionAsync();
//                throw;
//            }
//        }

//        #endregion

//    }
//}
