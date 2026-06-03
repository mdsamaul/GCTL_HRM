using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.PackagingInformation
{
    public class PackagingInformationSetupViewModel :BaseViewModel
    {
        public int Tc { get; set; }
        public string PackageId { get; set; }
        [Required(ErrorMessage = "Package Name is required")]
        public string PackageName { get; set; }
        public string Type { get; set; }
        public string Volume { get; set; }
        public int? MaxCapacity { get; set; }
        public string UnitTypId { get; set; }
        public string UnitTypeName { get; set; }
        public string PackageType { get; set; }
        public string Remarks { get; set; }
    }
}
