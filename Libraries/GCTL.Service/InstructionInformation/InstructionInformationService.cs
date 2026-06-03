//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.Common;
//using GCTL.Core.ViewModels.InstructionInformation;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using Microsoft.EntityFrameworkCore;

//namespace GCTL.Service.InstructionInformation
//{
//    public class InstructionInformationService : AppService<RmgProdDefInstruction>, IInstructionInformationService
//    {
//        #region Service & Repository
//        private readonly IRepository<RmgProdDefInstruction> instructionInformationrepository;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        private readonly ICommonService commonService;

//        string strMaxNO = string.Empty;

//        private const string TableName = "RMG_Prod_Def_Instruction";
//        private const string ColumnName = "InstructionId";
//        public InstructionInformationService(
//            IRepository<RmgProdDefInstruction> instructionInformationrepository,
//             IRepository<CoreAccessCode> accessCodeRepository,
//            ICommonService commonService

//            )

//    : base(instructionInformationrepository)
//        {
//            this.instructionInformationrepository = instructionInformationrepository;
//            this.accessCodeRepository = accessCodeRepository;
//            this.commonService = commonService;
//        }

//        #endregion

//        #region GetAllAsync

//        public async Task<List<InstructionInformationSetupViewModel>> GetAllAsync()
//        {
//            var entity = await instructionInformationrepository.GetAllAsync();
//            return entity.Select(entityVM => new InstructionInformationSetupViewModel
//            {
//                Tc = entityVM.Tc,
//                InstructionId = entityVM.InstructionId,
//                Instruction = entityVM.Instruction,
//                Ldate = entityVM.Ldate,
//                ModifyDate = entityVM.ModifyDate,
//                Luser = entityVM.Luser,
//                Lip = entityVM.Lip,
//                Lmac = entityVM.Lmac,

//            }).ToList();
//        }

//        #endregion

//        #region GetByIdAsync

//        public async Task<InstructionInformationSetupViewModel> GetByIdAsync(string code)
//        {
//            var entity = await instructionInformationrepository.GetByIdAsync(code);
//            if (entity == null) return null;

//            return new InstructionInformationSetupViewModel
//            {
//                Tc = entity.Tc,
//                InstructionId = entity.InstructionId,
//                Instruction = entity.Instruction,
//                Luser = entity.Luser,
//                Ldate = entity.Ldate,
//                ModifyDate = entity.ModifyDate,
//                Lip = entity.Lip,
//                Lmac = entity.Lmac
//            };
//        }

//        #endregion

//        #region SaveAsync

//        public async Task<bool> SaveAsync(InstructionInformationSetupViewModel entityVM)
//        {
//            try
//            {
//                // Generate next code
//                commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);

//                await instructionInformationrepository.BeginTransactionAsync();

//                var entity = new RmgProdDefInstruction
//                {
//                    InstructionId = strMaxNO,
//                    Instruction = entityVM.Instruction?.Trim(),
//                    Luser = entityVM.Luser,
//                    Lip = entityVM.Lip,
//                    Lmac = entityVM.Lmac ?? string.Empty,
//                    Ldate = DateTime.Now
//                };

//                await instructionInformationrepository.AddAsync(entity);
//                await instructionInformationrepository.CommitTransactionAsync();

//                return true;
//            }
//            catch (Exception ex)
//            {
//                await instructionInformationrepository.RollbackTransactionAsync();
//                Console.WriteLine($"Error saving Fabric Test: {ex.Message}");
//                return false;
//            }
//        }

//        #endregion

//        #region UpdateAsync

//        public async Task<bool> UpdateAsync(InstructionInformationSetupViewModel vm)
//        {
//            try
//            {
//                await instructionInformationrepository.BeginTransactionAsync();

//                // Get the existing record
//                var entity = await instructionInformationrepository.GetByIdAsync(vm.InstructionId);
//                if (entity == null)
//                {
//                    await instructionInformationrepository.RollbackTransactionAsync();
//                    Console.WriteLine("Instruction not found for update.");
//                    return false;
//                }

//                // Update fields
//                entity.InstructionId = vm.InstructionId;
//                entity.Instruction = vm.Instruction?.Trim();
//                entity.ModifyDate = DateTime.Now;
//                entity.Luser = vm.Luser;
//                entity.Lip = vm.Lip;
//                entity.Lmac = vm.Lmac ?? string.Empty;

//                await instructionInformationrepository.UpdateAsync(entity);
//                await instructionInformationrepository.CommitTransactionAsync();

//                return true;
//            }
//            catch (Exception ex)
//            {
//                await instructionInformationrepository.RollbackTransactionAsync();
//                Console.WriteLine($"Error updating Instruction Information: {ex.Message}");
//                return false;
//            }
//        }

//        #endregion

//        #region SelectionAsync
//        public async Task<IEnumerable<CommonSelectModel>> SelectionInstructionAsync()
//        {
//            var data = await instructionInformationrepository.All()
//                       .Select(x => new CommonSelectModel
//                       {
//                           Code = x.InstructionId,
//                           Name = x.Instruction,
//                       }).ToListAsync();
//            return data;
//        }

//        #endregion

//        #region DeleteTab
//        public async Task<bool> DeleteTab(List<string> ids)
//        {
//            var entity = await instructionInformationrepository.All().Where(x => ids.Contains(x.InstructionId)).ToListAsync();

//            if (!entity.Any())
//            {
//                return false;
//            }

//            instructionInformationrepository.Delete(entity);

//            return true;
//        }
//        #endregion

//        #region Duplicate Check 
//        public async Task<bool> IsExistByCodeAsync(string code)
//        {
//            return await instructionInformationrepository.All().AnyAsync(x => x.InstructionId == code);
//        }

//        public async Task<bool> IsExistAsync(string name)
//        {
//            return await instructionInformationrepository.All().AnyAsync(x => x.Instruction == name);
//        }

//        public async Task<bool> IsExistAsync(string name, string typeCode)
//        {
//            return await instructionInformationrepository.All().AnyAsync(x => x.Instruction == name && x.InstructionId != typeCode);
//        }

//        #endregion

//        #region Permission all type
//        public async Task<bool> PagePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Instruction Information" && x.TitleCheck);
//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Instruction Information" && x.CheckAdd);
//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Instruction Information" && x.CheckEdit);
//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Instruction Information" && x.CheckDelete);
//        }
//        #endregion
//    }
//}
