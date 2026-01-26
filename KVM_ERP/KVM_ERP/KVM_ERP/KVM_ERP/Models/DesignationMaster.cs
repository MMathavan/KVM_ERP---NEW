using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("DESIGNATIONMASTER")]
    public class DesignationMaster
    {
        [Key]
        public int DSGNID { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Please enter description")]
        [Remote("ValidateDSGNDESC", "Common", AdditionalFields = "i_DSGNDESC", ErrorMessage = "This is already used.")]
        public string DSGNDESC { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please enter code")]
        [MaxLength(15)]
        [Remote("ValidateDSGNCODE", "Common", AdditionalFields = "i_DSGNCODE", ErrorMessage = "This is already used.")]
        public string DSGNCODE { get; set; }

        // Created user id (username)
        public string CUSRID { get; set; }

        // Last modified user id (int)
        public int LMUSRID { get; set; }

        [DisplayName("Status")]
        public short DISPSTATUS { get; set; }

        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}
