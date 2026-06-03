using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.PaymentTerms;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.PaymentTerms
{
    public class PaymentTermsService : AppService<SalesDefPaymentTerms>, IPaymentTermsService
    {
        #region Service & Repository
        private readonly IRepository<SalesDefPaymentTerms> paymentTermsrepository;
        private readonly IRepository<SalesDefPaymentType> paymentTyperepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;
        private const string TableName = "Sales_Def_PaymentTerms";
        private const string ColumnName = "PaymentTermsId";
        public PaymentTermsService(
            IRepository<SalesDefPaymentTerms> paymentTermsrepository,
            IRepository<SalesDefPaymentType> paymentTyperepository,
            IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService

            )

    : base(paymentTermsrepository)
        {
            this.paymentTermsrepository = paymentTermsrepository;
            this.paymentTyperepository = paymentTyperepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        #endregion

        #region GetAllAsync

        public async Task<List<PaymentTermsSetupViewModel>> GetAllAsync()
        {
            var query = await (from per in paymentTermsrepository.All()
                               join hrm in paymentTyperepository.All()
                                   on per.Type equals hrm.PaymentTypeId into JobGroup
                               from hrm in JobGroup.DefaultIfEmpty()
                               select new PaymentTermsSetupViewModel
                               {
                                   Tc = per.AutoId,
                                   PaymentTermsId = per.PaymentTermsId,
                                   PaymentTermsName = per.PaymentTermsName,
                                   PaymentType = hrm.PaymentType,
                                   CreditDays = per.CreditDays,
                                   Percentise = per.Percentise,
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

        public async Task<PaymentTermsSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await paymentTermsrepository.GetByIdAsync(Convert.ToInt32(code));
            if (entity == null) return null;

            PaymentTermsSetupViewModel entityVM = new PaymentTermsSetupViewModel();
            entityVM.Tc = entity.AutoId;
            entityVM.PaymentTermsId = entity.PaymentTermsId;
            entityVM.Type = entity.Type;
            entityVM.Percentise = entity.Percentise;
            entityVM.CreditDays = entity.CreditDays;
            entityVM.PaymentTermsName = entity.PaymentTermsName;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(PaymentTermsSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            await paymentTermsrepository.BeginTransactionAsync();
            try
            {
                SalesDefPaymentTerms entity = new SalesDefPaymentTerms();
                entity.PaymentTermsId = strMaxNO;
                entity.Type = entityVM.Type;
                entity.Percentise = entityVM.Percentise;
                entity.CreditDays = entityVM.CreditDays;
                entity.PaymentTermsName = entityVM.PaymentTermsName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await paymentTermsrepository.AddAsync(entity);
                await paymentTermsrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await paymentTermsrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(PaymentTermsSetupViewModel entityVM)
        {
            await paymentTermsrepository.BeginTransactionAsync();
            try
            {
                var entity = await paymentTermsrepository.GetByIdAsync(entityVM.Tc);
                if (entity == null)
                {
                    await paymentTermsrepository.RollbackTransactionAsync();
                    return false;
                }
                entity.PaymentTermsId = entityVM.PaymentTermsId;
                entity.Type = entityVM.Type ?? string.Empty;
                entity.Percentise = entityVM.Percentise ?? string.Empty;
                entity.CreditDays = entityVM.CreditDays;
                entity.PaymentTermsName = entityVM.PaymentTermsName ?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await paymentTermsrepository.UpdateAsync(entity);
                await paymentTermsrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await paymentTermsrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region SelectionAsync
        public async Task<IEnumerable<CommonSelectModel>> SelectionPaymentTermsAsync()
        {

            var data = await paymentTermsrepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.PaymentTermsId,
                           Name = x.PaymentTermsName,
                       }).ToListAsync();
            return data;
        }

        #endregion

        #region DeleteTab
        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await paymentTermsrepository.All().Where(x => ids.Contains(x.PaymentTermsId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            paymentTermsrepository.Delete(entity);

            return true;
        }
        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await paymentTermsrepository.All().AnyAsync(x => x.PaymentTermsId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await paymentTermsrepository.All().AnyAsync(x => x.Type == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await paymentTermsrepository.All().AnyAsync(x => x.PaymentTermsName == name && x.PaymentTermsId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Payment Terms" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Payment Terms" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Payment Terms" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Payment Terms" && x.CheckDelete);
        }
        #endregion
    }
}
