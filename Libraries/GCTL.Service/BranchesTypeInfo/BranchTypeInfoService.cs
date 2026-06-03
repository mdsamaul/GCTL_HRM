using GCTL.Core.Data;
using GCTL.Core.ViewModels.BranchesTypeInfo;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Dynamic.Core;
using static Dapper.SqlMapper;

namespace GCTL.Service.BranchesTypeInfo
{
    public class BranchTypeInfoService : AppService<CoreBranch>, IBranchTypeInfoService
    {
        private readonly IRepository<CoreBranch> _coreBranchrepository;
        private readonly IRepository<CoreAccessCode> _accessCodeRepository;
        private readonly IRepository<CoreCompany> _companyRepository;
        private readonly IDeleteHistoryService deleteHistoryService;

        private const string TableName = "Core_Branch";
        private const string ColumnName = "BranchCode";

        public BranchTypeInfoService(
            IRepository<CoreBranch> coreBranchrepository, 
            IRepository<CoreAccessCode> accessCodeRepository,
            IRepository<CoreCompany> companyRepository, 
            IDeleteHistoryService deleteHistoryService
            
            ) 
    : base(coreBranchrepository)
        {
            _coreBranchrepository = coreBranchrepository;
            _accessCodeRepository = accessCodeRepository;
            _companyRepository = companyRepository;
            this.deleteHistoryService = deleteHistoryService;
        }

        public List<CoreBranch> GetBranches()
        {
            return GetAll();
        }

        public async Task <List<BranchTypeSetupViewModel>> GetCompaniess(string CompanyCode)
        {
            var data = await(from emp in _coreBranchrepository.All().AsNoTracking()
                        where emp.CompanyCode == CompanyCode
                        join empComp in _companyRepository.All().AsNoTracking()
                        on emp.CompanyCode equals empComp.CompanyCode into empComJoin
                        from empComp in empComJoin.DefaultIfEmpty()
                        select new BranchTypeSetupViewModel
                        {
                            BranchCode = emp.BranchCode,
                            BranchName = emp.BranchName,
                            CompanyCode = emp.CompanyCode,
                            CompanyName = empComp.CompanyName,
                            Address = emp.Address,
                            Phone = emp.Phone,
                            Email = emp.Email,
                            AddressBangla = emp.AddressBangla,
                            BanglaBranch = emp.BanglaBranch,
                            Fax = emp.Fax

                        }).ToListAsync();
     
            Debug.WriteLine($"Records retrieved for CompanyCode {CompanyCode}: {data.Count}");

            return data;
        }

        public CoreBranch GetBranch(string id)
        {
            return _coreBranchrepository.GetById(id);
        }

        public BranchTypeSetupViewModel GetBranchTypeSetupView(string code)
        {
            var query = (from branch in _coreBranchrepository.All()
                         join company in _companyRepository.All()
                         on branch.CompanyCode equals company.CompanyCode into companyComJoin
                         from company in companyComJoin.DefaultIfEmpty()
                         where branch.BranchCode == code
                         select new BranchTypeSetupViewModel
                         {
                             BranchName = branch.BranchName,
                             BranchCode = branch.BranchCode,
                             CompanyCode = branch.CompanyCode,
                             Address = branch.Address,
                             AddressBangla = branch.AddressBangla,
                             BanglaBranch = branch.BanglaBranch,
                             Phone = branch.Phone,
                             Ldate = branch.Ldate,
                             ModifyDate = branch.ModifyDate,
                             Email = branch.Email,
                             Fax = branch.Fax,
                             Company = company.CompanyName

                         }).FirstOrDefault(); 

            return query;
        }

        public async Task<(bool success, bool refSuccess, string message)> DeleteBranchTypeInfo(List<string> ids, DeleteHistoryViewModel model)
        {
            try
            {
                // Normalize to list of IDs (comma-separated support)

                if (ids.Count == 0)
                {
                    return (false, false, "No valid department codes found.");
                }

                // Dependency check
                var dependencyCheck = await deleteHistoryService.CheckDependenciesAsync(
                    _coreBranchrepository.GetTableName(),
                    ColumnName,
                    ids
                );

                if (!dependencyCheck.CanDelete)
                {
                    return (false, true, dependencyCheck.Message);
                }

                await _coreBranchrepository.BeginTransactionAsync();

                // Fetch entities
                var entities = await _coreBranchrepository.All()
                    .Where(x => ids.Contains(x.BranchCode))
                    .ToListAsync();

                if (entities == null || entities.Count == 0)
                {
                    await _coreBranchrepository.RollbackTransactionAsync();
                    return (false, false, "No matching departments found to delete.");
                }

                // Perform delete
                _coreBranchrepository.Delete(entities);
                model.tableName = TableName;
                // Log deleted records
                await deleteHistoryService.LogDeletedRecordsAsync(
                    entities, model
                );

                await _coreBranchrepository.CommitTransactionAsync();
                return (true, false, "Deleted successfully.");
            }
            catch (Exception ex)
            {
                await _coreBranchrepository.RollbackTransactionAsync();
                Console.WriteLine(ex);
                return (false, false, $"Delete failed: {ex.Message}");
            }
        }

        public CoreBranch SaveBranchTypeInfo(CoreBranch entity)
        {
            if (IsBranchTypeInfoExistByCode(entity.BranchCode))
                Update(entity);
            else
                Add(entity);
            return entity;
        }

        public async Task< IEnumerable<CommonSelectModel>> GetCompanieBranchSelections()
        {
            return await _coreBranchrepository.All()
                 .Select(x => new CommonSelectModel
                 {
                     Code = x.BranchCode,
                     Name = x.BranchName,
                 }).ToListAsync();
        }

        public IEnumerable<CommonSelectModel> DropSelection()
        {
            return _companyRepository.All().Select(x => new CommonSelectModel
            {
                Code = x.CompanyCode,
                Name = x.CompanyName
            });
        }

        public IEnumerable<CommonSelectModel> GetCompaniesSelections()
        {
            return (from company in _companyRepository.All()
                    select new CommonSelectModel
                    {
                        Code = company.CompanyCode,
                        Name = company.CompanyName
                    }).Distinct().ToList();
        }

        public bool IsBranchTypeInfoExistByCode(string code)
        {
            return _coreBranchrepository.All().Any(x => x.BranchCode == code);
        }

        public bool IsBranchTypeInfoExist(string name)
        {
           return _coreBranchrepository.All().Any(x => x.BranchName == name);
        }

        public bool IsBranchTypeInfoExist(string name, string CompanyCode, string BranchCode)
        {
            return _coreBranchrepository.All().Any(x => x.BranchName.ToLower().Trim() == name.ToLower().Trim() && x.CompanyCode == CompanyCode && x.BranchCode != BranchCode);
        }


        public bool PagePermission(string accessCode)
        {
            return _accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Branch" && x.TitleCheck);
        }

        public bool SavePermission(string accessCode)
        {
            return _accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Branch" && x.CheckAdd);
        }

        public bool UpdatePermission(string accessCode)
        {
            return _accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Branch" && x.CheckEdit);
        }

        public bool DeletePermission(string accessCode)
        {
            return _accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Branch" && x.CheckDelete);
        }
    }
}
