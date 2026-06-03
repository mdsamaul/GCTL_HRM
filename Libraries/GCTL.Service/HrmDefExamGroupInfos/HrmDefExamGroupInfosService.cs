using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefExamGroupInfos;
using GCTL.Core.ViewModels.HRMDefExamTitles;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefExamGroupInfos
{
    public class HrmDefExamGroupInfosService:AppService<HrmDefExamGroupInfo>, IHrmDefExamGroupInfosService
    {
        private readonly IRepository<HrmDefExamGroupInfo> hrmDefExamGroupInfoRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;
        private const string TableName = "HRM_Def_ExamGroupInfo";
        private const string ColumnName = "GroupCode";

        public HrmDefExamGroupInfosService(IRepository<HrmDefExamGroupInfo> hrmDefExamGroupInfoRepository, IRepository<CoreAccessCode> accessCodeRepository, ICommonService commonService):base(hrmDefExamGroupInfoRepository)
        {
            this.hrmDefExamGroupInfoRepository = hrmDefExamGroupInfoRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        public async Task<List<HrmDefExamGroupInfosSetupViewModel>> GetAllAsync()
        {
            var entity = await hrmDefExamGroupInfoRepository.GetAllAsync();
            return entity.Select(entityVM => new HrmDefExamGroupInfosSetupViewModel
            {
                AutoId = entityVM.AutoId,
                GroupCode = entityVM.GroupCode,
                GroupName = entityVM.GroupName,
                ShortName = entityVM.ShortName,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,



            }).ToList();
        }

        public async Task<HrmDefExamGroupInfosSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await hrmDefExamGroupInfoRepository.GetByIdAsync(code);
            if (entity == null) return null;

            HrmDefExamGroupInfosSetupViewModel entityVM = new HrmDefExamGroupInfosSetupViewModel();
            entityVM.AutoId = entity.AutoId;
            entityVM.GroupCode = entity.GroupCode;
            entityVM.GroupName = entity.GroupName;
            entityVM.ShortName = entity.ShortName;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        public async Task<IEnumerable<CommonSelectModel>> SelectionHrmDefExamGroupInfoTypeAsync()
        {

            var data =await hrmDefExamGroupInfoRepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.GroupCode,
                           Name = x.GroupName,
                       }).ToListAsync();
            return data;
        }


        public async Task<bool> SaveAsync(HrmDefExamGroupInfosSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 2);
            await hrmDefExamGroupInfoRepository.BeginTransactionAsync();
            try
            {

                HrmDefExamGroupInfo entity = new HrmDefExamGroupInfo();
                entity.GroupCode = strMaxNO;
                entity.GroupName = entityVM.GroupName;
                entity.ShortName = entityVM.ShortName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await hrmDefExamGroupInfoRepository.AddAsync(entity);
                await hrmDefExamGroupInfoRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await hrmDefExamGroupInfoRepository.RollbackTransactionAsync();

                return false;
            }
        }

        public async Task<bool> UpdateAsync(HrmDefExamGroupInfosSetupViewModel entityVM)
        {
            await hrmDefExamGroupInfoRepository.BeginTransactionAsync();
            try
            {

                var entity = await hrmDefExamGroupInfoRepository.GetByIdAsync(entityVM.GroupCode);
                if (entity == null)
                {
                    await hrmDefExamGroupInfoRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.GroupCode = entityVM.GroupCode;
                entity.GroupName = entityVM.GroupName;
                entity.ShortName = entityVM.ShortName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await hrmDefExamGroupInfoRepository.UpdateAsync(entity);
                await hrmDefExamGroupInfoRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await hrmDefExamGroupInfoRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await hrmDefExamGroupInfoRepository.All().Where(x => ids.Contains(x.GroupCode)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            hrmDefExamGroupInfoRepository.Delete(entity);

            return true;
        }


        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await hrmDefExamGroupInfoRepository.All().AnyAsync(x => x.GroupCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hrmDefExamGroupInfoRepository.All().AnyAsync(x => x.GroupName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await hrmDefExamGroupInfoRepository.All().AnyAsync(x => x.GroupName == name && x.GroupCode!= typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Group Title" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Group Title" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Group Title" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Group Title" && x.CheckDelete);
        }
        #endregion
    }
}
