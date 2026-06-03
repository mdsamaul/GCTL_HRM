using GCTL.Core.Data;
using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.ColorInformation
{
    public class ColorInformationService : AppService<RmgProdDefColor>, IColorInformationService
    {
        #region Service & Repository
        private readonly IRepository<RmgProdDefColor> colorInformationrepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;

        private const string TableName = "RMG_Prod_Def_Color";
        private const string ColumnName = "ColorId";
        public ColorInformationService(
            IRepository<RmgProdDefColor> colorInformationrepository,
            IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService

            ) 
            
    : base(colorInformationrepository)
        {
            this.colorInformationrepository = colorInformationrepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        #endregion

        #region GetAllAsync

        public async Task<List<ColorInformationSetupViewModel>> GetAllAsync()
        {
            var entity = await colorInformationrepository.GetAllAsync();
            return entity.Select(entityVM => new ColorInformationSetupViewModel
            {
                Tc = entityVM.Tc,
                ColorId = entityVM.ColorId,
                Color = entityVM.Color,
                Detail = entityVM.Detail,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,

            }).ToList();
        }

        #endregion

        #region GetByIdAsync

        public async Task<ColorInformationSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await colorInformationrepository.GetByIdAsync(code);
            if (entity == null) return null;

            ColorInformationSetupViewModel entityVM = new ColorInformationSetupViewModel();
            entityVM.Tc = entity.Tc;
            entityVM.ColorId = entity.ColorId;
            entityVM.Color = entity.Color;
            entityVM.Color = entity.Color;
            entityVM.Detail = entity.Detail;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(ColorInformationSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            await colorInformationrepository.BeginTransactionAsync();
            try
            {
                RmgProdDefColor entity = new RmgProdDefColor();
                entity.ColorId = strMaxNO;
                entity.Color = entityVM.Color;
                entity.Detail = entityVM.Detail ?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await colorInformationrepository.AddAsync(entity);
                await colorInformationrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await colorInformationrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(ColorInformationSetupViewModel entityVM)
        {
            await colorInformationrepository.BeginTransactionAsync();
            try
            {
                var entity = await colorInformationrepository.GetByIdAsync(entityVM.ColorId);
                if (entity == null)
                {
                    await colorInformationrepository.RollbackTransactionAsync();
                    return false;
                }
                entity.ColorId = entityVM.ColorId;
                entity.Color = entityVM.Color;
                entity.Detail = entityVM.Detail ?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await colorInformationrepository.UpdateAsync(entity);
                await colorInformationrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await colorInformationrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region SelectionAsync
        public async Task<IEnumerable<CommonSelectModel>> SelectionColorInformationAsync()
        {

            var data = await colorInformationrepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.ColorId,
                           Name = x.Color,
                       }).ToListAsync();
            return data;
        }

        #endregion

        #region DeleteTab
        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await colorInformationrepository.All().Where(x => ids.Contains(x.ColorId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            colorInformationrepository.Delete(entity);

            return true;
        }
        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await colorInformationrepository.All().AnyAsync(x => x.ColorId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await colorInformationrepository.All().AnyAsync(x => x.Color == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await colorInformationrepository.All().AnyAsync(x => x.Color == name && x.ColorId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Color Information" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Color Information" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Color Information" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Color Information" && x.CheckDelete);
        }
        #endregion

    }
}
