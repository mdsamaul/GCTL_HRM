using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.SeparationTypes;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.SeparationTypes
{
    public class SeparationTypesService : AppService<HrmDefSeparationType>, ISeparationTypesService
    {
        private readonly IRepository<HrmDefSeparationType> separationTypesRepository;
        private readonly IDeleteHistoryService deleteHistoryService;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;
        private const string TableName = "HRM_Def_SeparationType";
        private const string ColumnName = "SeparationTypeId";

        public SeparationTypesService(IRepository<HrmDefSeparationType> separationTypesRepository, IDeleteHistoryService deleteHistoryService, IRepository<CoreAccessCode> accessCodeRepository, ICommonService commonService) : base(separationTypesRepository)
        {
            this.separationTypesRepository = separationTypesRepository;
            this.deleteHistoryService = deleteHistoryService;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        public async Task<List<SeparationTypesSetupViewModel>> GetAllAsync()
        {
            var entity = await separationTypesRepository.GetAllAsync();
            return entity.Select(entityVM => new SeparationTypesSetupViewModel
            {
                SeparationTypeCode = entityVM.SeparationTypeCode,
                SeparationTypeId = entityVM.SeparationTypeId,
                SeparationType = entityVM.SeparationType,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,



            }).ToList();
        }

        public async Task<SeparationTypesSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await separationTypesRepository.GetByIdAsync(code);
            if (entity == null) return null;

            SeparationTypesSetupViewModel entityVM = new SeparationTypesSetupViewModel();
            entityVM.SeparationTypeCode = entity.SeparationTypeCode;
            entityVM.SeparationTypeId = entity.SeparationTypeId;
            entityVM.SeparationType = entity.SeparationType;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        public async Task<IEnumerable<CommonSelectModel>> SelectionSeparationTypeAsync()
        {

            var data = await separationTypesRepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.SeparationTypeId,
                           Name = x.SeparationType,
                       }).ToListAsync();
            return data;
        }


        public async Task<bool> SaveAsync(SeparationTypesSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 2);
            await separationTypesRepository.BeginTransactionAsync();
            try
            {

                HrmDefSeparationType entity = new HrmDefSeparationType();
                entity.SeparationTypeId = strMaxNO;
                entity.SeparationType = entityVM.SeparationType;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await separationTypesRepository.AddAsync(entity);
                await separationTypesRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await separationTypesRepository.RollbackTransactionAsync();

                return false;
            }
        }

        public async Task<bool> UpdateAsync(SeparationTypesSetupViewModel entityVM)
        {
            await separationTypesRepository.BeginTransactionAsync();
            try
            {

                var entity = await separationTypesRepository.GetByIdAsync(entityVM.SeparationTypeId);
                if (entity == null)
                {
                    await separationTypesRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.SeparationTypeId = entityVM.SeparationTypeId;
                entity.SeparationType = entityVM.SeparationType;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await separationTypesRepository.UpdateAsync(entity);
                await separationTypesRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await separationTypesRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<(bool succses, string messege, bool refSuccess)> DeleteTab(List<string> ids, DeleteHistoryViewModel model)
        {
            if (!ids.Any())
                return (false, "No Data found to delete", false);

            var _table = separationTypesRepository.GetTableName();

            var dependencyCheck = await deleteHistoryService.CheckDependenciesAsync(
                _table,
                ColumnName,
                ids.ToList()
            );

            if (!dependencyCheck.CanDelete)
                return (false, dependencyCheck.Message, true);

            await separationTypesRepository.BeginTransactionAsync();

            try
            {
                var entity = await separationTypesRepository.All().Where(x => ids.Contains(x.SeparationTypeId)).ToListAsync();


                separationTypesRepository.Delete(entity);
                model.tableName = _table;
                await deleteHistoryService.LogDeletedRecordsAsync(
                    entity, model);

                await separationTypesRepository.CommitTransactionAsync();
                return (true, "Delete Successfully", false);
            }
            catch (Exception ex)
            {
                await separationTypesRepository.RollbackTransactionAsync();
                Console.WriteLine(ex.ToString());
                return (false, "Delete Failed", false);

            }

        }


        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await separationTypesRepository.All().AnyAsync(x => x.SeparationTypeId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await separationTypesRepository.All().AnyAsync(x => x.SeparationType == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await separationTypesRepository.All().AnyAsync(x => x.SeparationType == name && x.SeparationTypeId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Separation Type" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Separation Type" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Separation Type" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Separation Type" && x.CheckDelete);
        }
        #endregion
    }
}
