//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.Common;
//using GCTL.Core.ViewModels.DeleteHistories;
//using GCTL.Core.ViewModels.TermsConditionInfo;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using GCTL.Service.DeleteHistories;
//using Microsoft.EntityFrameworkCore;

//namespace GCTL.Service.TermsConditionInfo
//{
//    public class TermsConditionInfoService : AppService<RmgTermsCondition>, ITermsConditionInfoService
//    {
//        #region Service & Repository
//        public readonly IRepository<RmgTermsCondition> termsConditionInforepository;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        private readonly ICommonService commonService;
//        private readonly IDeleteHistoryService deleteHistoryService;

//        string strMaxNO = string.Empty;

//        private const string TableName = "RMG_TermsCondition";
//        private const string ColumnName = "TermsConditionId";

//        public TermsConditionInfoService(
//            IRepository<RmgTermsCondition> termsConditionInforepository,
//            IRepository<CoreAccessCode> accessCodeRepository,
//            ICommonService commonService,
//            IDeleteHistoryService deleteHistoryService

//            )
//    : base(termsConditionInforepository)
//        {
//            this.termsConditionInforepository = termsConditionInforepository;
//            this.accessCodeRepository = accessCodeRepository;
//            this.commonService = commonService;
//            this.deleteHistoryService = deleteHistoryService;
//        }

//        #endregion

//        #region GetAllAsync

//        public async Task<List<TermsConditionInfoSetupViewModel>> GetAllAsync()
//        {
//            var entity = await termsConditionInforepository.GetAllAsync();
//            return entity.Select(entityVM => new TermsConditionInfoSetupViewModel
//            {
//                Tc = entityVM.Tc,
//                TermsConditionId = entityVM.TermsConditionId,
//                TermsConditionName = entityVM.TermsConditionName,
//                Ldate = entityVM.Ldate,
//                ModifyDate = entityVM.ModifyDate,
//                Luser = entityVM.Luser,
//                Lip = entityVM.Lip,
//                Lmac = entityVM.Lmac,

//            }).ToList();
//        }

//        #endregion

//        #region GetByIdAsync

//        public async Task<TermsConditionInfoSetupViewModel> GetByIdAsync(string code)
//        {
//            var entity = await termsConditionInforepository.GetByIdAsync(code);
//            if (entity == null) return null;

//            return new TermsConditionInfoSetupViewModel
//            {
//                Tc = entity.Tc,
//                TermsConditionId = entity.TermsConditionId,
//                TermsConditionName = entity.TermsConditionName,
//                Luser = entity.Luser,
//                Ldate = entity.Ldate,
//                ModifyDate = entity.ModifyDate,
//                Lip = entity.Lip,
//                Lmac = entity.Lmac
//            };
//        }

//        #endregion

//        #region SaveAsync

//        public async Task<bool> SaveAsync(TermsConditionInfoSetupViewModel vm)
//        {
//            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 4);

//            var entity = new RmgTermsCondition
//            {
//                TermsConditionId = strMaxNO,
//                TermsConditionName = vm.TermsConditionName?.Trim(),
//                CompanyId = vm.CompanyId ?? string.Empty,
//                EmployeeId = vm.UserInfoEmployeeId ?? string.Empty,
//                Luser = vm.Luser,
//                Lip = vm.Lip,
//                Lmac = vm.Lmac ?? string.Empty,
//                Ldate = DateTime.Now
//            };

//            await termsConditionInforepository.BeginTransactionAsync();

//            try
//            {
//                await termsConditionInforepository.AddAsync(entity);
//                await termsConditionInforepository.CommitTransactionAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                await termsConditionInforepository.RollbackTransactionAsync();
//                // log properly (Serilog / NLog / ILogger)
//                Console.WriteLine(ex);
//                return false;
//            }
//        }


//        #endregion

//        #region UpdateAsync
//        public async Task<bool> UpdateAsync(TermsConditionInfoSetupViewModel entityVM)
//        {
//            await termsConditionInforepository.BeginTransactionAsync();
//            try
//            {
//                var entity = await termsConditionInforepository.GetByIdAsync(entityVM.TermsConditionId);
//                if (entity == null)
//                {
//                    await termsConditionInforepository.RollbackTransactionAsync();
//                    return false;
//                }
//                entity.TermsConditionId = entityVM.TermsConditionId;
//                entity.TermsConditionName = entityVM.TermsConditionName;
//                entity.CompanyId = entityVM.CompanyId;
//                entity.EmployeeId = entityVM.UserInfoEmployeeId;
//                entity.Luser = entityVM.Luser;
//                entity.Lip = entityVM.Lip;
//                entity.Lmac = entityVM.Lmac;
//                entity.ModifyDate = DateTime.Now;
//                await termsConditionInforepository.UpdateAsync(entity);
//                await termsConditionInforepository.CommitTransactionAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error occurred : {ex.Message}");
//                await termsConditionInforepository.RollbackTransactionAsync();
//                return false;
//            }
//        }

//        #endregion

//        #region SelectionAsync
//        public async Task<IEnumerable<CommonSelectModel>> SelectionTermsConditionAsync()
//        {
//            var data = await termsConditionInforepository.All()
//                       .Select(x => new CommonSelectModel
//                       {
//                           Code = x.TermsConditionId,
//                           Name = x.TermsConditionName,
//                       }).ToListAsync();
//            return data;
//        }

//        #endregion

//        #region DeleteTab

//        public async Task<(bool succses, string messege)> DeleteTab(List<string> ids, DeleteHistoryViewModel model)
//        {
//            if (!ids.Any())
//                return (false, "No Data found to delete");

//            var alternateColumn = new List<string> { "TermsCondition" };
//            var _tableName = termsConditionInforepository.GetTableName();

//            var dependencyCheck = await deleteHistoryService.CheckDependenciesAsync(
//                _tableName,
//                ColumnName,
//                ids.Cast<string>().ToList(),
//                alternateColumn
//            );

//            if (!dependencyCheck.CanDelete)
//                return (false, dependencyCheck.Message);

//            await termsConditionInforepository.BeginTransactionAsync();

//            try
//            {
//                var entity = await termsConditionInforepository.All().Where(x => ids.Contains(x.TermsConditionId)).ToListAsync();


//                termsConditionInforepository.Delete(entity);
//                model.tableName = _tableName;
//                await deleteHistoryService.LogDeletedRecordsAsync(
//                    entity,
//                    model);

//                await termsConditionInforepository.CommitTransactionAsync();
//                return (true, "Delete Successfully");
//            }
//            catch (Exception ex)
//            {
//                await termsConditionInforepository.RollbackTransactionAsync();
//                Console.WriteLine(ex.ToString());
//                return (false, "Delete Failed");

//            }

//        }

//        #endregion

//        #region Duplicate Check 
//        public async Task<bool> IsExistByCodeAsync(string code)
//        {
//            return await termsConditionInforepository.All().AnyAsync(x => x.TermsConditionId == code);
//        }

//        public async Task<bool> IsExistAsync(string name)
//        {
//            return await termsConditionInforepository.All().AnyAsync(x => x.TermsConditionName == name);
//        }

//        public async Task<bool> IsExistAsync(string name, string typeCode)
//        {
//            return await termsConditionInforepository.All().AnyAsync(x => x.TermsConditionName == name && x.TermsConditionId != typeCode);
//        }

//        #endregion

//        #region Permission all type
//        public async Task<bool> PagePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Origin" && x.TitleCheck);
//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Origin" && x.CheckAdd);
//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Origin" && x.CheckEdit);
//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Supplier Origin" && x.CheckDelete);
//        }
//        #endregion
//    }
//}
