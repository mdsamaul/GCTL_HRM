using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SalaryInformationReport
{
    public class SalaryInformationReportDto
    {
        public int SL { get; set; }                         
        public string IdNo { get; set; }                     
        public string PayId { get; set; }                    
        public string DpUserId { get; set; }                 
        public string DbblEmployeesName { get; set; }         
        public string UcblEmployeesName { get; set; }         
        public string Status { get; set; }                   
        public string Department { get; set; }               
        public string Designation { get; set; }               
        public string Doh { get; set; }                       
        public string Dot { get; set; }                       
        public decimal? Duration { get; set; }
        public string Dbbl { get; set; }                      
        public string Ucbl { get; set; }                      
        public decimal? Salary { get; set; }
        public string YearlyBonusEligibility { get; set; }    
        public string GratuityEligibility { get; set; }       
        public decimal? EidBonusEligibility { get; set; }
        public decimal? PfEligiblity { get; set; }
        public string Gender { get; set; }                    
        public string CellPhone { get; set; }                 
        public string SpecialNotes { get; set; }               
        public string EndOfProbation { get; set; }             
        public string ModeOfPayment { get; set; }              
        public string EmploymentNature { get; set; }           
    }
}
