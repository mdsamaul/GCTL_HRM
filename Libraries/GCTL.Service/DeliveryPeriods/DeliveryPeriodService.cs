using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.DeliveryPeriods
{
    public class DeliveryPeriodService : AppService<HmsDeliveryPeriod>, IDeliveryPeriodService
    {
        private readonly IRepository<HmsDeliveryPeriod> deliveryPeriodRepository;

        public DeliveryPeriodService(IRepository<HmsDeliveryPeriod> deliveryPeriodRepository)
            : base(deliveryPeriodRepository)
        {
            this.deliveryPeriodRepository = deliveryPeriodRepository;
        }

        public List<HmsDeliveryPeriod> GetDeliveryPeriods()
        {
            return GetAll();
        }

        public HmsDeliveryPeriod GetDeliveryPeriod(string id)
        {
            return deliveryPeriodRepository.All().FirstOrDefault(x => x.DeliveryPeriodId == id);
        }

        public HmsDeliveryPeriod SaveDeliveryPeriod(HmsDeliveryPeriod entity)
        {
            if (IsDeliveryPeriodExistByCode(entity.DeliveryPeriodId))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteDeliveryPeriod(string id)
        {
            var entity = GetDeliveryPeriod(id);
            if (entity != null)
            {
                deliveryPeriodRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsDeliveryPeriodExistByCode(string code)
        {
            return deliveryPeriodRepository.All().Any(x => x.DeliveryPeriodId == code);
        }

        public bool IsDeliveryPeriodExist(string name)
        {
            return deliveryPeriodRepository.All().Any(x => x.DeliveryPeriod == name);
        }

        public bool IsDeliveryPeriodExist(string name, string typeCode)
        {
            return deliveryPeriodRepository.All().Any(x => x.DeliveryPeriod == name && x.DeliveryPeriodId != typeCode);
        }

        public IEnumerable<CommonSelectModel> DeliveryPeriodSelection()
        {
            return deliveryPeriodRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.DeliveryPeriodId,
                    Name = x.DeliveryPeriod
                });
        }
    }
}
