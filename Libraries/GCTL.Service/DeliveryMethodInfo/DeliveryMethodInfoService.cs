//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.Common;
//using GCTL.Core.ViewModels.DeleteHistories;
//using GCTL.Core.ViewModels.DeliveryMethodInfo;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using GCTL.Service.DeleteHistories;
//using Microsoft.EntityFrameworkCore;

//namespace GCTL.Service.DeliveryMethodInfo
//{
//    public class DeliveryMethodInfoService : AppService<RmgProdDefDeliveryMethod>, IDeliveryMethodInfoService
//    {
//        #region Service & Repository
//        public readonly IRepository<RmgProdDefDeliveryMethod> deliveryMethodInfoRepository;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        private readonly ICommonService commonService;
//        private readonly IDeleteHistoryService deleteHistoryService;

//        string strMaxNO = string.Empty;

//        private const string TableName = "RMG_Prod_Def_DeliveryMethod";
//        private const string ColumnName = "DeliveryMethodId";
//        public DeliveryMethodInfoService(
//            IRepository<RmgProdDefDeliveryMethod> deliveryMethodInfoRepository,
//            IRepository<CoreAccessCode> accessCodeRepository,
//            ICommonService commonService,
//            IDeleteHistoryService deleteHistoryService

//            )
//    : base(deliveryMethodInfoRepository)
//        {
//            this.deliveryMethodInfoRepository = deliveryMethodInfoRepository;
//            this.accessCodeRepository = accessCodeRepository;
//            this.commonService = commonService;
//            this.deleteHistoryService = deleteHistoryService;
//        }

//        #endregion

//        #region GetAllAsync

//        public async Task<List<DeliveryMethodInfoSetupViewModel>> GetAllAsync()
//        {
//            var entity = await deliveryMethodInfoRepository.GetAllAsync();
//            return entity.Select(entityVM => new DeliveryMethodInfoSetupViewModel
//            {
//                Tc = entityVM.Tc,
//                DeliveryMethodId = entityVM.DeliveryMethodId,
//                DeliveryMethod = entityVM.DeliveryMethod,
//                Detail = entityVM.Detail,
//                Ldate = entityVM.Ldate,
//                ModifyDate = entityVM.ModifyDate,
//                Luser = entityVM.Luser,
//                Lip = entityVM.Lip,
//                Lmac = entityVM.Lmac,

//            }).ToList();
//        }

//        #endregion

//        #region GetByIdAsync

//        public async Task<DeliveryMethodInfoSetupViewModel> GetByIdAsync(string code)
//        {
//            var entity = await deliveryMethodInfoRepository.GetByIdAsync(code);
//            if (entity == null) return null;

//            return new DeliveryMethodInfoSetupViewModel
//            {
//                Tc = entity.Tc,
//                DeliveryMethodId = entity.DeliveryMethodId,
//                DeliveryMethod = entity.DeliveryMethod,
//                Detail = entity.Detail,
//                Luser = entity.Luser,
//                Ldate = entity.Ldate,
//                ModifyDate = entity.ModifyDate,
//                Lip = entity.Lip,
//                Lmac = entity.Lmac
//            };
//        }

//        #endregion

//        #region SaveAsync

//        public async Task<bool> SaveAsync(DeliveryMethodInfoSetupViewModel vm)
//        {
//            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);

//            var entity = new RmgProdDefDeliveryMethod
//            {
//                DeliveryMethodId = strMaxNO,
//                DeliveryMethod = vm.DeliveryMethod?.Trim(),
//                Detail = vm.Detail,
//                CompanyId = vm.CompanyId ?? string.Empty,
//                EmployeeId = vm.UserInfoEmployeeId ?? string.Empty,
//                Luser = vm.Luser,
//                Lip = vm.Lip,
//                Lmac = vm.Lmac ?? string.Empty,
//                Ldate = DateTime.Now
//            };

//            await deliveryMethodInfoRepository.BeginTransactionAsync();

//            try
//            {
//                await deliveryMethodInfoRepository.AddAsync(entity);
//                await deliveryMethodInfoRepository.CommitTransactionAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                await deliveryMethodInfoRepository.RollbackTransactionAsync();
//                // log properly (Serilog / NLog / ILogger)
//                Console.WriteLine(ex);
//                return false;
//            }
//        }


