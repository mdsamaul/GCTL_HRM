//using GCTL.Service.ReferencePersons;

using GCTL.Service.AdmissionTypes;
using GCTL.Service.BloodGroups;
using GCTL.Service.BranchesTypeInfo;
using GCTL.Service.BuyerDepartmentEntry;
using GCTL.Service.Companies;
using GCTL.Service.CompanyInfo;
using GCTL.Service.Departments;
using GCTL.Service.Designations;
//using GCTL.Service.EmployeeOfficialInfo;
using GCTL.Service.Districts;
using GCTL.Service.Doctors;
using GCTL.Service.DoctorTypes;
using GCTL.Service.DoctorWorkingPalace;
using GCTL.Service.Grades;
using GCTL.Service.HolidayTypes;
using GCTL.Service.HRMDefBoardCountryNames;
//using GCTL.Service.BankInformations;
//using GCTL.Service.BankBranchInformations;
using GCTL.Service.HrmDefDegrees;
//using GCTL.Service.HrmEmployeeEducations;
using GCTL.Service.HrmDefExamGroupInfos;
using GCTL.Service.HRMDefExamTitles;
using GCTL.Service.HrmDefGradeTypes;
using GCTL.Service.HrmDefInstitutes;
using GCTL.Service.HrmDefOccupations;
using GCTL.Service.HrmDefSeparationTypes;
using GCTL.Service.JobTitles;
using GCTL.Service.PaymentModes;
using GCTL.Service.Qualifications;
using GCTL.Service.Relation;
using GCTL.Service.Relationships;
using GCTL.Service.Religions;
using GCTL.Service.SALES_Def_Inv_MainItemGroupService;
using GCTL.Service.Shifts;
using GCTL.Service.Specialities;
using GCTL.Service.StyleInformation;
using GCTL.Service.Units;
using Microsoft.AspNetCore.Mvc;


namespace GCTL.UI.Core.Controllers
{
    public class CascadingController : Controller
    {
        private readonly IDoctorTypeService doctorTypeService;
        private readonly IBloodGroupService bloodGroupService;
        private readonly IReligionService religionService;
        private readonly IDesignationService designationService;
        private readonly IDepartmentService departmentService;
        private readonly ISpecialityService specialityService;
        private readonly IQualificationService qualificationService;
        private readonly IShiftService shiftService;

        // private readonly IEmployeeService employeeService;
        //private readonly IReferencePersonService referencePersonService;

        private readonly IPaymentModeService paymentModeService;
        private readonly IDoctorService doctorService;

        private readonly IAdmissionTypeService admissionTypeService;

        private readonly IUnitService unitService;

        private readonly IDoctorWorkingPlaceService DoctorWorkingPlaceService;
        private readonly IRelationService RelationService;
        private readonly ICompanyService companyService;
        private readonly IHrmDefHolidayTypeService hrmDefHolidayTypeService;
        //private readonly IBankInformationsService bankInformationsService;
        //private readonly ISalesDefBankBranchInfosService salesDefBankBranchInfosService;
        private readonly IHrmDefDegreesService degree;
        private readonly IHRMDefBoardCountryNamesService boardCountry;
        private readonly IHRMDefExamTitlesService examTitle;
        private readonly IHrmDefInstitutesService institute;
        private readonly IHrmDefExamGroupInfosService examGroup;
        //private readonly IEmployeeOfficialInfoService _employeeOfficialInfoService;
        private readonly IBranchTypeInfoService branchTypeInfoService;
        private readonly IDistrictsService districtsService;
        private readonly IRelationshipsService relationshipsService;
        private readonly IHrmDefGradeTypesService hrmDefGradeTypesService;
        private readonly IGradesService gradesService;
        private readonly IHrmDefOccupationsService hrmDefOccupationsService;
        private readonly IHrmDefSeparationTypeService hrmDefSeparationTypeService;
        private readonly IJobTitleService _jobTitleService;
        private readonly ISALES_Def_Inv_MainItemGroup itemTypeInfoService;
        private readonly IStyleInformationService styleInformationService;
        private readonly ICompanyInfoSupService companyInfoSupService;
        //Nazmul 
        private readonly IBuyerDepEntryService buyerDepEntryService;


