using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmAtdShifts;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.HrmAtdShifts
{
    public class HrmAtdShiftService : AppService<HrmAtdShift>, IHrmAtdShiftService
    {
        private readonly IRepository<HrmAtdShift> hRmAtdShiftRepository;
        private readonly IRepository<HrmDefShiftType> shiftService;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public HrmAtdShiftService(IRepository<HrmAtdShift> hRmAtdShiftRepository, IRepository<HrmDefShiftType> shiftService, IRepository<CoreAccessCode> accessCodeRepository)
    : base(hRmAtdShiftRepository)
        {
            this.hRmAtdShiftRepository = hRmAtdShiftRepository;
            this.shiftService = shiftService;
            this.accessCodeRepository = accessCodeRepository;
        }


        public async Task<HrmAtdShiftSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await hRmAtdShiftRepository.GetByIdAsync(code);
            if (entity == null)
            {
                return null;
            }
            return new HrmAtdShiftSetupViewModel

            {
                AutoId = entity.AutoId, //id
                ShiftCode = entity.ShiftCode,
                ShiftName = entity.ShiftName,
                ShiftShortName = entity.ShiftShortName,
                Remarks = entity.Remarks,
                //Description = entity.Description,
                ShiftStartTime = entity.ShiftStartTime,
                ShiftEndTime = entity.ShiftEndTime,
                AbsentTime = entity.AbsentTime,
                LateTime = entity.LateTime,
                ShiftTypeId = entity.ShiftTypeId,
                Wef = entity.Wef,
                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Luser = entity.Luser,
                Lip = entity.Lip,
                Lmac = entity.Lmac,
                LunchInTime = entity.LunchInTime,
                LunchOutTime = entity.LunchOutTime,
                LunchBreakHour = entity.LunchBreakHour
            };
        }
        public async Task<List<HrmAtdShiftSetupViewModel>> GetAllAsync()
        {
            var entity = await hRmAtdShiftRepository.GetAllAsync();
            return entity.Select(entityVM => new HrmAtdShiftSetupViewModel
            {
                AutoId = entityVM.AutoId,
                ShiftCode = entityVM.ShiftCode,
                ShiftTypeId = entityVM.ShiftTypeId,
                ShiftTypeName = shiftService.All().Where(w => w.ShiftTypeId == entityVM.ShiftTypeId).Select(x => x.ShiftTypeName).FirstOrDefault() ?? "",
                ShiftName = entityVM.ShiftName,
                ShiftShortName = entityVM.ShiftShortName,
                Remarks = entityVM.Remarks,
                Description = entityVM.ShiftShortName,
                ShiftStartTime = entityVM.ShiftStartTime,
                ShiftEndTime = entityVM.ShiftEndTime,
                AbsentTime = entityVM.AbsentTime,
                LateTime = entityVM.LateTime,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Wef = entityVM.Wef,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac,
                LunchInTime = entityVM.LunchInTime,
                LunchOutTime = entityVM.LunchOutTime,
                LunchBreakHour = entityVM.LunchBreakHour

            }).ToList();
        }


        public async Task<bool> SaveAsync(HrmAtdShiftSetupViewModel entityVM)
        {
            await hRmAtdShiftRepository.BeginTransactionAsync();
            try
            {
                // Check if shift with same name or shift code already exists
                //string nextShiftCode = await GenerateNextCode();

                //var existingShift = await hRmAtdShiftRepository
                //    .FindByAsync(x => x.ShiftCode == nextShiftCode);

                //if (existingShift != null)
                //{
                //    // ShiftCode already used
                //    throw new InvalidOperationException($"ShiftCode '{nextShiftCode}' already exists.");
                //}

                HrmAtdShift entity = new HrmAtdShift
                {
                    ShiftCode = await GenerateNextCode(),
                    ShiftName = entityVM.ShiftName,
                    ShiftShortName = entityVM.Description ?? string.Empty,
                    Remarks = entityVM.Remarks ?? string.Empty,
                    ShiftStartTime = entityVM.ShiftStartTime,
                    ShiftEndTime = entityVM.ShiftEndTime,
                    AbsentTime = entityVM.AbsentTime,
                    LateTime = entityVM.LateTime,
                    Luser = entityVM.Luser ?? string.Empty,
                    Lip = entityVM.Lip ?? string.Empty,
                    Lmac = entityVM.Lmac ?? string.Empty,
                    Wef = entityVM.Wef,
                    ShiftTypeId = entityVM.ShiftTypeId ?? string.Empty,
                    Ldate = DateTime.Now,
                    LunchInTime = entityVM.LunchInTime,
                    LunchOutTime = entityVM.LunchOutTime,
                    LunchBreakHour = entityVM.LunchBreakHour
                };




                await hRmAtdShiftRepository.AddAsync(entity);
                await hRmAtdShiftRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error message: {ex.Message}");
                await hRmAtdShiftRepository.RollbackTransactionAsync();
                return false;
            }
        }




        public async Task<bool> UpdateAsync(HrmAtdShiftSetupViewModel entityVM)
        {
            await hRmAtdShiftRepository.BeginTransactionAsync();
            try
            {

                var entity = await hRmAtdShiftRepository.GetByIdAsync(entityVM.ShiftCode);
                if (entity == null)
                {
                    await hRmAtdShiftRepository.RollbackTransactionAsync();
                    return false;
                }

                entity.ShiftCode = entityVM.ShiftCode;
                entity.ShiftName = entityVM.ShiftName;
                entity.ShiftShortName = entityVM.ShiftShortName ?? string.Empty;
                entity.ShiftShortName = entityVM.Description ?? string.Empty;
                entity.Remarks = entityVM.Remarks ?? string.Empty;
                entity.ShiftStartTime = entityVM.ShiftStartTime;
                entity.ShiftEndTime = entityVM.ShiftEndTime;
                entity.AbsentTime = entityVM.AbsentTime;
                entity.LateTime = entityVM.LateTime;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Wef = entityVM.Wef;
                entity.ShiftTypeId = entityVM.ShiftTypeId ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                entity.LunchInTime = entityVM.LunchInTime;
                entity.LunchOutTime = entityVM.LunchOutTime;
                entity.LunchBreakHour = entityVM.LunchBreakHour;
                await hRmAtdShiftRepository.UpdateAsync(entity);
                await hRmAtdShiftRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await hRmAtdShiftRepository.RollbackTransactionAsync();
                return false;
            }
        }


        public bool DeleteLeaveType(string id)
        {
            var entity = GetLeaveType(id);
            if (entity != null)
            {
                hRmAtdShiftRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public HrmAtdShift GetLeaveType(string code)
        {
            return hRmAtdShiftRepository.GetById(code);
        }



        public async Task<IEnumerable<CommonSelectModel>> SelectionHrmAtdShiftAsync()
        {
            return await hRmAtdShiftRepository.All()
                      .Select(x => new CommonSelectModel
                      {
                          Code = x.ShiftCode,
                          Name = x.ShiftName
                      })
                      .ToListAsync();
        }



        public async Task<string> GenerateNextCode()
        {

            var leaveTypeCode = await hRmAtdShiftRepository.GetAllAsync();
            var lastCode = leaveTypeCode.Max(b => b.ShiftCode);
            int nextCode = 1;
            if (!string.IsNullOrEmpty(lastCode))
            {
                int lastNumber = int.Parse(lastCode.TrimStart('0'));
                lastNumber++;
                nextCode = lastNumber;
            }
            return nextCode.ToString();

        }

        //public async Task<string> GenerateNextCode()
        //{

        //    var customers = await hRmAtdShiftRepository.GetAllAsync();
        //    var lastCode = customers.Max(b => b.ShiftCode);
        //    string nextCode = "SC01";
        //    if (!string.IsNullOrEmpty(lastCode))
        //    {
        //        int lastNumber = int.Parse(lastCode.Substring(2));
        //        lastNumber++;
        //        nextCode = $"SC{lastNumber:D2}";
        //    }

        //    return nextCode;
        //}

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await hRmAtdShiftRepository.All().AnyAsync(x => x.ShiftCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hRmAtdShiftRepository.All().AnyAsync(x => x.ShiftName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await hRmAtdShiftRepository.All().AnyAsync(x => x.ShiftName == name && x.ShiftCode != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Shift Information" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Shift Information" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Shift Information" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Shift Information" && x.CheckDelete);
        }
        #endregion
    }
}
