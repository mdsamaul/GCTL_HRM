using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SalesDefVehicle
{
    public class SalesDefVehicleSetupViewModel : BaseViewModel
    {
        public int TC { get; set; }
        public string VehicleID { get; set; }
        public string? VehicleNo { get; set; }
        public string? VehicleTypeID { get; set; }
        public string? VehicleTypeName { get; set; }
        public string? Remarks { get; set; }
        //public string? LUser { get; set; }
        //public DateTime? LDate { get; set; }
        //public string? LIP { get; set; }
        //public string? LMAC { get; set; }
        //public DateTime? ModifyDate { get; set; }
        public string? FactoryID { get; set; }
        public string? CompanyCode { get; set; }
        public string? ShowCreateDate { get; set; }
        public string? ShowModifyDate { get; set; }
        public string? CompanyName { get; set; }
        public string? PortNameId { get; set; }
        public string? EmployeeID { get; set; }
        public string? Phone { get; set; }
        public string? VendorId { get; set; }
        public decimal? StorageSize { get; set; }
        public string? StorageUnitID { get; set; }
        public decimal? WeightLoadCapacity { get; set; }
        public decimal? TransportCapacity { get; set; }
        public string? CapacityUnitID { get; set; }
        public decimal? TransportLength { get; set; }
        public decimal? TransportWidth { get; set; }
        public decimal? TransportHeight { get; set; }
        public string? UnitTypID_CBM { get; set; }
        public decimal? TotalVolume { get; set; }
    }
}
