using GCTL.Core.Data;
using GCTL.Core.ViewModels.Doctors;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.Doctors;
using GCTL.UI.Core.ViewModels.Doctors;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GCTL.Core.ViewModels.Companies;
using GCTL.Service.Companies;
using GCTL.Service.Reports;
using GCTL.UI.Core.Helpers.Reports;

namespace GCTL.UI.Core.Controllers
{
    public class DoctorReportsController : BaseController
    {
        private readonly IDoctorService doctorService;
        private readonly ICommonService commonService;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefBloodGroup> bloodRepository;
        private readonly IRepository<HrmDefSex> sexRepository;
        private readonly IRepository<HrmDefReligion> religionRepository;
        private readonly IRepository<HmsDoctorType> doctorTypeRepository;
        private readonly IRepository<HmsSpeciality> specialityRepository;
        private readonly IRepository<HmsQualification> qualificationRepository;
        private readonly IRepository<HmsShift> shiftRepository;
        private readonly IRepository<HmsDoctorAppointmentDays> doctorAppointmentRepository;
        private readonly ICompanyService companyService;
        private readonly IReportService reportService;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IMapper mapper;
        public DoctorReportsController(IDoctorService doctorService,
                                 ICommonService commonService,
                                 IRepository<HrmDefDepartment> departmentRepository,
                                 IRepository<HrmDefDesignation> designationRepository,
                                 IRepository<HrmDefBloodGroup> bloodRepository,
                                 IRepository<HrmDefSex> sexRepository,
                                 IRepository<HrmDefReligion> religionRepository,
                                 IRepository<HmsDoctorType> doctorTypeRepository,
                                 IRepository<HmsSpeciality> specialityRepository,
                                 IRepository<HmsQualification> qualificationRepository,
                                 IRepository<HmsShift> shiftRepository,
                                 IRepository<HmsDoctorAppointmentDays> doctorAppointmentRepository,
                                 ICompanyService companyService,
                                 IReportService reportService,
                                 IWebHostEnvironment webHostEnvironment,
                                 IMapper mapper)
        {
            this.doctorService = doctorService;
            this.commonService = commonService;
            this.departmentRepository = departmentRepository;
            this.bloodRepository = bloodRepository;
            this.sexRepository = sexRepository;
            this.religionRepository = religionRepository;
            this.doctorTypeRepository = doctorTypeRepository;
            this.specialityRepository = specialityRepository;
            this.qualificationRepository = qualificationRepository;
            this.shiftRepository = shiftRepository;
            this.doctorAppointmentRepository = doctorAppointmentRepository;
            this.companyService = companyService;
            this.reportService = reportService;
            this.webHostEnvironment = webHostEnvironment;
            this.designationRepository = designationRepository;
            this.mapper = mapper;
        }

        public ActionResult Index()
        {
            DoctorPageViewModel model = new DoctorPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };

            ViewBag.DoctorType = new SelectList(doctorTypeRepository.All(), "DoctorTypeCode", "DoctorTypeName");
            ViewBag.Department = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName");
            ViewBag.Designation = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName");
            ViewBag.Speciality = new SelectList(specialityRepository.All(), "SpecialityCode", "SpecialityName");
            ViewBag.Qualification = new SelectList(qualificationRepository.All(), "QualificationCode", "QualificationName");
            return View(model);
        }


        public ActionResult Reports(DoctorReportRequest request)
        {
            var result = doctorService.GetDoctorReports(request);
            return Json(new { data = result });
        }

        public ActionResult Export(DoctorReportRequest request)
        {
            request.ProcessingRequest(webHostEnvironment);
            request.IsReport = true;
            request.ImagePath = $"{webHostEnvironment.WebRootPath}\\Images";
            var doctors = doctorService.GetDoctorReports(request);

            var sources = new Dictionary<string, System.Collections.IEnumerable>
            {
                { "Company", new List<CompanyModel>() { companyService.GetCompanyByCode("001") } },
                { "Doctors", doctors }
            };
            request.Sources = sources;

            var reportResponse = reportService.GenerateReport(request);
            return File(reportResponse.ReportResult.MainStream, reportResponse.MimeType, reportResponse.FileName + reportResponse.Extension);
        }
    }
}