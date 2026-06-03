using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.Companies;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRMPayrollLoan;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using GCTL.Service.EmployeeWeekendDeclaration;
using Microsoft.EntityFrameworkCore;
using NPOI.POIFS.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRMPayrollLoan
{
    public class HRMPayrollLoanService : AppService<HrmPayrollLoan>, IHRMPayrollLoanService
    {
        private readonly IRepository<HrmPayrollLoan> payrollLoanRepo;
        private readonly IRepository<CoreCompany> comRepo;
        private readonly IRepository<HrmEmployee> empRepo;
        private readonly IRepository<HrmEmployeeOfficialInfo> empOffRepo;
        private readonly IRepository<HrmDefDesignation> desiRepo;
        private readonly IRepository<HrmDefDepartment> dpRepo;
        private readonly IRepository<HrmPayLoanTypeEntry> payTypeRepo;
        private readonly IRepository<SalesDefPaymentMode> payModeRepo;
        private readonly IRepository<HrmPayPayHeadName> payHeadRepo;
        private readonly IRepository<SalesDefBankInfo> bankRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<HrmPayrollPaymentReceive> paymentRepo;
        private readonly IDeleteHistoryService deleteHistoryService;
        private readonly IRepository<HrmPayAdvancePay> advancePayRepo;

        public HRMPayrollLoanService(
            IRepository<HrmPayrollLoan> payrollLoanRepo,
            IRepository<CoreCompany> comRepo,
            IRepository<HrmEmployee> empRepo,
            IRepository<HrmEmployeeOfficialInfo> empOffRepo,
            IRepository<HrmDefDesignation> desiRepo,
            IRepository<HrmDefDepartment> dpRepo,
            IRepository<HrmPayLoanTypeEntry> payTypeRepo,
            IRepository<SalesDefPaymentMode> PayModeRepo,
            IRepository<HrmPayPayHeadName> payHeadRepo,
            IRepository<SalesDefBankInfo> bankRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
            IRepository<HrmPayrollPaymentReceive> paymentRepo,
            IDeleteHistoryService deleteHistoryService,
             IRepository<HrmPayAdvancePay> advancePayRepo
            ) : base(payrollLoanRepo)
        {
            this.payrollLoanRepo = payrollLoanRepo;
            this.comRepo = comRepo;
            this.empRepo = empRepo;
            this.empOffRepo = empOffRepo;
            this.desiRepo = desiRepo;
            this.dpRepo = dpRepo;
            this.payTypeRepo = payTypeRepo;
            payModeRepo = PayModeRepo;
            this.payHeadRepo = payHeadRepo;
            this.bankRepo = bankRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.paymentRepo = paymentRepo;
            this.deleteHistoryService = deleteHistoryService;
            this.advancePayRepo = advancePayRepo;
        }
        private readonly string CreateSuccess = "Data saved successfully.";
        private readonly string CreateFailed = "Data insertion failed.";
        private readonly string UpdateSuccess = "Data updated successfully.";
        private readonly string UpdateFailed = "Data update failed.";
        private readonly string DeleteSuccess = "Data deleted successfully.";
        private readonly string DeleteFailed = "Data deletion failed.";
        private readonly string DataExists = "Data already exists.";


        #region Permission all type

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AsNoTracking().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Loan" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AsNoTracking().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Loan" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AsNoTracking().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Loan" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AsNoTracking().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Loan" && x.CheckDelete);
        }

        #endregion




        public async Task<PayrollLoanFilterResultListDto> GetFilaterDataAsync(PayrollLoanFilterEntryDto filterEntryDto)
        {
            var queary = from eoi in empOffRepo.All().AsNoTracking()
                         join e in empRepo.All().AsNoTracking() on eoi.EmployeeId equals e.EmployeeId into e_join
                         from e in e_join.DefaultIfEmpty()
                         join c in comRepo.All().AsNoTracking() on eoi.CompanyCode equals c.CompanyCode into c_join
                         from c in c_join.DefaultIfEmpty()
                         join dg in desiRepo.All().AsNoTracking() on eoi.DesignationCode equals dg.DesignationCode into dg_join
                         from dg in dg_join.DefaultIfEmpty()
                         join dp in dpRepo.All().AsNoTracking() on eoi.DepartmentCode equals dp.DepartmentCode into dp_join
                         from dp in dp_join.DefaultIfEmpty()
                         select new
                         {
                             EmpId = e.EmployeeId,
                             CompanyCode = c.CompanyCode,
                             EmpName = e.FirstName + " " + e.LastName,
                             CompaneName = c.CompanyName,
                             DesignationName = dg.DesignationName,
                             DepartmentName = dp.DepartmentName,
                             JoinDate = eoi.JoiningDate.HasValue ? eoi.JoiningDate.Value.ToString("dd/MM/yyyy") : ""
                         };
            if (filterEntryDto.CompanyCodes?.Any() == true)
            {
                queary = queary.Where(x => x.CompanyCode != null && filterEntryDto.CompanyCodes.Contains(x.CompanyCode));
            }
            if (filterEntryDto.EmployeeIds?.Any() == true)
            {
                queary = queary.Where(x => x.EmpId != null && x.EmpName != null && filterEntryDto.EmployeeIds.Contains(x.EmpId));
            }
            var result = new PayrollLoanFilterResultListDto
            {
                Company = await queary.Where(x => x.CompanyCode != null && x.CompaneName != null).Select(x => new PayrollLoanFilterResultDto
                {
                    Code = x.CompanyCode,
                    Name = x.CompaneName
                }).Distinct().ToListAsyncSafe(),
                Employees = await queary.Where(x => x.EmpId != null && x.EmpName != null).Select(x => new PayrollLoanFilterResultDto
                {
                    Code = x.EmpId,
                    Name = x.EmpName,
                    DesignationName = x.DesignationName,
                    DepartmentName = x.DepartmentName,
                    joinDate = x.JoinDate,
                }).Distinct().ToListAsyncSafe(),
            };
            return result;
        }
        public async Task<PayrollLoanFilterResultListDto> GetFilterPaymentReceiveAsync(PayrollLoanFilterEntryDto filterEntryDto)
        {

            var queary = from le in payrollLoanRepo.All().AsNoTracking()
                         join eoi in empOffRepo.All().AsNoTracking() on le.EmployeeId equals eoi.EmployeeId into eoi_join
                         from eoi in eoi_join.DefaultIfEmpty()
                         join e in empRepo.All().AsNoTracking() on eoi.EmployeeId equals e.EmployeeId into e_join
                         from e in e_join.DefaultIfEmpty()
                         join c in comRepo.All().AsNoTracking() on eoi.CompanyCode equals c.CompanyCode into c_join
                         from c in c_join.DefaultIfEmpty()
                         join dg in desiRepo.All().AsNoTracking() on eoi.DesignationCode equals dg.DesignationCode into dg_join
                         from dg in dg_join.DefaultIfEmpty()
                         join dp in dpRepo.All().AsNoTracking() on eoi.DepartmentCode equals dp.DepartmentCode into dp_join
                         from dp in dp_join.DefaultIfEmpty()

                         join pm in payModeRepo.All().AsNoTracking() on le.PaymentModeId equals pm.PaymentModeId into pm_join
                         from pm in pm_join.DefaultIfEmpty()

                         join py in paymentRepo.All().AsNoTracking() on le.LoanId equals py.LoanId into py_join
                         from py in py_join.DefaultIfEmpty()

                         select new
                         {
                             EmpId = e.EmployeeId,
                             CompanyCode = c.CompanyCode,
                             EmpName = e.FirstName + " " + e.LastName,
                             CompaneName = c.CompanyName,
                             DesignationName = dg.DesignationName,
                             DepartmentName = dp.DepartmentName,
                             JoinDate = eoi.JoiningDate.HasValue ? eoi.JoiningDate.Value.ToString("dd/MM/yyyy") : "",
                             loanId = le.LoanId,
                             PaymentModeName = pm.PaymentModeName,
                             PaymentDate = py.PaymentDate.HasValue ? py.PaymentDate.Value.ToString("dd/MM/yyyy") : "",
                             PaymentAmount = py.PaymentAmount,
                             paymentId = py.PaymentId,
                             remark = py.Remarks
                         };
            if (filterEntryDto.CompanyCodes?.Any() == true)
            {
                queary = queary.Where(x => x.CompanyCode != null && filterEntryDto.CompanyCodes.Contains(x.CompanyCode));
            }
            if (filterEntryDto.EmployeeIds?.Any() == true)
            {
                queary = queary.Where(x => x.EmpId != null && x.EmpName != null && filterEntryDto.EmployeeIds.Contains(x.EmpId));
            }
            var result = new PayrollLoanFilterResultListDto
            {
                Company = await queary.Where(x => x.CompanyCode != null && x.CompaneName != null).Select(x => new PayrollLoanFilterResultDto
                {
                    Code = x.CompanyCode,
                    Name = x.CompaneName
                }).Distinct().ToListAsyncSafe(),
                Employees = await queary.Where(x => x.EmpId != null && x.EmpName != null).Select(x => new PayrollLoanFilterResultDto
                {
                    Code = x.EmpId,
                    Name = x.EmpName,
                    DesignationName = x.DesignationName,
                    DepartmentName = x.DepartmentName,
                    joinDate = x.JoinDate,
                }).Distinct().ToListAsyncSafe(),
                PaymentReceiveEmployee = await queary.Where(x => x.loanId != null).Select(x => new HrmPayrollPaymentReceiveDto
                {
                    LoanId = x.loanId,
                    PaymentId = x.paymentId,
                    ShowPaymentDate = x.PaymentDate,
                    PaymentAmount = x.PaymentAmount,
                    PaymentMode = x.PaymentModeName,
                    Remarks = x.remark,
                    EmpId = x.EmpId,
                    EmpName = x.EmpName,
                    Designation = x.DesignationName,

                }).Distinct().ToListAsyncSafe(),
            };
            if (result.Company == null || result.Company.Count == 0)
            {
                result.Company = await comRepo.All().AsNoTracking()
                    .Select(c => new PayrollLoanFilterResultDto
                    {
                        Code = c.CompanyCode,
                        Name = c.CompanyName
                    }).Distinct().ToListAsyncSafe();
            }

            if (result.Employees == null || result.Employees.Count == 0)
            {
                result.Employees = await (
                    from emp in empRepo.All().AsNoTracking()
                    join eoi in empOffRepo.All().AsNoTracking() on emp.EmployeeId equals eoi.EmployeeId into eoi_join
                    from eoi in eoi_join.DefaultIfEmpty()
                    join dis in desiRepo.All().AsNoTracking() on eoi.DesignationCode equals dis.DesignationCode into dis_join
                    from dis in dis_join.DefaultIfEmpty()
                    join dep in dpRepo.All().AsNoTracking() on eoi.DepartmentCode equals dep.DepartmentCode into dep_join
                    from depItem in dep_join.DefaultIfEmpty()
                    select new PayrollLoanFilterResultDto
                    {
                        Code = emp.EmployeeId,
                        Name = emp.FirstName + " " + emp.LastName,
                        DesignationName = dis.DesignationName,
                        DepartmentName = depItem.DepartmentName,
                        joinDate = eoi.JoiningDate.HasValue ? eoi.JoiningDate.Value.ToString("dd/MM/yyyy") : ""
                    }
                ).Distinct().ToListAsyncSafe();
            }

            return result;
        }



        public async Task<List<HrmPayrollPaymentReceiveDto>> GetPaymentReceiveAsync()
        {
            try
            {
                var result = await (from py in paymentRepo.All().AsNoTracking()

                                    join le in payrollLoanRepo.All().AsNoTracking() on py.LoanId equals le.LoanId into le_join
                                    from le in le_join.DefaultIfEmpty()

                                    join eoi in empOffRepo.All().AsNoTracking() on le.EmployeeId equals eoi.EmployeeId into eoi_join
                                    from eoi in eoi_join.DefaultIfEmpty()

                                    join e in empRepo.All().AsNoTracking() on eoi.EmployeeId equals e.EmployeeId into e_join
                                    from e in e_join.DefaultIfEmpty()

                                    join c in comRepo.All().AsNoTracking() on eoi.CompanyCode equals c.CompanyCode into c_join
                                    from c in c_join.DefaultIfEmpty()

                                    join dg in desiRepo.All().AsNoTracking() on eoi.DesignationCode equals dg.DesignationCode into dg_join
                                    from dg in dg_join.DefaultIfEmpty()

                                    join dp in dpRepo.All().AsNoTracking() on eoi.DepartmentCode equals dp.DepartmentCode into dp_join
                                    from dp in dp_join.DefaultIfEmpty()

                                    join pm in payModeRepo.All().AsNoTracking() on py.PaymentModeId equals pm.PaymentModeId into pm_join
                                    from pm in pm_join.DefaultIfEmpty()

                                    select new HrmPayrollPaymentReceiveDto
                                    {
                                        AutoId = py.AutoId,
                                        LoanId = le != null ? le.LoanId : "",
                                        PaymentId = py.PaymentId,
                                        ShowPaymentDate = py.PaymentDate.HasValue
                                                          ? py.PaymentDate.Value.ToString("dd/MM/yyyy")
                                                          : "",
                                        PaymentAmount = py.PaymentAmount,
                                        PaymentMode = pm != null ? pm.PaymentModeName : "",
                                        Remarks = py.Remarks,
                                        EmpId = e != null ? e.EmployeeId : "",
                                        EmpName = e != null ? e.FirstName + " " + e.LastName : "",
                                        Designation = dg != null ? dg.DesignationName : ""
                                    }).OrderByDescending(x => x.PaymentId).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public async Task<(bool isSuccess, string message, PayrollLoanFilterResultDto)> EmployeeGetById(string empId)
        {
            if (string.IsNullOrWhiteSpace(empId))
            {
                return (false, "Employee not found", null);
            }
            try
            {
                var e = empRepo.GetById(empId);
                if (e == null)
                {
                    return (false, "Employee not found", null);
                }

                var eoi = await empOffRepo.All().AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeId == e.EmployeeId);
                var c = await comRepo.All().AsNoTracking().FirstOrDefaultAsync(x => x.CompanyCode == e.CompanyCode);
                var dg = await desiRepo.All().AsNoTracking().FirstOrDefaultAsync(x => x.DesignationCode == eoi.DesignationCode);
                var dp = await dpRepo.All().AsNoTracking().FirstOrDefaultAsync(x => x.DepartmentCode == eoi.DepartmentCode);

                var employee = new PayrollLoanFilterResultDto
                {
                    Code = e?.EmployeeId,
                    Name = (e?.FirstName + " " + e?.LastName)?.Trim(),
                    DesignationName = dg?.DesignationName,
                    DepartmentName = dp?.DepartmentName,
                    joinDate = eoi.JoiningDate?.ToString("dd/MM/yyyy") ?? ""
                };
                return (true, "Employee found", employee);

            }
            catch (Exception ex)
            {
                return (true, ex.Message, null);
            }
        }



        public async Task<(bool isSuccess, string message, HrmPayrollPaymentReceiveListDto)> PaymentReciveEmployeeGetById(string empId)
        {
            if (string.IsNullOrWhiteSpace(empId))
            {
                return (false, "Employee not found", null);
            }

            try
            {
               

                var loanList = await (from eoi in empOffRepo.All().AsNoTracking()
                                      join e in empRepo.All().AsNoTracking() on eoi.EmployeeId equals e.EmployeeId into e_join
                                      from e in e_join.DefaultIfEmpty()
                                      join dg in desiRepo.All().AsNoTracking() on eoi.DesignationCode equals dg.DesignationCode into dg_join
                                      from dg in dg_join.DefaultIfEmpty()
                                      join dp in dpRepo.All().AsNoTracking() on eoi.DepartmentCode equals dp.DepartmentCode into dp_join
                                      from dp in dp_join.DefaultIfEmpty()
                                      join le in payrollLoanRepo.All().AsNoTracking()
                                          on eoi.EmployeeId equals le.EmployeeId into le_join
                                      from le in le_join.DefaultIfEmpty()
                                      where eoi.EmployeeId == empId
                                      select new PayrollLoanFilterResultDto
                                      {
                                          Code = le != null ? le.LoanId : "",
                                          Name = (e.FirstName + " " + e.LastName).Trim(),
                                          EmpId = e.EmployeeId,
                                          EmpName = (e.FirstName + " " + e.LastName).Trim(),
                                          DesignationName = dg.DesignationName,
                                          DepartmentName = dp.DepartmentName,
                                          joinDate = eoi.JoiningDate.HasValue ? eoi.JoiningDate.Value.ToString("dd/MM/yyyy") : ""
                                      }).ToListAsync();

                var loanIds = loanList.Select(l => l.Code).ToList();

                var paymentList = await (from p in paymentRepo.All().AsNoTracking()
                                         where loanIds.Contains(p.LoanId)
                                         join le in payrollLoanRepo.All().AsNoTracking() on p.LoanId equals le.LoanId
                                         join eoi in empOffRepo.All().AsNoTracking() on le.EmployeeId equals eoi.EmployeeId into eoi_join
                                         from eoi in eoi_join.DefaultIfEmpty()
                                         join e in empRepo.All().AsNoTracking() on eoi.EmployeeId equals e.EmployeeId into e_join
                                         from e in e_join.DefaultIfEmpty()
                                         join dg in desiRepo.All().AsNoTracking() on eoi.DesignationCode equals dg.DesignationCode into dg_join
                                         from dg in dg_join.DefaultIfEmpty()
                                         join pm in payModeRepo.All().AsNoTracking() on p.PaymentModeId equals pm.PaymentModeId into pm_join
                                         from pm in pm_join.DefaultIfEmpty()

                                         select new HrmPayrollPaymentReceiveDto
                                         {
                                             LoanId = p.LoanId,
                                             PaymentId = p.PaymentId,
                                             ShowPaymentDate = p.PaymentDate.HasValue ? p.PaymentDate.Value.ToString("dd/MM/yyyy") : "",
                                             PaymentAmount = p.PaymentAmount,
                                             PaymentMode = pm.PaymentModeName,
                                             Remarks = p.Remarks,
                                             EmpId = e.EmployeeId,
                                             EmpName = e.FirstName + " " + e.LastName,
                                             Designation = dg.DesignationName
                                         }).OrderByDescending(x => x.PaymentId).ToListAsync();

                var result = new HrmPayrollPaymentReceiveListDto
                {
                    PayrollLoanFilterResultDto = loanList,
                    hrmPayrollPaymentReceiveDtos = paymentList
                };

                return (true, "Employee found", result);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<HrmPayrollPaymentReceiveDto> getPaymentReceiveByIdAsync(string paymentId)
        {
            try
            {
                var result = paymentRepo.All().AsNoTracking().Where(x => x.PaymentId == paymentId).FirstOrDefault();
                if (result == null)
                    return null;

                // Manual mapping
                var dto = new HrmPayrollPaymentReceiveDto
                {
                    AutoId = result.AutoId,
                    PaymentId = result.PaymentId,
                    LoanId = result.LoanId,
                    PaymentAmount = result.PaymentAmount,
                    PaymentDate = result.PaymentDate,
                    Remarks = result.Remarks,
                    BankAccount = result.BankAccount,
                    BankName = await bankRepo.All().AsNoTracking().Where(x => x.BankId == result.BankId).Select(x => x.BankName).FirstOrDefaultAsync(),
                    BankId = result.BankId,
                    ChequeNo = result.ChequeNo,
                    ShowChequeDate = result.ChequeDate.HasValue ? result.ChequeDate.Value.ToString("dd/MM/yyyy") : "",
                    PaymentMode = result.PaymentModeId,
                    ShowCreateDate = result.Ldate.HasValue ? result.Ldate.Value.ToString("dd/MM/yyyy") : "",
                    ShowModifyDate = result.ModifyDate.HasValue ? result.ModifyDate.Value.ToString("dd/MM/yyyy") : "",
                };

                return dto;
            }
            catch (Exception e)
            {
                throw;
            }

        }


        public async Task<(bool isSuccess, string message, List<LoanTypeDto> data)> GetLoanTypeAsync()
        {
            var result = await payTypeRepo.GetAllAsync();

            var dtoList = result.Select(x => new LoanTypeDto
            {
                LoanTypeId = x.LoanTypeId,
                LoanType = x.LoanType,
                ShortName = x.ShortName,
                CompanyCode = x.CompanyCode
            }).ToList();
            return (true, "Loan Type fetched successfully", dtoList);
        }
        public async Task<List<PaymentModeDto>> getPaymentModeAsync()
        {
            var result = await payModeRepo.GetAllAsync();
            var ModeList = result.Select(x => new PaymentModeDto
            {
                PaymentModeId = x.PaymentModeId,
                PaymentModeName = x.PaymentModeName,
                PaymentModeShortName = x.PaymentModeShortName
            }).ToList();
            return ModeList;
        }
        public async Task<List<PayHeadNameDto>> GetPayHeadDeductionAsync()
        {
            var result = await payHeadRepo.GetAllAsync();
            var PayHeadList = result.Select(x => new PayHeadNameDto
            {
                LoanTypeId = x.LoanTypeId,
                Name = x.Name,
                PayHeadNameCode = x.PayHeadNameCode,
                PayHeadNameId = x.PayHeadNameId
            }).ToList();
            return PayHeadList;
        }
        public async Task<List<SalesDefBankInfoDto>> GetBankAsync()
        {
            var banks = await bankRepo.GetAllAsync();
            var bankList = banks.Select(x => new SalesDefBankInfoDto
            {
                BankId = x.BankId,
                BankName = x.BankName,
                AutoId = x.AutoId,
                Ldate = x.Ldate,
                Lip = x.Lip,
                Lmac = x.Lmac,
                Luser = x.Luser,
                ModifyDate = x.ModifyDate,
                ShortName = x.ShortName,
            }).ToList();
            return bankList;
        }
        public async Task<string> createLoanIdAsync()
        {
            var lastLoanId = await payrollLoanRepo.All().AsNoTracking().OrderByDescending(x => x.LoanId).Select(x => x.LoanId).FirstOrDefaultAsync();
            string newLoanId;
            if (!string.IsNullOrEmpty(lastLoanId))
            {
                int lastNumber = int.Parse(lastLoanId.Substring(1));
                newLoanId = "L" + (lastNumber + 1).ToString("D8");
            }
            else
            {
                newLoanId = "L00000001";
            }
            return newLoanId;
        }

        public async Task<string> createPaymentReceiveIdAsync()
        {
            var lastPaymentReceiveId = await paymentRepo.All().AsNoTracking().OrderByDescending(x => x.PaymentId).Select(x => x.PaymentId).FirstOrDefaultAsync();
            string newPaymentReceiveId;
            if (!string.IsNullOrEmpty(lastPaymentReceiveId))
            {
                int lastNumber = int.Parse(lastPaymentReceiveId.Substring(1));
                newPaymentReceiveId = (lastNumber + 1).ToString("D8");
            }
            else
            {
                newPaymentReceiveId = "00000001";
            }
            return newPaymentReceiveId;
        }

        //create and edit
        public async Task<(bool isSuccess, string message, object data)> CreateEditLoanAsycn(HRMPayrollLoanSetupViewModel modelData)
        {
            if (string.IsNullOrWhiteSpace(modelData.CompanyCode) || string.IsNullOrWhiteSpace(modelData.EmployeeId) || modelData.LoanAmount == null)
            {
                return (false, "Data Invalid", null);
            }
            if (modelData.LoanAmount <= 0)
            {
                return (false, "Data Invalid", null);
            }

            if (modelData.StartDate == null || modelData.EndDate == null)
            {
                return (false, "Start Date or End Date is missing", null);
            }

            DateTime startDate = modelData.StartDate.Value;
            DateTime endDate = modelData.EndDate.Value;

            if (endDate < startDate)
            {
                return (false, "End Date cannot be before Start Date", null);
            }

            // Calculate total months like JavaScript logic
            int yearDiff = endDate.Year - startDate.Year;
            int monthDiff = endDate.Month - startDate.Month;
            int totalMonths = (yearDiff * 12) + monthDiff + 1;

            if (totalMonths < 0)
            {
                return (false, "Invalid date range", null);
            }

            modelData.NoOfInstallment = totalMonths.ToString();

            if (totalMonths == 0)
            {
                modelData.MonthlyDeduction = modelData.LoanAmount;
            }
            else
            {
                decimal loanAmount = modelData.LoanAmount ?? 0;
                decimal monthlyDeduction = Math.Ceiling(loanAmount / totalMonths);
                modelData.MonthlyDeduction = monthlyDeduction;
            }
            if (modelData.AutoId == 0)
            {
                var entity = new HrmPayrollLoan
                {
                    LoanId = modelData.LoanId??"",
                    EmployeeId = modelData.EmployeeId??"",
                    LoanDate = modelData.LoanDate,
                    LoanTypeId = modelData.LoanTypeId,
                    StartDate = modelData.StartDate,
                    LoanStatus = "",
                    EndDate = modelData.EndDate,
                    LoanAmount = modelData.LoanAmount,
                    NoOfInstallment = modelData.NoOfInstallment??"",
                    MonthlyDeduction = modelData.MonthlyDeduction,
                    PayHeadNameId = modelData.PayHeadNameId??"",
                    PaymentModeId = modelData.PaymentModeId??"",
                    ChequeNo = modelData.ChequeNo??"",
                    ChequeDate = modelData.ChequeDate,
                    BankId = modelData.BankId??"",
                    BankAccount = modelData.BankAccount??"",
                    Remarks = modelData.Remarks??"",
                    ReasonOfLoanTaken = modelData.ReasonOfLoanTaken??"",
                    CompanyCode = modelData.CompanyCode??"",
                    Luser = modelData.Luser??"",
                    Lip = modelData.Lip??"",
                    Lmac = modelData.Lmac??"",
                    Ldate = modelData.Ldate
                };

                await payrollLoanRepo.AddAsync(entity);

                return (true, CreateSuccess, modelData);
            }
            else
            {
                var loanData = await payrollLoanRepo.GetByIdAsync(modelData.AutoId);
                if (loanData == null)
                {
                    return (false, "Update Faild", modelData);
                }
                loanData.LoanId = modelData.LoanId??"";
                loanData.EmployeeId = modelData.EmployeeId??"";
                loanData.LoanDate = modelData.LoanDate;
                loanData.LoanTypeId = modelData.LoanTypeId;
                loanData.StartDate = modelData.StartDate;
                loanData.LoanStatus = "";
                loanData.EndDate = modelData.EndDate;
                loanData.LoanAmount = modelData.LoanAmount;
                loanData.NoOfInstallment = modelData.NoOfInstallment??"";
                loanData.MonthlyDeduction = modelData.MonthlyDeduction;
                loanData.PayHeadNameId = modelData.PayHeadNameId??"";
                loanData.PaymentModeId = modelData.PaymentModeId??"";
                loanData.ChequeNo = modelData.ChequeNo??"";
                loanData.ChequeDate = modelData.ChequeDate;
                loanData.BankId = modelData.BankId??"";
                loanData.BankAccount = modelData.BankAccount??"";
                loanData.Remarks = modelData.Remarks??"";
                loanData.ReasonOfLoanTaken = modelData.ReasonOfLoanTaken??"";
                loanData.CompanyCode = modelData.CompanyCode??"";
                loanData.ModifyDate = modelData.ModifyDate;
                await payrollLoanRepo.UpdateAsync(loanData);
                return (true, UpdateSuccess, modelData);
            }

        }

        //create & edit payment receive 
        public async Task<(bool isSuccess, string message, object data)> CreateEditPaymentReceiveAsync(PaymentReceiveSetupViewModel modelData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(modelData.CompanyCode) ||
                    string.IsNullOrWhiteSpace(modelData.EmployeeId) ||
                    string.IsNullOrWhiteSpace(modelData.LoanId) ||
                    modelData.LoanAmount == null || modelData.LoanAmount <= 0)
                {
                    return (false, CreateFailed, null);
                }


                //            var advanceAdjustLoans = advancePayRepo.All()
                //.Where(x => x.LoanId == modelData.LoanId)
                //.OrderBy(x => x.AdvancePayId)
                //.ToList();

                //            if (advanceAdjustLoans == null || !advanceAdjustLoans.Any())
                //                return (false, "No installment records found for this loan.", null);

                //            var ModelMonth = modelData.LoanDate.ToString("MMMM");
                //            var ModelYear = modelData.LoanDate.ToString("yyyy");


                //            var paymentRow = advanceAdjustLoans
                //                .FirstOrDefault(x => x.SalaryMonth == ModelMonth && x.SalaryYear == ModelYear);

                //            if (paymentRow == null)
                //                return (false, $"No installment record found for {ModelMonth} {ModelYear}.", null);

                //            var rowsToPayment = advanceAdjustLoans
                //                .Where(x => string.Compare(x.AdvancePayId, paymentRow.AdvancePayId) < 0)
                //                .ToList();

                //            var rowsToDelete = advanceAdjustLoans
                //                .Where(x => string.Compare(x.AdvancePayId, paymentRow.AdvancePayId) >= 0)
                //                .ToList();

                //            decimal paidAmount = rowsToPayment.Sum(x => x.MonthlyDeduction);
                //            decimal remainingLoanAmount = paymentRow.AdvanceAmount - paidAmount;

                //            if (remainingLoanAmount != modelData.LoanAmount)
                //                return (false, $"Payment amount mismatch. Expected: {paidAmount}, Received: {modelData.LoanAmount}.", null);

                //            if (!rowsToDelete.Any())
                //                return (false, "No records found to delete for the selected month and onwards.", null);

                //            await advancePayRepo.DeleteRangeAsync(rowsToDelete);

                var advanceAdjustLoans = advancePayRepo.All()
     .Where(x => x.LoanId == modelData.LoanId)
     .OrderBy(x => x.AdvancePayId)
     .ToList();

                if (advanceAdjustLoans == null || !advanceAdjustLoans.Any())
                    return (false, "No installment records found for this loan.", null);

                var ModelMonth = modelData.LoanDate.ToString("MMMM");
                var ModelYear = modelData.LoanDate.ToString("yyyy");

                var paymentRow = advanceAdjustLoans
                    .FirstOrDefault(x => x.SalaryMonth == ModelMonth && x.SalaryYear == ModelYear);

                if (paymentRow == null)
                    return (false, $"No installment record found for {ModelMonth} {ModelYear}.", null);

                var rowsToPayment = advanceAdjustLoans
                    .Where(x => string.Compare(x.AdvancePayId, paymentRow.AdvancePayId) < 0)
                    .ToList();

                var rowsToDelete = advanceAdjustLoans
                    .Where(x => string.Compare(x.AdvancePayId, paymentRow.AdvancePayId) >= 0)
                    .ToList();

                decimal paidAmount = rowsToPayment.Sum(x => x.MonthlyDeduction);
                decimal remainingLoanAmount = paymentRow.AdvanceAmount - paidAmount;
                decimal receivedAmount = modelData.LoanAmount;

                if (receivedAmount > remainingLoanAmount)
                    return (false, $"Received amount ({receivedAmount}) exceeds remaining loan ({remainingLoanAmount}).", null);

                if (!rowsToDelete.Any())
                    return (false, "No records found to delete for the selected month and onwards.", null);

                // ✅ Exact match
                if (receivedAmount == remainingLoanAmount)
                {
                    await advancePayRepo.DeleteRangeAsync(rowsToDelete);
                }
                else
                {
                    decimal shortfall = remainingLoanAmount - receivedAmount;
                    decimal originalInstallment = paymentRow.MonthlyDeduction;

                  
                    int startId = rowsToDelete
                        .Select(x => int.Parse(x.AdvancePayId))
                        .Min();

                    await advancePayRepo.DeleteRangeAsync(rowsToDelete);

                    //var currentDate = new DateTime(
                    //    int.Parse(ModelYear),
                    //    DateTime.ParseExact(ModelMonth, "MMMM",
                    //        System.Globalization.CultureInfo.InvariantCulture).Month,
                    //    1
                    //).AddMonths(1);
                    var currentDate = new DateTime(
    int.Parse(ModelYear),
    DateTime.ParseExact(ModelMonth, "MMMM",
        System.Globalization.CultureInfo.InvariantCulture).Month,
    1
);
                    decimal remaining = shortfall;
                    int nextId = startId; 

                    var newRows = new List<HrmPayAdvancePay>();

                    while (remaining > 0)
                    {
                        decimal thisMonthAmount = Math.Min(remaining, originalInstallment);

                        newRows.Add(new HrmPayAdvancePay
                        {
                            AdvancePayId = nextId.ToString("D8"),
                            SalaryMonth = currentDate.ToString("MMMM"),
                            SalaryYear = currentDate.ToString("yyyy"),
                            MonthlyDeduction = thisMonthAmount,
                            ModifyDate = DateTime.Now,

                            LoanId = paymentRow.LoanId,
                            EmployeeId = paymentRow.EmployeeId,
                            AdvanceAmount = paymentRow.AdvanceAmount,
                            AdvanceAdjustStatus = paymentRow.AdvanceAdjustStatus,
                            NoOfPaymentInstallment = paymentRow.NoOfPaymentInstallment,
                            PayHeadNameId = paymentRow.PayHeadNameId,
                            Remarks = paymentRow.Remarks,
                            AdjustmentType = paymentRow.AdjustmentType,
                            CompanyCode = paymentRow.CompanyCode,
                            Luser = paymentRow.Luser,
                            Ldate = paymentRow.Ldate,
                            Lip = paymentRow.Lip,
                            Lmac = paymentRow.Lmac,
                        });

                        remaining -= thisMonthAmount;
                        currentDate = currentDate.AddMonths(1);
                        nextId++;
                    }

                    if (newRows.Any())
                        await advancePayRepo.AddRangeAsync(newRows);
                }

                if (modelData.AutoId == 0)
                {
                    var entity = new HrmPayrollPaymentReceive
                    {
                        PaymentId = modelData.PaymentId ?? "",
                        PaymentDate = modelData.LoanDate,
                        EmployeeId = modelData.EmployeeId ?? "",
                        LoanId = modelData.LoanId ?? "",
                        PaymentAmount = modelData.LoanAmount,
                        PaymentModeId = modelData.PaymentModeId ?? "",
                        ChequeNo = modelData.ChequeNo ?? "",
                        ChequeDate = string.IsNullOrWhiteSpace(modelData.ChequeDate) ? (DateTime?)null : DateTime.ParseExact(modelData.ChequeDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        BankId = modelData.BankId ?? "",
                        BankAccount = modelData.BankAccount ?? "",
                        Remarks = modelData.Remarks ?? "",
                        Luser = modelData.Luser ?? "",
                        Ldate = modelData.Ldate,
                        Lip = modelData.Lip ?? "",
                        Lmac = modelData.Lmac ?? "",
                        CompanyCode = modelData.CompanyCode ?? "",
                        EntryUserId = modelData.UserInfoEmployeeId ?? "",
                    };

                    await paymentRepo.AddAsync(entity);
                    return (true, CreateSuccess, entity);
                }
                else
                {
                    try
                    {
                        var ExistsData = await paymentRepo.GetByIdAsync(modelData.AutoId);
                        if (ExistsData != null)
                        {
                            ExistsData.PaymentId = modelData.PaymentId ?? "";
                            ExistsData.PaymentDate = modelData.LoanDate;
                            ExistsData.EmployeeId = modelData.EmployeeId ?? "";
                            ExistsData.LoanId = modelData.LoanId ?? "";
                            ExistsData.PaymentAmount = modelData.LoanAmount;
                            ExistsData.PaymentModeId = modelData.PaymentModeId ?? "";
                            ExistsData.ChequeNo = modelData.ChequeNo ?? "";
                            ExistsData.ChequeDate = string.IsNullOrWhiteSpace(modelData.ChequeDate) ? (DateTime?)null : DateTime.ParseExact(modelData.ChequeDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            ExistsData.BankId = modelData.BankId ?? "";
                            ExistsData.BankAccount = modelData.BankAccount ?? "";
                            ExistsData.Remarks = modelData.Remarks ?? "";
                            ExistsData.ModifyDate = DateTime.Today;
                        }
                        await paymentRepo.UpdateAsync(ExistsData);
                        return (true, UpdateSuccess, ExistsData);
                    }
                    catch (Exception ex)
                    {
                        return (false, UpdateFailed, null);
                    }

                }

            }
            catch (Exception ex)
            {

                return (false, $"Exception: {ex.Message}", null);
            }
        }


        public async Task<List<HRMPayrollLoanSetupViewModel>> GetLoanDataAsync()
        {
            var queary = from lon in payrollLoanRepo.All().AsNoTracking()
                         join eoi in empOffRepo.All().AsNoTracking() on lon.EmployeeId equals eoi.EmployeeId
                         join e in empRepo.All().AsNoTracking() on lon.EmployeeId equals e.EmployeeId into eJoin
                         from e in eJoin.DefaultIfEmpty()
                         join dg in desiRepo.All().AsNoTracking() on eoi.DesignationCode equals dg.DesignationCode into dgJoin
                         from dg in dgJoin.DefaultIfEmpty()
                         join lType in payTypeRepo.All().AsNoTracking() on lon.LoanTypeId equals lType.LoanTypeId into lTypeJoin
                         from lType in lTypeJoin.DefaultIfEmpty()
                         join pType in payModeRepo.All().AsNoTracking() on lon.PaymentModeId equals pType.PaymentModeId into pTypeJoin
                         from pType in pTypeJoin.DefaultIfEmpty()
                         join dp in dpRepo.All().AsNoTracking() on eoi.DepartmentCode equals dp.DepartmentCode into dpJoin
                         from dp in dpJoin.DefaultIfEmpty()
                         select new
                         {
                             lon.AutoId,
                             empId = e.EmployeeId ?? "",
                             loanId = lon.LoanId ?? "",
                             loanDate = lon.LoanDate,
                             showLoanDate = lon.LoanDate.HasValue ? lon.LoanDate.Value.ToString("dd/MM/yyyy") : "",
                             loanTypeName = lType.LoanType ?? "",
                             loanTypeId = lType.LoanTypeId ?? "",
                             empName = (e != null ? e.FirstName + " " + e.LastName : ""),
                             desiName = dg.DesignationName ?? "",
                             loanAmount = lon.LoanAmount,
                             startDate = lon.StartDate,
                             startShowDate = lon.StartDate.HasValue ? lon.StartDate.Value.ToString("dd/MM/yyyy") : "",
                             endDate = lon.EndDate,
                             endShowDate = lon.EndDate.HasValue ? lon.EndDate.Value.ToString("dd/MM/yyyy") : "",
                             noOfInstallments = lon.NoOfInstallment ?? "",
                             monthlyDeduction = lon.MonthlyDeduction,
                             paymentModeId = lon.PaymentModeId ?? "",
                             paymentMode = pType.PaymentModeName ?? "",
                             chequeNo = lon.ChequeNo ?? "",
                             chequeDate = lon.ChequeDate,
                             bankId = lon.BankId ?? "",
                             bankAccount = lon.BankAccount ?? "",
                             remarks = lon.Remarks ?? "",
                             companyCode = lon.CompanyCode ?? "",
                             dpName = dp.DepartmentName ?? "",
                             joiningDate = eoi.JoiningDate.HasValue ? eoi.JoiningDate.Value.ToString("dd/MM/yyyy") : "",
                             payHeadId = lon.PayHeadNameId ?? "",
                             createDate = lon.Ldate.HasValue ? lon.Ldate.Value.ToString("dd/MM/yyyy") : "",
                             updateDate = lon.ModifyDate.HasValue ? lon.ModifyDate.Value.ToString("dd/MM/yyyy") : "",
                             reasonOfLoanTaken = lon.ReasonOfLoanTaken ?? "",
                         };

            var loanDataList = queary.Select(x => new HRMPayrollLoanSetupViewModel
            {
                AutoId = x.AutoId,
                EmployeeId = x.empId,
                LoanId = x.loanId,
                LoanDate = x.loanDate,
                ShowLoanDate = x.showLoanDate,
                LoanTypeId = x.loanTypeId,
                LoanTypeName = x.loanTypeName,
                EmpName = x.empName,
                DesignationName = x.desiName,
                LoanAmount = x.loanAmount,
                StartDate = x.startDate,
                StartShowDate = x.startShowDate,
                EndDate = x.endDate,
                EndShowDate = x.endShowDate,
                NoOfInstallment = x.noOfInstallments,
                MonthlyDeduction = x.monthlyDeduction,
                PaymentModeId = x.paymentModeId,
                PaymentModeName = x.paymentMode,
                ChequeNo = x.chequeNo,
                ChequeDate = x.chequeDate,
                BankId = x.bankId,
                BankAccount = x.bankAccount,
                Remarks = x.remarks,
                CompanyCode = x.companyCode,
                DepartmentName = x.dpName,
                ShowJoiningDate = x.joiningDate,
                PayHeadNameId = x.payHeadId,
                showCreateDate = x.createDate,
                showModifyDate = x.updateDate,
                ReasonOfLoanTaken = x.reasonOfLoanTaken,
            }).ToList();

            return loanDataList.OrderByDescending(x => x.AutoId).ToList();
        }



        public async Task<(bool isSuccess, string message)> deleteLoanAsync(List<decimal> autoIds, DeleteHistoryViewModel dModel)
        {
            await payrollLoanRepo.BeginTransactionAsync();
            try
            {
                if (autoIds == null || autoIds.Count == 0)
                {
                    return (false, DeleteFailed);
                }

                var loansToDelete = await payrollLoanRepo.All().AsNoTracking()
                    .Where(x => autoIds.Contains(x.AutoId))
                    .ToListAsync();

                if (loansToDelete == null || loansToDelete.Count == 0)
                {
                    await payrollLoanRepo.RollbackTransactionAsync();
                    return (false, DeleteFailed);
                }

                var loanIds = loansToDelete.Select(x => x.LoanId).ToList();

                var paymentsToDelete = await paymentRepo.All().AsNoTracking()
                    .Where(x => loanIds.Contains(x.LoanId))
                    .ToListAsync();

                if (paymentsToDelete.Any())
                {
                    await paymentRepo.DeleteRangeAsync(paymentsToDelete);
                    dModel.tableName = paymentRepo.GetTableName();
                    await deleteHistoryService.LogDeletedRecordsAsync(paymentsToDelete, dModel);
                }

                await payrollLoanRepo.DeleteRangeAsync(loansToDelete);
                dModel.tableName = payrollLoanRepo.GetTableName();
                await deleteHistoryService.LogDeletedRecordsAsync(loansToDelete, dModel);

                await payrollLoanRepo.CommitTransactionAsync();
                return (true, DeleteSuccess);
            }
            catch (Exception)
            {
                await payrollLoanRepo.RollbackTransactionAsync();
                throw;
            }
        }


        public async Task<(bool isSuccess, string message)> deletePaymentReceiveAsync(List<decimal> autoIds, DeleteHistoryViewModel dModel)
        {
            await paymentRepo.BeginTransactionAsync();
            try
            {
                if (autoIds == null || autoIds.Count == 0)
                {
                    return (false, DeleteFailed);
                }

                var paymentsToDelete = await paymentRepo.All().AsNoTracking()
                    .Where(x => autoIds.Contains(x.AutoId))
                    .ToListAsync();

                if (paymentsToDelete == null || paymentsToDelete.Count == 0)
                {
                    await paymentRepo.RollbackTransactionAsync();
                    return (false, DeleteFailed);
                }

                // ✅ bulk delete
                await paymentRepo.DeleteRangeAsync(paymentsToDelete);
                dModel.tableName = paymentRepo.GetTableName();
                await deleteHistoryService.LogDeletedRecordsAsync(paymentsToDelete, dModel);

                await paymentRepo.CommitTransactionAsync();
                return (true, DeleteSuccess);
            }
            catch (Exception)
            {
                await paymentRepo.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<HRMPayrollLoanSetupViewModel> getLoanIdAsync(string loanId)
        {
            try
            {
                var LoanData = payrollLoanRepo.All().AsNoTracking().FirstOrDefault(x => x.LoanId == loanId);
                if (LoanData == null)
                    return null;

                var paymentDataList = await paymentRepo.All().AsNoTracking()
                    .Where(x => x.LoanId == loanId)
                    .ToListAsync();

                var paymentDataLast = await paymentRepo.All().AsNoTracking()
                    .Where(x => x.LoanId == loanId)
                    .OrderByDescending(x => x.PaymentDate)
                    .FirstOrDefaultAsync(); // ✅ LastOrDefaultAsync → FirstOrDefaultAsync

                int totalPaymentAmount = 0;
                int monthlyDeduction = LoanData.MonthlyDeduction.HasValue ? Convert.ToInt32(LoanData.MonthlyDeduction) : 0;
                int remaingPaymentAmount = monthlyDeduction;

                foreach (var item in paymentDataList)
                {
                    if (item.PaymentAmount.HasValue)
                        totalPaymentAmount += Convert.ToInt32(item.PaymentAmount);
                }

                if (totalPaymentAmount + monthlyDeduction > (LoanData.LoanAmount ?? 0))
                {
                    remaingPaymentAmount = Convert.ToInt32(LoanData.LoanAmount ?? 0) - totalPaymentAmount;
                }

                var loanTypeName = payTypeRepo.All().AsNoTracking()
                    .Where(x => x.LoanTypeId == LoanData.LoanTypeId)
                    .Select(x => x.LoanType)
                    .FirstOrDefault();

                // Manual mapping
                var result = new HRMPayrollLoanSetupViewModel
                {
                    LoanId = LoanData.LoanId,
                    EmployeeId = LoanData.EmployeeId,
                    LoanAmount = LoanData.LoanAmount ?? 0,
                    NoOfInstallment = LoanData.NoOfInstallment,
                    MonthlyDeduction = LoanData.MonthlyDeduction ?? 0,
                    ShowLoanDate = LoanData.LoanDate.HasValue ? LoanData.LoanDate.Value.ToString("dd/MM/yyyy") : "",
                    LoanTypeName = loanTypeName ?? "",
                    StartShowDate = LoanData.StartDate.HasValue ? LoanData.StartDate.Value.ToString("dd/MM/yyyy") : "",
                    EndShowDate = LoanData.EndDate.HasValue ? LoanData.EndDate.Value.ToString("dd/MM/yyyy") : "",
                    paymentLoanAmount = remaingPaymentAmount,
                    paymentLoanDate = paymentDataLast?.PaymentDate
                };
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in getLoanIdAsync: " + ex.Message, ex);
            }
        }
      


    }
}
