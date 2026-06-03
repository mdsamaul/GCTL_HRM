using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmAtdShifts;
using GCTL.Core.ViewModels.HRMCompanyWeekEnds;
using GCTL.Data.Models;
using GCTL.Service.HrmAtdShifts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRMCompanyWeekEnds
{
   
    public class HRMCompanyWeekEndService : AppService<HrmAtdCompanyWeekEnd>, IHRMCompanyWeekEndService
    {
        private readonly IRepository<HrmAtdCompanyWeekEnd> hRmComapnyWeekEndRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public HRMCompanyWeekEndService(IRepository<HrmAtdCompanyWeekEnd> hRmComapnyWeekEndRepository, IRepository<CoreAccessCode> accessCodeRepository)
    : base(hRmComapnyWeekEndRepository)
        {
            this.hRmComapnyWeekEndRepository = hRmComapnyWeekEndRepository;
            this.accessCodeRepository = accessCodeRepository;
        }


        public async Task<HRMCompanyWeekEndSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await hRmComapnyWeekEndRepository.GetByIdAsync(code);
            if (entity == null)
            {
                return null;
            }
            return new HRMCompanyWeekEndSetupViewModel

            {
                AutoId = entity.AutoId, //id
                CompanyWeekEndCode = entity.CompanyWeekEndCode,
                Weekend = entity.Weekend?.Split(',').ToList() ?? new List<string>(),
                EffectiveDate = entity.EffectiveDate,
                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Luser = entity.Luser,
                Lip = entity.Lip,
                Lmac = entity.Lmac
            };
        }
        public async Task<List<HRMCompanyWeekEndSetupViewModel>> GetAllAsync()
        {
            var entity = await hRmComapnyWeekEndRepository.GetAllAsync();
            return entity.Select(entityVM => new HRMCompanyWeekEndSetupViewModel
            {
                AutoId = entityVM.AutoId,
                CompanyWeekEndCode = entityVM.CompanyWeekEndCode,
                EffectiveDate = entityVM.EffectiveDate,
                Weekend = entityVM.Weekend?.Split(',').ToList() ?? new List<string>(),
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac

            }).ToList();
        }


        public async Task<bool> SaveAsync(HRMCompanyWeekEndSetupViewModel entityVM)
        {
            await hRmComapnyWeekEndRepository.BeginTransactionAsync();
            try
            {

                HrmAtdCompanyWeekEnd entity = new HrmAtdCompanyWeekEnd();
                entity.CompanyWeekEndCode = await GenerateNextCode();
                entity.Weekend = string.Join(",", entityVM.Weekend);
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                //entity.EffectiveDate = DateTime.ParseExact(entityVM.EffectiveDate.ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                entity.EffectiveDate =entityVM.EffectiveDate;
                entity.Ldate = DateTime.Now;
                await hRmComapnyWeekEndRepository.AddAsync(entity);
                await hRmComapnyWeekEndRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await hRmComapnyWeekEndRepository.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<bool> UpdateAsync(HRMCompanyWeekEndSetupViewModel entityVM)
        {
            await hRmComapnyWeekEndRepository.BeginTransactionAsync();
            try
            {

                var entity = await hRmComapnyWeekEndRepository.GetByIdAsync(entityVM.CompanyWeekEndCode);
                if (entity == null)
                {
                    await hRmComapnyWeekEndRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.CompanyWeekEndCode = entityVM.CompanyWeekEndCode;
                entity.Weekend = string.Join(",", entityVM.Weekend);
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.EffectiveDate = entityVM.EffectiveDate;
                entity.ModifyDate = DateTime.Now;
                await hRmComapnyWeekEndRepository.UpdateAsync(entity);
                await hRmComapnyWeekEndRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await hRmComapnyWeekEndRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public bool DeleteLeaveType(string id)
        {
            var entity = GetLeaveType(id);
            if (entity != null)
            {
                hRmComapnyWeekEndRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public HrmAtdCompanyWeekEnd GetLeaveType(string code)
        {
            return hRmComapnyWeekEndRepository.GetById(code);
        }



        public async Task<IEnumerable<CommonSelectModel>> SelectionHrmAtdShiftAsync()
        {
            return await hRmComapnyWeekEndRepository.All()
                      .Select(x => new CommonSelectModel
                      {
                          Code = x.CompanyWeekEndCode,
                          Name = x.Weekend
                      })
                      .ToListAsync();
        }



        //public async Task<string> GenerateNextCode()
        //{

        //    var leaveTypeCode = await hRmComapnyWeekEndRepository.GetAllAsync();
        //    var lastCode = leaveTypeCode.Max(b => b.CompanyWeekEndCode);
        //    int nextCode = 1;
        //    if (!string.IsNullOrEmpty(lastCode))
        //    {
        //        int lastNumber = int.Parse(lastCode.TrimStart('0'));
        //        lastNumber++;
        //        nextCode = lastNumber;
        //    }
        //    return nextCode.ToString("D2");

        //}


        //public async Task<string> GenerateNextCode()
        //{

        //    var customers = await hRmComapnyWeekEndRepository.GetAllAsync();
        //    var lastCode = customers.Max(b => b.CompanyWeekEndCode);
        //    string nextCode = "WC01";
        //    if (!string.IsNullOrEmpty(lastCode))
        //    {
        //        int lastNumber = int.Parse(lastCode.Substring(2));
        //        lastNumber++;
        //        nextCode = $"WC{lastNumber:D2}";
        //    }

        //    return nextCode;
        //}


        public async Task<string> GenerateNextCode()
        {

            var Code = await hRmComapnyWeekEndRepository.GetAllAsync();
            var lastCode = Code.Max(b => b.CompanyWeekEndCode);
            int nextCode = 1;
            if (!string.IsNullOrEmpty(lastCode))
            {
                int lastNumber = int.Parse(lastCode.TrimStart('0'));
                lastNumber++;
                nextCode = lastNumber;
            }
            return nextCode.ToString("D2");

        }


        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await hRmComapnyWeekEndRepository.All().AnyAsync(x => x.CompanyWeekEndCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hRmComapnyWeekEndRepository.All().AnyAsync(x => x.Weekend == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await hRmComapnyWeekEndRepository.All().AnyAsync(x => x.Weekend == name && x.CompanyWeekEndCode != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Company Weekend Setup" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Company Weekend Setup" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Company Weekend Setup" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Company Weekend Setup" && x.CheckDelete);
        }
        #endregion
    }
}
