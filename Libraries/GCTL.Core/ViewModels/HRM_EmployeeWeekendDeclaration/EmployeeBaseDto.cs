using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration
{
    public class EmployeeBaseDto
    {
        public string EmpId { get; set; }                

        public string EmpName { get; set; }  
        public string CompanyName { get; set; }       

        public string CompanyCode { get; set; }       

        public string BranchName { get; set; }        

        public string BranchCode { get; set; }        

        public string DesignationName { get; set; }   

        public string DesignationCode { get; set; }   

        public string DepartmentName { get; set; }    

        public string DepartmentCode { get; set; }    

        public DateTime? Date { get; set; }
        public string Remark { get; set; }            
        public string DayName { get; set; }  
    }

}
