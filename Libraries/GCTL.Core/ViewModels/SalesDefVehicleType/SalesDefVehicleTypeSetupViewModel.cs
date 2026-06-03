using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SalesDefVehicleType
{
    public class SalesDefVehicleTypeSetupViewModel : BaseViewModel
    {
        public int TC { get; set; }
        public string VehicleTypeID { get; set; }
        public string? VehicleType { get; set; }
        public string? ShortName { get; set; }        
        public string? ShowCreateDate { get; set; }        
        public string? ShowModifyDate { get; set; }        
        public decimal? StorageSize { get; set; }
        public string? StorageUnitID { get; set; }
        public decimal? WeightLoadCapacity { get; set; }
        public string? CapacityUnitID { get; set; }
    }
}