//        #endregion

//        #region UpdateAsync
//        public async Task<bool> UpdateAsync(DeliveryMethodInfoSetupViewModel entityVM)
//        {
//            await deliveryMethodInfoRepository.BeginTransactionAsync();
//            try
//            {
//                var entity = await deliveryMethodInfoRepository.GetByIdAsync(entityVM.DeliveryMethodId);
//                if (entity == null)
//                {
//                    await deliveryMethodInfoRepository.RollbackTransactionAsync();
//                    return false;
//                }
//                entity.DeliveryMethodId = entityVM.DeliveryMethodId;
//                entity.DeliveryMethod = entityVM.DeliveryMethod;
//                entity.Detail = entityVM.Detail;
//                entity.CompanyId = entityVM.CompanyId;
//                entity.EmployeeId = entityVM?.UserInfoEmployeeId ?? string.Empty;
//                entity.Luser = entityVM.Luser;
//                entity.Lip = entityVM.Lip;
//                entity.Lmac = entityVM.Lmac;
//                entity.ModifyDate = DateTime.Now;
//                await deliveryMethodInfoRepository.UpdateAsync(entity);
//                await deliveryMethodInfoRepository.CommitTransactionAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error occurred : {ex.Message}");
//                await deliveryMethodInfoRepository.RollbackTransactionAsync();
//                return false;
//            }
//        }

//        #endregion

//        #region SelectionAsync
//        public async Task<IEnumerable<CommonSelectModel>> SelectionDeliveryMethodAsync()
//        {
//            var data = await deliveryMethodInfoRepository.All()
//                       .Select(x => new CommonSelectModel
//                       {
//                           Code = x.DeliveryMethodId,
//                           Name = x.DeliveryMethod,
//                       }).ToListAsync();
//            return data;
//        }

//        #endregion

//        #region DeleteTab

//        public async Task<(bool succses, string messege)> DeleteTab(List<string> ids, DeleteHistoryViewModel model)
//        {
//            if (!ids.Any())
//                return (false, "No Data found to delete");

//            string _tableName = deliveryMethodInfoRepository.GetTableName();

//            var dependencyCheck = await deleteHistoryService.CheckDependenciesAsync(
//                _tableName,
//                ColumnName,
//                ids.Cast<string>().ToList()
//            );

//            if (!dependencyCheck.CanDelete)
//                return (false, dependencyCheck.Message);

//            await deliveryMethodInfoRepository.BeginTransactionAsync();

//            try
//            {
//                var entity = await deliveryMethodInfoRepository.All().Where(x => ids.Contains(x.DeliveryMethodId)).ToListAsync();


//                deliveryMethodInfoRepository.Delete(entity);

//                await deleteHistoryService.LogDeletedRecordsAsync(
//                    entity,
//                    model
//                    );

//                await deliveryMethodInfoRepository.CommitTransactionAsync();
//                return (true, "Delete Successfully");
//            }
//            catch (Exception ex)
//            {
//                await deliveryMethodInfoRepository.RollbackTransactionAsync();
//                Console.WriteLine(ex.ToString());
//                return (false, "Delete Failed");

//            }

//        }

//        #endregion

//        #region Duplicate Check 
//        public async Task<bool> IsExistByCodeAsync(string code)
//        {
//            return await deliveryMethodInfoRepository.All().AnyAsync(x => x.DeliveryMethodId == code);
//        }

//        public async Task<bool> IsExistAsync(string name)
//        {
//            return await deliveryMethodInfoRepository.All().AnyAsync(x => x.DeliveryMethod == name);
//        }

//        public async Task<bool> IsExistAsync(string name, string typeCode)
//        {
//            return await deliveryMethodInfoRepository.All().AnyAsync(x => x.DeliveryMethod == name && x.DeliveryMethodId != typeCode);
//        }

//        #endregion

//        #region Permission all type
//        public async Task<bool> PagePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Delivery Method Info" && x.TitleCheck);
//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Delivery Method Info" && x.CheckAdd);
//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Delivery Method Info" && x.CheckEdit);
//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Delivery Method Info" && x.CheckDelete);
//        }
//        #endregion

//    }
//}
