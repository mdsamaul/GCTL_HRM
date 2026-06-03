using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.DoctorTypes
{
    public class DoctorTypeService : AppService<HmsDoctorType>, IDoctorTypeService
    {
        private readonly IRepository<HmsDoctorType> doctorTypeRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        public DoctorTypeService(IRepository<HmsDoctorType> doctorTypeRepository,
            IRepository<CoreAccessCode> accessCodeRepository)
            : base(doctorTypeRepository)
        {
            this.doctorTypeRepository = doctorTypeRepository;
            this.accessCodeRepository = accessCodeRepository;
        }

        public List<HmsDoctorType> GetDoctorTypes()
        {
            return GetAll();
        }

        public HmsDoctorType GetDoctorType(string id)
        {
            return doctorTypeRepository.GetById(id);
        }

        public HmsDoctorType SaveDoctorType(HmsDoctorType entity)
        {
            if (IsDoctorTypeExistByCode(entity.DoctorTypeCode))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteDoctorType(string id)
        {
            var entity = GetDoctorType(id);
            if (entity != null)
            {
                doctorTypeRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsDoctorTypeExistByCode(string code)
        {
            return doctorTypeRepository.All().Any(x => x.DoctorTypeCode == code);
        }

        public bool IsDoctorTypeExist(string name)
        {
            return doctorTypeRepository.All().Any(x => x.DoctorTypeName == name);
        }

        public bool IsDoctorTypeExist(string name, string typeCode)
        {
            return doctorTypeRepository.All().Any(x => x.DoctorTypeName == name && x.DoctorTypeCode != typeCode);
        }

        public IEnumerable<CommonSelectModel> DoctorTypeSelection()
        {
            return doctorTypeRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.DoctorTypeCode,
                    Name = x.DoctorTypeName
                });
        }
        public bool SavePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Doctor Type" && x.CheckAdd);
        }
        public bool UpdatePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Doctor Type" && x.CheckEdit);
        }
        public bool DeletePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Doctor Type" && x.CheckDelete);
        }
    }
}
