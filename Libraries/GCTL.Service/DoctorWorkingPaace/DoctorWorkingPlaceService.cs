using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.DoctorWorkingPalace
{
    public class DoctorWorkingPlaceService : AppService<HmsDoctorWorkingPlace>, IDoctorWorkingPlaceService
    {
        private readonly IRepository<HmsDoctorWorkingPlace> doctorWorkingPlaceServiceRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        public DoctorWorkingPlaceService(IRepository<HmsDoctorWorkingPlace> doctorWorkingPlaceServiceRepository,
            IRepository<CoreAccessCode> accessCodeRepository)
            : base(doctorWorkingPlaceServiceRepository)
        {
            this.doctorWorkingPlaceServiceRepository = doctorWorkingPlaceServiceRepository;
            this.accessCodeRepository = accessCodeRepository;
        }
        public bool DeleteWorkingPlace(string id)
        {
            var entity = GetWorkingPlace(id);
            if (entity != null)
            {
                doctorWorkingPlaceServiceRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public IEnumerable<CommonSelectModel> WorkingPlaceSelection()
        {
            return doctorWorkingPlaceServiceRepository.All()
                  .Select(x => new CommonSelectModel
                  {
                      Code = x.WorkingPlaceCode,
                      Name = x.WorkingPlaceName
                  });
        }

        public HmsDoctorWorkingPlace GetWorkingPlace(string code)
        {
            return doctorWorkingPlaceServiceRepository.GetById(code);
          
        }

        public List<HmsDoctorWorkingPlace> GetWorkingPlaces()
        {
            return GetAll();
        }

        public bool IsWorkingPlaceExist(string name)
        {
            return doctorWorkingPlaceServiceRepository.All().Any(x => x.WorkingPlaceName == name);
        }

        public bool IsWorkingPlaceExist(string name, string typeCode)
        {
            return doctorWorkingPlaceServiceRepository.All().Any(x => x.WorkingPlaceName == name && x.WorkingPlaceCode != typeCode);
        }

        public bool IsWorkingPlaceExistByCode(string code)
        {
            return doctorWorkingPlaceServiceRepository.All().Any(x => x.WorkingPlaceCode == code);
        }

        public HmsDoctorWorkingPlace SaveWorkingPlace(HmsDoctorWorkingPlace entity)
        {
            if (IsWorkingPlaceExistByCode(entity.WorkingPlaceCode))
            {
                Update(entity);
            }
            else
            {
                Add(entity);

            }

                return entity;
        }
        public bool SavePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Working Place" && x.CheckAdd);
        }
        public bool UpdatePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Working Place" && x.CheckEdit);
        }
        public bool DeletePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Working Place" && x.CheckDelete);
        }
    }
}
