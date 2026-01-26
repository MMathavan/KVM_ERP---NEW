using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubMembership.Models
{
    [Table("RIMAGEMASTER")]
    public class RImageMaster
    {
        [Key]
        [Column("RIMGID")]
        public int RImgId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("RIMGDESC")]
        public string RImgDesc { get; set; }

        [Required]
        [StringLength(15)]
        [Column("RIMGCODE")]
        public string RImgCode { get; set; }

        [Required]
        [StringLength(100)]
        [Column("CUSRID")]
        public string CreatedUserId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("LMUSRID")]
        public string LastModifiedUserId { get; set; }

        [Column("DISPSTATUS")]
        public short DisplayStatus { get; set; }

        [Column("PRCSDATE")]
        public DateTime ProcessDate { get; set; }
    }
}