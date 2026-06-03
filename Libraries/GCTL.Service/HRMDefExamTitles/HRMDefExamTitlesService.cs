using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HRMDefBoardCountryNames;
using GCTL.Core.ViewModels.HRMDefExamTitles;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRMDefExamTitles
{
    public class HRMDefExamTitlesService:AppService<HrmDefExamTitle>, IHRMDefExamTitlesService
    {
        private readonly IRepository<HrmDefExamTitle> hrmDefExamTitleRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;
        private const string TableName = "HRM_Def_ExamTitle";
        private const string ColumnName = "ExamTitleCode";

     
        public HRMDefExamTitlesService(IRepository<HrmDefExamTitle> hrmDefExamTitleRepository, IRepository<CoreAccessCode> accessCodeRepository, ICommonService commonService):base(hrmDefExamTitleRepository)
        {
            this.hrmDefExamTitleRepository = hrmDefExamTitleRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        public async Task<List<HRMDefExamTitlesSetupViewModel>> GetAllAsync()
        {
            var entity = await hrmDefExamTitleRepository.GetAllAsync();
            return entity.Select(entityVM => new HRMDefExamTitlesSetupViewModel
            {
                AutoId = entityVM.AutoId,
                ExamTitleCode = entityVM.ExamTitleCode,
                ExamTitleName = entityVM.ExamTitleName,
                ShortName = entityVM.ShortName,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,
               


            }).ToList();
        }

        public async Task<HRMDefExamTitlesSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await hrmDefExamTitleRepository.GetByIdAsync(code);
            if (entity == null) return null;

            HRMDefExamTitlesSetupViewModel entityVM = new HRMDefExamTitlesSetupViewModel();
            entityVM.AutoId = entity.AutoId;
            entityVM.ExamTitleCode = entity.ExamTitleCode;
            entityVM.ExamTitleName = entity.ExamTitleName;
            entityVM.ShortName = entity.ShortName;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;
           
            return entityVM;
        }

        public async Task< IEnumerable<CommonSelectModel>> SelectionHrmDefExamTitleTypeAsync()
        {

            var data = await hrmDefExamTitleRepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.ExamTitleCode,
                           Name = x.ExamTitleName
                       }).ToListAsync();
            return data;
        }


        public async Task<bool> SaveAsync(HRMDefExamTitlesSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 2);
            await hrmDefExamTitleRepository.BeginTransactionAsync();
            try
            {

                HrmDefExamTitle entity = new HrmDefExamTitle();
                entity.ExamTitleCode = strMaxNO;
                entity.ExamTitleName = entityVM.ExamTitleName;
                entity.ShortName = entityVM.ShortName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await hrmDefExamTitleRepository.AddAsync(entity);
                await hrmDefExamTitleRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await hrmDefExamTitleRepository.RollbackTransactionAsync();

                return false;
            }
        }

        public async Task<bool> UpdateAsync(HRMDefExamTitlesSetupViewModel entityVM)
        {
            await hrmDefExamTitleRepository.BeginTransactionAsync();
            try
            {

                var entity = await hrmDefExamTitleRepository.GetByIdAsync(entityVM.ExamTitleCode);
                if (entity == null)
                {
                    await hrmDefExamTitleRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.ExamTitleCode = entityVM.ExamTitleCode;
                entity.ExamTitleName = entityVM.ExamTitleName;
                entity.ShortName = entityVM.ShortName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await hrmDefExamTitleRepository.UpdateAsync(entity);
                await hrmDefExamTitleRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await hrmDefExamTitleRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await hrmDefExamTitleRepository.All().Where(x => ids.Contains(x.ExamTitleCode)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            hrmDefExamTitleRepository.Delete(entity);

            return true;
        }


        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await hrmDefExamTitleRepository.All().AnyAsync(x => x.ExamTitleCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hrmDefExamTitleRepository.All().AnyAsync(x => x.ExamTitleName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await hrmDefExamTitleRepository.All().AnyAsync(x => x.ExamTitleName == name && x.ExamTitleCode != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Exam Title" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Exam Title" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Exam Title" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Exam Title" && x.CheckDelete);
        }
        #endregion
    }
}
