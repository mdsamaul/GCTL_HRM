using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GCTL.Service.Designations
{
    public class DesignationService : AppService<HrmDefDesignation>, IDesignationService
    {
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IDeleteHistoryService deleteHistoryService;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        private const string TableName = "HRM_Def_Designation";
        private const string ColumnName = "DesignationCode";

        public DesignationService(IRepository<HrmDefDesignation> designationRepository,
            IDeleteHistoryService deleteHistoryService,
            IRepository<CoreAccessCode> accessCodeRepository
            
            )
    : base(designationRepository)
        {
            this.designationRepository = designationRepository;
            this.deleteHistoryService = deleteHistoryService;
            this.accessCodeRepository = accessCodeRepository;
        }

        public List<HrmDefDesignation> GetDesignations()
        {
            return GetAll();
        }

        public HrmDefDesignation GetDesignation(string id)
        {
            return designationRepository.GetById(id);
        }

        public HrmDefDesignation SaveDesignation(HrmDefDesignation entity)
        {
            entity.BanglaDesignation = string.Empty;

            entity.BanglaShortName = string.Empty;

            entity.CompanyCode = string.Empty;
            entity.EmployeeId = string.Empty;
            entity.MobileAllowanceId = string.Empty;
            //if (IsDesignationExistByCode(entity.DesignationCode))
            //    Update(entity);


            //    Add(entity);



            //return entity;
            if (IsDesignationExistByCode(entity.DesignationCode))
            {
                Update(entity); // only update if it exists
            }
            else
            {
                Add(entity); // only add if it's new
            }

            return entity;
        }

        public async Task<(bool success, bool refSuccess, string message)> DeleteDesignationAsync(List<string> ids, DeleteHistoryViewModel model)

        {
            try
            {
                // Normalize to list of IDs (comma-separated support)

                if (ids.Count == 0)

                {
                    return (false, false, "No valid department codes found.");
                }

                // Dependency check
                var alternateColumn = new List<string> { "DesignationId", "Designation" };
                var dependencyCheck = await deleteHistoryService.CheckDependenciesAsync(

                    designationRepository.GetTableName(),
                    ColumnName,
                    ids,
                    alternateColumn
                );

                if (!dependencyCheck.CanDelete)

                {
                    return (false, true, dependencyCheck.Message);
                }

                await designationRepository.BeginTransactionAsync();

                // Fetch entities
                var entities = await designationRepository.All()
                    .Where(x => ids.Contains(x.DesignationCode))
                    .ToListAsync();

                if (entities == null || entities.Count == 0)
                {
                    await designationRepository.RollbackTransactionAsync();

                    return (false, false, "No matching departments found to delete.");
                }

                // Perform delete

                designationRepository.Delete(entities);

                // Log deleted records
                model.tableName = TableName;
                await deleteHistoryService.LogDeletedRecordsAsync(

                    entities, model
                );

                await designationRepository.CommitTransactionAsync();

                return (true, false, "Deleted successfully.");
            }
            catch (Exception ex)
            {
                await designationRepository.RollbackTransactionAsync();
                Console.WriteLine(ex);
                return (false, false, $"Delete failed: {ex.Message}");
            }

        }

        public bool IsDesignationExistByCode(string code)
        {
            return designationRepository.All().Any(x => x.DesignationCode == code);
        }

        public bool IsDesignationExist(string name)
        {
            return designationRepository.All().Any(x => x.DesignationName == name);
        }

        public bool IsDesignationExist(string name, string typeCode)
        {
            return designationRepository.All().Any(x => x.DesignationName == name && x.DesignationCode != typeCode);
        }

        public IEnumerable<CommonSelectModel> DesignationSelection()
        {
            return designationRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.DesignationCode,
                    Name = x.DesignationName
                });
        }
        public bool SavePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Designation" && x.CheckAdd);
        }
        public bool UpdatePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Designation" && x.CheckEdit);
        }
        public bool DeletePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Designation" && x.CheckDelete);
        }
    }
}
