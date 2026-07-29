// GCTL.Service.SalaryInformationReport/ISalaryInformationReportService.cs

using GCTL.Core.ViewModels.SalaryInformationReport;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GCTL.Service.SalaryInformationReport
{
    public interface ISalaryInformationReportService
    {
        // General
        Task<List<SalaryInformationReportDto>> GetPayrollMasterFileAsync(SalaryInformationReportFilterDto filter);
        Task<byte[]> ExportToExcelAsync(SalaryInformationReportFilterDto filter);

        // Gratuity
        Task<List<SalaryInformationReportGratuityDto>> GetPayrollMasterFileGratuityAsync(SalaryInformationReportFilterDto filter);
        Task<byte[]> ExportToExcelGratuityAsync(SalaryInformationReportFilterDto filter);
        // Yearly Bonus
        Task<List<SalaryInformationReportYearlyBonusDto>> GetPayrollMasterFileYearlyBonusAsync(SalaryInformationReportFilterDto filter);
        Task<byte[]> ExportToExcelYearlyBonusAsync(SalaryInformationReportFilterDto filter);
    }
}