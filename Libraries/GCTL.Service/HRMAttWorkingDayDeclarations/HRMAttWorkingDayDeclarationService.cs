using GCTL.Core.Data;
using GCTL.Core.ViewModels.HrmAttWorkingDayDeclarations;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GCTL.Service.HRMAttWorkingDayDeclarations
{
    public class HRMAttWorkingDayDeclarationService : AppService<HrmAttWorkingDayDeclaration>, IHRMAttWorkingDayDeclarationService
    {
        private readonly IRepository<HrmAttWorkingDayDeclaration> repository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public HRMAttWorkingDayDeclarationService(IRepository<HrmAttWorkingDayDeclaration> repository, IRepository<CoreAccessCode> accessCodeRepository) : base(repository)
        {
            this.repository = repository;
            this.accessCodeRepository = accessCodeRepository;
        }


        public async Task<HrmAttWorkingDayDeclarationSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await repository.GetByIdAsync(code);
            if (entity == null)
            {
                return null;
            }
            return new HrmAttWorkingDayDeclarationSetupViewModel

            {
                AutoId = entity.Tc, //id
                WorkingDayCode = entity.WorkingDayCode,
                //WorkingDayDate = entity.WorkingDayDate,
                Remarks = entity.Remarks,
                CompanyCode = entity.CompanyCode,
                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Luser = entity.Luser,
                Lip = entity.Lip,
                Lmac = entity.Lmac
            };
        }
        public async Task<List<HrmAttWorkingDayDeclarationSetupViewModel>> GetAllAsync()
        {
            var entity = await repository.GetAllAsync();
            return entity.Select(entityVM => new HrmAttWorkingDayDeclarationSetupViewModel
            {
                AutoId = entityVM.Tc,
                WorkingDayCode = entityVM.WorkingDayCode,
                //WorkingDayDate = entityVM.WorkingDayDate,
                Remarks = entityVM.Remarks,
                CompanyCode = entityVM.CompanyCode,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac

            }).ToList();
        }


        public async Task<bool> SaveAsync(HrmAttWorkingDayDeclarationSetupViewModel entityVM)
        {
            await repository.BeginTransactionAsync();
            try
            {

                HrmAttWorkingDayDeclaration entity = new HrmAttWorkingDayDeclaration();
                entity.WorkingDayCode = await GenerateNextCode();
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.WorkingDayDate = entityVM.WorkingDayDate;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.Remarks = entityVM.Remarks ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await repository.AddAsync(entity);
                await repository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await repository.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<bool> UpdateAsync(HrmAttWorkingDayDeclarationSetupViewModel entityVM)
        {
            await repository.BeginTransactionAsync();
            try
            {

                var entity = await repository.GetByIdAsync(entityVM.WorkingDayCode);
                if (entity == null)
                {
                    await repository.RollbackTransactionAsync();
                    return false;
                }
                entity.WorkingDayCode = entityVM.WorkingDayCode;
                entity.WorkingDayDate = entityVM.WorkingDayDate;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Remarks = entityVM.Remarks ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                await repository.UpdateAsync(entity);
                await repository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await repository.RollbackTransactionAsync();
                return false;
            }
        }


        public bool DeleteLeaveType(string id)
        {
            var entity = GetLeaveType(id);
            if (entity != null)
            {
                repository.Delete(entity);
                return true;
            }
            return false;
        }

        public HrmAttWorkingDayDeclaration GetLeaveType(string code)
        {
            return repository.GetById(code);
        }


        public async Task<string> GenerateNextCode()
        {

            var Code = await repository.GetAllAsync();
            var lastCode = Code.Max(b => b.WorkingDayCode);
            int nextCode = 1;
            if (!string.IsNullOrEmpty(lastCode))
            {
                int lastNumber = int.Parse(lastCode.TrimStart('0'));
                lastNumber++;
                nextCode = lastNumber;
            }
            return nextCode.ToString("D6");

        }


        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await repository.All().AnyAsync(x => x.WorkingDayCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await repository.All().AnyAsync(x => x.Remarks == name);
        }

        //public async Task<bool> IsExistAsync(string workingDayDate, string typeCode)
        //{
        //    return await repository.All().AnyAsync(x => x.WorkingDayDate.Date == workingDayDate && x.WorkingDayCode != typeCode);
        //}
        public async Task<bool> IsExistAsync(string typeCode, string workingDayDate)
        {



            DateTime? parsedToDate = null;
            if (DateTime.TryParseExact(workingDayDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var resultt))
            {
                // Use .Date to ignore the time part
                parsedToDate = resultt.Date;
            }

            return await repository.All().AnyAsync(x =>

                x.WorkingDayCode != typeCode
                //x.WorkingDayCode != typeCode &&

                //x.WorkingDayDate.Date == parsedToDate
                );
        }


        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Working Day Declaration" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Working Day Declaration" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Working Day Declaration" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Working Day Declaration" && x.CheckDelete);
        }
        #endregion


    }
}
