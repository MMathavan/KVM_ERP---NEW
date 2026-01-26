using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("CUSTOMERMASTER")]
    public class CustomerMaster
    {
        [Key]
        public int CATEID { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Please enter customer name")]
        [Remote("ValidateCATENAME", "Common", AdditionalFields = "i_CATENAME", ErrorMessage = "This is already used.")]
        public string CATENAME { get; set; }

        [DisplayName("Display Name")]
        [Required(ErrorMessage = "Please enter display name")]
        public string CATEDNAME { get; set; }

        [DisplayName("Address 1")]
        public string CATEADDR1 { get; set; }

        [DisplayName("Address 2")]
        public string CATEADDR2 { get; set; }

        [DisplayName("Address 3")]
        public string CATEADDR3 { get; set; }

        [DisplayName("Address 4")]
        public string CATEADDR4 { get; set; }

        [DisplayName("Pincode")]
        public string CATEADDR5 { get; set; }

        [DisplayName("Location")]
        [Required(ErrorMessage = "Please select location")]
        public int LOCTID { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "Please select state")]
        public int STATEID { get; set; }

        [DisplayName("Landline 1")]
        [MaxLength(25)]
        public string CATEPHN1 { get; set; }

        [DisplayName("Landline 2")]
        [MaxLength(25)]
        public string CATEPHN2 { get; set; }

        [DisplayName("Mobile 1")]
        [MaxLength(25)]
        public string CATEPHN3 { get; set; }

        [DisplayName("Mobile 2")]
        [MaxLength(25)]
        public string CATEPHN4 { get; set; }

        [DisplayName("Contact Person Name")]
        [MaxLength(100)]
        public string CATEPNAME { get; set; }

        [DisplayName("Email")]
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string CATEMAIL { get; set; }

        [DisplayName("GST No")]
        [MaxLength(50)]
        public string CATE_GST_NO { get; set; }

        [DisplayName("PAN No")]
        [MaxLength(50)]
        public string CATE_PAN_NO { get; set; }

        [DisplayName("TAN No")]
        [MaxLength(50)]
        public string CATE_TAN_NO { get; set; }

        [DisplayName("Pesticide Licence No")]
        [MaxLength(50)]
        public string CATE_PEST_LIC_NO { get; set; }

        [DisplayName("Seed Licence No")]
        [MaxLength(50)]
        public string CATE_SEED_LIC_NO { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please enter customer code")]
        [MaxLength(50)]
        [Remote("ValidateCATECODE", "Common", AdditionalFields = "i_CATECODE", ErrorMessage = "This is already used.")]
        public string CATECODE { get; set; }

        [DisplayName("Credit Note")]
        public int CATE_CRDTPRD { get; set; }

        // Hidden fields with default values
        [DisplayName("Bank Name")]
        public string CATE_BANK_NAME { get; set; } // NULL

        [DisplayName("Branch Name")]
        public string CATE_BRNCH_NAME { get; set; } // NULL

        [DisplayName("IFSC Code")]
        public string CATE_IFSC_CODE { get; set; } // NULL

        [DisplayName("Account No")]
        public string CATE_ACNO { get; set; } // NULL

        [DisplayName("IBAN Code")]
        public string CATE_IBAN_CODE { get; set; } // NULL

        [DisplayName("SWIFT Code")]
        public string CATE_SWIFT_CODE { get; set; } // NULL

        [DisplayName("Designation Description")]
        public string CATE_DSGNDESC { get; set; } // NULL
        public int CATENO { get; set; } = 1; // Default value 1
        public short CATESTYPE { get; set; } = 0; // Default value 0

        // Created user id (username)
        public string CUSRID { get; set; }

        // Last modified user id (int)
        public int LMUSRID { get; set; }

        [DisplayName("Status")]
        public short DISPSTATUS { get; set; }

        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }

        public int CATETID { get; set; } = 2; // Default value 2

        public int ACHEADID { get; set; } = 1; // Default value 1

        // Navigation properties (not mapped)
        [NotMapped]
        public string LocationName { get; set; }

        [NotMapped]
        public string StateName { get; set; }
    }
}
