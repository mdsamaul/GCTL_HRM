using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.TransportExpenseStatementReport
{
    public class TransportExpenseStatementReportSetupViewModel :BaseViewModel
    {
        public string VehicleID { get; set; }
        public string VehicleNo { get; set; }
        public string? VehicleTypeID { get; set; }
        public string VehicleType { get; set; }
        public decimal? TransportCapacity { get; set; }
        public string EmployeeID { get; set; }
        public string FullName { get; set; }
        public string HelperId { get; set; }
        public string HelperName { get; set; }

        public decimal CNGGasBill { get; set; }
        public decimal RMBill { get; set; }
        public decimal FuelOctaneBill { get; set; }
        public decimal PoliceDonation { get; set; }
        public decimal TollOthersBill { get; set; }
        public decimal TAXFitnessAndRutePermit { get; set; }
        public decimal SalaryDriverHelper { get; set; }
        public decimal MechanicSalary { get; set; }
        public decimal MonthlyPoliceDonation { get; set; }
        public decimal MonthlyEngOilPurchase { get; set; }
        public decimal AkeshTechnology { get; set; }
        public decimal GarageRent { get; set; }
        public decimal MonthlyRunningKM { get; set; }
        public decimal GasAverageCost { get; set; }
        public decimal TotalExpense { get; set; }
    }
}
