using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.CourseTitle;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.CourseTitle
{
    public class CourseTitleService : AppService<HrmDefCourseTitle>, ICourseTitleService
    {
        private readonly IRepository<HrmDefCourseTitle> courseTitleRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        private readonly IDeleteHistoryService deleteHistoryService;

        string strMaxNO = string.Empty;
        private const string TableName = "HRM_Def_CourseTitle";
        private const string ColumnName = "CourseCode";

        public CourseTitleService(IRepository<HrmDefCourseTitle> courseTitleRepository, IRepository<CoreAccessCode> accessCodeRepository, ICommonService commonService, IDeleteHistoryService deleteHistoryService) : base(courseTitleRepository)
        {
            this.courseTitleRepository = courseTitleRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
            this.deleteHistoryService = deleteHistoryService;
        }

        public async Task<List<CourseTitleSetupViewModel>> GetAllAsync()
        {
            var entity = await courseTitleRepository.GetAllAsync();
            return entity.Select(entityVM => new CourseTitleSetupViewModel
            {
                AutoId = entityVM.AutoId,
                CourseCode = entityVM.CourseCode,
                CourseName = entityVM.CourseName,
                ShortName = entityVM.ShortName,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,



            }).ToList();
        }

        public async Task<CourseTitleSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await courseTitleRepository.GetByIdAsync(code);
            if (entity == null) return null;

            CourseTitleSetupViewModel entityVM = new CourseTitleSetupViewModel();
            entityVM.AutoId = entity.AutoId;
            entityVM.CourseCode = entity.CourseCode;
            entityVM.CourseName = entity.CourseName;
            entityVM.ShortName = entity.ShortName;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        public async Task<IEnumerable<CommonSelectModel>> SelectionCourseTitleAsync()
        {

            var data = await courseTitleRepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.CourseCode,
                           Name = x.CourseName,
                       }).ToListAsync();
            return data;
        }

        public async Task<bool> SaveAsync(CourseTitleSetupViewModel entityVM, string CompanyCode)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            await courseTitleRepository.BeginTransactionAsync();
            try
            {

                HrmDefCourseTitle entity = new HrmDefCourseTitle();
                entity.CourseCode = strMaxNO;
                entity.CourseName = entityVM.CourseName;
                entity.ShortName = entityVM.ShortName ?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                entity.CompanyCode = CompanyCode ?? string.Empty;
                entity.UserInfoEmployeeId = entityVM.UserInfoEmployeeId ?? string.Empty;
                await courseTitleRepository.AddAsync(entity);
                await courseTitleRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await courseTitleRepository.RollbackTransactionAsync();

                return false;
            }
        }

        public async Task<bool> UpdateAsync(CourseTitleSetupViewModel entityVM)
        {
            await courseTitleRepository.BeginTransactionAsync();
            try
            {

                var entity = await courseTitleRepository.GetByIdAsync(entityVM.CourseCode);
                if (entity == null)
                {
                    await courseTitleRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.CourseCode = entityVM.CourseCode;
                entity.CourseName = entityVM.CourseName;
                entity.ShortName = entityVM.ShortName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await courseTitleRepository.UpdateAsync(entity);
                await courseTitleRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await courseTitleRepository.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<(bool succses, string messege)> DeleteTab(List<string> ids, DeleteHistoryViewModel model)
        {
            if (!ids.Any())
                return (false, "No Data found to delete");

            var alternateColumn = new List<string> { "CourseTitleCode" };
            var dependencyCheck = await deleteHistoryService.CheckDependenciesAsync(
                courseTitleRepository.GetTableName(),
                ColumnName,
                ids,
                alternateColumn
            );

            if (!dependencyCheck.CanDelete)
                return (false, dependencyCheck.Message);

            await courseTitleRepository.BeginTransactionAsync();

            try
            {
                var entity = await courseTitleRepository.All().Where(x => ids.Contains(x.CourseCode)).ToListAsync();


                courseTitleRepository.Delete(entity);
                model.tableName = TableName;
                await deleteHistoryService.LogDeletedRecordsAsync(
                    entity, model
                );

                await courseTitleRepository.CommitTransactionAsync();
                return (true, "Delete Successfully");
            }
            catch (Exception ex)
            {
                await courseTitleRepository.RollbackTransactionAsync();
                Console.WriteLine(ex.ToString());
                return (false, "Delete Failed");

            }

        }

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await courseTitleRepository.All().AnyAsync(x => x.CourseCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await courseTitleRepository.All().AnyAsync(x => x.CourseName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await courseTitleRepository.All().AnyAsync(x => x.CourseName == name && x.CourseCode != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Course Title" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Course Title" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Course Title" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Course Title" && x.CheckDelete);
        }
        #endregion
    }
}
