using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.Relation
{
    public interface IRelationService
    {
        List<HmsRelation> GetRelations();
        HmsRelation GetRelation(string code);
        bool DeleteRelation(string id);
        HmsRelation SaveRelation(HmsRelation entity);
        bool IsRelationExistByCode(string code);
        bool IsRelationExist(string name);
        bool IsRelationExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> RelationSelection();
    }
}
