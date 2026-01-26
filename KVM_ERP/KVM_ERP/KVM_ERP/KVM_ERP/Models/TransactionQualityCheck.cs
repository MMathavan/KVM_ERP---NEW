using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KVM_ERP.Models
{
    [Table("TRANSACTION_QUALITY_CHECK")]
    public class TransactionQualityCheck
    {
        [Key]
        [Column("TRANQID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TRANQID { get; set; }

        [Column("TRANMID")]
        [Required(ErrorMessage = "Transaction Master ID is required")]
        public int TRANMID { get; set; }

        [Column("LABOID")]
        [Required(ErrorMessage = "Laboratory is required")]
        [Display(Name = "Lab Testing")]
        public int LABOID { get; set; }

        [Column("STATUS")]
        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public int STATUS { get; set; }

        [Column("REMARKS")]
        [Required(ErrorMessage = "Remarks is required")]
        [Display(Name = "Remarks")]
        [StringLength(int.MaxValue, ErrorMessage = "Remarks cannot exceed maximum length")]
        public string REMARKS { get; set; }

        [Column("DONEBY")]
        [Required(ErrorMessage = "Done By is required")]
        [Display(Name = "Done By")]
        [StringLength(100, ErrorMessage = "Done By cannot exceed 100 characters")]
        public string DONEBY { get; set; }

        [Column("VERIFIEDBY")]
        [Required(ErrorMessage = "Verified By is required")]
        [Display(Name = "Verified By")]
        [StringLength(100, ErrorMessage = "Verified By cannot exceed 100 characters")]
        public string VERIFIEDBY { get; set; }

        [Column("LOTDATE")]
        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Date")]
        [DataType(DataType.DateTime)]
        public DateTime LOTDATE { get; set; }

        [Column("CUSRID")]
        [StringLength(100)]
        public string CUSRID { get; set; }

        [Column("LMUSRID")]
        [StringLength(100)]
        public string LMUSRID { get; set; }

        [Column("DISPSTATUS")]
        public short? DISPSTATUS { get; set; }

        [Column("PRCSDATE")]
        [DataType(DataType.DateTime)]
        public DateTime? PRCSDATE { get; set; }

        // Navigation properties
        [ForeignKey("TRANMID")]
        public virtual TransactionMaster TransactionMaster { get; set; }

        [ForeignKey("LABOID")]
        public virtual LaboratoryMaster LaboratoryMaster { get; set; }
    }
}
