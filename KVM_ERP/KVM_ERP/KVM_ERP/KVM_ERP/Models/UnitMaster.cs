using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("UNITMASTER")]
    public class UnitMaster
    {
        [Key]
        public int UNITID { get; set; }

        [DisplayName("Unit Description")]
        [Required(ErrorMessage = "Please enter unit description")]
        [MaxLength(100)]
        public string UNITDESC { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please enter code")]
        [MaxLength(15)]
        [Remote("ValidateUNITCODE", "UnitMaster", AdditionalFields = "UNITID", ErrorMessage = "This code is already used.")]
        public string UNITCODE { get; set; }

        // Created user id (username)
        [MaxLength(100)]
        public string CUSRID { get; set; }

        // Last modified user id (username)
        [MaxLength(100)]
        public string LMUSRID { get; set; }

        [DisplayName("Status")]
        public short DISPSTATUS { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime PRCSDATE { get; set; }
    }
}
