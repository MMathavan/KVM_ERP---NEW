using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KVM_ERP.Models
{
    [Table("YIELDPRECENTMASTER")]
    public class YieldPercentMaster
    {
        [Key]
        [Column("YIPREID")]
        public int YIPREID { get; set; }

        [Required]
        [Column("MTRLGID")]
        public int MTRLGID { get; set; }

        [Required]
        [Column("MTRLID")]
        public int MTRLID { get; set; }

        [Required]
        [Column("PACKMID")]
        public int PACKMID { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("YIPRECOUNTS")]
        public string YIPRECOUNTS { get; set; }

        [Required]
        [Column("YIPREVALUE")]
        public decimal YIPREVALUE { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("CUSRID")]
        public string CUSRID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("LMUSRID")]
        public string LMUSRID { get; set; }

        [Required]
        [Column("DISPSTATUS")]
        public short DISPSTATUS { get; set; }

        [Required]
        [Column("PRCSDATE")]
        public DateTime PRCSDATE { get; set; }
    }
}
