using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.JobTitles;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.JobTitles
{
    public class JobTitleService : AppService<HrmDefJobTitle>, IJobTitleService
    {
        private readonly IRepository<HrmDefJobTitle> JobTitleRepository;
        private readonly IRepository<CoreAccessCode> coreAccessCodeRepository;
        private readonly IDeleteHistoryService deleteHistoryService;

        private const string TableName = "HRM_Def_JobTitle";
        private const string ColumnName = "JobTitleId";

        public JobTitleService(
            IRepository<HrmDefJobTitle> JobTitleRepository,
            IRepository<CoreAccessCode> coreAccessCodeRepository,
            IDeleteHistoryService deleteHistoryService) 
     : base(JobTitleRepository)
        {
            this.JobTitleRepository = JobTitleRepository;
            this.coreAccessCodeRepository = coreAccessCodeRepository;
            this.deleteHistoryService = deleteHistoryService;
        }

        public async Task<JobTitleSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await JobTitleRepository.GetByIdAsync(code);
            if (entity == null)
            {
                return null;
            }
            return new JobTitleSetupViewModel

            {
                AutoId = entity.AutoId, //id
                JobTitleId = entity.JobTitleId,
                JobTitle = entity.JobTitle,

                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Luser = entity.Luser,
                Lip = entity.Lip,
                Lmac = entity.Lmac
            };
        }
        public async Task<List<JobTitleSetupViewModel>> GetAllAsync()
        {
            var entity = await JobTitleRepository.GetAllAsync();
            return entity.Select(entityVM => new JobTitleSetupViewModel
            {
                AutoId = entityVM.AutoId,
                JobTitleId = entityVM.JobTitleId,
                JobTitle = entityVM.JobTitle,

                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac

            }).ToList();
        }


        public async Task<bool> SaveAsync(JobTitleSetupViewModel entityVM, string CompanyCode)
        {
            await JobTitleRepository.BeginTransactionAsync();
            try
            {

                HrmDefJobTitle entity = new HrmDefJobTitle();

                entity.JobTitleId = await GenerateNextCode();
                entity.JobTitle = entityVM.JobTitle;

                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                entity.CompanyCode = CompanyCode;
                await JobTitleRepository.AddAsync(entity);
                await JobTitleRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await JobTitleRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> UpdateAsync(JobTitleSetupViewModel entityVM)
        {
            await JobTitleRepository.BeginTransactionAsync();
            try
            {

                var entity = await JobTitleRepository.GetByIdAsync(entityVM.JobTitleId);
                if (entity == null)
                {
                    await JobTitleRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.AutoId = entityVM.AutoId;
                entity.JobTitleId = entityVM.JobTitleId;
                entity.JobTitle = entityVM.JobTitle;

                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                await JobTitleRepository.UpdateAsync(entity);
                await JobTitleRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await JobTitleRepository.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<(bool succses, string messege)> DeleteTab(List<string> ids, DeleteHistoryViewModel model)
        {
            if (!ids.Any())
                return (false, "No Data found to delete");

            var dependencyCheck = await deleteHistoryService.CheckDependenciesAsync(
                JobTitleRepository.GetTableName(),
                ColumnName,
                ids.ToList()
            );

            if (!dependencyCheck.CanDelete)
                return (false, dependencyCheck.Message);

            await JobTitleRepository.BeginTransactionAsync();

            try
            {
                var entity = await JobTitleRepository.All().Where(x => ids.Contains(x.JobTitleId)).ToListAsync();


                JobTitleRepository.Delete(entity);
                model.tableName = TableName; 
                await deleteHistoryService.LogDeletedRecordsAsync(
                    entity, model
                );

                await JobTitleRepository.CommitTransactionAsync();
                return (true, "Delete Successfully");
            }
            catch (Exception ex)
            {
                await JobTitleRepository.RollbackTransactionAsync();
                Console.WriteLine(ex.ToString());
                return (false, "Delete Failed");

            }

        }

        public HrmDefJobTitle GetLeaveType(string code)
        {
            return JobTitleRepository.GetById(code);
        }

        public async Task<IEnumerable<CommonSelectModel>> SelectionHrmDefJobTitleAsync()
        {
            return await JobTitleRepository.All()
                      .Select(x => new CommonSelectModel
                      {
                          Code = x.JobTitleId,
                          Name = x.JobTitle,
                      })
                      .ToListAsync();
        }

        #region NextCode


        public async Task<string> GenerateNextCode()
        {

            var Code = await JobTitleRepository.GetAllAsync();
            var lastCode = Code.Max(b => b.JobTitleId);
            int nextCode = 1;
            if (!string.IsNullOrEmpty(lastCode))
            {
                int lastNumber = int.Parse(lastCode.TrimStart('0'));
                lastNumber++;
                nextCode = lastNumber;
            }
            return nextCode.ToString("D2");

        }

        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await JobTitleRepository.All().AnyAsync(x => x.JobTitleId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await JobTitleRepository.All().AnyAsync(x => x.JobTitle == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await JobTitleRepository.All().AnyAsync(x => x.JobTitle == name && x.JobTitleId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Job Title" && x.CheckDelete);
        }

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Job Title" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Job Title" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Job Title" && x.CheckEdit);

        }
        #endregion
    }
}
