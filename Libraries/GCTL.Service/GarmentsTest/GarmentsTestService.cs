//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.Common;
//using GCTL.Core.ViewModels.FebricTest;
//using GCTL.Core.ViewModels.GarmentsTest;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace GCTL.Service.GarmentsTest
//{
//    public class GarmentsTestService : AppService<InvDefGarmentsTesing>, IGarmentsTestService
//    {
//        #region Service & Repository
//        private readonly IRepository<InvDefGarmentsTesing> garmentsTestrepository;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        private readonly ICommonService commonService;

//        string strMaxNO = string.Empty;

//        private const string TableName = "Inv_Def_GarmentsTesing";
//        private const string ColumnName = "GarmentsTestD";

//        public GarmentsTestService(
//            IRepository<InvDefGarmentsTesing> garmentsTestrepository,
//            IRepository<CoreAccessCode> accessCodeRepository,
//            ICommonService commonService

//            )
//    : base(garmentsTestrepository)
//        {
//            this.garmentsTestrepository = garmentsTestrepository;
//            this.accessCodeRepository = accessCodeRepository;
//            this.commonService = commonService;
//        }

//        #endregion

//        #region GetAllAsync

//        public async Task<List<GarmentsTestSetupViewModel>> GetAllAsync()
//        {
//            var entity = await garmentsTestrepository.GetAllAsync();
//            return entity.Select(entityVM => new GarmentsTestSetupViewModel
//            {
//                Tc = entityVM.Tc,
//                GarmentsTestD = entityVM.GarmentsTestD,
//                GarmentsTestName = entityVM.GarmentsTestName,
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

//        public async Task<GarmentsTestSetupViewModel> GetByIdAsync(string code)
//        {
//            var entity = await garmentsTestrepository.GetByIdAsync(code);
//            if (entity == null)
//                return null;

//            return new GarmentsTestSetupViewModel
//            {
//                Tc = entity.Tc,
//                GarmentsTestD = entity.GarmentsTestD,
//                GarmentsTestName = entity.GarmentsTestName,
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

//        public async Task<bool> SaveAsync(GarmentsTestSetupViewModel entityVM)
//        {
//            try
//            {
//                // Generate next code
//                commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);

//                await garmentsTestrepository.BeginTransactionAsync();

//                var entity = new InvDefGarmentsTesing
//                {
//                    GarmentsTestD = strMaxNO,
//                    GarmentsTestName = entityVM.GarmentsTestName?.Trim(),
//                    Details = entityVM.Details ?? string.Empty,
//                    Luser = entityVM.Luser,
//                    Lip = entityVM.Lip,
//                    Lmac = entityVM.Lmac ?? string.Empty,
//                    Ldate = DateTime.Now
//                };

//                await garmentsTestrepository.AddAsync(entity);
//                await garmentsTestrepository.CommitTransactionAsync();

//                return true;
//            }
//            catch (Exception ex)
//            {
//                await garmentsTestrepository.RollbackTransactionAsync();
//                Console.WriteLine($"Error saving Fabric Test: {ex.Message}");
//                return false;
//            }
//        }


//        #endregion

//        #region UpdateAsync
//        public async Task<bool> UpdateAsync(GarmentsTestSetupViewModel entityVM)
//        {
//            await garmentsTestrepository.BeginTransactionAsync();
//            try
//            {
//                var entity = await garmentsTestrepository.GetByIdAsync(entityVM.GarmentsTestD);
//                if (entity == null)
//                {
//                    await garmentsTestrepository.RollbackTransactionAsync();
//                    return false;
//                }
//                entity.GarmentsTestD = entityVM.GarmentsTestD;
//                entity.GarmentsTestName = entityVM.GarmentsTestName;
//                entity.Details = entityVM.Details ?? string.Empty;
//                entity.Luser = entityVM.Luser;
//                entity.Lip = entityVM.Lip;
//                entity.Lmac = entityVM.Lmac;
//                entity.ModifyDate = DateTime.Now;
//                await garmentsTestrepository.UpdateAsync(entity);
//                await garmentsTestrepository.CommitTransactionAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error occurred : {ex.Message}");
//                await garmentsTestrepository.RollbackTransactionAsync();
//                return false;
//            }
//        }

//        #endregion

//        #region SelectionAsync
//        public async Task<IEnumerable<CommonSelectModel>> SelectionGarmentsTestAsync()
//        {

//            var data = await garmentsTestrepository.All()
//                       .Select(x => new CommonSelectModel
//                       {
//                           Code = x.GarmentsTestD,
//                           Name = x.GarmentsTestName,
//                       }).ToListAsync();
//            return data;
//        }

//        #endregion

//        #region DeleteTab
//        public async Task<bool> DeleteTab(List<string> ids)
//        {
//            var entity = await garmentsTestrepository.All().Where(x => ids.Contains(x.GarmentsTestD)).ToListAsync();

//            if (!entity.Any())
//            {
//                return false;
//            }

//            garmentsTestrepository.Delete(entity);

//            return true;
//        }
//        #endregion

//        #region Duplicate Check 
//        public async Task<bool> IsExistByCodeAsync(string code)
//        {
//            return await garmentsTestrepository.All().AnyAsync(x => x.GarmentsTestD == code);
//        }

//        public async Task<bool> IsExistAsync(string name)
//        {
//            return await garmentsTestrepository.All().AnyAsync(x => x.GarmentsTestName == name);
//        }

//        public async Task<bool> IsExistAsync(string name, string typeCode)
//        {
//            return await garmentsTestrepository.All().AnyAsync(x => x.GarmentsTestName == name && x.GarmentsTestD != typeCode);
//        }

//        #endregion

//        #region Permission all type
//        public async Task<bool> PagePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Garments Test" && x.TitleCheck);
//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Garments Test" && x.CheckAdd);
//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Garments Test" && x.CheckEdit);
//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Garments Test" && x.CheckDelete);
//        }
//        #endregion

//    }
//}
