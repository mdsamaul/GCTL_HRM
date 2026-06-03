using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ProbationPeriodExtension
{
    public class ProbationPeriodExtensionSetupViewModel : BaseViewModel
    {
        public string CompanyCode { get; set; }
        public decimal AutoId { get; set; }
        public string Ppeid { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string ExtendedPeriod { get; set; }
        public string PeriodInfoId { get; set; }
        public string Extended { get; set; }
        public string Period { get; set; }
        public string Wef { get; set; }
        public decimal? PreviousSalary { get; set; }
        public decimal? ExtensionSalary { get; set; }
        public decimal? CurrentSalary { get; set; }
        public string RefLetterNo { get; set; }
        public string RefLetterDate { get; set; }
        public string Remarks { get; set; }
        public string DurationSinceJoining { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationId { get; set; }
        public string DesignationName { get; set; }
        public decimal GrossSalary { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? JoiningDate { get; set; }
        public string ProbationPeriodType { get; set; }
        public string ProbationPeriod { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ContractEndDate { get; set; }
        public List<string> SelectedEmployeeIds { get; set; } = new List<string>();
    }
}
