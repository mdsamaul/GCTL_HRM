using GCTL.Core;
using GCTL.Core.Data;
using GCTL.Core.Messaging;
using GCTL.Core.ViewModels.Doctors;
using GCTL.Data.Models;
using System.Collections;
using System.Text;
using GCTL.Core.ViewModels.Common;

namespace GCTL.Service.Doctors
{
    public class DoctorService : AppService<HmsDoctor>, IDoctorService
    {
        private readonly IRepository<HmsDoctor> doctorRepository;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HmsSpeciality> specialityRepository;
        private readonly IRepository<HmsQualification> qualificationRepository;
        private readonly IRepository<HmsDoctorAppointmentDays> doctorAppointmentRepository;
        private readonly IRepository<HmsPatientRegistration> patientRepository;
        private readonly IRepository<HmsTestEntry> labTestRepository;
        private readonly IRepository<HmsDoctorWorkingPlace> doctorWorkingPlaceServiceRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        public DoctorService(IRepository<HmsDoctor> doctorRepository,
                             IRepository<HrmDefDepartment> departmentRepository,
                             IRepository<HrmDefDesignation> designationRepository,
                             IRepository<HmsSpeciality> specialityRepository,
                             IRepository<HmsQualification> qualificationRepository,
                             IRepository<HmsDoctorAppointmentDays> doctorAppointmentRepository,
                             IRepository<HmsPatientRegistration> patientRepository,
                             IRepository<HmsTestEntry> labTestRepository,
                             IRepository<HmsDoctorWorkingPlace> doctorWorkingPlaceServiceRepository,
            IRepository<CoreAccessCode> accessCodeRepository)
            : base(doctorRepository)
        {
            this.doctorRepository = doctorRepository;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.specialityRepository = specialityRepository;
            this.qualificationRepository = qualificationRepository;
            this.doctorAppointmentRepository = doctorAppointmentRepository;
            this.patientRepository = patientRepository;
            this.labTestRepository = labTestRepository;
            this.doctorWorkingPlaceServiceRepository = doctorWorkingPlaceServiceRepository;
            this.accessCodeRepository = accessCodeRepository;
        }

        public List<DoctorViewModel> GetDoctors(string doctorTypeCode, string departmentCode, string specialityCode, string qualificationCode)
        {
            var data = (from doctor in doctorRepository.All()
                        join department in departmentRepository.All()
                        on doctor.DepartmentCode equals department.DepartmentCode into doctorDepartment
                        from department in doctorDepartment.DefaultIfEmpty()
                        join designation in designationRepository.All()
                        on doctor.DesignationCode equals designation.DesignationCode into doctorDesignation
                        from designation in doctorDesignation.DefaultIfEmpty()
                        join speciality in specialityRepository.All()
                        on doctor.SpecialityCode equals speciality.SpecialityCode into doctorSpeciality
                        from speciality in doctorSpeciality.DefaultIfEmpty()
                        join qualification in qualificationRepository.All()
                        on doctor.QualificationCode equals qualification.QualificationCode into doctorQualification
                        from qualification in doctorQualification.DefaultIfEmpty()
                        orderby doctor.DoctorId descending
                        where (doctorTypeCode == null || doctor.DoctorTypeCode == doctorTypeCode)
                        && (departmentCode == null || doctor.DepartmentCode == departmentCode)
                        && (specialityCode == null || doctor.SpecialityCode == specialityCode)
                        && (qualificationCode == null || doctor.QualificationCode == qualificationCode)
                        select new DoctorViewModel
                        {
                            DoctorCode = doctor.DoctorCode,
                            DoctorName = doctor.DoctorName,
                            DepartmentName = department.DepartmentName,
                            DesignationName = designation.DesignationName,
                            Specialist = speciality.SpecialityName,
                            Qualification = qualification.QualificationName,
                            VisitingTime = doctor.VisitingTime.ToString(),
                            VisitingFee = doctor.VisitingFee.ToString(),
                            Phone = doctor.Phone,
                            Email = doctor.Email,
                            PhotoUrl = doctor.PhotoUrl,
                            ActivityStatus = doctor.ActivityStatus,
                            AppointmentDays = "",
                            BanglaDoctorName=doctor.BanglaDoctorName
                        }).ToList();

            foreach (var item in data)
            {
                var appointments = doctorAppointmentRepository.All().Where(x => x.DoctorCode == item.DoctorCode);
                if (appointments.Any())
                {
                    StringBuilder appBuilder = new StringBuilder();
                    StringBuilder dayBuilder = new StringBuilder();
                    StringBuilder timeBuilder = new StringBuilder();
                    string timeFromFormatted = string.Empty;
                    string timeToFormatted = string.Empty;
                    List<string> days = new List<string>();
                    List<string> formattedDays = new List<string>();
                    List<string> formattedTimes = new List<string>();
                    List<string> formattedAppointments = new List<string>();

                    foreach (var app in appointments)
                    {
                        formattedDays = new List<string>();
                        if (app.VisitingTimeFrom.HasValue)
                        {
                            timeFromFormatted = app.VisitingTimeFrom.Value.ToString("hh:mm tt");
                        }
                        if (app.VisitingTimeTo.HasValue)
                        {
                            timeToFormatted = app.VisitingTimeTo.Value.ToString("hh:mm tt");
                        }

                        days = new List<string>();
                        foreach (var day in app.AppointmentDays.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            days.Add(day.Substring(0, 3));
                        }
                        //days = new List<string>();
                        //days.AddRange(app.AppointmentDays.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

                        if (days.Count > 1)
                        {
                            formattedDays.Add($"{days.FirstOrDefault()}-{days.LastOrDefault()}");
                        }
                        else
                        {
                            formattedDays.Add($"{days.FirstOrDefault()}");
                        }


                        formattedAppointments.Add($"{formattedDays.FirstOrDefault()} ({timeFromFormatted}-{timeToFormatted})");

                        // formattedTimes.Add($"{timeFromFormatted}-{timeToFormatted}");
                    }


                    dayBuilder.AppendLine(string.Join(",<br/>", formattedAppointments));
                    // dayBuilder.AppendLine(string.Join(",<br/>", formattedDays));
                    // timeBuilder.AppendLine(string.Join(",<br/>", formattedTimes));


                    item.AppointmentDays = dayBuilder.ToString();
                    // item.AppointmentDays = dayBuilder.ToString();
                    // item.VisitingTime = timeBuilder.ToString();
                }
                else
                {
                    item.AppointmentDays = string.Empty;
                    item.VisitingTime = string.Empty;
                }
            }

            return data;
        }

