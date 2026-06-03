using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HolidayTypes;
using GCTL.Core.ViewModels.HRMCompanyWeekEnds;
using GCTL.Data.Models;
using GCTL.Service.HRMCompanyWeekEnds;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HolidayTypes
{
    

    public class HrmDefHolidayTypeService : AppService<HrmDefHolidayType>, IHrmDefHolidayTypeService
    {
        private readonly IRepository<HrmDefHolidayType> hRmHolidayTypeRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public HrmDefHolidayTypeService(IRepository<HrmDefHolidayType> hRmHolidayTypeRepository, IRepository<CoreAccessCode> accessCodeRepository)
    : base(hRmHolidayTypeRepository)
        {
            this.hRmHolidayTypeRepository = hRmHolidayTypeRepository;
            this.accessCodeRepository = accessCodeRepository;
        }


        public async Task<HRMDefHolidayTypeSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await hRmHolidayTypeRepository.GetByIdAsync(code);
            if (entity == null)
            {
                return null;
            }
            return new HRMDefHolidayTypeSetupViewModel

            {
                AutoId = entity.AutoId, //id
                HolidayTypeCode = entity.HolidayTypeCode,
                HolidayType = entity.HolidayType,
                ShortName=entity.ShortName,
                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Luser = entity.Luser,
                Lip = entity.Lip,
                Lmac = entity.Lmac
            };
        }
        public async Task<List<HRMDefHolidayTypeSetupViewModel>> GetAllAsync()
        {
            var entity = await hRmHolidayTypeRepository.GetAllAsync();
            return entity.Select(entityVM => new HRMDefHolidayTypeSetupViewModel
            {
                AutoId = entityVM.AutoId,
                HolidayTypeCode = entityVM.HolidayTypeCode,
                HolidayType = entityVM.HolidayType,
                ShortName=entityVM.ShortName,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac

            }).ToList();
        }


        public async Task<bool> SaveAsync(HRMDefHolidayTypeSetupViewModel entityVM)
        {
            await hRmHolidayTypeRepository.BeginTransactionAsync();
            try
            {

                HrmDefHolidayType entity = new HrmDefHolidayType();
                entity.HolidayTypeCode = await GenerateNextCode();
                entity.HolidayType = entityVM.HolidayType;
                entity.ShortName = entityVM.ShortName ?? string.Empty;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await hRmHolidayTypeRepository.AddAsync(entity);
                await hRmHolidayTypeRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await hRmHolidayTypeRepository.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<bool> UpdateAsync(HRMDefHolidayTypeSetupViewModel entityVM)
        {
            await hRmHolidayTypeRepository.BeginTransactionAsync();
            try
            {

                var entity = await hRmHolidayTypeRepository.GetByIdAsync(entityVM.HolidayTypeCode);
                if (entity == null)
                {
                    await hRmHolidayTypeRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.HolidayTypeCode = entityVM.HolidayTypeCode;
                entity.HolidayType = entityVM.HolidayType ;
                entity.ShortName = entityVM.ShortName ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                await hRmHolidayTypeRepository.UpdateAsync(entity);
                await hRmHolidayTypeRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await hRmHolidayTypeRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public bool DeleteLeaveType(string id)
        {
            var entity = GetLeaveType(id);
            if (entity != null)
            {
                hRmHolidayTypeRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public HrmDefHolidayType GetLeaveType(string code)
        {
            return hRmHolidayTypeRepository.GetById(code);
        }



        public async Task<IEnumerable<CommonSelectModel>> SelectionHrmDefHolidayTypeAsync()
        {
           
           var data=await hRmHolidayTypeRepository.All()
                      .Select(x => new CommonSelectModel
                      {
                          Code = x.HolidayTypeCode,
                          Name = x.HolidayType
                      }).ToListAsync();
            return data;
        }





        #region NextCode
        public async Task<string> GenerateNextCode()
        {

            var Code = await hRmHolidayTypeRepository.GetAllAsync();
            var lastCode = Code.Max(b => b.HolidayTypeCode);
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
            return await hRmHolidayTypeRepository.All().AnyAsync(x => x.HolidayTypeCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hRmHolidayTypeRepository.All().AnyAsync(x => x.HolidayType == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await hRmHolidayTypeRepository.All().AnyAsync(x => x.HolidayType == name && x.HolidayTypeCode != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Holiday Type" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Holiday Type" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Holiday Type" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Holiday Type" && x.CheckDelete);
        }
        #endregion
    }

}
