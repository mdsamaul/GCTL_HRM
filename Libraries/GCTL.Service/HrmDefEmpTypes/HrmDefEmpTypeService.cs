using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HrmDefEmpTypes;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefEmpTypes
{
    public class HrmDefEmpTypeService : AppService<HrmDefEmpType>, IHrmDefEmpTypeService
    {
        private readonly IRepository<HrmDefEmpType> repository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IDeleteHistoryService deleteHistoryService;

        private const string TableName = "HRM_Def_EmpType";
        private const string ColumnName = "EmpTypeCode";

        public HrmDefEmpTypeService(IRepository<HrmDefEmpType> repository, IRepository<CoreAccessCode> accessCodeRepository, IDeleteHistoryService deleteHistoryService) :base(repository)
        {
            this.repository = repository;
            this.accessCodeRepository = accessCodeRepository;
            this.deleteHistoryService = deleteHistoryService;
        }


        public async Task<HrmDefEmpTypeSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await repository.GetByIdAsync(code);
            if (entity == null)
            {
                return null;
            }
            return new HrmDefEmpTypeSetupViewModel
            {
                AutoId = entity.AutoId, //id
                EmpTypeCode = entity.EmpTypeCode,
                EmpTypeName = entity.EmpTypeName,
                EmpTypeShortName = entity.EmpTypeShortName,
                BanglaEmployeeType=entity.BanglaEmployeeType,
                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Luser = entity.Luser,
                Lip = entity.Lip,
                Lmac = entity.Lmac
            };
        }


        public async Task<List<HrmDefEmpTypeSetupViewModel>> GetAllAsync()
        {
            var entity = await repository.GetAllAsync();
            return entity.Select(entityVM => new HrmDefEmpTypeSetupViewModel
            {
                AutoId = entityVM.AutoId,
                EmpTypeCode = entityVM.EmpTypeCode,
                EmpTypeName = entityVM.EmpTypeName,
                EmpTypeShortName = entityVM.EmpTypeShortName,
                BanglaEmployeeType=entityVM.BanglaEmployeeType,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac
            }).ToList();
        }


        public async Task<bool> SaveAsync(HrmDefEmpTypeSetupViewModel entityVM, string CompanyCode)
        {
            await repository.BeginTransactionAsync();
            try
            {
                HrmDefEmpType entity = new HrmDefEmpType();
                entity.EmpTypeCode = await GenerateNextCode();
                entity.EmpTypeName = entityVM.EmpTypeName;
                entity.EmpTypeShortName = entityVM.EmpTypeShortName ?? string.Empty;
                entity.BanglaEmployeeType = entityVM.BanglaEmployeeType ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                entity.CompanyCode = CompanyCode;
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


        public async Task<bool> UpdateAsync(HrmDefEmpTypeSetupViewModel entityVM)
        {
            await repository.BeginTransactionAsync();
            try
            {
                var entity = await repository.GetByIdAsync(entityVM.EmpTypeCode);
                if (entity == null)
                {
                    await repository.RollbackTransactionAsync();
                    return false;
                }
                entity.EmpTypeCode = entityVM.EmpTypeCode;
                entity.EmpTypeName = entityVM.EmpTypeName;
                entity.EmpTypeShortName = entityVM.EmpTypeShortName ?? string.Empty;
                entity.BanglaEmployeeType = entityVM.BanglaEmployeeType ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
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

        public async Task<(bool succses, string messege)> DeleteTab(List<string> ids, DeleteHistoryViewModel model)
        {
            if (!ids.Any())
                return (false, "No Data found to delete");

            var alternateColumn = new List<string> { "JobNatureID" };
            var dependencyCheck = await deleteHistoryService.CheckDependenciesAsync(
                repository.GetTableName(),
                ColumnName,
                ids,
                alternateColumn
            );

            if (!dependencyCheck.CanDelete)
                return (false, dependencyCheck.Message);

            await repository.BeginTransactionAsync();

            try
            {
                var entity = await repository.All().Where(x => ids.Contains(x.EmpTypeCode)).ToListAsync();


                repository.Delete(entity);
                model.tableName = TableName;
                await deleteHistoryService.LogDeletedRecordsAsync(
                    entity, model
                );

                await repository.CommitTransactionAsync();
                return (true, "Delete Successfully");
            }
            catch (Exception ex)
            {
                await repository.RollbackTransactionAsync();
                Console.WriteLine(ex.ToString());
                return (false, "Delete Failed");

            }

        }

        #region DropDown
        public async Task<IEnumerable<CommonSelectModel>> SelectionHrEmployeeTypeAsync()
        {
            return await repository.All().OrderByDescending(x => x.EmpTypeCode)
                      .Select(x => new CommonSelectModel
                      {
                          Code = x.EmpTypeCode,
                          Name = x.EmpTypeName
                      }).ToListAsync();
        }
        #endregion

        #region NextCode
        public async Task<string> GenerateNextCode()
        {
            var Code = await repository.GetAllAsync();
            var lastCode = Code.Max(b => b.EmpTypeCode);
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
            return await repository.All().AnyAsync(x => x.EmpTypeCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await repository.All().AnyAsync(x => x.EmpTypeName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await repository.All().AnyAsync(x => x.EmpTypeName == name && x.EmpTypeCode!= typeCode);
        }
        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Type " && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Type " && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Type " && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Type " && x.CheckDelete);
        }
        #endregion
    }
}
