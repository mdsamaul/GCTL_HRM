using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;
using GCTL.Service.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.Relation
{
    public class RelationService : AppService<HmsRelation>, IRelationService
    {
        private readonly IRepository<HmsRelation> relationRepository;

        public RelationService(IRepository<HmsRelation> relationRepository)
            : base(relationRepository)
        {
            this.relationRepository = relationRepository;
        }

        public bool DeleteRelation(string id)
        {
            var entity = GetRelation(id);
            if (entity != null)
            {
                relationRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public HmsRelation GetRelation(string code)
        {
            return relationRepository.All().FirstOrDefault(x => x.RelationCode == code);
        }

        public List<HmsRelation> GetRelations()
        {
            return GetAll();
        }

        public bool IsRelationExist(string name)
        {
            return relationRepository.All().Any(x => x.Relation == name);
        }

        public bool IsRelationExist(string name, string typeCode)
        {
            return relationRepository.All().Any(x => x.Relation == name && x.RelationCode != typeCode);
        }

        public bool IsRelationExistByCode(string code)
        {
            return relationRepository.All().Any(x => x.RelationCode == code);
        }

        public IEnumerable<CommonSelectModel> RelationSelection()
        {
            return relationRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.RelationCode,
                    Name = x.Relation
                });
        }

        public HmsRelation SaveRelation(HmsRelation entity)
        {
            if (IsRelationExistByCode(entity.RelationCode))
                Update(entity);
            else
                Add(entity);

            return entity;
        }
    }
}
