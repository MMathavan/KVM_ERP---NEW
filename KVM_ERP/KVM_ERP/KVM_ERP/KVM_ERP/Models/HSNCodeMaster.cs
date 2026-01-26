using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("HSNCODEMASTER")]
    public class HSNCodeMaster
    {
        [Key]
        public int HSNID { get; set; }

        [DisplayName("HSN Description")]
        [Required(ErrorMessage = "Please enter HSN description")]
        [MaxLength(100)]
        public string HSNDESC { get; set; }

        [DisplayName("HSN Code")]
        [Required(ErrorMessage = "Please enter HSN code")]
        [MaxLength(20)]
        [Remote("ValidateHSNCODE", "HSNCodeMaster", AdditionalFields = "HSNID", ErrorMessage = "This HSN code is already used.")]
        public string HSNCODE { get; set; }

        [DisplayName("CGST %")]
        [Required(ErrorMessage = "Please enter CGST percentage")]
        [Range(0, 100, ErrorMessage = "CGST percentage must be between 0 and 100")]
        public decimal CGSTEXPRN { get; set; }

        [DisplayName("SGST %")]
        [Required(ErrorMessage = "Please enter SGST percentage")]
        [Range(0, 100, ErrorMessage = "SGST percentage must be between 0 and 100")]
        public decimal SGSTEXPRN { get; set; }

        [DisplayName("IGST %")]
        public decimal IGSTEXPRN { get; set; }

        // Additional GST fields with default values
        public decimal ACGSTEXPRN { get; set; } = 0.00m;

        public decimal ASGSTEXPRN { get; set; } = 0.00m;

        public decimal AIGSTEXPRN { get; set; } = 0.00m;

        public decimal TAXEXPRN { get; set; }

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
