using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRMATDAttendanceTypes;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.HRMATDAttendanceTypes
{
    public class HRMATDAttendanceTypeService : AppService<HrmAtdAttendanceType>, IHRMATDAttendanceTypeService
    {
        private readonly IRepository<HrmAtdAttendanceType> repository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IDeleteHistoryService deleteHistoryService;

        public HRMATDAttendanceTypeService(IRepository<HrmAtdAttendanceType> repository, IRepository<CoreAccessCode> accessCodeRepository, IDeleteHistoryService deleteHistoryService) : base(repository)
        {
            this.repository = repository;
            this.accessCodeRepository = accessCodeRepository;
            this.deleteHistoryService = deleteHistoryService;
        }


        public async Task<HRMATDAttendanceTypeSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await repository.GetByIdAsync(code);
            if (entity == null)
            {
                return null;
            }
            return new HRMATDAttendanceTypeSetupViewModel

            {
                AutoId = entity.AutoId, //id
                AttendanceTypeCode = entity.AttendanceTypeCode,
                AttendanceTypeName = entity.AttendanceTypeName,
                ShortName = entity.ShortName,
                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Luser = entity.Luser,
                Lip = entity.Lip,
                Lmac = entity.Lmac
            };
        }
        public async Task<List<HRMATDAttendanceTypeSetupViewModel>> GetAllAsync()
        {
            var entity = await repository.GetAllAsync();
            return entity.Select(entityVM => new HRMATDAttendanceTypeSetupViewModel
            {
                AutoId = entityVM.AutoId,
                AttendanceTypeCode = entityVM.AttendanceTypeCode,
                AttendanceTypeName = entityVM.AttendanceTypeName,
                ShortName = entityVM.ShortName,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac

            }).ToList();
        }


        public async Task<bool> SaveAsync(HRMATDAttendanceTypeSetupViewModel entityVM)
        {
            await repository.BeginTransactionAsync();
            try
            {

                HrmAtdAttendanceType entity = new HrmAtdAttendanceType();
                entity.AttendanceTypeCode = await GenerateNextCode();
                entity.AttendanceTypeName = entityVM.AttendanceTypeName;
                entity.ShortName = entityVM.ShortName ?? string.Empty;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
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

        public async Task<bool> UpdateAsync(HRMATDAttendanceTypeSetupViewModel entityVM)
        {
            await repository.BeginTransactionAsync();
            try
            {

                var entity = await repository.GetByIdAsync(entityVM.AttendanceTypeCode);
                if (entity == null)
                {
                    await repository.RollbackTransactionAsync();
                    return false;
                }
                entity.AttendanceTypeCode = entityVM.AttendanceTypeCode;
                entity.AttendanceTypeName = entityVM.AttendanceTypeName;
                entity.ShortName = entityVM.ShortName ?? string.Empty;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
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
        public async Task<(bool success, string message, bool refError)> DeleteLeaveType(List<string> ids, DeleteHistoryViewModel model)
        {
            string column = "AttendanceTypeCode";
            string table = repository.GetTableName();

            if (ids == null && !ids.Any())
                return (false, "Error deleting leave type", false);

            var entities = await repository.All().Where(x => ids.Contains(x.AttendanceTypeCode)).AsNoTracking().ToListAsync();

            if (!entities.Any())
                return (false, "No records found to delete", false);


            var dependencyCheck = await deleteHistoryService.CheckDependenciesAsync(
                table,
                column,
                entities.Select(x => x.AttendanceTypeCode).Cast<string>().ToList()
            );

            if (dependencyCheck != null && !dependencyCheck.CanDelete)
            {
                return (false, dependencyCheck.Message, true);
            }

            await repository.BeginTransactionAsync();


            try
            {
                await repository.DeleteRangeAsync(entities);
                model.tableName = table;
                await deleteHistoryService.LogDeletedRecordsAsync(entities, model);

                await repository.CommitTransactionAsync();
                return (true, "Deleted Successfully", false);
            }
            catch (Exception ex)
            {
                await repository.RollbackTransactionAsync();
                return (false, "Delete failed", false);
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

        public HrmAtdAttendanceType GetLeaveType(string code)
        {
            return repository.GetById(code);
        }



        public async Task<IEnumerable<CommonSelectModel>> SelectionHrmAttendanceTypeAsync()
        {
            return await repository.All().OrderByDescending(x => x.AttendanceTypeCode)
                      .Select(x => new CommonSelectModel
                      {
                          Code = x.AttendanceTypeCode,
                          Name = x.AttendanceTypeName
                      })
                      .ToListAsync();
        }





        #region NextCode
        public async Task<string> GenerateNextCode()
        {

            var Code = await repository.GetAllAsync();
            var lastCode = Code.Max(b => b.AttendanceTypeCode);
            int nextCode = 1;
            if (!string.IsNullOrEmpty(lastCode))
            {
                int lastNumber = int.Parse(lastCode.TrimStart('0'));
                lastNumber++;
                nextCode = lastNumber;
            }
            return nextCode.ToString("D1");

        }
        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await repository.All().AnyAsync(x => x.AttendanceTypeCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await repository.All().AnyAsync(x => x.AttendanceTypeName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await repository.All().AnyAsync(x => x.AttendanceTypeName == name && x.AttendanceTypeCode != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Attendance Type" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Attendance Type" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Attendance Type" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Attendance Type" && x.CheckDelete);
        }
        #endregion

    }
}
