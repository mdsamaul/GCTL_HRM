//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.Common;
//using GCTL.Core.ViewModels.FebricTest;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace GCTL.Service.FebricTest
//{
//    public class FebricTestService : AppService<InvDefFebricTesting>, IFebricTestService
//    {
//        #region Service & Repository
//        private readonly IRepository<InvDefFebricTesting> febricTestrepository;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        private readonly ICommonService commonService;

//        string strMaxNO = string.Empty;

//        private const string TableName = "Inv_Def_FebricTesting";
//        private const string ColumnName = "FebricTestD";

//        public FebricTestService(
//            IRepository<InvDefFebricTesting> febricTestrepository,
//            IRepository<CoreAccessCode> accessCodeRepository,
//            ICommonService commonService

//            )

//    : base(febricTestrepository)
//        {
//            this.febricTestrepository = febricTestrepository;
//            this.accessCodeRepository = accessCodeRepository;
//            this.commonService = commonService;
//        }

//        #endregion

//        #region GetAllAsync

//        public async Task<List<FebricTestSetupViewModel>> GetAllAsync()
//        {
//            var entity = await febricTestrepository.GetAllAsync();
//            return entity.Select(entityVM => new FebricTestSetupViewModel
//            {
//                Tc = entityVM.Tc,
//                FebricTestD = entityVM.FebricTestD,
//                FebricTestName = entityVM.FebricTestName,
//                Details = entityVM.Details,
//                Ldate = entityVM.Ldate,
//                ModifyDate = entityVM.ModifyDate,
//                Luser = entityVM.Luser,
//                Lip = entityVM.Lip,
//                Lmac = entityVM.Lmac,

//            }).ToList();
//        }

//        #endregion

//        #region GetByIdAsync

//        public async Task<FebricTestSetupViewModel> GetByIdAsync(string code)
//        {
//            var entity = await febricTestrepository.GetByIdAsync(code);
//            if (entity == null)
//                return null;

//            return new FebricTestSetupViewModel
//            {
//                Tc = entity.Tc,
//                FebricTestD = entity.FebricTestD,
//                FebricTestName = entity.FebricTestName,
//                Details = entity.Details,
//                Luser = entity.Luser,
//                Ldate = entity.Ldate,
//                ModifyDate = entity.ModifyDate,
//                Lip = entity.Lip,
//                Lmac = entity.Lmac
//            };
//        }


//        #endregion

//        #region SaveAsync

//        public async Task<bool> SaveAsync(FebricTestSetupViewModel entityVM)
//        {
//            try
//            {
//                // Generate next code
//                commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);

//                await febricTestrepository.BeginTransactionAsync();

//                var entity = new InvDefFebricTesting
//                {
//                    FebricTestD = strMaxNO,
//                    FebricTestName = entityVM.FebricTestName?.Trim(),
//                    Details = entityVM.Details ?? string.Empty,
//                    Luser = entityVM.Luser,
//                    Lip = entityVM.Lip,
//                    Lmac = entityVM.Lmac ?? string.Empty,
//                    Ldate = DateTime.Now
//                };

//                await febricTestrepository.AddAsync(entity);
//                await febricTestrepository.CommitTransactionAsync();

//                return true;
//            }
//            catch (Exception ex)
//            {
//                await febricTestrepository.RollbackTransactionAsync();
//                Console.WriteLine($"Error saving Fabric Test: {ex.Message}");
//                return false;
//            }
//        }


//        #endregion

//        #region UpdateAsync
//        public async Task<bool> UpdateAsync(FebricTestSetupViewModel entityVM)
//        {
//            await febricTestrepository.BeginTransactionAsync();
//            try
//            {
//                var entity = await febricTestrepository.GetByIdAsync(entityVM.FebricTestD);
//                if (entity == null)
//                {
//                    await febricTestrepository.RollbackTransactionAsync();
//                    return false;
//                }
//                entity.FebricTestD = entityVM.FebricTestD;
//                entity.FebricTestName = entityVM.FebricTestName;
//                entity.Details = entityVM.Details ?? string.Empty;
//                entity.Luser = entityVM.Luser;
//                entity.Lip = entityVM.Lip;
//                entity.Lmac = entityVM.Lmac;
//                entity.ModifyDate = DateTime.Now;
//                await febricTestrepository.UpdateAsync(entity);
//                await febricTestrepository.CommitTransactionAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error occurred : {ex.Message}");
//                await febricTestrepository.RollbackTransactionAsync();
//                return false;
//            }
//        }

//        #endregion

//        #region SelectionAsync
//        public async Task<IEnumerable<CommonSelectModel>> SelectionFebricTestAsync()
//        {

//            var data = await febricTestrepository.All()
//                       .Select(x => new CommonSelectModel
//                       {
//                           Code = x.FebricTestD,
//                           Name = x.FebricTestName,
//                       }).ToListAsync();
//            return data;
//        }

//        #endregion

//        #region DeleteTab
//        public async Task<bool> DeleteTab(List<string> ids)
//        {
//            var entity = await febricTestrepository.All().Where(x => ids.Contains(x.FebricTestD)).ToListAsync();

//            if (!entity.Any())
//            {
//                return false;
//            }

//            febricTestrepository.Delete(entity);

//            return true;
//        }
//        #endregion

//        #region Duplicate Check 
//        public async Task<bool> IsExistByCodeAsync(string code)
//        {
//            return await febricTestrepository.All().AnyAsync(x => x.FebricTestD == code);
//        }

//        public async Task<bool> IsExistAsync(string name)
//        {
//            return await febricTestrepository.All().AnyAsync(x => x.FebricTestName == name);
//        }

//        public async Task<bool> IsExistAsync(string name, string typeCode)
//        {
//            return await febricTestrepository.All().AnyAsync(x => x.FebricTestName == name && x.FebricTestD != typeCode);
//        }

//        #endregion

//        #region Permission all type
//        public async Task<bool> PagePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Febric Test" && x.TitleCheck);
//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Febric Test" && x.CheckAdd);
//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Febric Test" && x.CheckEdit);
//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Febric Test" && x.CheckDelete);
//        }
//        #endregion

//    }
//}
