// GCTL.UI.Core.Controllers/SalaryInformationReportController.cs

using GCTL.Core.Data;
using GCTL.Core.ViewModels.SalaryInformationReport;
using GCTL.Data.Models;
using GCTL.Service.SalaryInformationReport;
using GCTL.UI.Core.ViewModels.SalaryInformationReport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GCTL.UI.Core.Controllers
{
    public class SalaryInformationReportController : BaseController
    {
        private readonly ISalaryInformationReportService _salaryInformationReportService;
        private readonly IRepository<HrmPayMonth> monthRepo;
        private readonly IRepository<HrmEisDefDisbursementMethod> mopRepo;
        private readonly IRepository<HrmPayDefPayrollMasterFileType> masterRepo;
        private readonly IRepository<HrmEisDefEmploymentNature> employmentNatureRepo;

        public SalaryInformationReportController(
            ISalaryInformationReportService salaryInformationReportService,
            IRepository<HrmPayMonth> monthRepo,
            IRepository<HrmEisDefDisbursementMethod> mopRepo,
            IRepository<HrmPayDefPayrollMasterFileType> masterRepo,
            IRepository<HrmEisDefEmploymentNature> employmentNatureRepo)
        {
            _salaryInformationReportService = salaryInformationReportService;
            this.monthRepo = monthRepo;
            this.mopRepo = mopRepo;
            this.masterRepo = masterRepo;
            this.employmentNatureRepo = employmentNatureRepo;
        }

        public IActionResult Index()
        {
            var model = new SalaryInformationReportViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            ViewBag.MonthList = new SelectList(
                monthRepo.All().Select(e => new { id = e.MonthId, name = e.MonthName }).ToList(), "id", "name");

            ViewBag.EmploymentNatureList = new SelectList(
                employmentNatureRepo.All().Select(e => new { id = e.EmploymentNatureId, name = e.EmploymentNature }).ToList(), "id", "name");

            ViewBag.mopRepoList = new SelectList(
                mopRepo.All().Select(e => new { id = e.ShortName, name = e.DisbursementMethod }).ToList(), "id", "name");

            ViewBag.masterRepoList = new SelectList(
                masterRepo.All().Select(e => new { id = e.PmftId, name = e.PayrollMasterFileType }).ToList(), "id", "name");

            ViewBag.AccessCode = LoginInfo.AccessCode;
            return View(model);
        }

        // ===================== GENERAL =====================
        [HttpPost]
        public async Task<IActionResult> GetPayrollMasterFile([FromBody] SalaryInformationReportFilterDto filter)
        {
            try
            {
                var data = await _salaryInformationReportService.GetPayrollMasterFileAsync(filter);
                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel([FromBody] SalaryInformationReportFilterDto filter)
        {
            try
            {
                if (filter == null)
                    return BadRequest("Invalid filter data.");

                var fileBytes = await _salaryInformationReportService.ExportToExcelAsync(filter);
                string fileName = $"PayrollMasterFile_General_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ===================== GRATUITY =====================
        [HttpPost]
        public async Task<IActionResult> GetPayrollMasterFileGratuity([FromBody] SalaryInformationReportFilterDto filter)
        {
            try
            {
                var data = await _salaryInformationReportService.GetPayrollMasterFileGratuityAsync(filter);
                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcelGratuity([FromBody] SalaryInformationReportFilterDto filter)
        {
            try
            {
                if (filter == null)
                    return BadRequest("Invalid filter data.");

                var fileBytes = await _salaryInformationReportService.ExportToExcelGratuityAsync(filter);
                string fileName = $"PayrollMasterFile_Gratuity_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }
        // ===================== YEARLY BONUS =====================
        [HttpPost]
        public async Task<IActionResult> GetPayrollMasterFileYearlyBonus([FromBody] SalaryInformationReportFilterDto filter)
        {
            try
            {
                var data = await _salaryInformationReportService.GetPayrollMasterFileYearlyBonusAsync(filter);
                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcelYearlyBonus([FromBody] SalaryInformationReportFilterDto filter)
        {
            try
            {
                if (filter == null)
                    return BadRequest("Invalid filter data.");

                var fileBytes = await _salaryInformationReportService.ExportToExcelYearlyBonusAsync(filter);
                string fileName = $"YearlyBonus_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}