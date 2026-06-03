using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SeparationInfos
{
    public class SeparationInfosSetupViewModel: BaseViewModel
    {
        public decimal SeparationCode { get; set; }
        public string SeparationId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string SeparationDate { get; set; } = new DateTime().ToString("dd/MM/yyyy");
        [Required]
        public string SeparationTypeId { get; set; }
        public string SeparationType { get; set; }
        [Required]
        public decimal FinalPayment { get; set; }
        public string IsPaid { get; set; } = "N"; 
        public string Remark { get; set; }
        [Required]
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string RefLetterNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string RefLetterDate { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
    }
}
