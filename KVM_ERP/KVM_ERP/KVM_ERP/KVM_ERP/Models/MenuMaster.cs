using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubMembership.Models
{
    [Table("MENUMASTER")]
    public class MenuMaster
    {
        [Key]
        [Column("MENUID")]
        public int MenuId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("MENUDESC")]
        public string MenuDesc { get; set; }

        [Required]
        [StringLength(15)]
        [Column("MENUCODE")]
        public string MenuCode { get; set; }

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