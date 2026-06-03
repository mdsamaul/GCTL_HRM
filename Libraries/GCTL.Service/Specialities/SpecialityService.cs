using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Specialities
{
    public class SpecialityService : AppService<HmsSpeciality>, ISpecialityService
    {
        private readonly IRepository<HmsSpeciality> specialityRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        public SpecialityService(IRepository<HmsSpeciality> specialityRepository,
            IRepository<CoreAccessCode> accessCodeRepository)
            : base(specialityRepository)
        {
            this.specialityRepository = specialityRepository;
            this.accessCodeRepository = accessCodeRepository;
        }

        public List<HmsSpeciality> GetSpecialities()
        {
            return GetAll();
        }

        public HmsSpeciality GetSpeciality(string id)
        {
            return specialityRepository.GetById(id);
        }

        public HmsSpeciality SaveSpeciality(HmsSpeciality entity)
        {
            if (IsSpecialityExistByCode(entity.SpecialityCode))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteSpeciality(string id)
        {
            var entity = GetSpeciality(id);
            if (entity != null)
            {
                specialityRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsSpecialityExistByCode(string code)
        {
            return specialityRepository.All().Any(x => x.SpecialityCode == code);
        }

        public bool IsSpecialityExist(string name)
        {
            return specialityRepository.All().Any(x => x.SpecialityName == name);
        }

        public bool IsSpecialityExist(string name, string typeCode)
        {
            return specialityRepository.All().Any(x => x.SpecialityName == name && x.SpecialityCode != typeCode);
        }

        public IEnumerable<CommonSelectModel> SpecialitySelection()
        {
            return specialityRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.SpecialityCode,
                    Name = x.SpecialityName
                });
        }
        public bool SavePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Specialist" && x.CheckAdd);
        }
        public bool UpdatePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Specialist" && x.CheckEdit);
        }
        public bool DeletePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Specialist" && x.CheckDelete);
        }
    }
}
