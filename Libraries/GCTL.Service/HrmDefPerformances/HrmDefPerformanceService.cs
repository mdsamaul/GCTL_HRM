using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefPerformance;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefPerformances
{
    public class HrmDefPerformanceService : AppService<HrmDefPerformance>, IHrmDefPerformanceService
    {
        private readonly IRepository<HrmDefPerformance> hRmDefPerformanceRepository;
        private readonly IRepository<CoreAccessCode> coreAccessCodeRepository;

        public HrmDefPerformanceService(IRepository<HrmDefPerformance> hRmDefPerformanceRepository, IRepository<CoreAccessCode> coreAccessCodeRepository)
 : base(hRmDefPerformanceRepository)
        {
            this.hRmDefPerformanceRepository = hRmDefPerformanceRepository;
            this.coreAccessCodeRepository = coreAccessCodeRepository;
        }

        public async Task<HrmDefPerformanceSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await hRmDefPerformanceRepository.GetByIdAsync(code);
            if (entity == null)
            {
                return null;
            }
            return new HrmDefPerformanceSetupViewModel

            {
                AutoId = entity.AutoId, //id
                PerformanceCode = entity.PerformanceCode,
                JobTitleId = entity.JobTitleId,
                JobTitle = entity.JobTitleId,
                Performance = entity.Performance,
                PerformanceShortName = entity.PerformanceShortName,

                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Luser = entity.Luser,
                Lip = entity.Lip,
                Lmac = entity.Lmac
            };
        }
        public async Task<List<HrmDefPerformanceSetupViewModel>> GetAllAsync()
        {
            var entity = await hRmDefPerformanceRepository.GetAllAsync();
            return entity.Select(entityVM => new HrmDefPerformanceSetupViewModel
            {
                AutoId = entityVM.AutoId,
                PerformanceCode = entityVM.PerformanceCode,
                JobTitleId = entityVM.JobTitleId,
                JobTitle = entityVM.JobTitleId,
                Performance = entityVM.Performance,
                PerformanceShortName = entityVM.PerformanceShortName,

                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac

            }).ToList();
        }


        public async Task<bool> SaveAsync(HrmDefPerformanceSetupViewModel entityVM)
        {
            await hRmDefPerformanceRepository.BeginTransactionAsync();
            try
            {

                HrmDefPerformance entity = new HrmDefPerformance();

                entity.PerformanceCode = await GenerateNextCode();
                entity.JobTitleId = entityVM.JobTitleId;
                entity.JobTitleId = entityVM.JobTitle;
                entity.Performance = entityVM.Performance;
                entity.PerformanceShortName = entityVM.PerformanceShortName;

                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                entity.ModifyDate = DateTime.Now;
                await hRmDefPerformanceRepository.AddAsync(entity);
                await hRmDefPerformanceRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await hRmDefPerformanceRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> UpdateAsync(HrmDefPerformanceSetupViewModel entityVM)
        {
            await hRmDefPerformanceRepository.BeginTransactionAsync();
            try
            {

                var entity = await hRmDefPerformanceRepository.GetByIdAsync(entityVM.JobTitleId);
                if (entity == null)
                {
                    await hRmDefPerformanceRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.AutoId = entityVM.AutoId;
                entity.PerformanceCode = entityVM.PerformanceCode ?? string.Empty;
                entity.JobTitleId = entityVM.JobTitleId;
                entity.JobTitleId = entityVM.JobTitle;
                entity.Performance = entityVM.Performance ?? string.Empty;
                entity.PerformanceShortName = entityVM.PerformanceShortName ?? string.Empty;

                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                await hRmDefPerformanceRepository.UpdateAsync(entity);
                await hRmDefPerformanceRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await hRmDefPerformanceRepository.RollbackTransactionAsync();
                return false;
            }
        }

        public bool DeleteLeaveType(string id)
        {
            var entity = GetLeaveType(id);
            if (entity != null)
            {
                hRmDefPerformanceRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public HrmDefPerformance GetLeaveType(string code)
        {
            return hRmDefPerformanceRepository.GetById(code);
        }

        public async Task<IEnumerable<CommonSelectModel>> SelectionHrmDefPerformanceAsync()
        {
            return await hRmDefPerformanceRepository.All()
                      .Select(x => new CommonSelectModel
                      {
                          Code = x.JobTitleId,
                          Name = x.PerformanceShortName,
                      })
                      .ToListAsync();
        }

        #region NextCode


        public async Task<string> GenerateNextCode()
        {

            var Code = await hRmDefPerformanceRepository.GetAllAsync();
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
            return await hRmDefPerformanceRepository.All().AnyAsync(x => x.JobTitleId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hRmDefPerformanceRepository.All().AnyAsync(x => x.JobTitleId == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await hRmDefPerformanceRepository.All().AnyAsync(x => x.JobTitleId == name && x.JobTitleId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Performance Entry" && x.CheckDelete);
        }

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Performance Entry" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Performance Entry" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Performance Entry" && x.CheckEdit);

        }
        #endregion
    }
}
