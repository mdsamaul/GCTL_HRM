using GCTL.Core.Data;
using GCTL.Core.ViewModels.SalaryInformationReport;
using GCTL.Data.Models;
using GCTL.Service.SalaryInformationReport;
using GCTL.UI.Core.ViewModels.SalaryInformationReport;
using GCTL.UI.Core.ViewModels.SalesDefVehicle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
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

        // IWebHostEnvironment injection GCTL.Service.SalaryInformationReport.SalaryInformationReportFilterService
        // er nijer constructor e already hoy (DI container automatic kore dey) —
        // controller theke seta pass korar দরকার nai, শুধু ISalaryInformationReport
        // inject korলেই cholbe.
        public SalaryInformationReportController(ISalaryInformationReportService salaryInformationReportService ,
            IRepository<HrmPayMonth> monthRepo,
            IRepository<HrmEisDefDisbursementMethod> mopRepo,
            IRepository<HrmPayDefPayrollMasterFileType> masterRepo,
            IRepository<HrmEisDefEmploymentNature> employmentNatureRepo
            )
        {
            _salaryInformationReportService = salaryInformationReportService;
            this.monthRepo = monthRepo;
            this.mopRepo = mopRepo;
            this.masterRepo = masterRepo;
            this.employmentNatureRepo = employmentNatureRepo;

        }

        public IActionResult Index()
        {
        
            SalaryInformationReportViewModel model = new SalaryInformationReportViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            ViewBag.MonthList = new SelectList(monthRepo.All().Select(e => new { id = e.MonthId, name = e.MonthName }).ToList(), "id", "name");
            ViewBag.EmploymentNatureList = new SelectList(employmentNatureRepo.All().Select(e => new { id = e.EmploymentNatureId, name = e.EmploymentNature }).ToList(), "id", "name");
            ViewBag.mopRepoList = new SelectList(mopRepo.All().Select(e => new { id = e.ShortName, name = e.DisbursementMethod }).ToList(), "id", "name");
            ViewBag.masterRepoList = new SelectList(masterRepo.All().Select(e => new { id = e.PmftId, name = e.PayrollMasterFileType }).ToList(), "id", "name");
            ViewBag.AccessCode = LoginInfo.AccessCode;
            return View(model);
        }

        // Grid/table data load (AJAX)
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

        // Excel export (EPPlus, server-side, with logo)
        [HttpPost]
        public async Task<IActionResult> ExportToExcel([FromBody] SalaryInformationReportFilterDto filter)
        {
            try
            {
                if (filter != null && filter.MasterFileType == "01")
                {
                    var fileBytes = await _salaryInformationReportService.ExportToExcelAsync(filter);
                    string fileName = $"PayrollMasterFile_General_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    return File(
                        fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }
                return BadRequest("Invalid filter data.");
            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}