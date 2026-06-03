using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Qualifications
{
    public class QualificationService : AppService<HmsQualification>, IQualificationService
    {
        private readonly IRepository<HmsQualification> qualificationRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        public QualificationService(IRepository<HmsQualification> qualificationRepository,
            IRepository<CoreAccessCode> accessCodeRepository)
            : base(qualificationRepository)
        {
            this.qualificationRepository = qualificationRepository;
            this.accessCodeRepository = accessCodeRepository;
        }

        public List<HmsQualification> GetQualifications()
        {
            return GetAll();
        }

        public HmsQualification GetQualification(string id)
        {
            return qualificationRepository.GetById(id);
        }

        public HmsQualification SaveQualification(HmsQualification entity)
        {
            if (IsQualificationExistByCode(entity.QualificationCode))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteQualification(string id)
        {
            var entity = GetQualification(id);
            if (entity != null)
            {
                qualificationRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsQualificationExistByCode(string code)
        {
            return qualificationRepository.All().Any(x => x.QualificationCode == code);
        }

        public bool IsQualificationExist(string name)
        {
            return qualificationRepository.All().Any(x => x.QualificationName == name);
        }

        public bool IsQualificationExist(string name, string typeCode)
        {
            return qualificationRepository.All().Any(x => x.QualificationName == name && x.QualificationCode != typeCode);
        }

        public IEnumerable<CommonSelectModel> QualificationSelection()
        {
            return qualificationRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.QualificationCode,
                    Name = x.QualificationName
                });
        }
        public bool SavePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Qualification" && x.CheckAdd);
        }
        public bool UpdatePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Qualification" && x.CheckEdit);
        }
        public bool DeletePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Qualification" && x.CheckDelete);
        }
    }
}
