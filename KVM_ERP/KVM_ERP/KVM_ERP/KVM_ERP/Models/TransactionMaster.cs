using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KVM_ERP.Models
{
    [Table("TRANSACTIONMASTER")]
    public class TransactionMaster
    {
        [Key]
        [Column("TRANMID")]
        public int TRANMID { get; set; }

        [Required]
        [Column("TRANDATE")]
        public DateTime TRANDATE { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("CATENAME")]
        public string CATENAME { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("CATECODE")]
        public string CATECODE { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("VECHNO")]
        public string VECHNO { get; set; }

        [Required]
        [Column("CLIENTWGHT")]
        public decimal CLIENTWGHT { get; set; }

        [Required]
        [Column("DISPSTATUS")]
        public short DISPSTATUS { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("CUSRID")]
        public string CUSRID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("LMUSRID")]
        public string LMUSRID { get; set; }

        [Required]
        [Column("PRCSDATE")]
        public DateTime PRCSDATE { get; set; }

        // New columns
        [Required]
        [Column("COMPYID")]
        public int COMPYID { get; set; }

        [Required]
        [Column("REGSTRID")]
        public int REGSTRID { get; set; }

        [Required]
        [Column("TRANNO")]
        public int TRANNO { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("TRANDNO")]
        public string TRANDNO { get; set; }

        [Required]
        [Column("TRANREFID")]
        public int TRANREFID { get; set; }

        [Required]
        [Column("TRANNAMT")]
        public decimal TRANNAMT { get; set; }

        [Required]
        [Column("TRANPACKAMT")]
        public decimal TRANPACKAMT { get; set; }

        [Required]
        [Column("TRANGAMT")]
        public decimal TRANGAMT { get; set; }

        [MaxLength(250)]
        [Column("TRANAMTWRDS")]
        public string TRANAMTWRDS { get; set; }

        [MaxLength(100)]
        [Column("TRANREFNO")]
        public string TRANREFNO { get; set; }

        // GST Columns
        [Required]
        [Column("TRANCGSTAMT")]
        public decimal TRANCGSTAMT { get; set; }

        [Required]
        [Column("TRANSGSTAMT")]
        public decimal TRANSGSTAMT { get; set; }

        [Required]
        [Column("TRANIGSTAMT")]
        public decimal TRANIGSTAMT { get; set; }

        [Required]
        [Column("TRANCGSTEXPRN")]
        public decimal TRANCGSTEXPRN { get; set; }

        [Required]
        [Column("TRANSGSTEXPRN")]
        public decimal TRANSGSTEXPRN { get; set; }

        [Required]
        [Column("TRANIGSTEXPRN")]
        public decimal TRANIGSTEXPRN { get; set; }
    }
}
