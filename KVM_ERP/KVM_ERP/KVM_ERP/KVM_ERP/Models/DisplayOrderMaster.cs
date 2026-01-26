using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubMembership.Models
{
    [Table("DISPLAYORDERMASTER")]
    public class DisplayOrderMaster
    {
        [Key]
        public int DORDRID { get; set; }
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateDORDRDESC", "Common", AdditionalFields = "i_DORDRDESC", ErrorMessage = "This is already used.")]
        public string DORDRDESC { get; set; }
        public short DORDORDR { get; set; }
        public short DISPSTATUS { get; set; }
        public Nullable<DateTime> PRCSDATE { get; set; }
    }
}