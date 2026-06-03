using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.HRLettersReportViewModel;
using GCTL.Data.Models;
using GCTL.Service.HRLettersReport;
using GCTL.UI.Core.ViewModels.HRLettersReportSetupViewModel;
using GCTL.UI.Core.ViewModels.HRM_Def_Floor;
using GCTL.UI.Core.ViewModels.HrmAtdShifts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class HRLettersReportController : BaseController
    {
        private readonly IHRLettersReportService hRLettersReportService;
        private readonly IRepository<HrmDefHrletters> hrLettersRepo;
        private readonly IRepository<HrmNocinfo> nocRepo;

        public HRLettersReportController(
            IHRLettersReportService hRLettersReportService,
            IRepository<HrmDefHrletters> hrLettersRepo,
             IRepository<HrmNocinfo> nocRepo
            )
        {
            this.hRLettersReportService = hRLettersReportService;
            this.hrLettersRepo = hrLettersRepo;
            this.nocRepo = nocRepo;
        }
        public async Task<IActionResult> Index()
        {
            var hasPermission = await hRLettersReportService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            ViewBag.hrletters = new SelectList(hrLettersRepo.All().Select(x => new { id = x.HrlettersId, name = x.HrlettersName }).ToList(), "id", "name");
            HRLettersReportSetupViewModel model = new HRLettersReportSetupViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
           
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EmpDetailsAsync([FromBody] string id)
        {
            
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { message = "Employee code is required." });

            var profile = await hRLettersReportService.GetByEmployeeCodeAsync(id);

            if (profile == null)
                return NotFound(new { message = "Employee not found." });

            return Ok(profile);
        }

        // POST: Export → PDF preview or download
        [HttpPost]
        public async Task<IActionResult> Export([FromBody] HRLetterReportRequestViewModel request)
        {
            if (request == null)
                return BadRequest("Invalid request.");

            if (string.IsNullOrEmpty(request.EmployeeCode))
                return BadRequest(new { message = "Employee is required." });

            if (string.IsNullOrEmpty(request.HrLetterTypeId))
                return BadRequest(new { message = "HR Letter Type is required." });

            try
            {
                var tolist = request.ToAudit(LoginInfo);


                string nocId = nocRepo.All().Where(x => x.EmployeeId == request.EmployeeCode).Select(s => s.Nocid).FirstOrDefault();
                var refNo = await hRLettersReportService.SaveOrUpdateLetterAsync(new SaveOrUpdateLetterRequestDto
                {
                    EmployeeId = request.EmployeeCode,
                    LetterTypeId = request.HrLetterTypeId,
                    EntryUserEmployeeId = request.SignatoryEmployeeCode,
                    CompanyCode = LoginInfo.CompanyCode,
                    NocId = nocId??"",
                    Luser = tolist.Luser,
                    Lip = tolist.Lip,
                    Lmac = tolist.Lmac,
                    Ldate = tolist.Ldate,
                    ModifyDate = tolist.ModifyDate,
                    AppliedDate = request.AppliedDate,
                });
               
                request.LetterRefNo = refNo;

                var pdfBytes = await hRLettersReportService.GeneratePdfAsync(request);

                //var disposition = request.IsPreview ? "inline" : "attachment";
                ////var fileName = $"HRLetter_{request.EmployeeCode}_{DateTime.Now:yyyyMMdd}.pdf";
                //var letterTypeNames = new Dictionary<string, string>
                //    {
                //        { "005", "Termination_Letter" },
                //        { "010", "NOC_Travel" },
                //        { "019", "Internship_Offer_Letter" },
                //        { "014", "Internship_Certificate" },
                //        { "018", "Recommendation_Letter" },
                //        { "016", "NOC_Education" },
                //        { "017", "NOC_General" }
                //    };

                //string letterTypeName = letterTypeNames.TryGetValue(request.HrLetterTypeId, out var name)
                //    ? name
                //    : "Discharge_Certificate";

                //var fileName = $"{letterTypeName}_{request.EmployeeCode}_{DateTime.Now:yyyyMMdd}.pdf";

                //Response.Headers["Content-Disposition"] = $"{disposition}; filename={fileName}";
                //return File(pdfBytes, "application/pdf");

                var disposition = request.IsPreview ? "inline" : "attachment";
                Response.Headers["Content-Disposition"] = $"{disposition}";
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeesByLetterType(string letterTypeId)
        {
           
            if (!string.IsNullOrEmpty(letterTypeId))
            {
                var result = await hRLettersReportService.GetEmployeesByLetterTypeAsync(
                letterTypeId, LoginInfo.CompanyCode);

                return Ok(result);
            }
            else
            {
                return Ok(new List<EmployeeByLetterTypeDto>());
            }
            
        }
    }
}
