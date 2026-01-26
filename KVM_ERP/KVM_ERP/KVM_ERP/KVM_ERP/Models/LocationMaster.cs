using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("LOCATIONMASTER")]
    public class LocationMaster
    {
        [Key]
        public int LOCTID { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Please enter description")]
        [Remote("ValidateLOCTDESC", "Common", AdditionalFields = "i_LOCTDESC", ErrorMessage = "This is already used.")]
        public string LOCTDESC { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please enter code")]
        [MaxLength(15)]
        [Remote("ValidateLOCTCODE", "Common", AdditionalFields = "i_LOCTCODE", ErrorMessage = "This is already used.")]
        public string LOCTCODE { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "Please select state")]
        public int STATEID { get; set; }

        // Created user id (username)
        public string CUSRID { get; set; }

        // Last modified user id (int)
        public int LMUSRID { get; set; }

        [DisplayName("Status")]
        public short DISPSTATUS { get; set; }

        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }

        // Navigation property for State (if you have StateMaster)
        [NotMapped]
        public string StateName { get; set; }
    }
}