        public HmsDoctor GetDoctor(string code)
        {
            return doctorRepository.GetById(code);
        }


        public DoctorViewModel GetDoctorByCode(string doctorCode)
        {
            var data = (from doctor in doctorRepository.All()
                        join department in departmentRepository.All()
                        on doctor.DepartmentCode equals department.DepartmentCode into doctorDepartment
                        from department in doctorDepartment.DefaultIfEmpty()
                        join designation in designationRepository.All()
                        on doctor.DesignationCode equals designation.DesignationCode into doctorDesignation
                        from designation in doctorDesignation.DefaultIfEmpty()
                        join speciality in specialityRepository.All()
                        on doctor.SpecialityCode equals speciality.SpecialityCode into doctorSpeciality
                        from speciality in doctorSpeciality.DefaultIfEmpty()
                        join qualification in qualificationRepository.All()
                        on doctor.QualificationCode equals qualification.QualificationCode into doctorQualification
                        from qualification in doctorQualification.DefaultIfEmpty()

                        join dwp in doctorWorkingPlaceServiceRepository.All()
         on doctor.WorkingPlaceCode equals dwp.WorkingPlaceCode into dwpDetails
                        from dwp in dwpDetails.DefaultIfEmpty()

                        where doctor.DoctorCode == doctorCode
                        select new DoctorViewModel
                        {
                            DoctorCode = doctor.DoctorCode,
                            DoctorName = doctor.DoctorName,
                            Phone = doctor.Phone,
                            Email = doctor.Email,
                            Qualification = qualification.QualificationName,
                            DepartmentName = department.DepartmentName,
                            DesignationName = designation.DesignationName,
                            Specialist = speciality.SpecialityName,
                            BanglaDoctorName = doctor.BanglaDoctorName,
                            BanglaDepartment=department.BanglaDepartment,
                            BanglaDesignation=designation.BanglaDesignation,
                            BanglaQualification=qualification.BanglaQualification,
                            BanglaSpeciality= speciality.BanglaSpeciality,
                            WorkingPlaceName=dwp.WorkingPlaceName,
                            BanglaWorkingPlace=dwp.BanglaWorkingPlace,
                            BMDCRegNo =doctor.BmdcregNo

                        }).FirstOrDefault();

            if (data != null)
            {
                var appointments = doctorAppointmentRepository.All().Where(x => x.DoctorCode == data.DoctorCode);
                if (appointments.Any())
                {
                    StringBuilder dayBuilder = new StringBuilder();
                    string timeFromFormatted = string.Empty;
                    string timeToFormatted = string.Empty;
                    List<string> days = new List<string>();
                    List<string> formattedDays = new List<string>();
                    List<string> formattedTimes = new List<string>();
                    List<string> formattedAppointments = new List<string>();

                    foreach (var app in appointments)
                    {
                        formattedDays = new List<string>();
                        if (app.VisitingTimeFrom.HasValue)
                        {
                            timeFromFormatted = app.VisitingTimeFrom.Value.ToString("hh:mm tt");
                        }
                        if (app.VisitingTimeTo.HasValue)
                        {
                            timeToFormatted = app.VisitingTimeTo.Value.ToString("hh:mm tt");
                        }

                        days = new List<string>();
                        days.AddRange(app.AppointmentDays.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

                        if (days.Count > 1)
                        {
                            formattedDays.Add($"{days.FirstOrDefault()}-{days.LastOrDefault()}");
                        }
                        else
                        {
                            formattedDays.Add($"{days.FirstOrDefault()}");
                        }

                        formattedAppointments.Add($"{formattedDays.FirstOrDefault()} ({timeFromFormatted}-{timeToFormatted})");
                    }


                    dayBuilder.AppendLine(string.Join(",<br/>", formattedAppointments));
                    data.AppointmentDays = dayBuilder.ToString();
                }
            }

            return data;
        }

        public HmsDoctor SaveDoctor(HmsDoctor entity)
        {
            if (IsDoctorExistByCode(entity.DoctorCode))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public ApplicationResponse DeleteDoctor(string id)
        {
            ApplicationResponse response = new ApplicationResponse() { IsSuccess = true, Message = "Deleted successfully" };
            var doctor = GetDoctor(id);
            if (doctor != null)
            {
                bool isSuccess = true;
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("You will have to delete doctor information respectively from ");
                var patients = patientRepository.FindBy(x => x.DoctorCode == doctor.DoctorCode);
                if (patients.Any())
                {
                    builder.AppendLine("Patient");
                    isSuccess = false;

                    if (labTestRepository.FindBy(x => patients.Select(p => p.PatientCode).Contains(x.PatientCode)).Any())
                    {
                        builder.AppendLine(", Lab Test");
                        isSuccess = false;
                    }
                }


                if (isSuccess == false)
                {
                    builder.AppendLine("!");
                    response.Message = builder.ToString();
                    response.IsSuccess = isSuccess;
                }
                else
                {
                    response.IsSuccess = isSuccess;
                    doctorRepository.Delete(doctor);
                }
            }

            return response;
        }

        public bool IsDoctorExistByCode(string doctorCode)
        {
            return doctorRepository.All().Any(x => x.DoctorCode == doctorCode);
        }

        public bool IsDoctorExist(string name)
        {
            return doctorRepository.All().Any(x => x.DoctorName == name);
        }

        public bool IsDoctorExist(string name, string doctorCode)
        {
            return doctorRepository.All().Any(x => x.DoctorName == name && x.DoctorCode != doctorCode);
        }

        public IEnumerable GetDoctorSelections()
        {
            return doctorRepository.All()
                  .Select(u => new
                  {
                      Code = u.DoctorCode,
                      Name = $"{u.DoctorName} ({u.DoctorCode})"
                  }).ToList();
        }

        public IEnumerable<CommonSelectModel> LabTestDoctorSelection()
        {
            return (from doctor in doctorRepository.All()
                    select new CommonSelectModel
                    {
                        Code = doctor.DoctorCode,
                        Name = $"{doctor.DoctorName} ({doctor.DoctorCode})"
                    }).Distinct().ToList();
        }

        public IEnumerable GetDoctorSelections(string departmentCode)
        {
            return doctorRepository.All()
                .Where(x => x.DepartmentCode == departmentCode)
                  .Select(u => new
                  {
                      Code = u.DoctorCode,
                      Name = $"{u.DoctorName} ({u.DoctorCode})"
                  }).ToList();
        }

        public List<DoctorReportModel> GetDoctorReports(DoctorReportRequest request)
        {
            var data = (from doctor in doctorRepository.All()
                        join department in departmentRepository.All()
                        on doctor.DepartmentCode equals department.DepartmentCode into doctorDepartment
                        from department in doctorDepartment.DefaultIfEmpty()
                        join designation in designationRepository.All()
                        on doctor.DesignationCode equals designation.DesignationCode into doctorDesignation
                        from designation in doctorDesignation.DefaultIfEmpty()
                        join speciality in specialityRepository.All()
                        on doctor.SpecialityCode equals speciality.SpecialityCode into doctorSpeciality
                        from speciality in doctorSpeciality.DefaultIfEmpty()
                        join qualification in qualificationRepository.All()
                        on doctor.QualificationCode equals qualification.QualificationCode into doctorQualification
                        from qualification in doctorQualification.DefaultIfEmpty()
                        where (request.DoctorTypeCode == null || doctor.DoctorTypeCode == request.DoctorTypeCode)
                        && (request.DepartmentCode == null || doctor.DepartmentCode == request.DepartmentCode)
                        && (request.SpecialityCode == null || doctor.SpecialityCode == request.SpecialityCode)
                        && (request.QualificationCode == null || doctor.QualificationCode == request.QualificationCode)
                        select new DoctorReportModel
                        {
                            DoctorCode = doctor.DoctorCode,
                            DoctorName = doctor.DoctorName,
                            DepartmentName = department.DepartmentName,
                            DesignationName = designation.DesignationName,
                            Specialist = speciality.SpecialityName,
                            Qualification = qualification.QualificationName,
                            VisitingTime = doctor.VisitingTime.ToString(),
                            VisitingFee = doctor.VisitingFee.ToString(),
                            Phone = doctor.Phone,
                            Email = doctor.Email,
                            PhotoUrl = doctor.PhotoUrl,
                            ActivityStatus = doctor.ActivityStatus,
                            AppointmentDays = "",
                            JoiningDate = doctor.JoiningDate.GetValueOrDefault().Year > 1905 ? doctor.JoiningDate.Value.ToString(ApplicationConstants.DateTimeFormat) : "",
                            Salary = doctor.Salary.HasValue && doctor.Salary.GetValueOrDefault() > 0 ? doctor.Salary.Value.ToString() : ""
                        }).ToList();

            foreach (var item in data)
            {
                if (request.IsReport)
                    item.PhotoUrl = Base64String(request.ImagePath, item.PhotoUrl);
                var appointments = doctorAppointmentRepository.All().Where(x => x.DoctorCode == item.DoctorCode);
                if (appointments.Any())
                {
                    StringBuilder appBuilder = new StringBuilder();
                    StringBuilder dayBuilder = new StringBuilder();
                    StringBuilder timeBuilder = new StringBuilder();
                    string timeFromFormatted = string.Empty;
                    string timeToFormatted = string.Empty;
                    List<string> days = new List<string>();
                    List<string> formattedDays = new List<string>();
                    List<string> formattedTimes = new List<string>();
                    List<string> formattedAppointments = new List<string>();

                    foreach (var app in appointments)
                    {
                        formattedDays = new List<string>();
                        if (app.VisitingTimeFrom.HasValue)
                        {
                            timeFromFormatted = app.VisitingTimeFrom.Value.ToString("hh:mm tt");
                        }
                        if (app.VisitingTimeTo.HasValue)
                        {
                            timeToFormatted = app.VisitingTimeTo.Value.ToString("hh:mm tt");
                        }

                        days = new List<string>();
                        foreach (var day in app.AppointmentDays.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            days.Add(day.Substring(0, 3));
                        }
                        //days = new List<string>();
                        //days.AddRange(app.AppointmentDays.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

                        if (days.Count > 1)
                        {
                            formattedDays.Add($"{days.FirstOrDefault()}-{days.LastOrDefault()}");
                        }
                        else
                        {
                            formattedDays.Add($"{days.FirstOrDefault()}");
                        }


                        formattedAppointments.Add($"{formattedDays.FirstOrDefault()} ({timeFromFormatted}-{timeToFormatted})");

                        // formattedTimes.Add($"{timeFromFormatted}-{timeToFormatted}");
                    }

                    if (request.IsReport)
                        dayBuilder.AppendLine(string.Join("," + Environment.NewLine, formattedAppointments));
                    else
                        dayBuilder.AppendLine(string.Join(",<br/>", formattedAppointments));
                    // dayBuilder.AppendLine(string.Join(",<br/>", formattedDays));
                    // timeBuilder.AppendLine(string.Join(",<br/>", formattedTimes));


                    item.AppointmentDays = dayBuilder.ToString();
                    // item.AppointmentDays = dayBuilder.ToString();
                    // item.VisitingTime = timeBuilder.ToString();
                }
                else
                {
                    item.AppointmentDays = string.Empty;
                    item.VisitingTime = string.Empty;
                }
            }

            return data;
        }

        public string Base64String(string imagePath, string ImageName)
        {
            string result = string.Empty;
            string path = $"{imagePath}\\BlankImage.png"; ;
            if (!string.IsNullOrWhiteSpace(ImageName))
            {
                string doctorImagePath = $"{imagePath.Replace("Images", "")}Uploads\\Images\\Doctors\\{ImageName}";
                if (File.Exists(doctorImagePath))
                    path = doctorImagePath;
            }

            using (var fs = new FileStream(path, FileMode.Open))
            {
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    result = Convert.ToBase64String(ms.ToArray());
                }
            }
            return result;
        }
        public bool SavePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Doctor Information" && x.CheckAdd);
        }
        public bool UpdatePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Doctor Information" && x.CheckEdit);
        }
        public bool DeletePermission(string accessCode)
        {
            return accessCodeRepository.All().Any(x => x.AccessCodeId == accessCode && x.Title == "Doctor Information" && x.CheckDelete);
        }
    }
}
