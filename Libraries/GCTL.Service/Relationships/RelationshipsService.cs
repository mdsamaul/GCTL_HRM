using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.Relationship;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.Relationships
{
    public class RelationshipsService : AppService<HrmDefRelationship>, IRelationshipsService
    {
        private readonly IRepository<HrmDefRelationship> relationshipRepository;
        private readonly IRepository<CoreAccessCode> coreAccessCodeRepository;

        public RelationshipsService(IRepository<HrmDefRelationship> relationshipRepository, IRepository<CoreAccessCode> coreAccessCodeRepository)             
  : base(relationshipRepository)
        {
            this.relationshipRepository = relationshipRepository;
            this.coreAccessCodeRepository = coreAccessCodeRepository;
        }

        public async Task<RelationshipSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await relationshipRepository.GetByIdAsync(code);
            if (entity == null)
            {
                return null;
            }
            return new RelationshipSetupViewModel

            {
                AutoId = entity.AutoId, //id
                RelationshipCode = entity.RelationshipCode,
                Relationship = entity.Relationship,
                ShortName = entity.ShortName,

                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Luser = entity.Luser,
                Lip = entity.Lip,
                Lmac = entity.Lmac
            };
        }
        public async Task<List<RelationshipSetupViewModel>> GetAllAsync()
        {
            var entity = await relationshipRepository.GetAllAsync();
            return entity.Select(entityVM => new RelationshipSetupViewModel
            {
                AutoId = entityVM.AutoId,
                RelationshipCode = entityVM.RelationshipCode,
                Relationship = entityVM.Relationship,
                ShortName = entityVM.ShortName,

                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac

            }).ToList();
        }


        public async Task<bool> SaveAsync(RelationshipSetupViewModel entityVM)
        {
            await relationshipRepository.BeginTransactionAsync();
            try
            {

                HrmDefRelationship entity = new HrmDefRelationship();

                entity.RelationshipCode = await GenerateNextCode();
                entity.Relationship = entityVM.Relationship;
                entity.ShortName = entityVM.ShortName ?? string.Empty;

                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                entity.ModifyDate = DateTime.Now;
                await relationshipRepository.AddAsync(entity);
                await relationshipRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await relationshipRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> UpdateAsync(RelationshipSetupViewModel entityVM)
        {
            await relationshipRepository.BeginTransactionAsync();
            try
            {

                var entity = await relationshipRepository.GetByIdAsync(entityVM.RelationshipCode);
                if (entity == null)
                {
                    await relationshipRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.AutoId = entityVM.AutoId;
                entity.RelationshipCode = entityVM.RelationshipCode;
                entity.Relationship = entityVM.Relationship;
                entity.ShortName = entityVM.ShortName ?? string.Empty;

                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                await relationshipRepository.UpdateAsync(entity);
                await relationshipRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await relationshipRepository.RollbackTransactionAsync();
                return false;
            }
        }

        public bool DeleteLeaveType(string id)
        {
            var entity = GetLeaveType(id);
            if (entity != null)
            {
                relationshipRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public HrmDefRelationship GetLeaveType(string code)
        {
            return relationshipRepository.GetById(code);
        }

        public async Task<IEnumerable<CommonSelectModel>> RelationshipsSelectionAsync()
        {

            var data = await relationshipRepository.All()
                      .Select(x => new CommonSelectModel
                      {
                          Code = x.RelationshipCode,
                          Name = x.Relationship,
                      })
                      .ToListAsync();
            return data;
        }

        #region NextCode


        public async Task<string> GenerateNextCode()
        {

            var Code = await relationshipRepository.GetAllAsync();
            var lastCode = Code.Max(b => b.RelationshipCode);
            int nextCode = 1;
            if (!string.IsNullOrEmpty(lastCode))
            {
                int lastNumber = int.Parse(lastCode.TrimStart('0'));
                lastNumber++;
                nextCode = lastNumber;
            }
            return nextCode.ToString("D2");

        }

        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await relationshipRepository.All().AnyAsync(x => x.RelationshipCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await relationshipRepository.All().AnyAsync(x => x.Relationship == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await relationshipRepository.All().AnyAsync(x => x.Relationship == name && x.RelationshipCode != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Relationship" && x.CheckDelete);
        }

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Relationship" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Relationship" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Relationship" && x.CheckEdit);

        }
        #endregion
    }
}
