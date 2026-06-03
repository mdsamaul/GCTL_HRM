using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Qualifications
{
    public interface IQualificationService
    {
        List<HmsQualification> GetQualifications();
        HmsQualification GetQualification(string code); 
        bool DeleteQualification(string id);    
        HmsQualification SaveQualification(HmsQualification entity);
        bool IsQualificationExistByCode(string code);
        bool IsQualificationExist(string name);
        bool IsQualificationExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> QualificationSelection();
        bool SavePermission(string accessCode);
        bool UpdatePermission(string accessCode);
        bool DeletePermission(string accessCode);
    }
}