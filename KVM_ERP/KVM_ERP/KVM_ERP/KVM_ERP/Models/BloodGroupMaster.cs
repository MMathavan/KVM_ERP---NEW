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
    [Table("BLOODGROUPMASTER")]
    public class BloodGroupMaster
    {
        [Key]
        public int BLDGID { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        [Remote("ValidateBLDGDESC", "Common", AdditionalFields = "i_BLDGDESC", ErrorMessage = "This is already used.")]
        //  [Editable(true)]  
        public string BLDGDESC { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        [Remote("ValidateBLDGCODE", "Common", AdditionalFields = "i_BLDGCODE", ErrorMessage = "This is already used.")]
        public string BLDGCODE { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        [DisplayName("Status")]
        //  [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }

        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}