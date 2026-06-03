using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefBankAndNomineeInfos;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace GCTL.Service.HrmDefBankAndNomineeInfos
{
    public class HrmDefBankAndNomineeInfosService : AppService<HrmDefBankAndNomineeInfo>, IHrmDefBankAndNomineeInfosService
    {
        private readonly IRepository<HrmDefBankAndNomineeInfo> hrmEmpNomiee;
        private readonly IRepository<CoreBranch> branchTypeInfoService;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmEmployee> hrmEmp;
        private readonly IRepository<CoreCompany> coreComapny;
        private readonly IRepository<HrmNomineePhoto> nomineePhoto;
        private readonly IRepository<HrmNomineeSignature> nomineeSignature;
        private readonly IRepository<SalesDefBankInfo> bank;
        private readonly IRepository<SalesDefBankBranchInfo> branchBank;
        private readonly IRepository<HrmDefRelationship> relation;
        string strMaxNO = string.Empty;
        private const string TableName = "HRM_Def_BankAndNomineeInfo";
        private const string ColumnName = "BankAndNomineeId";
        public HrmDefBankAndNomineeInfosService(IRepository<HrmDefBankAndNomineeInfo> hrmEmpNomiee, IRepository<HrmNomineeSignature> nomineeSignature, IRepository<HrmNomineePhoto> nomineePhoto, IRepository<CoreBranch> branchTypeInfoService, ICommonService commonService, IRepository<CoreAccessCode> accessCodeRepository, IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo, IRepository<HrmDefDepartment> departmentRepository, IRepository<HrmDefDesignation> designationRepository, IRepository<HrmEmployee> hrmEmp, IRepository<CoreCompany> coreComapny, IRepository<SalesDefBankInfo> bank, IRepository<SalesDefBankBranchInfo> branchBank, IRepository<HrmDefRelationship> relation) : base(hrmEmpNomiee)
        {
            this.hrmEmpNomiee = hrmEmpNomiee;
            this.branchTypeInfoService = branchTypeInfoService;
            this.commonService = commonService;
            this.accessCodeRepository = accessCodeRepository;
            this.hrmEmpOffialInfo = hrmEmpOffialInfo;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.hrmEmp = hrmEmp;
            this.coreComapny = coreComapny;
            this.nomineePhoto = nomineePhoto;
            this.nomineeSignature = nomineeSignature;
            this.bank = bank;
            this.branchBank = branchBank;
            this.relation = relation;
        }

        public async Task<List<HrmDefBankAndNomineeInfosSetupViewModel>> GetAllAsync(string employeeId)
        {
            try
            {
                // Query to fetch employee document info with related data
                var data = await (from empnom in hrmEmpNomiee.All().AsNoTracking()
                                  where empnom.EmployeeId == employeeId

                                  join b in bank.All().AsNoTracking()
                                  on empnom.BankId equals b.BankId into empBankJoin
                                  from b in empBankJoin.DefaultIfEmpty()

                                  join br in branchBank.All().AsNoTracking()
                                  on empnom.BankBranchId equals br.BankBranchId into empBankBranchJoin
                                  from br in empBankBranchJoin.DefaultIfEmpty()

                                  join r in relation.All().AsNoTracking()
                                  on empnom.Relation equals r.RelationshipCode into empRelationJoin
                                  from r in empRelationJoin.DefaultIfEmpty()

                                  select new HrmDefBankAndNomineeInfosSetupViewModel
                                  {
                                      BankAndNomineeId = empnom.BankAndNomineeId,
                                      EmployeeId = empnom.EmployeeId,
                                      BankId = empnom.BankId,
                                      BankBranchId = empnom.BankBranchId,
                                      // BranchAddress = empDocu.DocumentType,
                                      BankAccountName = empnom.BankAccountName,
                                      BankAccountNo = empnom.BankAccountNo,
                                      AtmcardNo = empnom.AtmcardNo,
                                      NomineeName = empnom.NomineeName,
                                      Relation = empnom.Relation,
                                      PresentAddress = empnom.PresentAddress,
                                      ParmanentAddress = empnom.ParmanentAddress,
                                      BankName = b.BankName,
                                      BankBranchName = br.BankBranchName,
                                      RelationName = r.Relationship

                                      // NomineeSignatureUrl = signature != null ? $"data:{signature.ImgType};base64,{Convert.ToBase64String(signature.Photo)}" : "/images/signature.jpg",
                                      //NomineePhotoUrl= photo != null ? $"data:{photo.ImgType};base64,{Convert.ToBase64String(photo.Photo)}" : "/images/0001.jpg"
                                  }).ToListAsync();

                return data;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return new List<HrmDefBankAndNomineeInfosSetupViewModel>();
            }
        }


        public async Task<HrmDefBankAndNomineeInfosSetupViewModel> GetByIdAsync(string code)
        {
            try
            {
                var data = await (from empNomiee in hrmEmpNomiee.All().AsNoTracking()

                                      //join bra in branchTypeInfoService.All().AsNoTracking()
                                      //on empNomiee.BranchCode equals bra.BranchCode into empDocuBraJoin    // correction 
                                      //from bra in empDocuBraJoin.DefaultIfEmpty()
                                  where empNomiee.BankAndNomineeId == code

                                  join b in bank.All().AsNoTracking()
                                     on empNomiee.BankId equals b.BankId into empBankJoin
                                  from b in empBankJoin.DefaultIfEmpty()

                                  join br in branchBank.All().AsNoTracking()
                                  on empNomiee.BankBranchId equals br.BankBranchId into empBankBranchJoin
                                  from br in empBankBranchJoin.DefaultIfEmpty()

                                  join ofEmp in hrmEmpOffialInfo.All().AsNoTracking()
                                  on empNomiee.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                                  from ofEmp in eduOffJoin.DefaultIfEmpty()

                                  join desi in designationRepository.All().AsNoTracking()
                                  on ofEmp.DesignationCode equals desi.DesignationCode into edudesiJoin
                                  from desi in edudesiJoin.DefaultIfEmpty()

                                  join dept in departmentRepository.All().AsNoTracking()
                                  on ofEmp.DepartmentCode equals dept.DepartmentCode into eduDeptJoin
                                  from dept in eduDeptJoin.DefaultIfEmpty()

                                  join emp in hrmEmp.All().AsNoTracking()
                                  on empNomiee.EmployeeId equals emp.EmployeeId into empDocuEmpJoin
                                  from emp in empDocuEmpJoin.DefaultIfEmpty()


                                  join photo in nomineePhoto.All().AsNoTracking()
                                  on empNomiee.BankAndNomineeId equals photo.NomineeId into empPhoto
                                  from photo in empPhoto.DefaultIfEmpty()

                                  join signature in nomineeSignature.All().AsNoTracking()
                                  on empNomiee.BankAndNomineeId equals signature.NomineeId into empSignature
                                  from signature in empSignature.DefaultIfEmpty()

                                  join r in relation.All().AsNoTracking()
                                       on empNomiee.Relation equals r.RelationshipCode into empRelationJoin
                                  from r in empRelationJoin.DefaultIfEmpty()

                                  select new HrmDefBankAndNomineeInfosSetupViewModel
                                  {
                                      AutoId = empNomiee.AutoId,
                                      BankAndNomineeId = empNomiee.BankAndNomineeId,
                                      EmployeeId = empNomiee.EmployeeId,
                                      BankId = empNomiee.BankId,
                                      BankBranchId = empNomiee.BankBranchId,
                                      NomineeName = empNomiee.NomineeName,
                                      PresentAddress = empNomiee.PresentAddress,
                                      ParmanentAddress = empNomiee.ParmanentAddress,
                                      AtmcardNo = empNomiee.AtmcardNo,
                                      CompanyCode = empNomiee.CompanyCode,
                                      BankAccountName = empNomiee.BankAccountName,
                                      BankAccountNo = empNomiee.BankAccountNo,
                                      Relation = empNomiee.Relation,
                                      //BranchCode = empNomiee.BranchCode,
                                      Ldate = empNomiee.Ldate,
                                      ModifyDate = empNomiee.ModifyDate,
                                      DepartmentName = dept.DepartmentName ?? "",
                                      DesignationName = desi.DesignationName ?? " ",
                                      EmployeeName = $"{emp.FirstName} {emp.LastName}" ?? "",
                                      NomineeSignatureUrl = signature != null ? $"data:{signature.ImgType};base64,{Convert.ToBase64String(signature.Photo)}" : "/images/signature.jpg",
                                      NomineePhotoUrl = photo != null ? $"data:{photo.ImgType};base64,{Convert.ToBase64String(photo.Photo)}" : "/images/0001.jpg"
                                  }).FirstOrDefaultAsync();
                var jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine("Message: " + jsonData);

                Console.WriteLine("Message" + data);


                return data;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }


        }


        public async Task<List<HrmDefBankAndNomineeInfosSetupViewModel>> GetComapnyByBranchCode(string companyCode)
        {
            try
            {
                var result = await (
                                    from br in branchTypeInfoService.All().AsNoTracking()
                                    where (br.CompanyCode == companyCode)
                                    select new HrmDefBankAndNomineeInfosSetupViewModel
                                    {

                                        BranchCode = br.BranchCode,
                                        CoreBranchName = br.BranchName
                                    }).ToListAsync();
                return result ?? new List<HrmDefBankAndNomineeInfosSetupViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        //

        public async Task<List<HrmDefBankAndNomineeInfosSetupViewModel>> GetEmployeeByCompanyCode(string companyCode)
        {
            try
            {


                var result = await (from e in hrmEmp.All().AsNoTracking()

                                    where (e.CompanyCode == companyCode)

                                    select new HrmDefBankAndNomineeInfosSetupViewModel
                                    {
                                        EmployeeId = e.EmployeeId,
                                        EmployeeName = string.Format("{0} {1} ({2})", e.FirstName, e.LastName, e.EmployeeId),
                                    }).ToListAsync();



                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }
        public async Task<HrmDefBankAndNomineeInfosSetupViewModel> GetEmployeeNameDesDeptByCode(string employeeId)
        {
            try
            {

                var result = await (

                                    from e in hrmEmp.All().AsNoTracking()

                                    join oi in hrmEmpOffialInfo.All()
                                        .Select(x => new { x.EmployeeId, x.DepartmentCode, x.DesignationCode })
                                        .AsNoTracking() on e.EmployeeId equals oi.EmployeeId into oiGroup
                                    from oi in oiGroup.DefaultIfEmpty()

                                    join des in designationRepository.All()
                                        .Select(x => new { x.DesignationCode, x.DesignationName })
                                        .AsNoTracking() on oi.DesignationCode equals des.DesignationCode into desGroup
                                    from des in desGroup.DefaultIfEmpty()

                                    join dep in departmentRepository.All()
                                        .Select(x => new { x.DepartmentCode, x.DepartmentName })
                                        .AsNoTracking() on oi.DepartmentCode equals dep.DepartmentCode into depGroup
                                    from dep in depGroup.DefaultIfEmpty()

                                    where e.EmployeeId == employeeId

                                    select new HrmDefBankAndNomineeInfosSetupViewModel
                                    {
                                        EmployeeId = e.EmployeeId,
                                        EmployeeName = $"{e.FirstName} {e.LastName}" ?? " ",
                                        DesignationName = des.DesignationName ?? "",
                                        DepartmentName = dep.DepartmentName ?? "",


                                    }).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }


        }

        //


        public async Task<bool> SaveAsync(HrmDefBankAndNomineeInfosSetupViewModel entityVM, string CompanyCode)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 2);
            await hrmEmpNomiee.BeginTransactionAsync();
            try
            {

                var entity = new HrmDefBankAndNomineeInfo();
                entity.BankAndNomineeId = strMaxNO;



                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                //entity.BranchCode = entityVM.BranchCode ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.BankId = entityVM.BankId ?? string.Empty;
                entity.BankBranchId = entityVM.BankBranchId ?? string.Empty;
                entity.BankAccountName = entityVM.BankAccountName ?? string.Empty;
                entity.BankAccountNo = entityVM.BankAccountNo ?? string.Empty;
                entity.NomineeName = entityVM.NomineeName ?? string.Empty;
                entity.Relation = entityVM.Relation ?? string.Empty;
                entity.PresentAddress = entityVM.PresentAddress ?? string.Empty;
                entity.ParmanentAddress = entityVM.ParmanentAddress ?? string.Empty;
                entity.AtmcardNo = entityVM.AtmcardNo ?? string.Empty;
                entity.EmployeeId2 = entityVM.EmployeeId2 ?? string.Empty;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                entity.CompanyCode = CompanyCode;

                await hrmEmpNomiee.AddAsync(entity);
                //
                if (entityVM.NomineePhoto != null && entityVM.NomineePhoto.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {

                        await entityVM.NomineePhoto.CopyToAsync(memoryStream);


                        HrmNomineePhoto photo = new HrmNomineePhoto
                        {
                            EmployeeId = entity.EmployeeId,
                            NomineeId = entity.BankAndNomineeId,
                            Photo = memoryStream.ToArray(),
                            ImgType = entityVM.NomineePhoto.ContentType,
                            ImgSize = entityVM.NomineePhoto.Length
                        };

                        // Add photo to the database
                        await nomineePhoto.AddAsync(photo);
                    }
                }

                if (entityVM.NomineeSignature != null && entityVM.NomineeSignature.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {

                        await entityVM.NomineeSignature.CopyToAsync(memoryStream);


                        HrmNomineeSignature photor = new HrmNomineeSignature
                        {
                            EmployeeId = entity.EmployeeId,
                            NomineeId = entity.BankAndNomineeId,
                            Photo = memoryStream.ToArray(),
                            ImgType = entityVM.NomineeSignature.ContentType,
                            ImgSize = entityVM.NomineeSignature.Length
                        };

                        // Add photo to the database
                        await nomineeSignature.AddAsync(photor);
                    }
                }
                //
                await hrmEmpNomiee.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                await hrmEmpNomiee.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> UpdateAsync(HrmDefBankAndNomineeInfosSetupViewModel entityVM)
        {
            await hrmEmpNomiee.BeginTransactionAsync();
            var entity = await hrmEmpNomiee.GetByIdAsync(entityVM.BankAndNomineeId);
            if (entity == null)
            {
                await hrmEmpNomiee.RollbackTransactionAsync();
                return false;
            }
            try
            {

                entity.BankAndNomineeId = entityVM.BankAndNomineeId;
                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                // entity.BranchCode = entityVM.BranchCode ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.BankId = entityVM.BankId ?? string.Empty;
                entity.BankBranchId = entityVM.BankBranchId ?? string.Empty;
                entity.BankAccountName = entityVM.BankAccountName ?? string.Empty;
                entity.BankAccountNo = entityVM.BankAccountNo ?? string.Empty;
                entity.NomineeName = entityVM.NomineeName ?? string.Empty;
                entity.Relation = entityVM.Relation ?? string.Empty;
                entity.PresentAddress = entityVM.PresentAddress ?? string.Empty;
                entity.ParmanentAddress = entityVM.ParmanentAddress ?? string.Empty;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.AtmcardNo = entityVM.AtmcardNo ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.EmployeeId2 = entityVM.EmployeeId2 ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                await hrmEmpNomiee.UpdateAsync(entity);

                //
                if (entityVM.IsClearPhoto)
                {
                    var existingPhoto = await nomineePhoto.All().Where(x => x.EmployeeId == entity.EmployeeId).ToListAsync();
                    await nomineePhoto.DeleteRangeAsync(existingPhoto);
                }


                if (entityVM.NomineePhoto != null && entityVM.NomineePhoto.Length > 0)
                {
                    var existingPhotos = await nomineePhoto.All().Where(x => x.EmployeeId == entity.EmployeeId).ToListAsync();
                    if (existingPhotos.Any())
                    {
                        await nomineePhoto.DeleteRangeAsync(existingPhotos);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await entityVM.NomineePhoto.CopyToAsync(memoryStream);

                        HrmNomineePhoto photo = new HrmNomineePhoto
                        {
                            EmployeeId = entity.EmployeeId,
                            NomineeId = entity.BankAndNomineeId,
                            Photo = memoryStream.ToArray(),
                            ImgType = entityVM.NomineePhoto.ContentType,
                            ImgSize = entityVM.NomineePhoto.Length
                        };

                        await nomineePhoto.UpdateAsync(photo);
                    }
                }
                else
                {

                    var existingPhoto = await nomineePhoto.All().Where(x => x.EmployeeId == entity.EmployeeId).FirstOrDefaultAsync();
                    if (existingPhoto != null)
                    {
                        existingPhoto.ImgType = existingPhoto.ImgType;
                        existingPhoto.ImgSize = existingPhoto.ImgSize;
                        await nomineePhoto.UpdateAsync(existingPhoto);
                    }
                }


                if (entityVM.IsClearSignature)
                {
                    var existingSignature = await nomineeSignature.All().Where(x => x.EmployeeId == entity.EmployeeId).ToListAsync();
                    await nomineeSignature.DeleteRangeAsync(existingSignature);

                }
                if (entityVM.NomineeSignature != null && entityVM.NomineeSignature.Length > 0)
                {
                    var existingSignature = nomineeSignature.All().Where(x => x.EmployeeId == entity.EmployeeId).ToList();
                    if (existingSignature.Any())
                    {
                        await nomineeSignature.DeleteRangeAsync(existingSignature);
                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        await entityVM.NomineeSignature.CopyToAsync(memoryStream);

                        HrmNomineeSignature signature = new HrmNomineeSignature
                        {
                            EmployeeId = entity.EmployeeId,
                            NomineeId = entity.BankAndNomineeId,
                            Photo = memoryStream.ToArray(),
                            ImgType = entityVM.NomineeSignature.ContentType,
                            ImgSize = entityVM.NomineeSignature.Length
                        };


                        await nomineeSignature.UpdateAsync(signature);
                    }
                }
                else
                {

                    var existingSignature = await nomineeSignature.All().Where(x => x.EmployeeId == entity.EmployeeId).FirstOrDefaultAsync();
                    if (existingSignature != null)
                    {
                        existingSignature.ImgType = existingSignature.ImgType;
                        existingSignature.ImgSize = existingSignature.ImgSize;
                        await nomineeSignature.UpdateAsync(existingSignature);
                    }
                }
                //
                await hrmEmpNomiee.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                await hrmEmpNomiee.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await hrmEmpNomiee.All().Where(x => ids.Contains(x.BankAndNomineeId)).ToListAsync();
            var photos = nomineePhoto.All().Where(p => ids.Contains(p.NomineeId)).ToList();
            foreach (var photo in photos)
            {
                nomineePhoto.Delete(photo);
            }

            var sinature = nomineeSignature.All().Where(s => ids.Contains(s.NomineeId)).ToList();
            foreach (var sig in sinature)
            {
                nomineeSignature.Delete(sig);
            }
            if (!entity.Any())
            {
                return false;
            }

            hrmEmpNomiee.Delete(entity);

            return true;
        }


        #region Duplicate Check 

        public async Task<bool> IsExistAsync(string bankCode, string branchBankCode, string acName, string acNO, string nomineeName, string code)
        {
            return await hrmEmpNomiee.All().AnyAsync(x => x.BankId == bankCode && x.BankBranchId == branchBankCode && x.BankAccountName == acName && x.BankAccountNo == acNO && x.NomineeName == nomineeName && x.BankAndNomineeId != code);
        }



        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Document Upload Info Entry" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Document Upload Info Entry" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Document Upload Info Entry" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Document Upload Info Entry" && x.CheckDelete);
        }
        #endregion

        public IEnumerable<CommonSelectModel> SelectionHrmDefEmpDocumentTypeAsync()
        {
            throw new NotImplementedException();
        }



    }
}
