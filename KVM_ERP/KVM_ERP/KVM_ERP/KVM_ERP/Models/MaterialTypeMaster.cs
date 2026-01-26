using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("MATERIALTYPEMASTER")]
    public class MaterialTypeMaster
    {
        [Key]
        public int MTRLTID { get; set; }

        [DisplayName("Material Description")]
        [Required(ErrorMessage = "Please enter material description")]
        [MaxLength(100)]
        public string MTRLTDESC { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please enter code")]
        [MaxLength(15)]
        [Remote("ValidateMTRLTCODE", "MaterialTypeMaster", AdditionalFields = "MTRLTID", ErrorMessage = "This code is already used.")]
        public string MTRLTCODE { get; set; }

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

        [DisplayName("Display Order")]
        [Required(ErrorMessage = "Please enter display order")]
        [Range(1, int.MaxValue, ErrorMessage = "Display order must be a positive number")]
        public int DISPORDER { get; set; }
    }
}
