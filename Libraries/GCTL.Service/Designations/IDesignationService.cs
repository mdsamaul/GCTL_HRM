using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Data.Models;
using System.Threading.Tasks;

namespace GCTL.Service.Designations
{
    public interface IDesignationService
    {
        List<HrmDefDesignation> GetDesignations();
        HrmDefDesignation GetDesignation(string code); 
        Task<(bool success, bool refSuccess, string message)> DeleteDesignationAsync(List<string> ids, DeleteHistoryViewModel model);
        HrmDefDesignation SaveDesignation(HrmDefDesignation entity);
        bool IsDesignationExistByCode(string code);
        bool IsDesignationExist(string name);
        bool IsDesignationExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> DesignationSelection();
        bool SavePermission(string accessCode);
        bool UpdatePermission(string accessCode);
        bool DeletePermission(string accessCode);
    }
}