using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KVM_ERP.Models
{
    [Table("PRODUCTIONCOLOURMASTER")]
    public class ProductionColourMaster
    {
        [Key]
        [Column("PCLRID")]
        public int PCLRID { get; set; }

        [MaxLength(15)]
        [Column("PCLRCODE")]
        public string PCLRCODE { get; set; }

        [MaxLength(100)]
        [Column("PCLRDESC")]
        public string PCLRDESC { get; set; }

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
