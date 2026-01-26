using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KVM_ERP.Models
{
    [Table("TRANSACTIONMASTERFACTOR")]
    public class TransactionMasterFactor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("TRANMFID")]
        public int TRANMFID { get; set; }

        [Column("TRANMID")]
        public int TRANMID { get; set; }

        [Column("CFID")]
        public int CFID { get; set; }

        [Column("DEDEXPRN")]
        public decimal DEDEXPRN { get; set; }

        [MaxLength(50)]
        [Column("DEDMODE")]
        public string DEDMODE { get; set; }

        [Column("DEDTYPE")]
        public int DEDTYPE { get; set; }

        [Column("DEDORDR")]
        public int DEDORDR { get; set; }

        [Column("CFOPTN")]
        public short CFOPTN { get; set; }

        [Column("DORDRID")]
        public short DORDRID { get; set; }

        [Column("DEDVALUE")]
        public decimal DEDVALUE { get; set; }

        [MaxLength(100)]
        [Column("TRANCFDESC")]
        public string TRANCFDESC { get; set; }

        [Required]
        [Column("CFHSNID")]
        public int CFHSNID { get; set; }

        [Required]
        [Column("TRANCFCGSTEXPRN")]
        public decimal TRANCFCGSTEXPRN { get; set; }

        [Required]
        [Column("TRANCFSGSTEXPRN")]
        public decimal TRANCFSGSTEXPRN { get; set; }

        [Required]
        [Column("TRANCFIGSTEXPRN")]
        public decimal TRANCFIGSTEXPRN { get; set; }

        [Required]
        [Column("TRANCFCGSTAMT")]
        public decimal TRANCFCGSTAMT { get; set; }

        [Required]
        [Column("TRANCFSGSTAMT")]
        public decimal TRANCFSGSTAMT { get; set; }

        [Required]
        [Column("TRANCFIGSTAMT")]
        public decimal TRANCFIGSTAMT { get; set; }

        // Navigation property
        [ForeignKey("TRANMID")]
        public virtual TransactionMaster TransactionMaster { get; set; }
    }
}
