using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("LABORATORYMASTER")]
    public class LaboratoryMaster
    {
        [Key]
        public int LABOID { get; set; }

        [DisplayName("Laboratory Description")]
        [Required(ErrorMessage = "Please enter laboratory description")]
        [MaxLength(100)]
        [Remote("ValidateLABODESC", "Common", AdditionalFields = "i_LABODESC", ErrorMessage = "This laboratory description is already used.")]
        public string LABODESC { get; set; }

        [DisplayName("Laboratory Code")]
        [Required(ErrorMessage = "Please enter laboratory code")]
        [MaxLength(15)]
        [Remote("ValidateLABOCODE", "Common", AdditionalFields = "i_LABOCODE", ErrorMessage = "This laboratory code is already used.")]
        public string LABOCODE { get; set; }

        // Created user id (username)
        public string CUSRID { get; set; }

        // Last modified user id (username)
        public string LMUSRID { get; set; }

        [DisplayName("Status")]
        public short DISPSTATUS { get; set; }

        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}

