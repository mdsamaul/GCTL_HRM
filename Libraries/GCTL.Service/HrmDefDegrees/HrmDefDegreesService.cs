using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HolidayTypes;
using GCTL.Core.ViewModels.HrmDefDegrees;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.HolidayTypes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefDegrees
{
    public class HrmDefDegreesService : AppService<HrmDefDegree>, IHrmDefDegreesService
    {
        private readonly IRepository<HrmDefDegree> hrmdefDegreeepository; 
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        string strMaxNO = " ";
        public HrmDefDegreesService(IRepository<HrmDefDegree> hrmdefDegreeepository, ICommonService commonService, IRepository<CoreAccessCode> accessCodeRepository):base(hrmdefDegreeepository)
        {
            this.hrmdefDegreeepository = hrmdefDegreeepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }
       
        public async Task<List<HrmDefDegreesSetupViewModel>> GetAllAsync()
        {
            var entity = await hrmdefDegreeepository.GetAllAsync();
            return entity.Select(entityVM => new HrmDefDegreesSetupViewModel
            {
                AutoId = entityVM.AutoId,
                DegreeCode = entityVM.DegreeCode,
                DegreeName = entityVM.DegreeName,
                ShortName = entityVM.ShortName,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,
                UserInfoEmployeeId = entityVM.UserInfoEmployeeId,
              

            }).ToList();
        }
       
        public async Task<HrmDefDegreesSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await hrmdefDegreeepository.GetByIdAsync(code);
            if (entity == null) return null;

            HrmDefDegreesSetupViewModel entityVM = new HrmDefDegreesSetupViewModel();
            entityVM.AutoId = entity.AutoId;
            entityVM.DegreeCode = entity.DegreeCode;
            entityVM.DegreeName = entity.DegreeName;
            entityVM.ShortName = entity.ShortName;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;
            entityVM.UserInfoEmployeeId = entity.UserInfoEmployeeId;
            return entityVM;
        }

        public async Task< IEnumerable<CommonSelectModel>> SelectionHrmDefDegreeTypeAsync()
        {

            var data =await hrmdefDegreeepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.DegreeCode,
                           Name = x.DegreeName
                       }).ToListAsync();
            return data;
        }


        public async Task<bool> SaveAsync(HrmDefDegreesSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, "DegreeCode", "HRM_Def_Degree", 3);
            await hrmdefDegreeepository.BeginTransactionAsync();
            try
            {  

                HrmDefDegree entity = new HrmDefDegree();
                entity.DegreeCode =strMaxNO;
                entity.DegreeName = entityVM.DegreeName;
                entity.ShortName = entityVM.ShortName ;
                entity.Luser = entityVM.Luser ;
                entity.Lip = entityVM.Lip ;
                entity.UserInfoEmployeeId = entityVM.UserInfoEmployeeId;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode;
                entity.Ldate = DateTime.Now;
                await hrmdefDegreeepository.AddAsync(entity);
                await hrmdefDegreeepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await hrmdefDegreeepository.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<bool> UpdateAsync(HrmDefDegreesSetupViewModel entityVM)
        {
            await hrmdefDegreeepository.BeginTransactionAsync();
            try
            {

                var entity = await hrmdefDegreeepository.GetByIdAsync(entityVM.DegreeCode);
                if (entity == null)
                {
                    await hrmdefDegreeepository.RollbackTransactionAsync();
                    return false;
                }
                entity.DegreeCode = entityVM.DegreeCode;
                entity.DegreeName = entityVM.DegreeName;
                entity.ShortName = entityVM.ShortName ;
                entity.CompanyCode = entityVM.CompanyCode;
                entity.Luser = entityVM.Luser ;
                entity.UserInfoEmployeeId = entityVM.UserInfoEmployeeId;
                entity.Lip = entityVM.Lip ;
                entity.Lmac = entityVM.Lmac ;
                entity.ModifyDate = DateTime.Now;
                await hrmdefDegreeepository.UpdateAsync(entity);
                await hrmdefDegreeepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await hrmdefDegreeepository.RollbackTransactionAsync();
                return false;
            }
        }

      
        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity =await hrmdefDegreeepository.All().Where(x => ids.Contains(x.DegreeCode)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            hrmdefDegreeepository.Delete(entity);

            return true;
        }


        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await hrmdefDegreeepository.All().AnyAsync(x => x.DegreeCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hrmdefDegreeepository.All().AnyAsync(x => x.DegreeName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await hrmdefDegreeepository.All().AnyAsync(x => x.DegreeName == name && x.DegreeCode != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Degree Title" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Degree Title" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Degree Title" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Degree Title" && x.CheckDelete);
        }
        #endregion
    }
}

