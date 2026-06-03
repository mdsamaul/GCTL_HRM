using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Periods
{
    public class PeriodService : AppService<HmsLtrvPeriod>, IPeriodService
    {
        private readonly IRepository<HmsLtrvPeriod> periodRepository;

        public PeriodService(IRepository<HmsLtrvPeriod> periodRepository)
            : base(periodRepository)
        {
            this.periodRepository = periodRepository;
        }

        public List<HmsLtrvPeriod> GetPeriods()
        {
            return GetAll();
        }

        public HmsLtrvPeriod GetPeriod(string id)
        {
            return periodRepository.All().FirstOrDefault(x => x.PeriodId == id);
        }

        public HmsLtrvPeriod SavePeriod(HmsLtrvPeriod entity)
        {
            if (IsPeriodExistByCode(entity.PeriodId))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeletePeriod(string id)
        {
            var entity = GetPeriod(id);
            if (entity != null)
            {
                periodRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsPeriodExistByCode(string code)
        {
            return periodRepository.All().Any(x => x.PeriodId == code);
        }

        public bool IsPeriodExist(string name)
        {
            return periodRepository.All().Any(x => x.PeriodName == name);
        }

        public bool IsPeriodExist(string name, string typeCode)
        {
            return periodRepository.All().Any(x => x.PeriodName == name && x.PeriodId != typeCode);
        }

        public IEnumerable<CommonSelectModel> PeriodSelection()
        {
            return periodRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.PeriodId,
                    Name = x.PeriodName
                });
        }
    }
}
