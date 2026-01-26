using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("COMPANYMASTER")]
    public class CompanyMaster
    {
        [Key]
        public int COMPID { get; set; }


        [DisplayName("Name")]
      //  [Required(ErrorMessage = "Field is required")]
        //  [Remote("ValidateCOMPNAME", "Common", AdditionalFields = "i_COMPNAME", ErrorMessage = "This is already used.")]
        public string COMPNAME { get; set; }
        public string COMPDNAME { get; set; }
        [DisplayName("Address")]
        public string COMPADDR { get; set; }
        [DisplayName("Landline 1")]
        public string COMPPHNID { get; set; }
        public string COMPPHN1 { get; set; }
        [DisplayName("Landline 2")]
        public string COMPPHN2 { get; set; }
        [DisplayName("Mobile 1")]
        public string COMPPHN3 { get; set; }
        [DisplayName("Mobile 2")]
        public string COMPPHN4 { get; set; }
        [DisplayName("Contact Person")]
        public string COMPCPRSN { get; set; }

        [DisplayName("E-Mail")]
        public string COMPMAIL { get; set; }

        [DisplayName("Code")]
     //   [Required(ErrorMessage = "Field is required")]
        //  [Remote("ValidateCOMPCODE", "Common", AdditionalFields = "i_COMPCODE", ErrorMessage = "This is already used.")]
        public string COMPCODE { get; set; }
       public String CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]

        public short DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }
        //public DateTime COMPCOFFTIME { get; set; }
        //public string COMPPDESC1 { get; set; }
       //public string COMPPDESC2 { get; set; }

       // [DisplayName("State")]
        public int STATEID { get; set; }

      //  [DisplayName("HSNCode")]
        //public int HSNID { get; set; }

        public string COMPGSTNO { get; set; }
       public string COMPPANNO { get; set; }
        public string COMPTANNO { get; set; }

    }
}