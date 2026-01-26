using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KVM_ERP.Models
{
    [Table("GRADEMASTER")]
    public class GradeMaster
    {
        [Key]
        [Column("GRADEID")]
        public int GRADEID { get; set; }

        [MaxLength(15)]
        [Column("GRADECODE")]
        public string GRADECODE { get; set; }

        [MaxLength(100)]
        [Column("GRADEDESC")]
        public string GRADEDESC { get; set; }

        [MaxLength(100)]
        [Column("CUSRID")]
        public string CUSRID { get; set; }

        [MaxLength(100)]
        [Column("LMUSRID")]
        public string LMUSRID { get; set; }

        [Column("DISPSTATUS")]
        public short DISPSTATUS { get; set; }

        [Column("PRCSDATE")]
        public DateTime? PRCSDATE { get; set; }
    }
}
