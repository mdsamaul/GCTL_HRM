using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.BloodGroups
{
    public class BloodGroupService : AppService<HrmDefBloodGroup>, IBloodGroupService
    {
        private readonly IRepository<HrmDefBloodGroup> bloodGroupRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        public BloodGroupService(IRepository<HrmDefBloodGroup> bloodGroupRepository,
            IRepository<CoreAccessCode> accessCodeRepository)
            : base(bloodGroupRepository)
        {
            this.bloodGroupRepository = bloodGroupRepository;
            this.accessCodeRepository = accessCodeRepository;
        }

        public List<HrmDefBloodGroup> GetBloodGroups()
        {
            return GetAll();
        }

        public HrmDefBloodGroup GetBloodGroup(string id)
        {
            return bloodGroupRepository.GetById(id);
        }

        public HrmDefBloodGroup GetBloodGroup(decimal id)
        {
            return bloodGroupRepository.All().FirstOrDefault(x => x.AutoId == id);
        }

        public HrmDefBloodGroup SaveBloodGroup(HrmDefBloodGroup entity)
        {
            if (entity.AutoId > 0)
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteBloodGroup(string id)
        {
            var entity = GetBloodGroup(id);
            if (entity != null)
            {
                bloodGroupRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsBloodGroupExistByCode(string code)
        {
            return bloodGroupRepository.All().Any(x => x.BloodGroupCode == code);
        }

        public bool IsBloodGroupExist(string name)
        {
            return bloodGroupRepository.All().Any(x => x.BloodGroup == name);
        }

        public bool IsBloodGroupExist(string name, string typeCode)
        {
            return bloodGroupRepository.All().Any(x => x.BloodGroup == name && x.BloodGroupCode != typeCode);
        }

        public bool IsBloodGroupExist(string name, decimal autoId)
        {
            return bloodGroupRepository.All().Any(x => x.BloodGroup == name && x.AutoId != autoId);
        }

        public IEnumerable<CommonSelectModel> BloodGroupSelection()
        {
            return bloodGroupRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.BloodGroupCode,
                    Name = x.BloodGroup
                });
        }
        public bool SavePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Blood Group" && x.CheckAdd);
        }
        public bool UpdatePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Blood Group" && x.CheckEdit);
        }
        public bool DeletePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Blood Group" && x.CheckDelete);
        }
    }
}
