using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HRMDefBoardCountryNames;
using GCTL.Core.ViewModels.HrmDefDegrees;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRMDefBoardCountryNames
{
    public class HRMDefBoardCountryNamesService:AppService<HrmDefBoardCountryName>, IHRMDefBoardCountryNamesService
    {
        private readonly IRepository<HrmDefBoardCountryName> hrmDefBoardRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;
        private const string TableName = "HRM_Def_BoardCountryName";
        private const string ColumnName = "BoardCode";

        public HRMDefBoardCountryNamesService(IRepository<HrmDefBoardCountryName> hrmDefBoardRepository, IRepository<CoreAccessCode> accessCodeRepository, ICommonService commonService):base(hrmDefBoardRepository)
        {
            this.hrmDefBoardRepository = hrmDefBoardRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        public async Task<List<HRMDefBoardCountryNamesSetupViewModel>> GetAllAsync()
        {
            var entity = await hrmDefBoardRepository.GetAllAsync();
            return entity.Select(entityVM => new HRMDefBoardCountryNamesSetupViewModel
            {
                AutoId = entityVM.AutoId,
                BoardCode = entityVM.BoardCode,
                BoardName = entityVM.BoardName,
                ShortName = entityVM.ShortName,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,
                UserInfoEmployeeId = entityVM.UserInfoEmployeeId,


            }).ToList();
        }

        public async Task<HRMDefBoardCountryNamesSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await hrmDefBoardRepository.GetByIdAsync(code);
            if (entity == null) return null;

            HRMDefBoardCountryNamesSetupViewModel entityVM = new HRMDefBoardCountryNamesSetupViewModel();
            entityVM.AutoId = entity.AutoId;
            entityVM.BoardCode = entity.BoardCode;
            entityVM.BoardName = entity.BoardName;
            entityVM.ShortName = entity.ShortName;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;
            entityVM.UserInfoEmployeeId = entity.UserInfoEmployeeId;
            return entityVM;
        }

        public async Task< IEnumerable<CommonSelectModel>> SelectionHrmDefBoardCountryTypeAsync()
        {

            var data =await hrmDefBoardRepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.BoardCode,
                           Name = x.BoardName
                       }).ToListAsync();
            return data;
        }


        public async Task<bool> SaveAsync(HRMDefBoardCountryNamesSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            //commonService.FindMaxNo(ref strMaxNO, "BoardCode", "HRM_Def_BoardCountryName", 3);
            await hrmDefBoardRepository.BeginTransactionAsync();
            try
            {

                HrmDefBoardCountryName entity = new HrmDefBoardCountryName();
                entity.BoardCode = strMaxNO;
                entity.BoardName = entityVM.BoardName;
                entity.ShortName = entityVM.ShortName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.UserInfoEmployeeId = entityVM.UserInfoEmployeeId;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode;
                entity.Ldate = DateTime.Now;
                await hrmDefBoardRepository.AddAsync(entity);
                await hrmDefBoardRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await hrmDefBoardRepository.RollbackTransactionAsync();
                
                return false;
            }
        }

        public async Task<bool> UpdateAsync(HRMDefBoardCountryNamesSetupViewModel entityVM)
        {
            await hrmDefBoardRepository.BeginTransactionAsync();
            try
            {

                var entity = await hrmDefBoardRepository.GetByIdAsync(entityVM.BoardCode);
                if (entity == null)
                {
                    await hrmDefBoardRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.BoardCode = entityVM.BoardCode;
                entity.BoardName = entityVM.BoardName;
                entity.ShortName = entityVM.ShortName;
                entity.CompanyCode = entityVM.CompanyCode;
                entity.Luser = entityVM.Luser;
                entity.UserInfoEmployeeId = entityVM.UserInfoEmployeeId;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await hrmDefBoardRepository.UpdateAsync(entity);
                await hrmDefBoardRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await hrmDefBoardRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await hrmDefBoardRepository.All().Where(x => ids.Contains(x.BoardCode)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            hrmDefBoardRepository.Delete(entity);

            return true;
        }


        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await hrmDefBoardRepository.All().AnyAsync(x => x.BoardCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hrmDefBoardRepository.All().AnyAsync(x => x.BoardName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await hrmDefBoardRepository.All().AnyAsync(x => x.BoardName == name && x.BoardCode != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Exam Board" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Exam Board" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Exam Board" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Exam Board" && x.CheckDelete);
        }
        #endregion

    }
}