        public CascadingController(IDoctorTypeService doctorTypeService,
            IBloodGroupService bloodGroupService,
            IReligionService religionService,
            IDesignationService designationService,
            IDepartmentService departmentService,
            ISpecialityService specialityService,
            IQualificationService qualificationService,
            IShiftService shiftService,
            // IEmployeeService employeeService,
            //IReferencePersonService referencePersonService,
            IPaymentModeService paymentModeService,
            IDoctorService doctorService,
            IAdmissionTypeService admissionTypeService,
            IUnitService unitService,
            IDoctorWorkingPlaceService doctorWorkingPlaceService,
            IRelationService relationService,
            ICompanyService companyService,
            IHrmDefHolidayTypeService hrmDefHolidayTypeService,
            //IBankInformationsService bankInformationsService,
            //ISalesDefBankBranchInfosService salesDefBankBranchInfosService,
            IHrmDefDegreesService degree,
            IHRMDefBoardCountryNamesService boardCountry,
            IHRMDefExamTitlesService examTitle,
             IHrmDefInstitutesService institute,
             IHrmDefExamGroupInfosService examGroup,
             IBranchTypeInfoService branchTypeInfoService,
             IDistrictsService districtsService,
             IRelationshipsService relationshipsService,
             IHrmDefGradeTypesService hrmDefGradeTypesService,
             IGradesService gradesService,
             //IEmployeeOfficialInfoService employeeOfficialInfoService,
             IHrmDefOccupationsService hrmDefOccupationsService
,
             IHrmDefSeparationTypeService hrmDefSeparationTypeService
,
             IJobTitleService jobTitleService,
             IBuyerDepEntryService buyerDepEntryService,
            ICompanyInfoSupService companyInfoSupService,
            ISALES_Def_Inv_MainItemGroup _itemTypeInfoService,
            IStyleInformationService styleInformationService
            )
        {
            this.doctorTypeService = doctorTypeService;
            this.bloodGroupService = bloodGroupService;
            this.religionService = religionService;
            this.designationService = designationService;
            this.departmentService = departmentService;
            this.specialityService = specialityService;
            this.qualificationService = qualificationService;
            this.shiftService = shiftService;
            //this.employeeService = employeeService;
            //this.referencePersonService = referencePersonService;
            this.paymentModeService = paymentModeService;
            this.doctorService = doctorService;
            this.admissionTypeService = admissionTypeService;
            this.unitService = unitService;
            DoctorWorkingPlaceService = doctorWorkingPlaceService;
            RelationService = relationService;
            this.companyService = companyService;
            this.hrmDefHolidayTypeService = hrmDefHolidayTypeService;
            //this.bankInformationsService = bankInformationsService;
            //this.salesDefBankBranchInfosService = salesDefBankBranchInfosService;
            this.degree = degree;
            this.boardCountry = boardCountry;
            this.examTitle = examTitle;
            this.institute = institute;
            this.examGroup = examGroup;
            this.branchTypeInfoService = branchTypeInfoService;
            this.districtsService = districtsService;
            this.relationshipsService = relationshipsService;
            this.hrmDefGradeTypesService = hrmDefGradeTypesService;
            this.gradesService = gradesService;
            //_employeeOfficialInfoService = employeeOfficialInfoService;
            this.hrmDefOccupationsService = hrmDefOccupationsService;
            this.hrmDefSeparationTypeService = hrmDefSeparationTypeService;
            _jobTitleService = jobTitleService;
            itemTypeInfoService = _itemTypeInfoService;
            this.styleInformationService = styleInformationService;
            this.buyerDepEntryService = buyerDepEntryService;
            this.companyInfoSupService = companyInfoSupService;
        }

        public async Task<IActionResult> GetGradeDD()
        {
            var data = await gradesService.SelectionGradesTypeAsync();
            return Json(data);
        }

        public async Task<IActionResult> GetGradeTypeId()
        {
            var data = await hrmDefGradeTypesService.SelectionHrmDefGradeTypeAsync();
            return Json(data);
        }

        public async Task<IActionResult> GetDegree()
        {
            return Json(await degree.SelectionHrmDefDegreeTypeAsync());
        }

        public async Task<IActionResult> GetBoard()
        {
            return Json(await boardCountry.SelectionHrmDefBoardCountryTypeAsync());
        }

        public async Task<IActionResult> GetInstitute()
        {
            return Json(await institute.SelectionHrmDefInstituteTypeAsync());
        }

        public async Task<IActionResult> GetExamTitle()
        {
            return Json(await examTitle.SelectionHrmDefExamTitleTypeAsync());
        }

        public async Task<IActionResult> GetGroup()
        {
            return Json(await examGroup.SelectionHrmDefExamGroupInfoTypeAsync());
        }

        //


        //public IActionResult GetBankInfo()
        //{
        //    return Json( bankInformationsService.BankDropSelectionAsync());
        //}


