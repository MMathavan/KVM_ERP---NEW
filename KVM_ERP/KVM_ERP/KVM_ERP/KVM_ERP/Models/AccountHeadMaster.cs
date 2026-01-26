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
    [Table("ACCOUNTHEADMASTER")]
    public class AccountHeadMaster
    {
        [Key]
        public int ACHEADID { get; set; }
        [Required(ErrorMessage = "select Account Group")]
        public int ACHEADGID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        [Remote("ValidateACHEADDESC", "Common", AdditionalFields = "i_ACHEADDESC", ErrorMessage = "This is already used.")]
        public string ACHEADDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        [Remote("ValidateACHEADCODE", "Common", AdditionalFields = "i_ACHEADCODE", ErrorMessage = "This is already used.")]
        public string ACHEADCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        //  [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}