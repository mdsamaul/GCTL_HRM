//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.Common;
//using GCTL.Core.ViewModels.DeleteHistories;
//using GCTL.Core.ViewModels.ThreadCount;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using GCTL.Service.DeleteHistories;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace GCTL.Service.ThreadCount
//{
//    public class ThreadCountService : AppService<RmgProdDefThreadCount>, IThreadCountService
//    {
//        #region Service & Repository
//        private readonly IRepository<RmgProdDefThreadCount> threadCountrepository;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        private readonly ICommonService commonService;
//        private readonly IDeleteHistoryService deleteHistoryService;

//        string strMaxNO = string.Empty;

//        private const string TableName = "RMG_Prod_Def_ThreadCount";
//        private const string ColumnName = "ThreadCountID";

//        public ThreadCountService(
//            IRepository<RmgProdDefThreadCount> threadCountrepository,
//             IRepository<CoreAccessCode> accessCodeRepository,
//            ICommonService commonService,
//            IDeleteHistoryService deleteHistoryService
//            )

//    : base(threadCountrepository)
//        {
//            this.threadCountrepository = threadCountrepository;
//            this.accessCodeRepository = accessCodeRepository;
//            this.commonService = commonService;
//            this.deleteHistoryService = deleteHistoryService;
//        }

//        #endregion

//        #region GetAllAsync

//        public async Task<List<ThreadCountSetupViewModel>> GetAllAsync()
//        {
//            var entity = await threadCountrepository.GetAllAsync();
//            return entity.Select(entityVM => new ThreadCountSetupViewModel
//            {
//                Tc = entityVM.Tc,
//                ThreadCountId = entityVM.ThreadCountId,
//                ThreadCountName = entityVM.ThreadCountName,
//                ShortName = entityVM.ShortName,
//                Ldate = entityVM.Ldate,
//                ModifyDate = entityVM.ModifyDate,
//                Luser = entityVM.Luser,
//                Lip = entityVM.Lip,
//                Lmac = entityVM.Lmac,

//            }).ToList();
//        }

//        #endregion

//        #region GetByIdAsync

//        public async Task<ThreadCountSetupViewModel> GetByIdAsync(string code)
//        {
//            var entity = await threadCountrepository.GetByIdAsync(code);
//            if (entity == null)
//                return null;

//            return new ThreadCountSetupViewModel
//            {
//                Tc = entity.Tc,
//                ThreadCountId = entity.ThreadCountId,
//                ThreadCountName = entity.ThreadCountName,
//                ShortName = entity.ShortName,
//                Luser = entity.Luser,
//                Ldate = entity.Ldate,
//                ModifyDate = entity.ModifyDate,
//                Lip = entity.Lip,
//                Lmac = entity.Lmac
//            };
//        }


//        #endregion

//        #region SaveAsync

//        public async Task<bool> SaveAsync(ThreadCountSetupViewModel entityVM)
//        {
//            try
//            {
//                // Generate next code
//                commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);

//                await threadCountrepository.BeginTransactionAsync();

//                var entity = new RmgProdDefThreadCount
//                {
//                    ThreadCountId = strMaxNO,
//                    ThreadCountName = entityVM.ThreadCountName?.Trim(),
//                    ShortName = entityVM.ShortName,
//                    Luser = entityVM.Luser,
//                    Lip = entityVM.Lip,
//                    Lmac = entityVM.Lmac ?? string.Empty,
//                    Ldate = DateTime.Now
//                };

//                await threadCountrepository.AddAsync(entity);
//                await threadCountrepository.CommitTransactionAsync();

//                return true;
//            }
//            catch (Exception ex)
//            {
//                await threadCountrepository.RollbackTransactionAsync();
//                Console.WriteLine($"Error saving Fabric Test: {ex.Message}");
//                return false;
//            }
//        }

//        #endregion

