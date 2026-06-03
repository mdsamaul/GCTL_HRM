using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmEmployeeDocumentInfos
{
    public class HrmEmployeeDocumentInfosSetup:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string EmpDocId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentDiscription { get; set; }
        public string DocumentType { get; set; }
        public string? Doucment { get; set; }  //Image for VM 
        [Required]
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
       
        //public IFormFile Doucment { get; set; }
         public bool IsClearImage { get; set; }
        public IFormFile? Photo { get; set; }
        public string CoreBranchName { get; set; }


    }
}
