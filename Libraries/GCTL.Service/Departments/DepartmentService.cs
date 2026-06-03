using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.Departments
{
    public class DepartmentService : AppService<HrmDefDepartment>, IDepartmentService
    {
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IDeleteHistoryService deleteHistoryService;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        private const string TableName = "HRM_Def_Department";
        private const string ColumnName = "DepartmentCode";

        public DepartmentService(IRepository<HrmDefDepartment> departmentRepository,
            IDeleteHistoryService deleteHistoryService,
            IRepository<CoreAccessCode> accessCodeRepository
            
        )
    : base(departmentRepository)
        {
            this.departmentRepository = departmentRepository;
            this.deleteHistoryService = deleteHistoryService;
            this.accessCodeRepository = accessCodeRepository;
        }

        public List<HrmDefDepartment> GetDepartments()
        {
            return GetAll();
        }

        public HrmDefDepartment GetDepartment(string id)
        {
            return departmentRepository.GetById(id);
        }

        public HrmDefDepartment SaveDepartment(HrmDefDepartment entity)
        {
            if (IsDepartmentExistByCode(entity.DepartmentCode))

                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public async Task<(bool success, bool refSuccess, string message)> DeleteDepartment(List<string> ids, DeleteHistoryViewModel model)
        {

            try
            {
                // Normalize to list of IDs (comma-separated support)


                if (ids.Count == 0)
                {
                    return (false, false, "No valid department codes found.");
                }

                // Dependency check
                var alternateColumn = new List<string> { "DepartmentID" };
                var dependencyCheck = await deleteHistoryService.CheckDependenciesAsync(
                    departmentRepository.GetTableName(),
                    ColumnName,
                    ids,
                    alternateColumn
                );

                if (!dependencyCheck.CanDelete)
                {
                    return (false, true, dependencyCheck.Message);
                }

                await departmentRepository.BeginTransactionAsync();

                // Fetch entities
                var entities = await departmentRepository.All()
                    .Where(x => ids.Contains(x.DepartmentCode))
                    .ToListAsync();

                if (entities == null || entities.Count == 0)
                {
                    await departmentRepository.RollbackTransactionAsync();
                    return (false, false, "No matching departments found to delete.");
                }

                // Perform delete
                departmentRepository.Delete(entities);
                model.tableName = TableName;
                // Log deleted records
                await deleteHistoryService.LogDeletedRecordsAsync(
                    entities, model
                );

                await departmentRepository.CommitTransactionAsync();
                return (true, false, "Deleted successfully.");
            }
            catch (Exception ex)
            {
                await departmentRepository.RollbackTransactionAsync();
                Console.WriteLine(ex);
                return (false, false, $"Delete failed: {ex.Message}");
            }
        }

        public bool IsDepartmentExistByCode(string code)
        {
            return departmentRepository.All().Any(x => x.DepartmentCode == code);
        }

        public bool IsDepartmentExist(string name)
        {
            return departmentRepository.All().Any(x => x.DepartmentName == name);
        }

        public bool IsDepartmentExist(string name, string typeCode)
        {
            return departmentRepository.All().Any(x => x.DepartmentName == name && x.DepartmentCode != typeCode);
        }

        public IEnumerable<CommonSelectModel> DepartmentSelection()
        {
            return departmentRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.DepartmentCode,
                    Name = x.DepartmentName
                });
        }
        public bool SavePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Department" && x.CheckAdd);
        }
        public bool UpdatePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Department" && x.CheckEdit);
        }
        public bool DeletePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Department" && x.CheckDelete);
        }
    }
}
