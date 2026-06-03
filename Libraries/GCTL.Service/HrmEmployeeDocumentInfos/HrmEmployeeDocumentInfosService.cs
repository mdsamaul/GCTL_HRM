using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmEmployeeAdditionalInfos;
using GCTL.Core.ViewModels.HrmEmployeeDocumentInfos;
using GCTL.Core.ViewModels.HrmEmployeeFamilys;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
//
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
//

namespace GCTL.Service.HrmEmployeeDocumentInfos
{
    public class HrmEmployeeDocumentInfosService : AppService<HrmEmployeeDocumentInfo>, IHrmEmployeeDocumentInfosService
    {
        private readonly IRepository<HrmEmployeeDocumentInfo> hrmEmpDocu;
        private readonly IRepository<CoreBranch> branchTypeInfoService;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmEmployee> hrmEmp;
        private readonly IRepository<CoreCompany> coreComapny;
        string strMaxNO = string.Empty;
        private const string TableName = "HRM_EmployeeDocumentInfo";
        private const string ColumnName = "EmpDocID";
        public HrmEmployeeDocumentInfosService(IRepository<HrmEmployeeDocumentInfo> hrmEmpDocu, IRepository<CoreBranch> branchTypeInfoService, ICommonService commonService, IRepository<CoreAccessCode> accessCodeRepository, IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo, IRepository<HrmDefDepartment> departmentRepository, IRepository<HrmDefDesignation> designationRepository, IRepository<HrmEmployee> hrmEmp, IRepository<CoreCompany> coreComapny) : base(hrmEmpDocu)
        {
            this.hrmEmpDocu = hrmEmpDocu;
            this.branchTypeInfoService = branchTypeInfoService;
            this.commonService = commonService;
            this.accessCodeRepository = accessCodeRepository;
            this.hrmEmpOffialInfo = hrmEmpOffialInfo;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.hrmEmp = hrmEmp;
            this.coreComapny = coreComapny;
        }

        public async Task<List<HrmEmployeeDocumentInfosSetup>> GetAllAsync(string employeeId)
        {
            try
            {

                var data = await (from empDocu in hrmEmpDocu.All().AsNoTracking()
                                  where empDocu.EmployeeId == employeeId

                                  select new HrmEmployeeDocumentInfosSetup
                                  {
                                      EmpDocId = empDocu.EmpDocId,
                                      EmployeeId = empDocu.EmployeeId,
                                      DocumentName = empDocu.DocumentName,
                                      DocumentDiscription = empDocu.DocumentDiscription,
                                      DocumentType = empDocu.DocumentType,
                                      Doucment = empDocu.Doucment, // Image
                                      SerialNo = Convert.ToInt16(empDocu.EmpDocId)
                                  }).OrderByDescending(x=>x.SerialNo).ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                // Use a logging framework to capture errors
                Console.WriteLine(ex.Message);
                return new List<HrmEmployeeDocumentInfosSetup>();
            }
        }


