using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefExamGroupInfos;
using GCTL.Core.ViewModels.HrmDefGradeTypes;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefGradeTypes
{
    public class HrmDefGradeTypesService:AppService<HrmDefGradeType>, IHrmDefGradeTypesService
    {

        private readonly IRepository<HrmDefGradeType> hrmDefGradeRepository;
        
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;
        private const string TableName = "HRM_Def_GradeType";
        private const string ColumnName = "GradeTypeID";

        public HrmDefGradeTypesService(IRepository<HrmDefGradeType> hrmDefGradeRepository, IRepository<CoreAccessCode> accessCodeRepository, ICommonService commonService) : base(hrmDefGradeRepository)
        {
            this.hrmDefGradeRepository = hrmDefGradeRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
           
        }

        public async Task<List<HrmDefGradeTypesSetupViewModel>> GetAllAsync()
        {
            var entity = await hrmDefGradeRepository.GetAllAsync();
            return entity.Select(entityVM => new HrmDefGradeTypesSetupViewModel
            {
                AutoId = entityVM.AutoId,
                GradeTypeId = entityVM.GradeTypeId,
                GradeType = entityVM.GradeType,
                ShortName = entityVM.ShortName,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,



            }).ToList();
        }

        public async Task<HrmDefGradeTypesSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await hrmDefGradeRepository.GetByIdAsync(code);
            if (entity == null) return null;

            HrmDefGradeTypesSetupViewModel entityVM = new HrmDefGradeTypesSetupViewModel();
            entityVM.AutoId = entity.AutoId;
            entityVM.GradeTypeId = entity.GradeTypeId;
            entityVM.GradeType = entity.GradeType;
            entityVM.ShortName = entity.ShortName;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        public async Task<IEnumerable<CommonSelectModel>> SelectionHrmDefGradeTypeAsync()
        {

            var data = await hrmDefGradeRepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.GradeTypeId,
                           Name = x.GradeType,
                       }).ToListAsync();
            return data;
        }


        public async Task<bool> SaveAsync(HrmDefGradeTypesSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 2);
            await hrmDefGradeRepository.BeginTransactionAsync();
            try
            {

                HrmDefGradeType entity = new HrmDefGradeType();
                entity.GradeTypeId = strMaxNO;
                entity.GradeType = entityVM.GradeType;
                entity.ShortName = entityVM.ShortName?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await hrmDefGradeRepository.AddAsync(entity);
                await hrmDefGradeRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await hrmDefGradeRepository.RollbackTransactionAsync();

                return false;
            }
        }

        public async Task<bool> UpdateAsync(HrmDefGradeTypesSetupViewModel entityVM)
        {
            await hrmDefGradeRepository.BeginTransactionAsync();
            try
            {

                var entity = await hrmDefGradeRepository.GetByIdAsync(entityVM.GradeTypeId);
                if (entity == null)
                {
                    await hrmDefGradeRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.GradeTypeId = entityVM.GradeTypeId;
                entity.GradeType = entityVM.GradeType;
                entity.ShortName = entityVM.ShortName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await hrmDefGradeRepository.UpdateAsync(entity);
                await hrmDefGradeRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await hrmDefGradeRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await hrmDefGradeRepository.All().Where(x => ids.Contains(x.GradeTypeId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            hrmDefGradeRepository.Delete(entity);

            return true;
        }


        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await hrmDefGradeRepository.All().AnyAsync(x => x.GradeTypeId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hrmDefGradeRepository.All().AnyAsync(x => x.GradeType == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await hrmDefGradeRepository.All().AnyAsync(x => x.GradeType == name && x.GradeTypeId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Grade Type" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Grade Type" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Grade Type" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Grade Type" && x.CheckDelete);
        }
        #endregion

    }
}
