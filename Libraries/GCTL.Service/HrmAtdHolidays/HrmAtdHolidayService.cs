using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmAtdHolidays;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GCTL.Service.HrmAtdHolidays
{
    public class HrmAtdHolidayService : AppService<HrmAtdHoliday>, IHrmAtdHolidayService
    {
        private readonly IRepository<HrmAtdHoliday> hRmHolidayRepository;
        private readonly IRepository<HrmDefHolidayType> hRmdefHolidayTypeRepository;
        private readonly IRepository<HrmDefHolidayType> hrmDefHolidayTypeRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public HrmAtdHolidayService(IRepository<HrmAtdHoliday> hRmHolidayRepository, IRepository<HrmDefHolidayType> hRmdefHolidayTypeRepository, IRepository<HrmDefHolidayType> HrmDefHolidayTypeRepository, IRepository<CoreAccessCode> accessCodeRepository)
    : base(hRmHolidayRepository)
        {
            this.hRmHolidayRepository = hRmHolidayRepository;
            this.hRmdefHolidayTypeRepository = hRmdefHolidayTypeRepository;
            hrmDefHolidayTypeRepository = HrmDefHolidayTypeRepository;
            this.accessCodeRepository = accessCodeRepository;
        }


        public async Task<HrmAtdHolidaySetupViewModel> GetByIdAsync(string code)
        {
            var entity = await hRmHolidayRepository.All()
                .Include(c => c.HolidayTypeCode)
                .FirstOrDefaultAsync(c => c.HolidayCode == code);

            if (entity == null)
            {
                return null;
            }


            return new HrmAtdHolidaySetupViewModel
            {
                AutoId = entity.AutoId,
                HolidayCode = entity.HolidayCode,
                HolidayName = entity.HolidayName,
                HolidayTypeCode = entity.HolidayTypeCode,
                FromDate = entity.FromDate,
                ToDate = entity.ToDate,
                NoOfDays = entity.NoOfDays,
                Luser = entity.Luser,
                Lip = entity.Lip,
                Lmac = entity.Lmac,
                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate
            };
        }




        //public async Task<List<HrmAtdHolidaySetupViewModel>> GetAllAsync()
        //{
        //    try
        //    {
        //        return await hRmHolidayRepository.All()
        //       .Include(c => c.HolidayTypeCode)
        //       .Select(c => new HrmAtdHolidaySetupViewModel
        //       {
        //           AutoId = c.AutoId,
        //           HolidayCode = c.HolidayCode,
        //           HolidayName = c.HolidayName,
        //           HolidayTypeCode = c.HolidayTypeCode,
        //           HolidayTypeName = hrmDefHolidayTypeRepository.All().Where(x => x.HolidayTypeCode == c.HolidayTypeCode).Select(s => s.HolidayType).FirstOrDefault() ?? "",
        //           FromDate = c.FromDate,
        //           ToDate = c.ToDate,
        //           NoOfDays = c.NoOfDays,
        //           Ldate = c.Ldate,
        //           ModifyDate = c.ModifyDate,
        //           Luser = c.Luser,
        //           Lip = c.Lip,
        //           Lmac = c.Lmac
        //       })
        //       .ToListAsync();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}


        public async Task<List<HrmAtdHolidaySetupViewModel>> GetAllAsync()
        {
            try
            {
                var result = await (
                    from h in hRmHolidayRepository.All()
                    join t in hrmDefHolidayTypeRepository.All()
                        on h.HolidayTypeCode equals t.HolidayTypeCode into ht
                    from t in ht.DefaultIfEmpty()
                    select new HrmAtdHolidaySetupViewModel
                    {
                        AutoId = h.AutoId,
                        HolidayCode = h.HolidayCode,
                        HolidayName = h.HolidayName,
                        HolidayTypeCode = h.HolidayTypeCode,
                        HolidayTypeName = t != null ? t.HolidayType : "",
                        FromDate = h.FromDate,
                        ToDate = h.ToDate,
                        NoOfDays = h.NoOfDays,
                        Ldate = h.Ldate,
                        ModifyDate = h.ModifyDate,
                        Luser = h.Luser,
                        Lip = h.Lip,
                        Lmac = h.Lmac
                    }
                ).ToListAsync();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<bool> SaveAsync(HrmAtdHolidaySetupViewModel entityVM)
        {
            await hRmHolidayRepository.BeginTransactionAsync();
            try
            {
                //
                var duplicateHolidays = await hRmHolidayRepository.FindByAsync(h =>
                               h.FromDate >= entityVM.FromDate &&
                                h.FromDate <= entityVM.ToDate);


                // Delete all duplicates found within the range
                if (duplicateHolidays != null && duplicateHolidays.Any())
                {
                    foreach (var holiday in duplicateHolidays)
                    {
                        await hRmHolidayRepository.DeleteAsync(holiday);
                    }
                }


                //
                DateTime currentDate = entityVM.FromDate;
                int noOfDays = (entityVM.ToDate - entityVM.FromDate).Days;

                for (int i = 0; i <= noOfDays; i++)
                {

                    HrmAtdHoliday entity = new HrmAtdHoliday();
                    {
                        entity.HolidayCode = await GenerateNextCode();
                        entity.HolidayTypeCode = entityVM.HolidayTypeCode;
                        entity.HolidayName = entityVM.HolidayName;
                        entity.FromDate = currentDate;
                        entity.ToDate = currentDate;
                        entity.NoOfDays = 1;
                        entity.Luser = entityVM.Luser ?? string.Empty;
                        entity.Lip = entityVM.Lip ?? string.Empty;
                        entity.Lmac = entityVM.Lmac ?? string.Empty;
                        entity.Ldate = DateTime.Now;
                    }
                    ;

                    await hRmHolidayRepository.AddAsync(entity);
                    currentDate = currentDate.AddDays(1);
                }

                await hRmHolidayRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await hRmHolidayRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> UpdateAsync(HrmAtdHolidaySetupViewModel entityVM)
        {
            await hRmHolidayRepository.BeginTransactionAsync();
            try
            {

                var entity = await hRmHolidayRepository.GetByIdAsync(entityVM.HolidayCode);
                if (entity == null)
                {
                    await hRmHolidayRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.HolidayCode = entityVM.HolidayCode;
                entity.HolidayTypeCode = entityVM.HolidayTypeCode;
                entity.HolidayName = entityVM.HolidayName;
                entity.FromDate = entityVM.FromDate;
                entity.ToDate = entityVM.ToDate;
                entity.NoOfDays = entityVM.NoOfDays;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.ModifyDate = DateTime.Now;

                await hRmHolidayRepository.UpdateAsync(entity);
                await hRmHolidayRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await hRmHolidayRepository.RollbackTransactionAsync();
                return false;
            }
        }





        public IEnumerable<CommonSelectModel> DropSelection()
        {
            return hRmdefHolidayTypeRepository.All().Select(x => new CommonSelectModel
            {
                Code = x.HolidayTypeCode,
                Name = x.HolidayType
            });
        }

        public bool DeleteLeaveType(string id)
        {
            var entity = GetLeaveType(id);
            if (entity != null)
            {
                hRmHolidayRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public HrmAtdHoliday GetLeaveType(string code)
        {
            return hRmHolidayRepository.GetById(code);
        }

        public async Task<string> GenerateNextCode()
        {

            var Code = await hRmHolidayRepository.GetAllAsync();
            var lastCode = Code.Max(b => b.HolidayCode);
            int nextCode = 1;
            if (!string.IsNullOrEmpty(lastCode))
            {
                int lastNumber = int.Parse(lastCode.TrimStart('0'));
                lastNumber++;
                nextCode = lastNumber;
            }
            return nextCode.ToString("D2");

        }


        public async Task<IEnumerable<CommonSelectModel>> SelectionHrmAtdHoliday()
        {
            return await hRmHolidayRepository.All()
                      .Select(x => new CommonSelectModel
                      {
                          Code = x.HolidayCode,
                          Name = x.HolidayName
                      })
                      .ToListAsync();
        }

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await hRmHolidayRepository.All().AnyAsync(x => x.HolidayCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hRmHolidayRepository.All().AnyAsync(x => x.HolidayName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await hRmHolidayRepository.All().AnyAsync(x => x.HolidayName == name && x.HolidayCode != typeCode);
        }

        //public async Task<bool> IsExistAsync(string name, string code, string holidayTypeCode, DateTime? fromDate)
        //{
        //    Console.WriteLine($"Checking for duplicate: {fromDate}");

        //    return await hRmHolidayRepository.All().AnyAsync(x =>
        //        x.HolidayName == name && x.HolidayCode != code && x.HolidayTypeCode == holidayTypeCode && x.FromDate==fromDate);
        //}


        public async Task<bool> IsExistAsync(string name, string code, string holidayTypeCode, string fromDate, string toDate)
        {

            DateTime? parsedFromDate = null;
            if (DateTime.TryParseExact(fromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                // Use .Date to ignore the time part
                parsedFromDate = result.Date;
            }

            DateTime? parsedToDate = null;
            if (DateTime.TryParseExact(toDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var resultt))
            {
                // Use .Date to ignore the time part
                parsedToDate = resultt.Date;
            }

            return await hRmHolidayRepository.All().AnyAsync(x =>

                x.HolidayCode != code &&
                x.HolidayName == name &&
                x.HolidayTypeCode == holidayTypeCode &&
                x.FromDate.Date == parsedFromDate &&
                x.ToDate.Date == parsedToDate);
        }




        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Holiday Information" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Holiday Information" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Holiday Information" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Holiday Information" && x.CheckDelete);
        }
        #endregion

    }
}





//public async Task<HrmAtdHolidaySetupViewModel> GetByIdAsync(string code)
//{
//    var entity = await hRmHolidayRepository.GetByIdAsync(code);
//    if (entity == null)
//    {
//        return null;
//    }
//    return new HrmAtdHolidaySetupViewModel

//    {
//        AutoId = entity.AutoId, //id
//        HolidayTypeCode = entity.HolidayTypeCode,
//        HolidayName = entity.HolidayName,
//        HolidayTypeName = entity.HolidayTypeCodeNavigation.HolidayType,
//        FromDate = entity.FromDate,
//        ToDate = entity.ToDate,
//        Ldate = entity.Ldate,
//        NoOfDays = entity.NoOfDays,
//        ModifyDate = entity.ModifyDate,
//        Luser = entity.Luser,
//        Lip = entity.Lip,
//        Lmac = entity.Lmac
//    };
//}


//public async Task<List<HrmAtdHolidaySetupViewModel>> GetAllAsync()
//{
//    var entity = await hRmHolidayRepository.GetAllAsync();
//    return entity.Select(entityVM => new HrmAtdHolidaySetupViewModel
//    {
//        AutoId = entityVM.AutoId,
//        HolidayName=entityVM.HolidayName,
//        HolidayCode = entityVM.HolidayCode,
//        HolidayTypeCode = entityVM.HolidayTypeCode,
//        HolidayTypeName = entityVM.HolidayType,
//        FromDate = entityVM.FromDate,
//        ToDate=entityVM.ToDate,
//        NoOfDays=entityVM.NoOfDays,
//        Ldate = entityVM.Ldate,
//        ModifyDate = entityVM.ModifyDate,
//        Luser = entityVM.Luser,
//        Lip = entityVM.Lip,
//        Lmac = entityVM.Lmac

//    }).ToList();
//}



//public async Task<bool> SaveAsync(HrmAtdHolidaySetupViewModel entityVM)
//{
//    await hRmHolidayRepository.BeginTransactionAsync();
//    try
//    {

//        HrmAtdHoliday entity = new HrmAtdHoliday();
//        entity.HolidayCode = await GenerateNextCode();
//        entity.HolidayTypeCode = entityVM.HolidayTypeCode;
//        entity.HolidayName = entityVM.HolidayName;
//        entity.FromDate = entityVM.FromDate;
//        entity.ToDate = entityVM.ToDate;
//        entity.NoOfDays = entityVM.NoOfDays;
//        entity.Luser = entityVM.Luser ?? string.Empty;
//        entity.Lip = entityVM.Lip ?? string.Empty;
//        entity.Lmac = entityVM.Lmac ?? string.Empty;
//        entity.Ldate = DateTime.Now;
//        await hRmHolidayRepository.AddAsync(entity);
//        await hRmHolidayRepository.CommitTransactionAsync();
//        return true;
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"error message {ex.Message}");
//        await hRmHolidayRepository.RollbackTransactionAsync();
//        return false;
//    }
//}