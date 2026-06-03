using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.Districts;
using GCTL.Core.ViewModels.Relationship;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.Relationships
{
    public interface IRelationshipsService
    {
        HrmDefRelationship GetLeaveType(string code);
        bool DeleteLeaveType(string id);
        Task<List<RelationshipSetupViewModel>> GetAllAsync();
        Task<RelationshipSetupViewModel> GetByIdAsync(string code);
        Task<bool> SaveAsync(RelationshipSetupViewModel entityVM);
        Task<bool> UpdateAsync(RelationshipSetupViewModel entityVM);

        Task<string> GenerateNextCode();
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<IEnumerable<CommonSelectModel>> RelationshipsSelectionAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}