        public async Task<IActionResult> GetCompany()
        {
            return Json(await companyService.GetCompanyDropDown());
        }

        //public IActionResult GetBank()
        //{
        //    return Json(bankInformationsService.BankDropSelectionAsync());
        //}

        //public IActionResult GetBankBranch()
        //{
        //    return Json(salesDefBankBranchInfosService.BankBranchDropSelectionAsync());
        //}
        public async Task<IActionResult> GetHolidayTypes()
        {

            return Json(await hrmDefHolidayTypeService.SelectionHrmDefHolidayTypeAsync());
        }

        public IActionResult GetRelations()
        {
            return Json(RelationService.RelationSelection());
        }

        public IActionResult GetDoctorTypes()
        {
            return Json(doctorTypeService.DoctorTypeSelection());
        }

        public IActionResult GetBloodGroups()
        {
            return Json(bloodGroupService.BloodGroupSelection());
        }

        public IActionResult GetReligions()
        {
            return Json(religionService.ReligionSelection());
        }

        public IActionResult GetDesignations()
        {
            return Json(designationService.DesignationSelection());
        }

        public IActionResult GetDepartments()
        {
            return Json(departmentService.DepartmentSelection());
        }

        public IActionResult GetSpecialities()
        {
            return Json(specialityService.SpecialitySelection());
        }

        public IActionResult GetQualifications()
        {
            return Json(qualificationService.QualificationSelection());
        }


        public IActionResult GetPaymentModes()
        {
            return Json(paymentModeService.PaymentModeSelection());
        }

        //public IActionResult GetEmployeeSummary(string employeeId)
        //{
        //    return Json(employeeService.GetEmployee(employeeId));
        //}

        //public IActionResult GetReferencePersons()
        //{
        //    return Json(referencePersonService.ReferencePersonSelection());
        //}

        public IActionResult GetDoctors()
        {
            return Json(doctorService.GetDoctorSelections());
        }

        //public IActionResult GetLabReferencePersons()
        //{
        //    return Json(referencePersonService.LabReferencePersonSelection());
        //}

        //public IActionResult GetReferenceSummary(string referencePersonId)
        //{
        //    return Json(referencePersonService.GetReferencePerson(referencePersonId));
        //}

        public IActionResult GetDoctorSummary(string doctorCode)
        {
            return Json(doctorService.GetDoctorByCode(doctorCode));
        }





        [HttpPost]


        public IActionResult GetDoctorsByDepartment(string departmentCode)
        {
            return Json(doctorService.GetDoctorSelections(departmentCode));
        }

        public IActionResult GetAdmissionTypes()
        {
            return Json(admissionTypeService.AdmissionTypeSelection());
        }



        public IActionResult GetUnits()
        {
            return Json(unitService.UnitSelection());
        }


        public IActionResult GetDoctorWorkingPlace()
        {
            return Json(DoctorWorkingPlaceService.WorkingPlaceSelection());
        }

        //public IActionResult GetEmployeeTypes()
        //{
        //    return Json(_employeeOfficialInfoService.EmployeeTypeSelection());
        //}


        public async Task<IActionResult> GetCompanieBranchSelections()
        {
            var data = await branchTypeInfoService.GetCompanieBranchSelections();
            return Json(data);
        }

        public async Task<IActionResult> GetDistrictsSelections()
        {
            return Json(await districtsService.GetDistrictsSelectionsAsync());
        }

        public async Task<IActionResult> GetRelationship()
        {
            return Json(await relationshipsService.RelationshipsSelectionAsync());
        }

        public async Task<IActionResult> GetOccupation()
        {
            return Json(await hrmDefOccupationsService.SelectOccupationAsnyc());
        }

        public async Task<IActionResult> GetSepration()
        {
            return Json(await hrmDefSeparationTypeService.SelectionHrmDefSeparationTypeAsync());
        }

        public async Task<IActionResult> GetJobs()
        {
            return Json(await _jobTitleService.SelectionHrmDefJobTitleAsync());
        }
        public async Task<IActionResult> GetItemType()
        {
            return Json(await itemTypeInfoService.SelectionGetItemTypeTitleAsync());
        }

        public async Task<IActionResult> GetBuyerDepartment()
        {
            return Json(await buyerDepEntryService.SelectionBuyerDepAsync());
        }

        public async Task<IActionResult> GetBuyerCompany()
        {
            return Json(await companyInfoSupService.SelectionBuyerCompanyInfoAsync());
        }
        public async Task<IActionResult> GetStyle()
        {
            return Json(await styleInformationService.SelectionStyleInformationAsync());
        }
    }
}
