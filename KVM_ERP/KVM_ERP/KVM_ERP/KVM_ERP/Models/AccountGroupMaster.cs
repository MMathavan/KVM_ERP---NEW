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
    [Table("ACCOUNTGROUPMASTER")]
    public class AccountGroupMaster
    {
        [Key]
        public int ACHEADGID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        [Remote("ValidateACHEADGDESC", "Common", AdditionalFields = "i_ACHEADGDESC", ErrorMessage = "This is already used.")]
        //  [Editable(true)]  

        public string ACHEADGDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        [Remote("ValidateACHEADGCODE", "Common", AdditionalFields = "i_ACHEADGCODE", ErrorMessage = "This is already used.")]

        public string ACHEADGCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        //  [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}