using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.AdmissionTypes
{
    public class AdmissionTypeService : AppService<HmsAdmissionType>, IAdmissionTypeService
    {
        private readonly IRepository<HmsAdmissionType> admissionTypeRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public AdmissionTypeService(IRepository<HmsAdmissionType> admissionTypeRepository,
            IRepository<CoreAccessCode> accessCodeRepository)
            : base(admissionTypeRepository)
        {
            this.admissionTypeRepository = admissionTypeRepository;
            this.accessCodeRepository = accessCodeRepository;
        }

        public List<HmsAdmissionType> GetAdmissionTypes()
        {
            return GetAll();
        }

        public HmsAdmissionType GetAdmissionType(string id)
        {
            return admissionTypeRepository.GetById(id);
        }

        public HmsAdmissionType SaveAdmissionType(HmsAdmissionType entity)
        {
            if (IsAdmissionTypeExistByCode(entity.AdmissionTypeId))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteAdmissionType(string id)
        {
            var entity = GetAdmissionType(id);
            if (entity != null)
            {
                admissionTypeRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsAdmissionTypeExistByCode(string code)
        {
            return admissionTypeRepository.All().Any(x => x.AdmissionTypeId == code);
        }

        public bool IsAdmissionTypeExist(string name)
        {
            return admissionTypeRepository.All().Any(x => x.AdmissionTypeName == name);
        }

        public bool IsAdmissionTypeExist(string name, string typeCode)
        {
            return admissionTypeRepository.All().Any(x => x.AdmissionTypeName == name && x.AdmissionTypeId != typeCode);
        }

        public IEnumerable<CommonSelectModel> AdmissionTypeSelection()
        {
            return admissionTypeRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.AdmissionTypeId,
                    Name = x.AdmissionTypeName
                });
        }
        public bool SavePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Admission Type" && x.CheckAdd);
        }
        public bool UpdatePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Admission Type" && x.CheckEdit);
        }
        public bool DeletePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Admission Type" && x.CheckDelete);
        }
    }
}
