using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.CompanyInfo;
using GCTL.Core.ViewModels.ContactPersonInfo;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.ContactPersonInfo
{
    public class ContactPersonInfoService : AppService<SalesContactPerson>, IContactPersonInfoService
    {
        #region Service & Repository
        private readonly IRepository<SalesContactPerson> contactPersonrepository;
        private readonly IRepository<HrmDefDesignation> hrmDefDesignationrepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;

        private const string TableName = "Sales_ContactPerson";
        private const string ColumnName = "CPID";

        public ContactPersonInfoService(
            IRepository<SalesContactPerson> contactPersonrepository,
            IRepository<HrmDefDesignation> hrmDefDesignationrepository,
            IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService

            ) 
            
    : base(contactPersonrepository)
        {
            this.contactPersonrepository = contactPersonrepository;
            this.hrmDefDesignationrepository = hrmDefDesignationrepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        #endregion

        #region GetAllAsync

        public async Task<List<ContactPersonInfoSetupViewModel>> GetAllAsync()
        {
            var query = await (from per in contactPersonrepository.All()
                               join hrm in hrmDefDesignationrepository.All()
                               on per.DesignationCode equals hrm.DesignationCode into JobGroup
                               from hrm in JobGroup.DefaultIfEmpty()
                               select new ContactPersonInfoSetupViewModel
                               {
                                   AutoId = per.AutoId,
                                   Cpid = per.Cpid,
                                   ContactPersonName = per.ContactPersonName,
                                   DesignationName = hrm.DesignationName,
                                   ContactPersonMobile = per.ContactPersonMobile,
                                   ContactPersonEmail = per.ContactPersonEmail,
                                   Ldate = per.Ldate,
                                   ModifyDate = per.ModifyDate,
                                   Luser = per.Luser,
                                   Lip = per.Lip,
                                   Lmac = per.Lmac
                               }).ToListAsync();

            return query;
        }


        #endregion

        #region GetByIdAsync

        public async Task<ContactPersonInfoSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await contactPersonrepository.GetByIdAsync(code);
            if (entity == null) return null;

            ContactPersonInfoSetupViewModel entityVM = new ContactPersonInfoSetupViewModel
            { 
                
              AutoId= entity.AutoId,
              Cpid= entity.Cpid,
              ContactPersonName = entity.ContactPersonName,
              DesignationCode = entity.DesignationCode,
              ContactPersonMobile = entity.ContactPersonMobile,
              ContactPersonEmail = entity.ContactPersonEmail,
              Ldate= entity.Ldate,
              ModifyDate= entity.ModifyDate,   
              Luser = entity.Luser,
              Lip = entity.Lip,
              Lmac = entity.Lmac,
            };

            return entityVM;
        }

        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(ContactPersonInfoSetupViewModel entityVM)
        {
            var nextCode= commonService.GenerateNextCode("CPID", "Sales_ContactPerson", 3, "CP");
            await contactPersonrepository.BeginTransactionAsync();
            try
            {
                SalesContactPerson entity = new SalesContactPerson();
                entity.Cpid = nextCode;
                entity.ContactPersonName = entityVM.ContactPersonName;
                entity.DesignationCode = entityVM.DesignationCode ?? string.Empty;
                entity.ContactPersonMobile = entityVM.ContactPersonMobile ?? string.Empty;
                entity.ContactPersonEmail = entityVM.ContactPersonEmail ?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await contactPersonrepository.AddAsync(entity);
                await contactPersonrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await contactPersonrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region UpdateAsync

        public async Task<bool> UpdateAsync(ContactPersonInfoSetupViewModel entityVM)
        {
            await contactPersonrepository.BeginTransactionAsync();
            try
            {
                var entity = await contactPersonrepository.GetByIdAsync(entityVM.Cpid);
                if (entity == null)
                {
                    await contactPersonrepository.RollbackTransactionAsync();
                    return false;
                }
                entity.Cpid = entityVM.Cpid;
                entity.ContactPersonName = entityVM.ContactPersonName ?? string.Empty;
                entity.DesignationCode = entityVM.DesignationCode ?? string.Empty;
                entity.ContactPersonMobile = entityVM.ContactPersonMobile ?? string.Empty;
                entity.ContactPersonEmail = entityVM.ContactPersonEmail ?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await contactPersonrepository.UpdateAsync(entity);
                await contactPersonrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await contactPersonrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region SelectionAsync

        public async Task<IEnumerable<CommonSelectModel>> SelectionContactPersonAsync()
        {

            var data = await contactPersonrepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.Cpid,
                           Name = x.ContactPersonName,
                       }).ToListAsync();
            return data;
        }

        #endregion

        #region DeleteTab

        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await contactPersonrepository.All().Where(x => ids.Contains(x.Cpid)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            contactPersonrepository.Delete(entity);

            return true;
        }
        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await contactPersonrepository.All().AnyAsync(x => x.Cpid == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await contactPersonrepository.All().AnyAsync(x => x.ContactPersonName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await contactPersonrepository.All().AnyAsync(x => x.ContactPersonName == name && x.Cpid != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Contact Person Info" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Contact Person Info" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Contact Person Info" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Contact Person Info" && x.CheckDelete);
        }
        #endregion

        #region Next Id Substring

        public Task<string> Autoid()
        {
            var appList = contactPersonrepository.All().OrderByDescending(x => x.AutoId).FirstOrDefault();

            int newId = 1;
            if (appList != null)
            {
                string lastId = appList.Cpid;
                string numericPart = lastId.Substring(5);
                if (int.TryParse(numericPart, out int parsedId))
                {
                    newId = parsedId + 1;
                }
            }

            string formattedId = $"CP{newId:D5}";

            return Task.FromResult(formattedId);
        }

        #endregion

    }
}
