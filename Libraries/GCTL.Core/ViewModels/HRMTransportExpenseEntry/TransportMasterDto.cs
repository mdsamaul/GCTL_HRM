using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMTransportExpenseEntry
{
    public class TransportMasterDto
    {
        public decimal AutoId { get; set; }
        public string TEID { get; set; }
        public string? TEDate { get; set; }
        public string? VehicleNo { get; set; }
        public string? FullName { get; set; }
        public string? Telephone { get; set; }
        public string CompanyCode { get; set; }
        public string? CompanyName { get; set; }
    }
}
