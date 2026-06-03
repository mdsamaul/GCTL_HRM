using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.SalesDefTransportExpenseHead;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.SalesDefTransportExpenseHeadService
{
    public class SalesDefTransportExpenseHeads : AppService<SalesDefTransportExpenseHead>, ISalesDefTransportExpenseHead
    {
        private readonly IRepository<SalesDefTransportExpenseHead> TransportExpenseHeadRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public SalesDefTransportExpenseHeads(
            IRepository<SalesDefTransportExpenseHead> TransportExpenseHeadRepo,
            IRepository<CoreAccessCode> accessCodeRepository
            ) : base(TransportExpenseHeadRepo)
        {
            this.TransportExpenseHeadRepo = TransportExpenseHeadRepo;
            this.accessCodeRepository = accessCodeRepository;
        }

        private readonly string CreateSuccess = "Data saved successfully.";
        private readonly string CreateFailed = "Data insertion failed.";
        private readonly string UpdateSuccess = "Data updated successfully.";
        private readonly string UpdateFailed = "Data update failed.";
        private readonly string DeleteSuccess = "Data deleted successfully.";
        private readonly string DeleteFailed = "Data deletion failed.";
        private readonly string DataExists = "Data already exists.";

        #region Permission all type

        public async Task<bool> PagePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Def Transport Expense Head" && x.TitleCheck);

        }

        public async Task<bool> SavePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Def Transport Expense Head" && x.CheckAdd);

        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Def Transport Expense Head" && x.CheckEdit);

        }

        public async Task<bool> DeletePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Sales Def Transport Expense Head" && x.CheckDelete);

        }

        #endregion

        public async Task<List<SalesDefTransportExpenseHeadSetupViewModel>> GetAllAsync()
        {
            //await Task.Delay(100);
            try
            {
                return TransportExpenseHeadRepo.All().Select(c => new SalesDefTransportExpenseHeadSetupViewModel
                {
                    TC = c.Tc,
                    ExpenseHeadID = c.ExpenseHeadId,
                    ExpenseHead = c.ExpenseHead,
                    ShortName = c.ShortName
                }).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<SalesDefTransportExpenseHeadSetupViewModel> GetByIdAsync(string id)
        {


            try
            {
                var entity = TransportExpenseHeadRepo.All().Where(x => x.ExpenseHeadId == id).FirstOrDefault();
                if (entity == null) return null;

                return new SalesDefTransportExpenseHeadSetupViewModel
                {
                    TC = entity.Tc,
                    ExpenseHeadID = entity.ExpenseHeadId,
                    ExpenseHead = entity.ExpenseHead,
                    ShortName = entity.ShortName,
                    ShowCreateDate = entity.Ldate.HasValue ? entity.Ldate.Value.ToString("dd/MM/yyyy") : "",
                    ShowModifyDate = entity.ModifyDate.HasValue ? entity.ModifyDate.Value.ToString("dd/MM/yyyy") : "",
                };
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(SalesDefTransportExpenseHeadSetupViewModel model, string companyCode, string employeeId)
        {
            try
            {
                if (model.TC == 0)
                {
                    var entity = new SalesDefTransportExpenseHead
                    {
                        ExpenseHeadId = model.ExpenseHeadID,
                        ExpenseHead = model.ExpenseHead,
                        ShortName = model.ShortName,
                        CompanyCode = companyCode,
                        EntryUserEmployeeId = employeeId,
                        Luser = model.Luser,
                        Lip = model.Lip,
                        Ldate = model.Ldate,
                        Lmac = model.Lmac,
                    };

                    await TransportExpenseHeadRepo.AddAsync(entity);
                    return (true, CreateSuccess, entity);
                }

                var exData = await TransportExpenseHeadRepo.GetByIdAsync(model.TC);
                if (exData != null)
                {
                    // Update existing data
                    exData.ExpenseHeadId = model.ExpenseHeadID;
                    exData.ExpenseHead = model.ExpenseHead;
                    exData.ShortName = model.ShortName;
                    exData.CompanyCode = companyCode;
                    exData.EntryUserEmployeeId = employeeId;
                    exData.ModifyDate = DateTime.Now;
                    await TransportExpenseHeadRepo.UpdateAsync(exData);
                    return (true, UpdateSuccess, exData);
                }

                return (false, UpdateFailed, null);
            }
            catch (Exception)
            {
                return (false, CreateFailed, null);
            }
        }

        public async Task<(bool isSuccess, string message, object data)> DeleteAsync(List<int> ids)
        {
            foreach (var id in ids)
            {

                try
                {
                    var entity = await TransportExpenseHeadRepo.GetByIdAsync(id);
                    if (entity == null)
                    {
                        continue;
                    }

                    await TransportExpenseHeadRepo.DeleteAsync(entity);
                }
                catch (Exception)
                {
                    return (false, DeleteFailed, null);
                }
            }

            return (true, DeleteSuccess, null);
        }

        public async Task<string> AutoIdAsync()
        {
            var TransportExpenseList = (await TransportExpenseHeadRepo.GetAllAsync()).ToList();

            int newTransportExpenseId;

            if (TransportExpenseList != null && TransportExpenseList.Count > 0)
            {
                var lastTransportExpenseId = TransportExpenseList
                    .OrderByDescending(x => x.Tc)
                    .Select(x => x.ExpenseHeadId)
                    .FirstOrDefault();

                // Try parse in case TransportExpenseCode is string
                int.TryParse(lastTransportExpenseId, out int lastId);
                newTransportExpenseId = lastId + 1;
            }
            else
            {
                newTransportExpenseId = 1;
            }

            return newTransportExpenseId.ToString("D3");
        }

        public async Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string TransportExpenseValue)
        {
            bool Exists = TransportExpenseHeadRepo.All().Any(x => x.ExpenseHead == TransportExpenseValue);
            return (Exists, DataExists, null);
        }
    }
}
