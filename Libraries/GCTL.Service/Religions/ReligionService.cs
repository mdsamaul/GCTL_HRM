using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Religions
{
    public class ReligionService : AppService<HrmDefReligion>, IReligionService
    {
        private readonly IRepository<HrmDefReligion> religionRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public ReligionService(IRepository<HrmDefReligion> religionRepository,
            IRepository<CoreAccessCode> accessCodeRepository)
            : base(religionRepository)
        {
            this.religionRepository = religionRepository;
            this.accessCodeRepository = accessCodeRepository;
        }

        public List<HrmDefReligion> GetReligions()
        {
            return GetAll();
        }

        public HrmDefReligion GetReligion(string id)
        {
            return religionRepository.GetById(id);
        }

        public HrmDefReligion SaveReligion(HrmDefReligion entity)
        {
            if (IsReligionExistByCode(entity.ReligionCode))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteReligion(string id)
        {
            var entity = GetReligion(id);
            if (entity != null)
            {
                religionRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsReligionExistByCode(string code)
        {
            return religionRepository.All().Any(x => x.ReligionCode == code);
        }

        public bool IsReligionExist(string name)
        {
            return religionRepository.All().Any(x => x.Religion == name);
        }

        public bool IsReligionExist(string name, string typeCode)
        {
            return religionRepository.All().Any(x => x.Religion == name && x.ReligionCode != typeCode);
        }

        public IEnumerable<CommonSelectModel> ReligionSelection()
        {
            return religionRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.ReligionCode,
                    Name = x.Religion
                });
        }
        public bool SavePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Religion" && x.CheckAdd);
        }
        public bool UpdatePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Religion" && x.CheckEdit);
        }
        public bool DeletePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Religion" && x.CheckDelete);
        }
    }
}
