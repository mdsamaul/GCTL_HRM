using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.PaymentModes
{
    public class PaymentModeService : AppService<SalesDefPaymentMode>, IPaymentModeService
    {
        private readonly IRepository<SalesDefPaymentMode> paymentModeRepository;

        public PaymentModeService(IRepository<SalesDefPaymentMode> paymentModeRepository)
            : base(paymentModeRepository)
        {
            this.paymentModeRepository = paymentModeRepository;
        }

        public List<SalesDefPaymentMode> GetPaymentModes()
        {
            return GetAll();
        }

        public SalesDefPaymentMode GetPaymentMode(string id)
        {
            return paymentModeRepository.GetById(id);
        }

        public SalesDefPaymentMode SavePaymentMode(SalesDefPaymentMode entity)
        {
            if (IsPaymentModeExistByCode(entity.PaymentModeId))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeletePaymentMode(string id)
        {
            var entity = GetPaymentMode(id);
            if (entity != null)
            {
                paymentModeRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsPaymentModeExistByCode(string code)
        {
            return paymentModeRepository.All().Any(x => x.PaymentModeId == code);
        }

        public bool IsPaymentModeExist(string name)
        {
            return paymentModeRepository.All().Any(x => x.PaymentModeName == name);
        }

        public bool IsPaymentModeExist(string name, string typeCode)
        {
            return paymentModeRepository.All().Any(x => x.PaymentModeName == name && x.PaymentModeId != typeCode);
        }

        public IEnumerable<CommonSelectModel> PaymentModeSelection()
        {
            return paymentModeRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.PaymentModeId,
                    Name = x.PaymentModeName
                });
        }
        public IEnumerable<CommonSelectModel> PaymentModeSelectionOnlyCash()
        {
            return paymentModeRepository.All().Where(x=>x.PaymentModeId== "001")
                .Select(x => new CommonSelectModel
                {
                    Code = x.PaymentModeId,
                    Name = x.PaymentModeName
                });
        }
    }
}
