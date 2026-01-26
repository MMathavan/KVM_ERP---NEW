using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KVM_ERP.Models
{
    [Table("RECEIVEDTYPEMASTER")]
    public class ReceivedTypeMaster
    {
        [Key]
        [Column("RCVDTID")]
        public int RCVDTID { get; set; }

        [MaxLength(15)]
        [Column("RCVDTCODE")]
        public string RCVDTCODE { get; set; }

        [MaxLength(100)]
        [Column("RCVDTDESC")]
        public string RCVDTDESC { get; set; }

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

        // BKN value to be applied for calculations (e.g., Head On, Head Less, Easy Peel)
        [Column("BKN")]
        public decimal? BKN { get; set; }
    }
}
