using GCTL.Core.Data;
using GCTL.Core.ViewModels.EditUserVM;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GCTL.Service.UserEditEntry
{
    public class UserEditService : AppService<CoreUserInfo>, IUserEditService
    {
        private readonly ICommonService commonService;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<CoreUserInfo> entryRepository;

        public UserEditService(ICommonService commonService, IRepository<CoreAccessCode> accessCodeRepository, IRepository<CoreUserInfo> entryRepository) : base(entryRepository)
        {
            this.commonService = commonService;
            this.accessCodeRepository = accessCodeRepository;
            this.entryRepository = entryRepository;
        }

        public async Task<EditUserSetupViewModel> GetByIdAsync(int id)
        {
            try
            {
                var data = await entryRepository.GetByIdAsync(id);

                if (data == null) return null;

                var record = new EditUserSetupViewModel
                {
                    UserId = id,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    EmployeeId = data.EmployeeId,
                    Dob = data.Dob,
                    OffPhone = data.OffPhone,
                    PerPhone = data.PerPhone,
                    OffEmail = data.OffEmail,
                    PerEmail = data.PerEmail,
                    WorkStation = data.WorkStation,
                    Regulation = data.Regulation,
                    Ldate = data.Ldate,
                    ModifyDate = data.ModifyDate
                };
                return record;
            }
            catch (Exception ex)
            {

                throw;
            }


            throw new NotImplementedException();
        }

        public async Task<EditUserSetupViewModel> GetByIdAsync(string userName)
        {
            try
            {
                var data = await entryRepository.All().Where(x=>x.Username == userName).FirstOrDefaultAsync();
                    //await entryRepository.GetByIdAsync(id);

                if (data == null) return null;

                var record = new EditUserSetupViewModel
                {
                    UserId = data.Id,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    EmployeeId = data.EmployeeId,
                    Dob = (data.Dob.HasValue && data.Dob.Value != new DateTime(1900, 1, 1))
                        ? data.Dob : null,
                    OffPhone = data.OffPhone,
                    PerPhone = data.PerPhone,
                    OffEmail = data.OffEmail,
                    PerEmail = data.PerEmail,
                    WorkStation = data.WorkStation,
                    Regulation = data.Regulation,
                    Ldate = data.Ldate,
                    Username = data.Username,
                    ModifyDate = data.ModifyDate
                };
                return record;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<(List<EditUserGridViewModel> Data, int totalRecord, int curentRecord)> GetPaginatedDataAsync(string searchValue, int page, int pageSize, string sortColumn, string sortDirection, string id)
        {
            var query = await (from e in entryRepository.All().AsNoTracking()
                               where e.Username == id
                               select new EditUserGridViewModel
                               {
                                   Id = e.Id,
                                   Username = e.Username,
                                   FullName = e.FirstName + " " + e.LastName,
                                   UserType = e.Type,
                                   EntryDate = e.EntryDate.HasValue
                                        ? e.EntryDate.Value.ToString("dd/MM/yyyy")
                                        : string.Empty
                               }).ToListAsync();

            var totalRecord = query.Count;

            IEnumerable<EditUserGridViewModel> filterQuery = query;

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                filterQuery = filterQuery.Where(x =>
                    (x.Username?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.FullName?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.UserType?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.EntryDate?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.Id.ToString()?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) 
                );
            }

            var currentRecord = filterQuery.Count();
            if (!string.IsNullOrWhiteSpace(sortColumn) && !string.IsNullOrWhiteSpace(sortDirection))
            {
                filterQuery = sortColumn.ToLower() switch
                {
                    "username" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.Username) : filterQuery.OrderByDescending(x => x.Username),
                    "fullname" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.FullName) : filterQuery.OrderByDescending(x => x.FullName),
                    "usertype" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.UserType) : filterQuery.OrderByDescending(x => x.UserType),
                    "entrydate" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.EntryDate) : filterQuery.OrderByDescending(x => x.EntryDate),
                    "id" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.Id) : filterQuery.OrderByDescending(x => x.Id),

                    _ => filterQuery.OrderBy(a => a.Id)
                };
            }
            else
            {
                filterQuery = filterQuery.OrderBy(a => a.Id);
            }

            var data = pageSize < 0 ? filterQuery.ToList() : filterQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return (data, totalRecord, currentRecord);
        }

        public async Task<(bool isSuccesss, string message, object data)> SaveAsync(EditUserSetupViewModel model)
        {
            if(model == null || string.IsNullOrWhiteSpace(model.Username))
            {
                return (false, "Internal error!", null);
            }

            await entryRepository.BeginTransactionAsync();

            try
            {
                if (model.UserId == 0)
                    return (false, "Internal Error!", null);

                var exData = await entryRepository.GetByIdAsync(model.UserId);

                if (exData == null)
                    return (false, "Data does not exists!", null);

                //exData.Id = model.Id;
                exData.ModifyDate = model.ModifyDate;
                exData.Luser = model.Luser;
                exData.Lip = model.Lip;
                exData.Lmac = model.Lmac;
                exData.Password = model.Password;
                
                exData.SecureCode = model.SecureCode ?? string.Empty;
                

                await entryRepository.UpdateAsync(exData);
                await entryRepository.CommitTransactionAsync();
                return (true, "Update Successfully", exData);
            }
            catch (Exception ex)
            {
                await entryRepository?.RollbackTransactionAsync();

                return (false, "Internal Server Error!", null);
            }
        }

        public Task<bool> SavePermissionAsync(string accessCode)
        {
            throw new NotImplementedException();
        }
        public Task<bool> PagePermissionAsync(string accessCode)
        {
            throw new NotImplementedException();
        }

    }
}
