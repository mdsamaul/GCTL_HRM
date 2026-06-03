using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;


namespace GCTL.Service.DoctorWorkingPalace
{
    public  interface IDoctorWorkingPlaceService
    {
        List<HmsDoctorWorkingPlace> GetWorkingPlaces();
        HmsDoctorWorkingPlace GetWorkingPlace(string code);
        bool DeleteWorkingPlace(string id);
        HmsDoctorWorkingPlace SaveWorkingPlace(HmsDoctorWorkingPlace entity);
        bool IsWorkingPlaceExistByCode(string code);
        bool IsWorkingPlaceExist(string name);
        bool IsWorkingPlaceExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> WorkingPlaceSelection();
        bool SavePermission(string accessCode);
        bool UpdatePermission(string accessCode);
        bool DeletePermission(string accessCode);
    }
}
