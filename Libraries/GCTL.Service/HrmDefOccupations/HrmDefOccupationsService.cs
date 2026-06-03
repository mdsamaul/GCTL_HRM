using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefOccupations;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefOccupations
{
    public class HrmDefOccupationsService:AppService<HrmDefOccupation>,IHrmDefOccupationsService
    {
        private readonly IRepository<HrmDefOccupation> hrmDefOccupation;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        string strMaxCode=string.Empty;
        private const string TableName = "HRM_Def_Occupation";
        private const string ColumnName = "OccupationCode";


        public HrmDefOccupationsService(IRepository<HrmDefOccupation> hrmDefOccupation, IRepository<CoreAccessCode> accessCodeRepository, ICommonService commonService):base(hrmDefOccupation) 
        {
            this.hrmDefOccupation = hrmDefOccupation;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
        }

        public async Task<List<HrmDefOccupationsSetupViewModel>> GetAllAsync()
        {
            var data = await hrmDefOccupation.All().Select(x => new HrmDefOccupationsSetupViewModel
            {
               AutoId=x.AutoId,
               OccupationCode=x.OccupationCode,
               Occupation=x.Occupation,
               ShortName=x.ShortName,
               Ldate=x.Ldate,
               ModifyDate=x.ModifyDate,
               Luser=x.Luser,
               Lmac=x.Lmac,
               Lip=x.Lip
            }).ToListAsync();
            return data;
        }

        public async Task<HrmDefOccupationsSetupViewModel> GetByIdAsync(string code)
        {
            var data = await hrmDefOccupation.All()
                .Where(x => x.OccupationCode == code)
                .Select(x => new HrmDefOccupationsSetupViewModel
            {
                AutoId = x.AutoId,
                OccupationCode = x.OccupationCode,
                Occupation = x.Occupation,
                ShortName = x.ShortName,
                Ldate = x.Ldate,
                ModifyDate = x.ModifyDate,
                Luser = x.Luser,
                Lmac = x.Lmac,
                Lip = x.Lip
            }).FirstOrDefaultAsync();
            return data;
        }

        public async Task<bool> SaveAsync(HrmDefOccupationsSetupViewModel entityVM)
        {
            await hrmDefOccupation.BeginTransactionAsync();
            commonService.FindMaxNo(ref strMaxCode, ColumnName, TableName, 2);
            try
            {
                HrmDefOccupation entity = new HrmDefOccupation();
                entity.OccupationCode = strMaxCode;
                entity.Occupation = entityVM.Occupation?? string.Empty;
                entity.ShortName = entityVM.ShortName?? string.Empty;
                entity.Ldate = DateTime.Now;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.Luser = entityVM.Luser;
                await hrmDefOccupation.AddAsync(entity);
                await hrmDefOccupation.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error{ex.Message}");
                await hrmDefOccupation.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<bool> UpdateAsync(HrmDefOccupationsSetupViewModel entityVM)
        {
             await hrmDefOccupation.BeginTransactionAsync();
            try
            {
                var entity = hrmDefOccupation.GetById(entityVM.OccupationCode);
                if(entity==null)
                {
                    await hrmDefOccupation.RollbackTransactionAsync();
                    return false;
                }

                entity.OccupationCode = entityVM.OccupationCode;
                entity.Occupation = entityVM.Occupation ?? string.Empty;
                entity.ShortName = entityVM.ShortName ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                entity.Lip = entityVM.Lip?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Luser = entityVM.Luser ?? string.Empty;
                await hrmDefOccupation.UpdateAsync(entity);
                await hrmDefOccupation.CommitTransactionAsync();
                return true;

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
                await hrmDefOccupation.RollbackTransactionAsync();
                return false;
            }

        }


        #region Delete

        public async Task<bool> DeleteAsyncTab(List<string> ids)
        {
            await hrmDefOccupation.BeginTransactionAsync();
            try
            {
                var data = await hrmDefOccupation.All().Where(x => ids.Contains(x.OccupationCode)).ToListAsync();
                if (!data.Any())
                {
                    return false;
                }
                hrmDefOccupation.Delete(data);
                await hrmDefOccupation.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
                await hrmDefOccupation.RollbackTransactionAsync();
                return false;
            }
            finally
            {
                await Task.Delay(100);
            }

        }
        #endregion



        #region Duplicate Check
        public async Task<bool> IsexistByCodeAsnyc(string code)
        {
            return await hrmDefOccupation.All().AnyAsync(x => x.OccupationCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hrmDefOccupation.All().AnyAsync(x => x.Occupation == name);
        }

        public async Task<bool> IsExistAsync(string name, string code)
        {
            return await hrmDefOccupation.All().AnyAsync(x => x.Occupation == name && x.OccupationCode != code);
        }
        #endregion


        #region Drop Selection
        public async Task<IEnumerable<CommonSelectModel>> SelectOccupationAsnyc()
        {
            var data = await hrmDefOccupation.All().Select(x => new CommonSelectModel
            {
                Code = x.OccupationCode,
                Name = x.Occupation,
            }).ToListAsync();

            return data;
        }

        #endregion


        #region Permisson
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Occupation" && x.TitleCheck);
        }

        public async Task<bool> SavePaermissonAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Occupation" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermisson(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Occupation" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissonAsync(string accesCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accesCode && x.Title == "Occupation" && x.CheckDelete);

        }
        #endregion

    }
}
