using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.Grades;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.Grades
{
    public class GradesService : AppService<HrmDefGrade>, IGradesService
    {
        private readonly IRepository<HrmDefGrade> graderepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<HrmDefGradeType> hrmDefGradeTypeRepository;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;
        private const string TableName = "HRM_Def_Grade";
        private const string ColumnName = "GradeCode";
        public GradesService(IRepository<HrmDefGrade> graderepository, IRepository<HrmDefGradeType> hrmDefGradeTypeRepository,IRepository<CoreAccessCode> accessCodeRepository, ICommonService commonService) : base(graderepository)
        {
            this.graderepository = graderepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
            this.hrmDefGradeTypeRepository = hrmDefGradeTypeRepository;
        }

        public async Task<List<GradesSetupViewModel>> GetAllAsync()
        {
            var data =await (from g in graderepository.All().AsNoTracking()
                             join gt in hrmDefGradeTypeRepository.All().Select(x=>new {x.GradeTypeId,x.GradeType }).AsNoTracking()
                             on g.GradeTypeId equals gt.GradeTypeId into gGTJoin
                             from gt in gGTJoin.DefaultIfEmpty()
                             select new GradesSetupViewModel
                             {
                                 AutoId = g.AutoId,
                                 GradeCode = g.GradeCode,
                                 GradeTypeId=g.GradeTypeId,
                                 GradeTypeName=gt.GradeType,
                                 GradeName = g.GradeName,
                                 GradeShortName = g.GradeShortName,
                                 FromGrossSalary = g.FromGrossSalary,
                                 ToGrossSalary = g.ToGrossSalary,
                                 Ldate = g.Ldate,
                                 ModifyDate = g.ModifyDate,
                                 Luser = g.Luser,
                                 Lip = g.Lip,
                                 Lmac = g.Lmac,
                             }).ToListAsync();

            return data;
        }

       

        public async Task<GradesSetupViewModel> GetByIdAsync(string code)
        {
            // Fetch the entity from the grade repository
            var entity = await graderepository.All()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GradeCode == code);

            if (entity == null) return null;

            // Fetch the related GradeTypeName
            var gradeType = await hrmDefGradeTypeRepository.All()
                .AsNoTracking()
                .Where(gt => gt.GradeTypeId == entity.GradeTypeId)
                .Select(gt => gt.GradeType)
                .FirstOrDefaultAsync();

            // Map entity to the view model
            GradesSetupViewModel entityVM = new GradesSetupViewModel
            {
                AutoId = entity.AutoId,
                GradeCode = entity.GradeCode,
                GradeTypeId = entity.GradeTypeId,
                GradeTypeName = gradeType, 
                GradeName = entity.GradeName,
                GradeShortName = entity.GradeShortName,
                FromGrossSalary = entity.FromGrossSalary,
                ToGrossSalary = entity.ToGrossSalary,
                Luser = entity.Luser,
                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Lip = entity.Lip,
                Lmac = entity.Lmac
            };

            return entityVM;
        }


        public async Task<IEnumerable<CommonSelectModel>> SelectionGradesTypeAsync()
        {

            var data = await graderepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.GradeCode,
                           Name = x.GradeName,
                       }).ToListAsync();
            return data;
        }


        public async Task<bool> SaveAsync(GradesSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 2);
            await graderepository.BeginTransactionAsync();
            try
            {

                HrmDefGrade entity = new HrmDefGrade();
                entity.GradeCode = strMaxNO;
                entity.GradeName = entityVM.GradeName ?? string.Empty;
                entity.GradeShortName = entityVM.GradeShortName ?? string.Empty;
                entity.FromGrossSalary = entityVM.FromGrossSalary ?? decimal.Zero;
                entity.GradeTypeId=entityVM.GradeTypeId ?? string.Empty;
                entity.ToGrossSalary = entityVM.ToGrossSalary ?? decimal.Zero;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await graderepository.AddAsync(entity);
                await graderepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await graderepository.RollbackTransactionAsync();

                return false;
            }
        }

        public async Task<bool> UpdateAsync(GradesSetupViewModel entityVM)
        {
            await graderepository.BeginTransactionAsync();
            try
            {

                var entity = await graderepository.GetByIdAsync(entityVM.GradeCode);
                if (entity == null)
                {
                    await graderepository.RollbackTransactionAsync();
                    return false;
                }
                entity.GradeCode = entityVM.GradeCode;
                entity.GradeTypeId = entityVM.GradeTypeId?? string.Empty;
                entity.GradeName = entityVM.GradeName?? string.Empty;
                entity.GradeShortName = entityVM.GradeShortName ?? string.Empty;
                entity.FromGrossSalary = entityVM.FromGrossSalary?? decimal.Zero;
                entity.ToGrossSalary = entityVM.ToGrossSalary?? decimal.Zero;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                await graderepository.UpdateAsync(entity);
                await graderepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await graderepository.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await graderepository.All().Where(x => ids.Contains(x.GradeCode)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            graderepository.Delete(entity);

            return true;
        }


        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await graderepository.All().AnyAsync(x => x.GradeCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await graderepository.All().AnyAsync(x => x.GradeName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await graderepository.All().AnyAsync(x => x.GradeName == name && x.GradeCode != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Grade" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Grade" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Grade" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Grade" && x.CheckDelete);
        }
        #endregion
    }
}
