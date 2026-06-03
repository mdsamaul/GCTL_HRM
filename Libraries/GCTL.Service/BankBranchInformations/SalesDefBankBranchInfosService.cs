using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmEmployees2;
using GCTL.Core.ViewModels.SalesDefBankBranchInfos;
using GCTL.Data.Models;
using GCTL.Service.HrmEmployees2;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.BankBranchInformations
{
   
    public class SalesDefBankBranchInfosService:AppService<SalesDefBankBranchInfo>,ISalesDefBankBranchInfosService
    {
        private readonly IRepository<SalesDefBankBranchInfo> branchBankRepository;
        private readonly IRepository<CoreAccessCode > accessCodeRepository;
        private readonly IRepository<SalesDefBankInfo> bankRepository;
        public SalesDefBankBranchInfosService(
            IRepository<SalesDefBankBranchInfo> branchBankRepository,
            IRepository<SalesDefBankInfo> bankRepository,
            IRepository<CoreAccessCode> accessCodeRepository
            )
    :base(branchBankRepository) 
        {
            this.branchBankRepository = branchBankRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.bankRepository = bankRepository;
        }

        #region GetById
        public async Task<SalesDefBankBranchInfoSetupViewModel> GetByIdAsync(string code)
        {
            var data = await (from branchBank in branchBankRepository.All().AsNoTracking()
                              join bank in bankRepository.All().AsNoTracking() 
                              on branchBank.BankId equals bank.BankId into branchBankJOin
                              from bank in branchBankJOin.DefaultIfEmpty() 
                              where branchBank.BankBranchId==code
                              select new SalesDefBankBranchInfoSetupViewModel
                              {
                                  AutoId= branchBank.AutoId,
                                  BankId=bank.BankId,
                                  BankName=bank.BankName,
                                  BankBranchId=branchBank.BankBranchId,
                                  BankBranchName=branchBank.BankBranchName,
                                  ShortName=branchBank.ShortName,
                                  Address=branchBank.Address,
                                  Swiftcode=branchBank.Swiftcode,
                                  Ldate=branchBank.Ldate,
                                  Phone=branchBank.Phone,
                                  ModifyDate=branchBank.ModifyDate,
                                  Luser=branchBank.Luser,
                                  Lmac=branchBank.Lmac,
                                  Lip=branchBank.Lip,
                              }).FirstOrDefaultAsync();

            return  data;
                              
        }

        public async Task<List<SalesDefBankBranchInfoSetupViewModel>> GetAllAsync()
        {
            var data = await (from branchBank in branchBankRepository.All().AsNoTracking()
                              join bank in bankRepository.All().AsNoTracking()
                              on branchBank.BankId equals bank.BankId into branchBankJOin
                              from bank in branchBankJOin.DefaultIfEmpty()
                              select new SalesDefBankBranchInfoSetupViewModel
                              {
                                  AutoId = branchBank.AutoId,
                                  BankId = bank.BankId,
                                  BankName = bank.BankName,
                                  BankBranchId = branchBank.BankBranchId,
                                  BankBranchName = branchBank.BankBranchName,
                                  ShortName = branchBank.ShortName,
                                  Address = branchBank.Address,
                                  Swiftcode = branchBank.Swiftcode,
                                  Phone=branchBank.Phone,
                                  Ldate = branchBank.Ldate,
                                  ModifyDate = branchBank.ModifyDate,
                                  Luser = branchBank.Luser,
                                  Lmac = branchBank.Lmac,
                                  Lip = branchBank.Lip,

                              }).ToListAsync();
            return data;
        }


        #endregion

        #region NextCode
        public async Task<string> GenearateNextCode()
        {
            var code = await branchBankRepository.GetAllAsync();
            var lastCode = code.Max(x => x.BankBranchId);
            int nextCode = 1;
            if(!string.IsNullOrEmpty(lastCode))
            {
                int lastNumber = int.Parse(lastCode.TrimStart('0'));
                lastNumber++;
                nextCode = lastNumber;
            }
            return nextCode.ToString("D2");
        }
        #endregion

        #region Permission
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Bank Branch Information" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Bank Branch Information" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Bank Branch Information" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Bank Branch Information" && x.CheckDelete);
        }


        #endregion

        #region Save Update 
        public async Task<bool> SaveAsync(SalesDefBankBranchInfoSetupViewModel entityVM)
        {
         await branchBankRepository.BeginTransactionAsync();
            try
            {
                SalesDefBankBranchInfo entity = new SalesDefBankBranchInfo();
                entity.BankBranchId = await GenearateNextCode();
                entity.BankBranchName = entityVM.BankBranchName;
                entity.BankId = entityVM.BankId;
                entity.Address = entityVM.Address;
                entity.ShortName = entityVM.ShortName;
                entity.Swiftcode = entityVM.Swiftcode;
                entity.Phone = entityVM.Phone;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.Ldate = DateTime.Now;
                await branchBankRepository.AddAsync(entity);
                await branchBankRepository.CommitTransactionAsync();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
                await branchBankRepository.RollbackTransactionAsync();
                return false;
            }
                

        }

        public async Task<bool> UpdateAsync(SalesDefBankBranchInfoSetupViewModel entityVM)
        {
            await branchBankRepository.BeginTransactionAsync();
            try
            {
                var entity = await branchBankRepository.GetByIdAsync(entityVM.BankBranchId);
                if(entity==null)
                {
                    await branchBankRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.BankBranchId = entityVM.BankBranchId;
                entity.BankBranchName = entityVM.BankBranchName;
                entity.BankId = entityVM.BankId;
                entity.Address = entityVM.Address;
                entity.ShortName = entityVM.ShortName;
                entity.Swiftcode = entityVM.Swiftcode;
                entity.Phone = entityVM.Phone;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await branchBankRepository.UpdateAsync(entity);
                await branchBankRepository.CommitTransactionAsync();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
                await branchBankRepository.RollbackTransactionAsync();
                return false;
            }
        }
        #endregion

        #region Delelete
        public bool DeleteLeaveType(string id)
        {

            var entity = GetLeaveType(id);
          
            if (entity != null)
            {
                // Delete_Photo(id).Wait();
                branchBankRepository.Delete(entity);
                return true;
            }
            return false;
        }

      

        public SalesDefBankBranchInfo GetLeaveType(string code)
        {
            return branchBankRepository.GetById(code);
        }

        #endregion
        public IEnumerable<CommonSelectModel> BankBranchDropSelectionAsync()
        {
            var data= branchBankRepository.All()
                      .Select(x => new CommonSelectModel
                      {
                          Code = x.BankBranchId,
                          Name = x.BankBranchName,
                          Address = x.Address

                      });
            return data;
        }
        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await branchBankRepository.All().AnyAsync(x => x.BankBranchId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await branchBankRepository.All().AnyAsync(x => x.BankBranchName == name);
        }

        public async Task<bool> IsExistAsync(string name, string typeCode, string bankId)
        {
            return await branchBankRepository.All().AnyAsync(x => x.BankBranchName == name &&  x.BankId==bankId  && x.BankBranchId != typeCode);
        }

      

      

        #endregion


    }
}
