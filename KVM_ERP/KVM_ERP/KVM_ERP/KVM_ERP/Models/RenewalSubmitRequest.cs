using System;
using System.ComponentModel.DataAnnotations;

namespace KVM_ERP.Models
{
    public class RenewalSubmitRequest
    {
        [Required]
        public int MemberID { get; set; }

        [Required]
        public int MemberTypeId { get; set; }

        public string UPI_ID { get; set; }
        public string RRN_NO { get; set; }
        public string Payment_Type { get; set; }
    }
}
