

namespace GCTL.Service.EmployeeOfficialInfoReport
{
    public class ProcedureBackUP
    {
        
    }

}




//CREATE PROCEDURE GetOfficialInfo
//    @DepartmentCodes NVARCHAR(MAX) = NULL,
//    @DesignationCodes NVARCHAR(MAX) = NULL,
//    @EmployeeCodes NVARCHAR(MAX) = NULL,
//    @BranchCodes NVARCHAR(MAX) = NULL,
//    @CompanyCodes NVARCHAR(MAX) = NULL,
//    @EmployeeTypeCode NVARCHAR(50) = NULL,
//    @EmploymentNatureId NVARCHAR(50) = NULL,
//    @NationalId NVARCHAR(50) = NULL,
//    @TinNo NVARCHAR(50) = NULL,
//    @PassportNo NVARCHAR(50) = NULL,
//    @DrivingLicense NVARCHAR(50) = NULL,
//    @IsExpatriate NVARCHAR(10) = NULL,
//    @ImmediateSup NVARCHAR(50) = NULL,
//    @HOD NVARCHAR(50) = NULL,
//    @ShiftCode NVARCHAR(50) = NULL,
//    @EmployeeStatus NVARCHAR(50) = NULL,
//    @SalaryFrom DECIMAL(18,2) = NULL,
//    @SalaryTo DECIMAL(18,2) = NULL,
//    @AppointmentDateFrom DATE = NULL,
//    @AppointmentDateTo DATE = NULL,
//    @JoiningDatefrom date=null,
//    @JoiningDateTo date=null,
//    @TerminationDateFrom DATE = NULL,
//   @TerminationDateTo DATE = NULL,
//   @ProbationDateFrom DATE = NULL,
//  @ProbationDateTo DATE = NULL,
//  @ConfirmationDateFrom DATE = NULL,
//  @ConfirmationDateTo DATE = NULL
//AS
//BEGIN
//    SET NOCOUNT ON;

//SELECT


//        offi.EmployeeID, 
//        e.FirstName + ' ' + e.LastName AS EmpName,
//        desi.DesignationName,
//        dept.DepartmentName,
//        br.BranchName,
//        eNt.EmploymentNature,
//        eT.EmpTypeName,
//offi.JoiningDate,
//ISNULL(s.SeparationDate, GETDATE()) AS SeparationDate,

