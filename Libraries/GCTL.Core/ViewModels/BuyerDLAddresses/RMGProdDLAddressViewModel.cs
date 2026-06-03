using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.BuyerDLAddress

{
    public class RMGProdDLAddressViewModel : BaseViewModel
    {
        public decimal Tc { get; set; }
        [DisplayName("Delivery Address ID")]
        public string DeliveryAddressId { get; set; }
        [DisplayName("Buyer ID")]
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
        [DisplayName("Delivery Name")]
        public string Name { get; set; }
        [DisplayName("Delivery Address")]
        public string DeliveryAddress { get; set; }
        public string ContactPerson { get; set; }
        public string Designation { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
