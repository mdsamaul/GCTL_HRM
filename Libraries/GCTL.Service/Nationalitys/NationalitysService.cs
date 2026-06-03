using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.Nationalitys;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.Nationalitys
{
    public class NationalitysService: AppService<HrmDefNationality>, INationalitysService
    {
        private readonly IRepository<HrmDefNationality> nationalRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;
        private const string TableName = "HRM_Def_Nationality";
        private const string ColumnName = "NationalityCode";

        public NationalitysService(IRepository<HrmDefNationality> nationalRepository, IRepository<CoreAccessCode> accessCodeRepository, ICommonService commonService) : base(nationalRepository)
        {
            this.nationalRepository = nationalRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        public async Task<List<NationalitysSetupViewModel>> GetAllAsync()
        {
            var entity = await nationalRepository.GetAllAsync();
            return entity.Select(entityVM => new NationalitysSetupViewModel
            {
                AutoId = entityVM.AutoId,
                NationalityCode = entityVM.NationalityCode,
                Nationality = entityVM.Nationality,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,



            }).ToList();
        }

        public async Task<NationalitysSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await nationalRepository.GetByIdAsync(code);
            if (entity == null) return null;

            NationalitysSetupViewModel entityVM = new NationalitysSetupViewModel();
            entityVM.AutoId = entity.AutoId;
            entityVM.NationalityCode = entity.NationalityCode;
            entityVM.Nationality = entity.Nationality;
            entityVM.Luser = entity.Luser;
            entityVM.Ldate = entity.Ldate;
            entityVM.ModifyDate = entity.ModifyDate;
            entityVM.Lip = entity.Lip;
            entityVM.Lmac = entity.Lmac;

            return entityVM;
        }

        public async Task<IEnumerable<CommonSelectModel>> SelectionNationalityAsync()
        {

            var data = await nationalRepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.NationalityCode,
                           Name = x.Nationality,
                       }).ToListAsync();
            return data;
        }


        public async Task<bool> SaveAsync(NationalitysSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 2);
            await nationalRepository.BeginTransactionAsync();
            try
            {

                HrmDefNationality entity = new HrmDefNationality();
                entity.NationalityCode = strMaxNO;
                entity.Nationality = entityVM.Nationality;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await nationalRepository.AddAsync(entity);
                await nationalRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await nationalRepository.RollbackTransactionAsync();

                return false;
            }
        }

        public async Task<bool> UpdateAsync(NationalitysSetupViewModel entityVM)
        {
            await nationalRepository.BeginTransactionAsync();
            try
            {

                var entity = await nationalRepository.GetByIdAsync(entityVM.NationalityCode);
                if (entity == null)
                {
                    await nationalRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.NationalityCode = entityVM.NationalityCode;
                entity.Nationality = entityVM.Nationality;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await nationalRepository.UpdateAsync(entity);
                await nationalRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await nationalRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await nationalRepository.All().Where(x => ids.Contains(x.NationalityCode)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            nationalRepository.Delete(entity);

            return true;
        }


        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await nationalRepository.All().AnyAsync(x => x.NationalityCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await nationalRepository.All().AnyAsync(x => x.Nationality == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await nationalRepository.All().AnyAsync(x => x.Nationality == name && x.NationalityCode != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Nationality" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Nationality " && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Nationality" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Nationality" && x.CheckDelete);
        }
        #endregion
    }
}
