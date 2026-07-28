using GCTL.Core.ViewModels.SalaryInformationReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.SalaryInformationReport
{
    public interface ISalaryInformationReportService
    {
        /// <summary>
        /// Executes dbo.usp_GetPayrollMasterFile_General with the supplied filters
        /// and returns the mapped result set.
        /// </summary>
        Task<List<SalaryInformationReportDto>> GetPayrollMasterFileAsync(SalaryInformationReportFilterDto filter);

        /// <summary>
        /// Builds the "Payroll Master File - General" Excel workbook (EPPlus) with the
        /// company logo on the left of the header, and returns the file bytes.
        /// </summary>
        Task<byte[]> ExportToExcelAsync(SalaryInformationReportFilterDto filter);
    }
}
