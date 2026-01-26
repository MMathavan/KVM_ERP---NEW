using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("CURRENCYMASTER")]
    public class CurrencyMaster
    {
        [Key]
        public int CURNID { get; set; }

        [DisplayName("Currency Description")]
        [Required(ErrorMessage = "Please enter currency description")]
        [MaxLength(100)]
        [Remote("ValidateCURNDESC", "Common", AdditionalFields = "i_CURNDESC", ErrorMessage = "This currency description is already used.")]
        public string CURNDESC { get; set; }

        [DisplayName("Currency Code")]
        [Required(ErrorMessage = "Please enter currency code")]
        [MaxLength(10)]
        [Remote("ValidateCURNCODE", "Common", AdditionalFields = "i_CURNCODE", ErrorMessage = "This currency code is already used.")]
        public string CURNCODE { get; set; }

        [DisplayName("Currency Amount")]
        [Required(ErrorMessage = "Please enter currency amount")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Currency amount must be greater than 0")]
        public decimal CURNAMT { get; set; }

        // Created user id (username)
        public string CUSRID { get; set; }

        // Last modified user id (int)
        public int LMUSRID { get; set; }

        [DisplayName("Status")]
        public short DISPSTATUS { get; set; }

        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}
