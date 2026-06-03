using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.Districts;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.Districts
{
    public class DistrictsService : AppService<HrmDefDistrict>, IDistrictsService
    {
        private readonly IRepository<HrmDefDistrict> districtRepository;
        private readonly IRepository<CoreAccessCode> coreAccessCodeRepository;

        public DistrictsService(IRepository<HrmDefDistrict> districtRepository, IRepository<CoreAccessCode> coreAccessCodeRepository)
  : base(districtRepository)
        {
            this.districtRepository = districtRepository;
            this.coreAccessCodeRepository = coreAccessCodeRepository;
        }

        public async Task<DistrictsSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await districtRepository.GetByIdAsync(code);
            if (entity == null)
            {
                return null;
            }
            return new DistrictsSetupViewModel

            {
                AutoId = entity.AutoId, //id
                DistrictId = entity.DistrictId,
                District = entity.District,

                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Luser = entity.Luser,
                Lip = entity.Lip,
                Lmac = entity.Lmac
            };
        }
        public async Task<List<DistrictsSetupViewModel>> GetAllAsync()
        {
            var entity = await districtRepository.GetAllAsync();
            return entity.Select(entityVM => new DistrictsSetupViewModel
            {
                AutoId = entityVM.AutoId,
                DistrictId = entityVM.DistrictId,
                District = entityVM.District,

                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac

            }).ToList();
        }


        public async Task<bool> SaveAsync(DistrictsSetupViewModel entityVM)
        {
            await districtRepository.BeginTransactionAsync();
            try
            {

                HrmDefDistrict entity = new HrmDefDistrict();

                entity.DistrictId = await GenerateNextCode();
                entity.District = entityVM.District;

                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;

                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                entity.ModifyDate = DateTime.Now;
                await districtRepository.AddAsync(entity);
                await districtRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await districtRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> UpdateAsync(DistrictsSetupViewModel entityVM)
        {
            await districtRepository.BeginTransactionAsync();
            try
            {

                var entity = await districtRepository.GetByIdAsync(entityVM.DistrictId);
                if (entity == null)
                {
                    await districtRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.AutoId = entityVM.AutoId;
                entity.DistrictId = entityVM.DistrictId;
                entity.District = entityVM.District;

                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                await districtRepository.UpdateAsync(entity);
                await districtRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await districtRepository.RollbackTransactionAsync();
                return false;
            }
        }

        public bool DeleteLeaveType(string id)
        {
            var entity = GetLeaveType(id);
            if (entity != null)
            {
                districtRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public HrmDefDistrict GetLeaveType(string code)
        {
            return districtRepository.GetById(code);
        }

        public async Task<IEnumerable<CommonSelectModel>> GetDistrictsSelectionsAsync()
        {
            var data = await districtRepository.All()
                      .Select(x => new CommonSelectModel
                      {
                          Code = x.DistrictId,
                          Name = x.District,
                      })
                      .ToListAsync();
            return data;
        }

        #region NextCode


        public async Task<string> GenerateNextCode()
        {

            var Code = await districtRepository.GetAllAsync();
            var lastCode = Code.Max(b => b.DistrictId);
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
            return await districtRepository.All().AnyAsync(x => x.DistrictId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await districtRepository.All().AnyAsync(x => x.District == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await districtRepository.All().AnyAsync(x => x.District == name && x.DistrictId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "District" && x.CheckDelete);
        }

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "District" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "District" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "District" && x.CheckEdit);

        }
        #endregion
    }
}
