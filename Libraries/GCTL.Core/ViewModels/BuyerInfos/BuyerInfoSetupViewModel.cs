using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.BuyerInfos
{
    public class BuyerInfoSetupViewModel : BaseViewModel
    {
        public decimal Tc { get; set; }
        [DisplayName("Buyer ID")]
        public string BuyerId { get; set; }
        [DisplayName("Buyer Name")]
        public string BuyerName { get; set; }
        [DisplayName("Head Office Address")]
        public string Address { get; set; }
        [DisplayName("Local Office Address")]
        public string LocalOfficeAddress { get; set; }
        public string CompanyId { get; set; }
        public string BuyerDepartmentId { get; set; }
        public string CountryId { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        [DisplayName("URL")]
        public string Url { get; set; }
        public string ContatPerson1 { get; set; }
        public string DesignationId { get; set; }
        public string Phone1 { get; set; }
        public string Email1 { get; set; }
        public string ContatPerson2 { get; set; }
        public string DesignationId2 { get; set; }
        public string Phone2 { get; set; }
        public string Email2 { get; set; }
        public string ContatPerson3 { get; set; }
        public string DesignationId3 { get; set; }
        public string Phone3 { get; set; }
        public string Email3 { get; set; }
        public string BuyerTypeId { get; set; }
        public string SalesPersonId { get; set; }
        public string Remarks { get; set; }
        public string Active { get; set; }



        public IFormFile BuyerPhoto { get; set; }
        public string Photo { get; set; }
        public string PhotoType { get; set; }





        public string CompanyCode { get; set; }



    }

    public class BuyerInfoGridViewModel
    {
        public decimal Tc { get; set; }
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string Address { get; set; }
        public string CountryName {  get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ContactPersonName { get; set; }
    }
}
