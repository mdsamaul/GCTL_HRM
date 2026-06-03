using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.AdvanceLoanAdjustment
{
    public class LoanDataDto
    {
        public string LoanId { get; set; }
        public string LoanDate { get; set; }
        public string LoanType { get; set; }
        public string LoanStartEndDate { get; set; }
        public string NoOfInstallment { get; set; }
        public decimal? LoanAmount { get; set; }
        public string StarDate { get; set; }
        public string EndDate { get; set; }
        public decimal? MonthlyDeduction { get; set; }
        public string PayHeadNameId { get; set; }
        public string PayHeadNameName { get; set; }
    }
}
