using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Units
{
    public class UnitService : AppService<HmsLtrvUnit>, IUnitService
    {
        private readonly IRepository<HmsLtrvUnit> unitRepository;

        public UnitService(IRepository<HmsLtrvUnit> unitRepository)
            : base(unitRepository)
        {
            this.unitRepository = unitRepository;
        }

        public List<HmsLtrvUnit> GetUnits()
        {
            return GetAll();
        }

        public HmsLtrvUnit GetUnit(string id)
        {
            return unitRepository.All().FirstOrDefault(x => x.UnitId == id);
        }

        public HmsLtrvUnit SaveUnit(HmsLtrvUnit entity)
        {
            if (IsUnitExistByCode(entity.UnitId))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteUnit(string id)
        {
            var entity = GetUnit(id);
            if (entity != null)
            {
                unitRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsUnitExistByCode(string code)
        {
            return unitRepository.All().Any(x => x.UnitId == code);
        }

        public bool IsUnitExist(string name)
        {
            return unitRepository.All().Any(x => x.UnitName == name);
        }

        public bool IsUnitExist(string name, string typeCode)
        {
            return unitRepository.All().Any(x => x.UnitName == name && x.UnitId != typeCode);
        }

        public IEnumerable<CommonSelectModel> UnitSelection()
        {
            return unitRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.UnitId,
                    Name = x.UnitName
                });
        }
    }
}
