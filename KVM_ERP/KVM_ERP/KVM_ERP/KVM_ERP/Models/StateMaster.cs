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
    [Table("STATEMASTER")]
    public class StateMaster
    {
        [Key]
        public int STATEID { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        [Remote("ValidateSTATEDESC", "Common", AdditionalFields = "i_STATEDESC", ErrorMessage = "This is already used.")]
        //  [Editable(true)]  
        public string STATEDESC { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        [Remote("ValidateSTATECODE", "Common", AdditionalFields = "i_STATECODE", ErrorMessage = "This is already used.")]
        public string STATECODE { get; set; }

        public string CUSRID { get; set; }

        [DisplayName("State Type")]
        public short STATETYPE { get; set; } // Added missing field

        public int LMUSRID { get; set; }

        [DisplayName("Status")]
        //  [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }

        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }


        [DisplayName("Region ID")]
        public int SREGNID { get; set; }

        // Optional: Navigation property for Region
        [ForeignKey("SREGNID")]
        public virtual RegionMaster Region { get; set; }
    }
}