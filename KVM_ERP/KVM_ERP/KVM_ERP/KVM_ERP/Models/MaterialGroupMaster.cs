using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("MATERIALGROUPMASTER")]
    public class MaterialGroupMaster
    {
        [Key]
        public int MTRLGID { get; set; }

        [DisplayName("Material Group Description")]
        [Required(ErrorMessage = "Please enter material group description")]
        [MaxLength(100)]
        public string MTRLGDESC { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please enter code")]
        [MaxLength(15)]
        [Remote("ValidateMTRLGCODE", "MaterialGroupMaster", AdditionalFields = "MTRLGID", ErrorMessage = "This code is already used.")]
        public string MTRLGCODE { get; set; }

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
