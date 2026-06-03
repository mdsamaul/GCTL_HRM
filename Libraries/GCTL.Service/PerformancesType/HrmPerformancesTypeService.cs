using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefPerformance2;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.PerformancesType
{
    public class HrmPerformancesTypeService : AppService<HrmDefPerformance>, IHrmPerformancesTypeService
    {
        private readonly IRepository<HrmDefPerformance> HrmPerformancesTypeRepository;
        private readonly IRepository<HrmDefJobTitle> JobTitleRepository;
        private readonly IRepository<CoreAccessCode> coreAccessCodeRepository;

        public HrmPerformancesTypeService(IRepository<HrmDefPerformance> HrmPerformancesTypeRepository, IRepository<CoreAccessCode> coreAccessCodeRepository, IRepository<HrmDefJobTitle> JobTitleRepository)
 : base(HrmPerformancesTypeRepository)
        {
            this.HrmPerformancesTypeRepository = HrmPerformancesTypeRepository;
            this.JobTitleRepository = JobTitleRepository;
            this.coreAccessCodeRepository = coreAccessCodeRepository;
        }


        public Task<List<HrmDefPerformance2SetupViewModel>> GetByIdJobTitleAsync(string jobTitleCode)
        {
            var query = (from performance in HrmPerformancesTypeRepository.All()


                         join hrm in JobTitleRepository.All()
                         on performance.JobTitleId equals hrm.JobTitle into JobGroup
                         from hrm in JobGroup.DefaultIfEmpty()

                         select new HrmDefPerformance2SetupViewModel
                         {
                             AutoId = performance.AutoId,
                             PerformanceCode = performance.PerformanceCode,
                             JobTitleId = performance.JobTitleId,
                             JobTitle = hrm.JobTitle,
                             Performance = performance.Performance,
                             PerformanceShortName = performance.PerformanceShortName,
                             Ldate = performance.Ldate,
                             ModifyDate = performance.ModifyDate,
                             Luser = performance.Luser,
                             Lip = performance.Lip,
                             Lmac = performance.Lmac
                         }).Where(x => x.JobTitleId == jobTitleCode).ToListAsync();

            return query;
        }



        public async Task<HrmDefPerformance2SetupViewModel> GetByIdAsync(string code)
        {
            var entity = await HrmPerformancesTypeRepository.GetByIdAsync(code);
            if (entity == null)
            {
                return null;
            }
            return new HrmDefPerformance2SetupViewModel
            {
                AutoId = entity.AutoId, //id
                PerformanceCode = entity.PerformanceCode,
                JobTitleId = entity.JobTitleId,
                Performance = entity.Performance,
                PerformanceShortName = entity.PerformanceShortName,

                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Luser = entity.Luser,
                Lip = entity.Lip,
                Lmac = entity.Lmac
            };
        }

        public async Task<List<HrmDefPerformance2SetupViewModel>> GetAllAsync()
        {
            var query = from performance in HrmPerformancesTypeRepository.All()

                        join hrm in JobTitleRepository.All()
                         on performance.JobTitleId equals hrm.JobTitle into JobGroup
                        from hrm in JobGroup.DefaultIfEmpty()


                        select new HrmDefPerformance2SetupViewModel
                        {
                            AutoId = performance.AutoId,
                            PerformanceCode = performance.PerformanceCode,
                            JobTitleId = performance.JobTitleId,
                            JobTitle = hrm.JobTitle,
                            Performance = performance.Performance,
                            PerformanceShortName = performance.PerformanceShortName,
                            Ldate = performance.Ldate,
                            ModifyDate = performance.ModifyDate,
                            Luser = performance.Luser,
                            Lip = performance.Lip,
                            Lmac = performance.Lmac
                        };

            return await query.ToListAsync();
        }


        public async Task<bool> SaveAsync(HrmDefPerformance2SetupViewModel entityVM)
        {
            await HrmPerformancesTypeRepository.BeginTransactionAsync();
            try
            {

                HrmDefPerformance entity = new HrmDefPerformance();

                entity.PerformanceCode = await GenerateNextCode();
                entity.JobTitleId = entityVM.JobTitleId;
                entity.Performance = entityVM.Performance;
                entity.PerformanceShortName = entityVM.PerformanceShortName;

                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                entity.ModifyDate = DateTime.Now;
                await HrmPerformancesTypeRepository.AddAsync(entity);
                await HrmPerformancesTypeRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await HrmPerformancesTypeRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> UpdateAsync(HrmDefPerformance2SetupViewModel entityVM)
        {
            await HrmPerformancesTypeRepository.BeginTransactionAsync();
            try
            {

                var entity = await HrmPerformancesTypeRepository.GetByIdAsync(entityVM.PerformanceCode);
                if (entity == null)
                {
                    await HrmPerformancesTypeRepository.RollbackTransactionAsync();
                    return false;
                }

                entity.PerformanceCode = entityVM.PerformanceCode;
                entity.JobTitleId = entityVM.JobTitleId;
                entity.Performance = entityVM.Performance ?? string.Empty;
                entity.PerformanceShortName = entityVM.PerformanceShortName ?? string.Empty;

                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                await HrmPerformancesTypeRepository.UpdateAsync(entity);
                await HrmPerformancesTypeRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await HrmPerformancesTypeRepository.RollbackTransactionAsync();
                return false;
            }
        }

        public bool DeleteLeaveType(string id)
        {
            var entity = GetLeaveType(id)
;
            if (entity != null)
            {
                HrmPerformancesTypeRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public HrmDefPerformance GetLeaveType(string code)
        {
            return HrmPerformancesTypeRepository.GetById(code);
        }

        public async Task<IEnumerable<CommonSelectModel>> SelectionPerformancesTypeAsync()
        {
            return await HrmPerformancesTypeRepository.All()
                      .Select(x => new CommonSelectModel
                      {
                          Code = x.PerformanceCode,
                          Name = x.Performance,
                      })
                      .ToListAsync();
        }

        #region NextCode


        public async Task<string> GenerateNextCode()
        {

            var Code = await HrmPerformancesTypeRepository.GetAllAsync();
            var lastCode = Code.Max(b => b.PerformanceCode);
            int nextCode = 1;
            if (!string.IsNullOrEmpty(lastCode))
            {
                int lastNumber = int.Parse(lastCode.TrimStart('0'));
                lastNumber++;
                nextCode = lastNumber;
            }
            return nextCode.ToString("D3");

        }
        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await HrmPerformancesTypeRepository.All().AnyAsync(x => x.PerformanceCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await HrmPerformancesTypeRepository.All().AnyAsync(x => x.Performance == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await HrmPerformancesTypeRepository.All().AnyAsync(x => x.Performance == name && x.PerformanceCode != typeCode);
        }

        #endregion
        #region Permission all type
        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Performance" && x.CheckDelete);
        }

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Performance" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Performance" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Performance" && x.CheckEdit);

        }

        #endregion
    }
}