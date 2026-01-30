using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KVM_ERP.Models
{
    [Table("TRANSACTION_PRODUCT_CALCULATION")]
    public class TransactionProductCalculation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("TRANPID")]
        public int TRANPID { get; set; }

        [Required]
        [Column("TRANMID")]
        public int TRANMID { get; set; }

        [Required]
        [Column("TRANDID")]
        public int TRANDID { get; set; }

        [Required]
        [Column("PACKMID")]
        public int PACKMID { get; set; }

        [Column("PACKTMID")]
        public int PACKTMID { get; set; } = 0;

        [NotMapped]
        public decimal PCK1 { get; set; } = 0;

        [NotMapped]
        public decimal PCK2 { get; set; } = 0;

        [NotMapped]
        public decimal PCK3 { get; set; } = 0;

        [NotMapped]
        public decimal PCK4 { get; set; } = 0;

        [NotMapped]
        public decimal PCK5 { get; set; } = 0;

        [NotMapped]
        public decimal PCK6 { get; set; } = 0;

        [NotMapped]
        public decimal PCK7 { get; set; } = 0;

        [NotMapped]
        public decimal PCK8 { get; set; } = 0;

        [NotMapped]
        public decimal PCK9 { get; set; } = 0;

        [NotMapped]
        public decimal PCK10 { get; set; } = 0;

        [NotMapped]
        public decimal PCK11 { get; set; } = 0;

        [NotMapped]
        public decimal PCK12 { get; set; } = 0;

        [NotMapped]
        public decimal PCK13 { get; set; } = 0;

        [NotMapped]
        public decimal PCK14 { get; set; } = 0;

        [NotMapped]
        public decimal PCK15 { get; set; } = 0;

        [NotMapped]
        public decimal PCK16 { get; set; } = 0;

        [NotMapped]
        public decimal PCK17 { get; set; } = 0;

        [Column("TOPCK")]
        public decimal TOPCK { get; set; } = 0;

        [Column("PCKLVALUE")]
        public decimal PCKLVALUE { get; set; } = 0;

        [Column("AVGPCKVALUE")]
        public decimal AVGPCKVALUE { get; set; } = 0;

        [Column("PNDSVALUE")]
        public decimal PNDSVALUE { get; set; } = 0;

        [Column("TOTALPNDS")]
        public decimal TOTALPNDS { get; set; } = 0;

        [Column("YELDPERCENT")]
        public decimal YELDPERCENT { get; set; } = 0;

        [Column("TOTALYELDCOUNTS")]
        public decimal TOTALYELDCOUNTS { get; set; } = 0;

        [Column("KGWGT")]
        public decimal KGWGT { get; set; } = 0;

        [Column("PCKKGWGT")]
        public decimal PCKKGWGT { get; set; } = 0;

        [Column("PCKBOX")]
        public int PCKBOX { get; set; } = 0;

        [Column("WASTEWGT")]
        public decimal WASTEWGT { get; set; } = 0;

        [Column("WASTEPWGT")]
        public decimal WASTEPWGT { get; set; } = 0;

        [Column("FACTORYWGT")]
        public decimal FACTORYWGT { get; set; } = 0;

        [Column("DISPSTATUS")]
        public short DISPSTATUS { get; set; } = 0;

        [MaxLength(100)]
        [Column("CUSRID")]
        public string CUSRID { get; set; }

        [MaxLength(100)]
        [Column("LMUSRID")]
        public string LMUSRID { get; set; }

        [Column("PRCSDATE")]
        public DateTime? PRCSDATE { get; set; }

        [Column("PRODDATE")]
        public DateTime? PRODDATE { get; set; }

        [Column("CALCULATIONMODE")]
        public int CALCULATIONMODE { get; set; } = 0;

        [Column("FACAVGWGT")]
        public decimal FACAVGWGT { get; set; } = 0;

        [Column("FACAVGCOUNT")]
        public decimal FACAVGCOUNT { get; set; } = 0;

        [Column("BKN")]
        public decimal BKN { get; set; } = 0;

        [Column("OTHERS")]
        public decimal OTHERS { get; set; } = 0;

        [Column("GRADEID")]
        public int GRADEID { get; set; } = 0;

        [Column("PCLRID")]
        public int PCLRID { get; set; } = 0;

        [Column("RCVDTID")]
        public int RCVDTID { get; set; } = 0;

        [Column("ATRANDID")]
        public int ATRANDID { get; set; } = 0;

        [Column("SLABVALUE")]
        public decimal SLABVALUE { get; set; } = 0;
    }
}
