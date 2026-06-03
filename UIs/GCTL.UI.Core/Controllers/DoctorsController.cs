using GCTL.Core.Data;
using GCTL.Core.ViewModels.Doctors;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.Doctors;
using GCTL.UI.Core.ViewModels.Doctors;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GCTL.Core.Helpers;
using System.Globalization;

namespace GCTL.UI.Core.Controllers
{
    public class DoctorsController : BaseController
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
        private readonly IRepository<HrmDefDesignation> designationRepository;

      

        private readonly IRepository<HmsDoctorWorkingPlace> doctorWorkingPlaceRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IMapper mapper;
        public DoctorsController(IDoctorService doctorService,
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
                                 IRepository<HmsDoctorWorkingPlace> doctorWorkingPlaceRepository,

                                

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
            this.webHostEnvironment = webHostEnvironment;
            this.designationRepository = designationRepository;
            this.doctorWorkingPlaceRepository = doctorWorkingPlaceRepository;
           
            this.mapper = mapper;
        }

        public ActionResult Index()
        {
            DoctorPageViewModel model = new DoctorPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index)),
                AddUrl = Url.Action(nameof(Setup))
            };

            ViewBag.DoctorType = new SelectList(doctorTypeRepository.All(), "DoctorTypeCode", "DoctorTypeName");
            ViewBag.Department = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName");
            ViewBag.Designation = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName");
            ViewBag.Speciality = new SelectList(specialityRepository.All(), "SpecialityCode", "SpecialityName");
            ViewBag.Qualification = new SelectList(qualificationRepository.All(), "QualificationCode", "QualificationName");
            return View(model);
        }

        public ActionResult Details(string id)
        {
            DoctorSetupViewModel model = new DoctorSetupViewModel();
            var result = doctorService.GetDoctor(id);
            if (result == null)
            {
                return NotFound();
            }

            model = mapper.Map<DoctorSetupViewModel>(result);
            model.Code = id;

            ViewBag.DoctorType = new SelectList(doctorTypeRepository.All(), "DoctorTypeCode", "DoctorTypeName", model.DoctorTypeCode);
            ViewBag.BloodGroup = new SelectList(bloodRepository.All(), "BloodGroupCode", "BloodGroup", model.BloodGroupCode);
            ViewBag.Sex = new SelectList(sexRepository.All(), "SexCode", "Sex", model.SexCode);
            ViewBag.Religion = new SelectList(religionRepository.All(), "ReligionCode", "Religion", model.ReligionCode);
            ViewBag.Department = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName", model.DepartmentCode);
            ViewBag.Designation = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName", model.DesignationCode);
            ViewBag.Speciality = new SelectList(specialityRepository.All(), "SpecialityCode", "SpecialityName", model.SpecialityCode);
            ViewBag.Qualification = new SelectList(qualificationRepository.All(), "QualificationCode", "QualificationName", model.QualificationCode);
            ViewBag.Shift = new SelectList(shiftRepository.All(), "ShiftCode", "ShiftName", model.ShiftCode);

            return View(model);
        }

        public ActionResult Setup(string id)
        {
            DoctorSetupViewModel model = new DoctorSetupViewModel();
            var result = doctorService.GetDoctor(id);
            if (result != null)
            {
                model = mapper.Map<DoctorSetupViewModel>(result);
                model.Code = id;
            }
            else
            {
                model.DoctorCode = commonService.NextCode("DoctorCode", "HMS_Doctor", 4);
            }

            model.PageUrl = Url.Action(nameof(Index));

            var appointments = doctorAppointmentRepository.All().Where(x => x.DoctorCode == model.DoctorCode);
            foreach (var appointment in appointments)
            {
                string timeFrom = "";
                string timeTo = "";

                if (appointment.VisitingTimeFrom.HasValue)
                {
                    timeFrom = appointment.VisitingTimeFrom.Value.ToString("hh:mm tt");
                }
                if (appointment.VisitingTimeTo.HasValue)
                {
                    timeTo = appointment.VisitingTimeTo.Value.ToString("hh:mm tt");
                }

                model.Appointments.Add(new DoctorAppointment
                {
                    DoctorCode = appointment.DoctorCode,
                    AppointmentDays = appointment.AppointmentDays,
                    VisitingTimeFrom = timeFrom,
                    VisitingTimeTo = timeTo
                });
            }
            var working = new SelectList(doctorWorkingPlaceRepository.All(), "WorkingPlaceCode", "WorkingPlaceName", model.WorkingPlaceCode);
            ViewBag.workingPlaceCode = working;
            ViewBag.DoctorTypes = new SelectList(doctorTypeRepository.All(), "DoctorTypeCode", "DoctorTypeName", model.DoctorTypeCode);
            ViewBag.BloodGroups = new SelectList(bloodRepository.All(), "BloodGroupCode", "BloodGroup", model.BloodGroupCode);
            ViewBag.Sexes = new SelectList(sexRepository.All(), "SexCode", "Sex", model.SexCode);
            ViewBag.Religions = new SelectList(religionRepository.All(), "ReligionCode", "Religion", model.ReligionCode);
            ViewBag.Departments = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName", model.DepartmentCode);
            ViewBag.Designations = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName", model.DesignationCode);
            ViewBag.Specialities = new SelectList(specialityRepository.All(), "SpecialityCode", "SpecialityName", model.SpecialityCode);
            ViewBag.Qualifications = new SelectList(qualificationRepository.All(), "QualificationCode", "QualificationName", model.QualificationCode);
            ViewBag.Shifts = new SelectList(shiftRepository.All(), "ShiftCode", "ShiftName", model.ShiftCode);
            ViewBag.ActivityStatuses = new SelectList(new string[] { "Active", "Inactive" }, model.ActivityStatus);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(DoctorSetupViewModel model)
        {
            if (doctorService.IsDoctorExist(model.DoctorName, model.DoctorCode))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                if (doctorService.IsDoctorExistByCode(model.DoctorCode))
                {
                    var hasPermission = doctorService.UpdatePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HmsDoctor doctor = doctorService.GetDoctor(model.DoctorCode) ?? new HmsDoctor();
                        DateTime dateOfBirth = model.DateOfBirth.ToDate();
                        DateTime joiningDate = model.JoiningDate.ToDate();

                        model.ToAudit(LoginInfo);
                        mapper.Map(model, doctor);
                        doctor.DateOfBirth = dateOfBirth;
                        doctor.JoiningDate = joiningDate;
                        string directory = @"Uploads/Images/Doctors";

                        if (model.IsClearPhoto)
                        {
                            doctor.PhotoUrl = string.Empty;
                            model.Photo = null;
                        }

                        if (model.IsClearSignature)
                        {
                            doctor.DigitalSignatureUrl = string.Empty;
                            model.Signature = null;
                        }

                        string fileName = string.Empty;
                        if (model.Photo != null && model.Photo.Length > 0)
                        {
                            string path = $"{webHostEnvironment.WebRootPath}/{directory}";
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);

                            fileName = model.DoctorCode + Path.GetExtension(model.Photo.FileName);
                            using (var stream = new FileStream($"{path}/{fileName}", FileMode.Create))
                            {
                                await model.Photo.CopyToAsync(stream);
                            }

                            doctor.PhotoUrl = fileName;
                        }
                        else
                        {
                            fileName = "~/images/images.png";
                        }

                        if (model.Signature != null && model.Signature.Length > 0)
                        {
                            string path = $"{webHostEnvironment.WebRootPath}/{directory}";
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);

                            fileName = $"{model.DoctorCode}_Signature {Path.GetExtension(model.Signature.FileName)}";
                            using (var stream = new FileStream($"{path}/{fileName}", FileMode.Create))
                            {
                                await model.Signature.CopyToAsync(stream);
                            }

                            doctor.DigitalSignatureUrl = fileName;
                        }
                        else
                        {
                            fileName = "~/images/images.png";
                        }

                        doctorService.SaveDoctor(doctor);

                        // Appointments
                        if (model.Appointments.Any())
                        {
                            var appointments = doctorAppointmentRepository.All().Where(x => x.DoctorCode == doctor.DoctorCode).ToList();
                            if (appointments.Any())
                            {
                                foreach (var appointment in appointments)
                                {
                                    doctorAppointmentRepository.Delete(appointment);      
                                }
                            }

                            foreach (var appointment in model.Appointments)
                            {
                                string appCode = string.Empty;
                                commonService.FindMaxNo(ref appCode, "AppointmentCode", "HMS_DoctorAppointmentDays", 3);
                                appointment.ToAudit(LoginInfo);

                                TimeOnly timeFrom = TimeOnly.ParseExact(appointment.VisitingTimeFrom,
                                          "hh:mm tt", CultureInfo.InvariantCulture);

                                TimeOnly timeTo = TimeOnly.ParseExact(appointment.VisitingTimeTo,
                                            "hh:mm tt", CultureInfo.InvariantCulture);

                                var doctorAppointment = new HmsDoctorAppointmentDays
                                {
                                    DoctorCode = doctor.DoctorCode,
                                    AppointmentCode = appCode,
                                    AppointmentDays = appointment.AppointmentDays,
                                    VisitingTimeFrom = timeFrom,
                                    VisitingTimeTo = timeTo,
                                    Ldate = appointment.Ldate,
                                    Lip = appointment.Lip,
                                    Lmac = appointment.Lmac,
                                    Luser = appointment.Luser,
                                    ModifyDate = appointment.ModifyDate
                                };

                                doctorAppointmentRepository.Add(doctorAppointment);
                            }
                        }

                        return RedirectToAction("Index", "Doctors");
                    }
                    else
                    {

                        return Json(new { isSuccess = false, message = "You have no access" });
                    }

                }
                else
                {
                    var hasPermission = doctorService.SavePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HmsDoctor doctor = doctorService.GetDoctor(model.DoctorCode) ?? new HmsDoctor();
                        DateTime dateOfBirth = model.DateOfBirth.ToDate();
                        DateTime joiningDate = model.JoiningDate.ToDate();

                        model.ToAudit(LoginInfo);
                        mapper.Map(model, doctor);
                        doctor.DateOfBirth = dateOfBirth;
                        doctor.JoiningDate = joiningDate;
                        string directory = @"Uploads/Images/Doctors";

                        if (model.IsClearPhoto)
                        {
                            doctor.PhotoUrl = string.Empty;
                            model.Photo = null;
                        }

                        if (model.IsClearSignature)
                        {
                            doctor.DigitalSignatureUrl = string.Empty;
                            model.Signature = null;
                        }

                        string fileName = string.Empty;
                        if (model.Photo != null && model.Photo.Length > 0)
                        {
                            string path = $"{webHostEnvironment.WebRootPath}/{directory}";
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);

                            fileName = model.DoctorCode + Path.GetExtension(model.Photo.FileName);
                            using (var stream = new FileStream($"{path}/{fileName}", FileMode.Create))
                            {
                                await model.Photo.CopyToAsync(stream);
                            }

                            doctor.PhotoUrl = fileName;
                        }
                        else
                        {
                            fileName = "~/images/images.png";
                        }

                        if (model.Signature != null && model.Signature.Length > 0)
                        {
                            string path = $"{webHostEnvironment.WebRootPath}/{directory}";
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);

                            fileName = $"{model.DoctorCode}_Signature {Path.GetExtension(model.Signature.FileName)}";
                            using (var stream = new FileStream($"{path}/{fileName}", FileMode.Create))
                            {
                                await model.Signature.CopyToAsync(stream);
                            }

                            doctor.DigitalSignatureUrl = fileName;
                        }
                        else
                        {
                            fileName = "~/images/images.png";
                        }

                        doctorService.SaveDoctor(doctor);

                        // Appointments
                        if (model.Appointments.Any())
                        {
                            var appointments = doctorAppointmentRepository.All().Where(x => x.DoctorCode == doctor.DoctorCode).ToList();
                            if (appointments.Any())
                            {
                                foreach (var appointment in appointments)
                                {
                                    doctorAppointmentRepository.Delete(appointment);
                                }
                            }

                            foreach (var appointment in model.Appointments)
                            {
                                string appCode = string.Empty;
                                commonService.FindMaxNo(ref appCode, "AppointmentCode", "HMS_DoctorAppointmentDays", 3);
                                appointment.ToAudit(LoginInfo);

                                TimeOnly timeFrom = TimeOnly.ParseExact(appointment.VisitingTimeFrom,
                                          "hh:mm tt", CultureInfo.InvariantCulture);

                                TimeOnly timeTo = TimeOnly.ParseExact(appointment.VisitingTimeTo,
                                            "hh:mm tt", CultureInfo.InvariantCulture);

                                var doctorAppointment = new HmsDoctorAppointmentDays
                                {
                                    DoctorCode = doctor.DoctorCode,
                                    AppointmentCode = appCode,
                                    AppointmentDays = appointment.AppointmentDays,
                                    VisitingTimeFrom = timeFrom,
                                    VisitingTimeTo = timeTo,
                                    Ldate = appointment.Ldate,
                                    Lip = appointment.Lip,
                                    Lmac = appointment.Lmac,
                                    Luser = appointment.Luser,
                                    ModifyDate = appointment.ModifyDate
                                };

                                doctorAppointmentRepository.Add(doctorAppointment);
                            }
                        }

                        return RedirectToAction("Index", "Doctors");
                    }
                    else
                    {

                        return Json(new { isSuccess = false, message = "You have no access" });
                    }
                }



            }

            ViewBag.DoctorTypes = new SelectList(doctorTypeRepository.All(), "DoctorTypeCode", "DoctorTypeName", model.DoctorTypeCode);
            ViewBag.BloodGroups = new SelectList(bloodRepository.All(), "BloodGroupCode", "BloodGroup", model.BloodGroupCode);
            ViewBag.Sexes = new SelectList(sexRepository.All(), "SexCode", "Sex", model.SexCode);
            ViewBag.Religions = new SelectList(religionRepository.All(), "ReligionCode", "Religion", model.ReligionCode);
            ViewBag.Departments = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName", model.DepartmentCode);
            ViewBag.Designations = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName", model.DesignationCode);
            ViewBag.Specialities = new SelectList(specialityRepository.All(), "SpecialityCode", "SpecialityName", model.SpecialityCode);
            ViewBag.Qualifications = new SelectList(qualificationRepository.All(), "QualificationCode", "QualificationName", model.QualificationCode);
            ViewBag.Shifts = new SelectList(shiftRepository.All(), "ShiftCode", "ShiftName", model.ShiftCode);
            ViewBag.ActivityStatuses = new SelectList(new string[] { "Active", "Inactive" }, model.ActivityStatus);

            return View(model);
        }

        public ActionResult Grid(string doctorTypeCode, string departmentCode, string specialityCode, string qualificationCode)
        {
            var result = doctorService.GetDoctors(doctorTypeCode, departmentCode, specialityCode, qualificationCode);
            return Json(new { data = result });
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            bool success = false;
            var hasPermission = doctorService.DeletePermission(LoginInfo.AccessCode);
            if (hasPermission)
            {
               
                foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    var response = doctorService.DeleteDoctor(item);
                    return Json(response);
                }
                return Json(new { success = success, message = "Deleted Successfully" });
            }
            else
            {
                return Json(new { success = success, message = "You have No access" });
            }

           
        }



        [HttpPost]
        public JsonResult CheckAvailability(string name, string code)
        {
            if (doctorService.IsDoctorExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }
    }
}