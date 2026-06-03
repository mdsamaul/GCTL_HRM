using GCTL.Core.Data;
using GCTL.Core.DataTables;
using GCTL.Core.ViewModels.Loggers;
using GCTL.Data.Models;

namespace GCTL.Service.UnitTypes
{
    public class UnitTypeService : AppService<RmgProdDefUnitType>, IUnitTypeService
    {
        private readonly IRepository<RmgProdDefUnitType> unitTypeRepository;

        public UnitTypeService(IRepository<RmgProdDefUnitType> unitTypeRepository)
            : base(unitTypeRepository)
        {
            this.unitTypeRepository = unitTypeRepository;
        }

        public List<RmgProdDefUnitType> GetUnitTypes()
        {
            return GetAll();
        }

        public RmgProdDefUnitType GetUnitType(string id)
        {
            return unitTypeRepository.GetById(id);
        }

        public RmgProdDefUnitType SaveUnitType(RmgProdDefUnitType entity)
        {
            if (IsUnitTypeExistByCode(entity.UnitTypId))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteUnitType(string id)
        {
            var entity = GetUnitType(id);
            if (entity != null)
            {
                unitTypeRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsUnitTypeExistByCode(string code)
        {
            return unitTypeRepository.All().Any(x => x.UnitTypId == code);
        }

        public bool IsUnitTypeExist(string name)
        {
            return unitTypeRepository.All().Any(x => x.UnitTypeName == name);
        }

        public bool IsUnitTypeExist(string name, string typeCode)
        {
            return unitTypeRepository.All().Any(x => x.UnitTypeName == name && x.UnitTypId != typeCode);
        }

        public PagedList<RmgProdDefUnitType> GetPagedUnitTypes(DataTablesOptions options)
        {
            return unitTypeRepository.All()
                .ApplySearch(options)
                .ApplySort(options)
                .ToPagedList(options);
        }
    }
}
