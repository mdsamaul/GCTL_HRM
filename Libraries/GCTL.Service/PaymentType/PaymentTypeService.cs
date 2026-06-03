using GCTL.Core.Data;
using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.PaymentModes;
using GCTL.Core.ViewModels.PaymentType;
using GCTL.Core.ViewModels.StyleInformation;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.PaymentType
{
    public class PaymentTypeService : AppService<SalesDefPaymentType>, IPaymentTypeService
    {
        #region Service & Repository
        private readonly IRepository<SalesDefPaymentType> paymentTyperepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;
        private const string TableName = "Sales_Def_PaymentType";
        private const string ColumnName = "PaymentTypeID";

        public PaymentTypeService(
            IRepository<SalesDefPaymentType> paymentTyperepository,
             IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService

            ) 
            
    : base(paymentTyperepository)
        {
            this.paymentTyperepository = paymentTyperepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        #endregion

        #region GetAllAsync

        public async Task<List<PaymentTypeSetupViewModel>> GetAllAsync()
        {
            var entity = await paymentTyperepository.GetAllAsync();
            return entity.Select(entityVM => new PaymentTypeSetupViewModel
            {
                Tc = entityVM.Tc,
                PaymentTypeId = entityVM.PaymentTypeId,
                PaymentType = entityVM.PaymentType,
                ShortName = entityVM.ShortName,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,

            }).ToList();
        }

        #endregion

        #region GetByIdAsync

        public async Task<PaymentTypeSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await paymentTyperepository.GetByIdAsync(code);
            if (entity == null) return null;

            PaymentTypeSetupViewModel entityVM = new PaymentTypeSetupViewModel();
            entityVM.Tc = entity.Tc;
            entityVM.PaymentTypeId = entity.PaymentTypeId;
            entityVM.PaymentType = entity.PaymentType;
            entityVM.ShortName = entity.ShortName;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(PaymentTypeSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            await paymentTyperepository.BeginTransactionAsync();
            try
            {
                SalesDefPaymentType entity = new SalesDefPaymentType();
                entity.PaymentTypeId = strMaxNO;
                entity.PaymentType = entityVM.PaymentType;
                entity.ShortName = entityVM.ShortName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await paymentTyperepository.AddAsync(entity);
                await paymentTyperepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await paymentTyperepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(PaymentTypeSetupViewModel entityVM)
        {
            await paymentTyperepository.BeginTransactionAsync();
            try
            {
                var entity = await paymentTyperepository.GetByIdAsync(entityVM.PaymentTypeId);
                if (entity == null)
                {
                    await paymentTyperepository.RollbackTransactionAsync();
                    return false;
                }
                entity.PaymentTypeId = entityVM.PaymentTypeId;
                entity.PaymentType = entityVM.PaymentType;
                entity.ShortName = entityVM.ShortName;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await paymentTyperepository.UpdateAsync(entity);
                await paymentTyperepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await paymentTyperepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region SelectionAsync
        public async Task<IEnumerable<CommonSelectModel>> SelectionPaymentTypeAsync()
        {

            var data = await paymentTyperepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.PaymentTypeId,
                           Name = x.PaymentType,
                       }).ToListAsync();
            return data;
        }

        #endregion

        #region DeleteTab
        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await paymentTyperepository.All().Where(x => ids.Contains(x.PaymentTypeId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            paymentTyperepository.Delete(entity);

            return true;
        }

        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await paymentTyperepository.All().AnyAsync(x => x.PaymentTypeId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await paymentTyperepository.All().AnyAsync(x => x.PaymentType == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await paymentTyperepository.All().AnyAsync(x => x.PaymentType == name && x.PaymentTypeId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Payment Type" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Payment Type" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Payment Type" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Payment Type" && x.CheckDelete);
        }
        #endregion

    }
}