        public async Task<List<HrmEmployeeDocumentInfosSetup>> GetComapnyByBranchCode(string companyCode)
        {
            try
            {
                var result = await (
                                    from br in branchTypeInfoService.All().AsNoTracking()
                                    where (br.CompanyCode == companyCode)
                                    select new HrmEmployeeDocumentInfosSetup
                                    {

                                        BranchCode = br.BranchCode,
                                        CoreBranchName = br.BranchName
                                    }).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        //

        public async Task<List<HrmEmployeeDocumentInfosSetup>> GetEmployeeByCompanyCode(string companyCode)
        {
            try
            {


                var result = await (from e in hrmEmp.All().AsNoTracking()

                                    where (e.CompanyCode == companyCode)

                                    select new HrmEmployeeDocumentInfosSetup
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
        public async Task<HrmEmployeeDocumentInfosSetup> GetEmployeeNameDesDeptByCode(string employeeId)
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

                                    select new HrmEmployeeDocumentInfosSetup
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



        public async Task<HrmEmployeeDocumentInfosSetup> GetByIdAsync(string code)
        {

            var data = await (from empDocu in hrmEmpDocu.All().AsNoTracking()

                              join bra in branchTypeInfoService.All().AsNoTracking()
                              on empDocu.BranchCode equals bra.BranchCode into empDocuBraJoin
                              from bra in empDocuBraJoin.DefaultIfEmpty()
                              where empDocu.EmpDocId == code

                              join ofEmp in hrmEmpOffialInfo.All().AsNoTracking()
                              on empDocu.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                              from ofEmp in eduOffJoin.DefaultIfEmpty()

                              join desi in designationRepository.All().AsNoTracking()
                              on ofEmp.DesignationCode equals desi.DesignationCode into edudesiJoin
                              from desi in edudesiJoin.DefaultIfEmpty()

                              join dept in departmentRepository.All().AsNoTracking()
                              on ofEmp.DepartmentCode equals dept.DepartmentCode into eduDeptJoin
                              from dept in eduDeptJoin.DefaultIfEmpty()

                              join emp in hrmEmp.All().AsNoTracking()
                              on empDocu.EmployeeId equals emp.EmployeeId into empDocuEmpJoin
                              from emp in empDocuEmpJoin.DefaultIfEmpty()


                              select new HrmEmployeeDocumentInfosSetup
                              {
                                  AutoId = empDocu.AutoId,
                                  EmpDocId = empDocu.EmpDocId,
                                  EmployeeId = empDocu.EmployeeId,
                                  DocumentName = empDocu.DocumentName,
                                  DocumentDiscription = empDocu.DocumentDiscription,
                                  DocumentType = empDocu.DocumentType,
                                  Doucment = empDocu.Doucment,   //image
                                  CompanyCode = empDocu.CompanyCode,
                                  BranchCode = empDocu.BranchCode,
                                  Ldate = empDocu.Ldate,
                                  ModifyDate = empDocu.ModifyDate,
                                  DepartmentName = dept.DepartmentName,
                                  DesignationName = desi.DesignationName,
                                  EmployeeName = $"{emp.FirstName} {emp.LastName}" ?? ""
                              }).FirstOrDefaultAsync();
            return data;


        }




        public async Task<bool> SaveAsync(HrmEmployeeDocumentInfosSetup entityVM, string CompanyCode)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 2);
            await hrmEmpDocu.BeginTransactionAsync();
            try
            {

                var entity = new HrmEmployeeDocumentInfo();
                entity.EmpDocId = strMaxNO;


                if (entityVM.Photo != null && entityVM.Photo.Length > 0)
                {
                    var extension = Path.GetExtension(entityVM.Photo.FileName).ToLower();
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };

                    if (allowedExtensions.Contains(extension))
                    {
                        var empId = entityVM.EmployeeId;
                        var fileGuid = Guid.NewGuid();
                        var fileName = $"{empId}{extension}";

                        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Documents");
                        if (!Directory.Exists(uploadFolder))
                            Directory.CreateDirectory(uploadFolder);

                        var filePath = Path.Combine(uploadFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await entityVM.Photo.CopyToAsync(stream);
                        }

                        entityVM.Doucment = fileName;
                    }
                }


                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                entity.BranchCode = entityVM.BranchCode ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.DocumentDiscription = entityVM.DocumentDiscription ?? string.Empty;
                entity.DocumentName = entityVM.DocumentName ?? string.Empty;
                entity.DocumentType = entityVM.DocumentType ?? string.Empty;
                entity.Doucment = entityVM.Doucment; // Store the image GUID
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                entity.CompanyCode = CompanyCode;
                entity.UserInfoEmployeeId = entityVM.UserInfoEmployeeId ?? string.Empty;

                await hrmEmpDocu.AddAsync(entity);
                await hrmEmpDocu.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                await hrmEmpDocu.RollbackTransactionAsync();
                return false;
            }
        }







        public async Task<bool> UpdateAsync(HrmEmployeeDocumentInfosSetup entityVM)
        {
            await hrmEmpDocu.BeginTransactionAsync();
            var entity = await hrmEmpDocu.GetByIdAsync(entityVM.EmpDocId);
            if (entity == null)
            {
                await hrmEmpDocu.RollbackTransactionAsync();
                return false;
            }
            try
            {
                if (entityVM.Photo != null && entityVM.Photo.Length > 0)
                {
                    var extension = Path.GetExtension(entityVM.Photo.FileName).ToLower();
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };

                    if (allowedExtensions.Contains(extension))
                    {

                        var fileGuid = Guid.NewGuid();
                        var fileName = $"{fileGuid}{extension}";

                        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Documents");
                        if (!Directory.Exists(uploadFolder))
                            Directory.CreateDirectory(uploadFolder);

                        var filePath = Path.Combine(uploadFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await entityVM.Photo.CopyToAsync(stream);
                        }

                        entityVM.Doucment = fileName;
                    }
                }
                else
                {

                    entityVM.Doucment = entity.Doucment;
                }

                entity.EmpDocId = entityVM.EmpDocId;
                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                entity.BranchCode = entityVM.BranchCode ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.DocumentDiscription = entityVM.DocumentDiscription ?? string.Empty;
                entity.DocumentName = entityVM.DocumentName ?? string.Empty;
                entity.DocumentType = entityVM.DocumentType ?? string.Empty;
                entity.Doucment = entityVM.Doucment;  //Image
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                await hrmEmpDocu.UpdateAsync(entity);
                await hrmEmpDocu.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                await hrmEmpDocu.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await hrmEmpDocu.All().Where(x => ids.Contains(x.EmpDocId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }
            hrmEmpDocu.Delete(entity);
            return true;
        }


        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await hrmEmpDocu.All().AnyAsync(x => x.EmpDocId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hrmEmpDocu.All().AnyAsync(x => x.DocumentName == name);
        }

        public async Task<bool> IsExistAsync(string employeeCode, string typeCode, string degreeCode)
        {
            return await hrmEmpDocu.All().AnyAsync(x => x.EmployeeId == employeeCode && x.DocumentName == degreeCode && x.EmpDocId != typeCode);
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