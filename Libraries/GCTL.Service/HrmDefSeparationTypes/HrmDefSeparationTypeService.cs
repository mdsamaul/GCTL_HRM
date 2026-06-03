using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefSeparationTypes;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace GCTL.Service.HrmDefSeparationTypes
{
    public class HrmDefSeparationTypeService : AppService<HrmDefSeparationType>, IHrmDefSeparationTypeService
    {
        private readonly IRepository<HrmDefSeparationType> hRmDefSeparationTypeRepository;
        private readonly IRepository<CoreAccessCode> coreAccessCodeRepository;

        public HrmDefSeparationTypeService(IRepository<HrmDefSeparationType> hRmDefSeparationTypeRepository, IRepository<CoreAccessCode> coreAccessCodeRepository) 
 : base(hRmDefSeparationTypeRepository)
        {
            this.hRmDefSeparationTypeRepository = hRmDefSeparationTypeRepository;
            this.coreAccessCodeRepository = coreAccessCodeRepository;
        }

        public async Task<HrmDefSeparationTypeSetupViewModel> GetByIdAsync(string code)
        {
            var entity = await hRmDefSeparationTypeRepository.GetByIdAsync(code);
            if (entity == null)
            {
                return null;
            }
            return new HrmDefSeparationTypeSetupViewModel

            {
                AutoId = entity.SeparationTypeCode, //id
                //AutoId = entity.AutoId,
                SeparationTypeId = entity.SeparationTypeId,
                SeparationType = entity.SeparationType,
                Ldate = entity.Ldate,
                ModifyDate = entity.ModifyDate,
                Luser = entity.Luser,
                Lip = entity.Lip,
                Lmac = entity.Lmac
            };
        }
        public async Task<List<HrmDefSeparationTypeSetupViewModel>> GetAllAsync()
        {
            var entity = await hRmDefSeparationTypeRepository.GetAllAsync();
            return entity.Select(entityVM => new HrmDefSeparationTypeSetupViewModel
            {
                AutoId = entityVM.SeparationTypeCode,
                //AutoId = entityVM.SeparationTypeCode,
                SeparationTypeId = entityVM.SeparationTypeId,
                SeparationType = entityVM.SeparationType,
                Ldate = entityVM.Ldate,
                ModifyDate = entityVM.ModifyDate,
                Luser = entityVM.Luser,
                Lip = entityVM.Lip,
                Lmac = entityVM.Lmac

            }).ToList();
        }


        public async Task<bool> SaveAsync(HrmDefSeparationTypeSetupViewModel entityVM)
        {
            await hRmDefSeparationTypeRepository.BeginTransactionAsync();
            try
            {

                HrmDefSeparationType entity = new HrmDefSeparationType();
                
                entity.SeparationTypeId = await GenerateNextCode();
                entity.SeparationType = entityVM.SeparationType;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await hRmDefSeparationTypeRepository.AddAsync(entity);
                await hRmDefSeparationTypeRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await hRmDefSeparationTypeRepository.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<bool> UpdateAsync(HrmDefSeparationTypeSetupViewModel entityVM)
        {
            await hRmDefSeparationTypeRepository.BeginTransactionAsync();
            try
            {

                var entity = await hRmDefSeparationTypeRepository.GetByIdAsync(entityVM.SeparationTypeId);
                if (entity == null)
                {
                    await hRmDefSeparationTypeRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.SeparationTypeCode = entityVM.AutoId;
                entity.SeparationTypeId = entityVM.SeparationTypeId;
                entity.SeparationType = entityVM.SeparationType ?? string.Empty;
               
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                await hRmDefSeparationTypeRepository.UpdateAsync(entity);
                await hRmDefSeparationTypeRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await hRmDefSeparationTypeRepository.RollbackTransactionAsync();
                return false;
            }
        }

        public bool DeleteLeaveType(string id)
        {
            var entity = GetLeaveType(id);
            if (entity != null)
            {
                hRmDefSeparationTypeRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public HrmDefSeparationType GetLeaveType(string code)
        {
            return hRmDefSeparationTypeRepository.GetById(code);
        }



        public async Task<IEnumerable<CommonSelectModel>> SelectionHrmDefSeparationTypeAsync()
        {
            return await hRmDefSeparationTypeRepository.All()
                      .Select(x => new CommonSelectModel
                      {
                          Code = x.SeparationTypeId,
                          Name = x.SeparationType
                      })
                      .ToListAsync();
        }





        #region NextCode
        public async Task<string> GenerateNextCode()
        {

            var Code = await hRmDefSeparationTypeRepository.GetAllAsync();
            var lastCode = Code.Max(b => b.SeparationTypeId);
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
            return await hRmDefSeparationTypeRepository.All().AnyAsync(x => x.SeparationTypeId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hRmDefSeparationTypeRepository.All().AnyAsync(x => x.SeparationType == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode)
        {
            return await hRmDefSeparationTypeRepository.All().AnyAsync(x => x.SeparationType == name && x.SeparationTypeId != typeCode);
        }

        #endregion

        #region Permission all type

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Separation Type" && x.CheckDelete);
        }

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Separation Type" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Separation Type" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Separation Type" && x.CheckEdit);

        }
        #endregion
    }

}
