using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Shifts
{
    public class ShiftService : AppService<HmsShift>, IShiftService
    {
        private readonly IRepository<HmsShift> shiftRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        public ShiftService(IRepository<HmsShift> shiftRepository,
            IRepository<CoreAccessCode> accessCodeRepository)
            : base(shiftRepository)
        {
            this.shiftRepository = shiftRepository;
            this.accessCodeRepository = accessCodeRepository;
        }

        public List<HmsShift> GetShifts()
        {
            return GetAll();
        }

        public HmsShift GetShift(string id)
        {
            return shiftRepository.GetById(id);
        }

        public HmsShift SaveShift(HmsShift entity)
        {
            if (IsShiftExistByCode(entity.ShiftCode))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteShift(string id)
        {
            var entity = GetShift(id);
            if (entity != null)
            {
                shiftRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsShiftExistByCode(string code)
        {
            return shiftRepository.All().Any(x => x.ShiftCode == code);
        }

        public bool IsShiftExist(string name)
        {
            return shiftRepository.All().Any(x => x.ShiftName == name);
        }

        public bool IsShiftExist(string name, string typeCode)
        {
            return shiftRepository.All().Any(x => x.ShiftName == name && x.ShiftCode != typeCode);
        }

        public IEnumerable<CommonSelectModel> ShiftSelection()
        {
            return shiftRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.ShiftCode,
                    Name = x.ShiftName
                });
        }
        public bool SavePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Shift" && x.CheckAdd);
        }
        public bool UpdatePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Shift" && x.CheckEdit);
        }
        public bool DeletePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Shift" && x.CheckDelete);
        }
    }
}
