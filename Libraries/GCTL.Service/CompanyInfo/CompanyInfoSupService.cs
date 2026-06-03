//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.Common;
//using GCTL.Core.ViewModels.CompanyInfo;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using Microsoft.EntityFrameworkCore;

//namespace GCTL.Service.CompanyInfo
//{
//    public class CompanyInfoSupService : AppService<InvDefCompanyInfo>, ICompanyInfoSupService
//    {
//        #region Service & Repository
//        private readonly IRepository<InvDefCompanyInfo> companyInforepository;
//        private readonly IRepository<InvDefCompanyFor> invcompanyInforepository;
//        private readonly IRepository<CaDefCountry> cadefcountryrepository;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        private readonly ICommonService commonService;

//        string strMaxNO = string.Empty;
//        private const string TableName = "Inv_Def_CompanyInfo";
//        private const string ColumnName = "CompanyID";

//        public CompanyInfoSupService(
//            IRepository<InvDefCompanyInfo> companyInforepository,
//            IRepository<InvDefCompanyFor> invcompanyInforepository,
//            IRepository<CaDefCountry> cadefcountryrepository,
//             IRepository<CoreAccessCode> accessCodeRepository,
//            ICommonService commonService

//            )
//    : base(companyInforepository)
//        {
//            this.companyInforepository = companyInforepository;
//            this.invcompanyInforepository = invcompanyInforepository;
//            this.cadefcountryrepository = cadefcountryrepository;
//            this.accessCodeRepository = accessCodeRepository;
//            this.commonService = commonService;
//        }

//        #endregion

//        #region GetAllAsync

//        public async Task<List<CompanyInfoSetupViewModel>> GetAllAsync()
//        {
//            var query = await (from per in companyInforepository.All()
//                               join hrm in invcompanyInforepository.All()
//                               on per.CompanyForId equals hrm.CompanyForId into JobGroup
//                               from hrm in JobGroup.DefaultIfEmpty()
//                               select new CompanyInfoSetupViewModel
//                               {
//                                   Tc = per.Tc,
//                                   CompanyId = per.CompanyId,
//                                   CompanyName = per.CompanyName,
//                                   ShortName = per.ShortName,
//                                   CompanyAddress = per.Address,
//                                   CompanyForName = hrm.CompanyForName,
//                                   Ldate = per.Ldate,
//                                   ModifyDate = per.ModifyDate,
//                                   Luser = per.Luser,
//                                   Lip = per.Lip,
//                                   Lmac = per.Lmac
//                               }).ToListAsync();

//            return query;
//        }

//        #endregion

//        #region GetByIdAsync

//        public async Task<CompanyInfoSetupViewModel> GetByIdAsync(string code)
//        {
//            var entity = await companyInforepository.GetByIdAsync(Convert.ToInt32(code));
//            if (entity == null) return null;

//            CompanyInfoSetupViewModel entityVM = new CompanyInfoSetupViewModel();
//            entityVM.Tc = entity.Tc;
//            entityVM.CompanyId = entity.CompanyId;
//            entityVM.CompanyForId = entity.CompanyForId;
//            entityVM.CompanyName = entity.CompanyName;
//            entityVM.ShortName = entity.ShortName;
//            entityVM.CompanyAddress = entity.Address;
//            entityVM.LocalOfficeAddress = entity.LocalOfficeAddress;
//            entityVM.ZipCode = entity.ZipCode;
//            entityVM.City = entity.City;
//            entityVM.State = entity.State;
//            entityVM.CountryId = entity.CountryId;
//            entityVM.Phone = entity.Phone;
//            entityVM.Fax = entity.Fax;
//            entityVM.Email = entity.Email;
//            entityVM.Url = entity.Url;
//            entityVM.Remarks = entity.Remarks;
//            entityVM.Luser = entity.Luser;
//            entityVM.Ldate = entity.Ldate;
//            entityVM.ModifyDate = entity.ModifyDate;
//            entityVM.Lip = entity.Lip;
//            entityVM.Lmac = entity.Lmac;

//            return entityVM;
//        }

//        #endregion

//        #region SaveAsync

