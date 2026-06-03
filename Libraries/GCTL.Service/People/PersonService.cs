using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.People
{
    public class PersonService : AppService<HmsLtrvPerson>, IPersonService
    {
        private readonly IRepository<HmsLtrvPerson> personRepository;

        public PersonService(IRepository<HmsLtrvPerson> personRepository)
            : base(personRepository)
        {
            this.personRepository = personRepository;
        }

        public List<HmsLtrvPerson> GetPersons()
        {
            return GetAll();
        }

        public HmsLtrvPerson GetPerson(string id)
        {
            return personRepository.All().FirstOrDefault(x => x.PersonId == id);
        }

        public HmsLtrvPerson SavePerson(HmsLtrvPerson entity)
        {
            if (IsPersonExistByCode(entity.PersonId))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeletePerson(string id)
        {
            var entity = GetPerson(id);
            if (entity != null)
            {
                personRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsPersonExistByCode(string code)
        {
            return personRepository.All().Any(x => x.PersonId == code);
        }

        public bool IsPersonExist(string name)
        {
            return personRepository.All().Any(x => x.PersonName == name);
        }

        public bool IsPersonExist(string name, string typeCode)
        {
            return personRepository.All().Any(x => x.PersonName == name && x.PersonId != typeCode);
        }

        public IEnumerable<CommonSelectModel> PersonSelection()
        {
            return personRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.PersonId,
                    Name = x.PersonName
                });
        }
    }
}