//     LTRIM(RTRIM(
//CONCAT(
//            CASE
//                WHEN DATEDIFF(YEAR, offi.JoiningDate, ISNULL(s.SeparationDate, GETDATE())) > 0
//                THEN CONCAT(DATEDIFF(YEAR, offi.JoiningDate, ISNULL(s.SeparationDate, GETDATE())), ' Y ')
//                ELSE ''
//            END,
//            CASE
//                WHEN DATEDIFF(MONTH, offi.JoiningDate, ISNULL(s.SeparationDate, GETDATE())) % 12 > 0
//                THEN CONCAT(DATEDIFF(MONTH, offi.JoiningDate, ISNULL(s.SeparationDate, GETDATE())) % 12, ' M ')
//                ELSE ''
//            END,
//CASE
//                WHEN DATEDIFF(DAY, DATEADD(MONTH, DATEDIFF(MONTH, offi.JoiningDate, ISNULL(s.SeparationDate, GETDATE())), offi.JoiningDate), ISNULL(s.SeparationDate, GETDATE())) > 0
//                THEN CONCAT(DATEDIFF(DAY, DATEADD(MONTH, DATEDIFF(MONTH, offi.JoiningDate, ISNULL(s.SeparationDate, GETDATE())), offi.JoiningDate), ISNULL(s.SeparationDate, GETDATE())), ' D')
//                ELSE ''
//            END
//)
//)) AS ServiceLength,
//sh.ShiftName,
//Supervisor.FirstName + ' ' + Supervisor.LastName AS ImmediateSupervisorName,
//HOD.FirstName + ' ' + HOD.LastName AS HeadOfDepartmentName,
//offi.MobileNo,
//offi.Email,
//        es.EmployeeStatus
//    FROM HRM_EmployeeOfficialInfo AS offi
//    LEFT JOIN HRM_Employee AS e ON offi.EmployeeID = e.EmployeeID
//   left  JOIN HRM_Def_Designation AS desi ON offi.DesignationCode = desi.DesignationCode
//   left  JOIN HRM_Def_Department AS dept ON offi.DepartmentCode = dept.DepartmentCode
//   left  JOIN Core_Branch AS br ON offi.BranchCode = br.BranchCode
//   left  JOIN HRM_EIS_Def_EmploymentNature AS eNt ON offi.EmploymentNatureId = eNt.EmploymentNatureId
//  left   JOIN HRM_Def_EmpType AS eT ON offi.EmpTypeCode = eT.EmpTypeCode
//   left    JOIN HRM_Employee AS HOD ON offi.HOD = HOD.EmployeeID  
//  left   JOIN HRM_Employee AS Supervisor ON offi.ReportingTo = Supervisor.EmployeeID
//  left   JOIN HRM_Def_EmployeeStatus es ON offi.EmployeeStatus = es.EmployeeStatusId
//  left   JOIN HRM_Separation AS s ON offi.EmployeeID = s.EmployeeID
//    left JOIN HRM_ATD_Shift AS sh ON offi.ShiftCode = sh.ShiftCode
//  left   JOIN HRM_EmployeeAdditionalInfo AS addi ON offi.EmployeeID = addi.EmployeeID 
//    WHERE 
//        (@DepartmentCodes IS NULL OR offi.DepartmentCode IN (SELECT value FROM STRING_SPLIT(@DepartmentCodes, ','))) AND
//        (@DesignationCodes IS NULL OR offi.DesignationCode IN (SELECT value FROM STRING_SPLIT(@DesignationCodes, ','))) AND
//        (@EmployeeCodes IS NULL OR offi.EmployeeID IN (SELECT value FROM STRING_SPLIT(@EmployeeCodes, ','))) AND
//        (@BranchCodes IS NULL OR offi.BranchCode IN (SELECT value FROM STRING_SPLIT(@BranchCodes, ','))) AND
//        (@CompanyCodes IS NULL OR offi.CompanyCode IN (SELECT value FROM STRING_SPLIT(@CompanyCodes, ','))) AND
//        (@EmployeeTypeCode IS NULL OR offi.EmpTypeCode = @EmployeeTypeCode) AND
//        (@EmploymentNatureId IS NULL OR offi.EmploymentNatureId = @EmploymentNatureId) AND
//        (@NationalId IS NULL OR e.NationalIdno = @NationalId) AND
//        (@TinNo IS NULL OR e.TinNo = @TinNo) AND
//        (@PassportNo IS NULL OR addi.PassportNo = @PassportNo) AND
//        (@DrivingLicense IS NULL OR addi.LicenseNo = @DrivingLicense) AND
//        (@ShiftCode IS NULL OR offi.ShiftCode = @ShiftCode) AND
//        (@EmployeeStatus IS NULL OR offi.EmployeeStatus = @EmployeeStatus) AND
//        (@ImmediateSup IS NULL OR offi.ReportingTo = @ImmediateSup) AND
//        (@HOD IS NULL OR offi.HOD = @HOD) AND
//        (@SalaryFrom IS NULL OR offi.GrossSalary >= @SalaryFrom) AND
//        (@SalaryTo IS NULL OR offi.GrossSalary <= @SalaryTo) AND
//        (@AppointmentDateFrom IS NULL OR offi.AppointmentLetterDate >= @AppointmentDateFrom) AND
//        (@AppointmentDateTo IS NULL OR offi.AppointmentLetterDate <= @AppointmentDateTo) and
//		(@JoiningDatefrom is null or offi.JoiningDate >=@JoiningDatefrom) and
//		(@JoiningDateTo is null or offi.JoiningDate <=@JoiningDateTo)and 

//		(@TerminationDateFrom IS NULL OR s.SeparationDate >= @TerminationDateFrom) AND
//      (@TerminationDateTo IS NULL OR s.SeparationDate <= @TerminationDateTo) AND

//       (@ProbationDateFrom IS NULL OR offi.ProbationEffectDate >= @ProbationDateFrom) AND
//      (@ProbationDateTo IS NULL OR offi.ProbationEffectDate <= @ProbationDateTo) AND

//      (@ConfirmationDateFrom IS NULL OR offi.ConfirmeDate >= @ConfirmationDateFrom) AND
//     (@ConfirmationDateTo IS NULL OR offi.ConfirmeDate <= @ConfirmationDateTo) 

//    ORDER BY offi.EmployeeID;
//END;
