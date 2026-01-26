using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("COSTFACTORMASTER")]
    public class CostFactorMaster
    {
        [Key]
        public int CFID { get; set; }

        // Transaction ID - default 0
        public int TRANTID { get; set; } = 0;

        [DisplayName("Cost Factor Description")]
        [Required(ErrorMessage = "Please enter cost factor description")]
        [MaxLength(100)]
        public string CFDESC { get; set; }

        [DisplayName("Mode")]
        [Required(ErrorMessage = "Please select mode")]
        [MaxLength(5)]
        public string CFMODE { get; set; }

        [DisplayName("As")]
        [Required(ErrorMessage = "Please select type")]
        public short CFTYPE { get; set; }

        [DisplayName("Value")]
        [Required(ErrorMessage = "Please enter value")]
        public decimal CFEXPR { get; set; }

        [DisplayName("On")]
        [Required(ErrorMessage = "Please select nature")]
        public short CFNATR { get; set; }

        [DisplayName("Calculated On")]
        [Required(ErrorMessage = "Please select calculated on")]
        public short CFOPTN { get; set; }

        [DisplayName("Belongs to")]
        [Required(ErrorMessage = "Please select belongs to")]
        public short DORDRID { get; set; }

        [DisplayName("Status")]
        public short DISPSTATUS { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime PRCSDATE { get; set; }

        // Account Head ID - default 0
        public int ACHEADID { get; set; } = 0;

        // Last modified user id (username)
        [MaxLength(100)]
        public string LMUSRID { get; set; }

        // Created user id (username)
        [MaxLength(100)]
        public string CUSRID { get; set; }
    }
}
