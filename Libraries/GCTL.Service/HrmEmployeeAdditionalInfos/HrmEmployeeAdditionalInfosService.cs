using GCTL.Core.Data;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HrmEmployeeAdditionalInfos;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.HrmEmployeeAdditionalInfos
{
    public class HrmEmployeeAdditionalInfosService : AppService<HrmEmployeeAdditionalInfo>, IHrmEmployeeAdditionalInfosService
    {
        #region Services & Repositories
        private readonly IRepository<HrmEmployeeAdditionalInfo> empAddInfoRepository;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<SalesDefBankInfo> bankRepository;
        private readonly IRepository<SalesDefBankBranchInfo> bankBranchRepository;
        private readonly IRepository<CoreBranch> coreBranchRepository;
        private readonly IRepository<HrmEmployee> hrmEmployee;
        private readonly IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;

        private readonly IDeleteHistoryService deleteHistoryService;


        string strMaxNO = string.Empty;
        private const string TableName = "HRM_EmployeeAdditionalInfo";
        private const string ColumnName = "EmployeAddInfoID";


        public HrmEmployeeAdditionalInfosService(
            IRepository<HrmEmployeeAdditionalInfo> empAddInfoRepository, 
            IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo,
            IRepository<HrmDefDesignation> designationRepository, 
            IRepository<HrmDefDepartment> departmentRepository, 
            IRepository<HrmEmployee> hrmEmployee, 
            IRepository<CoreBranch> coreBranchRepository, 
            IRepository<SalesDefBankInfo> bankRepository,
            IRepository<SalesDefBankBranchInfo> bankBranchRepository, 
            IRepository<CoreAccessCode> accessCodeRepository, 
            IRepository<CoreCompany> coreCompanyRepository,
            IDeleteHistoryService deleteHistoryService

            ) 
            
    : base(empAddInfoRepository)
        {
            this.empAddInfoRepository = empAddInfoRepository;
            this.coreCompanyRepository = coreCompanyRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.bankRepository = bankRepository;
            this.bankBranchRepository = bankBranchRepository;
            this.coreBranchRepository = coreBranchRepository;
            this.hrmEmployee = hrmEmployee;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.hrmEmpOffialInfo = hrmEmpOffialInfo;
            this.deleteHistoryService = deleteHistoryService;

        }

        #endregion

        #region GetAllById   
        public async Task<List<HrmEmployeeAdditionalInfoSetupViewModel>> GetAllAsync()
        {
            var data = await (from empAddInfo in empAddInfoRepository.All().AsNoTracking()
                              join empComp in coreCompanyRepository.All().AsNoTracking()
                                  on empAddInfo.CompanyCode equals empComp.CompanyCode into empComJoin
                              from empComp in empComJoin.DefaultIfEmpty()

                                  // DBBL
                              join dbblBranch in bankBranchRepository.All().AsNoTracking()
                                  on empAddInfo.SalaryBranchId equals dbblBranch.BankBranchId into dbblBranchJoin
                              from dbblBranch in dbblBranchJoin.DefaultIfEmpty()
                              join dbblBank in bankRepository.All().AsNoTracking()
                                  on empAddInfo.SalaryBankId equals dbblBank.BankId into dbblBankJoin
                              from dbblBank in dbblBankJoin.DefaultIfEmpty()

                                  // UCBL
                              join ucblBranch in bankBranchRepository.All().AsNoTracking()
                                  on empAddInfo.BankBranchIducbl equals ucblBranch.BankBranchId into ucblBranchJoin
                              from ucblBranch in ucblBranchJoin.DefaultIfEmpty()
                              join ucblBank in bankRepository.All().AsNoTracking()
                                  on empAddInfo.BankIducbl equals ucblBank.BankId into ucblBankJoin
                              from ucblBank in ucblBankJoin.DefaultIfEmpty()

                                  // SIBL
                              join siblBranch in bankBranchRepository.All().AsNoTracking()
                                  on empAddInfo.BankBranchIdsibl equals siblBranch.BankBranchId into siblBranchJoin
                              from siblBranch in siblBranchJoin.DefaultIfEmpty()
                              join siblBank in bankRepository.All().AsNoTracking()
                                  on empAddInfo.BankIdsibl equals siblBank.BankId into siblBankJoin
                              from siblBank in siblBankJoin.DefaultIfEmpty()

                              join hrmEmp in hrmEmployee.All().AsNoTracking()
                                  on empAddInfo.EmployeeId equals hrmEmp.EmployeeId into empAddInfoEmployeeJoin
                              from hrmEmp in empAddInfoEmployeeJoin.DefaultIfEmpty()

                              join coreBranch in coreBranchRepository.All().AsNoTracking()
                                  on empAddInfo.BranchCode equals coreBranch.BranchCode into coreBranchJoin
                              from coreBranch in coreBranchJoin.DefaultIfEmpty()

                              join ofEmp in hrmEmpOffialInfo.All().AsNoTracking()
                               on empAddInfo.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                              from ofEmp in eduOffJoin.DefaultIfEmpty()

                              join desi in designationRepository.All().AsNoTracking()
                              on ofEmp.DesignationCode equals desi.DesignationCode into edudesiJoin
                              from desi in edudesiJoin.DefaultIfEmpty()

                              join dept in departmentRepository.All().AsNoTracking()
                              on ofEmp.DepartmentCode equals dept.DepartmentCode into eduDeptJoin
                              from dept in eduDeptJoin.DefaultIfEmpty()

                              select new HrmEmployeeAdditionalInfoSetupViewModel
                              {
                                  EmployeAddInfoId = empAddInfo.EmployeAddInfoId,
                                  EmployeeName = $"{hrmEmp.FirstName} {hrmEmp.LastName}",
                                  BankAcname = empAddInfo.BankAcname,
                                  BankAcNo = empAddInfo.BankAcNo,
                                  SalaryBankName = dbblBank != null ? dbblBank.BankName : " ",
                                  SalaryBranchName = dbblBranch != null ? dbblBranch.BankBranchName : " ",
                              }).ToListAsync();
            return data;
        }


        public async Task<HrmEmployeeAdditionalInfoSetupViewModel> GetByIdAsync(string id)
        {

            var data = await (from empAddInfo in empAddInfoRepository.All().AsNoTracking()
                              join empComp in coreCompanyRepository.All().AsNoTracking()
                                  on empAddInfo.CompanyCode equals empComp.CompanyCode into empComJoin
                              from empComp in empComJoin.DefaultIfEmpty()
                              where empAddInfo.EmployeAddInfoId == id
                              // DBBL
                              join dbblBranch in bankBranchRepository.All().AsNoTracking()
                                  on empAddInfo.SalaryBranchId equals dbblBranch.BankBranchId into dbblBranchJoin
                              from dbblBranch in dbblBranchJoin.DefaultIfEmpty()
                              join dbblBank in bankRepository.All().AsNoTracking()
                                  on dbblBranch.BankId equals dbblBank.BankId into dbblBankJoin
                              from dbblBank in dbblBankJoin.DefaultIfEmpty()

                                  // UCBL
                              join ucblBranch in bankBranchRepository.All().AsNoTracking()
                                  on empAddInfo.BankBranchIducbl equals ucblBranch.BankBranchId into ucblBranchJoin
                              from ucblBranch in ucblBranchJoin.DefaultIfEmpty()
                              join ucblBank in bankRepository.All().AsNoTracking()
                                  on ucblBranch.BankId equals ucblBank.BankId into ucblBankJoin
                              from ucblBank in ucblBankJoin.DefaultIfEmpty()

                                  // SIBL
                              join siblBranch in bankBranchRepository.All().AsNoTracking()
                                  on empAddInfo.BankBranchIdsibl equals siblBranch.BankBranchId into siblBranchJoin
                              from siblBranch in siblBranchJoin.DefaultIfEmpty()
                              join siblBank in bankRepository.All().AsNoTracking()
                                  on siblBranch.BankId equals siblBank.BankId into siblBankJoin
                              from siblBank in siblBankJoin.DefaultIfEmpty()

                              join hrmEmp in hrmEmployee.All().AsNoTracking()
                                  on empAddInfo.EmployeeId equals hrmEmp.EmployeeId into empAddInfoEmployeeJoin
                              from hrmEmp in empAddInfoEmployeeJoin.DefaultIfEmpty()

                              join coreBranch in coreBranchRepository.All().AsNoTracking()
                                  on empAddInfo.BranchCode equals coreBranch.BranchCode into coreBranchJoin
                              from coreBranch in coreBranchJoin.DefaultIfEmpty()

                              join ofEmp in hrmEmpOffialInfo.All().AsNoTracking() // note
                                    on empAddInfo.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                              from ofEmp in eduOffJoin.DefaultIfEmpty()

                              join desi in designationRepository.All().AsNoTracking()
                              on ofEmp.DesignationCode equals desi.DesignationCode into edudesiJoin
                              from desi in edudesiJoin.DefaultIfEmpty()

                              join dept in departmentRepository.All().AsNoTracking()
                              on ofEmp.DepartmentCode equals dept.DepartmentCode into eduDeptJoin
                              from dept in eduDeptJoin.DefaultIfEmpty()

                              select new HrmEmployeeAdditionalInfoSetupViewModel
                              {
                                  AutoId = empAddInfo.AutoId,
                                  EmployeAddInfoId = empAddInfo.EmployeAddInfoId,


                                  PassportName = empAddInfo.PassportName,
                                  PassportNo = empAddInfo.PassportNo,
                                  PassportIssueDate = empAddInfo.PassportIssueDate,
                                  PassportPlaceOfIssue = empAddInfo.PassportPlaceOfIssue,
                                  PassportExpiryDate = empAddInfo.PassportExpiryDate,
                                  LicenseNo = empAddInfo.LicenseNo,
                                  LicenseType = empAddInfo.LicenseType,
                                  LicenseIssueDate = empAddInfo.LicenseIssueDate,
                                  LicenseExpireDate = empAddInfo.LicenseExpireDate,
                                  LicensePlaceOfIssue = empAddInfo.LicensePlaceOfIssue,
                                  SymbolOfVehicleClass = empAddInfo.SymbolOfVehicleClass,

                                  WorkPermitNo = empAddInfo.WorkPermitNo,
                                  WorkPermitType = empAddInfo.WorkPermitType,
                                  WpEffectiveDate = empAddInfo.WpEffectiveDate,
                                  WpExpireDate = empAddInfo.WpExpireDate,
                                  AtmCardNo = empAddInfo.AtmCardNo,

                                  // DBBL
                                  SalaryBankId = empAddInfo.SalaryBankId,
                                  SalaryBankName = dbblBank.BankName,
                                  SalaryBranchId = empAddInfo.SalaryBranchId,
                                  SalaryBranchName = dbblBranch.BankBranchName,
                                  BranchAddres = empAddInfo.BranchAddres,
                                  BankAcname = empAddInfo.BankAcname,
                                  BankAcNo = empAddInfo.BankAcNo,

                                  // UCBL
                                  BankIducbl = empAddInfo.BankIducbl,
                                  BankNameUcbl = ucblBank.BankName,
                                  BankBranchIducbl = empAddInfo.BankBranchIducbl,
                                  BankBranchNameUcbl = ucblBranch.BankBranchName,
                                  BranchAddressUcbl = empAddInfo.BranchAddressUcbl,
                                  BankAcNameUcbl = empAddInfo.BankAcNameUcbl,
                                  BankAcNoUcbl = empAddInfo.BankAcNoUcbl,

                                  // SIBL
                                  BankIdsibl = empAddInfo.BankIdsibl,
                                  BankNameSibl = siblBank.BankName,
                                  BankBranchIdsibl = empAddInfo.BankBranchIdsibl,
                                  BankBranchNameSibl = siblBranch.BankBranchName,
                                  BranchAddressSibl = empAddInfo.BranchAddressSibl,
                                  BankAcNameSibl = empAddInfo.BankAcNameSibl,
                                  BankAcNoSibl = empAddInfo.BankAcNoSibl,

                                  CompanyCode = empComp.CompanyCode,
                                  CompanyName = empComp.CompanyName,
                                  BranchCode = empAddInfo.BranchCode,
                                  CoreBranchName = coreBranch.BranchName,
                                  EmployeeId = empAddInfo.EmployeeId,
                                  EmployeeName = $"{hrmEmp.FirstName} {hrmEmp.LastName}",
                                  //DepartmentName = dept != null ? dept.DepartmentName : "",
                                  //DesignationName = desi != null ? desi.DesignationName : "",
                                  DepartmentName = dept.DepartmentName ?? "",
                                  DesignationName = desi.DesignationName ?? "",

                                  Luser = empAddInfo.Luser,
                                  UserInfoEmployeeId = empAddInfo.UserInfoEmployeeId,
                                  Lmac = empAddInfo.Lmac,
                                  Lip = empAddInfo.Lip,
                                  Ldate = empAddInfo.Ldate,
                                  ModifyDate = empAddInfo.ModifyDate,

                              }).FirstOrDefaultAsync();

            return data;
        }


        #endregion

        #region PostUpdate
        public async Task<bool> SaveAsync(HrmEmployeeAdditionalInfoSetupViewModel entityVM, string CompanyCode)
        {
            await empAddInfoRepository.BeginTransactionAsync();
            try
            {
                HrmEmployeeAdditionalInfo entity = new HrmEmployeeAdditionalInfo();
                entity.EmployeAddInfoId = await GenerateNextCode();
                entity.CompanyCode = entityVM.CompanyCode;
                entity.BranchCode = entityVM.BranchCode;
                entity.EmployeeId = entityVM.EmployeeId;

                entity.PassportName = entityVM.PassportName;
                entity.PassportNo = entityVM.PassportNo;
                entity.PassportPlaceOfIssue = entityVM.PassportPlaceOfIssue;
                entity.PassportIssueDate = entityVM.PassportIssueDate;
                entity.PassportExpiryDate = entityVM.PassportExpiryDate;

                entity.SalaryBankId = entityVM.SalaryBankId;   //DBBL Information 
                entity.SalaryBranchId = entityVM.SalaryBranchId;
                entity.BranchAddres = entityVM.BranchAddres;
                entity.BankAcname = entityVM.BankAcname;
                entity.BankAcNo = entityVM.BankAcNo;

                entity.BankIducbl = entityVM.BankIducbl;   //UCBL Information 
                entity.BankBranchIducbl = entityVM.BankBranchIducbl;
                entity.BranchAddressUcbl = entityVM.BranchAddressUcbl;
                entity.BankAcNameUcbl = entityVM.BankAcNameUcbl;
                entity.BankAcNoUcbl = entityVM.BankAcNoUcbl;

                entity.BankIdsibl = entityVM.BankIdsibl;   //SIBL Information 
                entity.BankBranchIdsibl = entityVM.BankBranchIdsibl;
                entity.BranchAddressSibl = entityVM.BranchAddressSibl;
                entity.BankAcNameSibl = entityVM.BankAcNameSibl;
                entity.BankAcNoSibl = entityVM.BankAcNoSibl;

                entity.AtmCardNo = entityVM.AtmCardNo;
                entity.LicenseNo = entityVM.LicenseNo;
                entity.LicenseType = entityVM.LicenseType;
                entity.LicenseIssueDate = entityVM.LicenseIssueDate;
                entity.LicenseExpireDate = entityVM.LicenseExpireDate;
                entity.SymbolOfVehicleClass = entityVM.SymbolOfVehicleClass;
                entity.LicensePlaceOfIssue = entityVM.LicensePlaceOfIssue;
                entity.WorkPermitNo = entityVM.WorkPermitNo;
                entity.WorkPermitType = entityVM.WorkPermitType;
                entity.WpExpireDate = entityVM.WpExpireDate;
                entity.WpEffectiveDate = entityVM.WpEffectiveDate;
                entity.AtmCardNo = entityVM.AtmCardNo;

                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.Ldate = DateTime.Now;
                entity.CompanyCode = CompanyCode;
                entity.UserInfoEmployeeId = entityVM.UserInfoEmployeeId ?? string.Empty;

                await empAddInfoRepository.AddAsync(entity);

                await empAddInfoRepository.CommitTransactionAsync();
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
                await empAddInfoRepository.RollbackTransactionAsync();
                return false;
            }

        }

        public async Task<bool> UpdateAsync(HrmEmployeeAdditionalInfoSetupViewModel entityVM)
        {
            await empAddInfoRepository.BeginTransactionAsync();
            try
            {

                var entity = await empAddInfoRepository.GetByIdAsync(entityVM.EmployeAddInfoId);
                if (entity == null)
                {
                    await empAddInfoRepository.RollbackTransactionAsync();
                    return false;
                }

                entity.EmployeAddInfoId = entityVM.EmployeAddInfoId;
                entity.CompanyCode = entityVM.CompanyCode;
                entity.BranchCode = entityVM.BranchCode;
                entity.EmployeeId = entityVM.EmployeeId;

                entity.PassportName = entityVM.PassportName;
                entity.PassportNo = entityVM.PassportNo;
                entity.PassportPlaceOfIssue = entityVM.PassportPlaceOfIssue;
                entity.PassportIssueDate = entityVM.PassportIssueDate;
                entity.PassportExpiryDate = entityVM.PassportExpiryDate;

                entity.SalaryBankId = entityVM.SalaryBankId;   //DBBL Information 
                entity.SalaryBranchId = entityVM.SalaryBranchId;
                entity.BranchAddres = entityVM.BranchAddres;
                entity.BankAcname = entityVM.BankAcname;
                entity.BankAcNo = entityVM.BankAcNo;

                entity.BankIducbl = entityVM.BankIducbl;   //UCBL Information 
                entity.BankBranchIducbl = entityVM.BankBranchIducbl;
                entity.BranchAddressUcbl = entityVM.BranchAddressUcbl;
                entity.BankAcNameUcbl = entityVM.BankAcNameUcbl;
                entity.BankAcNoUcbl = entityVM.BankAcNoUcbl;

                entity.BankIdsibl = entityVM.BankIdsibl;   //SIBL Information 
                entity.BankBranchIdsibl = entityVM.BankBranchIdsibl;
                entity.BranchAddressSibl = entityVM.BranchAddressSibl;
                entity.BankAcNameSibl = entityVM.BankAcNameSibl;
                entity.BankAcNoSibl = entityVM.BankAcNoSibl;


                entity.LicenseNo = entityVM.LicenseNo;
                entity.LicenseType = entityVM.LicenseType;
                entity.LicenseIssueDate = entityVM.LicenseIssueDate;
                entity.LicenseExpireDate = entityVM.LicenseExpireDate;
                entity.SymbolOfVehicleClass = entityVM.SymbolOfVehicleClass;
                entity.LicensePlaceOfIssue = entityVM.LicensePlaceOfIssue;
                entity.WorkPermitNo = entityVM.WorkPermitNo;
                entity.WorkPermitType = entityVM.WorkPermitType;
                entity.WpExpireDate = entityVM.WpExpireDate;
                entity.WpEffectiveDate = entityVM.WpEffectiveDate;
                entity.AtmCardNo = entityVM.AtmCardNo;


                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.UserInfoEmployeeId = entityVM.UserInfoEmployeeId;
                entity.ModifyDate = DateTime.Now;
                await empAddInfoRepository.UpdateAsync(entity);
                await empAddInfoRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await empAddInfoRepository.RollbackTransactionAsync();
                return false;
            }
            finally
            {
                await empAddInfoRepository.DisposeTransactionAsync();
            }
        }
        #endregion

        #region Generate Next Code

        public async Task<string> GenerateNextCode()
        {
            var Code = await empAddInfoRepository.GetAllAsync();
            var lastCode = Code.Max(b => b.EmployeAddInfoId);
            int nextCode = 1;
            if (!string.IsNullOrEmpty(lastCode))
            {
                int lastNumber = int.Parse(lastCode.TrimStart('0'));
                lastNumber++;
                nextCode = lastNumber;
            }
            return nextCode.ToString("D4");
        }

        #endregion

        #region Delelete

        public async Task<bool> DeleteTab(List<string> ids, DeleteHistoryViewModel model)
        {
            var entity = await empAddInfoRepository.All().Where(x => ids.Contains(x.EmployeAddInfoId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            empAddInfoRepository.Delete(entity);
            model.tableName = TableName;
            await deleteHistoryService.LogDeletedRecordsAsync(
                entity, model
            );

            return true;
        }

        //public bool DeleteLeaveType(string id)
        //{
        //    var entity = GetLeaveType(id);

        //    if (entity != null)
        //    {
        //        empAddInfoRepository.Delete(entity);
        //        return true;
        //    }
        //    return false;
        //}

        public HrmEmployeeAdditionalInfo GetLeaveType(string code)
        {
            return empAddInfoRepository.GetById(code);
        }

        #endregion

        #region GetEmployee with Dept, Designation, EmpOfficialInfo


        #region Get Employee and Branch 
        public async Task<HrmEmployeeAdditionalInfoSetupViewModel> GetEmployeeByCode(string employeeId)
        {
            try
            {

                var result = await (

                                    from e in hrmEmployee.All().AsNoTracking()

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

                                    select new HrmEmployeeAdditionalInfoSetupViewModel
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

        public async Task<List<HrmEmployeeAdditionalInfoSetupViewModel>> GetComapnyByCode(string companyCode)
        {
            try
            {


                var result = await (from e in hrmEmployee.All().AsNoTracking()

                                    where (e.CompanyCode == companyCode)

                                    select new HrmEmployeeAdditionalInfoSetupViewModel
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

        //



        //Branch filter
        public async Task<List<HrmEmployeeAdditionalInfoSetupViewModel>> GetComapnyByBranchCode(string companyCode)
        {
            try
            {


                var result = await (
                                    from br in coreBranchRepository.All().AsNoTracking()
                                    where (br.CompanyCode == companyCode)
                                    select new HrmEmployeeAdditionalInfoSetupViewModel
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

        #endregion


        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Information System" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Information System" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Information System" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Information System" && x.CheckDelete);
        }

        #endregion

        #region IsExistByCode
        public async Task<bool> IsExistByCodeAsync(string code, string employeeCode)
        {
            return await empAddInfoRepository.All().AnyAsync(x => x.EmployeAddInfoId != code && x.EmployeeId == employeeCode);
        }

        #endregion

    }
}