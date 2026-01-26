using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("PURCHASEINVOICESTATUS")]
    public class PurchaseInvoiceStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PUINSTID { get; set; }

        [DisplayName("Status Description")]
        [Required(ErrorMessage = "Please enter status description")]
        [MaxLength(50)]
        public string PUINSTDESC { get; set; }

        [DisplayName("Status Code")]
        [MaxLength(15)]
        public string PUINSTCODE { get; set; }

        // Created user id (username)
        [MaxLength(100)]
        public string CUSRID { get; set; }

        // Last modified user id (username)
        [MaxLength(100)]
        public string LMUSRID { get; set; }

        [DisplayName("Status")]
        public short? DISPSTATUS { get; set; } = 0;

        [DisplayName("Process Date")]
        [DataType(DataType.DateTime)]
        public DateTime? PRCSDATE { get; set; }
    }
}