//        public async Task<bool> SaveAsync(CompanyInfoSetupViewModel entityVM)
//        {
//            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
//            await companyInforepository.BeginTransactionAsync();
//            try
//            {
//                InvDefCompanyInfo entity = new InvDefCompanyInfo();
//                entity.CompanyId = strMaxNO;
//                entity.CompanyForId = entityVM.CompanyForId ?? string.Empty;
//                entity.CompanyName = entityVM.CompanyName ?? string.Empty;
//                entity.ShortName = entityVM.ShortName ?? string.Empty;
//                entity.Address = entityVM.CompanyAddress ?? string.Empty;
//                entity.LocalOfficeAddress = entityVM.LocalOfficeAddress ?? string.Empty;
//                entity.ZipCode = entityVM.ZipCode ?? string.Empty;
//                entity.City = entityVM.City ?? string.Empty;
//                entity.State = entityVM.State ?? string.Empty;
//                entity.CountryId = entityVM.CountryId ?? string.Empty;
//                entity.Phone = entityVM.Phone ?? string.Empty;
//                entity.Fax = entityVM.Fax ?? string.Empty;
//                entity.Email = entityVM.Email ?? string.Empty;
//                entity.Url = entityVM.Url ?? string.Empty;
//                entity.Remarks = entityVM.Remarks ?? string.Empty;
//                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
//                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
//                entity.Luser = entityVM.Luser;
//                entity.Lip = entityVM.Lip;
//                entity.Lmac = entityVM.Lmac ?? string.Empty;
//                entity.Ldate = DateTime.Now;
//                await companyInforepository.AddAsync(entity);
//                await companyInforepository.CommitTransactionAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"error message {ex.Message}");
//                await companyInforepository.RollbackTransactionAsync();
//                return false;
//            }
//        }

//        #endregion

//        #region UpdateAsync
//        public async Task<bool> UpdateAsync(CompanyInfoSetupViewModel entityVM)
//        {
//            await companyInforepository.BeginTransactionAsync();
//            try
//            {
//                var entity = await companyInforepository.GetByIdAsync(entityVM.Tc);
//                if (entity == null)
//                {
//                    await companyInforepository.RollbackTransactionAsync();
//                    return false;
//                }
//                entity.CompanyId = entityVM.CompanyId;
//                entity.CompanyForId = entityVM.CompanyForId ?? string.Empty;
//                entity.CompanyName = entityVM.CompanyName ?? string.Empty;
//                entity.ShortName = entityVM.ShortName ?? string.Empty;
//                entity.Address = entityVM.CompanyAddress ?? string.Empty;
//                entity.LocalOfficeAddress = entityVM.LocalOfficeAddress ?? string.Empty;
//                entity.ZipCode = entityVM.ZipCode ?? string.Empty;
//                entity.City = entityVM.City ?? string.Empty;
//                entity.State = entityVM.State ?? string.Empty;
//                entity.CountryId = entityVM.CountryId ?? string.Empty;
//                entity.Phone = entityVM.Phone ?? string.Empty;
//                entity.Fax = entityVM.Fax ?? string.Empty;
//                entity.Email = entityVM.Email ?? string.Empty;
//                entity.Url = entityVM.Url ?? string.Empty;
//                entity.Remarks = entityVM.Remarks ?? string.Empty;
//                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
//                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
//                entity.Luser = entityVM.Luser;
//                entity.Lip = entityVM.Lip;
//                entity.Lmac = entityVM.Lmac;
//                entity.ModifyDate = DateTime.Now;
//                await companyInforepository.UpdateAsync(entity);
//                await companyInforepository.CommitTransactionAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error occurred : {ex.Message}");
//                await companyInforepository.RollbackTransactionAsync();
//                return false;
//            }
//        }

//        #endregion

//        #region SelectionAsync
//        public async Task<IEnumerable<CommonSelectModel>> SelectionCompanyInfoAsync()
//        {
//            var data = await companyInforepository.All().Where(x => x.CompanyForId == "03")
//                       .Select(x => new CommonSelectModel
//                       {
//                           Code = x.CompanyId,
//                           Name = x.CompanyName,
//                       }).ToListAsync();
//            return data;
//        }
//        public async Task<IEnumerable<CommonSelectModel>> SelectionBuyerCompanyInfoAsync()
//        {
//            var data = await companyInforepository.All().Where(x => x.CompanyForId == "02")
//                       .Select(x => new CommonSelectModel
//                       {
//                           Code = x.CompanyId,
//                           Name = x.CompanyName,
//                       }).ToListAsync();
//            return data;
//        }

//        #endregion

//        #region DeleteTab
//        public async Task<bool> DeleteTab(List<string> ids)
//        {
//            var entity = await companyInforepository.All().Where(x => ids.Contains(x.CompanyId)).ToListAsync();

//            if (!entity.Any())
//            {
//                return false;
//            }

//            companyInforepository.Delete(entity);

//            return true;
//        }
//        #endregion

//        #region Duplicate Check 
//        public async Task<bool> IsExistByCodeAsync(string code)
//        {
//            return await companyInforepository.All().AnyAsync(x => x.CompanyForId == code);
//        }

//        public async Task<bool> IsExistAsync(string name)
//        {
//            return await companyInforepository.All().AnyAsync(x => x.CompanyName == name);
//        }

//        public async Task<bool> IsExistAsync(string name, string typeCode)
//        {
//            return await companyInforepository.All().AnyAsync(x => x.CompanyName == name && x.CompanyForId != typeCode);
//        }

//        #endregion

//        #region Permission all type
//        public async Task<bool> PagePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Company Info" && x.TitleCheck);
//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Company Info" && x.CheckAdd);
//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Company Info" && x.CheckEdit);
//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Company Info" && x.CheckDelete);
//        }
//        #endregion
//    }
//}
