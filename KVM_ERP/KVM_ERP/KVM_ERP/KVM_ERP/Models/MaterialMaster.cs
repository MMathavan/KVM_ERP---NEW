using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("MATERIALMASTER")]
    public class MaterialMaster
    {
        [Key]
        public int MTRLID { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please enter code")]
        [MaxLength(15)]
        [Remote("ValidateMTRLCODE", "MaterialMaster", AdditionalFields = "MTRLID", ErrorMessage = "This code is already used.")]
        public string MTRLCODE { get; set; }

        [DisplayName("Material Description")]
        [Required(ErrorMessage = "Please enter material description")]
        [MaxLength(100)]
        public string MTRLDESC { get; set; }

        [DisplayName("Group")]
        [Required(ErrorMessage = "Please select material group")]
        public int MTRLGID { get; set; }

        [DisplayName("Type")]
        [Required(ErrorMessage = "Please select material type")]
        public int MTRLTID { get; set; }

        [DisplayName("Unit")]
        [Required(ErrorMessage = "Please select unit")]
        public int UNITID { get; set; }

        [DisplayName("HSN Code")]
        [Required(ErrorMessage = "Please select HSN code")]
        public int HSNID { get; set; }

        // Additional fields with default values
        public int? ACHEADID { get; set; } = 0;

        public int? SCHLID { get; set; } = 0;

        public decimal? ROLNQTY { get; set; } = 0;

        public decimal? ROLXQTY { get; set; } = 0;

        public decimal? EOQTY { get; set; } = 0;

        public decimal? MTRLPRFT { get; set; } = 0;

        public decimal? MTRLBQTY { get; set; } = 0;

        public short? MTRLESTATUS { get; set; } = 0;

        public short? MTRLBSTATUS { get; set; } = 0;

        public short? MTRLASTATUS { get; set; } = 0;

        public short? MTRLRTYPE { get; set; } = 0;

        public decimal MTRLBRATE { get; set; } = 0;

        public int MTRLCATEID { get; set; } = 0;

        public int PACKMID { get; set; } = 0;

        public int DISPORDER { get; set; } = 1;

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
