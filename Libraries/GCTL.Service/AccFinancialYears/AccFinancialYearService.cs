using GCTL.Core.Data;
using GCTL.Core.ViewModels.AccFinancialYears;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.AccFinancialYears
{
    public class AccFinancialYearService : AppService<AccFinancialYear>, IAccFinancialYearService
    {
        private readonly IRepository<AccFinancialYear> yearRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;
        private const string TableName = "Acc_FinancialYear";
        private const string ColumnName = "FinancialCodeNo";

        public AccFinancialYearService
            (
            IRepository<AccFinancialYear> yearRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService
            ) : base(yearRepo)
        {
            this.yearRepo = yearRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        public async Task<List<AccFinancialYearSetupViewModel>> GetAllAsync()
        {
            var entity = await yearRepo.GetAllAsync();
            return entity.Select(entityVM => new AccFinancialYearSetupViewModel
            {
                Tc = entityVM.Tc,
                FinancialCodeNo = entityVM.FinancialCodeNo,
                StartDate = entityVM.StartDate,
                EndDate = entityVM.EndDate,
                Name = entityVM.Name,

                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,
            }).ToList();
        }

        public async Task<AccFinancialYearSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await yearRepo.All().Where(x => x.FinancialCodeNo == code).FirstOrDefaultAsync();

            if (entity == null) return null;

            AccFinancialYearSetupViewModel entityVM = new AccFinancialYearSetupViewModel();
            entityVM.Tc = entity.Tc;
            entityVM.FinancialCodeNo = entity.FinancialCodeNo;
            entityVM.Name = entity.Name;
            entityVM.StartDate = entity.StartDate;
            entityVM.EndDate = entity.EndDate;

            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        public async Task<bool> IsExistAcync(DateTime? startDate, DateTime? endDate, string typeCode)
        {
            return await yearRepo.All().AnyAsync(x => x.StartDate == startDate && x.EndDate == endDate && x.FinancialCodeNo != typeCode);
        }

        public async Task<bool> SaveAsync(AccFinancialYearSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            await yearRepo.BeginTransactionAsync();
            try
            {
                AccFinancialYear entity = new AccFinancialYear();
                entity.FinancialCodeNo = strMaxNO;
                entity.Name = entityVM.Name;
                entity.StartDate = entityVM.StartDate;
                entity.EndDate = entityVM.EndDate;

                entity.Luser = entityVM.Luser;
                entity.Ldate = entityVM.Ldate;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;

                await AddAsync(entity);
                await yearRepo.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await yearRepo.RollbackTransactionAsync();

                return false;
            }
        }

        public async Task<bool> UpdateAsync(AccFinancialYearSetupViewModel entityVM)
        {
            await yearRepo.BeginTransactionAsync();
            try
            {

                var entity = await yearRepo.GetByIdAsync(entityVM.Tc);
                if (entity == null)
                {
                    await yearRepo.RollbackTransactionAsync();
                    return false;
                }
                entity.FinancialCodeNo = entityVM.FinancialCodeNo;
                entity.Name = entityVM.Name;
                entity.StartDate = entityVM.StartDate;
                entity.EndDate = entityVM.EndDate;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await yearRepo.UpdateAsync(entity);
                await yearRepo.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await yearRepo.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await yearRepo.All().Where(x => ids.Contains(x.FinancialCodeNo)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            yearRepo.Delete(entity);

            return true;
        }

        #region SelectionAsync
        public async Task<IEnumerable<CommonSelectModel>> SelectionFinancialYearAsync()
        {

            var data = await yearRepo.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.FinancialCodeNo,
                           Name = x.Name,
                       }).ToListAsync();
            return data;
        }

        #endregion

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Financial Year" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Financial Year " && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Financial Year" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Financial Year" && x.CheckDelete);
        }
    }
}
