using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("PACKINGTYPEMASTER")]
    public class PackingTypeMaster
    {
        [Key]
        public int PACKTMID { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please enter code")]
        [MaxLength(15)]
        [Remote("ValidatePACKTMCODE", "PackingTypeMaster", AdditionalFields = "PACKTMID", ErrorMessage = "This code is already used.")]
        public string PACKTMCODE { get; set; }

        [DisplayName("Packing Description")]
        [Required(ErrorMessage = "Please enter packing description")]
        [MaxLength(100)]
        public string PACKTMDESC { get; set; }

        [DisplayName("Packing Master")]
        [Required(ErrorMessage = "Please select packing master")]
        public int PACKMID { get; set; }

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
