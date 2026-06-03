using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Surnames
{
    public class SurnameService : AppService<HrmDefSurName>, ISurnameService
    {
        private readonly IRepository<HrmDefSurName> surnameRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public SurnameService(IRepository<HrmDefSurName> surnameRepository,
            IRepository<CoreAccessCode> accessCodeRepository)
            : base(surnameRepository)
        {
            this.surnameRepository = surnameRepository;
            this.accessCodeRepository = accessCodeRepository;
        }

        public List<HrmDefSurName> GetSurnames()
        {
            return GetAll();
        }

        public HrmDefSurName GetSurname(string id)
        {
            return surnameRepository.All().FirstOrDefault(x => x.SurnameId == id);          
        }

        public HrmDefSurName GetSurname(decimal id)
        {
            return surnameRepository.GetById(id);
        }

        public HrmDefSurName SaveSurname(HrmDefSurName entity)
        {
            if (entity.AutoId > 0)
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteSurname(string id)
        {
            var entity = GetSurname(id);
            if (entity != null)
            {
                surnameRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsSurnameExistByCode(string code)
        {
            return surnameRepository.All().Any(x => x.SurnameId == code);
        }

        public bool IsSurnameExist(string name)
        {
            return surnameRepository.All().Any(x => x.Surname == name);
        }

        public bool IsSurnameExist(string name, string typeCode)
        {
            return surnameRepository.All().Any(x => x.Surname == name && x.SurnameId != typeCode);
        }

        public bool IsSurnameExist(string name, decimal autoId)
        {
            return surnameRepository.All().Any(x => x.Surname == name && x.AutoId != autoId);
        }

        public IEnumerable<CommonSelectModel> SurnameSelection()
        {
            return surnameRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.SurnameId,
                    Name = x.Surname
                });
        }
        public bool SavePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Surname" && x.CheckAdd);
        }
        public bool UpdatePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Surname" && x.CheckEdit);
        }
        public bool DeletePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Surname" && x.CheckDelete);
        }
    }
}
