using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("PACKINGMASTER")]
    public class PackingMaster
    {
        [Key]
        public int PACKMID { get; set; }

        [DisplayName("Packing Description")]
        [Required(ErrorMessage = "Please enter packing description")]
        [MaxLength(100)]
        public string PACKMDESC { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please enter code")]
        [MaxLength(15)]
        [Remote("ValidatePACKMCODE", "PackingMaster", AdditionalFields = "PACKMID", ErrorMessage = "This code is already used.")]
        public string PACKMCODE { get; set; }

        [DisplayName("No of Unit")]
        [Required(ErrorMessage = "Please enter number of units")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of units must be a positive number")]
        public short PACKMNOU { get; set; }

        // Created user id (username)
        [MaxLength(100)]
        public string CUSRID { get; set; }

        // Last modified user id (username)
        [MaxLength(100)]
        public string LMUSRID { get; set; }

        [DisplayName("Status")]
        public short DISPSTATUS { get; set; }

        [DisplayName("Material Group")]
        public int? MTRLGID { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime PRCSDATE { get; set; }
    }
}
