using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KVM_ERP.Models
{
    [Table("TRANSACTIONDETAIL")]
    public class TransactionDetail
    {
        [Key]
        [Column("TRANDID")]
        public int TRANDID { get; set; }

        [Required]
        [Column("TRANMID")]
        public int TRANMID { get; set; }

        [Required]
        [Column("MTRLGID")]
        public int MTRLGID { get; set; }

        [Required]
        [Column("MTRLID")]
        public int MTRLID { get; set; }

        [Required]
        [Column("MTRLNBOX")]
        public int MTRLNBOX { get; set; }

        [Required]
        [Column("MTRLCOUNTS")]
        public int MTRLCOUNTS { get; set; }

        [Required]
        [Column("GRADEID")]
        public int GRADEID { get; set; }

        [Required]
        [Column("PCLRID")]
        public int PCLRID { get; set; }

        [Required]
        [Column("RCVDTID")]
        public int RCVDTID { get; set; }

        [Required]
        [Column("HSNID")]
        public int HSNID { get; set; }

        [Required]
        [Column("TRANAQTY")]
        public decimal TRANAQTY { get; set; }

        [Required]
        [Column("TRANDQTY")]
        public decimal TRANDQTY { get; set; }

        [Required]
        [Column("TRANEQTY")]
        public decimal TRANEQTY { get; set; }

        [Required]
        [Column("TRANDRATE")]
        public decimal TRANDRATE { get; set; }

        [Required]
        [Column("TRANDAMT")]
        public decimal TRANDAMT { get; set; }

        [Required]
        [Column("TRANDDISCEXPRN")]
        public decimal TRANDDISCEXPRN { get; set; }

        [Required]
        [Column("TRANDDISCAMT")]
        public decimal TRANDDISCAMT { get; set; }

        [Required]
        [Column("TRANDGAMT")]
        public decimal TRANDGAMT { get; set; }

        [Required]
        [Column("TRANDCGSTEXPRN")]
        public decimal TRANDCGSTEXPRN { get; set; }

        [Required]
        [Column("TRANDSGSTEXPRN")]
        public decimal TRANDSGSTEXPRN { get; set; }

        [Required]
        [Column("TRANDIGSTEXPRN")]
        public decimal TRANDIGSTEXPRN { get; set; }

        [Required]
        [Column("TRANDCGSTAMT")]
        public decimal TRANDCGSTAMT { get; set; }

        [Required]
        [Column("TRANDSGSTAMT")]
        public decimal TRANDSGSTAMT { get; set; }

        [Required]
        [Column("TRANDIGSTAMT")]
        public decimal TRANDIGSTAMT { get; set; }

        [Required]
        [Column("TRANDNAMT")]
        public decimal TRANDNAMT { get; set; }

        [Required]
        [Column("TRANDAID")]
        public int TRANDAID { get; set; }

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
