//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.Common;
//using GCTL.Core.ViewModels.StyleInformation;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace GCTL.Service.StyleInformation
//{
//    public class StyleInformationService : AppService<ProdDefStyle>, IStyleInformationService
//    {
//        #region Service & Repository
//        private readonly IRepository<ProdDefStyle> styleInformationrepository;
//        private readonly IRepository<ProdDefBuyer> buyerRepository;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        private readonly ICommonService commonService;
//        string strMaxNO = string.Empty;
//        private const string TableName = "Prod_Def_Style";
//        private const string ColumnName = "StyleId";
//        public StyleInformationService(
//            IRepository<ProdDefStyle> styleInformationrepository,
//            IRepository<ProdDefBuyer> buyerRepository,
//            IRepository<CoreAccessCode> accessCodeRepository,
//            ICommonService commonService

//            ) 

//    : base(styleInformationrepository)
//        {
//            this.styleInformationrepository = styleInformationrepository;
//            this.buyerRepository = buyerRepository;
//            this.accessCodeRepository = accessCodeRepository;
//            this.commonService = commonService;
//        }

//        #endregion

//        #region GetAllAsync

//        public async Task<List<StyleInformationSetupViewModel>> GetAllAsync(string id)
//        {
//            try
//            {
//                var query = await (from per in styleInformationrepository.All().Where(x => x.BuyerId == id)
//                                   join hrm in buyerRepository.All()
//                                       on per.BuyerId equals hrm.BuyerId into JobGroup
//                                   from hrm in JobGroup.DefaultIfEmpty()
//                                   select new StyleInformationSetupViewModel
//                                   {
//                                       Tc = per.Tc,
//                                       StyleId = per.StyleId,
//                                       Style = per.Style,
//                                       Name = hrm.Name,
//                                       ShortName = per.ShortName,
//                                       Ldate = per.Ldate,
//                                       ModifyDate = per.ModifyDate,
//                                       Luser = per.Luser,
//                                       Lip = per.Lip,
//                                       Lmac = per.Lmac
//                                   }).ToListAsync();

//                return query;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//                throw;
//            }

//        }
//        public async Task<List<StyleInformationSetupViewModel>> GetAllAsync()
//        {
//            try
//            {
//                var query = await (from per in styleInformationrepository.All()
//                                   join hrm in buyerRepository.All()
//                                       on per.BuyerId equals hrm.BuyerId into JobGroup
//                                   from hrm in JobGroup.DefaultIfEmpty()
//                                   select new StyleInformationSetupViewModel
//                                   {
//                                       Tc = per.Tc,
//                                       StyleId = per.StyleId,
//                                       Style = per.Style,
//                                       Name = hrm.Name,
//                                       ShortName = per.ShortName,
//                                       Ldate = per.Ldate,
//                                       ModifyDate = per.ModifyDate,
//                                       Luser = per.Luser,
//                                       Lip = per.Lip,
//                                       Lmac = per.Lmac
//                                   }).ToListAsync();

//                return query;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//                throw;
//            }

//        }


//        #endregion

//        #region GetByIdAsync

//        public async Task<StyleInformationSetupViewModel> GetByIdAsync(string code)
//        {
//            var entity = await styleInformationrepository.GetByIdAsync(code);
//            if (entity == null) return null;

//            StyleInformationSetupViewModel entityVM = new StyleInformationSetupViewModel();
//            entityVM.Tc = entity.Tc;
//            entityVM.StyleId = entity.StyleId;
//            entityVM.Style = entity.Style;
//            entityVM.ShortName = entity.ShortName;
//            entityVM.BuyerId = entity.BuyerId;
//            entityVM.Luser = entity.Luser;
//            entityVM.Ldate = entity.Ldate;
//            entityVM.ModifyDate = entity.ModifyDate;
//            entityVM.Lip = entity.Lip;
//            entityVM.Lmac = entity.Lmac;

//            return entityVM;
//        }

//        #endregion

//        #region SaveAsync

//        public async Task<bool> SaveAsync(StyleInformationSetupViewModel entityVM)
//        {
//            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
//            await styleInformationrepository.BeginTransactionAsync();
//            try
//            {
//                ProdDefStyle entity = new ProdDefStyle();
//                entity.StyleId = strMaxNO;
//                entity.Style = entityVM.Style;
//                entity.ShortName = entityVM.ShortName;
//                entity.BuyerId = entityVM.BuyerId;
//                entity.Luser = entityVM.Luser;
//                entity.Lip = entityVM.Lip;
//                entity.Lmac = entityVM.Lmac ?? string.Empty;
//                entity.Ldate = DateTime.Now;
//                await styleInformationrepository.AddAsync(entity);
//                await styleInformationrepository.CommitTransactionAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"error message {ex.Message}");
//                await styleInformationrepository.RollbackTransactionAsync();
//                return false;
//            }
//        }

//        #endregion

//        #region UpdateAsync
//        public async Task<bool> UpdateAsync(StyleInformationSetupViewModel entityVM)
//        {
//            await styleInformationrepository.BeginTransactionAsync();
//            try
//            {
//                var entity = await styleInformationrepository.GetByIdAsync(entityVM.StyleId);
//                if (entity == null)
//                {
//                    await styleInformationrepository.RollbackTransactionAsync();
//                    return false;
//                }
//                entity.StyleId = entityVM.StyleId;
//                entity.Style = entityVM.Style;
//                entity.ShortName = entityVM.ShortName;
//                entity.BuyerId = entityVM.BuyerId ?? string.Empty;
//                entity.Luser = entityVM.Luser;
//                entity.Lip = entityVM.Lip;
//                entity.Lmac = entityVM.Lmac;
//                entity.ModifyDate = DateTime.Now;
//                await styleInformationrepository.UpdateAsync(entity);
//                await styleInformationrepository.CommitTransactionAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error occurred : {ex.Message}");
//                await styleInformationrepository.RollbackTransactionAsync();
//                return false;
//            }
//        }

//        #endregion

//        #region SelectionAsync
//        public async Task<IEnumerable<CommonSelectModel>> SelectionStyleInformationAsync()
//        {

//            var data = await styleInformationrepository.All()
//                       .Select(x => new CommonSelectModel
//                       {
//                           Code = x.StyleId,
//                           Name = x.Style,
//                       }).ToListAsync();
//            return data;
//        }

//        #endregion

//        #region DeleteTab
//        public async Task<bool> DeleteTab(List<string> ids)
//        {
//            var entity = await styleInformationrepository.All().Where(x => ids.Contains(x.StyleId)).ToListAsync();

//            if (!entity.Any())
//            {
//                return false;
//            }

//            styleInformationrepository.Delete(entity);

//            return true;
//        }
//        #endregion

//        #region Duplicate Check 
//        public async Task<bool> IsExistByCodeAsync(string code)
//        {
//            return await styleInformationrepository.All().AnyAsync(x => x.StyleId == code);
//        }

//        public async Task<bool> IsExistAsync(string name)
//        {
//            return await styleInformationrepository.All().AnyAsync(x => x.Style == name);
//        }

//        public async Task<bool> IsExistAsync(string name, string typeCode)
//        {
//            return await styleInformationrepository.All().AnyAsync(x => x.Style == name && x.StyleId != typeCode);
//        }

//        #endregion

//        #region Permission all type
//        public async Task<bool> PagePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Style Information" && x.TitleCheck);
//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Style Information" && x.CheckAdd);
//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Style Information" && x.CheckEdit);
//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Style Information" && x.CheckDelete);
//        }
//        #endregion
//    }
//}