//        #region UpdateAsync
//        public async Task<bool> UpdateAsync(ThreadCountSetupViewModel entityVM)
//        {
//            await threadCountrepository.BeginTransactionAsync();
//            try
//            {
//                var entity = await threadCountrepository.GetByIdAsync(entityVM.ThreadCountId);
//                if (entity == null)
//                {
//                    await threadCountrepository.RollbackTransactionAsync();
//                    return false;
//                }
//                entity.ThreadCountId = entityVM.ThreadCountId;
//                entity.ThreadCountName = entityVM.ThreadCountName;
//                entity.ShortName = entityVM.ShortName;
//                entity.Luser = entityVM.Luser;
//                entity.Lip = entityVM.Lip;
//                entity.Lmac = entityVM.Lmac;
//                entity.ModifyDate = DateTime.Now;
//                await threadCountrepository.UpdateAsync(entity);
//                await threadCountrepository.CommitTransactionAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error occurred : {ex.Message}");
//                await threadCountrepository.RollbackTransactionAsync();
//                return false;
//            }
//        }

//        #endregion

//        #region SelectionAsync
//        public async Task<IEnumerable<CommonSelectModel>> SelectionThreadCountAsync()
//        {

//            var data = await threadCountrepository.All()
//                       .Select(x => new CommonSelectModel
//                       {
//                           Code = x.ThreadCountId,
//                           Name = x.ThreadCountName,
//                       }).ToListAsync();
//            return data;
//        }

//        #endregion

//        #region DeleteTab

//        public async Task<(bool succses, string messege)> DeleteTab(List<string> ids, DeleteHistoryViewModel model)
//        {
//            if (!ids.Any())
//                return (false, "No Data found to delete");

//            var dependencyCheck = await deleteHistoryService.CheckDependenciesAsync(
//                threadCountrepository.GetTableName(),
//                ColumnName,
//                ids.Cast<string>().ToList()
//            );

//            if (!dependencyCheck.CanDelete)
//                return (false, dependencyCheck.Message);

//            await threadCountrepository.BeginTransactionAsync();

//            try
//            {
//                var entity = await threadCountrepository.All().Where(x => ids.Contains(x.ThreadCountId)).ToListAsync();


//                threadCountrepository.Delete(entity);
//                model.tableName = TableName;
//                await deleteHistoryService.LogDeletedRecordsAsync(
//                    entity, model
//                );

//                await threadCountrepository.CommitTransactionAsync();
//                return (true, "Delete Successfully");
//            }
//            catch (Exception ex)
//            {
//                await threadCountrepository.RollbackTransactionAsync();
//                Console.WriteLine(ex.ToString());
//                return (false, "Delete Failed");

//            }

//        }

//        //public async Task<bool> DeleteTab(List<string> ids)
//        //{
//        //    var entity = await threadCountrepository.All().Where(x => ids.Contains(x.ThreadCountId)).ToListAsync();

//        //    if (!entity.Any())
//        //    {
//        //        return false;
//        //    }

//        //    threadCountrepository.Delete(entity);

//        //    await deleteHistoryService.LogDeletedRecordsAsync(
//        //    entity,
//        //    threadCountrepository.GetTableName());

//        //    return true;
//        //}
//        #endregion

//        #region Duplicate Check 
//        public async Task<bool> IsExistByCodeAsync(string code)
//        {
//            return await threadCountrepository.All().AnyAsync(x => x.ThreadCountId == code);
//        }

//        public async Task<bool> IsExistAsync(string name)
//        {
//            return await threadCountrepository.All().AnyAsync(x => x.ThreadCountName == name);
//        }

//        public async Task<bool> IsExistAsync(string name, string typeCode)
//        {
//            return await threadCountrepository.All().AnyAsync(x => x.ThreadCountName == name && x.ThreadCountId != typeCode);
//        }

//        #endregion

//        #region Permission all type
//        public async Task<bool> PagePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Thread Count" && x.TitleCheck);
//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Thread Count" && x.CheckAdd);
//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Thread Count" && x.CheckEdit);
//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Thread Count" && x.CheckDelete);
//        }
//        #endregion
//    }
//}
