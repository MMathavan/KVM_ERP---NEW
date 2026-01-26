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
    [Table("REGIONMASTER")]
    public class RegionMaster
    {
        [Key]
        public int REGNID { get; set; }

        [DisplayName("Region Description")]
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        [Remote("ValidateREGNDESC", "Common", AdditionalFields = "i_REGNDESC", ErrorMessage = "This is already used.")]
        //  [Editable(true)]  
        public string REGNDESC { get; set; }

        [DisplayName("Region Code")]
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        [Remote("ValidateREGNCODE", "Common", AdditionalFields = "i_REGNCODE", ErrorMessage = "This is already used.")]
        public string REGNCODE { get; set; }

        public string CUSRID { get; set; }

        public int LMUSRID { get; set; }

        public short DISPSTATUS { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? PRCSDATE { get; set; }

    }
}