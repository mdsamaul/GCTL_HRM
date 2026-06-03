using GCTL.Core.Data;
using GCTL.Core.Enums;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Units
{
    public class MeasurementUnitService : AppService<CoreMeasurementUnit>, IMeasurementUnitService
    {
        private readonly IRepository<CoreMeasurementUnit> unitRepository;

        public MeasurementUnitService(IRepository<CoreMeasurementUnit> unitRepository)
            : base(unitRepository)
        {
            this.unitRepository = unitRepository;
        }

        public List<CoreMeasurementUnit> GetUnits()
        {
            return GetAll();
        }

        public List<CoreMeasurementUnit> GetUnits(UnitType unitType)
        {
            return unitRepository.FindBy(x => x.UnitType == unitType.ToString()).ToList();
        }

        public CoreMeasurementUnit GetUnit(string id)
        {
            return unitRepository.All().FirstOrDefault(x => x.UnitId == id);
        }

        public CoreMeasurementUnit SaveUnit(CoreMeasurementUnit entity)
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
