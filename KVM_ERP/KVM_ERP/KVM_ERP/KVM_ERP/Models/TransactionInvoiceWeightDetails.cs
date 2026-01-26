using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KVM_ERP.Models
{
    [Table("TRANSACTION_INVOICE_WEIGHT_DETAILS")]
    public class TransactionInvoiceWeightDetails
    {
        [Key]
        [Column("TRANRID")]
        public int TRANRID { get; set; }

        [Column("TRANDID")]
        [Required]
        public int TRANDID { get; set; }

        [Column("PACKMID")]
        [Required]
        public int PACKMID { get; set; }

        [Column("PACKTMID")]
        [Required]
        public int PACKTMID { get; set; }

        [Column("SLABVALUE")]
        [Required]
        public decimal SLABVALUE { get; set; }

        [Column("PNDSVALUE")]
        [Required]
        public decimal PNDSVALUE { get; set; }

        [Column("TOTALPNDS")]
        [Required]
        public decimal TOTALPNDS { get; set; }

        [Column("PACKWGT")]
        [Required]
        public decimal PACKWGT { get; set; }

        [Column("TOTALWGHT")]
        [Required]
        public decimal TOTALWGHT { get; set; }

        [Column("ONEDOLLAR")]
        [Required]
        public decimal ONEDOLLAR { get; set; }

        [Column("TOTALDOLVAL")]
        [Required]
        public decimal TOTALDOLVAL { get; set; }

        // Packing discount fields
        [Column("TRANIDISCEXPRN")]
        [Required]
        public decimal TRANIDISCEXPRN { get; set; }

        [Column("WASTEPWGT")]
        [Required]
        public decimal WASTEPWGT { get; set; }

        [Column("TRANIDISCAMT")]
        [Required]
        public decimal TRANIDISCAMT { get; set; }

        [Column("TOTALDOLDISCAMT")]
        [Required]
        public decimal TOTALDOLDISCAMT { get; set; }

        [Column("WEIGHTINKGS")]
        [Required]
        public decimal WEIGHTINKGS { get; set; }

        [Column("PERKGRATE")]
        [Required]
        public decimal PERKGRATE { get; set; }

        [Column("INCENTIVEPERCENT")]
        [Required]
        public decimal INCENTIVEPERCENT { get; set; }

        [Column("INCENTIVEVALUE")]
        [Required]
        public decimal INCENTIVEVALUE { get; set; }

        [Column("INCENTIVETOTALVALUE")]
        [Required]
        public decimal INCENTIVETOTALVALUE { get; set; }

        [Column("CUSRID")]
        [Required]
        [StringLength(100)]
        public string CUSRID { get; set; }

        [Column("LMUSRID")]
        [Required]
        [StringLength(100)]
        public string LMUSRID { get; set; }

        [Column("DISPSTATUS")]
        [Required]
        public short DISPSTATUS { get; set; }

        [Column("PRCSDATE")]
        [Required]
        public DateTime PRCSDATE { get; set; }

        // Navigation properties (optional)
        [ForeignKey("TRANDID")]
        public virtual TransactionDetail TransactionDetail { get; set; }

        [ForeignKey("PACKMID")]
        public virtual PackingMaster PackingMaster { get; set; }

        [ForeignKey("PACKTMID")]
        public virtual PackingTypeMaster PackingTypeMaster { get; set; }
    }
}
